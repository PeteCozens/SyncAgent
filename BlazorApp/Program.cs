using BlazorApp.Components;
using ApplicationLogic.Extensions;
using Common;
using Common.Extensions;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Serilog;
using Infrastructure.Services.Identity;
using BlazorApp.Services;
using BlazorApp.Models;

namespace BlazorApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
            builder.Configuration.AddJsonFile("appsettings.json", optional: true);
            if (System.Diagnostics.Debugger.IsAttached || args.Contains("--applicationName"))  // Debugger is attached or we're running from the Package Manager Console
            {
                Console.WriteLine("Adding development configuration");
                builder.Configuration.AddJsonFile("appsettings.development.json", optional: true);
            }

            ConfigureLogging(builder);

            // Add Services to the container.

            AddAuthServices(builder);
            AddPresentationLayerServices(builder.Services);
            builder.Services.RegisterApplicationLogicServices(builder.Configuration);
            builder.Services.RegisterInfrastructureServices(builder.Configuration);

            // Build the Application 

            var app = builder.Build();

            ServiceFactory.Initialise(app.Services);
            var log = ServiceFactory.GetRequiredService<ILogger<Program>>();

            log.Here().LogDebug("Migrating the database");
            ServiceFactory.GetRequiredService<AppDbContext>().Migrate();

            // Configure the HTTP request pipeline.

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            log.Here().LogInformation("Startup Complete");

            app.Run();
        }

        private static void ConfigureLogging(WebApplicationBuilder builder)
        {
            // Configure Logging. See https://kenlea.blog/net-7-console-app-with-serilog-step-by-step-instructions

            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration
                    .Enrich.FromLogContext()
                    .ReadFrom.Configuration(context.Configuration);
            });

            // Logging config. See https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/configuration?view=aspnetcore-7.0

            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
        }

        private static void AddAuthServices(WebApplicationBuilder builder)
        {
            // Windows Authentication

            const string authScheme = Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme; // Windows Auth
            builder.Services.AddAuthentication(authScheme);

            // Authorization Policy, defined in the config file, using @inject IAuthorizationService AuthorizationService

            var authPolicies = builder.Configuration.GetSection("AuthPolicy").Get<AuthGroups[]>()
                ?? throw new Exception("AuthPolicy has not been set in the appsettings");

            builder.Services.AddSingleton(authPolicies);
            builder.Services.AddAuthorization(options =>
            {
                foreach (var authPolicy in authPolicies)
                {
                    options.AddPolicy(authPolicy.Key, policy =>
                    {
                        policy.RequireAssertion(context =>
                        {
                            foreach (var group in authPolicy.Groups)
                                if (context.User.IsInRole(group))
                                    return true;
                            return false;
                        });
                    });
                }
            });

            // Access to the HttpContext, via Dependency Injection, using: @inject IHttpContextAccessor HttpContextAccessor

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<IAuthenticatedIdentity, WebIdentityService>();
        }

        private static void AddPresentationLayerServices(IServiceCollection services)
        {
            services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate(); // Windows Authentication
            services.AddRazorComponents().AddInteractiveServerComponents();
        }
    }
}

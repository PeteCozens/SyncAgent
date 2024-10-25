using Infrastructure.Services.Identity;

namespace BlazorApp.Services
{
    public class WebIdentityService : IAuthenticatedIdentity
    {
        public WebIdentityService(IHttpContextAccessor accessor)
        {
            var identity = accessor.HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrEmpty(identity))
                throw new Exception("Unable to determine the username");

            if (identity.Contains('@'))
            {
                var elements = identity.Split('@', 2);
                UserName = elements[0];
                Domain = elements[1];
            }
            else
            {
                var elements = identity.Split('\\', 2);
                Domain = elements[0];
                UserName = elements[1];
            }
        }

        public string UserName { get; }

        public string Domain { get; }
    }
}

<!--
Markdown Guide: https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax
-->

# .Net Core Solution Template

This solution provides a template for multiple types of .Net Core applications - all you need to do is delete the projects that aren't relevant to what you're doing. 

The projects that make up the solution are as follows:

## ApplicationLogic

<sup>Required for all application types</sup>

This class library contains all of the business logic for your application. It does not know where its data comes from, nor where it goes - all that is handled by the relevant services in the Infrastructure project. The Application Logic layer only deals with the service interfaces.

## BlazorApp

<sup>Only required for Intranet/Internet applications</sup>

This project contains the Web UI and should be set as the startup project for any Intranet or Internet applications. It makes use of the publicly available  Radzen Blazor Components library (https://blazor.radzen.com/) for many of its user-interface elements.

## Common

<sup>Required for all application types</sup>

This class library contains elements that are shared between all of the other projects in the solution, such as:
- Attributes
- Extenders
- Interfaces
- Data Models
- Enums
- Static Utility Classes

None of the classes in this library implement any form of business logic. Classes only contain generic functionality that can be of use across all other projects in the solution.

## ConsoleApp

<sup>Only required for Console Applications</sup>

This Console App is designed to act as the basis for an app that can be executed autonomously from the Windows Task Scheduler. 

This project template provides:
- Dependency Injection
- Configuration in .JSON files
- Command Line argument parsing
- Logging to Console & Rolling Files
- Entity Framework (Code First)
- SMTP Mail Sending Service
- Unit Tests

## Infrastructure

<sup>Required for all application types</sup>

This class library contains all of the services that interact with external elements, such as data sources, Azure services, etc.. It does not contain any business / application logic - just the code to interface with external objects.

Service classes should all implement the **IApplicationLogic** interface. Those that do so will automatically be registered in the application's Service Provider.

## UnitTests

<sup>Optional, but recommended for all application types</sup>

This class library contains unit tests for all of the other projects in the solution.

Code coverage is provided by the [FineCodeCoverage extension](https://marketplace.visualstudio.com/items?itemName=FortuneNgwenya.FineCodeCoverage), which you will need to have installed in Visual Studio. 


# Starting your new Solution

Once you have taken a copy of this template to begin your new solution, you will need to do a few things before you begin:

1. Remove any projects that are not requrired (including their associated folders in the UnitTest project).
2. Set your startup project and create its secrets.json and secrets.development.json files. Look in the appsettings files to see which sections must be copied into the new files and configured accordingly.
3. Update all NuGet packages at the solution level.
4. Build the app to check that everything's working.
5. If you are using Entity Framework Code First in an existing database, change the default schema in **Infrastructure.Data.AppDbContext** if you need to create a dedicated database schema in which you can host your objects, including the migration history.
6. In the **Infrastructure.Models** folder, create model classes for EF Code First. Apply the ```[Table("Foo", Schema = AppDbContext.Schema)]``` attribute to each class change the table name in the database or use a schema other than dbo (or the default specified in the **AppDbContext** class).
7. For model classes that will be used with Dapper (eg: for querying existing tables that are not mananged by EF), be sure to implement **IDapperModel** and use the ```[Column("DbColumnName")]``` attribute to map the database column names to something more friendly in your code. The Column attribute will be ignored on any model classes that do not implement **IDapperModel**
8. Add your Data I/O methods to the relevant classes in the **Infrastructure.Services.Repositories** folder, which will use EF and/or Dapper internally to talk to the DB. All Data I/O within the application should be via the repository. Nothing else should use the AppDbContext directly.
9. Call ```Update-Database 0``` in the package manager window to undo all updates to the database.
10. For Console Applications, create a class containing your busines logic in the **ApplicationLogic.Services** folder, then update ```ConsoleApp.Program.DoWork()``` to instantiate your service class and call its worker method.

# Logging

The templates are designed such that logging objects should be instantiated using Dependency Injection, usually in your class constructors, as follows:

```
public class MyService(ILogger<MyService> log, IRepository repo) : IApplicationLogic
```

Whenever you call a logging method, always use the ```.Here()``` extension method to ensure that the location that the logging method is called from is included in the logs, e.g.: 

```
log.Here().LogDebug("This is my log message");
```

# Entity Framework

The template allows the use of both Entity Framework and Dapper. Generally, it is advised to use Entity Framework in your Repository class(es), although Dapper may prove more suitable for querying third-party databases. 

Repositories should be placed in the **Infrastructure\Services\Repositories** folder. All data models should be placed in **Common\Models**.

DbContext classes should be placed in **Infrastructure\Data**.

Here are some useful Powershell commands:

* Undo all database migrations: ```Update-Database 0```
* Add a new migration: ```Add-Migration [unique name]```
* Update the database to the latest migration: ```Update-Database```
* Update the database to a specific migration: ```Update-Database [migration name]```
* Remove the latest migration from the project: ```Remove-Migration```
* Scaffold (create Model classes for) an existing database: [Scaffold-DbContext](Microsoft.EntityFrameworkCore.SqlServer)

# Adding Application Services

Services that contain Application (Business) Logic should all be placed in the **ApplicationLogic\Services** folder and must implement the **IApplicationLogic** interface. By doing this, the services are automatically registered in the application's ServiceProvider for Dependency Injection. 

Note that Application Logic Services must **ONLY** implement application logic - they should not interface directly with the file system, database, SMTP server or any external resources. All that should be done using services in the Infrastructure project.

## Service Configuration

If the service class requires any configuration from the appsettings JSON files, this is easily achieved as follows:

1. Create an section in the appsettings file with the same property name as the service class. So, for example, if our service class is called **MyAppService**, the configuration might look like this:
```
{
	"MyAppService" : {
		"Foo": "Hello",
		"Bar": "World"
	}
}
```
2. Create a model class that reflects this configuration in the same .cs file as the service. The class must have the same name as the service, with **Config** at the end. e.g.:
```
public class MyAppServiceConfig
{
	public string Foo { get; set; } = string.Empty;
	public string Bar { get; set; } = string.Empty;
}
```
3. In the service class definition, pass the config class to the constructor as follows:
```
public class MyAppService(MyAppServiceConfig config)
```

This is all that is required. When the service class (identified by its implementation of **IApplicationLogic**) is registered, the system looks for an associated config class and registers a singleton in the ServiceProvider, populated from the related section in the config file. 

# Adding Infrastructure Services

Infrastructure Services know nothing of the application's business logic. All that they do is interface with external resources to PUSH or PULL data. These classes should be placed in sub-folders of the **Services** folder of the **Infrastructure** project.

Your project may contain multiple services for a given action - the choice of which service to use can be configured on an environment-by-environment basis in the **InfrastructureServices** section of the application's configuration. On startup, the application will enumerate the interfaces listed in this section and register the specified service class for each. 

An example of this may be found in the **Infrastructure\Services\File** folder, where there are two services that implement IFileService: FileSystemService and AzureBlobService. In the config we can specify which of these service classes will be used by the ApplicationLogic services when reading and writing files, so the local file system can be used for development, and Azure BLOBs for production without any code changes.
```
  "InfrastructureServices": {
    ...
    "IFileService": "FileSystemService" // or "AzureBlobService"
  },
```

## Service Configuration

If your Infrastructuer Service classes require configuration, you'll need to follow the same process as you would for [Application Logic Services](#service-configuration). 

# TODO...
* ~~Convert Console App's Main() function to async~~
* ~~Check that all scaffolded data models are generated in the Common.Models folder~~
* ~~Secrets files in ConsoleApp Template~~
* ~~Infrastructure - IFileService interface to read and write (text & binary) to files. LocalFileService & AzureBlobService both implement~~
* Re-work IFileService implementations so that folders are keyed from appsettings. File contains path. Azure contains Container, Account Name & Account Key 
* Azure Function example (new project?)
* Unit tests for the Common Library
* Unit tests for the Infrastructure Library
* Unit tests for the ApplicationLogic Library
* Blazor project
	* Secrets files in BlazorApp Template
	* Blazor.Radzen test/demo page for components
		* CheckBoxList component. https://blazor.radzen.com/checkboxlist
		* Autocomplete. https://blazor.radzen.com/autocomplete
		* Modal dialogs. https://blazor.radzen.com/dialog
	* Select2 (single & multi) component
	* Form Validation
	* JavaScript & back end for time zone
	* Datagrid page & loading panel (Blazor / Radzen ?)
	* Windows Authentication for Intranet Apps
	* Email & Password Authentication for Internet Apps (use Radzen for UI components)
	* Third-party Authentication (Google, Facebook, etc.) for Internet Apps
	* Windows Authorization (policy attributes, checks in code)
	* Claims cache for policies
	* Impersonation for Auth users
	* Redirect to 403/404/500
	* Panel for not authorized
	* Anonymous pages (error, etc)
	* WebApplicationFactory for unit testing API & Blazor?
	* Time Zones to display UTC date times in local
	* Text generation (for emails, etc.) using Razor
	* JavaScript integration for button (call JavaScript to validate inputs, then disable button before POST, and re-enable after)
	* JavaScript spinner object in global with show & hide methods for Ajax calls, POSTing, etc.
	* HTML to PDF https://code.soundaranbu.com/convert-html-to-pdf-in-asp-net-core/#HTML_To_PDF
* Logging ERROR and above to email
* Sample code for SMS provider (TextAnywhere API?)
* Logging FATAL to SMS
* Windows Service example (new project?)
* Holiday Data. Use API at https://api.11holidays.com/. Add example code to Console, Infrastructure, etc. to download holidays and save updates in the DB. https://api.11holidays.com/v1/countries?offset=0&limit=1000 for country list. https://api.11holidays.com/v1/holidays?country=GB&offset=0&limit=1000&year=2024 for holidays.
* Infrastructure - EPPlus extension methods
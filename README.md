<!--
Markdown Guide: https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax
-->

# SyncAgent

This solution provides data synchronisation between a Client's Core Business Systems and a system hosted in the cloud. 

The solution structure is as follows:


## ApplicationLogic

This class library contains all of the business logic for the SyncAgent. It does not know where its data comes from, nor where it goes - all that is handled by the relevant services in the Infrastructure project. The Application Logic layer only deals with the service interfaces.

## Common

This class library contains elements that are shared between all of the other projects in the solution, such as:
- Attributes
- Extenders
- Interfaces
- Data Models
- Enums
- Static Utility Classes

None of the classes in this library implement any form of business logic. Classes only contain generic functionality that can be of use across all other projects in the solution.

## ConsoleApp

This Console App is the entry point for the SyncAgent and is intended to be executed autonomously from the Windows Task Scheduler. It contains no application logic - it merely handles the basic setup of the environment in which the application runs, and then hands over to a service in the Infrastructure project to do the actual work. This way, when we look to deliver this functionality as an Azure funtion, we only need create a basic Azure Function entry point project that calls the same services.

This project template provides:
- Dependency Injection
- Configuration in .JSON files
- Command Line argument parsing
- Logging to Console & Rolling Files
- Entity Framework (Code First)
- SMTP Mail Sending Service
- Unit Tests

## Infrastructure

This class library contains all of the services that interact with external elements, such as data sources, Azure services, etc.. It does not contain any business / application logic - just the code to interface with external objects.

Service classes should all implement the **IApplicationLogic** interface. Those that do so will automatically be registered in the application's Service Provider.

## UnitTests

This class library contains unit tests for all of the other projects in the solution.

Code coverage is provided by the [FineCodeCoverage extension](https://marketplace.visualstudio.com/items?itemName=FortuneNgwenya.FineCodeCoverage), which you will need to have installed in Visual Studio.
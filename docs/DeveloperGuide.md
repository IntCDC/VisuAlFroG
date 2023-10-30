# VisuAlFroG Developer Guide

<!-- TOC -->

## Contents

- [Code Formatting](#code-formatting)
- [Setup Entity Framework](#setup-entity-framework)
- [WebAPI](#webapi)
- [References](#references)

<!-- /TOC -->
-----


<!-- ###################################################################### -->
## Code Formatting

Suggestions on how to format the code are stored in `.editorconfig` and are applied via [dotnet-format](https://github.com/dotnet/format). 
See messages in *Error List* after building *VisuAlFroG*.


<!-- ###################################################################### -->
## Setup Entity Framework

- Install `EntityFramework` via NuGet Package Manager for the respective project.
  The actual startup project also needs `EntityFramework` to be installed.
- Open file `App.config` and add the following content below the `<configuration>` tag *(-> replace 'myContext' with appropriate context name)*
    
```XML
	<connectionStrings>
		<add name="myContext" connectionString="Data Source=(localdb)\MSSQLLocalDB; Integrated Security=True; MultipleActiveResultSets=True; AttachDbFilename=|DataDirectory|database.mdf" providerName="System.Data.SqlClient" />
	</connectionStrings>
```
- Open *Tools* / *NuGet Package Manager* / **Package Manager Console** (PMC)
  - Select 'EntityFrameworkDatabase' in the drop down menu on the top of Visual Studio as well as in the *Default Project* drop down menu of the Package Manager Console
  - In the Package Manager Console enter and confirm:
    *PM>* `Enable-Migrations`
  - Then, open the file `.../Migrations/Configuration.cs`, and in the function `Configuration()`add:
```C#
        // Set data directory variable given in connection string of DatabaseContext
        AppDomain.CurrentDomain.SetData("DataDirectory", Artefacts.Path());
```
  - *PM>* `Add-Migration Initial`
  - *PM>* `Update-Database`


<!-- ###################################################################### -->
## WebAPI

The actual startup (currently *WPFApplication*) project needs `Microsoft.Owin.Host.HttpListener` to be installed.


<!-- ###################################################################### -->
## References

- [use-owin-to-self-host-web-api](https://learn.microsoft.com/en-us/aspnet/web-api/overview/hosting-aspnet-web-api/use-owin-to-self-host-web-api)
- [self-host-a-web-api](https://learn.microsoft.com/en-us/aspnet/web-api/overview/older-versions/self-host-a-web-api)
- [create-a-rest-api-with-attribute-routing](https://learn.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/create-a-rest-api-with-attribute-routing)

<!-- ###################################################################### -->

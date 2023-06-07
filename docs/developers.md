# Development Notes





## Setup Entity Framework

- Open file `App.config` and add the following content below the `<configuration>` tag *(-> replace 'myContext' with appropriate context name)*
    
```XML
	<connectionStrings>
		<add name="myContext" connectionString="Data Source=(localdb)\MSSQLLocalDB; Integrated Security=True; MultipleActiveResultSets=True; AttachDbFilename=|DataDirectory|myContext-Database.mdf" providerName="System.Data.SqlClient" />
	</connectionStrings>
```
- Open *Tools* / *NuGet Package Manager* / **Package Manager Console** (PMC)
  - Select 'EntityFrameworkDatabase' in the drop down menu on the top of Visual Studio as well as in the *Default Project* drop down menu of the Package Manager Console
  - In the Package Manager Console enter and confirm:
    *PM>* `Enable-Migrations`
  - Then, open the file `.../Migrations/Configuration.cs`, and in the function `Configuration()`add:
```C#
        var locPath = System.AppContext.BaseDirectory;
        var dataDirPath = System.IO.Path.GetDirectoryName(locPath);
        AppDomain.CurrentDomain.SetData("DataDirectory", dataDirPath);
```
  - *PM>* `Add-Migration Initial`
  - *PM>* `Update-Database`

## References

- [use-owin-to-self-host-web-api](https://learn.microsoft.com/en-us/aspnet/web-api/overview/hosting-aspnet-web-api/use-owin-to-self-host-web-api)
- [self-host-a-web-api](https://learn.microsoft.com/en-us/aspnet/web-api/overview/older-versions/self-host-a-web-api)
- [create-a-rest-api-with-attribute-routing](https://learn.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/create-a-rest-api-with-attribute-routing)




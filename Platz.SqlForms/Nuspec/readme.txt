Thank you for using Platz.SqlForms

Installation process.

-------------------------------------------------------------------------------------------------------------------------------------------------------------
1. Install Platz.SqlForms NuGet package to your Blazor Server App project

    Important: You should see folders [Platz.Example.Link] and [Platz.Config.Link] in your project with code samples, configs and scripts

2. Modify your project startup.cs file add [using Platz.SqlForms;] and add code to ConfigureServices method:
    services.AddPlatzSqlForms();

3. Modify _Imports.razor and add references:
    @using Platz.SqlForms

4. [Platz.Example.Link] contains examples of a form definition and a razor page that uses it

5. To run the example you will need to create Sql database and Entity Framework DB Context for the provided sample entities

6. Working example you can download from our GitHub page:
    https://github.com/ProCodersPtyLtd/MasterDetailsDataEntry/tree/main

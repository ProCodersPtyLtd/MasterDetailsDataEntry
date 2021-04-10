Thank you for using Platz.SqlForms

Installation process.

-------------------------------------------------------------------------------------------------------------------------------------------------------------
1. Install Platz.SqlForms NuGet package to your Blazor Server App project

    Important: You should see folders [Platz.Example.Link] and [Platz.Config.Link] in your project with code samples, configs and scripts

2. Modify your project startup.cs file add [using Platz.SqlForms;] and add code to ConfigureServices method:
    services.AddPlatzSqlForms();

3. Modify _Imports.razor and add references:
    @using Platz.SqlForms

4. Add the following references to jquery and bootstrap just before your styles to _Host.cshtml:

    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.5.3/dist/css/bootstrap.min.css" integrity="sha384-TX8t27EcRE3e/ihU7zmQxVncDAy5uIKz4rEkgIXeMed4M0jlfIDPvg6uqKI2xXr2" crossorigin="anonymous">
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js" integrity="sha384-DfXdz2htPH0lsSSs5nCTpuj/zy4C+OGpamoFVy38MVBnE+IbbVYUew+OrCXaRkfj" crossorigin="anonymous"></script>
    <script src="https://cdn.jsdelivr.net/npm/popper.js@1.16.1/dist/umd/popper.min.js" integrity="sha384-9/reFTGAW83EW2RDu2S0VKaIzap3H66lZH81PoYlFhbGU+6BZp6G7niu735Sk7lN" crossorigin="anonymous"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@4.5.3/dist/js/bootstrap.min.js" integrity="sha384-w1Q4orYjBQndcko6MimVbzY0tgp4pWB4lZ7lr30WKz0vr/aWKhXdBNmNb5D92v7s" crossorigin="anonymous"></script>


*. [Platz.Example.Link] contains examples of a form definition and a razor page that uses it

*. To run the example you will need to create Sql database and Entity Framework DB Context for the provided sample entities

*. Working example you can download from our GitHub page:
    https://github.com/ProCodersPtyLtd/MasterDetailsDataEntry/tree/main


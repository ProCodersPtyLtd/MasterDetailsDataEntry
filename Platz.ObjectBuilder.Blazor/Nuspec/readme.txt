Thank you for using Platz.ObjectBuilder

Installation process.

-------------------------------------------------------------------------------------------------------------------------------------------------------------
1. Install Platz.ObjectBuilder NuGet package to your Blazor Server App project

    Important: You should see folder [Platz.ObjectBuilder.Link] in your project 

2. Add the following using line to your Startup.cs file:

    using Platz.ObjectBuilder;

3. Add the following line to [ConfigureServices] in your Startup.cs file:

    services.AddPlatzObjectBuilder();

4. Place the following code to your razor page, and change AdventureWorksContext to your DB Context:

    @using Platz.ObjectBuilder.Blazor

    <QueryComponent DbContextType="typeof(AdventureWorksContext)" StoreDataPath="StoreData"
                    DataService="MyDataService" Namespace="Default" />

5. Create new folder named [StoreData] in your project

6. Add the following references to jquery and bootstrap jsut befor your styles:

    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.5.3/dist/css/bootstrap.min.css" integrity="sha384-TX8t27EcRE3e/ihU7zmQxVncDAy5uIKz4rEkgIXeMed4M0jlfIDPvg6uqKI2xXr2" crossorigin="anonymous">
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js" integrity="sha384-DfXdz2htPH0lsSSs5nCTpuj/zy4C+OGpamoFVy38MVBnE+IbbVYUew+OrCXaRkfj" crossorigin="anonymous"></script>
    <script src="https://cdn.jsdelivr.net/npm/popper.js@1.16.1/dist/umd/popper.min.js" integrity="sha384-9/reFTGAW83EW2RDu2S0VKaIzap3H66lZH81PoYlFhbGU+6BZp6G7niu735Sk7lN" crossorigin="anonymous"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@4.5.3/dist/js/bootstrap.min.js" integrity="sha384-w1Q4orYjBQndcko6MimVbzY0tgp4pWB4lZ7lr30WKz0vr/aWKhXdBNmNb5D92v7s" crossorigin="anonymous"></script>

7. Run the application and go to your page containing QueryComponent



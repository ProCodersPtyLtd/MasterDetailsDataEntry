# Platz.SqlForms

If this project helps you reduce time to develop, you can give me a cup of coffee :)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=Q7XEPGTBQFWNG)

Created by Pro Coders team.
Please visit our web site https://www.procoders.com.au

Framework for generating dynamic UI for master-details CRUD operations

CodeProject Blog:

https://www.codeproject.com/Articles/5291832/Microsoft-Blazor-Rapid-Development-with-SQL-Forms

Wiki:

https://github.com/ProCodersPtyLtd/MasterDetailsDataEntry/wiki

Programming reference:

https://github.com/ProCodersPtyLtd/MasterDetailsDataEntry/wiki/Platz.SqlForms-programming-reference

Please submit bugs or feature requests here: 

https://github.com/ProCodersPtyLtd/MasterDetailsDataEntry/issues

NuGet packages available:

https://www.nuget.org/packages/Platz.SqlForms

https://www.nuget.org/packages/Platz.ObjectBuilder

Demo run:
1. Download or clone repository https://github.com/euklad/BlogCode/tree/main/DemoSqlForms-story6
2. Open DemoSqlForms.sln
3. Set DemoSqlForms.App as Stratup Project and run

Another demo with inline edit:
1. Clone or download this repository https://github.com/ProCodersPtyLtd/MasterDetailsDataEntry
2. Open MasterDetailsDataEntry.sln
3. Goto https://docs.microsoft.com/en-us/ef/ef6/resources/school-database and create the School database 
4. Change connection string in MasterDetailsDataEntry.Demo.Database\School\SchoolContext.cs if needed
5. Set  SqlForms.Demo as StartUp Project
6. Run (F5)
7. Click on [Department Details] menu

<img src="https://github.com/ProCodersPtyLtd/MasterDetailsDataEntry/blob/main/MasterDetails3.gif">

This is another good database to try:
https://docs.microsoft.com/en-us/sql/samples/adventureworks-install-configure?view=sql-server-ver15&tabs=ssms

## Official roadmap 
### Release 0.2.0 - done
- Support data services that returns business objects that may differ from EF entities
- UI component to show a list of business objects and context menu
- UI form to entry/edit EF entities
- Platz.ObjectBuilder library to define data services using visual interface
- ObjectBuilder T4 template to generate data services C# code
### Release 0.3.0 - Mar'21 
- SqlForms custom rules to change field properties (hidden, required, read-only, etc.)
- SchemaDesigner prototype, that allows to design database entities and save the schema to json
- T4 template to geneate SchemaDesigner entities and data access layer
- Upgrade ObjectBuilder to support SchemaDesigner entities
### Release 0.4.0 - May'21
- UI SQL forms to entry/edit business objects
- ObjectBuilder definition of business object - entity mappings and CRUD operations
- ObjectBuilder T4 template to generate business object CRUD operations' C# code
- ObjectBuiler Select functions
- ObjectBuilder Group By queries
- ObjectBuilder Subqueries
### Release 0.5.0 - Jun'21
- Usability and error data recovery for builders and designers
- Bug fixes
### Release 1.0.0 - Aug'21
- Support documentation
- Tutorials
- Bug fixes

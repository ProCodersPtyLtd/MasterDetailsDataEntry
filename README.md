# Platz.SqlForms

Created by Pro Coders team.
Please visit our web site https://www.procoders.com.au

Framework for generating dynamic UI for master-details CRUD operations

Wiki:
https://github.com/ProCodersPtyLtd/MasterDetailsDataEntry/wiki

Programming reference:
https://github.com/ProCodersPtyLtd/MasterDetailsDataEntry/wiki/Platz.SqlForms-programming-reference

NuGet package available:
https://www.nuget.org/packages/Platz.SqlForms

Demo run (download or clone this repository):

1. Goto https://docs.microsoft.com/en-us/ef/ef6/resources/school-database and create the School database 
2. Change connection string in MasterDetailsDataEntry.Demo.Database\School\SchoolContext.cs if needed
3. Set  MasterDetailsDataEntry.Demo as StartUp Project
4. Run (F5)
5. Click on [Department Details] menu

<img src="https://github.com/ProCodersPtyLtd/MasterDetailsDataEntry/blob/main/MasterDetails3.gif">

This is another good database to try:
https://docs.microsoft.com/en-us/sql/samples/adventureworks-install-configure?view=sql-server-ver15&tabs=ssms

## Official roadmap 
### Release 0.2.0
- Support data services that returns business objects that may differ from EF entities
- UI component to show a list of business objects and context menu
- UI form to entry/edit EF entities
- Platz.ObjectBuilder library to define data services using visual interface
- ObjectBuilder T4 template to generate data services C# code
### Release 0.3.0
- UI form to entry/edit business objects
- ObjectBuilder definition of business object - entity mappings and CRUD operations
- ObjectBuilder T4 tempalte to generate business object CRUD operations' C# code

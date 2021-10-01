step 1
	Create data project	-	contains dbcontext class
		- add sql server nuget package to project
		- entity framework core tools package to project
	Create domain project - contains object classes
		- define objects
	Create UI console app

step 2 
	define dbsets in context class
	create onconfiguring routine for connection string

step 3
	add EnsureCreated method in Main function of UI project and execute project
		- this will create the database.  drop it afterwards, so update migration can be used to recreate it later

step 4
	check EF Core commands by typing in "get-help entityframework in PM console"

step 5 
	add initial migration
		- remember to set data project (the one with the dbcontext) to default project

		NOTE:	for production deployments script migrations are recommened
				type in "script-migration" in PM console to generate sql script for creating database, tables, etc.

	type in "update-database -verbose"
		- the plural names of the tables is driven by the dbset names in the context class
		- the database was dropped in step 2, so that the efmigration history can be shown in the table


step 6
	created battle class that has a many to many relationship to samaurai.
		- it should be noted that in previous versions of EF, a combined entity class (ie BattleSamurai) needed to be created with additional navigation code in the context.
		  that is no longer needed in EFCore 5 and the BattleSamurai table is automatically created with a migration

	add migration to account for many to many relationship

	execute update-database and BattleSamurai table was created


step 7
	created BattleSamurai class in code to add a field, DateJoined
	

step 8
	in context class add OnModelCreating routine that explicitly defines the many to many relationship
		- columns are renamed.  reference the 20210929203543_m2mpayload migration


step 9
	create horse object that has a one to one relationship with samurai.
		add migration
		update database


step 10
	add startdate and enddate to Battle class



step 11
	add SamuraiBattleStats migration
		- no model changes were made.  this is a blank one to work with creating functions and views
	in the up routine of the migration, add SQL code for creating function and view
	in the down migration, add the code for drops


step 12
	add migration for stored procedures
		- no model changes were made.  this is a blank one to work with
	in the up routine of the migration, add SQL code for creating sps
	in the down migration, add the code for drops




NOTE:  Smarter to set variable for results and then enumerate

 - BAD: lots of work for each result.  Connection stays open until last result is fetched

 foreach (var s in context.Samurais) {
	RunSomeValidator(s.Name);
	CallSomeService(s.Id);
	GetSomeMoreDataBasedOn(s.Id);
 }


 - GOOD: smarter to get results first

 var samurais = context.Samurais.ToList();
 foreach (var s in samurais) {
	RunSomeValidator(s.Name);
	CallSomeService(s.Id);
	GetSomeMoreDataBasedOn(s.Id);
 }



 NOTE ON WORKING WITH MANY TO MANY RELATIONSHIPS:
	Adding payload mapping to existing skip navigation does not break existing code  -- need to research this more


	


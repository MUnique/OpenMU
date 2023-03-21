# Migrations Cheatsheet

Here are my notes about how to (re-)generate entity framework core migrations.

## Adding a new migration

* First make sure that you have merged the most current code from master. Your
  migration shouldn't be merged in between existing migrations.

* Open Package Manager Console

* Select default project *MUnique.OpenMU.Persistence.EntityFramework*

* Set the startup project to *MUnique.OpenMU.Persistence.EntityFramework*

* Run this command (replace *[Name]* by the name of the migration):
  > Add-Migration *[Name]* -context EntityDataContext

## Replacing the initial migration

Replacing the initial migration is only allowed until a stable version of OpenMU
is released. After that, just new migrations should be added.

* Delete *00000000000000_Initial* and *EntityDataContextModelSnapshot*

* Do the same steps like adding a new migration, with "Initial" as *[Name]*

* Replace migration name in the designer file by changing the parameter of the
  Migration-Attribute (paste from clipboard)

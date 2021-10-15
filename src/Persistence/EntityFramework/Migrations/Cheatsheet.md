# Migrations Cheatsheet

Here are my notes about how to (re-)generate entity framework core migrations.

## Adding a new migration

* Open Package Manager Console

* Select default project *MUnique.OpenMU.Persistence.EntityFramework*

* Set the startup project to *MUnique.OpenMU.Persistence.EntityFramework*

* Run this command (replace *[Name]* by the name of the migration):
  > Add-Migration *[Name]* -context EntityDataContext

* Replace the timestamp in the file name by the next sequential number and copy
  the new name into your clipboard

* Replace migration name in the designer file by changing the parameter of the
  Migration-Attribute (paste from clipboard)

## Replacing the initial migration

Replacing the initial migration is only allowed until a stable version of OpenMU
is released. After that, just new migrations should be added.

* Delete *00000000000000_Initial* and *EntityDataContextModelSnapshot*

* Do the same steps like adding a new migration, with "Initial" as *[Name]*

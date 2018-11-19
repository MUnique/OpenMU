# Quick Start OpenMU

At the moment OpenMU isn't in a finished state and there is no easy installation package.
That means, you should be a developer and know what you're doing if you want to use it.


Requirements:
* Windows OS
  * It runs under Linux and MacOS, too. However, this guide describes it for windows.
* PostgreSQL installed
* Visual Studio 2017 installed
* [TypeScript SDK 3.0.1](https://www.microsoft.com/en-US/download/details.aspx?id=55258)
* [.NET Core SDK 2.1.300](https://www.microsoft.com/net/download/dotnet-core/2.1)
* This repository cloned
* Free TCP ports:
  * 1234 (admin panel)
  * 55901, 55902, 55903 (game servers)
  * 44405 (connect server)
  * 55980 (chat server)
* Knowledge or way to start a game client, connecting to the server (I wont provide that, but there is a ClientLauncher project) :)

If you have that, you'll need to do:
* Open the solution of OpenMU with Visual Studio
* Right click the solution and 'Restore NuGet Packages'
* Edit OpenMU\Persistence\EntityFramework\ConnectionSettings.xml, so that the connection strings are correct - however only the user/password of the first connection string needs to be correct. The server will try to create the other roles specified by the settings.
* Build the solution 
* Start MUnique.OpenMU.Startup
  * If required, it will create the database schemas, the required roles and gives permissions to this roles
  * If you update to a newer state of the master-branch, it could be possible that you have to delete the database again before starting. Currently, we are not providing patches for database updates.
  * You can reinstall the database by adding a '-reinit' parameter
  * Optional: you can add the parameter '-local' to bind the servers tcp listeners to an ip address of a local network interface.
  * Optional: you can add the parameter '-autostart' to save the next step.
* When the Admin Panel is initialized, go to http://localhost:1234/admin. Then you should see three gameservers,
the chat server and the connect server. Start the connect server and at least one gameserver.
If all goes well, you should be able to expand a gameserver and see the hosted game maps.
* Then you can connect to the server through the game client.
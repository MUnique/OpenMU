# Quick Start OpenMU

General requirements:
  * Free TCP ports:
    * 1234 (admin panel)
    * 55901, 55902, 55903 (game servers)
    * 44405 (connect server)
    * 55980 (chat server)
  * Knowledge or way to start a game client, connecting to the server (I wont provide that, but there is a ClientLauncher project) :)

This guide describes two ways of starting the server. Use Docker, if you just want to play around. If you want to develop or debug the server, choose the manual way.

## Docker

If you just want to play around with the server, you can find the newest docker image on the Docker Hub:
https://hub.docker.com/r/munique/openmu

This guide assumes you know how to use docker in general and have docker installed (e.g. by using Docker Desktop on Windows).

To pull and run the latest docker image, run this command:
> docker run --name openmu -d -p 1234:1234 -p 44405:44405 -p 55901:55901 -p 55902:55902 -p 55903:55903 -p 55980:55980 munique/openmu:latest -demo

The last argument is there to start the server in demo mode, without a database. To use a postgres database, we still have to do some extensions:
  * Currently, these connection strings are defined in the ConnectionSettings.xml file. So we need to make the connection strings configurable
    from the outside of a container, e.g. by environment variables or by storing in it in docker volumes. This probably requires changes in the code.   
  * Adding a docker-compose.yml with a container for the postgres database and set up a connection between the two containers.
  * Extension of this guide :-)

If you know how to do this, feel free to submit a pull request.

## Manually

Use this way, if you want to develop or debug for OpenMU.

Requirements:
* Windows OS
  * It runs under Linux and MacOS, too. However, this guide describes it for windows.
* PostgreSQL installed
* Visual Studio 2017 or 2019 installed
* [TypeScript SDK 3.0.1](https://www.microsoft.com/en-US/download/details.aspx?id=55258)
* [.NET Core SDK 2.2.108](https://www.microsoft.com/net/download/dotnet-core/2.2)
* This repository cloned

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
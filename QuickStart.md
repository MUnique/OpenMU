# Quick Start OpenMU

General requirements:

* Free TCP ports:
  * 80 (admin panel)
  * 55901, 55902, 55903 (game servers)
  * 44405 (connect server)
  * 55980 (chat server)

* A game client (check our Discord FAQs)
* Knowledge or way to start the game client, so that it connects to the server. Our Launcher will do that.

  * Launcher binaries: [MUnique.OpenMU.ClientLauncher v0.8.0.zip](https://github.com/MUnique/OpenMU/releases/download/v0.8.0/MUnique.OpenMU.ClientLauncher_0.8.0.zip)
    * It requires the [.NET 8 runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
  * If your server and client runs on your local host, use any IP of 127.x.x.x, except 127.0.0.1, because this one is blocked by the client. For example, you could use 127.127.127.127

This guide describes two ways of starting the server. Use Docker, if you just
want to play around. If you want to develop or debug the server, choose the
manual way.

## Docker

Please take a look at the deploy-folder of this project. There you'll find a more
detailed guide about how to set up this project.

### Demo Mode

If you just want to play around with the server, you can find the newest docker
all-in-one image on the Docker Hub: <https://hub.docker.com/r/munique/openmu>

To pull and run the latest docker image, run this command:
`docker run --name openmu -d -p 80:8080 -p 44405:44405 -p 55901:55901 -p 55902:55902 -p 55903:55903 -p 55980:55980 munique/openmu:latest -demo`

The last argument is there to start the server in demo mode, without a
database. To use a postgres database, you can use docker-compose.

### Environment Variables

It's possible to define additional environment variables to influence the
postgres database connection strings.

| Name | Description         |
|------|---------------------|
| DB_HOST | The hostname of the database. If the local configuration file is still configured to use 'localhost', the value of this variable replaces it |
| DB_ADMIN_USER | The user name of the postgres admin account. If the local configuration file is still configured to use 'postgres' for the user name of the admin (first entry in the ConnectionSettings.xml), the value of this variable replaces it. |
| DB_ADMIN_PW | The user name of the postgres admin account. If the local configuration file is still configured to use 'admin' for the user password of the admin (first entry in the ConnectionSettings.xml), the value of this variable replaces it. |

## Manually

Use this way, if you want to develop or debug for OpenMU.

Requirements:

* Windows OS, 10 or higher

  * It runs under Linux and MacOS, too. However, this guide describes it for
    windows.

  * PostgreSQL installed

  * Visual Studio 2022 (17.8+) installed, with workloads for ASP.NET Web development
    and .NET Desktop development. Please keep it up-to-date to prevent any issues.

  * [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)
    (it should be included in Visual Studio 17.8+)
    * `winget install Microsoft.DotNet.SDK.8`

  * [NodeJS 16+](https://nodejs.org) installed
    * `winget install OpenJS.NodeJS.LTS`

  * This repository cloned

If you have that, you'll need to do:

* Open the solution of OpenMU with Visual Studio

* Right click the solution and 'Restore NuGet Packages'

* Edit OpenMU\Persistence\EntityFramework\ConnectionSettings.xml, so that the
  connection strings are correct - however only the user/password of the first
  and second connection string need to be correct. The server will try to create
  the other roles specified by the settings.

* Build the solution

* Start MUnique.OpenMU.Startup

  * If required, it will create the database schemas, the required roles and
    gives permissions to this roles.

  * Optional: You can reinitialize the database by adding a ```-reinit``` parameter.

* When the Admin Panel is initialized, go to <http://localhost/>. Then you
  should see three gameservers, the chat server and the connect server. Start
  the connect server and at least one gameserver.

* If you update to a newer state of the master-branch, it could be possible
    that you have to update the database and configuration.
    You can find updates in the admin panel.

* Then you can connect to the server through the game client.

## Helpful (optional) steps

* __Auto Start__: If you don't want to start each server listener after starting
  the process, you can either activate "Auto Start"

  * in the admin panel at ```Configuration -> System```,

  * or with the start parameter ```-autostart```.

* __IP Resolving__: If you encounter disconnects after selecting a server, it's most
  likely a wrong setting for the IP resolver. You can change it easily over the
  admin panel at ```Configuration -> System``` as well.
  You may also change the setting by providing start parameters or environment
  parameters, however I just recommend this for experienced users. Look at this
  [Readme](src/Startup/Readme.md) for more information.

* __Changing the game version__: If you want to player another version than
  season 6, you may initialize the database with another game version.
  You can do this over the admin panel as well, at the ```Setup``` page.

## Test Accounts

To test some features of the server, test accounts are created automatically
when the database is initialized.

These are the user names:

* test0 - test9: General test accounts, level 1 to 90, in 10 level steps

Season 6 only:
* test300: General test account with level 300
* test400: General test account with level 400, master characters
* testgm: Test account of a game master
* testgm2: Test account of a game master with summoner and rage fighter characters
* testunlock: Test account without characters, but unlocked character classes
* quest1: Test account for the level 150 quests
* quest2: Test account for the level 220 quests
* quest3: Test account for the level 400 quests
* ancient: Test account with ancient item sets, level 330 characters
* socket: Test account with socket item sets, level 380 characters

The __passwords__ of these accounts are the __same as the user name__.

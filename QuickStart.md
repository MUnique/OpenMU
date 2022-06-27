# Quick Start OpenMU

General requirements:

* Free TCP ports:
  * 80 (admin panel)
  * 55901, 55902, 55903 (game servers)
  * 44405 (connect server)
  * 55980 (chat server)

* A game client (check our Discord FAQs)
* Knowledge or way to start the game client, so that it connects to the server. Our Launcher will do that.

  * Launcher binaries: [MUnique.OpenMU.ClientLauncher.v0.4.0.zip](https://github.com/MUnique/OpenMU/releases/download/v0.4.0/MUnique.OpenMU.ClientLauncher.v0.4.0.zip)
  * If your server and client runs on your local host, use any IP of 127.x.x.x, except 127.0.0.1, because this one is blocked by the client. For example, you could use 127.127.127.127

This guide describes two ways of starting the server. Use Docker, if you just
want to play around. If you want to develop or debug the server, choose the
manual way.

## Docker

Please take a look at the deploy-folder of this project. There you'll find a more detailed guide about how to set up this project.

### Demo Mode

If you just want to play around with the server, you can find the newest docker
all-in-one image on the Docker Hub: <https://hub.docker.com/r/munique/openmu>

To pull and run the latest docker image, run this command:
> docker run --name openmu -d -p 80:80 -p 44405:44405 -p 55901:55901 -p 55902:55902 -p 55903:55903 -p 55980:55980 munique/openmu:latest -demo

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

* Windows OS

  * It runs under Linux and MacOS, too. However, this guide describes it for
    windows.

  * PostgreSQL installed

  * Visual Studio 2022 (17.0) installed

  * [.NET Core SDK SDK 6.0.100](https://dotnet.microsoft.com/download/dotnet/6.0)
    (it should be included in Visual Studio 17.0)

  * [Saxon HE 9.9.1.6](https://sourceforge.net/projects/saxon/files/Saxon-HE/9.9/SaxonHE9-9-1-6N-setup.exe/download) installed

  * This repository cloned

If you have that, you'll need to do:

* Open the solution of OpenMU with Visual Studio

* Right click the solution and 'Restore NuGet Packages'

* Edit OpenMU\Persistence\EntityFramework\ConnectionSettings.xml, so that the
  connection strings are correct - however only the user/password of the first
  connection string needs to be correct. The server will try to create the
  other roles specified by the settings.

* Build the solution

* Start MUnique.OpenMU.Startup

  * If required, it will create the database schemas, the required roles and
    gives permissions to this roles

  * If you update to a newer state of the master-branch, it could be possible
    that you have to delete the database again before starting. Currently, we
    are not providing patches for database updates.

  * You can reinstall the database by adding a '-reinit' parameter

  * Optional: you can add the parameter '-resolveIP:' to bind the servers tcp
    listeners to an ip address of a local network interface. Look at this
    [Readme](src/Startup/Readme.md) for more information.

  * Optional: you can add the parameter '-autostart' to save the next step.

* When the Admin Panel is initialized, go to <http://localhost:1234/>. Then you
  should see three gameservers, the chat server and the connect server. Start
  the connect server and at least one gameserver.
  If all goes well, you should be able to expand a gameserver and see the
  hosted game maps.

* Then you can connect to the server through the game client.

## Test Accounts

To test some features of the server, test accounts are created automatically
when the database is initialized.

These are the user names:

* test0 - test9: General test accounts, level 1 to 90, in 10 level steps
* test300: General test account with level 300
* test400: General test account with level 400, master characters
* testgm: Test account of a game master
* quest1: Test account for the level 150 quests
* quest2: Test account for the level 220 quests
* quest3: Test account for the level 400 quests
* ancient: Test account with ancient item sets, level 330 characters
* socket: Test account with socket item sets, level 380 characters

The passwords of these accounts are the same as the user name.

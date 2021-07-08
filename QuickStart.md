# Quick Start OpenMU

General requirements:

* Free TCP ports:
  * 1234 (admin panel)
  * 55901, 55902, 55903 (game servers)
  * 44405 (connect server)
  * 55980 (chat server)

* Knowledge or way to start a game client, connecting to the server (I wont
    provide that, but there is a ClientLauncher project) :)

  * Launcher binaries: [MUnique.OpenMU.ClientLauncher.v0.4.0.zip](https://github.com/MUnique/OpenMU/releases/download/v0.4.0/MUnique.OpenMU.ClientLauncher.v0.4.0.zip)

This guide describes two ways of starting the server. Use Docker, if you just
want to play around. If you want to develop or debug the server, choose the
manual way.

## Docker

This guide assumes you know how to use docker in general and have docker
installed (e.g. by using Docker Desktop on Windows).

### Generates certificate

The project needs a certificate to run the public API.

* How to create one depends on your host OS, see below.

* `{ password here }` must be replaced with your certificate password.

* It needs to be run once on the host. If you forgot your password, you can
    run it again.

#### Linux

Build shell commands and run:

```bash
source dev.sh
generate_certificates { password here }
```

The script generates the certificate on a dedicated docker container and
exports the environment variable `CERTIFICATE_PASSWORD` which can be used at
docker-compose.

#### Windows

```cmd
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p { password here }
dotnet dev-certs https --trust
setx CERTIFICATE_PASSWORD "{ password here }"
```

### Demo Mode

If you just want to play around with the server, you can find the newest docker
image on the Docker Hub: <https://hub.docker.com/r/munique/openmu>

To pull and run the latest docker image, run this command:
> docker run --name openmu -d -p 1234:1234 -p 44405:44405 -p 55901:55901 -p 55902:55902 -p 55903:55903 -p 55980:55980 munique/openmu:latest -demo

The last argument is there to start the server in demo mode, without a
database. To use a postgres database, you can use docker-compose.

### Docker-compose

To start the server and database in one go, you can use docker-compose with our
[docker-compose.yml](docker-compose.yml). The command is as follows when you
are in the folder which includes the yml file:
> docker-compose up -d --build

If you want to pass additional arguments you can just call ```run``` and
append them at the end, for example if you want to start it with the
'-resolveIp:local' parameter:
> docker-compose run -d munique -resolveIp:local

Docker compose pulls the newest images from the docker hub, sets the network
and disk volume up and finally starts OpenMU and the postgres database in
separate containers.

### Environment Variables

It's possible to define additional environment variables to influence the
postgres database connection strings.

| Name | Description         |
|------|---------------------|
| DB_HOST | The hostname of the database. If the local configuration file is still configured to use 'localhost', the value of this variable replaces it |
| DB_ADMIN_USER | The user name of the postgres admin account. If the local configuration file is still configured to use 'postgres' for the user name of the admin (first entry in the ConnectionSettings.xml), the value of this variable replaces it. |
| DB_ADMIN_PW | The user name of the postgres admin account. If the local configuration file is still configured to use 'admin' for the user password of the admin (first entry in the ConnectionSettings.xml), the value of this variable replaces it. |
| ASPNETCORE_Kestrel__Certificates__Default__Password | The ASP NET application certificate password. When docker-compose-dev is used, the environment variable CERTIFICATE_PASSWORD must be exported/set on the host system instead. |

## Manually

Use this way, if you want to develop or debug for OpenMU.

Requirements:

* Windows OS

  * It runs under Linux and MacOS, too. However, this guide describes it for
    windows.

  * PostgreSQL installed

  * Visual Studio 2019 (16.8) installed

  * [.NET Core SDK SDK 5.0.100](https://dotnet.microsoft.com/download/dotnet/5.0)
    (it should be included in Visual Studio 16.8)

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

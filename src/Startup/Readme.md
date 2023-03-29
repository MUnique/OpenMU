# Startup

The startup console program is basically what glues all components together and
starts the server as a single process.

## Logging

Logging can be configured by the *appsettings.json* file.
By default, not a lot is configured. If you want to extend the configuration,
have a look a the [serilog documentation](https://github.com/serilog/serilog-settings-configuration).
The server makes good use of scopes, so you can configure it to log
only actions of certain players, for example.

In the future, it might be possible to change logging settings over the admin
panel, too.

## Parameters

**Please note, that the most of these parameters (except ```-demo``` and ```-adminpanel```)
are not necessary anymore, because these settings/actions can be done more conveniently
over the admin panel, too.**

You can start the server with the following parameters:

| Parameter   | Description       |
|-------------|-------------------|
| -autostart  | It automatically initializes the game servers and starts the tcp listeners of all (sub-)servers |
| -reinit     | It recreates and reinitializes the database. It doesn't have any effect when *-demo* is used. |
| -version:[season6\|0.75\|0.95d]    | Defines the version of the game client. Has only effect with *-reinit* or *-demo* and affects the initial data creation. Default: season6|
| -demo       | Instead of using an external database, it uses in-memory repositories and data is initialized at each start. Only for testing, not for production usage, as player progress is **not saved** to a database or file. |
| -deamon     | Deactivates handling of console inputs |
| -adminpanel:[enabled\|disabled] | Defines if the admin panel is available. If disabled, *-autostart* is applied automatically. Default: enabled |

### -resolveIP

Defines how the own ip address is determined which is reported back to the game
client in case it requests to connect to a selected game server (server selection
screen) or the chat server (when starting a chat with the in-game messenger).
This may be helpful, if the server is started in an environment where the public
IP is not reachable from the outside (e.g. because you share your IPv4-Address
or behind a firewall) and you want to use it within your computer or private network.

It supports the following values:

| Value  | Description  | Example |
|--------|--------------|---------|
| public | Default value, if nothing is specified. The public ip is automatically determined by an [external API](https://www.ipify.org/). | -resolveIP:public |
| local  | Determines a local ip. If none is found, a loopback IP is used (127.127.127.127). | -resolveIP:local |
| loopback  | For testing on the same machine, a loopback IP is used (127.127.127.127). | -resolveIP:loopback |
| [An IPv4-Address] | Defines a custom and constant IP address or a host name. | -resolveIP:140.82.118.4 |

## Environment variables

Additionally (and optionally), there are some settings which can be controlled with environment variables.
They may be helpful when running the server in a container or under linux.

| Variable    | Description       |
|-------------|-------------------|
| RESOLVE_IP  | See *-resolveIP* parameter. Same description and values applies here. Is only considered, when there is no *-resolveIP* parameter. |
| ASPNETCORE_ENVIRONMENT | If no *-resolveIP* parameter and no *RESOLVE_IP* variable is defined, the variable *ASPNETCORE_ENVIRONMENT* is considered to find the optimal ip resolver. If the value is "Development", it uses 'loopback'; Otherwise, it uses 'public'. |
| ASPNETCORE_URLS | Defines the address of the admin panel. Example: 'http://+:80' |
| DB_HOST | Host name/address of the postgres database |
| DB_ADMIN_USER | User name of the admin user of the postgres database |
| DB_ADMIN_PW   | Password of the admin user of the postgres database |

## Settings priority

As you noticed, you can set some options in different ways. Therefore, a clear
priority has been worked out to make the most sense:

1. Start parameters
2. Environment variables
3. Settings over the admin panel (Configuration -> System)

Start parameters have the highest priority, then environment variables and then
the settings over the admin panel. The idea is, that start parameters and
environment variables should only be used in special cases by experienced users.

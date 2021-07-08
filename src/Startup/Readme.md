# Startup

The startup console program is basically what glues all components together and
starts the server as a single process.

## Logging

Logging can be configured by the *MUnique.OpenMU.Startup.exe.log4net.xml* file.
By default, not a lot is configured. If you want to extend the configuration,
have a look a the [log4net documentation](https://logging.apache.org/log4net/release/manual/configuration.html).
The server makes good use of log4net contexts, so you can configure it to log
only actions of certain players, for example.

## Parameters

You can start the server with the following parameters:

| Parameter   | Description       |
|-------------|-------------------|
| -autostart  | It automatically initializes the game servers and starts the tcp listeners of all (sub-)servers |
| -reinit     | It recreates and reinitializes the database. It doesn't have any effect when *-demo* is used. |
| -version:[season6\|0.75]    | Defines the version of the game client. Has only effect with *-reinit* or *-demo* and affects the initial data creation. Default: season6|
| -demo       | Instead of using an external database, it uses in-memory repositories and data is initialized at each start. Only for testing, not for production usage, as player progress is **not saved** to a database or file. |
| -deamon     | Deactivates handling of console inputs |
| -adminport:[1 to 65535] | Defines the tcp port which should be used for the admin panel. Default: 1234 |
| -adminpanel:[enabled\|disabled] | Defines if the admin panel is available. If disabled, *-autostart* is applied automatically. Default: enabled |
| -api:[enabled\|disabled] | Defines if the admin panel is available. Default: enabled |

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
| [An IPv4-Address] | Defines a custom and constant IP address| -resolveIP:140.82.118.4 |

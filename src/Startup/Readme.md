# Startup

The startup console program is basically what glues all components together and starts the server as a single process.

## Logging

Logging can be configured by the MUnique.OpenMU.Startup.exe.log4net.xml file. By default, not a lot is configured.
If you want to extend the configuration, have a look a the [log4net documentation](https://logging.apache.org/log4net/release/manual/configuration.html).
The server makes good use of log4net contexts, so you can configure it to log only actions of certain players, for example.

## Parameters

You can start the server with the following parameters:

| Parameter   | Description       |
|-------------|-------------------|
| -autostart  | It automatically initializes the game servers and starts the tcp listeners of all (sub-)servers |
| -reinit     | It recreates and reinitializes the database. It doesn't have any effect when *-demo* is used. |
| -demo       | Instead of using an external database, it uses in-memory repositories and data is initialized at each start. Only for testing, not for production usage, as player progress is **not saved** to a database or file. |
| -local      | Uses the local ip address to bind listeners on it. Otherwise, the public ip is automatically determined by an [external API](https://www.ipify.org/). |

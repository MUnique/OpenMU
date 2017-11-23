# Startup

The startup console program is basically what glues all components together and starts the server as a single process.

## Logging

Logging can be configured by the MUnique.OpenMU.Startup.exe.log4net.xml file. By default, not a lot is configured.
If you want to extend the configuration, have a look a the [log4net documentation](https://logging.apache.org/log4net/release/manual/configuration.html).
The server makes good use of log4net contexts, so you can configure it to log only actions of certain players, for example.
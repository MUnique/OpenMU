
var Constants = {
    SERVER_SHUTDOWN: "SERVER_SHUTDOWN",
    SERVER_START: "SERVER_START",
    FILTER_SERVER: "FILTER_SERVER",
    FILTER_CHARACTER: "FILTER_CHARACTER",
    FILTER_LOGGER: "FILTER_LOGGER"
};

var ServerState = {
    Stopped: 0,
    Starting: 1,
    Started: 2,
    Stopping: 3
};

var FluxMixin = Fluxxor.FluxMixin(React),
    FluxChildMixin = Fluxxor.FluxChildMixin(React),
    StoreWatchMixin = Fluxxor.StoreWatchMixin;
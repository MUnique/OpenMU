/*  
 * The log store contains all the logic to retrieve the data by react.
 */
var LogStore = Fluxxor.createStore({
  initialize: function() {
    this.entries = [];
    this.visibleEntries = [];
    this.loggers = [];
    this.serverFilter = null;
    this.loggerFilter = null;
    this.characterFilter = null;
    this.autoRefresh = true;
    this.subscribers = 0;
    this.bindActions(
      Constants.LOG_FILTER_SERVER, this.filterByServer,
      Constants.LOG_FILTER_CHARACTER, this.filterByCharacter,
      Constants.LOG_FILTER_LOGGER, this.filterByLogger,
      Constants.LOG_SETAUTOREFRESH, this.setAutoRefresh,
      Constants.LOG_SUBSCRIBE, this.subscribe,
      Constants.LOG_UNSUBSCRIBE, this.unsubscribe
      );
    
    this.log4NetHub = $.connection.fooBarHub;

    var entries = this.entries;
    this.log4NetHub.client.onLoggedEvent = function(formattedEvent, loggedEvent) {
        if (console && console.log) {
            console.log("onLoggedEvent", formattedEvent, loggedEvent);
        }
        
        entries[entries.length] = loggedEvent;
        if (entries.length > 200) {
          entries.shift();
        }
        
        this.updateVisibleEntries();
        this.emitChange();
    }.bind(this);
    $.connection.hub.logging = true;
    
    this.loadLoggers();
  },

  start: function() {
    $.connection.hub.start(function () {
        this.log4NetHub.server.listen('MyGroup');
    }.bind(this));  
  },

  stop: function() {
      $.connection.hub.stop();  
  },

  subscribe: function() {
      this.subscribers++;
      if (this.subscribers === 1) {
          this.start();
      }
  },

  unsubscribe: function() {
      this.subscribers--;
      if (this.subscribers === 0) {
          this.stop();
      }
  },
  
  updateVisibleEntries: function() {
    if (this.autoRefresh === true) {
        this.visibleEntries = [];
        for(var i = this.entries.length - 1; i >= 0; i--) {
            var entry = this.entries[i];
            if (this.loggerFilter) {
              if (this.loggerFilter !== entry.LoggerName) {
                continue;
              }
            }
            
            if (this.characterFilter) {
              if (this.characterFilter !== entry.Properties["character"]) {
                continue;
              }
            }
            
            if (this.serverFilter) {
              if (this.serverFilter !== entry.Properties["gameserver"]) {
                continue;
              }
            }
            
            this.visibleEntries[this.visibleEntries.length] = entry;
            if (this.visibleEntries.length === 20) {
              break;
            }
        }
        
        this.visibleEntries = this.visibleEntries.reverse();
    }
  },

  getAll: function () {
    return this.visibleEntries;
  },
  
  getLoggers: function () {
    return this.loggers;
  },
  
  setAutoRefresh: function(value) {
    this.autoRefresh = value;
    this.updateVisibleEntries();
    this.emitChange();
  },
  
  getAutoRefresh: function() {
    return this.autoRefresh;
  },
  
  loadLoggers: function() {
    $.ajax({
      url: "/admin/log/loggers",
      dataType: 'json',
      success: function(data) {
          this.loggers = data.sort();
        this.emitChange();
      }.bind(this),
      error: function(xhr, status, err) {
        console.error(this.props.url, status, err.toString());
      }
    });
  },
  
  emitChange: function() {
    this.emit("change");
  },
  
  filterByServer: function(server) {
    this.serverFilter = server;
    this.updateVisibleEntries();
    this.emitChange();
  },
  
  filterByCharacter: function(character) {
    this.characterFilter = character;
    this.updateVisibleEntries();
    this.emitChange();
  },
  
  filterByLogger: function(logger) {
    this.loggerFilter = logger;
    this.updateVisibleEntries();
    this.emitChange();
  }
  
  
});
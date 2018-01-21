/*  
 * The server store contains all the logic to retrieve the data and to execute the actions (all AJAX-Calls here).
 */
var ServerStore = Fluxxor.createStore({

  initialize: function() {
    this.servers = [];
    this.subscribers = 0;
    this.bindActions(
      Constants.SERVER_START, this.start,
      Constants.SERVER_SHUTDOWN, this.shutdown,
      Constants.SERVERLIST_SUBSCRIBE, this.subscribe,
      Constants.SERVERLIST_UNSUBSCRIBE, this.unsubscribe
    );
   
    setInterval(this.reload, 2000); //reload every 2 seconds;
  },

  reload: function () {
    if (this.subscribers === 0) {
        return;
    }

    this.setLoading(true);
    $.ajax({
      url: "/admin/server/list",
      dataType: "json",
      success: function(data) {
          this.servers = data;
          this.setLoading(false);
          this.emitChange();
      }.bind(this),
      error: function(xhr, status, err) {
          console.error(this.url, status, err.toString());
          // we don't set loading to true in this case
      }.bind(this)
    });
  },

  subscribe: function() {
    this.subscribers++;
  },

  unsubscribe: function() {
    this.subscribers--;
  },


  setLoading: function (value) {
      if (this.flux !== undefined) {
          this.flux.store("CommonStateStore").setLoading(value);
      }
  },
 
  getAll: function () {
    return this.servers;
  },

  getServerWithId: function (serverId) {
      for (var i = 0; i < this.servers.length; i++) {
          if (this.servers[i].id === serverId) {
              return this.servers[i];
          }
      }

      return null;
  },
  
  updateServerData: function(serverData) {
      for (var i = 0; i < this.servers.length; i++) {
          if (this.servers[i].id === serverData.id) {
              this.servers[i] = serverData;
              this.emitChange();
              break;
          }
      }
  },
  
  start: function(serverId) {
    this.serverAction(serverId, "start");
  },
  
  shutdown: function(serverId) {
    this.serverAction(serverId, "shutdown");
  },

  serverAction: function (serverId, actionName) {
    this.setLoading(true);
    $.ajax({
      url: '/admin/server/' + actionName + '/' + serverId,
      dataType: 'json',
      success: function(serverData) {
        this.updateServerData(serverData);
      }.bind(this),
      
      error: function(xhr, status, err) {
        console.error("", status, err.toString());
      }.bind(this)
    });
  },
  
  emitChange: function() {
    this.emit("change");
  }
});
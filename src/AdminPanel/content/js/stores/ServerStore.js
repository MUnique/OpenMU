/*  
 * The server store contains all the logic to retrieve the data and to execute the actions (all AJAX-Calls here).
 */
var ServerStore = Fluxxor.createStore({

  initialize: function() {
    this.servers = [];
    this.bindActions(
      Constants.SERVER_START, this.start,
      Constants.SERVER_SHUTDOWN, this.shutdown
    );
    
    this.reload();
    setInterval(this.reload, 2000); //reload every 2 seconds;
  },

  reload: function() {
    $.ajax({
      url: "/admin/list",
      dataType: "json",
      success: function(data) {
        this.servers = data;
        this.emitChange();
      }.bind(this),
      error: function(xhr, status, err) {
        console.error(this.url, status, err.toString());
      }
    });
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
  
  serverAction: function(serverId, actionName) {
    $.ajax({
      url: '/admin/' + actionName + '/' + serverId,
      dataType: 'json',
      success: function(serverData) {
        this.updateServerData(serverData);
        this.emitChange();
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
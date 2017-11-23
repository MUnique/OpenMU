var ServerListActions = {
  shutdown: function(serverId) {
    this.dispatch(Constants.SERVER_SHUTDOWN, serverId);
  },

  start: function(serverId) {
    this.dispatch(Constants.SERVER_START, serverId);
  }
};
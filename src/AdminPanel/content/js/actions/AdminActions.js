var AdminActions = {
  shutdown: function(serverId) {
    this.dispatch(Constants.SERVER_SHUTDOWN, serverId);
  },

  start: function(serverId) {
    this.dispatch(Constants.SERVER_START, serverId);
  },

  saveAccount: function(accountId) {
    this.dispatch(Constants.ACCOUNT_SAVE, accountId);
  },

  deleteAccount: function (accountId) {
    this.dispatch(Constants.ACCOUNT_DELETE, accountId);
  },

  filterLogByServer: function (server) {
      this.dispatch(Constants.LOG_FILTER_SERVER, server);
  },

  filterLogByLogger: function (logger) {
      this.dispatch(Constants.LOG_FILTER_LOGGER, logger);
  },

  filterLogByCharacter: function (character) {
      this.dispatch(Constants.LOG_FILTER_CHARACTER, character);
  },

  setLogAutoRefresh: function(value) {
      this.dispatch(Constants.LOG_SETAUTOREFRESH, value);
  },

  logSubscribe: function() {
      this.dispatch(Constants.LOG_SUBSCRIBE);
  },

  logUnsubscribe: function() {
      this.dispatch(Constants.LOG_UNSUBSCRIBE);
  },

  serverListSubscribe: function() {
      this.dispatch(Constants.SERVERLIST_SUBSCRIBE);
  },

  serverListUnsubscribe: function() {
      this.dispatch(Constants.SERVERLIST_UNSUBSCRIBE);
  }
};
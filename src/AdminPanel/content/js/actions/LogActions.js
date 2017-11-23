var LogActions = {
  filterByServer: function(server) {
    this.dispatch(Constants.FILTER_SERVER, server);
  },

  filterByLogger: function(logger) {
    this.dispatch(Constants.FILTER_LOGGER, logger);
  },
  
  filterByCharacter: function(character) {
    this.dispatch(Constants.FILTER_CHARACTER, character);
  }
};
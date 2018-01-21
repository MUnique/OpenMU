var ServerList = React.createClass({
  mixins: [FluxChildMixin, StoreWatchMixin("ServerStore")],
  
  getStateFromFlux: function() {
    return {
      servers: this.getFlux().store("ServerStore").getAll()
    };
  },

  componentDidMount: function () {
      this.getFlux().actions.serverListSubscribe();
  },

  componentWillUnmount: function () {
      this.getFlux().actions.serverListUnsubscribe();
  },

  render: function() {
    var serverList = this.state.servers.map(function (server) {
      return (
        <ServerItem server={server} key={server.id}/>
      );
    });
           
    return (
          <table className="table table-striped table-hover" >
            <thead>
              <tr>
                <th className="col-xs-0"></th>
                <th className="col-xs-7">Server Name</th>
                <th className="col-xs-1">Players</th>
                <th className="col-xs-2">Current State</th>
                <th className="col-xs-2">Action</th>
              </tr>
            </thead>
            <tfoot>
              <TotalOnlineCounter servers={this.state.servers} />
            </tfoot>
            <tbody>
                {serverList}
            </tbody>
          </table>
          
        );
  }
});
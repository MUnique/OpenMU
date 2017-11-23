var ServerItem = React.createClass({
  mixins: [FluxChildMixin],
  
  getInitialState: function () {
      return { expanded: false };
  },

  handleClick: function(event) {
    var serverId = this.props.server.id;
    if (this.props.server.state === ServerState.Started) {
      this.getFlux().actions.shutdown(serverId);
    } else if (this.props.server.state === ServerState.Stopped) {
      this.getFlux().actions.start(serverId);
    }
  },

  expand: function(event) {
      this.setState({ expanded: !this.state.expanded });
  },
  
  getActionCaption: function() {
      if (this.props.server.state === ServerState.Started)
          return "Shutdown";
      else
          return "Start";
  },

  getServerStateCaption: function () {
      if (this.props.server.state === ServerState.Started)
          return "Started";
      else if (this.props.server.state === ServerState.Stopped)
          return "Stopped";
      else if (this.props.server.state === ServerState.Starting)
          return "Starting...";
      else if (this.props.server.state === ServerState.Stopping)
          return "Stopping...";
      return "";
  },

  isActionAvailable: function() {
      return (this.props.server.state === ServerState.Started || this.props.server.state === ServerState.Stopped);
  },
  
  getActionClass: function() {
    
    if (this.props.server.state === ServerState.Started)
      return 'btn btn-xs btn-success';
    else
      return 'btn btn-xs btn-warning';
  },

  getExpandItemClass: function () {
      var buttonClass = "btn btn-default btn-xs ";
      if (this.state.expanded) {
          return buttonClass + 'glyphicon glyphicon-minus';
      } else {
          return buttonClass + 'glyphicon glyphicon-plus';
      }
  },
  
  render: function() {
    return (
        
        <tr className={this.props.server.state === ServerState.Started ? 'success' : 'warning'}>
            <td>
                <button type="button" className={this.getExpandItemClass()} onClick={this.expand}>
                </button>
            </td>
            <td>
                <table>
                    <thead>
                    <tr>
                        <td>{this.props.server.description}</td>
                    </tr>
                    </thead>
                {this.state.expanded ? <tbody><tr><td><MapList serverId={this.props.server.id} /></td></tr></tbody> : null}
                </table>
            </td>
            <td><div>{this.props.server.onlinePlayerCount} / {this.props.server.maximumPlayers}</div></td>
            <td>{this.getServerStateCaption()}</td>
            <td>
                <button type="button" disabled={!this.isActionAvailable()} className={this.getActionClass()} onClick={this.handleClick}>{this.getActionCaption()}</button>
            </td>
        </tr>
    );
  }
});

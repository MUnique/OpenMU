var TotalOnlineCounter = React.createClass({
  mixins: [FluxChildMixin],
  
  summarizePlayerCount: function() {
    var servers = this.props.servers;
    var count = 0;
    for (var i = 0; i < servers.length; i++) {
        if (servers[i].Id < 0x10000) { // we only count the game servers
            count += servers[i].onlinePlayerCount;
        }
    }
    
    return count;
  },

  render: function() {          
    return (
        <tr className="info">
          <td colSpan="5">
             <span>Total Players: {this.summarizePlayerCount()}</span>
          </td>
        </tr>
    );
  }
});
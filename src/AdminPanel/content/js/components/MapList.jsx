var MapList = React.createClass({
    mixins: [FluxChildMixin, StoreWatchMixin("ServerStore")],

  getStateFromFlux: function () {
    return {
        maps: this.getFlux().store("ServerStore").getServerWithId(this.props.serverId).maps
    };
  },

  render: function () {
    var mapList = this.state.maps.map(function (map) {
          return (
            <MapItem map={map} key={map.id + map.serverId * 0x10000}/>
      );
    });

    return (
        <table className="table table-striped table-hover">
            <thead>
                <tr>
                    <td className="col-xs-10">Map Name</td>
                    <td className="col-xs-1">Players</td>
                    <td className="col-xs-0">Spectate</td>
                </tr>
            </thead>
            <tbody>
                {mapList}
            </tbody>
        </table>
    );
  }
});

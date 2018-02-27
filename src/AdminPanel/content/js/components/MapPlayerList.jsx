var MapPlayerList = React.createClass({
    mixins: [FluxMixin, StoreWatchMixin("MapPlayerStore")],

    getStateFromFlux: function () {
        return {
            players: this.getFlux().store("MapPlayerStore").getPlayers()
        };
    },

    render: function () {
        let players = this.state.players;
        let playerList = Object.keys(players).map(function(key, index) {
            let player = players[key];
            return (
                <MapPlayerListItem player={player} key={player.Id}/>
            );
        });

        return (
            <table className="table table-striped table-hover">
                <thead>
                <tr>
                    <td className="col-xs-2">Player Name</td>
                    <td className="col-xs-1">ID</td>
                    <td className="col-xs-2">Actions</td>
                </tr>
                </thead>
                <tbody>
                    {playerList}
                </tbody>
            </table>
        );
    }
});

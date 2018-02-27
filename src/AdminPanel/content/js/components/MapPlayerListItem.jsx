var MapPlayerListItem = React.createClass({
    mixins: [FluxChildMixin],

    handleClickOnHighlightButton: function (event) {
        if (this.props.player.isHighlighted) {
            this.getFlux().actions.highlightOff(this.props.player.Id);
        } else {
            this.getFlux().actions.highlightOn(this.props.player.Id);
        }
    },

    handleClickOnDisconnectButton: function(event) {
        this.getFlux().actions.disconnectPlayer(this.props.player.Id);
    },

    handleClickOnBanButton: function(event) {
        this.getFlux().actions.banPlayer(this.props.player.Id);
    },

    render: function () {
        return (
            <tr>
                <td>{this.props.player.Name}</td>
                <td>{this.props.player.Id}</td>
                <td>
                    {this.renderHighlightButton()}
                    <button type="button" className="btn btn-default btn-xs glyphicon glyphicon-log-out" onClick={this.handleClickOnDisconnectButton} title="Disconnect"></button>
                    <button type="button" className="btn btn-default btn-xs glyphicon glyphicon-ban-circle" onClick={this.handleClickOnBanButton} title="Temporarily ban"></button>
                </td>
            </tr>
        );
    },

    renderHighlightButton: function () {
        let className = "btn btn-default btn-xs glyphicon glyphicon-map-marker";
        if (this.props.player.isHighlighted) {
            className += " active";
        }

        return (<button type="button" className={className} onClick={this.handleClickOnHighlightButton} title="Highlight on map"></button>);
    }


});

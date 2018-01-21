var AccountItem = React.createClass({
    mixins: [FluxChildMixin],

    /*
    getInitialState: function () {
        return { expanded: false };
    },

    expand: function (event) {
        this.setState({ expanded: !this.state.expanded });
    },

    getActionCaption: function () {
        if (this.props.server.state === ServerState.Started)
            return "Shutdown";
        else
            return "Start";
    },

    

    isActionAvailable: function () {
        return (this.props.server.state === ServerState.Started || this.props.server.state === ServerState.Stopped);
    },

    getActionClass: function () {

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
    */


    render: function () {
        return (

            <tr>
                <td></td>
                <td>{this.props.account.loginName}</td>
                <td>{AccountState.getCaption(this.props.account.state)}</td>
                <td>{this.props.account.eMail}</td>
                <td>
                    <button type="button" className='btn btn-xs' onClick={this.handleClick}>Ban</button>
                </td>
            </tr>
        );
    },

    handleClick: function (event) {
        this.props.account.state = AccountState.Banned;
        this.getFlux().actions.saveAccount(this.props.account);
    }
});

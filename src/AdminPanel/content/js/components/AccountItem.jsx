var AccountItem = React.createClass({
    mixins: [FluxChildMixin],

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

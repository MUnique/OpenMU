var CreateAccountModal = React.createClass({
    mixins: [FluxChildMixin],

    render: function() {
        const backdropStyle = {
            position: 'fixed',
            top: 0,
            bottom: 0,
            left: 0,
            right: 0,
            backgroundColor: 'rgba(0,0,0,0.3)',
            padding: 50
        };

        // The modal "window"
        const modalStyle = {
            backgroundColor: '#fff',
            borderRadius: 5,
            maxWidth: 500,
            minHeight: 300,
            margin: '0 auto',
            padding: 30
        };

        if (!this.props.visible) {
            return null;
        }

        return (
            <div style={backdropStyle}>
                <div style={modalStyle}>
                    <h2>Create Account</h2>
                    <form onSubmit={this.submit}>
                    <table>
                        <tbody>
                            <tr>
                                <td>Login Name:</td>
                                <td><input type="text" maxlength="10" required="true" onChange={this.loginNameChanged}/></td>
                            </tr>
                            <tr>
                                <td>E-Mail Address:</td>
                                <td><input type="text" required="true" onChange={this.eMailChanged}/></td>
                            </tr>
                            <tr>
                                <td>Password:</td>
                                <td><input type="password" maxlength="10" id="password" onChange={this.passwordChanged}/></td>
                            </tr>
                            <tr>
                                <td>State:</td>
                                <td>TODO</td>
                            </tr>
                        </tbody>
                        <tfoot>
                            <tr>
                                <td><button type="submit" className="btn btn-xs btn-success">Save</button></td>
                                <td><button type="button" className="btn btn-xs btn-warning" onClick={this.cancel}>Cancel</button></td>
                            </tr>
                        </tfoot>
                    </table>
                    </form>
                </div>
            </div>);
    },

    loginNameChanged: function(event) {
        this.setState({ loginName: event.target.value });
    },

    passwordChanged: function(event) {
        this.setState({ newPassword: event.target.value });
    },

    eMailChanged: function(event) {
        this.setState({ eMail: event.target.value });
    },

    submit: function (event) {
        this.getFlux().actions.saveAccount(this.state);
        // TODO: wait for result until close?
        this.props.onClose();
        event.preventDefault();
    },

    cancel: function() {
        this.props.onClose();
    }
});
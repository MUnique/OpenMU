import React from "react";
import { connect } from "react-redux";
import { saveAccount, hideCreateDialog } from "../stores/accounts/actions";
import { NewAccount, AccountState } from "../stores/accounts/types";

export interface ICreateAccountModalProps {
    cancel: () => void;
    save: (account: NewAccount) => Promise<void>;
}

export interface ICreateAccountModalState {
    loginName: string;
    newPassword: string;
    eMail: string;
}

class CreateAccountModal extends React.Component<ICreateAccountModalProps, ICreateAccountModalState> {

    public render() {
        const backdropStyle = {
            position: 'fixed' as 'fixed',
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

        return (
            <div style={backdropStyle}>
                <div style={modalStyle}>
                    <h2>Create Account</h2>
                    <form onSubmit={(event: any) => this.submit(event)}>
                        <table>
                            <tbody>
                            <tr>
                                <td>Login Name:</td>
                                    <td><input type="text" maxLength={10} required={true} onChange={(e: InputFormEvent) => this.loginNameChanged(e)}/></td>
                            </tr>
                            <tr>
                                <td>E-Mail Address:</td>
                                    <td><input type="text" required={true} onChange={(e: InputFormEvent) => this.eMailChanged(e)}/></td>
                            </tr>
                            <tr>
                                <td>Password:</td>
                                    <td><input type="password" required={true} maxLength={10} id="password" onChange={(e: InputFormEvent) => this.passwordChanged(e)}/></td>
                            </tr>
                            <tr>
                                <td>State:</td>
                                <td>TODO</td>
                            </tr>
                            </tbody>
                            <tfoot>
                            <tr>
                                <td><button type="submit" className="btn btn-xs btn-success">Save</button></td>
                                <td><button type="button" className="btn btn-xs btn-warning" onClick={() => this.props.cancel()}>Cancel</button></td>
                            </tr>
                            </tfoot>
                        </table>
                    </form>
                </div>
            </div>);
    }

    private loginNameChanged(event: InputFormEvent) {
        this.setState({ loginName: event.target.value });
    }

    private passwordChanged(event: InputFormEvent) {
        this.setState({ newPassword: event.target.value });
    }

    private eMailChanged(event: InputFormEvent) {
        this.setState({ eMail: event.target.value });
    }

    private submit(event: any) {

        let newAccount: NewAccount = {
            loginName: this.state.loginName,
            state: AccountState.Normal,
            eMail: this.state.eMail,
            password: this.state.newPassword,
            id: null,
        };

        this.props.save(newAccount);
        event.preventDefault();
    }

    private cancel() {
        this.props.cancel();
    }
}


const mapDispatchToProps = (dispatch: any) => {
    return {
        cancel: () => dispatch(hideCreateDialog()),
        save: (account : NewAccount) => dispatch(saveAccount(account))
    };
}

export default connect(null, mapDispatchToProps)(CreateAccountModal);
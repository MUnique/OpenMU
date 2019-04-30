import React from "react";
import { connect } from "react-redux";
import { saveAccount } from "../stores/accounts/actions";
import { NewAccount, AccountState } from "../stores/accounts/types";
import { hideModal } from "content/js/stores/modal/actions";

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
        return (
            <div>
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
            id: '00000000-0000-0000-0000-000000000000',
            loginName: this.state.loginName,
            state: AccountState.Normal,
            eMail: this.state.eMail,
            password: this.state.newPassword,
        };

        this.props.save(newAccount);
        event.preventDefault();
    }
}


const mapDispatchToProps = (dispatch: any) => {
    return {
        cancel: () => dispatch(hideModal()),
        save: (account : NewAccount) => dispatch(saveAccount(account))
    };
}

export default connect(null, mapDispatchToProps)(CreateAccountModal);
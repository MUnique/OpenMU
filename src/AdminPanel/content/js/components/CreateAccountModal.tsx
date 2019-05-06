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
                    <div className="form-group">
                        <label>Login Name</label>
                        <input className="form-control" type="text" maxLength={10} required={true} onChange={(e: InputFormEvent) => this.loginNameChanged(e)} />
                    </div>
                    <div className="form-group">
                        <label>E-Mail Address</label>
                        <input className="form-control" type="text" required={true} onChange={(e: InputFormEvent) => this.eMailChanged(e)} />
                    </div>
                    <div className="form-group">
                        <label>Password</label>
                        <input className="form-control" type="password" required={true} maxLength={10} id="password" onChange={(e: InputFormEvent) => this.passwordChanged(e)} />
                    </div>
                    <div className="form-group">
                        <label>State</label>
                        <select className="form-control" disabled={true}>
                            <option>TODO</option>
                        </select>
                    </div>
                    <div className="btn-toolbar" role="toolbar" aria-label="Toolbar with button groups" >
                        <div className="btn-group" role="group" aria-label="Save group">
                            <button type="submit" className="btn btn-success btn-secondary">Save</button>
                        </div>
                        <div className="btn-group" role="group" aria-label="Cancel group">
                            {this.props.cancel ? <button type="button" className="btn btn-warning btn-secondary" onClick={() => this.props.cancel()}>Cancel</button> : null}
                        </div>
                    </div>
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
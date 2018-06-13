import React from "react";
import { connect } from "react-redux";
import { Account, AccountState } from "../stores/accounts/types";
import { saveAccount } from "../stores/accounts/actions";

interface IAccountItemProps {
    account: Account;
    save: (account: Account) => Promise<void>;
}

class AccountItem extends React.Component<IAccountItemProps, {}> {

    public render() {
        return (
            <tr>
                <td></td>
                <td>{this.props.account.loginName}</td>
                <td>{AccountState.getCaption(this.props.account.state)}</td>
                <td>{this.props.account.eMail}</td>
                <td>
                    <button type="button" className='btn btn-xs' onClick={() => this.ban()}>Ban</button>
                </td>
            </tr>
        );
    }

    private ban() {
        // make a copy of the instance, so we're not editing the props or state of the application yet
        let account = { ...this.props.account, state : AccountState.Banned };
        this.props.save(account);
    }
}


const mapDispatchToProps = (dispatch: any) => {
    return {
        save: (account: Account) => dispatch(saveAccount(account))
    };
}

export default connect(null, mapDispatchToProps)(AccountItem);
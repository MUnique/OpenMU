import React from "react";
import { connect } from "react-redux";

import { ApplicationState } from "../stores/index";
import { Account } from "../stores/accounts/types";
import { fetchAccounts, showCreateDialog } from "../stores/accounts/actions";

import AccountItem from "./AccountItem";
import CreateAccountModal from "./CreateAccountModal";


interface IAccountListProps {
    accounts: Account[];
    page: number;
    pageSize: number;
    hasMoreEntries: boolean;
    createDialogVisible: boolean;
    showCreateDialog: () => void;
    fetchPage(newPage: number, entriesPerPage: number): Promise<void>;
}

class AccountList extends React.Component<IAccountListProps, {}> {
    public componentDidMount() {
        this.props.fetchPage(this.props.page, this.props.pageSize);
    }

    public render() {
        let accountList = this.props.accounts.map(
            account => <AccountItem account={account} key={account.id}/>);
        let createDialog = this.props.createDialogVisible
            ? <CreateAccountModal/>
            : <span></span>;
        return (
            <div>
                <button type="button" className={this.getPreviousButtonClass()} onClick={() => this.props.fetchPage(this.props.page - 1, this.props.pageSize)}>&lt;</button> Page {this.props.page} <button type="button" className={this.getNextButtonClass()} onClick={() => this.props.fetchPage(this.props.page + 1, this.props.pageSize)}>&gt;</button>
                <table className="table table-striped table-hover">
                    <thead>
                    <tr>
                        <th className="col-xs-1">Login Name</th>
                        <th className="col-xs-1">State</th>
                        <th className="col-xs-2">E-Mail</th>
                        <th className="col-xs-2">Action</th>
                    </tr>
                    </thead>
                    <tbody>
                        {accountList}
                    </tbody>
                    <tfoot>
                    <tr>
                        <td><button type="button" className="btn btn-xs btn-success" onClick={this.props.showCreateDialog}>Create</button></td>
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>
                    </tfoot>
                </table>
                {createDialog}
            </div>
        );
    }

    getPreviousButtonClass(): string {
        const buttonClass = "btn btn-xs ";
        if (this.props.page <= 1) {
            return buttonClass + 'disabled';
        }

        return buttonClass;
    }

    getNextButtonClass() : string {
        const buttonClass = "btn btn-xs ";
        if (!this.props.hasMoreEntries) {
            return buttonClass + 'disabled';
        }

        return buttonClass;
    }
}


const mapDispatchToProps = (dispatch: any) => {
    return {
        fetchPage: (newPage: number, entriesPerPage: number) => {
            if (newPage > 0) {
                dispatch(fetchAccounts(newPage, entriesPerPage));
            }
        },
        showCreateDialog: () => dispatch(showCreateDialog())
    };
}

const mapStateToProps = (state: ApplicationState) => {
    return {
        accounts: state.accountListState.accounts,
        page: state.accountListState.page,
        pageSize: state.accountListState.pageSize,
        hasMoreEntries: state.accountListState.hasMoreEntries,
        createDialogVisible: state.accountListState.createDialogVisible,
    };
};


export default connect(mapStateToProps, mapDispatchToProps)(AccountList);
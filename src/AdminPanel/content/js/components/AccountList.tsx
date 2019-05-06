import React from "react";
import { connect } from "react-redux";

import { ApplicationState } from "../stores/index";
import { Account } from "../stores/accounts/types";
import { fetchAccounts } from "../stores/accounts/actions";

import AccountItem from "./AccountItem";
import CreateAccountModal from "./CreateAccountModal";
import { showModal } from "content/js/stores/modal/actions";
import { ListComponent, IListProps } from "./ListComponent";


interface IAccountListProps extends IListProps {
    accounts: Account[];
    showModal: (content: any) => void;
    fetchPage(newPage: number, entriesPerPage: number): Promise<void>;
}

class AccountList extends ListComponent<IAccountListProps, {}> {
    public componentDidMount() {
        this.props.fetchPage(this.props.page, this.props.pageSize);
    }

    public render() {
        let accountList = this.props.accounts.map(
            account => <AccountItem account={account} key={account.id}/>);

        return (
            <div>
                {this.getToolbar()}
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
                        <td><button type="button" className="btn btn-xs btn-success" onClick={() => this.showCreateDialog()}>Create</button></td>
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>
                    </tfoot>
                </table>
            </div>
        );
    }

    fetchNextPage() {
        this.props.fetchPage(this.props.page + 1, this.props.pageSize);
    }

    fetchPreviousPage() {
        this.props.fetchPage(this.props.page - 1, this.props.pageSize);
    }

    showCreateDialog() {
        this.props.showModal((<CreateAccountModal/>));
    }
}


const mapDispatchToProps = (dispatch: any) => {
    return {
        fetchPage: (newPage: number, entriesPerPage: number) => {
            if (newPage > 0) {
                dispatch(fetchAccounts(newPage, entriesPerPage));
            }
        },
        showModal: (content: any) => dispatch(showModal(content)),
    };
}

const mapStateToProps = (state: ApplicationState) => {
    return {
        accounts: state.accountListState.accounts,
        page: state.accountListState.page,
        pageSize: state.accountListState.pageSize,
        hasMoreEntries: state.accountListState.hasMoreEntries,
    };
};


export default connect(mapStateToProps, mapDispatchToProps)(AccountList);
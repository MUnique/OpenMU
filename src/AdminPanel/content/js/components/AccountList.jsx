var AccountList = React.createClass({
    mixins: [FluxChildMixin, StoreWatchMixin("AccountStore")],

    getInitialState: function() {
        let initialPageSize = 20;
        // this.getFlux().store("AccountStore").setRange(0, initialPageSize);
        return { pageSize: initialPageSize };
    },

    getStateFromFlux: function() {

        return {
            accounts: this.getFlux().store("AccountStore").getCurrent(),
            page: this.getFlux().store("AccountStore").getCurrentPage()
        };
    },

    render: function() {
        let accountList = this.state.accounts.map(function(account) {
            return (
                <AccountItem account={account} key={account.id}/>
            );
        });

        return (
            <div>
                <table className="table table-striped table-hover">
                    <thead>
                        <tr>
                            <th className="col-xs-1"></th>
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
                            <td><button type="button" className="btn btn-xs btn-success" onClick={this.showCreateDialog}>Create</button></td>
                            <td colSpan="3"><button type="button" className="btn btn-xs" onClick={this.previousPage}>&lt;</button> Page {this.state.page} <button type="button" className="btn btn-xs" onClick={this.nextPage}>&gt;</button></td>
                            <td></td>
                        </tr>
                    </tfoot>
                </table>
                <CreateAccountModal visible={this.state.showCreate} onClose={this.hideCreateDialog}/>
            </div>
        );
    },

    hideCreateDialog: function() {
        this.setState({ showCreate: false });
    },

    showCreateDialog: function() {
        this.setState({ showCreate: true });
    },

    nextPage: function() {
        this.setPageInStore(this.state.page + 1);
    },

    previousPage : function () {
        this.setPageInStore(this.state.page - 1);
    },

    setPageInStore(newPage) {
        let offset = (newPage - 1) * this.state.pageSize;
        
        this.getFlux().store("AccountStore").setRange(Math.max(offset, 0), this.state.pageSize);
    }
});
/*  
 * The account store contains all the logic to retrieve the data and to execute the actions (all AJAX-Calls here).
 */
var AccountStore = Fluxxor.createStore({

    initialize: function () {
        this.accounts = [];
        this.currentOffset = 0;
        this.currentCount = 1;
        this.bindActions(
            Constants.ACCOUNT_SAVE, this.save,
            Constants.ACCOUNT_DELETE, this.delete
        );

        this.setRange(0, 20);
    },
    
    setRange: function(offset, count) {
        if (offset !== this.currentOffset || count !== this.currentCount) {
            this.reload(offset, count);
        }
    },

    reloadCurrent: function() {
        this.reload(this.currentOffset, this.currentCount);
    },

    reload: function (offset, count) {
        this.setLoading(true);
        $.ajax({
            url: "/admin/account/list/" + offset + "/" + count,
            dataType: "json",
            success: function(data) {
                this.accounts = data;
                this.currentOffset = offset;
                this.currentCount = count;
                this.setLoading(false);
                this.emitChange();
            }.bind(this),
            error: function(xhr, status, err) {
                console.error(this.url, status, err.toString());
                this.setLoading(false);
            }.bind(this)
        });
    },

    setLoading: function (value) {
        if (this.flux !== undefined) {
            this.flux.store("CommonStateStore").setLoading(value);
        }
    },

    getCurrentPage: function () {
        return this.currentOffset / this.currentCount + 1;
    },

    getCurrent: function() {
        return this.accounts;
    },

    getAccountWithId: function (accountId) {
        for (var i = 0; i < this.accounts.length; i++) {
            if (this.accounts[i].id === accountId) {
                return this.accounts[i];
            }
        }

        return null;
    },

    save: function (account) {
        this.setLoading(true);
        $.ajax({
            url: '/admin/account/save',
            dataType: 'json',
            data: account,
            success: function (accountData) {
                this.setLoading(false);
                this.reloadCurrent();
            }.bind(this),

            error: function (xhr, status, err) {
                console.error("", status, err.toString());
                this.setLoading(false);
                this.reloadCurrent();
            }.bind(this)
        });
    },

    delete: function (accountId) {
        this.setLoading(true);
        var account = this.getAccountWithId(accountId);
        $.ajax({
            url: '/admin/account/delete/' + account.id,
            dataType: 'json',
            success: function (accountData) {
                accounts.remove(account);
                this.setLoading(false);
                this.emitChange();
            }.bind(this),

            error: function (xhr, status, err) {
                console.error("", status, err.toString());
            }.bind(this)
        });
    },

    emitChange: function () {
        this.emit("change");
    }
});
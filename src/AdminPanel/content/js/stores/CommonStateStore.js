/*  
 * The account store contains all the logic to retrieve the data and to execute the actions (all AJAX-Calls here).
 */
var CommonStateStore = Fluxxor.createStore({

    initialize: function () {
        this.loading = true;
    },

    setLoading: function (value) {
        this.loading = value;
        this.emitChange();
    },

    isLoading: function () {
        return this.loading;
    },

    emitChange: function () {
        this.emit("change");
    }
});
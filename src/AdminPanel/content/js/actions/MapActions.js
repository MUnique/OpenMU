var MapActions = {
    highlightOn: function (objectId) {
        this.dispatch(Constants.MAP_HIGHLIGHT_ON, objectId);
    },

    highlightOff: function (objectId) {
        this.dispatch(Constants.MAP_HIGHLIGHT_OFF, objectId);
    },

    disconnectPlayer: function(objectId) {
        this.dispatch(Constants.MAP_DISCONNECT_PLAYER, objectId);
    },

    banPlayer: function(objectId) {
        this.dispatch(Constants.MAP_BAN_PLAYER, objectId);
    }
};
var MapPlayerStore = Fluxxor.createStore({

    initialize: function () {
        this.players = {};
        this.bindActions(
            Constants.MAP_HIGHLIGHT_ON, this.highlightOn,
            Constants.MAP_HIGHLIGHT_OFF, this.highlightOff,
            Constants.MAP_BAN_PLAYER, this.banPlayer,
            Constants.MAP_DISCONNECT_PLAYER, this.disconnectPlayer
        );
    },

    addPlayer: function (newPlayer) {
        this.players[newPlayer.Id] = newPlayer;
        this.emitChange();
    },

    removePlayer: function (removedPlayer) {
        delete this.players[removedPlayer.Id];
        this.emitChange();
    },

    highlightOn: function(objectId) {
        let player = this.players[objectId];
        player.isHighlighted = true;
        this.emitChange();
    },

    highlightOff: function (objectId) {
        let player = this.players[objectId];
        player.isHighlighted = false;
        this.emitChange();
    },

    disconnectPlayer: function (objectId) {
        let player = this.players[objectId];
        $.ajax({
            url: "/admin/player/disconnect/" + player.ServerId + "/" + player.Name,
            dataType: "json"
        });
    },

    banPlayer: function (objectId) {
        let player = this.players[objectId];
        $.ajax({
            url: "/admin/player/ban/" + player.ServerId + "/" + player.Name,
            dataType: "json"
        });
    },

    getPlayers: function() {
        return this.players;
    },

    emitChange: function () {
        this.emit("change");
    }
});
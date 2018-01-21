var AccountState = {

    Normal: 0,

    Spectator: 1,

    GameMaster: 2,

    GameMasterInvisible: 3,

    Banned: 4,

    TemporarilyBanned: 5,


    getCaption: function (state) {
        switch (state) {
        case AccountState.Normal:
            return "Normal";
        case AccountState.Banned:
            return "Banned";
        case AccountState.GameMaster:
            return "Game Master";
        case AccountState.Spectator:
            return "Spectator";
        case AccountState.GameMasterInvisible:
            return "Invisible GameMaster";
        case AccountState.TemporarilyBanned:
            return "Temporarily Banned";
        }

        return "";
    }
}
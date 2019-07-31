// <copyright file="PlayerState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The states of a player, used by the <see cref="ITrader.PlayerState"/>.
    /// </summary>
    public class PlayerState
    {
        private readonly State tradeButtonPressed;

        private readonly State tradeOpened;

        private readonly State npcDialogOpened;

        private readonly State partyRequest;

        private readonly State guildRequest;

        private PlayerState()
        {
            this.tradeButtonPressed = new State(new Guid("706A9618-1C31-4357-8C1A-0AA71B4E89E9"))
            {
                Name = "Trade Button Pressed",
                PossibleTransitions = new List<State>
                {
                    Disconnected,
                    EnteredWorld,
                },
            };
            this.tradeOpened = new State(new Guid("F4367C99-1F11-4D42-9ADF-AD4655F022A0"))
            {
                Name = "Trade Opened",
                PossibleTransitions = new List<State>
                {
                    Disconnected,
                    Dead,
                    EnteredWorld,
                    this.tradeButtonPressed,
                },
            };
            this.tradeButtonPressed.PossibleTransitions.Add(this.tradeOpened);
            TradeRequested = new State(new Guid("E15623A1-D125-4327-AE6A-9F36C5744BC0"))
            {
                Name = "Trade Requested",
                PossibleTransitions = new List<State>
                {
                    Disconnected,
                    this.tradeOpened,
                    EnteredWorld,
                },
            };
            EnteredWorld.PossibleTransitions.Add(TradeRequested);
            this.npcDialogOpened = new State(new Guid("B0DFD9AC-009C-496C-A1B9-C8D6C6BFE23F"))
            {
                Name = "Monster Dialog Opened",
                PossibleTransitions = new List<State>
                {
                    Disconnected,
                    EnteredWorld,
                },
            };
            EnteredWorld.PossibleTransitions.Add(this.npcDialogOpened);
            this.partyRequest = new State(new Guid("FD96D6EC-88F3-4B0C-ADFB-D5DDF1554C48"))
            {
                Name = "Party Requested",
                PossibleTransitions = new List<State>
                {
                    Disconnected,
                    EnteredWorld,
                },
            };
            EnteredWorld.PossibleTransitions.Add(this.partyRequest);
            this.guildRequest = new State(new Guid("B8C69D28-4CEF-45CE-9D11-37576A97E116"))
            {
                Name = "Guild Requested",
                PossibleTransitions = new List<State>
                {
                    Disconnected,
                    EnteredWorld,
                },
            };
            EnteredWorld.PossibleTransitions.Add(this.guildRequest);

            EnteredWorld.PossibleTransitions.Add(PlayerState.Dead);

            EnteredWorld.PossibleTransitions.Add(PlayerState.CharacterSelection);
        }

        /// <summary>
        /// Gets the finished state. When this state is active, the player session was saved and the player object can be safely removed from the game.
        /// </summary>
        public static State Finished { get; } = new State(new Guid("AB24C7C4-4F37-40ED-B874-0F6C7984C471"))
        {
            Name = "Finished",
        };

        /// <summary>
        /// Gets the disconnected state. When this state is active, the player disconnected from the game, but his session is not saved yet.
        /// </summary>
        public static State Disconnected { get; } = new State(new Guid("DE145F30-9A3A-4895-8B15-4F34F7D203F0"))
        {
            Name = "Disconnected",
            PossibleTransitions = new List<State>
            {
                Finished,
            },
        };

        /// <summary>
        /// Gets the entered world state. When this state is active, the player entered the world with one of his characters.
        /// </summary>
        public static State EnteredWorld { get; } = new State(new Guid("83005EA6-7398-4D21-A51E-1E77B85CF6F0"))
        {
            Name = "Entered World",
            PossibleTransitions = new List<State>
            {
                Disconnected,
            },
        };

        /// <summary>
        /// Gets the dead state. When this state is active, the player has been killed with one of his characters.
        /// </summary>
        public static State Dead { get; } = new State(new Guid("171CBF68-EE19-4C99-8B89-AA8A51AE9876"))
        {
            Name = "Dead",
            PossibleTransitions = new List<State>
            {
                Disconnected,
                EnteredWorld,
            },
        };

        /// <summary>
        /// Gets the character selection state. When this state is active, the character selection screen is shown.
        /// </summary>
        public static State CharacterSelection { get; } = new State(new Guid("6EB2A8EA-1B86-4622-A513-18EB8F3FE512"))
        {
            Name = "Character Selection Screen",
            PossibleTransitions = new List<State>
            {
                Disconnected,
                EnteredWorld,
            },
        };

        /// <summary>
        /// Gets the authenticated state. When this state is active, the player has been successfully authenticated.
        /// </summary>
        public static State Authenticated { get; } = new State(new Guid("4D1D2157-E2BB-48EF-9F19-E1892AC49E84"))
        {
            Name = "Authenticated",
            PossibleTransitions = new List<State>
            {
                Disconnected,
                CharacterSelection,
            },
        };

        /// <summary>
        /// Gets the login screen state. When this state is active, the login screen is shown to the player.
        /// </summary>
        public static State LoginScreen { get; } = new State(new Guid("C59E7072-306F-47BF-A5A0-1C9DA38143C3"))
        {
            Name = "Login Screen",
            PossibleTransitions = new List<State>
            {
                Disconnected,
                Authenticated,
            },
        };

        /// <summary>
        /// Gets the initial state.
        /// </summary>
        public static State Initial { get; } = new State(new Guid("D93BC463-810F-46B9-A9F3-8592A055705D"))
        {
            Name = "Initial State",
            PossibleTransitions = new List<State>
            {
                LoginScreen,
            },
        };

        /// <summary>
        /// Gets the changing map state.
        /// </summary>
        public static State ChangingMap { get; } = new State(new Guid("FF660582-460C-4B69-9D99-F5EB156E83B9"))
        {
            Name = "Changing Map",
            PossibleTransitions = new List<State>
            {
                Disconnected,
                EnteredWorld,
            },
        };

        /// <summary>
        /// Gets the trade requested state. When this state is active, a player got requested by another player to open a trade.
        /// </summary>
        public static State TradeRequested { get; private set; }

        /// <summary>
        /// Gets the trade opened state. When this state is active, the player has a trade going on.
        /// </summary>
        public static State TradeOpened => Instance.tradeOpened;

        /// <summary>
        /// Gets the trade button pressed state. When this state is active, the player pressed its trade button to close his trade.
        /// </summary>
        public static State TradeButtonPressed => Instance.tradeButtonPressed;

        /// <summary>
        /// Gets the Monster dialog opened state. When this state is active, the player has opened a Monster dialog.
        /// </summary>
        public static State NpcDialogOpened => Instance.npcDialogOpened;

        /// <summary>
        /// Gets the party request state. When this state is active, the player has requested for party.
        /// </summary>
        public static State PartyRequest => Instance.partyRequest;

        /// <summary>
        /// Gets the guild request state. When this state is active, the player has requested for guild.
        /// TODO: set this state.
        /// </summary>
        public static State GuildRequest => Instance.guildRequest;

        private static PlayerState Instance { get; } = new PlayerState();
    }
}

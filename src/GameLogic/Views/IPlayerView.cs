// <copyright file="IPlayerView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views
{
    using System.Collections.Generic;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// The type of a chat message.
    /// </summary>
    public enum ChatMessageType
    {
        /// <summary>
        /// A normal chat message which is sent to all observing players.
        /// </summary>
        Normal,

        /// <summary>
        /// A whispered chat message, which only the recipient can read.
        /// </summary>
        Whisper,

        /// <summary>
        /// A chat message which can only be read inside the same party.
        /// </summary>
        Party,

        /// <summary>
        /// A chat message which can only be read inside the same guild.
        /// </summary>
        Guild,

        /// <summary>
        /// A chat message which can only be read inside the same guild alliance.
        /// </summary>
        Alliance,

        /// <summary>
        /// A chat message which can only be read inside the same gens side.
        /// </summary>
        Gens,

        /// <summary>
        /// A command message.
        /// </summary>
        Command
    }

    /// <summary>
    /// Specifies the move type of objects.
    /// </summary>
    public enum MoveType
    {
        /// <summary>
        /// Moving by walking.
        /// </summary>
        Walk,

        /// <summary>
        /// Moving by instantly moving the object on the map, without animations.
        /// </summary>
        Instant
    }

    /// <summary>
    /// Result of a character delete request.
    /// </summary>
    public enum CharacterDeleteResult
    {
        /// <summary>
        /// Deleting was not successful.
        /// </summary>
        Unsuccessful = 0,

        /// <summary>
        /// Deleting was successful.
        /// </summary>
        Successful = 1,

        /// <summary>
        /// Deleting was not successful because a wrong security code was entered.
        /// </summary>
        WrongSecurityCode = 2
    }

    /// <summary>
    /// The result of a login request.
    /// </summary>
    public enum LoginResult : byte
    {
        /// <summary>
        /// The password was wrong.
        /// </summary>
        InvalidPassword = 0,

        /// <summary>
        /// The login succeeded.
        /// </summary>
        OK = 1,

        /// <summary>
        /// The account is invalid.
        /// </summary>
        AccountInvalid = 2,

        /// <summary>
        /// The account is already connected.
        /// </summary>
        AccountAlreadyConnected = 3,

        /// <summary>
        /// The server is full.
        /// </summary>
        ServerIsFull = 4,

        /// <summary>
        /// The account is blocked.
        /// </summary>
        AccountBlocked = 5,

        /// <summary>
        /// The client has a wrong version.
        /// </summary>
        WrongVersion = 6,

        /// <summary>
        /// An error occured during connection.
        /// </summary>
        /// <remarks>I think in the original game server it is the connection to some of the servers behind.</remarks>
        ConnectionError = 7,

        /// <summary>
        /// Connection closed because of three failed login requests.
        /// </summary>
        ConnectionClosed3Fails = 8,

        /// <summary>
        /// There is no payment information.
        /// </summary>
        NoChargeInfo = 9,

        /// <summary>
        /// Subscription term is over.
        /// </summary>
        SubscriptionTermOver = 10,

        /// <summary>
        /// Subscription time is over.
        /// </summary>
        SubscriptionTimeOver = 11,

        /// <summary>
        /// Only players over 15 years are allowed.
        /// </summary>
        OnlyPlayersOver15Yrs = 0x11,

        /// <summary>
        /// The account is temporarily blocked.
        /// </summary>
        TemporaryBlocked = 0x0E,

        /// <summary>
        /// The client connected from a blocked country.
        /// </summary>
        BadCountry = 0xD2
    }

    /// <summary>
    /// The type of the logout.
    /// </summary>
    public enum LogoutType : byte
    {
        /// <summary>
        /// This is sent when the player closes the game.
        /// </summary>
        CloseGame,

        /// <summary>
        /// This is sent by the client when the player wants to go back to the character selection screen.
        /// </summary>
        BackToCharacterSelection,

        /// <summary>
        /// This is sent by the client when the player wants to go back to the server selection screen.
        /// </summary>
        BackToServerSelection
    }

    /// <summary>
    /// The main view of the player.
    /// </summary>
    public interface IPlayerView : IChatView
    {
        /// <summary>
        /// Gets the party view.
        /// </summary>
        IPartyView PartyView { get; }

        /// <summary>
        /// Gets the messenger view.
        /// </summary>
        IMessengerView MessengerView { get; }

        /// <summary>
        /// Gets the trade view.
        /// </summary>
        ITradeView TradeView { get; }

        /// <summary>
        /// Gets the guild view.
        /// </summary>
        IGuildView GuildView { get; }

        /// <summary>
        /// Gets the world view.
        /// </summary>
        IWorldView WorldView { get; }

        /// <summary>
        /// Gets the inventory view.
        /// </summary>
        IInventoryView InventoryView { get; }

        /// <summary>
        /// Shows the character list.
        /// </summary>
        void ShowCharacterList();

        /// <summary>
        /// Shows that the character creation failed.
        /// </summary>
        void ShowCharacterCreationFailed();

        /// <summary>
        /// Shows the created character.
        /// </summary>
        /// <param name="character">The character.</param>
        void ShowCreatedCharacter(Character character);

        /// <summary>
        /// The appearance of a player changed.
        /// </summary>
        /// <param name="changedPlayer">The changed player.</param>
        void AppearanceChanged(Player changedPlayer);

        /// <summary>
        /// Adds the skill to the skill list.
        /// </summary>
        /// <param name="skill">The skill.</param>
        void AddSkill(Skill skill);

        /// <summary>
        /// Removes the skill from the skill list.
        /// </summary>
        /// <param name="skill">The skill.</param>
        void RemoveSkill(Skill skill);

        /// <summary>
        /// Shows the effects of drinking alcohol.
        /// </summary>
        void DrinkAlcohol();

        /// <summary>
        /// Shows that the requested item consumption failed.
        /// </summary>
        void RequestedItemConsumptionFailed();

        /// <summary>
        /// Shows the character delete response result.
        /// </summary>
        /// <param name="result">The result.</param>
        void ShowCharacterDeleteResponse(CharacterDeleteResult result);

        /// <summary>
        /// Updates the current mana and hp bars.
        /// </summary>
        void UpdateCurrentManaAndHp();

        /// <summary>
        /// Updates the current mana.
        /// </summary>
        void UpdateCurrentMana();

        /// <summary>
        /// Updates the current health.
        /// </summary>
        void UpdateCurrentHealth();

        /// <summary>
        /// Updates the maximum mana.
        /// </summary>
        void UpdateMaximumMana();

        /// <summary>
        /// Updates the maximum health.
        /// </summary>
        void UpdateMaximumHealth();

        /// <summary>
        /// Updates the level.
        /// </summary>
        void UpdateLevel();

        /// <summary>
        /// Adds Experience after the object has been killed.
        /// </summary>
        /// <param name="gainedExperience">The experience gain.</param>
        /// <param name="killedObject">The killed object.</param>
        void AddExperience(int gainedExperience, IIdentifiable killedObject);

        /// <summary>
        /// Closes the vault.
        /// </summary>
        void CloseVault();

        /// <summary>
        /// Shows the vault.
        /// </summary>
        void ShowVault();

        /// <summary>
        /// Updates the character stats.
        /// </summary>
        void UpdateCharacterStats();

        /// <summary>
        /// Shows the hit damage over of an object.
        /// </summary>
        /// <param name="hitReceiver">The hit receiver.</param>
        /// <param name="hitInfo">The hit information.</param>
        void ShowHit(IAttackable hitReceiver, HitInfo hitInfo);

        /// <summary>
        /// Activates the magic effect.
        /// </summary>
        /// <param name="effect">The effect.</param>
        /// <param name="affectedPlayer">The affected player.</param>
        void ActivateMagicEffect(MagicEffect effect, Player affectedPlayer);

        /// <summary>
        /// Deactivates the magic effect.
        /// </summary>
        /// <param name="effect">The effect.</param>
        /// <param name="affectedPlayer">The affected player.</param>
        void DeactivateMagicEffect(MagicEffect effect, Player affectedPlayer);

        /// <summary>
        /// Updates the skill list.
        /// </summary>
        void UpdateSkillList();

        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="messageType">Type of the message.</param>
        void ShowMessage(string message, MessageType messageType);

        /// <summary>
        /// Shows the shop item list of the requested players shop.
        /// </summary>
        /// <param name="requestedPlayer">The requested player.</param>
        void ShowShopItemList(Player requestedPlayer);

        /// <summary>
        /// A player opened his shop.
        /// </summary>
        /// <param name="player">The player who opened the shop.</param>
        void PlayerShopOpened(Player player);

        /// <summary>
        /// Shows the shops of players.
        /// </summary>
        /// <param name="playersWithShop">The players with shop.</param>
        void ShowShopsOfPlayers(ICollection<Player> playersWithShop);

        /// <summary>
        /// A player closed his shop.
        /// </summary>
        /// <param name="playerWithClosedShop">Player of closing shop.</param>
        void PlayerShopClosed(Player playerWithClosedShop);

        /// <summary>
        /// Logouts with the specified logout type.
        /// </summary>
        /// <param name="logoutType">Type of the logout.</param>
        void Logout(LogoutType logoutType);

        /// <summary>
        /// A characters has been focused on the character selection screen.
        /// </summary>
        /// <param name="character">The character which has been focused.</param>
        void CharacterFocused(Character character);

        /// <summary>
        /// Shows the stat increase result.
        /// </summary>
        /// <param name="statType">Type of the stat.</param>
        /// <param name="success">if set to <c>true</c> the increment was successful.</param>
        void StatIncreaseResult(AttributeDefinition statType, bool success);

        /// <summary>
        /// Opens the Monster window.
        /// </summary>
        /// <param name="window">The window.</param>
        void OpenNpcWindow(NpcWindow window);

        /// <summary>
        /// Shows the merchant store item list.
        /// </summary>
        /// <param name="storeItems">The store items.</param>
        void ShowMerchantStoreItemList(ICollection<Item> storeItems);

        /// <summary>
        /// Shows the login result.
        /// </summary>
        /// <param name="loginResult">The login result.</param>
        void ShowLoginResult(LoginResult loginResult);

        /// <summary>
        /// Shows the login window.
        /// </summary>
        void ShowLoginWindow();
    }
}

// <copyright file="ChatCommandPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// The base of every chat command plug in.
    /// </summary>
    /// <typeparam name="T">The type of arguments base.</typeparam>
    public abstract class ChatCommandPlugInBase<T> : IChatCommandPlugIn
        where T : ArgumentsBase
    {
        /// <inheritdoc/>
        public abstract string Key { get; }

        /// <inheritdoc/>
        public abstract CharacterStatus MinCharacterStatusRequirement { get; }

        /// <inheritdoc/>
        public virtual void HandleCommand(Player player, string command)
        {
            try
            {
                var arguments = command.ParseArguments<T>();
                this.DoHandleCommand(player, arguments);
            }
            catch (ArgumentException argEx)
            {
                this.ShowMessageTo(player, $"[{this.Key}] {argEx.Message}");
            }
            catch (Exception ex)
            {
                player.Logger.LogError(ex, $"Unexpected error handling the chat command '{this.Key}'.", command);
            }
        }

        /// <summary>
        /// handle the chat command safely.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="arguments">The arguments.</param>
        protected abstract void DoHandleCommand(Player player, T arguments);

        /// <summary>
        /// Shows a message to a player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="message">The message.</param>
        /// <param name="messageType">The message type.</param>
        protected void ShowMessageTo(Player player, string message, MessageType messageType = MessageType.BlueNormal)
        {
            player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage(message, messageType);
        }

        /// <summary>
        /// Gets a player by his character name.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="characterName">The character name.</param>
        /// <returns>The target player.</returns>
        protected Player GetPlayerByCharacterName(Player player, string characterName)
        {
            if (string.IsNullOrWhiteSpace(characterName))
            {
                throw new ArgumentException("Character name is required.");
            }

            return player.GameContext.GetPlayerByCharacterName(characterName)
                ?? throw new ArgumentException($"Character {characterName} not found.");
        }

        /// <summary>
        /// Gets a character's location.
        /// </summary>
        /// <param name="character">The target character.</param>
        /// <returns>An instance of the ExitGate.</returns>
        protected ExitGate GetLocationFrom(Character character)
        {
            return new ExitGate
            {
                Map = character.CurrentMap,
                X1 = character.PositionX,
                X2 = (byte)(character.PositionX + 2),
                Y1 = character.PositionY,
                Y2 = (byte)(character.PositionY + 2),
            };
        }
    }
}

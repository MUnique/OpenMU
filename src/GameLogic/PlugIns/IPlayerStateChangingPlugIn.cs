// <copyright file="IPlayerStateChangingPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin interface which is called when the state of a player is about to be changed.
    /// </summary>
    /// <remarks>
    /// Example cases:
    /// - Player Entered Game
    /// - Player Left Game
    /// - Before saving player data
    /// - Player Authenticated
    /// - Trade finished.
    /// </remarks>
    [Guid("DF9A11EE-6494-48BD-9B04-9413A24DF0D7")]
    [PlugInPoint("Player state changing", "Is called when a player state changes.")]
    public interface IPlayerStateChangingPlugIn
    {
        /// <summary>
        /// Is called when the <see cref="Player.PlayerState" /> is about to change.
        /// The change can be cancelled by setting the cancel flag in the event args.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="eventArgs">The <see cref="StateMachine.StateChangeEventArgs"/> instance containing the event data.</param>
        void PlayerStateChanging(Player player, StateMachine.StateChangeEventArgs eventArgs);
    }
}
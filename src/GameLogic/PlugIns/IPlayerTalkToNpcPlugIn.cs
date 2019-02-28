// <copyright file="IPlayerTalkToNpcPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Plugin interface which is called when a player talks to a npc.
    /// </summary>
    [Guid("95E5446D-10D9-4399-BF98-D32808D2BBD1")]
    [PlugInPoint("Player talks to NPC", "Plugins which will be executed when a player talks to an npc.")]
    public interface IPlayerTalkToNpcPlugIn
    {
        /// <summary>
        /// Is called when the player talks to a NPC.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="npc">The NPC.</param>
        /// <param name="eventArgs">The event args.</param>
        void PlayerTalksToNpc(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs);
    }

    /// <summary>
    /// Event args for the <see cref="IPlayerTalkToNpcPlugIn"/>.
    /// </summary>
    /// <seealso cref="System.ComponentModel.CancelEventArgs" />
    public class NpcTalkEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether talking to the NPC leaves a dialog open on the client.
        /// If that's the case, the clients needs to call a <see cref="CloseNpcDialogAction"/> later.
        /// </summary>
        public bool LeavesDialogOpen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether talking to the NPC has been handled and no further execution of <see cref="IPlayerTalkToNpcPlugIn"/>s is required.
        /// </summary>
        public bool HasBeenHandled
        {
            get => this.Cancel;
            set => this.Cancel = value;
        }
    }
}
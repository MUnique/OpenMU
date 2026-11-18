// <copyright file="NpcTalkEventArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel;
using MUnique.OpenMU.GameLogic.PlayerActions;

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

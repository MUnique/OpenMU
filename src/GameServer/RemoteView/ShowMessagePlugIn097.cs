// <copyright file="ShowMessagePlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameServer.Compatibility;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Message plugin for 0.97 clients which expect latin1 encoded strings.
/// </summary>
[PlugIn("ShowMessagePlugIn 0.97", "Show messages for 0.97 clients with latin1 encoding.")]
[Guid("1DF2A59B-0C95-4D62-80ED-53F2C9E35397")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class ShowMessagePlugIn097 : IShowMessagePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowMessagePlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowMessagePlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowMessageAsync(string message, OpenMU.Interfaces.MessageType messageType)
    {
        await Version097CompatibilityProfile.SendServerMessageAsync(this._player, message, messageType).ConfigureAwait(false);
    }
}

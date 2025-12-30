// <copyright file="ChatViewPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameServer.Compatibility;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Chat plugin for 0.97 clients which expect latin1 encoded strings.
/// </summary>
[PlugIn(nameof(ChatViewPlugIn097), "Chat plugin for 0.97 clients with latin1 encoding.")]
[Guid("2C1A5E6D-8D77-4DA7-A4F3-98B35347F2A2")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class ChatViewPlugIn097 : IChatViewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatViewPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ChatViewPlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ChatMessageAsync(string message, string sender, ChatMessageType type)
    {
        await Version097CompatibilityProfile.SendChatMessageAsync(this._player, message, sender, type).ConfigureAwait(false);
    }
}

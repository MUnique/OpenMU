// <copyright file="ShowAllianceListUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowAllianceListUpdatePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowAllianceListUpdatePlugIn), "The default implementation of the IShowAllianceListUpdatePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("D4E5F678-9012-3456-7890-ABCDEF012345")]
public class ShowAllianceListUpdatePlugIn : IShowAllianceListUpdatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowAllianceListUpdatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowAllianceListUpdatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateAllianceListAsync()
    {
        var guildStatus = this._player.GuildStatus;
        if (guildStatus?.GuildId is not { } guildId)
        {
            return;
        }

        if (this._player.GameContext is not IGameServerContext serverContext)
        {
            return;
        }

        var allianceGuildIds = await serverContext.GuildServer.GetAllianceMemberGuildIdsAsync(guildId).ConfigureAwait(false);
        var allianceGuilds = new List<Interfaces.Guild>();

        foreach (var allianceGuildId in allianceGuildIds)
        {
            var guild = await serverContext.GuildServer.GetGuildAsync(allianceGuildId).ConfigureAwait(false);
            if (guild is not null)
            {
                allianceGuilds.Add(guild);
            }
        }

        await this._player.InvokeViewPlugInAsync<IShowAllianceListPlugIn>(p => p.ShowAllianceListAsync(allianceGuilds)).ConfigureAwait(false);
    }
}

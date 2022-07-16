// <copyright file="GuildWarRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using MUnique.OpenMU.GameLogic.GuildWar;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to request a guild war.
/// </summary>
public class GuildWarRequestAction
{
    /// <summary>
    /// Requests the a guild war at the guild master of the target guild.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="targetGuildName">Name of the target guild.</param>
    public ValueTask RequestWarAsync(Player player, string targetGuildName)
    {
        return this.TryRequestWarAsync(player, targetGuildName, GuildWarType.Normal);
    }

    /// <summary>
    /// Requests the a battle soccer at the guild master of the target guild.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="targetGuildName">Name of the target guild.</param>
    public ValueTask RequestBattleSoccerAsync(Player player, string targetGuildName)
    {
        return this.TryRequestWarAsync(player, targetGuildName, GuildWarType.Soccer);
    }

    private async ValueTask TryRequestWarAsync(Player player, string targetGuildName, GuildWarType guildWarType)
    {
        if (player.GuildStatus is not { } guildStatus
            || player.GameContext is not IGameServerContext serverContext
            || await serverContext.GuildServer.GetGuildAsync(player.GuildStatus.GuildId).ConfigureAwait(false) is not { Name: not null } guild)
        {
            await player.InvokeViewPlugInAsync<IShowShowGuildWarRequestResultPlugIn>(p => p.ShowResultAsync(GuildWarRequestResult.NotInGuild)).ConfigureAwait(false);
            return;
        }

        if (guildStatus.Position != GuildPosition.GuildMaster)
        {
            await player.InvokeViewPlugInAsync<IShowShowGuildWarRequestResultPlugIn>(p => p.ShowResultAsync(GuildWarRequestResult.NotTheGuildMaster)).ConfigureAwait(false);
            return;
        }

        if (!await serverContext.GuildServer.GuildExistsAsync(targetGuildName).ConfigureAwait(false))
        {
            await player.InvokeViewPlugInAsync<IShowShowGuildWarRequestResultPlugIn>(p => p.ShowResultAsync(GuildWarRequestResult.GuildNotFound)).ConfigureAwait(false);
            return;
        }

        var targetGuildId = await serverContext.GuildServer.GetGuildIdByNameAsync(targetGuildName).ConfigureAwait(false);

        Player? targetGuildMaster = null;
        await serverContext.ForEachGuildPlayerAsync(targetGuildId, p =>
        {
            targetGuildMaster = p.GuildStatus?.Position == GuildPosition.GuildMaster ? p : targetGuildMaster;
            return Task.CompletedTask;
        }).ConfigureAwait(false);
        if (targetGuildMaster is null)
        {
            await player.InvokeViewPlugInAsync<IShowShowGuildWarRequestResultPlugIn>(p => p.ShowResultAsync(GuildWarRequestResult.GuildMasterOffline)).ConfigureAwait(false);
            return;
        }

        if (targetGuildMaster.GuildWarContext is not null || player.GuildWarContext is not null)
        {
            await player.InvokeViewPlugInAsync<IShowShowGuildWarRequestResultPlugIn>(p => p.ShowResultAsync(GuildWarRequestResult.AlreadyInWar)).ConfigureAwait(false);
            return;
        }

        var score = new GuildWarScore
        {
            FirstGuildName = targetGuildName,
            SecondGuildName = guild.Name!,
            MaximumScore = (byte)(guildWarType == GuildWarType.Soccer ? 100 : 20),
        };

        targetGuildMaster.GuildWarContext = new GuildWarContext(guildWarType, score, GuildWarTeam.First, player);
        player.GuildWarContext = new GuildWarContext(guildWarType, score, GuildWarTeam.Second, null);

        await targetGuildMaster.InvokeViewPlugInAsync<IShowGuildWarRequestPlugIn>(p => p.ShowRequestAsync(guild.Name!, guildWarType)).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IShowShowGuildWarRequestResultPlugIn>(p => p.ShowResultAsync(GuildWarRequestResult.RequestSentToGuildMaster)).ConfigureAwait(false);
    }
}
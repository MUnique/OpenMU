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
    public void RequestWar(Player player, string targetGuildName)
    {
        this.TryRequestWar(player, targetGuildName, GuildWarType.Normal);
    }

    /// <summary>
    /// Requests the a battle soccer at the guild master of the target guild.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="targetGuildName">Name of the target guild.</param>
    public void RequestBattleSoccer(Player player, string targetGuildName)
    {
        this.TryRequestWar(player, targetGuildName, GuildWarType.Soccer);
    }

    private void TryRequestWar(Player player, string targetGuildName, GuildWarType guildWarType)
    {
        if (player.GuildStatus is not { } guildStatus
            || player.GameContext is not IGameServerContext serverContext
            || serverContext.GuildServer.GetGuild(player.GuildStatus.GuildId) is not { Name: not null } guild)
        {
            player.ViewPlugIns.GetPlugIn<IShowShowGuildWarRequestResultPlugIn>()?.ShowResult(GuildWarRequestResult.NotInGuild);
            return;
        }

        if (guildStatus.Position != GuildPosition.GuildMaster)
        {
            player.ViewPlugIns.GetPlugIn<IShowShowGuildWarRequestResultPlugIn>()?.ShowResult(GuildWarRequestResult.NotTheGuildMaster);
            return;
        }

        if (!serverContext.GuildServer.GuildExists(targetGuildName))
        {
            player.ViewPlugIns.GetPlugIn<IShowShowGuildWarRequestResultPlugIn>()?.ShowResult(GuildWarRequestResult.GuildNotFound);
            return;
        }

        var targetGuildId = serverContext.GuildServer.GetGuildIdByName(targetGuildName);

        Player? targetGuildMaster = null;
        serverContext.ForEachGuildPlayer(targetGuildId, p => targetGuildMaster = p.GuildStatus?.Position == GuildPosition.GuildMaster ? p : targetGuildMaster);
        if (targetGuildMaster is null)
        {
            player.ViewPlugIns.GetPlugIn<IShowShowGuildWarRequestResultPlugIn>()?.ShowResult(GuildWarRequestResult.GuildMasterOffline);
            return;
        }

        if (targetGuildMaster.GuildWarContext is not null || player.GuildWarContext is not null)
        {
            player.ViewPlugIns.GetPlugIn<IShowShowGuildWarRequestResultPlugIn>()?.ShowResult(GuildWarRequestResult.AlreadyInWar);
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

        targetGuildMaster.ViewPlugIns.GetPlugIn<IShowGuildWarRequestPlugIn>()?.ShowRequest(guild.Name!, guildWarType);
        player.ViewPlugIns.GetPlugIn<IShowShowGuildWarRequestResultPlugIn>()?.ShowResult(GuildWarRequestResult.RequestSentToGuildMaster);
    }
}
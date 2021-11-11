﻿// <copyright file="GuildCreateAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// Action to create a guild.
/// </summary>
public class GuildCreateAction
{
    /// <summary>
    /// Creates the guild.
    /// </summary>
    /// <param name="creator">The creator.</param>
    /// <param name="guildName">Name of the guild.</param>
    /// <param name="guildEmblem">The guild emblem.</param>
    public void CreateGuild(Player creator, string guildName, byte[] guildEmblem)
    {
        using var loggerScope = creator.Logger.BeginScope(this.GetType());
        if (creator.PlayerState.CurrentState != PlayerState.EnteredWorld)
        {
            creator.Logger.LogError($"Account {creator.Account?.LoginName} not in the right state, but {creator.PlayerState.CurrentState}.");
            return;
        }

        var guildServer = (creator.GameContext as IGameServerContext)?.GuildServer;
        if (guildServer is null)
        {
            creator.Logger.LogError($"No guild server available");
            return;
        }

        if (guildServer.GuildExists(guildName))
        {
            creator.ViewPlugIns.GetPlugIn<IShowGuildCreateResultPlugIn>()?.ShowGuildCreateResult(GuildCreateErrorDetail.GuildAlreadyExist);
            return;
        }

        creator.GuildStatus = guildServer.CreateGuild(guildName, creator.SelectedCharacter!.Name, creator.SelectedCharacter.Id, guildEmblem, ((IGameServerContext)creator.GameContext).Id);
        if (creator.GuildStatus is null)
        {
            creator.ViewPlugIns.GetPlugIn<IShowGuildCreateResultPlugIn>()?.ShowGuildCreateResult(GuildCreateErrorDetail.GuildAlreadyExist);
            return;
        }

        (creator.GameContext as IGameServerContext)?.RegisterGuildMember(creator);
        creator.ViewPlugIns.GetPlugIn<IShowGuildCreateResultPlugIn>()?.ShowGuildCreateResult(GuildCreateErrorDetail.None);
        creator.ForEachObservingPlayer(p => p.ViewPlugIns.GetPlugIn<IAssignPlayersToGuildPlugIn>()?.AssignPlayerToGuild(creator, false), true);

        creator.Logger.LogInformation("Guild created: [{0}], Master: [{1}]", guildName, creator.SelectedCharacter.Name);
    }
}
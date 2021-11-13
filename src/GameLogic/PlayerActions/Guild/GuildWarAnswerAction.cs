// <copyright file="GuildWarAnswerAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using System.ComponentModel;
using MUnique.OpenMU.GameLogic.GuildWar;
using MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// Action to handle the response of the requested guild master about the guild war.
/// </summary>
public class GuildWarAnswerAction
{
    /// <summary>
    /// Processes the answer.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="isWarAccepted">The answer.</param>
    public void ProcessAnswer(Player player, bool isWarAccepted)
    {
        if (player.GuildWarContext is not { } guildWarContext
            || guildWarContext.Requester is not { } requester)
        {
            player.ViewPlugIns.GetPlugIn<IShowShowGuildWarRequestResultPlugIn>()?.ShowResult(GuildWarRequestResult.GuildNotFound);
            return;
        }

        SoccerGameMap? soccerMap = null;
        if (guildWarContext.WarType == GuildWarType.Soccer
            && player.GameContext.Configuration.Maps.FirstOrDefault(m => m.BattleZone?.Type == BattleType.Soccer) is { } definition)
        {
            soccerMap = (SoccerGameMap?)player.GameContext.GetMap(definition.Number.ToUnsigned());
        }

        var soccerInitFailed = false;
        if (guildWarContext.WarType == GuildWarType.Soccer && (soccerMap is null || soccerMap.IsBattleOngoing))
        {
            soccerInitFailed = true;
            player.ViewPlugIns.GetPlugIn<IShowShowGuildWarRequestResultPlugIn>()?.ShowResult(GuildWarRequestResult.Failed);
        }

        if (!isWarAccepted
            || requester.GuildWarContext is not { } requesterGuildWarContext
            || soccerInitFailed)
        {
            player.GuildWarContext = null;
            requester.GuildWarContext = null;
            return;
        }

        soccerMap?.InitializeBattle();
        guildWarContext.State = GuildWarState.Started;
        requesterGuildWarContext.State = GuildWarState.Started;
        var playerTeam = this.GetTeamPlayers(player);
        var requesterTeam = this.GetTeamPlayers(requester);
        var score = guildWarContext.Score;
        score.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(score.HasEnded))
            {
                guildWarContext.State = GuildWarState.Ended;
                requesterGuildWarContext.State = GuildWarState.Ended;
                if (player.GameContext is IGameServerContext gameContext && score.Winners.HasValue)
                {
                    var winner = score.Winners == player.GuildWarContext.Team ? player.GuildStatus!.GuildId : requester.GuildStatus!.GuildId;
                    gameContext.GuildServer.IncreaseGuildScore(winner);
                }
            }
        };

        foreach (var guildPlayer in playerTeam)
        {
            guildPlayer.GuildWarContext = guildWarContext;
            guildPlayer.ViewPlugIns.GetPlugIn<IShowGuildWarDeclaredPlugIn>()?.ShowDeclared();
            guildPlayer.ViewPlugIns.GetPlugIn<IGuildWarScoreUpdatePlugIn>()?.UpdateScore();
            RegisterScoreChangedEventWeakly(guildPlayer, score, soccerMap);
        }

        foreach (var guildPlayer in requesterTeam)
        {
            guildPlayer.GuildWarContext = requesterGuildWarContext;
            guildPlayer.ViewPlugIns.GetPlugIn<IShowGuildWarDeclaredPlugIn>()?.ShowDeclared();
            guildPlayer.ViewPlugIns.GetPlugIn<IGuildWarScoreUpdatePlugIn>()?.UpdateScore();
            RegisterScoreChangedEventWeakly(guildPlayer, score, soccerMap);
        }

        if (guildWarContext.WarType == GuildWarType.Soccer && soccerMap is { })
        {
            this.MovePartyToArena(player.GuildWarContext.Team, playerTeam, soccerMap);
            this.MovePartyToArena(requester.GuildWarContext.Team, requesterTeam, soccerMap);
            soccerMap.StartBattle(score);
        }
    }

    private static void RegisterScoreChangedEventWeakly(Player guildPlayer, GuildWarScore score, SoccerGameMap? soccerMap)
    {
        var playerReference = new WeakReference<Player>(guildPlayer);

        void OnScorePropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (playerReference.TryGetTarget(out var p))
            {
                OnScoreChanged(score, p, soccerMap, args);
            }
            else
            {
                score.PropertyChanged -= OnScorePropertyChanged;
            }
        }

        score.PropertyChanged += OnScorePropertyChanged;
    }

    private static void OnScoreChanged(GuildWarScore score, Player player, SoccerGameMap? soccerMap, PropertyChangedEventArgs args)
    {
        try
        {
            if (args.PropertyName == nameof(score.HasEnded))
            {
                if (player.GuildWarContext is { } context)
                {
                    var isWinner = context.Team == score.Winners;
                    player.ViewPlugIns.GetPlugIn<IShowGuildWarResultPlugIn>()?.ShowResult(context.EnemyTeamName, isWinner ? GuildWarResult.Won : GuildWarResult.Lost);
                    if (soccerMap is not null)
                    {
                        var spawnGates = soccerMap.Definition.ExitGates.Where(g => g.IsSpawnGate);
                        if (spawnGates.Any())
                        {
                            player.WarpTo(spawnGates.SelectRandom()!);
                        }
                    }

                    player.GuildWarContext = null;
                }
            }
            else
            {
                player.ViewPlugIns.GetPlugIn<IGuildWarScoreUpdatePlugIn>()?.UpdateScore();
            }
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Unexpected error when notifying the player about a guild war score update");
        }
    }

    private ICollection<Player> GetTeamPlayers(Player guildMaster)
    {
        if (guildMaster.Party is { } party)
        {
            return party.PartyList.OfType<Player>().Where(p => p.GuildStatus?.GuildId == guildMaster.GuildStatus?.GuildId).ToList();
        }

        return new List<Player>(1) { guildMaster };
    }

    private void MovePartyToArena(GuildWarTeam team, ICollection<Player> members, SoccerGameMap soccerMap)
    {
        var ground = soccerMap.Definition.BattleZone?.Ground;
        if (ground is null)
        {
            return;
        }

        var increaseX = soccerMap.Definition.BattleZone?.LeftTeamSpawnPointX is not null;

        var exitGate = new ExitGate
        {
            X1 = (team == GuildWarTeam.First ? soccerMap.Definition.BattleZone?.LeftTeamSpawnPointX : soccerMap.Definition.BattleZone?.RightTeamSpawnPointX) ?? ground.X1,
            X2 = (team == GuildWarTeam.First ? soccerMap.Definition.BattleZone?.LeftTeamSpawnPointX : soccerMap.Definition.BattleZone?.RightTeamSpawnPointX) ?? ground.X2,
            Y1 = (team == GuildWarTeam.First ? soccerMap.Definition.BattleZone?.LeftTeamSpawnPointY : soccerMap.Definition.BattleZone?.RightTeamSpawnPointY) ?? ground.Y1,
            Y2 = (team == GuildWarTeam.First ? soccerMap.Definition.BattleZone?.LeftTeamSpawnPointY : soccerMap.Definition.BattleZone?.RightTeamSpawnPointY) ?? ground.Y2,
            Map = soccerMap.Definition,
        };

        foreach (var member in members)
        {
            member.WarpTo(exitGate);
            if (increaseX)
            {
                exitGate.X1++;
                exitGate.X2++;
            }
        }
    }
}
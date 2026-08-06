// <copyright file="CastleSiegeRegisterGuildAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Validates and processes Castle Siege guild registrations.
/// </summary>
public class CastleSiegeRegisterGuildAction
{
    /// <summary>
    /// Tries to register the player's guild or alliance for Castle Siege.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context, if initialized.</param>
    /// <returns>A task which represents the asynchronous operation.</returns>
    public async ValueTask RegisterAsync(Player player, CastleSiegeContext? context)
    {
        var (result, guildName) = await RegisterCoreAsync(player, context).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ICastleSiegeRegistrationResultPlugIn>(
            view => view.ShowRegistrationResultAsync(result, guildName)).ConfigureAwait(false);
    }

    private static async ValueTask<(CastleSiegeRegistrationResult Result, string GuildName)> RegisterCoreAsync(
        Player player,
        CastleSiegeContext? context)
    {
        if (context is not { Configuration.Enabled: true })
        {
            return (CastleSiegeRegistrationResult.Failed, string.Empty);
        }

        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (context.CurrentState != CastleSiegeState.RegisterGuild)
            {
                return (CastleSiegeRegistrationResult.NotRegistrationPeriod, string.Empty);
            }

            var (guild, resolutionResult) = await CastleSiegeGuildResolver.ResolveAuthorizedGuildAsync(player).ConfigureAwait(false);
            if (guild is null)
            {
                return (resolutionResult, string.Empty);
            }

            var combinedLevel = player.Level + (int)(player.Attributes?[Stats.MasterLevel] ?? 0);
            if (combinedLevel < context.Configuration.RegisterMinLevel)
            {
                return (CastleSiegeRegistrationResult.LevelInsufficient, guild.Name);
            }

            if (player.GameContext is not IGameServerContext gameServerContext)
            {
                return (CastleSiegeRegistrationResult.InvalidGuild, guild.Name);
            }

            var members = await gameServerContext.GuildServer.GetGuildListAsync(guild.RuntimeId).ConfigureAwait(false);
            if (members.Count < context.Configuration.RegisterMinMembers)
            {
                return (CastleSiegeRegistrationResult.NotEnoughMembers, guild.Name);
            }

            if (context.RegisteredGuilds.ContainsKey(guild.PersistentId))
            {
                return (CastleSiegeRegistrationResult.AlreadyRegistered, guild.Name);
            }

            if (context.SiegeData.OwnerGuildId == guild.PersistentId)
            {
                return (CastleSiegeRegistrationResult.IsDefender, guild.Name);
            }

            await context.AddRegistrationAsync(guild.PersistentId, guild.Name).ConfigureAwait(false);
            return (CastleSiegeRegistrationResult.Success, guild.Name);
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }
}

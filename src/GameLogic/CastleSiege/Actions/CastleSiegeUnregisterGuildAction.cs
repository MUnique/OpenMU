// <copyright file="CastleSiegeUnregisterGuildAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Validates and processes Castle Siege guild unregistrations.
/// </summary>
public class CastleSiegeUnregisterGuildAction
{
    /// <summary>
    /// Tries to unregister the player's guild or alliance.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context, if initialized.</param>
    /// <param name="isGivingUp">Whether the guild requests to give up its registration.</param>
    /// <returns>A task which represents the asynchronous operation.</returns>
    public async ValueTask UnregisterAsync(
        Player player,
        CastleSiegeContext? context,
        bool isGivingUp)
    {
        var (result, guildName) = await UnregisterCoreAsync(player, context).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ICastleSiegeRegistrationResultPlugIn>(
            view => view.ShowUnregistrationResultAsync(result, isGivingUp, guildName)).ConfigureAwait(false);
    }

    private static async ValueTask<(CastleSiegeUnregistrationResult Result, string GuildName)> UnregisterCoreAsync(
        Player player,
        CastleSiegeContext? context)
    {
        if (context is not { Configuration.Enabled: true })
        {
            return (CastleSiegeUnregistrationResult.Failed, string.Empty);
        }

        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (context.CurrentState != CastleSiegeState.RegisterGuild)
            {
                return (CastleSiegeUnregistrationResult.WrongState, string.Empty);
            }

            var (guild, _) = await CastleSiegeGuildResolver.ResolveAuthorizedGuildAsync(player).ConfigureAwait(false);
            if (guild is null)
            {
                // The client protocol has no separate NoGuild or InvalidGuild result for unregistration.
                return (CastleSiegeUnregistrationResult.Failed, string.Empty);
            }

            if (!context.RegisteredGuilds.TryGetValue(guild.PersistentId, out var registration))
            {
                return (CastleSiegeUnregistrationResult.NotRegistered, guild.Name);
            }

            await context.RemoveRegistrationAsync(registration).ConfigureAwait(false);
            return (CastleSiegeUnregistrationResult.Success, guild.Name);
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }
}

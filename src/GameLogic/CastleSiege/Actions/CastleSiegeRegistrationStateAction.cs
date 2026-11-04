// <copyright file="CastleSiegeRegistrationStateAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Returns the Castle Siege registration state visible to a player.
/// </summary>
public class CastleSiegeRegistrationStateAction
{
    /// <summary>
    /// Sends the player's guild or alliance registration state.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context, if initialized.</param>
    /// <returns>A task which represents the asynchronous operation.</returns>
    public async ValueTask ShowStateAsync(Player player, CastleSiegeContext? context)
    {
        var (result, guildName, marks, registrationRank) = await GetStateAsync(player, context).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ICastleSiegeRegistrationStatePlugIn>(
            view => view.ShowRegistrationStateAsync(result, guildName, marks, false, registrationRank)).ConfigureAwait(false);
    }

    private static async ValueTask<(
        CastleSiegeRegistrationStateResult Result,
        string GuildName,
        int Marks,
        byte RegistrationRank)> GetStateAsync(
        Player player,
        CastleSiegeContext? context)
    {
        if (context is not { Configuration.Enabled: true })
        {
            return (CastleSiegeRegistrationStateResult.Unavailable, string.Empty, 0, 0);
        }

        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            var guildId = await CastleSiegeGuildResolver.ResolveRegistrationGuildIdAsync(player).ConfigureAwait(false);
            if (guildId is null
                || !context.RegisteredGuilds.TryGetValue(guildId.Value, out var registration))
            {
                return (CastleSiegeRegistrationStateResult.NotRegistered, string.Empty, 0, (byte)0);
            }

            // MuMain carries the rank in one byte, so later registrations use the highest representable rank.
            var registrationRank = (byte)Math.Min(registration.RegistrationOrder, byte.MaxValue);
            return (
                CastleSiegeRegistrationStateResult.Registered,
                registration.GuildName,
                registration.Marks,
                registrationRank);
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }
}

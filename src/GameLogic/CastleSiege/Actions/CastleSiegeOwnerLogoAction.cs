// <copyright file="CastleSiegeOwnerLogoAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Returns the guild logo of the castle owner, which the client requests when it enters the Castle Siege map.
/// </summary>
public class CastleSiegeOwnerLogoAction
{
    /// <summary>
    /// Sends the guild logo of the castle owner to the player.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context, if initialized.</param>
    /// <returns>A task which represents the asynchronous operation.</returns>
    public async ValueTask ShowOwnerLogoAsync(Player player, CastleSiegeContext? context)
    {
        // An empty logo still has to be sent: it replaces the logo of a previous owner which the client may still show.
        var logo = await GetOwnerLogoAsync(context).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ICastleSiegeOwnerLogoPlugIn>(view => view.ShowOwnerLogoAsync(logo)).ConfigureAwait(false);
    }

    private static async ValueTask<ReadOnlyMemory<byte>> GetOwnerLogoAsync(CastleSiegeContext? context)
    {
        if (context is not { Configuration.Enabled: true, IsInitialized: true }
            || context.GameContext is not IGameServerContext gameServerContext)
        {
            return ReadOnlyMemory<byte>.Empty;
        }

        Guid? ownerGuildId;
        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            ownerGuildId = context.SiegeData.IsOccupied ? context.SiegeData.OwnerGuildId : null;
        }
        finally
        {
            context.ExecutionLock.Release();
        }

        if (ownerGuildId is not { } persistentGuildId)
        {
            return ReadOnlyMemory<byte>.Empty;
        }

        var guildServer = gameServerContext.GuildServer;
        var runtimeGuildId = await guildServer.GetGuildIdAsync(persistentGuildId).ConfigureAwait(false);
        if (runtimeGuildId == 0)
        {
            return ReadOnlyMemory<byte>.Empty;
        }

        var guild = await guildServer.GetGuildAsync(runtimeGuildId).ConfigureAwait(false);
        return guild?.Logo ?? ReadOnlyMemory<byte>.Empty;
    }
}

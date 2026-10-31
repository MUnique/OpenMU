// <copyright file="CastleSiegeContextTestExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.CastleSiege;

/// <summary>
/// Helpers to access the selected guilds of a <see cref="CastleSiegeContext"/> in tests.
/// </summary>
internal static class CastleSiegeContextTestExtensions
{
    /// <summary>
    /// Gets the selected guild with the specified runtime identifier and fails the test if there is none.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="runtimeGuildId">The runtime guild identifier.</param>
    /// <returns>The selected guild.</returns>
    public static CastleSiegeGuildParticipant GetSelectedGuild(this CastleSiegeContext context, uint runtimeGuildId)
    {
        return context.GetFinalGuild(runtimeGuildId)
               ?? throw new AssertionException($"The guild {runtimeGuildId} is not selected.");
    }

    /// <summary>
    /// Adds a guild to the selected guilds, keyed by its persistent identifier.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="guild">The guild to add.</param>
    public static void AddSelectedGuild(this CastleSiegeContext context, CastleSiegeGuildParticipant guild)
    {
        context.FinalGuildList[guild.PersistentGuildId] = guild;
    }
}

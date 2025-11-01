// <copyright file="IShowCastleSiegeRegisteredGuildsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Interface for showing the list of registered guilds for castle siege.
/// </summary>
public interface IShowCastleSiegeRegisteredGuildsPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the list of registered guilds/alliances for castle siege.
    /// </summary>
    /// <param name="registeredGuilds">The list of registered guilds with their information.</param>
    ValueTask ShowRegisteredGuildsAsync(IEnumerable<(Guild Guild, int MarksSubmitted)> registeredGuilds);
}

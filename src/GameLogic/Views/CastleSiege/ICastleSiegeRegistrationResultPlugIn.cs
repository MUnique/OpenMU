// <copyright file="ICastleSiegeRegistrationResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// A view which reports Castle Siege guild registration results.
/// </summary>
public interface ICastleSiegeRegistrationResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows a guild registration result.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="guildName">The guild name.</param>
    /// <returns>A task which represents the asynchronous operation.</returns>
    ValueTask ShowRegistrationResultAsync(CastleSiegeRegistrationResult result, string guildName);

    /// <summary>
    /// Shows a guild unregistration result.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="isGivingUp">Whether the guild requested to give up its registration.</param>
    /// <param name="guildName">The guild name.</param>
    /// <returns>A task which represents the asynchronous operation.</returns>
    ValueTask ShowUnregistrationResultAsync(
        CastleSiegeUnregistrationResult result,
        bool isGivingUp,
        string guildName);
}

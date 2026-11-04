// <copyright file="ICastleSiegeRegistrationStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// A view which reports the registration state of a guild.
/// </summary>
public interface ICastleSiegeRegistrationStatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the registration state.
    /// </summary>
    /// <param name="result">The query result.</param>
    /// <param name="guildName">The registered guild name, or an empty string.</param>
    /// <param name="marks">The number of submitted Signs of Lord.</param>
    /// <param name="isGivingUp">Whether the guild gave up its registration.</param>
    /// <param name="registrationRank">The registration rank, or zero when not registered.</param>
    /// <returns>A task which represents the asynchronous operation.</returns>
    ValueTask ShowRegistrationStateAsync(
        CastleSiegeRegistrationStateResult result,
        string guildName,
        int marks,
        bool isGivingUp,
        byte registrationRank);
}

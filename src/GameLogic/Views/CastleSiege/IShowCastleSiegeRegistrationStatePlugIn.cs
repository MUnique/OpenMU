// <copyright file="IShowCastleSiegeRegistrationStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Interface for showing the castle siege registration state to the player.
/// </summary>
public interface IShowCastleSiegeRegistrationStatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the registration state including guild marks submitted.
    /// </summary>
    /// <param name="isRegistered">Whether the player's alliance is registered for the siege.</param>
    /// <param name="totalMarksSubmitted">The total number of guild marks submitted by the alliance.</param>
    ValueTask ShowRegistrationStateAsync(bool isRegistered, int totalMarksSubmitted);
}

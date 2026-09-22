// <copyright file="ICastleSiegeHuntZoneGuardInfoPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// A view which shows the Land of Trials guardsman dialog.
/// </summary>
public interface ICastleSiegeHuntZoneGuardInfoPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the available actions and the entrance fee which applies to the player.
    /// </summary>
    /// <param name="accessType">The player's access type.</param>
    /// <param name="isPublic">Whether public access is enabled.</param>
    /// <param name="currentPrice">The configured entrance fee.</param>
    /// <param name="maximumPrice">The maximum entrance fee.</param>
    /// <param name="priceStep">The fee adjustment step.</param>
    ValueTask ShowHuntZoneGuardInfoAsync(
        CastleSiegeHuntZoneAccessType accessType,
        bool isPublic,
        int currentPrice,
        int maximumPrice,
        int priceStep);
}

// <copyright file="WarpInfoExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Extension methods for <see cref="WarpInfo"/>: who may use a warp. The <see cref="WarpAction"/>,
/// the bots and the summon skill decide it the same way.
/// </summary>
public static class WarpInfoExtensions
{
    /// <summary>
    /// Determines whether the player reaches the level requirement of the warp, which is reduced for some
    /// classes (see <see cref="CharacterExtensions.GetEffectiveMoveLevelRequirement"/>). The plain
    /// character level counts, not the master level.
    /// </summary>
    /// <param name="warpInfo">The warp.</param>
    /// <param name="player">The player.</param>
    /// <param name="levelRequirement">The level which the player needs.</param>
    /// <returns><c>True</c>, if the player reaches the level.</returns>
    public static bool IsLevelReachedBy(this WarpInfo warpInfo, Player player, out int levelRequirement)
    {
        levelRequirement = player.SelectedCharacter?.GetEffectiveMoveLevelRequirement(warpInfo.LevelRequirement) ?? warpInfo.LevelRequirement;
        return levelRequirement <= (player.Attributes?[Stats.Level] ?? 0);
    }

    /// <summary>
    /// Gets the first of the <see cref="WarpInfo.Requirements"/> which the player doesn't fulfill.
    /// </summary>
    /// <param name="warpInfo">The warp.</param>
    /// <param name="player">The player.</param>
    /// <returns>The missed requirement, or <c>null</c> if the player fulfills all of them.</returns>
    public static AttributeRequirement? GetMissingRequirement(this WarpInfo warpInfo, Player player)
    {
        return warpInfo.Requirements.GetMissingRequirement(player);
    }

    /// <summary>
    /// Determines whether the player reaches the level of the warp and fulfills its <see cref="WarpInfo.Requirements"/>.
    /// The requirements of the target map, the costs and whether the warp is only for game masters aren't part of it.
    /// </summary>
    /// <param name="warpInfo">The warp.</param>
    /// <param name="player">The player.</param>
    /// <returns><c>True</c>, if the player may use the warp.</returns>
    public static bool IsAllowedFor(this WarpInfo warpInfo, Player player)
    {
        return warpInfo.IsLevelReachedBy(player, out _) && warpInfo.GetMissingRequirement(player) is null;
    }
}

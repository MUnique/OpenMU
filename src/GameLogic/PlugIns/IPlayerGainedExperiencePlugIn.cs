// <copyright file="IPlayerGainedExperiencePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called after a player gained experience for a kill.
/// </summary>
[Guid("D5A0F2F1-3C1E-4E60-9E8B-59A6EF8A0C21")]
[PlugInPoint("Player gained experience", "Plugins which are executed after a player gained experience for a kill.")]
public interface IPlayerGainedExperiencePlugIn
{
    /// <summary>
    /// Is called after the player gained experience for a kill.
    /// </summary>
    /// <param name="player">The player which gained the experience.</param>
    /// <param name="experience">The gained experience.</param>
    /// <param name="killedObject">The killed object which caused the experience gain.</param>
    /// <param name="isMasterExperience">If set to <c>true</c>, master experience was gained.</param>
    ValueTask PlayerGainedExperienceAsync(Player player, int experience, IAttackable killedObject, bool isMasterExperience);
}

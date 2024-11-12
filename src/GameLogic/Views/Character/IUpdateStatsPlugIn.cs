// <copyright file="IUpdateStatsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character;

using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// Interface of a view whose implementation informs about updated stats.
/// </summary>
public interface IUpdateStatsPlugIn : IViewPlugIn
{
    /// <summary>
    /// Updates the maximum stats.
    /// </summary>
    /// <param name="updatedStats">The updated stats.</param>
    ValueTask UpdateMaximumStatsAsync(UpdatedStats updatedStats = UpdatedStats.Health | UpdatedStats.Mana | UpdatedStats.Speed);

    /// <summary>
    /// Updates the current stats.
    /// </summary>
    /// <param name="updatedStats">The updated stats.</param>
    ValueTask UpdateCurrentStatsAsync(UpdatedStats updatedStats = UpdatedStats.Health | UpdatedStats.Mana | UpdatedStats.Speed);

    /// <summary>
    /// The updated stat.
    /// This might be replaced by the actual <see cref="AttributeDefinition"/> in the future.
    /// </summary>
    [Flags]
    public enum UpdatedStats
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined,

        /// <summary>
        /// The health or shield changed.
        /// </summary>
        Health = 0x01,

        /// <summary>
        /// The mana or ability changed.
        /// </summary>
        Mana = 0x02,

        /// <summary>
        /// The attack speed changed.
        /// </summary>
        Speed = 0x04,
    }
}
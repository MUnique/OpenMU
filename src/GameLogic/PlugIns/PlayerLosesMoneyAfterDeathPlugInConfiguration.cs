// <copyright file="PlayerLosesMoneyAfterDeathPlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel.DataAnnotations;
using MUnique.OpenMU.DataModel.Composition;

/// <summary>
/// Configuration for the <see cref="PlayerLosesMoneyAfterDeathPlugIn"/>.
/// </summary>
public class PlayerLosesMoneyAfterDeathPlugInConfiguration
{
    /// <summary>
    /// Gets or sets the losses per level.
    /// </summary>
    /// <value>
    /// The losses per level.
    /// </value>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    public ICollection<LossOfLevel> LossesPerLevel { get; set; } = new List<LossOfLevel>();

    /// <summary>
    /// Defines the losses for a specific level range.
    /// </summary>
    public class LossOfLevel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LossOfLevel"/> class.
        /// </summary>
        public LossOfLevel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LossOfLevel"/> class.
        /// </summary>
        /// <param name="minimumLevel">The minimum level.</param>
        /// <param name="isMaster">If set to <c>true</c>, this entry only applies to master characters.</param>
        /// <param name="inventoryMoneyLossPercentage">The money loss percentage for the inventory.</param>
        /// <param name="vaultMoneyLossPercentage">The money loss percentage for the vault.</param>
        public LossOfLevel(int minimumLevel, bool isMaster, double inventoryMoneyLossPercentage, double vaultMoneyLossPercentage)
        {
            this.MinimumLevel = minimumLevel;
            this.IsMaster = isMaster;
            this.InventoryMoneyLossPercentage = inventoryMoneyLossPercentage;
            this.VaultMoneyLossPercentage = vaultMoneyLossPercentage;
        }

        /// <summary>
        /// Gets or sets the minimum level.
        /// </summary>
        /// <value>
        /// The minimum level.
        /// </value>
        public int MinimumLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance applies only to master characters.
        /// </summary>
        public bool IsMaster { get; set; }

        /// <summary>
        /// Gets or sets the money loss percentage of the inventory.
        /// </summary>
        public double InventoryMoneyLossPercentage { get; set; }

        /// <summary>
        /// Gets or sets the money loss percentage of the vault.
        /// </summary>
        public double VaultMoneyLossPercentage { get; set; }
    }
}
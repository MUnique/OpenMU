// <copyright file="PlayerLosesExperienceAfterDeathPlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel.DataAnnotations;
using MUnique.OpenMU.DataModel.Composition;

/// <summary>
/// Configuration for the <see cref="QuestMonsterKillCountPlugInConfiguration"/>.
/// </summary>
public class PlayerLosesExperienceAfterDeathPlugInConfiguration
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
        /// <param name="losses">The losses.</param>
        public LossOfLevel(int minimumLevel, bool isMaster, List<LossPerStage> losses)
        {
            this.MinimumLevel = minimumLevel;
            this.IsMaster = isMaster;
            this.Losses = losses;
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
        /// Gets or sets the loss per <see cref="HeroState"/>.
        /// </summary>
        [MemberOfAggregate]
        [ScaffoldColumn(true)]
        public ICollection<LossPerStage> Losses { get; set; } = [];
    }

    /// <summary>
    /// Defines the experience loss for a specific <see cref="HeroState"/>.
    /// </summary>
    public class LossPerStage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LossPerStage"/> class.
        /// </summary>
        public LossPerStage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LossPerStage"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="experienceLossPercentage">The experience loss percentage.</param>
        public LossPerStage(HeroState state, double experienceLossPercentage)
        {
            this.State = state;
            this.ExperienceLossPercentage = experienceLossPercentage;
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public HeroState State { get; set; }

        /// <summary>
        /// Gets or sets the experience loss percentage.
        /// </summary>
        public double ExperienceLossPercentage { get; set; }
    }
}
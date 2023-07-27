// <copyright file="RecoverConsumeHandlerConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.ComponentModel;
using MUnique.OpenMU.DataModel.Composition;

/// <summary>
/// The configuration of a <see cref="RecoverConsumeHandlerPlugIn"/>.
/// </summary>
public class RecoverConsumeHandlerConfiguration
{
    /// <summary>
    /// Gets or sets the total recover percentage.
    /// </summary>
    public double TotalRecoverPercentage { get; set; }

    /// <summary>
    /// Gets or sets the recover percentage increase by potion level.
    /// </summary>
    public double RecoverPercentageIncreaseByPotionLevel { get; set; }

    /// <summary>
    /// Gets or sets the recover delay reduction by potion level.
    /// A value between 0 and 1 (exclusive).
    /// 1 would mean that the recover works instantly since level 1.
    /// </summary>
    public double RecoverDelayReductionByPotionLevel { get; set; }

    /// <summary>
    /// Gets or sets the value which is additionally recovered.
    /// From this value, the character level is subtracted.
    /// </summary>
    public int AdditionalRecoverMinusCharacterLevel { get; set; }

    /// <summary>
    /// Gets or sets the cooldown time for the next consumption.
    /// </summary>
    public TimeSpan CooldownTime { get; set; }

    /// <summary>
    /// Gets or sets the recover steps. If none are defined, the recover happens
    /// instantly.
    /// </summary>
    [Browsable(false)] // They cannot be edited on the admin panel yet.
    [MemberOfAggregate]
    public IList<RecoverStep> RecoverSteps { get; set; } = new List<RecoverStep>();
}
﻿// <copyright file="CombinationBonusRequirement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Defines a requirement of existing item options on the equipped items of a character.
/// </summary>
public class CombinationBonusRequirement
{
    /// <summary>
    /// Gets or sets the required <see cref="ItemOption.OptionType"/>.
    /// </summary>
    public virtual ItemOptionType? OptionType { get; set; }

    /// <summary>
    /// Gets or sets the required <see cref="ItemOption.SubOptionType"/>.
    /// </summary>
    public int SubOptionType { get; set; }

    /// <summary>
    /// Gets or sets the minimum count of options in order to fulfill the requirement.
    /// </summary>
    public int MinimumCount { get; set; } = 1;
}
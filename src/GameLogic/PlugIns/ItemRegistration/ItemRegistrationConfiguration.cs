// <copyright file="ItemRegistrationConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ItemRegistration;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MUnique.OpenMU.DataModel.Composition;

/// <summary>
/// Configuration for the item registration feature.
/// </summary>
public class ItemRegistrationConfiguration
{
    /// <summary>
    /// Gets or sets the list of item registration rules for NPCs.
    /// </summary>
    [Display(Name = "NPC Registration Rules", Description = "Configure item registration rules for different NPCs.")]
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    public ICollection<NpcItemRegistrationRule> Rules { get; set; } = new List<NpcItemRegistrationRule>
    {
        new NpcItemRegistrationRule
        {
            NpcNumber = 236, // Golden Archer
            AcceptedItemGroup = 14,
            AcceptedItemNumber = 21, // Rena
            RequiredItemsCount = 1,
            RewardZen = 5000000,

            // No RewardDropItemGroup by default - select the DropItemGroup for Rena in the admin panel to enable item drops.
        },
    };
}

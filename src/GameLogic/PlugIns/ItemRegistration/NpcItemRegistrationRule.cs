// <copyright file="NpcItemRegistrationRule.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ItemRegistration;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Rule defining item registration settings for a specific NPC.
/// </summary>
public class NpcItemRegistrationRule
{
    /// <summary>
    /// Gets or sets the NPC definition number.
    /// </summary>
    [Display(Name = "NPC Number", Description = "The NPC definition number (e.g. 236 for Golden Archer).")]
    public short NpcNumber { get; set; } = 236;

    /// <summary>
    /// Gets or sets the item group of the accepted item. Default is 14.
    /// </summary>
    [Display(Name = "Accepted Item Group", Description = "Group of the accepted item (e.g. 14 for event items).")]
    public byte AcceptedItemGroup { get; set; } = 14;

    /// <summary>
    /// Gets or sets the item number of the accepted item. Default is 21 (Rena).
    /// </summary>
    [Display(Name = "Accepted Item Number", Description = "Number of the accepted item (e.g. 21 for Rena).")]
    public short AcceptedItemNumber { get; set; } = 21;

    /// <summary>
    /// Gets or sets the number of items required to register to receive a reward. Default is 1.
    /// </summary>
    [Display(Name = "Required Items Count", Description = "Number of items required to receive a reward.")]
    public int RequiredItemsCount { get; set; } = 1;

    /// <summary>
    /// Gets or sets the Zen reward amount when the required count is reached. Default is 5,000,000.
    /// </summary>
    [Display(Name = "Reward Zen", Description = "Zen awarded upon reaching the required item count.")]
    public int RewardZen { get; set; } = 5000000;

    /// <summary>
    /// Gets or sets the description of the <see cref="MUnique.OpenMU.DataModel.Configuration.DropItemGroup"/> used to generate the item reward drop.
    /// </summary>
    [Display(Name = "Reward Drop Item Group Description", Description = "Description of the DropItemGroup (e.g. Golden Archer Reward). The DropItemGroup determines drop chances and items. If empty, no item drops (Zen reward only).")]
    public string? RewardDropItemGroupDescription { get; set; }
}

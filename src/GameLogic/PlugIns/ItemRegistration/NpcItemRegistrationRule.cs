// <copyright file="NpcItemRegistrationRule.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ItemRegistration;

using System.ComponentModel.DataAnnotations;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Rule defining item registration settings for a specific NPC.
/// </summary>
public class NpcItemRegistrationRule
{
    /// <summary>
    /// Gets or sets the NPC definition number.
    /// </summary>
    /// <remarks>
    /// Defaults to 0 (an invalid, unassigned NPC number) rather than an existing NPC's number.
    /// Otherwise, a freshly-added rule would look like a valid duplicate of that NPC's rule
    /// until an admin edits it, and <c>Rules.FirstOrDefault(r => r.NpcNumber == npcNumber)</c>
    /// lookups could silently pick the wrong one of the two.
    /// </remarks>
    [Display(Name = "NPC Number", Description = "The NPC definition number (e.g. 236 for Golden Archer).")]
    [Range(1, short.MaxValue, ErrorMessage = "Please set a valid NPC number.")]
    public short NpcNumber { get; set; }

    /// <summary>
    /// Gets or sets the item group of the accepted item.
    /// </summary>
    [Display(Name = "Accepted Item Group", Description = "Group of the accepted item (e.g. 14 for event items).")]
    [Range(0, 15, ErrorMessage = "Please set a valid item group.")]
    public byte AcceptedItemGroup { get; set; }

    /// <summary>
    /// Gets or sets the item number of the accepted item.
    /// </summary>
    [Display(Name = "Accepted Item Number", Description = "Number of the accepted item (e.g. 21 for Rena).")]
    [Range(0, short.MaxValue, ErrorMessage = "Please set a valid item number.")]
    public short AcceptedItemNumber { get; set; }

    /// <summary>
    /// Gets or sets the number of items required to register to receive a reward. Default is 1.
    /// </summary>
    [Display(Name = "Required Items Count", Description = "Number of items required to receive a reward.")]
    [Range(1, int.MaxValue, ErrorMessage = "At least 1 item must be required.")]
    public int RequiredItemsCount { get; set; } = 1;

    /// <summary>
    /// Gets or sets the Zen reward amount when the required count is reached.
    /// </summary>
    [Display(Name = "Reward Zen", Description = "Zen awarded upon reaching the required item count.")]
    [Range(0, int.MaxValue, ErrorMessage = "Reward Zen can't be negative.")]
    public int RewardZen { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DropItemGroup"/> used to generate the item reward drop.
    /// </summary>
    [Display(Name = "Reward Drop Item Group", Description = "The DropItemGroup used to generate the item reward drop. If empty, no item drops (Zen reward only).")]
    public virtual DropItemGroup? RewardDropItemGroup { get; set; }
}

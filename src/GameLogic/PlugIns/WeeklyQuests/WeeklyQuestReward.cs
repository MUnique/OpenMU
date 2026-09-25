// <copyright file="WeeklyQuestReward.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// A reward of a <see cref="WeeklyQuestDefinition"/>.
/// </summary>
public class WeeklyQuestReward
{
    /// <summary>
    /// Gets or sets the type of the reward.
    /// </summary>
    [Display(Name = "Tipo")]
    public WeeklyQuestRewardType RewardType { get; set; }

    /// <summary>
    /// Gets or sets the amount: zen or experience points, or the number of items.
    /// </summary>
    [Display(Name = "Cantidad", Description = "Zen, puntos de experiencia o cantidad de items.")]
    [Range(1, int.MaxValue)]
    public int Amount { get; set; } = 1;

    /// <summary>
    /// Gets or sets the item, for <see cref="WeeklyQuestRewardType.Item"/>.
    /// </summary>
    [Display(Name = "Item", Description = "Solo para premios de tipo Item.")]
    public virtual ItemDefinition? Item { get; set; }

    /// <summary>
    /// Gets or sets the level of the item.
    /// </summary>
    [Display(Name = "Nivel del item")]
    [Range(0, 15)]
    public byte ItemLevel { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item has its skill.
    /// </summary>
    [Display(Name = "Skill")]
    public bool Skill { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item has luck.
    /// </summary>
    [Display(Name = "Luck")]
    public bool Luck { get; set; }

    /// <summary>
    /// Gets or sets the level of the additional option of the item (0 to 4).
    /// </summary>
    [Display(Name = "Nivel de opción adicional", Description = "0 a 4.")]
    [Range(0, 4)]
    public byte OptionLevel { get; set; }

    /// <summary>
    /// Gets or sets the excellent options of the item as bit mask, like the /item command:
    /// 1, 2, 4, 8, 16, 32 for the first to sixth option. 63 = all options.
    /// </summary>
    [Display(Name = "Opciones excelentes", Description = "Máscara de bits como en /item: 1, 2, 4, 8, 16, 32. 63 = todas.")]
    [Range(0, 63)]
    public byte ExcellentOptions { get; set; }

    /// <inheritdoc />
    public override string ToString() => this.RewardType == WeeklyQuestRewardType.Item
        ? $"{this.Amount}x {this.Item?.Name ?? "?"} +{this.ItemLevel}"
        : $"{this.Amount} {this.RewardType}";
}

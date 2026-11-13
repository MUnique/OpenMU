// <copyright file="ShopItemExport.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages.MerchantShop;

/// <summary>
/// Serializable representation of an item of a merchant shop.
/// </summary>
public sealed record ShopItemExport
{
    /// <summary>
    /// Gets or sets the item slot in the shop.
    /// </summary>
    public byte Slot { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition.Group"/>
    /// of the item's definition.
    /// </summary>
    public byte ItemGroup { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition.Number"/>
    /// of the item's definition.
    /// </summary>
    public short ItemNumber { get; set; }

    /// <summary>
    /// Gets or sets the item level.
    /// </summary>
    public byte Level { get; set; }

    /// <summary>
    /// Gets or sets the remaining durability, or the amount of a stackable item.
    /// </summary>
    public double Durability { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item has its skill.
    /// </summary>
    public bool HasSkill { get; set; }

    /// <summary>
    /// Gets or sets the number of sockets of the item.
    /// </summary>
    public int SocketCount { get; set; }

    /// <summary>
    /// Gets or sets the experience of a trainable pet item.
    /// </summary>
    public int PetExperience { get; set; }

    /// <summary>
    /// Gets or sets the item options, e.g. excellent, normal, harmony or socket options.
    /// </summary>
    public ICollection<ShopItemOptionExport> Options { get; set; } = new List<ShopItemOptionExport>();

    /// <summary>
    /// Gets or sets the names of the ancient item sets the item belongs to.
    /// </summary>
    public ICollection<string> AncientSets { get; set; } = new List<string>();
}

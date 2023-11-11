// <copyright file="ItemCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.ItemCrafting;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Description of IItemCrafting.
/// </summary>
[Cloneable]
public partial class ItemCrafting
{
    /// <summary>
    /// Gets or sets the number.
    /// </summary>
    /// <remarks>
    /// Referenced by the client with this number.
    /// </remarks>
    public byte Number { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the item crafting handler class.
    /// </summary>
    public string ItemCraftingHandlerClassName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the simple crafting settings.
    /// </summary>
    [MemberOfAggregate]
    public virtual SimpleCraftingSettings? SimpleCraftingSettings { get; set; }
}
// <copyright file="ItemCraftingHandlerFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;

/// <summary>
/// Creates the item crafting handlers by their type name, see <see cref="ItemCrafting.ItemCraftingHandlerClassName"/>.
/// </summary>
internal static partial class ItemCraftingHandlerFactory
{
    /// <summary>
    /// Creates the item crafting handler of the specified type, which is implemented by a code generator.
    /// </summary>
    /// <param name="typeName">The full name of the type of the handler.</param>
    /// <param name="settings">The settings, which are passed to the constructor of a <see cref="SimpleItemCraftingHandler"/>.</param>
    /// <returns>The created handler; <c>null</c>, if the type is unknown.</returns>
    [TypeNameFactory]
    public static partial IItemCraftingHandler? Create(string typeName, SimpleCraftingSettings? settings);
}

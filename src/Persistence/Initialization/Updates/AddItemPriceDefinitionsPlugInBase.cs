// <copyright file="AddItemPriceDefinitionsPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// This update adds the <see cref="ItemPriceDefinition"/>s and assigns them to the items.
/// The prices of the special items were hard-coded in the item price calculator before, which now requires them.
/// </summary>
/// <remarks>
/// It's mandatory, because without it, special items like jewels would be priced like usual equipment.
/// It only assigns definitions to items which don't have one yet, so it doesn't override customizations.
/// </remarks>
public abstract class AddItemPriceDefinitionsPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add item price definitions";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the price definitions of special items like jewels, potions and event tickets, which were hard-coded before. Without it, these items are priced like usual equipment.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 3, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        new ItemPriceDefinitions(context, gameConfiguration).Initialize();
        return ValueTask.CompletedTask;
    }
}

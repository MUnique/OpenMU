// <copyright file="AddProtectionTalismansUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the Talisman of Guardian (13/81), which protects from the death penalty,
/// and the Talisman of Item Protection (13/82), which keeps the durability of the equipped items.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddProtectionTalismansUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddProtectionTalismansUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("CDE78D33-BDAF-4E0B-BEA4-29DC58F5D72A")]
public class AddProtectionTalismansUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add the protection talismans";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the Talisman of Guardian, which protects from the death penalty, and the Talisman of Item Protection, which keeps the durability of the equipped items.";

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 09, 8, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The talismans are created by the <see cref="BuffItems"/> initializer, like the items of the <see cref="AddBuffItemsUpdatePlugIn"/>,
    /// which also adds the attributes of their effects.
    /// </remarks>
    public override IEnumerable<UpdateDependency> DependsOn => [typeof(AddBuffItemsUpdatePlugIn).GUID];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var addedItems = new BuffItems(context, gameConfiguration).CreateItems();
        ItemRules.Apply(addedItems);
        return ValueTask.CompletedTask;
    }
}

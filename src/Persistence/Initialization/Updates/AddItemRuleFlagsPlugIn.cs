// <copyright file="AddItemRuleFlagsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update sets the new item rule flags (<see cref="ItemDefinition.IsTradable"/>,
/// <see cref="ItemDefinition.IsDroppable"/>, <see cref="ItemDefinition.IsStorable"/>,
/// <see cref="ItemDefinition.IsSellableToNpc"/>, <see cref="ItemDefinition.IsPersonalStoreSellable"/>
/// and <see cref="ItemDefinition.IsRepairable"/>) on the existing Season 6 items, see <see cref="ItemRules"/>.
/// The database migration adds them allowing everything.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("C0F83AEA-C8E1-4E40-9B85-D344B7F93238")]
public class AddItemRuleFlagsPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add item rule flags";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update sets which items can't be traded, dropped, stored, sold to NPCs, offered in personal stores or repaired, matching the client item data.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddItemRuleFlags;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 26, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        ItemRules.Apply(gameConfiguration);
        return ValueTask.CompletedTask;
    }
}

// <copyright file="AddSmallWingsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the missing small wings and capes (Small Cape of Lord, Small Wing of Curse, Small Wings of Elf,
/// Small Wings of Heaven, Small Wings of Satan and Little Warrior's Cloak), which are sold in the in-game shop.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddSmallWingsUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddSmallWingsUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("53749584-DF06-4022-9B10-6F7313D3563A")]
public class AddSmallWingsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add small wings and capes";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the missing small wings and capes of the in-game shop: Small Cape of Lord, Small Wing of Curse, Small Wings of Elf, Small Wings of Heaven, Small Wings of Satan and Little Warrior's Cloak. Existing items with the same numbers are left unchanged.";

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 02, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The wings get the movement speed attributes and the item rule flags, so these have to exist first.
    /// </remarks>
    public override IEnumerable<Guid> DependsOn => [typeof(AddMovementSpeedAttributesPlugInSeason6).GUID, typeof(AddItemRuleFlagsPlugIn).GUID];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var addedItems = new Wings(context, gameConfiguration).AddSmallWings();
        ItemRules.Apply(addedItems);
        return ValueTask.CompletedTask;
    }
}

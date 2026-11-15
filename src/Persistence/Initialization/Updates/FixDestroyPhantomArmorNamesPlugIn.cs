// <copyright file="FixDestroyPhantomArmorNamesPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Linq.Expressions;
using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update gives the armor sets 47 and 48 their names: 47 is Phantom and 48 is Destroy.
/// </summary>
/// <remarks>
/// The sets were initialized with each other's name, and three pieces of the set 47 were spelled "Destory".
/// The client (e.g. its english, korean and japanese item data) calls the set 47 Phantom and the set 48 Destroy.
/// The update also renames the defense bonuses of these sets, which are named after them.
/// Names which were changed in the configuration are kept.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.FixDestroyPhantomArmorNamesPlugIn_Name), Description = nameof(PlugInResources.FixDestroyPhantomArmorNamesPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("435DBA26-4732-4763-A349-FC1033D8BE32")]
public class FixDestroyPhantomArmorNamesPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Destroy and Phantom Armor Names";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Swaps the names of the Phantom (47) and Destroy (48) armor sets and of their set bonuses, which were initialized with each other's name, and corrects the spelling \"Destory\".";

    private const byte PhantomNumber = 47;

    private const byte DestroyNumber = 48;

    private const string PreviousPhantomSetName = "Destory";

    private const string PreviousDestroySetName = "Phantom";

    private static readonly IReadOnlyList<(byte Group, byte Number, string PreviousName, Expression<Func<string>> Name)> Pieces =
    [
        ((byte)ItemGroups.Armor, PhantomNumber, "Destory Armor", () => ItemNames.PhantomArmor),
        ((byte)ItemGroups.Pants, PhantomNumber, "Destory Pants", () => ItemNames.PhantomPants),
        ((byte)ItemGroups.Gloves, PhantomNumber, "Destroy Gloves", () => ItemNames.PhantomGloves),
        ((byte)ItemGroups.Boots, PhantomNumber, "Destory Boots", () => ItemNames.PhantomBoots),
        ((byte)ItemGroups.Armor, DestroyNumber, "Phantom Armor", () => ItemNames.DestroyArmor),
        ((byte)ItemGroups.Pants, DestroyNumber, "Phantom Pants", () => ItemNames.DestroyPants),
        ((byte)ItemGroups.Gloves, DestroyNumber, "Phantom Gloves", () => ItemNames.DestroyGloves),
        ((byte)ItemGroups.Boots, DestroyNumber, "Phantom Boots", () => ItemNames.DestroyBoots),
    ];

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 11, 20, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var piece in Pieces)
        {
            if (gameConfiguration.Items.FirstOrDefault(item => item.Group == piece.Group && item.Number == piece.Number) is { } item
                && item.Name.ValueInNeutralLanguage == piece.PreviousName)
            {
                item.Name = LocalizedString.FromResource(piece.Name);
            }
        }

        RenameSetBonuses(gameConfiguration, PhantomNumber, PreviousPhantomSetName);
        RenameSetBonuses(gameConfiguration, DestroyNumber, PreviousDestroySetName);
        return ValueTask.CompletedTask;
    }

    private static void RenameSetBonuses(GameConfiguration gameConfiguration, byte number, string previousSetName)
    {
        foreach (var set in gameConfiguration.ItemSetGroups)
        {
            var pieces = set.Items
                .Select(item => item.ItemDefinition)
                .OfType<ItemDefinition>()
                .OrderBy(item => item.Group)
                .ToList();
            if (pieces.Count == 0 || pieces.Any(item => item.Number != number || item.Group is < (byte)ItemGroups.Helm or > (byte)ItemGroups.Boots))
            {
                continue;
            }

            byte? level = set.SetLevel > 0 ? (byte)set.SetLevel : null;
            var previousName = level is { } setLevel
                ? $"{previousSetName} Defense Bonus (Level {setLevel})"
                : previousSetName + " Defense Rate Bonus";
            if (set.Name.ValueInNeutralLanguage == previousName)
            {
                set.Name = ArmorInitializerBase.GetSetName(pieces, level);
            }
        }
    }
}

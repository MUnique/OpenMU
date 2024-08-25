// <copyright file="FixSocketSeedCraftingUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update sets the right settings for the socket seed crafting.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("C802EFC2-1D42-4218-871E-8886D115F3ED")]
public class FixSocketSeedCraftingUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fixed socket seed crafting";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update sets the right settings for the socket seed crafting.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixSocketSeedCrafting;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 08, 25, 15, 00, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var seedMaster = gameConfiguration.Monsters.FirstOrDefault(m => m.NpcWindow == NpcWindow.SeedMaster);
        if (seedMaster is null)
        {
            return;
        }

        var seedCrafting = seedMaster.ItemCraftings.FirstOrDefault(c => c.Number == 42);
        if (seedCrafting?.SimpleCraftingSettings is not { } craftingSettings)
        {
            return;
        }

        var option = gameConfiguration.ItemOptionTypes.First(o => object.Equals(o, ItemOptionTypes.Option));
        foreach (var requirement in craftingSettings.RequiredItems)
        {
            requirement.RequiredItemOptions.Remove(option);
        }
    }
}

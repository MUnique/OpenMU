// <copyright file="FixWingsAndCapesCraftingsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update sets the right settings for the wings and capes craftings.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("618A53AF-ED2A-4C78-A103-BAD061FFB0D2")]
public class FixWingsAndCapesCraftingsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fixed wings and capes craftings";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update sets the right settings for the wings and capes craftings.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixWingsAndCapesCraftings;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 08, 14, 8, 00, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var chaosGoblin = gameConfiguration.Monsters.FirstOrDefault(m => m.NpcWindow == NpcWindow.ChaosMachine);
        if (chaosGoblin is null)
        {
            return;
        }

        FixSecondWingCrafting(chaosGoblin);
        FixFlameOfCondorCrafting(chaosGoblin, gameConfiguration);
        FixCapeOfFighterCrafting(chaosGoblin, gameConfiguration);
    }

    private static void FixCapeOfFighterCrafting(MonsterDefinition chaosGoblin, GameConfiguration gameConfiguration)
    {
        var capeCrafting = chaosGoblin.ItemCraftings.FirstOrDefault(c => c.Number == 24);
        if (capeCrafting is null)
        {
            return;
        }

        if (gameConfiguration.Items.FirstOrDefault(item => item.Group == 13 && item.Number == 49) is not { } oldScroll)
        {
            return;
        }

        if (gameConfiguration.Items.FirstOrDefault(item => item.Group == 12 && item.Number == 49) is not { } capeOfFighter)
        {
            return;
        }

        if (capeCrafting.SimpleCraftingSettings?.ResultItems.FirstOrDefault(it => object.Equals(it.ItemDefinition, oldScroll)) is { } resultItem)
        {
            resultItem.ItemDefinition = capeOfFighter;
        }
    }

    private static void FixFlameOfCondorCrafting(MonsterDefinition chaosGoblin, GameConfiguration gameConfiguration)
    {
        var flameOfCondorCrafting = chaosGoblin.ItemCraftings.FirstOrDefault(c => c.Number == 38);
        if (flameOfCondorCrafting is null)
        {
            return;
        }

        var requiredItems = flameOfCondorCrafting.SimpleCraftingSettings?.RequiredItems
            .FirstOrDefault(it => it.PossibleItems.Any(item => item.Group == 12 && item.Number == 3));

        if (requiredItems is null)
        {
            return;
        }

        if (gameConfiguration.Items.FirstOrDefault(item => item.Group == 12 && item.Number == 49) is { } capeOfFighter
            && !requiredItems.PossibleItems.Any(it => object.Equals(it, capeOfFighter)))
        {
            requiredItems.PossibleItems.Add(capeOfFighter);
        }

        if (gameConfiguration.Items.FirstOrDefault(item => item.Group == 13 && item.Number == 30) is { } capeOfLord
            && !requiredItems.PossibleItems.Any(it => object.Equals(it, capeOfLord)))
        {
            requiredItems.PossibleItems.Add(capeOfLord);
        }
    }

    private static void FixSecondWingCrafting(MonsterDefinition chaosGoblin)
    {
        var secondWingsCrafting = chaosGoblin.ItemCraftings.FirstOrDefault(c => c.Number == 7);
        if (secondWingsCrafting is null)
        {
            return;
        }

        var requiredItems = secondWingsCrafting.SimpleCraftingSettings?.RequiredItems
            .FirstOrDefault(it => it.PossibleItems.Any(item => item.Group == 12 && item.Number == 0));

        if (requiredItems is null)
        {
            return;
        }

        requiredItems.MinimumItemLevel = 0;
    }
}
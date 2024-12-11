// <copyright file="FixChaosMixesPlugIn095D.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes the Chaos Weapon, First Wings, Dinorant, and Item Level Upgrade craftings' settings.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("68BC1F35-FC9A-468F-89FB-0940485AC107")]
public class FixChaosMixesPlugIn095D : FixChaosMixesPlugInBase
{
    /// <summary>
    /// The plug in description.
    /// </summary>
    private new const string PlugInDescription = "This update fixes the Chaos Weapon, First Wings, Dinorant, and Item Level Upgrade crafting settings.";

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixChaosMixes095d;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);

        var craftings = gameConfiguration.Monsters.First(m => m.NpcWindow == NpcWindow.ChaosMachine).ItemCraftings;
        this.ApplyFirstWingsCraftingUpdate(craftings);
        this.ApplyDinorantCraftingUpdate(craftings);
        this.ApplyDinorantOptionsUpdate(gameConfiguration);

        // Item Level Upgrade craftings
        int[] itemLevelUpgradeCraftingNos = [3, 4];

        for (int i = 0; i < itemLevelUpgradeCraftingNos.Length; i++)
        {
            if (craftings.Single(c => c.Number == itemLevelUpgradeCraftingNos[i])?.SimpleCraftingSettings is { } craftingSettings)
            {
                craftingSettings.Money = 2_000_000 * (10 + i - 9);
                craftingSettings.SuccessPercent = (byte)(10 + i == 10 ? 50 : 45);
                craftingSettings.SuccessPercentageAdditionForLuck = 25;
                craftingSettings.SuccessPercentageAdditionForExcellentItem = 0;
                craftingSettings.SuccessPercentageAdditionForAncientItem = 0;
                craftingSettings.SuccessPercentageAdditionForSocketItem = 0;

                foreach (var item in craftingSettings.RequiredItems)
                {
                    item.FailResult = MixResult.Disappear;
                    if (item.MaximumAmount == 0)
                    {
                        item.MaximumAmount = item.MinimumAmount;
                    }
                }
            }
        }
    }
}
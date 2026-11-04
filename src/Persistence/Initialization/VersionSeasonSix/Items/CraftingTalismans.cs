// <copyright file="CraftingTalismans.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// Initializer for the talismans of the in-game shop which are put into a crafting, and their requirements
/// in the craftings which accept them:
/// <list type="bullet">
///   <item>The Talisman of Luck increases the success rate of a crafting by one percent per durability,
///   up to the maximum rate of the crafting. Up to <see cref="TalismanOfLuckMaximumDurability"/> can be put in.</item>
///   <item>The Talisman of Chaos Assembly keeps the item of a +10 to +15 item upgrade, when the upgrade fails.</item>
/// </list>
/// </summary>
/// <remarks>
/// The craftings which accept them are the ones of the client (Data\Local\mix.bmd, the charm options of the recipes),
/// as far as they're configured by <see cref="SimpleCraftingSettings"/>.
/// The craftings of the event tickets and the Fenrir upgrade are defined in code and don't accept them.
/// Existing items and requirements aren't changed or added twice, so it can be used by an update, too.
/// The item rules are applied by the <see cref="ItemRules"/>.
/// </remarks>
internal class CraftingTalismans : InitializerBase
{
    /// <summary>
    /// The item group of the talismans.
    /// </summary>
    internal const byte Group = 14;

    /// <summary>
    /// The item number of the Talisman of Luck.
    /// </summary>
    internal const short TalismanOfLuckNumber = 53;

    /// <summary>
    /// The item number of the Talisman of Chaos Assembly.
    /// </summary>
    internal const short TalismanOfChaosAssemblyNumber = 96;

    /// <summary>
    /// The maximum durability of a Talisman of Luck, which is also the maximum sum of durability which can be put into
    /// one crafting. Each durability adds one percent. The client accepts talismans with a total durability of up to 10.
    /// </summary>
    internal const byte TalismanOfLuckMaximumDurability = 10;

    /// <summary>
    /// The numbers of the craftings of the chaos machine which accept the Talisman of Luck: the item upgrades
    /// from +10 to +15, the fruits, the wings and capes, the Fenrir stages, the shield potions and the 3rd wings.
    /// </summary>
    private static readonly byte[] ChaosMachineLuckCraftings = [3, 4, 22, 23, 49, 50, 6, 7, 11, 24, 25, 26, 27, 30, 31, 32, 38, 39];

    /// <summary>
    /// The numbers of the craftings of the pet trainer which accept the Talisman of Luck: the Dark Horse and the Dark Raven.
    /// </summary>
    private static readonly byte[] PetTrainerLuckCraftings = [13, 14];

    /// <summary>
    /// The numbers of the craftings of the chaos machine which accept the Talisman of Chaos Assembly: the item upgrades from +10 to +15.
    /// </summary>
    private static readonly byte[] ChaosAssemblyCraftings = [3, 4, 22, 23, 49, 50];

    /// <summary>
    /// Initializes a new instance of the <see cref="CraftingTalismans"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public CraftingTalismans(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        this.CreateTalismans();
    }

    /// <summary>
    /// Creates the talismans, if they don't exist yet, and adds them to the craftings which accept them.
    /// </summary>
    /// <returns>The talismans.</returns>
    internal IReadOnlyList<ItemDefinition> CreateTalismans()
    {
        var talismanOfLuck = this.GetOrCreateItem(TalismanOfLuckNumber, LocalizedString.FromResource(() => ItemNames.TalismanOfLuck), TalismanOfLuckMaximumDurability);
        var talismanOfChaosAssembly = this.GetOrCreateItem(TalismanOfChaosAssemblyNumber, LocalizedString.FromResource(() => ItemNames.TalismanOfChaosAssembly), 1);

        foreach (var crafting in this.GetCraftings(NpcWindow.ChaosMachine, ChaosMachineLuckCraftings)
                     .Concat(this.GetCraftings(NpcWindow.PetTrainer, PetTrainerLuckCraftings)))
        {
            this.AddRequirement(crafting, talismanOfLuck, TalismanOfLuckMaximumDurability, 1, MixResult.Disappear);
        }

        foreach (var crafting in this.GetCraftings(NpcWindow.ChaosMachine, ChaosAssemblyCraftings))
        {
            this.AddRequirement(crafting, talismanOfChaosAssembly, 1, 0, MixResult.DisappearAndProtectReferencedItems);
        }

        return [talismanOfLuck, talismanOfChaosAssembly];
    }

    private IEnumerable<SimpleCraftingSettings> GetCraftings(NpcWindow npcWindow, byte[] numbers)
    {
        return this.GameConfiguration.Monsters
            .Where(monster => monster.NpcWindow == npcWindow)
            .SelectMany(monster => monster.ItemCraftings)
            .Where(crafting => numbers.Contains(crafting.Number))
            .Select(crafting => crafting.SimpleCraftingSettings)
            .OfType<SimpleCraftingSettings>()
            .Distinct();
    }

    private void AddRequirement(SimpleCraftingSettings settings, ItemDefinition talisman, byte maximumAmount, byte addPercentage, MixResult failResult)
    {
        if (settings.RequiredItems.Any(requiredItem => requiredItem.PossibleItems.Contains(talisman)))
        {
            return;
        }

        var requirement = this.Context.CreateNew<ItemCraftingRequiredItem>();
        requirement.PossibleItems.Add(talisman);
        requirement.MinimumAmount = 0;
        requirement.MaximumAmount = maximumAmount;
        requirement.AddPercentage = addPercentage;
        requirement.SuccessResult = MixResult.Disappear;
        requirement.FailResult = failResult;
        settings.RequiredItems.Add(requirement);
    }

    private ItemDefinition GetOrCreateItem(short number, LocalizedString name, byte durability)
    {
        if (this.GameConfiguration.Items.FirstOrDefault(item => item.Group == Group && item.Number == number) is { } existing)
        {
            return existing;
        }

        var item = this.Context.CreateNew<ItemDefinition>();
        item.Group = Group;
        item.Number = number;
        item.Name = name;
        item.Width = 1;
        item.Height = 1;
        item.Durability = durability;
        item.SetGuid(item.Group, item.Number);
        this.GameConfiguration.Items.Add(item);
        return item;
    }
}

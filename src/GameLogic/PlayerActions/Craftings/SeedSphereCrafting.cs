// <copyright file="SeedSphereCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Crafting for seed spheres.
/// Creates the corresponding seed sphere for the given sphere and seed,
/// sets the option of the seed and the level of the sphere to the resulting seed sphere.
/// </summary>
public class SeedSphereCrafting : SimpleItemCraftingHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeedSphereCrafting"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public SeedSphereCrafting(SimpleCraftingSettings settings)
        : base(settings)
    {
    }

    /// <summary>
    /// Gets the reference to the affected sphere which must be specified in the <see cref="ItemCraftingRequiredItem.Reference"/>.
    /// </summary>
    public static byte SphereReference { get; } = 0x66;

    /// <summary>
    /// Gets the reference to the affected seed which must be specified in the <see cref="ItemCraftingRequiredItem.Reference"/>.
    /// </summary>
    public static byte SeedReference { get; } = 0x77;

    /// <inheritdoc />
    protected override async ValueTask<List<Item>> CreateOrModifyResultItemsAsync(IList<CraftingRequiredItemLink> requiredItems, Player player, byte socketSlot, byte successRate)
    {
        var seed = requiredItems.Single(i => i.ItemRequirement.Reference == SeedReference).Items.Single();
        var sphere = requiredItems.Single(i => i.ItemRequirement.Reference == SphereReference).Items.Single();
        seed.ThrowNotInitializedProperty(seed.Definition is null, nameof(seed.Definition));
        sphere.ThrowNotInitializedProperty(sphere.Definition is null, nameof(sphere.Definition));

        // The following is a bit "magic", because it implicitly relies on the item data and how its built up.
        // Because it has some structure, we can calculate which number the resulting seed sphere will have.
        const int seedTypes = 6; // There are 6 different kind of seeds
        const int seedNumberStart = 60;
        const int sphereNumberStart = 70;
        const int seedSphereNumberStart = 100;

        var sphereLevel = sphere.Definition.Number - sphereNumberStart;
        var resultSphereNumber = seedSphereNumberStart
                                 + (seed.Definition.Number - seedNumberStart)
                                 + (sphereLevel * seedTypes);

        var result = player.PersistenceContext.CreateNew<Item>();
        result.Definition = player.GameContext.Configuration.Items.Single(i => i.Number == resultSphereNumber && i.Group == 12);
        result.Level = seed.Level; // The level defines the kind of option

        await player.TemporaryStorage!.AddItemAsync(result).ConfigureAwait(false);
        return new List<Item> { result };
    }
}
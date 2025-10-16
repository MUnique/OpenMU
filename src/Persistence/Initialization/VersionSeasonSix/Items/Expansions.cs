// Copyright (c) MUnique. Licensed under the MIT license.

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Initialization for Inventory and Vault Expansion items used by Wandering Merchant Zyro.
/// </summary>
public class Expansions : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Expansions"/> class.
    /// </summary>
    public Expansions(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        // Defaults are section 14, numbers 91/92 to avoid collisions with existing initializers.
        this.GameConfiguration.Items.Add(this.CreateVaultExtensionItem(91));
        this.GameConfiguration.Items.Add(this.CreateInventoryExpansionItem(92));

        // Quest materials used by Zyro quests (numbers can be adjusted if needed).
        this.GameConfiguration.Items.Add(this.CreateQuestMaterial(93, "Magic Cloth"));
        this.GameConfiguration.Items.Add(this.CreateQuestMaterial(94, "Space Cloth"));
        this.GameConfiguration.Items.Add(this.CreateQuestMaterial(95, "Ancient Figurine"));
    }

    private ItemDefinition CreateVaultExtensionItem(byte number)
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        item.Name = "Vault Extension";
        item.Number = number;
        item.Group = 14;
        item.Durability = 1;
        item.Width = 1;
        item.Height = 1;
        item.DropsFromMonsters = false;
        item.StorageLimitPerCharacter = 1; // effectively per account; limits farming
        item.IsBoundToCharacter = true;
        item.SetGuid(item.Group, item.Number);
        return item;
    }

    private ItemDefinition CreateQuestMaterial(byte number, string name)
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        item.Name = name;
        item.Number = number;
        item.Group = 14;
        item.Durability = 1;
        item.Width = 1;
        item.Height = 1;
        item.DropsFromMonsters = false; // enabled only during quest via dedicated drop group
        item.IsBoundToCharacter = true;
        item.StorageLimitPerCharacter = 1;
        item.SetGuid(item.Group, item.Number);
        return item;
    }

    private ItemDefinition CreateInventoryExpansionItem(byte number)
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        item.Name = "Inventory Expansion";
        item.Number = number;
        item.Group = 14;
        item.Durability = 1;
        item.Width = 1;
        item.Height = 1;
        item.DropsFromMonsters = false;
        item.StorageLimitPerCharacter = 4; // 4 extensions max
        item.IsBoundToCharacter = true;
        item.SetGuid(item.Group, item.Number);
        return item;
    }
}

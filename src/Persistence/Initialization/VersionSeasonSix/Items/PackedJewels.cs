// <copyright file="PackedJewels.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Class which contains item definitions for packed jewels.
/// </summary>
public class PackedJewels : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PackedJewels"/> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public PackedJewels(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        this.GameConfiguration.Items.Add(this.CreateJewelOfBless());
        this.GameConfiguration.Items.Add(this.CreateJewelOfSoul());
        this.GameConfiguration.Items.Add(this.CreateJewelOfLife());
        this.GameConfiguration.Items.Add(this.CreateJewelOfCreation());
        this.GameConfiguration.Items.Add(this.CreateJewelOfGuardian());
        this.GameConfiguration.Items.Add(this.CreateJewelOfHarmony());
        this.GameConfiguration.Items.Add(this.CreateGemstone());
        this.GameConfiguration.Items.Add(this.CreateJewelOfChaos());
        this.GameConfiguration.Items.Add(this.CreateLowerRefineStone());
        this.GameConfiguration.Items.Add(this.CreateHigherRefineStone());
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Packed Jewel of Bless'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Packed Jewel of Bless'.</returns>
    private ItemDefinition CreateJewelOfBless()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Packed Jewel of Bless";
        itemDefinition.Number = 30;
        itemDefinition.Group = 12;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.MaximumItemLevel = 2;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        return itemDefinition;
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Packed Jewel of Soul'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Packed Jewel of Soul'.</returns>
    private ItemDefinition CreateJewelOfSoul()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Packed Jewel of Soul";
        itemDefinition.Number = 31;
        itemDefinition.Group = 12;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.MaximumItemLevel = 2;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        return itemDefinition;
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Packed Jewel of Life'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Packed Jewel of Life'.</returns>
    private ItemDefinition CreateJewelOfLife()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Packed Jewel of Life";
        itemDefinition.Number = 136;
        itemDefinition.Group = 12;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.MaximumItemLevel = 2;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        return itemDefinition;
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Packed Jewel of Creation'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Packed Jewel of Creation'.</returns>
    private ItemDefinition CreateJewelOfCreation()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Packed Jewel of Creation";
        itemDefinition.Number = 137;
        itemDefinition.Group = 12;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.MaximumItemLevel = 2;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        return itemDefinition;
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Packed Jewel of Guardian'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Packed Jewel of Guardian'.</returns>
    private ItemDefinition CreateJewelOfGuardian()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Packed Jewel of Guardian";
        itemDefinition.Number = 138;
        itemDefinition.Group = 12;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.MaximumItemLevel = 2;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        return itemDefinition;
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Packed Gemstone'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Packed Gemstone'.</returns>
    private ItemDefinition CreateGemstone()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Packed Gemstone";
        itemDefinition.Number = 139;
        itemDefinition.Group = 12;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.MaximumItemLevel = 2;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        return itemDefinition;
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Packed Jewel of Harmony'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Packed Jewel of Harmony'.</returns>
    private ItemDefinition CreateJewelOfHarmony()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Packed Jewel of Harmony";
        itemDefinition.Number = 140;
        itemDefinition.Group = 12;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.MaximumItemLevel = 2;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        return itemDefinition;
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Packed Jewel of Chaos'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Packed Jewel of Chaos'.</returns>
    private ItemDefinition CreateJewelOfChaos()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Packed Jewel of Chaos";
        itemDefinition.Number = 141;
        itemDefinition.Group = 12;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.MaximumItemLevel = 2;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        return itemDefinition;
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Packed Lower refine stone'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Packed Lower refine stone'.</returns>
    private ItemDefinition CreateLowerRefineStone()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Packed Lower refine stone";
        itemDefinition.Number = 142;
        itemDefinition.Group = 12;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.MaximumItemLevel = 2;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        return itemDefinition;
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Packed Higher refine stone'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Packed Higher refine stone'.</returns>
    private ItemDefinition CreateHigherRefineStone()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Packed Higher refine stone";
        itemDefinition.Number = 143;
        itemDefinition.Group = 12;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.MaximumItemLevel = 2;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        return itemDefinition;
    }
}
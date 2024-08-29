// <copyright file="Jewels.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Class which contains item definitions for jewels for season 6.
/// </summary>
public class Jewels : Version097d.Items.Jewels
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Jewels"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Jewels(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        this.GameConfiguration.Items.Add(this.CreateJewelOfGuardian());
        this.GameConfiguration.Items.Add(this.CreateJewelOfHarmony());
        this.GameConfiguration.Items.Add(this.CreateGemstone());
        this.GameConfiguration.Items.Add(this.CreateLowerRefineStone());
        this.GameConfiguration.Items.Add(this.CreateHigherRefineStone());
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Jewel of Guardian'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Jewel of Guardian'.</returns>
    private ItemDefinition CreateJewelOfGuardian()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Jewel of Guardian";
        itemDefinition.Number = 31;
        itemDefinition.Group = 14;
        itemDefinition.DropsFromMonsters = false;
        itemDefinition.DropLevel = 75;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);

        return itemDefinition;
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Gemstone'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Gemstone'.</returns>
    private ItemDefinition CreateGemstone()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Gemstone";
        itemDefinition.Number = 41;
        itemDefinition.Group = 14;
        itemDefinition.DropsFromMonsters = false;
        itemDefinition.DropLevel = 150;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.Value = 25;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        return itemDefinition;
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Jewel of Harmony'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Jewel of Harmony'.</returns>
    private ItemDefinition CreateJewelOfHarmony()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Jewel of Harmony";
        itemDefinition.Number = 42;
        itemDefinition.Group = 14;
        itemDefinition.DropsFromMonsters = false;
        itemDefinition.DropLevel = 150;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.Value = 25;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        return itemDefinition;
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Lower refine stone'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Lower refine stone'.</returns>
    private ItemDefinition CreateLowerRefineStone()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Lower refine stone";
        itemDefinition.Number = 43;
        itemDefinition.Group = 14;
        itemDefinition.DropsFromMonsters = false;
        itemDefinition.DropLevel = 150;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.Value = 25;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        return itemDefinition;
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Higher refine stone'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Higher refine stone'.</returns>
    private ItemDefinition CreateHigherRefineStone()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Higher refine stone";
        itemDefinition.Number = 44;
        itemDefinition.Group = 14;
        itemDefinition.DropsFromMonsters = false;
        itemDefinition.DropLevel = 150;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.Value = 25;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        return itemDefinition;
    }
}
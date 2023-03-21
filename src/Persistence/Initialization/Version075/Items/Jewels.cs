// <copyright file="Jewels.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Jewels for MU Version 0.75.
/// </summary>
public class Jewels : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Jewels"/> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Jewels(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        this.GameConfiguration.Items.Add(this.CreateJewelOfBless());
        this.GameConfiguration.Items.Add(this.CreateJewelOfSoul());
        this.GameConfiguration.Items.Add(this.CreateJewelOfChaos());
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Jewel of Bless'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Jewel of Bless'.</returns>
    private ItemDefinition CreateJewelOfBless()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Jewel of Bless";
        itemDefinition.Number = 13;
        itemDefinition.Group = 14;
        itemDefinition.DropsFromMonsters = true;
        itemDefinition.DropLevel = 25;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.Value = 150;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        return itemDefinition;
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Jewel of Bless'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Jewel of Bless'.</returns>
    private ItemDefinition CreateJewelOfSoul()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Jewel of Soul";
        itemDefinition.Number = 14;
        itemDefinition.Group = 14;
        itemDefinition.DropsFromMonsters = true;
        itemDefinition.DropLevel = 30;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.Value = 150;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        return itemDefinition;
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Jewel of Chaos'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Jewel of Chaos'.</returns>
    private ItemDefinition CreateJewelOfChaos()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Jewel of Chaos";
        itemDefinition.Number = 15;
        itemDefinition.Group = 12;
        itemDefinition.DropsFromMonsters = true;
        itemDefinition.DropLevel = 12;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        return itemDefinition;
    }
}
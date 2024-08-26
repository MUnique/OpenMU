// <copyright file="Jewels.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Jewels for MU Version 0.75.
/// </summary>
public class Jewels : Version075.Items.Jewels
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
        base.Initialize();
        this.GameConfiguration.Items.Add(this.CreateJewelOfLife());
    }

    /// <summary>
    /// Creates an <see cref="ItemDefinition"/> for the 'Jewel of Life'.
    /// </summary>
    /// <returns><see cref="ItemDefinition"/> for the 'Jewel of Life'.</returns>
    private ItemDefinition CreateJewelOfLife()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Jewel of Life";
        itemDefinition.Number = 16;
        itemDefinition.Group = 14;
        itemDefinition.DropsFromMonsters = false;
        itemDefinition.DropLevel = 72;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        this.AddItemToJewelItemDrop(itemDefinition);
        return itemDefinition;
    }
}
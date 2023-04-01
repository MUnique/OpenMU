// <copyright file="Misc.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Initializing of misc items which don't fit into the other categories.
/// </summary>
public class Misc : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Misc"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Misc(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        this.CreateLifeStone();
        this.CreateGoldenCherryBlossomBranch();
    }

    private void CreateLifeStone()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Life Stone";
        itemDefinition.Number = 11;
        itemDefinition.Group = 13;
        itemDefinition.DropLevel = 75;
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        this.GameConfiguration.Items.Add(itemDefinition);
    }

    private void CreateGoldenCherryBlossomBranch()
    {
        var itemDefinition = this.Context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Golden Cherry Blossom Branch";
        itemDefinition.Number = 90;
        itemDefinition.Group = 14;
        itemDefinition.Durability = 255;
        itemDefinition.Width = 1;
        itemDefinition.Height = 2;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        this.GameConfiguration.Items.Add(itemDefinition);
    }
}
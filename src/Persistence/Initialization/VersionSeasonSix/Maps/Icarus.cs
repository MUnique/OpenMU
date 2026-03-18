// <copyright file="Icarus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initialization for the Icarus map.
/// </summary>
internal class Icarus : Version095d.Maps.Icarus
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Icarus"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Icarus(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    protected override void InitializeDropItemGroups()
    {
        base.InitializeDropItemGroups();
        var lochsFeather = this.GameConfiguration.Items.First(item => item.Group == 13 && item.Number == 14);

        var feather = this.Context.CreateNew<DropItemGroup>();
        feather.SetGuid(this.MapNumber, 1);
        feather.Chance = 0.001;
        feather.Description = "Loch's Feather";
        feather.MinimumMonsterLevel = 82;
        feather.PossibleItems.Add(lochsFeather);
        this.MapDefinition!.DropItemGroups.Add(feather);
        this.GameConfiguration.DropItemGroups.Add(feather);

        var crest = this.Context.CreateNew<DropItemGroup>();
        crest.SetGuid(this.MapNumber, 2);
        crest.Chance = 0.001;
        crest.Description = "Crest of Monarch";
        crest.MinimumMonsterLevel = 82;
        crest.ItemLevel = 1;
        crest.PossibleItems.Add(lochsFeather);
        this.MapDefinition.DropItemGroups.Add(crest);
        this.GameConfiguration.DropItemGroups.Add(crest);
    }
}

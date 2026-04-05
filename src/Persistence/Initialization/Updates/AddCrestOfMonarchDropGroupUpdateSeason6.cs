// <copyright file="AddCrestOfMonarchDropGroupUpdateSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the Crest of Monarch drop item group for the Icarus map.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("FF14A478-3EA8-4C41-A298-8E6698D5973D")]
public class AddCrestOfMonarchDropGroupUpdateSeason6 : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add Crest of Monarch Drop Group";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Adds the Crest of Monarch (Loch's Feather +1) drop item group to Icarus.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddCrestOfMonarchDropGroupSeason6;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 03, 13, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
#pragma warning disable CS1998
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
#pragma warning restore CS1998
    {
        var map = gameConfiguration.Maps.First(m => m.Number == Icarus.Number && m.Discriminator == 0);
        var lochsFeather = gameConfiguration.Items.First(item => item.Group == 13 && item.Number == 14);
        var crestId = GuidHelper.CreateGuid<DropItemGroup>(Icarus.Number, 2);

        var crestGroup = gameConfiguration.DropItemGroups.FirstOrDefault(group => group.GetId() == crestId);
        if (crestGroup is null)
        {
            crestGroup = context.CreateNew<DropItemGroup>();
            crestGroup.SetGuid(Icarus.Number, 2);
            gameConfiguration.DropItemGroups.Add(crestGroup);
        }

        crestGroup.Description = "Crest of Monarch";
        crestGroup.Chance = 0.001;
        crestGroup.MinimumMonsterLevel = 82;
        crestGroup.MaximumMonsterLevel = null;
        crestGroup.ItemLevel = 1;
        if (crestGroup.PossibleItems.Count != 1 || crestGroup.PossibleItems.First().GetItemId() != lochsFeather.GetItemId())
        {
            crestGroup.PossibleItems.Clear();
            crestGroup.PossibleItems.Add(lochsFeather);
        }

        if (!map.DropItemGroups.Any(group => group.GetId() == crestGroup.GetId()))
        {
            map.DropItemGroups.Add(crestGroup);
        }
    }
}

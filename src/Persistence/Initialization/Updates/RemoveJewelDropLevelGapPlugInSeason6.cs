// <copyright file="RemoveJewelDropLevelGapPlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update removes the existing drop level gap condition for jewels and similar items that should always drop.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("AB1C9F8B-5E3B-4F2A-BDCD-9C0F1E5A6B7C")]
public class RemoveJewelDropLevelGapPlugInSeason6 : RemoveJewelDropLevelGapPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.RemoveJewelDropLevelGapSeason6;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);

        var jewelOfGuardianDropGroupId = new Guid("00000200-001f-0001-0000-000000000000");
        if (gameConfiguration.DropItemGroups.FirstOrDefault(x => x.GetId() == jewelOfGuardianDropGroupId) is { } jewelOfGuardianDropGroup)
        {
            jewelOfGuardianDropGroup.ItemType = SpecialItemType.Jewel;
        }
    }
}
// <copyright file="FixMurenRingOfFireBonusPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update gives the Ring of Fire of the Muren ancient set its energy bonus, which was initialized as vitality.
/// </summary>
/// <remarks>
/// The client takes the stat of an ancient bonus from the item, not from the set: for the Ring of Fire it's energy,
/// as in the Anubis set. With the vitality bonus, the client showed more energy than the server calculated, so
/// e.g. a skill scroll whose energy requirement seemed to be met was refused without a message.
/// Items which were created before keep the bonus option they were created with.
/// </remarks>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixMurenRingOfFireBonusPlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixMurenRingOfFireBonusPlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("6E276910-8475-41E2-8085-978EA007C68B")]
public class FixMurenRingOfFireBonusPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Muren Ring of Fire Bonus";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Gives the Ring of Fire of the Muren ancient set its energy bonus, which was initialized as vitality.";

    private const string MurenSetName = "Muren";

    private const byte RingOfFireNumber = 21;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 04, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var energyBonus = gameConfiguration.ItemOptions
            .SelectMany(definition => definition.PossibleOptions)
            .FirstOrDefault(option => option.OptionType == ItemOptionTypes.AncientBonus
                                      && option.PowerUpDefinition?.TargetAttribute == Stats.TotalEnergy);
        var ringOfFire = gameConfiguration.ItemSetGroups
            .FirstOrDefault(set => set.Name.ValueInNeutralLanguage == MurenSetName)?
            .Items.FirstOrDefault(item => item.ItemDefinition?.Group == (byte)ItemGroups.Misc1 && item.ItemDefinition?.Number == RingOfFireNumber);
        if (energyBonus is not null && ringOfFire is not null)
        {
            ringOfFire.BonusOption = energyBonus;
        }

        return ValueTask.CompletedTask;
    }
}

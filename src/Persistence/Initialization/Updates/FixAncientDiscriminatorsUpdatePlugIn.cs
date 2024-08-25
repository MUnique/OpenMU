// <copyright file="FixAncientDiscriminatorsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes the discriminators of some ancient items.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("AB664421-1CA6-4FCE-A150-0007971017E1")]
public class FixAncientDiscriminatorsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Warrior Morning Star";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes the discriminators of some ancient items.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixAncientDiscriminators;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 08, 25, 17, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        ChangeDiscriminator(gameConfiguration, "Anonymous", ItemGroups.Shields, 0, 1);

        ChangeDiscriminator(gameConfiguration, "Mist", ItemGroups.Gloves, 0, 1);
        ChangeDiscriminator(gameConfiguration, "Mist", ItemGroups.Helm, 0, 1);

        ChangeDiscriminator(gameConfiguration, "Berserker", ItemGroups.Gloves, 6, 1);
        ChangeDiscriminator(gameConfiguration, "Berserker", ItemGroups.Boots, 6, 1);

        ChangeDiscriminator(gameConfiguration, "Cloud", ItemGroups.Helm, 8, 1);

        ChangeDiscriminator(gameConfiguration, "Rave", ItemGroups.Helm, 9, 1);
        ChangeDiscriminator(gameConfiguration, "Rave", ItemGroups.Pants, 9, 1);

        ChangeDiscriminator(gameConfiguration, "Barnake", ItemGroups.Boots, 2, 1);

        ChangeDiscriminator(gameConfiguration, "Sylion", ItemGroups.Gloves, 4, 1);
        ChangeDiscriminator(gameConfiguration, "Sylion", ItemGroups.Helm, 4, 1);

        ChangeDiscriminator(gameConfiguration, "Drake", ItemGroups.Armor, 10, 1);

        ChangeDiscriminator(gameConfiguration, "Fase", ItemGroups.Boots, 11, 1);
    }

    private static void ChangeDiscriminator(GameConfiguration gameConfiguration, string setName, ItemGroups itemGroup, byte itemNumber, byte discriminator)
    {
        var itemSetGroup = gameConfiguration.ItemSetGroups.First(set => set.Name == setName);
        var itemOfItemSet = itemSetGroup.Items.FirstOrDefault(item => item.ItemDefinition?.Group == (byte)itemGroup && item.ItemDefinition?.Number == itemNumber);
        if (itemOfItemSet != null)
        {
            itemOfItemSet.AncientSetDiscriminator = discriminator;
        }
    }
}
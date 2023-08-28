namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes the weapon of the warrior ancient set. The Hand Axe is replaced by the Morning Star.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("65BA79B5-1DBF-4C97-9628-0D8A429A8C88")]
public class FixWarriorMorningStarPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Warrior Morning Star";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes the weapon of the warrior ancient set. The Hand Axe is replaced by the Morning Star.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixWarriorMorningStar;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2023, 08, 28, 20, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var warriorSet = gameConfiguration.ItemSetGroups.First(set => set.Name == "Warrior");
        var itemSet = warriorSet.Items.FirstOrDefault(item => item.ItemDefinition?.Group == (byte)ItemGroups.Axes);
        if (itemSet != null)
        {
            itemSet.ItemDefinition = gameConfiguration.Items.First(item => item is { Group: (byte)ItemGroups.Scepters, Number: 1 });
        }
    }
}
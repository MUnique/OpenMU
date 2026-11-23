// <copyright file="FixFlameOfCondorDropLevelUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Lets the Flame of Condor drop in the Barracks of Balgass, by removing its drop level.
/// </summary>
/// <remarks>
/// The item was created with a drop level of 120, but the monsters of the Barracks of Balgass only have the levels 117 to 119.
/// The drop generator refuses an item with a drop level above the monster level, so the Flame of Condor never dropped.
/// Only the original value of 120 is changed; a drop level which was configured otherwise is kept.
/// </remarks>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixFlameOfCondorDropLevelUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixFlameOfCondorDropLevelUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("B5D2E7A1-4C93-48F0-9E6A-7D1B0C3F8A52")]
public class FixFlameOfCondorDropLevelUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Let the Flame of Condor drop";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "This update removes the drop level of the Flame of Condor, which was higher than the level of the monsters of the Barracks of Balgass and prevented the item from dropping.";

    private const byte FlameOfCondorGroup = 13;
    private const short FlameOfCondorNumber = 52;
    private const byte OriginalDropLevel = 120;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 22, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Items.FirstOrDefault(item => item.Group == FlameOfCondorGroup && item.Number == FlameOfCondorNumber) is { } flame
            && flame.DropLevel == OriginalDropLevel)
        {
            flame.DropLevel = 0;
        }

        return ValueTask.CompletedTask;
    }
}

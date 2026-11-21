// <copyright file="AddLordsMixUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the Lord's Mix to the castle senior NPC, and limits it to one mix per day.
/// </summary>
/// <remarks>
/// It's optional, because it adds a new source of excellent items with more than two excellent options.
/// A castle siege configuration which already has a limit keeps it and its period.
/// </remarks>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddLordsMixUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddLordsMixUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("DAF04934-DE4D-47E5-BF80-78C15245BDFB")]
public class AddLordsMixUpdatePlugIn : UpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => Properties.PlugInResources.AddLordsMixUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => Properties.PlugInResources.AddLordsMixUpdatePlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 19, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        LordsMix.AddIfMissing(context, gameConfiguration);
        if (gameConfiguration.CastleSiegeConfiguration is { LordsMixLimit: 0 } castleSiegeConfiguration)
        {
            castleSiegeConfiguration.LordsMixLimit = LordsMix.Limit;
            castleSiegeConfiguration.LordsMixLimitPeriod = LordsMix.LimitPeriod;
        }

        return ValueTask.CompletedTask;
    }
}

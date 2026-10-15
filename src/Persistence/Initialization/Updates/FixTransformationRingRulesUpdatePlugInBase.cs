// <copyright file="FixTransformationRingRulesUpdatePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Adds the configured high-level requirement and maximum-health bonus to the original transformation ring.
/// </summary>
public abstract class FixTransformationRingRulesUpdatePlugInBase : UpdatePlugInBase
{
    /// <inheritdoc />
    public override string Name => PlugInResources.FixTransformationRingRulesUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.FixTransformationRingRulesUpdatePlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var ring = gameConfiguration.Items.FirstOrDefault(item => item.Group == 13 && item.Number == 10);
        if (ring is not null)
        {
            TransformationRingConfiguration.ConfigureHighLevelTransformationRing(context, gameConfiguration, ring);
        }

        return ValueTask.CompletedTask;
    }
}

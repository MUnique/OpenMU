// <copyright file="AddPointsPerResetAttributePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System;
using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the <see cref="Stats.PointsPerReset"/>.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("6011A1B8-7FA5-48EB-935D-EEAF83017799")]
public class AddPointsPerResetAttributePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Adds attribute PointsPerReset";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the attribute PointsPerReset.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddPointsPerResetByClassAttribute;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 06, 09, 13, 30, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Attributes.Contains(Stats.PointsPerReset))
        {
            return;
        }

        var attribute = Stats.PointsPerReset;
        var persistentAttribute = context.CreateNew<AttributeDefinition>(attribute.Id, attribute.Designation, attribute.Description);
        gameConfiguration.Attributes.Add(persistentAttribute);
    }
}
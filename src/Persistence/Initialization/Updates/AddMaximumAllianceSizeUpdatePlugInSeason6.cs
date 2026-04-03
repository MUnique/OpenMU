// <copyright file="AddMaximumAllianceSizeUpdatePlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the <see cref="Stats.MaximumAllianceSize"/> global base attribute
/// (default value 5) to Season 6 game configurations.
/// Older game versions (below season 1) do not support alliances and therefore do not
/// receive this attribute.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("2C2743B0-1305-47BF-85D9-09F6CA64AD54")]
public class AddMaximumAllianceSizeUpdatePlugInSeason6 : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add Maximum Alliance Size Attribute (Season 6)";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Adds the MaximumAllianceSize global base attribute with a default value of 5 to the Season 6 game configuration.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddMaximumAllianceSizeSeason6;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 04, 03, 18, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
#pragma warning disable CS1998
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
#pragma warning restore CS1998
    {
        this.AddStatIfNotExists(context, gameConfiguration, Stats.MaximumAllianceSize);

        var maximumAllianceSizeDef = gameConfiguration.Attributes.FirstOrDefault(a => a.Id == Stats.MaximumAllianceSize.Id);
        if (maximumAllianceSizeDef is null)
        {
            return;
        }

        if (!gameConfiguration.GlobalBaseAttributeValues.Any(a => a.Definition?.Id == Stats.MaximumAllianceSize.Id))
        {
            var maximumAllianceSizeValue = context.CreateNew<ConstValueAttribute>(5f, maximumAllianceSizeDef);
            gameConfiguration.GlobalBaseAttributeValues.Add(maximumAllianceSizeValue);
        }
    }
}

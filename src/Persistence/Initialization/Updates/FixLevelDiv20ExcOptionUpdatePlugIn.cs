// <copyright file="FixLevelDiv20ExcOptionUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The chaos castle update plugin.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("B0F275DC-B3C2-4826-8263-FFDC8A8AFAEA")]
public class FixLevelDiv20ExcOptionUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Excellent Option Level/20 damage fix";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes the excellent option which adds level / 20 as wizardry damage";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixLevelDiv20ExcOptionUpdate;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2023, 04, 03, 20, 5, 0, DateTimeKind.Utc);

    /// <inheritdoc />
#pragma warning disable CS1998
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
#pragma warning restore CS1998
    {
        var level20Relationships = gameConfiguration.ItemOptions
            .SelectMany(io => io.PossibleOptions.Where(p => p.OptionType == ItemOptionTypes.Excellent))
            .SelectMany(o => o.PowerUpDefinition?.Boost?.RelatedValues.Where(s => s.InputAttribute == Stats.Level && Math.Abs(s.InputOperand - 20f) < 0.01) ?? Enumerable.Empty<AttributeRelationship>())
            .ToList();
        foreach (var relationship in level20Relationships)
        {
            relationship.InputOperand = 1f / 20f;
        }
    }
}
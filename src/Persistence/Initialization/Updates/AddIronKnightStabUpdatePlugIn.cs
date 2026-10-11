// <copyright file="AddIronKnightStabUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Raklion;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Lets the iron knight of raklion use its stab, by setting its intelligence to the <see cref="IronKnightIntelligence"/>.
/// </summary>
/// <remarks>
/// An intelligence which was already configured for the iron knight is kept.
/// </remarks>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddIronKnightStabUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddIronKnightStabUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("6C0E4B2A-93F1-4D7E-A58B-1F2D7C9E3A40")]
public class AddIronKnightStabUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Let the Iron Knight use its stab";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "This update lets the Iron Knight of Raklion attack with its stab, which the client shows with a combo effect.";

    private const short IronKnightNumber = 458;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 09, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == IronKnightNumber) is { } ironKnight
            && string.IsNullOrWhiteSpace(ironKnight.IntelligenceTypeName))
        {
            ironKnight.IntelligenceTypeName = typeof(IronKnightIntelligence).FullName;
        }

        return ValueTask.CompletedTask;
    }
}

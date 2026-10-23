// <copyright file="ConfigureCastleSiegeGuardTalkPlugInsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Configures the NPCs of the Castle Siege guard talk plug-ins:
/// the guardsmen (224) at the bridge open the registration window,
/// and the guard (220) at the Land of Trials entrance opens the Land of Trials dialog.
/// </summary>
/// <remarks>
/// Before, the guardsmen opened the Land of Trials dialog, and no NPC opened the registration window.
/// The guard was an attacking guard with its own intelligence and combat attributes,
/// which it doesn't need as a passive NPC which players talk to.
/// </remarks>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.ConfigureCastleSiegeGuardTalkPlugInsUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.ConfigureCastleSiegeGuardTalkPlugInsUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("0B9D1938-0A3F-41D3-B5F1-1BAA1512BE6F")]
public class ConfigureCastleSiegeGuardTalkPlugInsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Configure Castle Siege guards";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "The guardsmen at the bridge of the Castle Siege map open the registration window, and the guard at the Land of Trials entrance becomes a passive NPC which opens the Land of Trials dialog.";

    private const short GuardNumber = 220;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 21, 18, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override IEnumerable<UpdateDependency> DependsOn => [typeof(AddCastleSiegeDataUpdatePlugIn).GUID, typeof(ConfigureNpcTalkPlugInsPlugInSeason6).GUID];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == GuardNumber) is { } guard)
        {
            MakePassive(guard);
        }

        Configure(context, gameConfiguration, new CastleSiegeRegistrationGuardsmanTalkPlugIn());
        Configure(context, gameConfiguration, new CastleSiegeLandOfTrialsGuardTalkPlugIn());
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Turns the guard into a passive NPC, like the other Castle Siege NPCs which players talk to.
    /// </summary>
    /// <param name="guard">The guard.</param>
    private static void MakePassive(MonsterDefinition guard)
    {
        guard.ObjectKind = NpcObjectKind.PassiveNpc;
        guard.IntelligenceTypeName = null;
        guard.MoveRange = default;
        guard.AttackRange = default;
        guard.ViewRange = default;
        guard.MoveDelay = default;
        guard.AttackDelay = default;
        guard.RespawnDelay = default;
        guard.NumberOfMaximumItemDrops = default;
        foreach (var attribute in guard.Attributes.ToList())
        {
            guard.Attributes.Remove(attribute);
        }
    }

    private static void Configure(IContext context, GameConfiguration gameConfiguration, NpcTalkPlugInBase plugIn)
    {
        var typeId = plugIn.GetType().GUID;
        if (gameConfiguration.PlugInConfigurations.FirstOrDefault(c => c.TypeId == typeId) is not { } plugInConfiguration)
        {
            plugInConfiguration = context.CreateNew<PlugInConfiguration>();
            plugInConfiguration.SetGuid(typeId);
            plugInConfiguration.TypeId = typeId;
            plugInConfiguration.IsActive = true;
            gameConfiguration.PlugInConfigurations.Add(plugInConfiguration);
        }

        plugInConfiguration.SetConfiguration(plugIn.CreateDefaultConfig(gameConfiguration), new ConfigureNpcTalkPlugInsPlugInBase.IdReferenceHandler());
    }
}

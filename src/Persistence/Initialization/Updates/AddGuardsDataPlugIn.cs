// <copyright file="AddGuardsDataPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This updates adds the data for Guard NPCs.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("1EF50759-0A5F-4301-A5E9-B68A8B7D29F9")]
public class AddGuardsDataPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add guard npc data";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds data for guard npcs, so they can attack and move around.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddGuardsData;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2023, 06, 01, 20, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var guard = gameConfiguration.Monsters.FirstOrDefault(m => m.Number == 220);
        if (guard is { AttackRange: 0 })
        {
            guard.Designation = "Guard";
            guard.ObjectKind = NpcObjectKind.Guard;
            guard.MoveRange = 3;
            guard.AttackRange = 2;
            guard.ViewRange = 8;
            guard.IntelligenceTypeName = typeof(GuardIntelligence).FullName;
            guard.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            guard.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            guard.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
            guard.NumberOfMaximumItemDrops = 0;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 2 },
                { Stats.MaximumHealth, 500 },
                { Stats.MinimumPhysBaseDmg, 15 },
                { Stats.MaximumPhysBaseDmg, 30 },
                { Stats.AttackRatePvm, 30 },
                { Stats.DefenseRatePvm, 20 },
                { Stats.DefenseBase, 70 },
            };
            guard.AddAttributes(attributes, context, gameConfiguration);
        }

        var crossbowGuard = gameConfiguration.Monsters.FirstOrDefault(m => m.Number == 247);
        if (crossbowGuard is { AttackRange: 0 })
        {
            crossbowGuard.Designation = "Crossbow Guard";
            crossbowGuard.ObjectKind = NpcObjectKind.Guard;
            crossbowGuard.MoveRange = 3;
            crossbowGuard.AttackRange = 5;
            crossbowGuard.ViewRange = 7;
            crossbowGuard.IntelligenceTypeName = typeof(GuardIntelligence).FullName;
            crossbowGuard.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            crossbowGuard.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            crossbowGuard.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
            crossbowGuard.NumberOfMaximumItemDrops = 0;
            crossbowGuard.Attribute = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 90 },
                { Stats.MaximumHealth, 10000 },
                { Stats.MinimumPhysBaseDmg, 180 },
                { Stats.MaximumPhysBaseDmg, 195 },
                { Stats.AttackRatePvm, 300 },
                { Stats.DefenseRatePvm, 100 },
                { Stats.DefenseBase, 70 },
            };
            crossbowGuard.AddAttributes(attributes, context, gameConfiguration);
        }

        var berdyshGuard = gameConfiguration.Monsters.FirstOrDefault(m => m.Number == 249);
        if (berdyshGuard is { AttackRange: 0 })
        {
            berdyshGuard.Designation = "Berdysh Guard";
            berdyshGuard.ObjectKind = NpcObjectKind.Guard;
            berdyshGuard.MoveRange = 3;
            berdyshGuard.AttackRange = 2;
            berdyshGuard.ViewRange = 7;
            berdyshGuard.IntelligenceTypeName = typeof(GuardIntelligence).FullName;
            berdyshGuard.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            berdyshGuard.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            berdyshGuard.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
            berdyshGuard.NumberOfMaximumItemDrops = 0;
            berdyshGuard.Attribute = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 90 },
                { Stats.MaximumHealth, 10000 },
                { Stats.MinimumPhysBaseDmg, 180 },
                { Stats.MaximumPhysBaseDmg, 195 },
                { Stats.AttackRatePvm, 300 },
                { Stats.DefenseRatePvm, 100 },
                { Stats.DefenseBase, 70 },
            };
            berdyshGuard.AddAttributes(attributes, context, gameConfiguration);
        }
    }
}
// <copyright file="TargetedSkillTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

using MagicEffectDefinition = MUnique.OpenMU.Persistence.BasicModel.MagicEffectDefinition;
using PowerUpDefinition = MUnique.OpenMU.Persistence.BasicModel.PowerUpDefinition;
using PowerUpDefinitionValue = MUnique.OpenMU.Persistence.BasicModel.PowerUpDefinitionValue;
using Skill = MUnique.OpenMU.Persistence.BasicModel.Skill;

/// <summary>
/// Unit tests for targeted skill behavior.
/// </summary>
[TestFixture]
public class TargetedSkillTests
{
    private IGameContext _gameContext = null!;
    private readonly TargetedSkillDefaultPlugin _plugin = new();

    /// <summary>
    /// Sets up the test context.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._gameContext = GameContextTestHelper.CreateGameContext();
    }

    /// <summary>
    /// Tests that an ImplicitParty buff is successfully applied to both the caster and their party members.
    /// </summary>
    [Test]
    public async Task TestImplicitPartyBuffAppliesToCasterInPartyAsync()
    {
        // Arrange: Create caster and member in a party
        var caster = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        var member = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);

        var party = new Party(new PartyManager(5, new NullLogger<Party>()), 5, new NullLogger<Party>());
        caster.Party = party;
        member.Party = party;
        await party.AddAsync(caster).ConfigureAwait(false);
        await party.AddAsync(member).ConfigureAwait(false);

        // Put member in the caster's observers (meaning they are in range/sight)
        caster.Observers.Add(member);

        // Configure map terrain: disable safe zone at their positions
        var mapTerrain = caster.CurrentMap!.Terrain;
        System.Array.Clear(mapTerrain.SafezoneMap, 0, mapTerrain.SafezoneMap.Length);
        caster.Position = new Point(10, 10);
        member.Position = new Point(11, 10);

        // Define the party buff skill
        var skill = new Skill
        {
            Number = 1000,
            Name = "Test Party Buff",
            SkillType = SkillType.Buff,
            Target = SkillTarget.ImplicitParty,
            Range = 7,
            MagicEffectDef = new MagicEffectDefinition
            {
                Number = 100,
                Name = "Test Buff Effect",
                StopByDeath = true,
                InformObservers = true,
                Duration = new PowerUpDefinitionValue { Value = 60f }
            }
        };

        var powerUpDef = new PowerUpDefinition
        {
            TargetAttribute = Stats.DefenseBase,
            Boost = new PowerUpDefinitionValue { Value = 10f }
        };
        skill.MagicEffectDef.PowerUpDefinitions.Add(powerUpDef);

        await caster.SkillList!.AddLearnedSkillAsync(skill).ConfigureAwait(false);

        // Act: Perform the skill
        await this._plugin.PerformSkillAsync(caster, caster, (ushort)skill.Number).ConfigureAwait(false);

        // Assert: Caster should receive their own buff, and the observed party member should also receive it
        var casterHasBuff = caster.MagicEffectList.ActiveEffects.Values.Any(e => e.Definition == skill.MagicEffectDef);
        var memberHasBuff = member.MagicEffectList.ActiveEffects.Values.Any(e => e.Definition == skill.MagicEffectDef);

        Assert.That(casterHasBuff, Is.True, "Caster did not receive their own party buff.");
        Assert.That(memberHasBuff, Is.True, "Party member did not receive the party buff.");
    }

    /// <summary>
    /// Tests that casting a buff in a safe zone applies the buff.
    /// </summary>
    [Test]
    public async Task TestBuffCastingAllowedInSafezoneAsync()
    {
        // Arrange: Create player
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);

        // Configure map terrain: mark position as safe zone
        var mapTerrain = player.CurrentMap!.Terrain;
        System.Array.Clear(mapTerrain.SafezoneMap, 0, mapTerrain.SafezoneMap.Length);
        player.Position = new Point(20, 20);
        mapTerrain.SafezoneMap[20, 20] = true;

        Assert.That(player.IsAtSafezone(), Is.True);

        // Define a self buff skill
        var skill = new Skill
        {
            Number = 1001,
            Name = "Test Self Buff",
            SkillType = SkillType.Buff,
            Target = SkillTarget.ImplicitPlayer,
            Range = 3,
            MagicEffectDef = new MagicEffectDefinition
            {
                Number = 101,
                Name = "Test Self Buff Effect",
                StopByDeath = true,
                InformObservers = true,
                Duration = new PowerUpDefinitionValue { Value = 60f }
            }
        };

        var powerUpDef = new PowerUpDefinition
        {
            TargetAttribute = Stats.DefenseBase,
            Boost = new PowerUpDefinitionValue { Value = 10f }
        };
        skill.MagicEffectDef.PowerUpDefinitions.Add(powerUpDef);

        await player.SkillList!.AddLearnedSkillAsync(skill).ConfigureAwait(false);

        // Act: Perform the skill
        await this._plugin.PerformSkillAsync(player, player, (ushort)skill.Number).ConfigureAwait(false);

        // Assert: Player should receive their own buff even in a safe zone
        var playerHasBuff = player.MagicEffectList.ActiveEffects.Values.Any(e => e.Definition == skill.MagicEffectDef);
        Assert.That(playerHasBuff, Is.True, "Player should be allowed to buff themselves in a safe zone.");
    }
}

// <copyright file="KanturuEventDefinitionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// Tests for the default <see cref="KanturuEventDefinition"/>.
/// The expected timers and kill targets come from new-requirements.txt:
/// each wave (monsters + boss together) shares one clock — 15 minutes for
/// waves 1-2, 20 minutes for waves 3-4.
/// </summary>
[TestFixture]
public class KanturuEventDefinitionTests
{
    private readonly KanturuEventDefinition _definition = KanturuEventDefinition.CreateDefault(CreateGameConfiguration());

    /// <summary>
    /// Tests that the first phase of each wave carries the shared wave time limit.
    /// </summary>
    /// <param name="phaseName">The name of the phase.</param>
    /// <param name="expectedMinutes">The expected time limit in minutes.</param>
    [TestCase("Phase 1 - Monsters", 15)]
    [TestCase("Phase 2 - Monsters", 15)]
    [TestCase("Phase 3 - Monsters", 20)]
    [TestCase("Nightmare - Guardians", 20)]
    public void WaveStartPhase_HasRequiredTimeLimit(string phaseName, int expectedMinutes)
    {
        var phase = this._definition.Phases.First(phase => phase.Name == phaseName);

        Assert.That(phase.TimeLimit, Is.EqualTo(TimeSpan.FromMinutes(expectedMinutes)));
    }

    /// <summary>
    /// Tests that the first phase of each wave starts a shared wave clock.
    /// </summary>
    /// <param name="phaseName">The name of the phase.</param>
    [TestCase("Phase 1 - Monsters")]
    [TestCase("Phase 2 - Monsters")]
    [TestCase("Phase 3 - Monsters")]
    [TestCase("Nightmare - Guardians")]
    public void WaveStartPhase_StartsSharedWaveClock(string phaseName)
    {
        var phase = this._definition.Phases.First(phase => phase.Name == phaseName);

        Assert.That(phase.TimeLimitGroup, Is.Not.Null);
    }

    /// <summary>
    /// Tests that the boss phases carry no own time limit.
    /// </summary>
    /// <param name="phaseName">The name of the phase.</param>
    [TestCase("Phase 1 - Maya's left hand")]
    [TestCase("Phase 2 - Maya's right hand")]
    [TestCase("Phase 3 - Both hands of Maya")]
    [TestCase("Nightmare")]
    public void BossPhase_HasNoOwnTimeLimit(string phaseName)
    {
        var phase = this._definition.Phases.First(phase => phase.Name == phaseName);

        Assert.That(phase.TimeLimit, Is.Null);
    }

    /// <summary>
    /// Tests that the boss phases inherit the remaining wave time instead of
    /// getting a fresh timer.
    /// </summary>
    /// <param name="phaseName">The name of the phase.</param>
    /// <param name="expectedGroup">The expected shared wave clock.</param>
    [TestCase("Phase 1 - Maya's left hand", KanturuWaveGroup.MayaLeftHand)]
    [TestCase("Phase 2 - Maya's right hand", KanturuWaveGroup.MayaRightHand)]
    [TestCase("Phase 3 - Both hands of Maya", KanturuWaveGroup.MayaBothHands)]
    [TestCase("Nightmare", KanturuWaveGroup.Nightmare)]
    public void BossPhase_InheritsSharedWaveTime(string phaseName, KanturuWaveGroup expectedGroup)
    {
        var phase = this._definition.Phases.First(phase => phase.Name == phaseName);

        Assert.That(phase.TimeLimitGroup, Is.EqualTo(expectedGroup));
    }

    /// <summary>
    /// Tests that a Nightmare phase definition exists.
    /// </summary>
    [Test]
    public void NightmarePhaseDefinition_Exists()
    {
        var nightmare = this._definition.Phases.First(phase => phase.Kind == KanturuPhaseKind.Nightmare).Nightmare;

        Assert.That(nightmare, Is.Not.Null);
    }

    /// <summary>
    /// Tests that every Nightmare health phase spawns its summon wave.
    /// </summary>
    [Test]
    public void NightmareHpPhases_SummonConfiguredWaves()
    {
        var nightmare = this._definition.Phases.First(phase => phase.Kind == KanturuPhaseKind.Nightmare).Nightmare;

        Assert.That(nightmare!.HpPhases.Select(phase => phase.SummonWaveNumber), Is.EqualTo(new byte?[] { 9, 10, 11 }));
    }

    /// <summary>
    /// Tests that the Maya hand standbys allow refilling up to 15 players.
    /// </summary>
    /// <param name="phaseName">The name of the phase.</param>
    [TestCase("Phase 1 - Maya's left hand")]
    [TestCase("Phase 2 - Maya's right hand")]
    public void MayaHandPhase_HasTwoMinuteStandby_ForRefills(string phaseName)
    {
        var phase = this._definition.Phases.First(phase => phase.Name == phaseName);

        Assert.That(phase.StandbyDuration, Is.EqualTo(TimeSpan.FromMinutes(2)));
    }

    /// <summary>
    /// Tests that the wave kill targets match the wave details (40/40/20).
    /// </summary>
    /// <param name="phaseName">The name of the phase.</param>
    /// <param name="expectedKillTarget">The expected kill target.</param>
    [TestCase("Phase 1 - Monsters", 40)]
    [TestCase("Phase 1 - Maya's left hand", 1)]
    [TestCase("Phase 2 - Monsters", 40)]
    [TestCase("Phase 2 - Maya's right hand", 1)]
    [TestCase("Phase 3 - Monsters", 20)]
    [TestCase("Phase 3 - Both hands of Maya", 2)]
    [TestCase("Nightmare", 1)]
    public void DefaultPhase_HasRequiredKillTarget(string phaseName, int expectedKillTarget)
    {
        var phase = this._definition.Phases.First(phase => phase.Name == phaseName);

        Assert.That(phase.KillTarget, Is.EqualTo(expectedKillTarget));
    }

    private static GameConfiguration CreateGameConfiguration()
    {
        var gameConfiguration = new Mock<GameConfiguration>();
        gameConfiguration.Setup(c => c.Monsters).Returns(new List<MonsterDefinition>());
        return gameConfiguration.Object;
    }
}

// <copyright file="PartyBenchmarks.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Benchmarks;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Benchmarks.Helpers;

/// <summary>
/// Benchmarks for <see cref="Party"/> class methods related to XP distribution and item drops.
/// </summary>
[MemoryDiagnoser]
[ThreadingDiagnoser]
[InvocationCount(100)]
public class PartyBenchmarks
{
    private Party _party = null!;
    private Player _killer = null!;
    private IAttackable _killedObject = null!;
    private List<Player> _players = null!;

    /// <summary>
    /// Sets up the benchmark by creating a party with 5 players.
    /// </summary>
    [GlobalSetup]
    public async Task Setup()
    {
        var partyManager = new PartyManager(5, new NullLogger<Party>());
        _party = new Party(partyManager, 5, new NullLogger<Party>());

        _players = new List<Player>();
        for (int i = 0; i < 5; i++)
        {
            var player = await PlayerTestHelper.CreatePlayerAsync();
            await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
            if (player.Attributes is { } attrs)
            {
                attrs[Stats.Level] = (short)(100 + i);
            }

            _players.Add(player);
            await _party.AddAsync(player).ConfigureAwait(false);
        }

        _killer = _players[0];
        foreach (var player in _players)
        {
            _killer.Observers.Add(player);
        }

        _killedObject = CreateMockAttackable(50);
    }

    /// <summary>
    /// Benchmarks the <see cref="Party.DistributeExperienceAfterKillAsync"/> method.
    /// </summary>
    [Benchmark]
    public async ValueTask DistributeExperienceAfterKillAsync()
    {
        await _party.DistributeExperienceAfterKillAsync(_killedObject, _killer);
    }

    /// <summary>
    /// Benchmarks the <see cref="Party.DistributeMoneyAfterKillAsync"/> method.
    /// </summary>
    [Benchmark]
    public async ValueTask DistributeMoneyAfterKillAsync()
    {
        await _party.DistributeMoneyAfterKillAsync(_killedObject, _killer, 10000);
    }

    /// <summary>
    /// Benchmarks the <see cref="Party.GetQuestDropItemGroupsAsync"/> method.
    /// </summary>
    [Benchmark]
    public async ValueTask GetQuestDropItemGroupsAsync()
    {
        await _party.GetQuestDropItemGroupsAsync(_killer);
    }

    private static IAttackable CreateMockAttackable(float level)
    {
        var attributes = new Mock<IAttributeSystem>();
        attributes.Setup(a => a[Stats.Level]).Returns(level);

        var result = new Mock<IAttackable>();
        result.SetupAllProperties();
        result.SetupGet(a => a.Attributes).Returns(attributes.Object);

        GameMap? nullMap = null;
        result.SetupGet(a => a.CurrentMap).Returns(nullMap);

        return result.Object;
    }
}
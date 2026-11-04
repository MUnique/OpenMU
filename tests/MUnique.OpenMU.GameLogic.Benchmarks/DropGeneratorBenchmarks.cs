// <copyright file="DropGeneratorBenchmarks.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Benchmarks.Helpers;

namespace MUnique.OpenMU.GameLogic.Benchmarks;

/// <summary>
/// Benchmarks for the <see cref="DefaultDropGenerator"/>.
/// </summary>
[MemoryDiagnoser]
[ThreadingDiagnoser]
[InvocationCount(100)]
public class DropGeneratorBenchmarks
{
    private DefaultDropGenerator _generator = null!;
    private MonsterDefinition _monster = null!;
    private Player _player = null!;

    /// <summary>
    /// Global setup for the benchmarks.
    /// </summary>
    [GlobalSetup]
    public async Task Setup()
    {
        var config = GameConfigurationTestHelper.Create();
        var randomizer = RandomizerTestHelper.Create();
        _generator = new DefaultDropGenerator(config, randomizer);
        _monster = MonsterTestHelper.Create(10, 1);
        _player = await PlayerTestHelper.CreatePlayerAsync();
    }

    /// <summary>
    /// Benchmarks the drop generation with a single player and monster.
    /// </summary>
    /// <returns>A value task.</returns>
    [Benchmark]
    public async ValueTask GenerateItemDropsAsync()
        => await _generator.GenerateItemDropsAsync(_monster, 1000, _player);
}
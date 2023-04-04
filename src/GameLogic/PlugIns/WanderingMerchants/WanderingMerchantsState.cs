// <copyright file="WanderingMerchantsState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WanderingMerchants;

using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// Keeps track of the state of the wandering merchants of a game server.
/// </summary>
public class WanderingMerchantsState : PeriodicTaskGameServerState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WanderingMerchantsState"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public WanderingMerchantsState(IGameContext context)
        : base(context)
    {
        var groupedMerchants = context.Configuration.Maps
            .SelectMany(m => m.MonsterSpawns.Where(s => s.SpawnTrigger == SpawnTrigger.Wandering))
            .GroupBy(s => s.MonsterDefinition)
            .Where(s => s.Key is not null)
            .Select(g => (g.Key, g.ToList()))
            .ToList();
        foreach (var (merchant, spawns) in groupedMerchants)
        {
            var merchantState = new MerchantSpawnState(merchant!, spawns);
            this.Merchants.Add(merchantState);
        }
    }

    /// <summary>
    /// Gets the states of the merchants.
    /// </summary>
    public ICollection<MerchantSpawnState> Merchants { get; } = new List<MerchantSpawnState>();
}
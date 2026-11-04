// <copyright file="MerchantSpawnState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WanderingMerchants;

using MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// Spawn state of a wandering merchant.
/// </summary>
public class MerchantSpawnState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MerchantSpawnState"/> class.
    /// </summary>
    /// <param name="merchantDefinition">The merchant definition.</param>
    /// <param name="possibleSpawns">The possible spawns.</param>
    public MerchantSpawnState(MonsterDefinition merchantDefinition, List<MonsterSpawnArea> possibleSpawns)
    {
        this.MerchantDefinition = merchantDefinition;
        this.PossibleSpawns = possibleSpawns;
        this.NextWanderingAt = DateTime.UtcNow.AddSeconds(10);
    }

    /// <summary>
    /// Gets or sets the merchant definition.
    /// </summary>
    public MonsterDefinition MerchantDefinition { get; set; }

    /// <summary>
    /// Gets the possible spawn points of the merchant.
    /// </summary>
    public List<MonsterSpawnArea> PossibleSpawns { get; }

    /// <summary>
    /// Gets or sets the spawned merchant.
    /// </summary>
    public NonPlayerCharacter? Merchant { get; set; }

    /// <summary>
    /// Gets or sets time when the next wandering should occur.
    /// </summary>
    public DateTime NextWanderingAt { get; set; }
}
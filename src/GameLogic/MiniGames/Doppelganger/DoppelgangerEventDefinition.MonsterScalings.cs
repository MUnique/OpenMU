// <copyright file="DoppelgangerEventDefinition.MonsterScalings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;

/// <content>
/// The default multipliers for the monsters.
/// </content>
public partial class DoppelgangerEventDefinition
{
    private const int MaximumPlayerCount = 5;
    private const int ScalingLevelStep = 50;
    private const int ScalingMaximumPlayerLevel = 800;

    /// <summary>
    /// Creates simple default multipliers of the monsters, which are meant to be adjusted by the
    /// server administrator. There is one entry for every 50 player levels (including the master level).
    /// The health, damage and defense grow linearly with the player level, and each additional player
    /// adds to them.
    /// </summary>
    private static IList<DoppelgangerMonsterScaling> CreateDefaultMonsterScalings()
    {
        var scalings = new List<DoppelgangerMonsterScaling>();
        for (var playerLevel = ScalingLevelStep; playerLevel <= ScalingMaximumPlayerLevel; playerLevel += ScalingLevelStep)
        {
            var level = 1 + (playerLevel / 100f);
            var health = Math.Max(1, playerLevel / 10f);
            var damage = Math.Max(1, playerLevel / 50f);
            var defense = Math.Max(1, playerLevel / 40f);
            scalings.Add(new DoppelgangerMonsterScaling
            {
                MaximumPlayerLevel = playerLevel,
                LevelMultipliers = PerPlayerCount(level, 0),
                HealthMultipliers = PerPlayerCount(health, 0.5f),
                DamageMultipliers = PerPlayerCount(damage, 0.2f),
                DefenseMultipliers = PerPlayerCount(defense, 0.2f),
            });
        }

        return scalings;

        static IList<float> PerPlayerCount(float multiplier, float increasePerAdditionalPlayer) =>
            Enumerable.Range(0, MaximumPlayerCount)
                .Select(additionalPlayers => MathF.Round(multiplier * (1 + (additionalPlayers * increasePerAdditionalPlayer)), 2))
                .ToList();
    }
}

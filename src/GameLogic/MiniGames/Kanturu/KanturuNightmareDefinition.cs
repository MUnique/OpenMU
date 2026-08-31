// <copyright file="KanturuNightmareDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// The definition of the Nightmare boss fight.
/// </summary>
public class KanturuNightmareDefinition
{
    /// <summary>
    /// Gets or sets the monster number of the boss.
    /// </summary>
    public short MonsterNumber { get; set; }

    /// <summary>
    /// Gets or sets how long the context waits for the boss to spawn before it gives up
    /// monitoring its health.
    /// </summary>
    public TimeSpan SpawnTimeout { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Gets or sets the interval in which the boss' health is checked for a phase change.
    /// </summary>
    public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets the delay between restoring the boss' health and teleporting it, so the
    /// clients can process the health update first.
    /// </summary>
    public TimeSpan TeleportDelay { get; set; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Gets or sets the number of the skill whose animation is broadcasted as the boss'
    /// special attack. It maps to the client side <c>AT_SKILL_*</c> values; the default (14,
    /// Inferno) selects the ATTACK4 animation of the Nightmare model.
    /// </summary>
    /// <remarks>
    /// This is a purely visual broadcast. The actual area damage is done by the
    /// <see cref="MUnique.OpenMU.DataModel.Configuration.MonsterDefinition.AttackSkill"/>.
    /// </remarks>
    public short SpecialAttackSkillNumber { get; set; } = 14;

    /// <summary>
    /// Gets or sets the interval in which the special attack animation is broadcasted.
    /// Set it to <see cref="TimeSpan.Zero"/> to disable it.
    /// </summary>
    public TimeSpan SpecialAttackInterval { get; set; } = TimeSpan.FromSeconds(20);

    /// <summary>
    /// Gets or sets the detail state which is sent to the clients once the boss has spawned,
    /// so they show the boss HUD.
    /// </summary>
    public byte BattleDetailState { get; set; } = (byte)KanturuNightmareDetailState.Battle;

    /// <summary>
    /// Gets or sets the health based phases of the fight, in the order in which they occur.
    /// </summary>
    public IList<KanturuNightmareHpPhase> HpPhases { get; set; } = new List<KanturuNightmareHpPhase>();
}

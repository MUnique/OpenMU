// <copyright file="BotPvpRules.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// The single source of truth for when a server-side bot may attack a player.
/// </summary>
/// <remarks>
/// Invariant: a bot must never escalate its own <see cref="HeroState"/>. A bot which turns outlaw
/// is a broken toy - it can be killed penalty-free forever, loses the warp command, and visibly
/// marks itself as a misbehaving AI. The game escalates the killer's hero state (see
/// <c>Player.AfterKilledPlayerAsync</c>) unless the victim is already an outlaw or the kill happened
/// in active self-defense (duels and rival-guild wars are exempt as well, but bots have neither),
/// so those two cases are exactly what this rule allows.
/// The bot's grudge memory (<see cref="Offline.OfflinePlayer.RecentAggressor"/>, ~5 minutes, and the
/// revenge march after a death) is deliberately longer than the game's self-defense window: the
/// grudge only decides WHOM the bot prioritizes and where it walks; whether it may actually strike
/// is decided here, per attack, against the game's own rules.
/// </remarks>
public static class BotPvpRules
{
    /// <summary>
    /// How much of the self-defense window must still remain for an attack to count as safe. The
    /// legality check runs when the attack is issued, but the kill (and with it the game's own
    /// self-defense evaluation) can land moments later - without a margin, a final blow right at
    /// the window's edge would escalate the bot's hero state after all. While the player keeps
    /// attacking, every damaging hit renews the window, so the margin never interrupts an ongoing fight.
    /// </summary>
    private static readonly TimeSpan SelfDefenseSafetyMargin = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Determines whether the bot may legally attack the given player, i.e. without any risk of
    /// escalating the bot's own <see cref="HeroState"/>.
    /// </summary>
    /// <param name="bot">The bot (or offline player) which wants to attack.</param>
    /// <param name="target">The player it wants to attack.</param>
    /// <returns><c>true</c> if attacking is free of PK consequences; otherwise, <c>false</c>.</returns>
    public static bool IsLegalPvpTarget(Player bot, Player target)
    {
        // A running mini game with free player killing (Chaos Castle): every fellow participant
        // is fair game - such kills never escalate the hero state (see Player.OnDeathAsync), the
        // game's self-defense bookkeeping doesn't even track them. Gated on the running state, so
        // bots don't swing at players during the countdown before the event starts.
        if (!ReferenceEquals(bot, target)
            && bot.CurrentMiniGame is { AllowPlayerKilling: true, IsEventRunning: true } miniGame
            && ReferenceEquals(target.CurrentMiniGame, miniGame))
        {
            return true;
        }

        // Outlaws are fair game for everyone - killing them never escalates the killer's state.
        if (target.SelectedCharacter?.State >= HeroState.PlayerKiller1stStage)
        {
            return true;
        }

        // Active self-defense: the target attacked this bot recently (SelfDefenseState is keyed
        // (attacker, defender) and renewed on every damaging hit by the SelfDefensePlugIn).
        if (bot.GameContext.SelfDefenseState.TryGetValue((target, bot), out var timeout)
            && timeout > DateTime.UtcNow.Add(SelfDefenseSafetyMargin))
        {
            return true;
        }

        return false;
    }
}

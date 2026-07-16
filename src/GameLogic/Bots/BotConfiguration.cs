// <copyright file="BotConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// The admin-panel editable configuration of the <see cref="BotFeaturePlugIn"/>.
/// </summary>
public class BotConfiguration
{
    /// <summary>
    /// The hard limit of characters a single account can hold in the game.
    /// </summary>
    public const int MaxCharactersPerAccountLimit = 5;

    /// <summary>
    /// Gets or sets a value indicating whether the bot feature is enabled.
    /// Disabled by default so that enabling bots is always an explicit, deliberate action.
    /// </summary>
    [Display(Name = "Enabled", Description = "If enabled, bots are spawned after the server has started.")]
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether all bot accounts and characters should be deleted.
    /// When set, the feature purges every bot account on the next startup before generating fresh
    /// ones, and then automatically clears this flag again. Use it to reset the bot population.
    /// </summary>
    [Display(Name = "Reset bots", Description = "Deletes all bot accounts and characters on the next start, then regenerates them. Clears itself afterwards.")]
    public bool ResetBots { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the bot population rotates its presence over the day:
    /// fewer bots are online at night, most in the evening, with bots smoothly logging in and out -
    /// like a real player base, instead of the same characters being online 24/7.
    /// </summary>
    [Display(Name = "Presence rotation", Description = "Bots log in and out over the day (fewest at night, most in the evening) instead of all being online 24/7.")]
    public bool PresenceRotation { get; set; } = true;

    /// <summary>
    /// Gets or sets the share (in percent) of bots which stays online at the quietest time of day.
    /// 100 effectively disables the rotation effect.
    /// </summary>
    [Display(Name = "Min. online share %", Description = "Percentage of the bot population which stays online at the quietest hour (100 = no rotation effect).")]
    public int MinOnlineSharePercent { get; set; } = 60;

    /// <summary>
    /// Gets or sets the number of bot accounts. Together with <see cref="MaxCharactersPerAccount"/>
    /// this defines the generated bot population, e.g. 10 accounts × 5 characters = 50 bot characters.
    /// </summary>
    [Display(Name = "Number of accounts", Description = "How many bot accounts to maintain.")]
    [Range(0, 1000)]
    public int NumberOfAccounts { get; set; } = 10;

    /// <summary>
    /// Gets or sets the number of characters per bot account. An account can hold at most
    /// <see cref="MaxCharactersPerAccountLimit"/> (5) characters, so this value is clamped on use.
    /// </summary>
    [Display(Name = "Characters per account", Description = "How many characters each bot account holds (max 5).")]
    [Range(1, MaxCharactersPerAccountLimit)]
    public int MaxCharactersPerAccount { get; set; } = MaxCharactersPerAccountLimit;

    /// <summary>
    /// Gets or sets the share (in percent) of a game server's maximum player count which its bots may
    /// occupy. Bots count towards that limit like players do, and a full server turns new clients away -
    /// so the rest of the capacity stays reserved for real players, who must never be denied a slot by a
    /// bot. The population is split over all configured game servers accordingly (see
    /// <see cref="BotServerPartition"/>); accounts which do not fit stay offline until the servers offer
    /// the room for them.
    /// </summary>
    [Display(Name = "Bot capacity %", Description = "Share of a game server's maximum player count which its bots may occupy; the rest stays reserved for real players.")]
    [Range(1, 100)]
    public int BotCapacityPercent { get; set; } = 60;

    /// <summary>
    /// Gets or sets a value indicating whether bots pay the configured reset costs (zen, reset items)
    /// when they reset their character on a server with the reset feature enabled. Off by default:
    /// bots don't take part in the player economy the costs are balanced for, so charging them only
    /// stalls their progression (a bot can't farm zen for a billion-zen reset the way players trade).
    /// </summary>
    [Display(Name = "Bots pay reset costs", Description = "If enabled, bots consume the configured zen/item costs for their resets like human players (default: free bot resets).")]
    public bool BotsPayResetCosts { get; set; }

    /// <summary>
    /// Gets or sets a comma separated list of login names of existing accounts to animate as bots.
    /// This is an optional extra hook alongside the generated population (see
    /// <see cref="NumberOfAccounts"/>): every listed account gets a bot driving its first character.
    /// These accounts are animated as-is and are not part of the partitioned, capacity-limited
    /// population, so leave it empty unless you specifically want to drive existing accounts.
    /// </summary>
    [Display(Name = "Extra accounts to animate", Description = "Comma separated login names of existing accounts to animate as bots, in addition to the generated population.")]
    public string ProofOfConceptAccounts { get; set; } = string.Empty;

    /// <summary>
    /// Gets the effective, clamped number of characters per account.
    /// </summary>
    /// <returns>A value between 1 and <see cref="MaxCharactersPerAccountLimit"/>.</returns>
    /// <remarks>Deliberately a method: a get-only property would end up in the serialized plugin configuration JSON.</remarks>
    public int GetEffectiveCharactersPerAccount()
        => Math.Clamp(this.MaxCharactersPerAccount, 1, MaxCharactersPerAccountLimit);

    /// <summary>
    /// Gets the effective, clamped share of a server's player capacity which its bots may occupy.
    /// </summary>
    /// <returns>A value between 1 and 100.</returns>
    /// <remarks>Deliberately a method, like <see cref="GetEffectiveCharactersPerAccount"/>.</remarks>
    public int GetEffectiveBotCapacityPercent()
        => Math.Clamp(this.BotCapacityPercent, 1, 100);

    /// <summary>
    /// Parses <see cref="ProofOfConceptAccounts"/> into the distinct, trimmed login names.
    /// </summary>
    /// <returns>The list of login names.</returns>
    /// <remarks>Deliberately a method: a get-only property would end up in the serialized plugin configuration JSON.</remarks>
    public IReadOnlyList<string> ParseProofOfConceptAccounts()
        => this.ProofOfConceptAccounts
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
}

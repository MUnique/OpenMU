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
    /// this defines the bot population, e.g. 10 accounts × 5 characters = 50 bot characters.
    /// </summary>
    /// <remarks>Used by the generation step (next phase). The proof of concept animates the
    /// accounts listed in <see cref="ProofOfConceptAccounts"/> instead.</remarks>
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
    /// Gets or sets a comma separated list of login names of existing accounts to animate as bots.
    /// This is the proof-of-concept entry point: every listed account gets a bot driving its
    /// first character. A later phase replaces this with generated, persistent bot accounts.
    /// </summary>
    [Display(Name = "Proof of concept accounts", Description = "Comma separated login names of existing accounts to animate as bots (PoC).")]
    public string ProofOfConceptAccounts { get; set; } = string.Empty;

    /// <summary>
    /// Gets the effective, clamped number of characters per account.
    /// </summary>
    /// <returns>A value between 1 and <see cref="MaxCharactersPerAccountLimit"/>.</returns>
    public int GetEffectiveCharactersPerAccount()
        => Math.Clamp(this.MaxCharactersPerAccount, 1, MaxCharactersPerAccountLimit);

    /// <summary>
    /// Parses <see cref="ProofOfConceptAccounts"/> into the distinct, trimmed login names.
    /// </summary>
    /// <returns>The list of login names.</returns>
    public IReadOnlyList<string> GetProofOfConceptAccounts()
        => this.ProofOfConceptAccounts
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
}

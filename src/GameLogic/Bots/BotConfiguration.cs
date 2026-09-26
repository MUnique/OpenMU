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
    [Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display1_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display1_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether all bot accounts and characters should be deleted.
    /// When set, the feature purges every bot account on the next startup before generating fresh
    /// ones, and then automatically clears this flag again. Use it to reset the bot population.
    /// </summary>
    [Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display2_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display2_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public bool ResetBots { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether all bot accounts and characters should be deleted
    /// WITHOUT being regenerated. Unlike <see cref="ResetBots"/> this also turns <see cref="Enabled"/>
    /// off - otherwise the very same pass would generate the population again - so it is the single
    /// switch for "I do not want bots on this server anymore". Clears itself afterwards.
    /// </summary>
    [Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display3_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display3_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public bool PurgeBots { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether newly generated bot characters start as fresh
    /// characters, the way a player who just created one does: at level 1 with level-0 starter
    /// equipment, no reset history and only the skills its level-1 stats allow. This only affects
    /// characters which are generated from now on - an existing bot population keeps its state
    /// until it is regenerated (<see cref="ResetBots"/>).
    /// </summary>
    [Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display4_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display4_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public bool StartAsFreshCharacters { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the bot population rotates its presence over the day:
    /// fewer bots are online at night, most in the evening, with bots smoothly logging in and out -
    /// like a real player base, instead of the same characters being online 24/7.
    /// </summary>
    [Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display5_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display5_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public bool PresenceRotation { get; set; } = true;

    /// <summary>
    /// Gets or sets the share (in percent) of bots which stays online at the quietest time of day.
    /// 100 effectively disables the rotation effect.
    /// </summary>
    [Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display6_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display6_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public int MinOnlineSharePercent { get; set; } = 60;

    /// <summary>
    /// Gets or sets the number of bot accounts. Together with <see cref="MaxCharactersPerAccount"/>
    /// this defines the generated bot population, e.g. 10 accounts × 5 characters = 50 bot characters.
    /// </summary>
    [Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display7_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display7_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    [Range(0, 1000)]
    public int NumberOfAccounts { get; set; } = 10;

    /// <summary>
    /// Gets or sets the number of characters per bot account. An account can hold at most
    /// <see cref="MaxCharactersPerAccountLimit"/> (5) characters, so this value is clamped on use.
    /// </summary>
    [Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display8_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display8_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
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
    [Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display9_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display9_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    [Range(1, 100)]
    public int BotCapacityPercent { get; set; } = 60;

    /// <summary>
    /// Gets or sets a value indicating whether bots pay the configured reset costs (zen, reset items)
    /// when they reset their character on a server with the reset feature enabled. Off by default:
    /// bots don't take part in the player economy the costs are balanced for, so charging them only
    /// stalls their progression (a bot can't farm zen for a billion-zen reset the way players trade).
    /// </summary>
    [Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display10_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display10_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public bool BotsPayResetCosts { get; set; }

    /// <summary>
    /// Gets or sets how many Jewels of Bless, Soul and Life a bot keeps of each kind. Bots only pick up
    /// the jewels they can actually spend on their own gear (see <c>BotJewelHandler</c>) and stop
    /// collecting a kind once they hold this many; whatever they carry above it is sold on the next
    /// merchant visit. The sensible value depends entirely on the server's drop rates - on a high rate
    /// server a bot refills a big stock within hours, so a low limit keeps its backpack usable.
    /// </summary>
    [Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display11_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display11_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    [Range(0, 100)]
    public int JewelStockPerKind { get; set; } = 10;

    /// <summary>
    /// Gets or sets the number of potion charges (per healing and per mana potions) a bot stocks up to
    /// at a merchant. Merchants sell potions in stacks of different sizes, so this is the target the bot
    /// buys towards, not a stack count.
    /// </summary>
    [Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display12_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display12_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    [Range(10, 255)]
    public int PotionStockCharges { get; set; } = 60;

    /// <summary>
    /// Gets or sets a comma separated list of login names of existing accounts to animate as bots.
    /// This is an optional extra hook alongside the generated population (see
    /// <see cref="NumberOfAccounts"/>): every listed account gets a bot driving its first character.
    /// These accounts are animated as-is and are not part of the partitioned, capacity-limited
    /// population, so leave it empty unless you specifically want to drive existing accounts.
    /// </summary>
    [Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display13_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BotConfiguration_Display13_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
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
    /// Gets the effective, clamped jewel stock a bot keeps of each usable kind.
    /// </summary>
    /// <returns>A value between 0 and 100.</returns>
    /// <remarks>Deliberately a method, like <see cref="GetEffectiveCharactersPerAccount"/>.</remarks>
    public int GetEffectiveJewelStockPerKind()
        => Math.Clamp(this.JewelStockPerKind, 0, 100);

    /// <summary>
    /// Gets the effective, clamped potion charges a bot stocks up to.
    /// </summary>
    /// <returns>A value between 10 and 255.</returns>
    /// <remarks>Deliberately a method, like <see cref="GetEffectiveCharactersPerAccount"/>.</remarks>
    public int GetEffectivePotionStockCharges()
        => Math.Clamp(this.PotionStockCharges, 10, 255);

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

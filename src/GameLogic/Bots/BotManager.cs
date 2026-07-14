// <copyright file="BotManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Collections.Concurrent;
using System.Linq;
using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// Manages the lifecycle of server-side bots.
/// </summary>
/// <remarks>
/// A bot reuses the connection-less <see cref="OfflinePlayer"/> together with its MU Helper AI,
/// but is spawned in a fully standalone way: the account is loaded fresh in the bot's own
/// persistence context (see <see cref="OfflinePlayer.InitializeAsync"/>), so there is no
/// cross-context attach of entities owned by another player - which is the root cause of the
/// known data-corruption issue of the <c>/offlevel</c> handover. Because each bot drives a
/// distinct character in its own context, several bots can animate different characters of the
/// same account at once (the shared account row is only attached, never modified).
/// </remarks>
public sealed class BotManager
{
    private readonly ConcurrentDictionary<string, BotPlayer> _bots = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets a snapshot of the currently active bots.
    /// </summary>
    public IReadOnlyCollection<BotPlayer> Bots => this._bots.Values.ToList();

    /// <summary>
    /// Spawns a bot which drives a specific character of the given account.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="loginName">The login name of an existing account.</param>
    /// <param name="characterSlot">The character slot to drive; <c>null</c> drives the first character by slot.</param>
    /// <returns><c>true</c> if a bot was started; <c>false</c> if it could not be started or was already active.</returns>
    public async ValueTask<bool> SpawnBotAsync(IGameContext gameContext, string loginName, byte? characterSlot = null)
    {
        if (string.IsNullOrWhiteSpace(loginName))
        {
            return false;
        }

        var bot = new BotPlayer(gameContext);
        var added = false;
        string? key = null;
        try
        {
            // Load the account through the bot's OWN persistence context (no cross-context attach).
            var account = await bot.PersistenceContext.GetAccountByLoginNameAsync(loginName).ConfigureAwait(false);
            var character = characterSlot is { } slot
                ? account?.Characters.FirstOrDefault(c => c.CharacterSlot == slot)
                : account?.Characters.OrderBy(c => c.CharacterSlot).FirstOrDefault();
            if (account is null || character is null)
            {
                bot.Logger.LogWarning("Bot account '{LoginName}' (slot {Slot}) could not be loaded or has no character.", loginName, characterSlot);
                await bot.DisposeAsync().ConfigureAwait(false);
                return false;
            }

            key = GetKey(loginName, character.CharacterSlot);
            if (!this._bots.TryAdd(key, bot))
            {
                // Already animating this character.
                await bot.DisposeAsync().ConfigureAwait(false);
                return false;
            }

            added = true;

            // Provide AI settings so the bot actually hunts (a missing config means a single-tile range).
            bot.MuHelperSettings = new BotMuHelperSettings();

            if (!await bot.InitializeAsync(loginName, character.Name).ConfigureAwait(false))
            {
                await this.RemoveAndDisposeAsync(key, bot).ConfigureAwait(false);
                return false;
            }

            BotSkillProgressionPlugIn.CatchUpPendingProgress(bot);

            bot.Logger.LogInformation("Bot started for account '{LoginName}', character '{Character}'.", loginName, character.Name);
            return true;
        }
        catch (Exception ex)
        {
            bot.Logger.LogError(ex, "Failed to spawn bot for account '{LoginName}' (slot {Slot}).", loginName, characterSlot);
            if (added && key is not null)
            {
                await this.RemoveAndDisposeAsync(key, bot).ConfigureAwait(false);
            }
            else
            {
                await bot.DisposeAsync().ConfigureAwait(false);
            }

            return false;
        }
    }

    /// <summary>
    /// Stops and removes all currently active bots.
    /// </summary>
    /// <returns>The task.</returns>
    public async ValueTask StopAllAsync()
    {
        foreach (var key in this._bots.Keys.ToList())
        {
            if (this._bots.TryRemove(key, out var bot))
            {
                await StopAndDisposeAsync(bot, key, "shutdown").ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Determines whether the given character of the given account is currently animated by a bot.
    /// </summary>
    /// <param name="loginName">The account login name.</param>
    /// <param name="slot">The character slot.</param>
    public bool IsActive(string loginName, byte slot) => this._bots.ContainsKey(GetKey(loginName, slot));

    /// <summary>
    /// Stops one randomly chosen bot (used by the presence rotation, so the population ebbs and flows
    /// like a real player base). The bot leaves its party cleanly first, then disconnects - which also
    /// saves its progress, like a regular logout.
    /// </summary>
    /// <returns>The name of the stopped bot's character, or null if no bot was active.</returns>
    public async ValueTask<string?> StopRandomBotAsync()
    {
        var key = this._bots.Keys.ToList().SelectRandom();
        if (key is null || !this._bots.TryRemove(key, out var bot))
        {
            return null;
        }

        var name = bot.Name;
        try
        {
            if (bot.Party is { } party)
            {
                await party.KickMySelfAsync(bot).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            // A failed party goodbye must not skip the stop below - the party cleans up a
            // disconnected member itself.
            bot.Logger.LogWarning(ex, "Bot '{Key}' couldn't leave its party for the presence rotation.", key);
        }

        await StopAndDisposeAsync(bot, key, "presence rotation").ConfigureAwait(false);
        return name;
    }

    /// <summary>
    /// Stops the given bot like a regular logout and immediately brings the same character back
    /// online - the ghost equivalent of a player relogging. Used after the master evolution: the
    /// master class's base attributes (master experience rate, master points per level) and the
    /// master level stat are only mounted when the character enters the world, so the class change
    /// must be followed by a fresh world entry before master experience can flow.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="bot">The bot to restart.</param>
    /// <returns><c>true</c> if the bot came back online.</returns>
    public async ValueTask<bool> RestartBotAsync(IGameContext gameContext, BotPlayer bot)
    {
        // Captured before the stop - the disposed bot loses its account and character.
        var loginName = bot.Account?.LoginName;
        var characterSlot = bot.SelectedCharacter?.CharacterSlot;
        if (loginName is null
            || characterSlot is not { } slot
            || !this._bots.TryRemove(GetKey(loginName, slot), out var removed))
        {
            return false;
        }

        if (removed.Party is { } party)
        {
            try
            {
                await party.KickMySelfAsync(removed).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // A failed party goodbye must not skip the stop below - the party cleans up a
                // disconnected member itself.
                removed.Logger.LogWarning(ex, "Bot '{Login}/{Slot}' couldn't leave its party for the restart.", loginName, slot);
            }
        }

        try
        {
            await removed.StopAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // The old instance may still be (partially) alive - put it back under management and
            // don't spawn a second player driving the same character (two persistence contexts
            // saving one character would be last-write-wins data loss). Retried on a later pass.
            removed.Logger.LogError(ex, "Error while stopping bot '{Login}/{Slot}' for a restart; keeping the old instance.", loginName, slot);
            this._bots.TryAdd(GetKey(loginName, slot), removed);
            return false;
        }

        // The stopped instance is done for good - release its persistence context and the tracked
        // account graph before the fresh one loads them again.
        await removed.DisposeAsync().ConfigureAwait(false);

        return await this.SpawnBotAsync(gameContext, loginName, slot).ConfigureAwait(false);
    }

    /// <summary>
    /// Groups a share of the active bots into small hunting parties of level-wise similar characters,
    /// like real players do: the party members follow their leader (see the follow logic in
    /// <see cref="BotNavigator"/>), the elf heals the group, buffs are shared and the party experience
    /// bonus applies. The rest of the bots keep hunting solo, so the population stays varied.
    /// </summary>
    /// <param name="gameContext">The game context (provides the party manager).</param>
    public async ValueTask FormPartiesAsync(IGameContext gameContext)
    {
        const int minPartySize = 2;
        const int maxPartySize = 5;
        const int maxLevelGap = 12;
        const int partiedSharePercent = 60;

        // Matched by the reset-aware effective level (see BotResetHandler.GetEffectiveLevel), so on a
        // reset server a freshly reset veteran groups with its peers instead of with real newbies.
        var candidates = this._bots.Values
            .Where(b => b.Party is null && b.Attributes is not null)
            .OrderBy(BotResetHandler.GetEffectiveLevel)
            .ToList();

        var index = 0;
        while (index < candidates.Count - 1)
        {
            if (Rand.NextInt(0, 100) >= partiedSharePercent)
            {
                index++; // this bot stays solo
                continue;
            }

            var leader = candidates[index];
            var leaderLevel = BotResetHandler.GetEffectiveLevel(leader);
            var targetSize = Rand.NextInt(minPartySize, maxPartySize + 1);
            var members = new List<BotPlayer> { leader };
            var next = index + 1;
            while (next < candidates.Count
                   && members.Count < targetSize
                   && BotResetHandler.GetEffectiveLevel(candidates[next]) - leaderLevel <= maxLevelGap)
            {
                members.Add(candidates[next]);
                next++;
            }

            if (members.Count >= minPartySize)
            {
                var party = gameContext.PartyManager.CreateParty();
                foreach (var member in members)
                {
                    if (!await party.AddAsync(member).ConfigureAwait(false))
                    {
                        break;
                    }
                }

                leader.Logger.LogInformation(
                    "Formed bot party of {Count} around '{Leader}' (level {Level}).",
                    members.Count,
                    leader.Name,
                    leaderLevel);
            }

            index = next;
        }
    }

    private static string GetKey(string loginName, byte slot) => $"{loginName}/{slot}";

    /// <summary>
    /// Stops the bot like a regular logout (which saves its progress) and releases its resources.
    /// A stopped-but-not-disposed player keeps its persistence context - and with it the whole tracked
    /// account graph - alive; with the presence rotation stopping bots around the clock, that adds up
    /// to a leak. Mirrors the teardown of the <see cref="Offline.OfflinePlayerManager"/>; the removal
    /// from the game context happens through the disconnect event.
    /// </summary>
    private static async ValueTask StopAndDisposeAsync(BotPlayer bot, string key, string reason)
    {
        try
        {
            await bot.StopAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            bot.Logger.LogError(ex, "Error while stopping bot '{Key}' ({Reason}).", key, reason);
        }
        finally
        {
            await bot.DisposeAsync().ConfigureAwait(false);
        }
    }

    private async ValueTask RemoveAndDisposeAsync(string key, BotPlayer bot)
    {
        this._bots.TryRemove(key, out _);
        await bot.DisposeAsync().ConfigureAwait(false);
    }
}

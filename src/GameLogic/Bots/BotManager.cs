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

            if (!await bot.InitializeAsync(account, character).ConfigureAwait(false))
            {
                await this.RemoveAndDisposeAsync(key, bot).ConfigureAwait(false);
                return false;
            }

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
                try
                {
                    await bot.StopAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    bot.Logger.LogError(ex, "Error while stopping bot '{Key}'.", key);
                }
            }
        }
    }

    private static string GetKey(string loginName, byte slot) => $"{loginName}/{slot}";

    private async ValueTask RemoveAndDisposeAsync(string key, BotPlayer bot)
    {
        this._bots.TryRemove(key, out _);
        await bot.DisposeAsync().ConfigureAwait(false);
    }
}

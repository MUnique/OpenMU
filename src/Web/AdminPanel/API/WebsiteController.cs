// <copyright file="WebsiteController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.WeeklyQuests;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// API for the community website: who is in the game, and the tools of the player's account panel.
/// </summary>
/// <remarks>
/// It only exposes what the game server knows and the database doesn't (the in-memory session state).
/// The website decides which account the signed-in visitor owns, so the API key must never leave the
/// website's server.
/// </remarks>
[Route("api/web/")]
[Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.ApiSchemes, Policy = AdminPolicies.Viewer)]
public class WebsiteController : Controller
{
    /// <summary>
    /// The culture of the texts for the website, e.g. the names of the reward items.
    /// </summary>
    private static readonly CultureInfo WebsiteCulture = CultureInfo.GetCultureInfo("es");

    private readonly IDictionary<int, IGameServer> _gameServers;
    private readonly ILoginServer _loginServer;
    private readonly IPersistenceContextProvider _contextProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebsiteController"/> class.
    /// </summary>
    /// <param name="gameServers">The game servers.</param>
    /// <param name="loginServer">The login server.</param>
    /// <param name="contextProvider">The persistence context provider.</param>
    public WebsiteController(IDictionary<int, IGameServer> gameServers, ILoginServer loginServer, IPersistenceContextProvider contextProvider)
    {
        this._gameServers = gameServers;
        this._loginServer = loginServer;
        this._contextProvider = contextProvider;
    }

    /// <summary>
    /// Gets the names of the characters which are currently playing.
    /// </summary>
    /// <returns>The online character names.</returns>
    [HttpGet]
    [Route("online")]
    public async Task<IActionResult> GetOnlineAsync()
    {
        var names = new List<string>();
        foreach (var server in this._gameServers.Values.OfType<GameServer>())
        {
            var players = await server.Context.GetPlayersAsync().ConfigureAwait(false);
            names.AddRange(players.Select(p => p.SelectedCharacter?.Name).OfType<string>());
        }

        return this.Ok(new { characters = names });
    }

    /// <summary>
    /// Gets the session state of an account.
    /// </summary>
    /// <param name="login">The login name of the account.</param>
    /// <returns>
    /// <c>connected</c>: the login server considers the account logged in;
    /// <c>inGame</c>: a game server really has a player of this account.
    /// A connected account which is not in game is a stuck session.
    /// </returns>
    [HttpGet]
    [Route("account/{login}")]
    public async Task<IActionResult> GetAccountAsync(string login)
    {
        var connected = (await this._loginServer.GetSnapshotAsync().ConfigureAwait(false))
            .Keys.Any(k => string.Equals(k, login, StringComparison.OrdinalIgnoreCase));
        var (inGame, character) = await this.FindPlayerAsync(login).ConfigureAwait(false);
        return this.Ok(new { connected, inGame, character });
    }

    /// <summary>
    /// Ends the session of an account: kicks its player, if any, and releases the login.
    /// </summary>
    /// <param name="login">The login name of the account.</param>
    /// <returns>Whether a player was kicked and whether a stuck login was released.</returns>
    [HttpPost]
    [Route("account/{login}/disconnect")]
    [Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.ApiSchemes, Policy = AdminPolicies.Operator)]
    public async Task<IActionResult> DisconnectAccountAsync(string login)
    {
        var kicked = false;
        foreach (var server in this._gameServers.Values)
        {
            kicked |= await server.DisconnectAccountAsync(login).ConfigureAwait(false);
        }

        // Same as the "set offline" button of the admin panel: it also clears a login which has no player.
        var snapshot = await this._loginServer.GetSnapshotAsync().ConfigureAwait(false);
        var released = false;
        foreach (var (name, serverId) in snapshot.Where(e => string.Equals(e.Key, login, StringComparison.OrdinalIgnoreCase)))
        {
            await this._loginServer.LogOffAsync(name, serverId).ConfigureAwait(false);
            released = true;
        }

        return this.Ok(new { kicked, released });
    }

    /// <summary>
    /// Gets the weekly quests of the characters of an account, with their progress in the current week.
    /// </summary>
    /// <param name="login">The login name of the account.</param>
    /// <returns>
    /// <c>enabled</c>: whether the weekly quests plugin is active;
    /// <c>nextResetUtc</c>: when the next week starts;
    /// <c>characters</c>: the quests per character, like they're shown in the game.
    /// </returns>
    /// <remarks>
    /// The progress of a character which is in the game is saved every minute, so it may be a bit behind.
    /// </remarks>
    [HttpGet]
    [Route("account/{login}/weekly-quests")]
    public async Task<IActionResult> GetWeeklyQuestsAsync(string login)
    {
        var context = this._gameServers.Values.OfType<GameServer>().FirstOrDefault()?.Context;
        var plugInId = typeof(WeeklyQuestsPlugIn).GUID;
        if (context is null
            || WeeklyQuestProgressRepositoryRegistry.Current is not { } repository
            || !context.PlugInManager.IsPlugInActive(plugInId)
            || context.Configuration.PlugInConfigurations.FirstOrDefault(c => c.TypeId == plugInId)
                ?.GetConfiguration<WeeklyQuestsConfiguration>(context.PlugInManager.CustomConfigReferenceHandler) is not { } configuration)
        {
            return this.Ok(new { enabled = false });
        }

        using var playerContext = this._contextProvider.CreateNewPlayerContext(context.Configuration);
        var account = await playerContext.GetAccountByLoginNameAsync(login).ConfigureAwait(false);
        if (account is null)
        {
            return this.NotFound();
        }

        var periodStart = WeeklyPeriod.GetPeriodStartUtc(DateTime.UtcNow, configuration.ResetDay, configuration.ResetTime, context.ServerTimeZone);
        var progressPerCharacter = new Dictionary<Guid, Dictionary<string, WeeklyQuestProgress>>();
        foreach (var character in account.Characters)
        {
            var id = character.GetId();
            var loaded = await repository.LoadAsync(id, periodStart).ConfigureAwait(false);
            progressPerCharacter[id] = loaded.ToDictionary(p => p.QuestId);
        }

        var characters = account.Characters
            .OrderBy(c => c.CharacterSlot)
            .Select(character =>
            {
                var id = character.GetId();
                var rewardedByOthers = progressPerCharacter
                    .Where(p => p.Key != id)
                    .SelectMany(p => p.Value.Values)
                    .Where(p => p.RewardedAt is not null)
                    .Select(p => p.QuestId)
                    .ToHashSet();
                var info = new WeeklyQuestCharacterInfo(
                    character.CharacterClass,
                    (int)(character.Attributes.FirstOrDefault(a => a.Definition == Stats.Level)?.Value ?? 1),
                    (int)(character.Attributes.FirstOrDefault(a => a.Definition == Stats.Resets)?.Value ?? 0));
                var quests = WeeklyQuestSelector.CreateEntries(configuration, periodStart, info, progressPerCharacter[id], rewardedByOthers)
                    .Select(e => new
                    {
                        id = e.Quest.Id,
                        name = e.Quest.Name,
                        description = e.Quest.Description,
                        count = e.Count,
                        required = e.Quest.RequiredCount,
                        completed = e.IsCompleted,
                        rewarded = e.IsRewarded,
                        rewards = e.Quest.GetRewardsText(WebsiteCulture),
                        isBonus = e.Quest.Id == WeeklyQuestSelector.AllCompletedBonusId,
                    })
                    .ToList();
                return new { name = character.Name, quests };
            })
            .ToList();

        var nextResetUtc = WeeklyPeriod.GetNextPeriodStartUtc(periodStart, context.ServerTimeZone);
        return this.Ok(new { enabled = true, nextResetUtc, characters });
    }

    private async Task<(bool InGame, string? Character)> FindPlayerAsync(string login)
    {
        foreach (var server in this._gameServers.Values.OfType<GameServer>())
        {
            var players = await server.Context.GetPlayersAsync().ConfigureAwait(false);
            var player = players.FirstOrDefault(p => string.Equals(p.Account?.LoginName, login, StringComparison.OrdinalIgnoreCase));
            if (player is not null)
            {
                return (true, player.SelectedCharacter?.Name);
            }
        }

        return (false, null);
    }
}

// <copyright file="WebsiteOperations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// The functions of the website API, which a website of the server uses with an API key
/// with the <see cref="AdminRoles.WebManager"/> role.
/// </summary>
/// <remarks>
/// Each function needs its <see cref="WebsitePermissions"/> entry in the allowlist of the key.
/// The results only contain what a website may show to its visitors, so a website never needs
/// a key which can read the admin views of accounts and characters.
/// </remarks>
/// <param name="servers">The server provider.</param>
/// <param name="persistence">The persistence context provider.</param>
/// <param name="configurations">The configuration data source.</param>
/// <param name="accounts">The account operations, which the registration shares with the admin panel.</param>
/// <param name="rankingCache">The cache of the rankings.</param>
/// <param name="miniGames">The mini game operations, which provide the leaderboards of the admin panel.</param>
/// <param name="executor">The authorization and audit service.</param>
public sealed class WebsiteOperations(
    IServerProvider servers,
    IPersistenceContextProvider persistence,
    IDataSource<GameConfiguration> configurations,
    AccountWriteOperations accounts,
    WebsiteRankingCache rankingCache,
    MiniGameReadOperations miniGames,
    AdminOperationExecutor executor)
{
    /// <summary>
    /// The maximum number of entries of a ranking.
    /// </summary>
    internal const int MaximumRankingCount = AdminReadResult.MaximumRows;

    /// <summary>Reads the public state of the game servers.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The game servers with their state and number of players.</returns>
    [AdminOperation("WebManager")]
    [NotExposedToAgents("Public projection for websites; agents use server_overview.")]
    public async ValueTask<WebsiteServerStatus> StatusAsync(CancellationToken cancellationToken)
    {
        await executor.RequireOperationAsync(AdminRoles.WebManager, WebsitePermissions.Status, cancellationToken).ConfigureAwait(false);
        var gameServers = servers.Servers
            .OfType<IGameServer>()
            .OrderBy(server => server.Id)
            .Take(AdminReadResult.MaximumRows)
            .Select(server => new WebsiteGameServerStatus(
                server.Id,
                server.Description,
                server.ServerState == ServerState.Started,
                server.CurrentConnections,
                server.MaximumConnections))
            .ToList();
        return new WebsiteServerStatus(gameServers.Sum(server => server.Players), gameServers);
    }

    /// <summary>Registers a new account, which can log in right away.</summary>
    /// <param name="loginName">The login name of the new account.</param>
    /// <param name="password">The password of the new account; never recorded in the audit trail.</param>
    /// <param name="email">The optional e-mail address.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c>, if the account was created; <c>false</c>, if the login name is already taken.</returns>
    /// <remarks>
    /// The account is created exactly like by the admin panel, and the registration is audited with the name of the key.
    /// The website has to protect its form against abuse (a captcha, a limit per visitor): this server only sees the website.
    /// </remarks>
    [AdminOperation("WebManager", ReadOnly = false)]
    [NotExposedToAgents("Registration form of websites; agents use accounts_create.")]
    public async ValueTask<bool> RegisterAsync(string loginName, string password, string? email, CancellationToken cancellationToken)
    {
        var result = await executor.ExecuteAsync(
            AdminRoles.WebManager,
            WebsitePermissions.Register,
            new { loginName, email },
            _ => accounts.CreateAccountAsync(loginName, password, email, cancellationToken),
            cancellationToken).ConfigureAwait(false);
        return result["status"]?.GetValue<string>() == "created";
    }

    /// <summary>Reads the best characters, ranked by resets, master level and level.</summary>
    /// <param name="count">The number of characters, up to <see cref="MaximumRankingCount"/>.</param>
    /// <param name="characterClassNumber">
    /// The number of a character class, to rank only the characters of its line, e.g. Dark Wizard, Soul Master
    /// and Grand Master; <c>null</c> for all classes.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The ranked characters.</returns>
    /// <remarks>
    /// Bots, templates and game masters are not ranked. The query is the one of the ranking of the Discord bot;
    /// it reads all characters. The guilds of the ranked characters are looked up afterwards, one membership per
    /// character and the names of all their guilds at once. Like the ranking of the Discord bot, the result is cached
    /// for <see cref="WebsiteRankingCache.Duration"/> (2 minutes), so it may be that old, see <see cref="WebsiteRankingCache"/>.
    /// </remarks>
    [AdminOperation("WebManager")]
    [NotExposedToAgents("Public projection for websites; agents use characters_get and the leaderboards.")]
    public async ValueTask<IReadOnlyList<WebsiteRankingEntry>> RankingAsync(int count, byte? characterClassNumber, CancellationToken cancellationToken)
    {
        await executor.RequireOperationAsync(AdminRoles.WebManager, WebsitePermissions.Ranking, cancellationToken).ConfigureAwait(false);
        if (count is < 1 or > MaximumRankingCount)
        {
            throw new AdminOperationException($"The {nameof(count)} must be between 1 and {MaximumRankingCount}.", AdminOperationErrorKind.InvalidInput);
        }

        var gameConfiguration = await configurations.GetOwnerAsync(Guid.Empty, cancellationToken).ConfigureAwait(false);
        HashSet<CharacterClass>? classLine = null;
        if (characterClassNumber is { } number)
        {
            var characterClass = gameConfiguration.CharacterClasses.FirstOrDefault(c => c.Number == number)
                ?? throw new AdminOperationException("The character class is unknown.", AdminOperationErrorKind.InvalidInput);
            classLine = GetClassLine(gameConfiguration, characterClass);
        }

        var ranking = await rankingCache.GetOrLoadAsync(
            classLine?.Min(c => c.Number),
            token => this.LoadRankingAsync(gameConfiguration, classLine, token),
            DateTime.UtcNow,
            cancellationToken).ConfigureAwait(false);
        return ranking.Take(count).ToList();
    }

    /// <summary>Reads the leaderboard of a mini game type, with all its levels combined.</summary>
    /// <param name="miniGameType">The type of the mini game; <c>null</c> for the first one which has a leaderboard.</param>
    /// <param name="days">The number of past days of which finished games are counted; <c>null</c> for all time.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The leaderboard, and the mini game types which have one.</returns>
    /// <remarks>
    /// It's the leaderboard of the admin panel, which is only kept for the mini games which save their ranking statistics.
    /// A mini game which starts to save them later shows up here without a change of the website API.
    /// </remarks>
    [AdminOperation("WebManager")]
    [NotExposedToAgents("Public projection for websites; agents use minigames_leaderboard.")]
    public async ValueTask<WebsiteMiniGameRanking> MiniGameRankingAsync(MiniGameType? miniGameType, int? days, CancellationToken cancellationToken)
    {
        await executor.RequireOperationAsync(AdminRoles.WebManager, WebsitePermissions.MiniGameRanking, cancellationToken).ConfigureAwait(false);
        MiniGameLeaderboard leaderboard;
        try
        {
            leaderboard = await miniGames.LeaderboardAsync(miniGameType, days, cancellationToken).ConfigureAwait(false);
        }
        catch (ArgumentException error)
        {
            throw new AdminOperationException(error.Message, AdminOperationErrorKind.InvalidInput);
        }

        // The leaderboard of the admin panel names the classes in the culture of the process; the website gets them in english.
        var gameConfiguration = await configurations.GetOwnerAsync(Guid.Empty, cancellationToken).ConfigureAwait(false);
        var englishClassNames = gameConfiguration.CharacterClasses
            .GroupBy(characterClass => characterClass.Name.ToString() ?? string.Empty)
            .ToDictionary(group => group.Key, group => group.First().Name.ValueInNeutralLanguage);
        return new WebsiteMiniGameRanking(
            leaderboard.MiniGameType?.ToString(),
            leaderboard.AvailableTypes.Select(type => type.ToString()).ToList(),
            leaderboard.Days,
            leaderboard.Entries.Select(entry => entry with { ClassName = englishClassNames.GetValueOrDefault(entry.ClassName, entry.ClassName) }).ToList());
    }

    /// <summary>
    /// Gets the classes of the line of a character class, which are connected by their <see cref="CharacterClass.NextGenerationClass"/>.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="characterClass">A class of the line.</param>
    /// <returns>All classes of the line.</returns>
    private static HashSet<CharacterClass> GetClassLine(GameConfiguration gameConfiguration, CharacterClass characterClass)
    {
        var line = new HashSet<CharacterClass> { characterClass };
        bool added;
        do
        {
            added = false;
            foreach (var candidate in gameConfiguration.CharacterClasses)
            {
                var isConnected = (candidate.NextGenerationClass is { } next && line.Contains(next))
                    || line.Any(member => member.NextGenerationClass == candidate);
                if (isConnected && line.Add(candidate))
                {
                    added = true;
                }
            }
        }
        while (added);

        return line;
    }

    /// <summary>
    /// Loads the ranking with all <see cref="MaximumRankingCount"/> entries, for the <see cref="WebsiteRankingCache"/>.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="classLine">The classes of the ranked line; <c>null</c> for all classes.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The ranked characters.</returns>
    private async ValueTask<IReadOnlyList<WebsiteRankingEntry>> LoadRankingAsync(GameConfiguration gameConfiguration, HashSet<CharacterClass>? classLine, CancellationToken cancellationToken)
    {
        var classIds = classLine?.Select(c => c.GetId()).ToList();
        using var context = persistence.CreateNewPlayerContext(gameConfiguration);
        var ranking = await context.GetCharacterRankingAsync(Stats.Level.Id, Stats.MasterLevel.Id, Stats.Resets.Id, MaximumRankingCount, classIds, cancellationToken).ConfigureAwait(false);

        // The ranking query only knows the identifier of the class; its name is configuration.
        var classNames = gameConfiguration.CharacterClasses.ToDictionary(characterClass => characterClass.GetId(), characterClass => characterClass.Name.ValueInNeutralLanguage);
        var guildNames = await this.GetGuildNamesAsync(ranking.Select(entry => entry.Name), cancellationToken).ConfigureAwait(false);
        return ranking
            .Select((entry, index) => new WebsiteRankingEntry(
                index + 1,
                entry.Name,
                entry.CharacterClassId is { } classId && classNames.TryGetValue(classId, out var className) ? className : entry.CharacterClassName,
                entry.Level,
                entry.MasterLevel,
                entry.Resets,
                guildNames.GetValueOrDefault(entry.Name)))
            .ToList();
    }

    /// <summary>
    /// Gets the names of the guilds of characters.
    /// </summary>
    /// <param name="characterNames">The names of the characters, at most <see cref="MaximumRankingCount"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The guild name per character name; characters without a guild are missing.</returns>
    private async ValueTask<IReadOnlyDictionary<string, string>> GetGuildNamesAsync(IEnumerable<string> characterNames, CancellationToken cancellationToken)
    {
        using var guildContext = persistence.CreateNewGuildContext();
        var guildIds = new Dictionary<string, Guid>();
        foreach (var characterName in characterNames)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (await guildContext.GetGuildMembershipAsync(characterName).ConfigureAwait(false) is { } membership)
            {
                guildIds[characterName] = membership.GuildId;
            }
        }

        if (guildIds.Count == 0)
        {
            return new Dictionary<string, string>();
        }

        // Many ranked characters share a guild, so each guild name is read only once.
        var guildNames = await guildContext.GetPersistentGuildNamesAsync(guildIds.Values.Distinct().ToList()).ConfigureAwait(false);
        return guildIds
            .Where(pair => guildNames.ContainsKey(pair.Value))
            .ToDictionary(pair => pair.Key, pair => guildNames[pair.Value]);
    }
}

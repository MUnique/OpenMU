// <copyright file="WebsiteOperations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

using System.ComponentModel.DataAnnotations;
using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.API;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Services;

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
/// <param name="loginServer">The login server, which knows the online accounts.</param>
/// <param name="loginThrottle">The limit of the failed logins.</param>
/// <param name="coins">The cash shop coin service, which the grants share with the cash shop API.</param>
/// <param name="logger">The logger.</param>
/// <param name="executor">The authorization and audit service.</param>
public sealed class WebsiteOperations(
    IServerProvider servers,
    IPersistenceContextProvider persistence,
    IDataSource<GameConfiguration> configurations,
    AccountWriteOperations accounts,
    WebsiteRankingCache rankingCache,
    MiniGameReadOperations miniGames,
    ILoginServer loginServer,
    WebsiteLoginThrottle loginThrottle,
    CashShopCoinService coins,
    ILogger<WebsiteOperations> logger,
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

    /// <summary>Reads the next starts of the scheduled events of the started game servers.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The events per game server.</returns>
    /// <remarks>
    /// It includes every active plugin with a schedule, like the mini games, the invasions and the happy hour,
    /// so an event which is added or activated later shows up without a change of the website API.
    /// Each game server reports its own events, because an event can be limited to some game servers.
    /// </remarks>
    [AdminOperation("WebManager")]
    [NotExposedToAgents("Public projection for websites; agents use events_schedule.")]
    public async ValueTask<IReadOnlyList<WebsiteServerEvents>> EventsAsync(CancellationToken cancellationToken)
    {
        await executor.RequireOperationAsync(AdminRoles.WebManager, WebsitePermissions.Events, cancellationToken).ConfigureAwait(false);
        var result = new List<WebsiteServerEvents>();
        foreach (var server in servers.Servers.OfType<IGameServer>().Where(s => s.ServerState == ServerState.Started).OrderBy(s => s.Id).Take(AdminReadResult.MaximumRows))
        {
            if (server is not IGameServerAdministration administration)
            {
                continue;
            }

            IReadOnlyList<ScheduledEventInfo> events;
            try
            {
                events = await administration.GetScheduledEventsAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // A game server which doesn't answer, e.g. in the distributed deployment, must not hide the events of the others.
                logger.LogWarning(ex, "Could not read the scheduled events of game server {ServerId} for the website.", server.Id);
                continue;
            }

            result.Add(new WebsiteServerEvents(
                server.Id,
                server.Description,
                events.Select(e => new WebsiteEvent(e.PlugInId, e.Name, e.NextStartUtc)).ToList()));
        }

        return result;
    }

    /// <summary>Reads the public profile of a character, with its equipped items.</summary>
    /// <param name="characterName">The exact name of the character.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The profile of the character.</returns>
    /// <remarks>
    /// Unlike characters_get of the administration, it doesn't contain the account, the inventory, the money or the position,
    /// and it doesn't tell whether the player is online: that's only for the player, in the answer of the login.
    /// The state is the saved one, so the items of an online character may have changed since its last save.
    /// Characters of bots, of templates and of game masters are not found, as they are not ranked either.
    /// Only the character is loaded, not the other characters and the vault of its account.
    /// </remarks>
    [AdminOperation("WebManager")]
    [NotExposedToAgents("Public projection for websites; agents use characters_get.")]
    public async ValueTask<WebsiteCharacter> CharacterAsync(string characterName, CancellationToken cancellationToken)
    {
        await executor.RequireOperationAsync(AdminRoles.WebManager, WebsitePermissions.Character, cancellationToken).ConfigureAwait(false);
        var gameConfiguration = await configurations.GetOwnerAsync(Guid.Empty, cancellationToken).ConfigureAwait(false);
        Character? character;
        using (var context = persistence.CreateNewPlayerContext(gameConfiguration))
        {
            character = await context.GetRankedCharacterAsync(characterName, cancellationToken).ConfigureAwait(false);
        }

        if (character is null)
        {
            throw new AdminResourceNotFoundException("Character not found.", nameof(characterName));
        }

        string? guildName = null;
        using (var guildContext = persistence.CreateNewGuildContext())
        {
            if (await guildContext.GetGuildMembershipAsync(character.Name).ConfigureAwait(false) is { } membership
                && await guildContext.GetByIdAsync<DataModel.Entities.Guild>(membership.GuildId, cancellationToken).ConfigureAwait(false) is { } guild)
            {
                guildName = guild.Name;
            }
        }

        var equippedItems = (character.Inventory?.Items ?? [])
            .Where(item => item.ItemSlot < InventoryConstants.EquippableSlotsCount && item.Definition is not null)
            .OrderBy(item => item.ItemSlot)
            .Select(item => new WebsiteItem(
                item.ItemSlot,
                item.Definition!.Group,
                item.Definition.Number,
                item.Definition.Name.ValueInNeutralLanguage,
                item.Level,
                item.IsExcellent(),
                item.IsAncient(),
                item.SocketCount,
                WebsiteItemDetails.GetPictureName(item),
                item.HasSkill,
                WebsiteItemDetails.GetAncientSetName(item),
                WebsiteItemDetails.GetOptions(item)))
            .ToList();
        return new WebsiteCharacter(
            character.Name,
            character.CharacterClass?.Name.ValueInNeutralLanguage ?? string.Empty,
            (int)GetAttribute(character, Stats.Level),
            (int)GetAttribute(character, Stats.MasterLevel),
            (int)GetAttribute(character, Stats.Resets),
            guildName,
            equippedItems);
    }

    /// <summary>Checks the login name and password of a game account, for a login at the website.</summary>
    /// <param name="loginName">The login name.</param>
    /// <param name="password">The password; it's never logged or audited.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result, with the account and its characters if the login succeeded.</returns>
    /// <remarks>
    /// A wrong login name and a wrong password give the same answer. After too many failed logins, the account is
    /// locked out for the website for a while, see <see cref="WebsiteLoginThrottle"/>; the game itself is not affected.
    /// The website keeps the login in its own session: this server doesn't issue a token, and the website must not store the password.
    /// </remarks>
    [AdminOperation("WebManager")]
    [NotExposedToAgents("Checks passwords of players for websites; agents never handle player credentials.")]
    public async ValueTask<WebsiteLoginResult> LoginAsync(string loginName, string password, CancellationToken cancellationToken)
    {
        var actor = await executor.RequireOperationAsync(AdminRoles.WebManager, WebsitePermissions.Login, cancellationToken).ConfigureAwait(false);
        if (!IsValidLogin(loginName, password))
        {
            return new WebsiteLoginResult(WebsiteLoginStatus.Failed, null);
        }

        var gameConfiguration = await configurations.GetOwnerAsync(Guid.Empty, cancellationToken).ConfigureAwait(false);
        using var context = persistence.CreateNewPlayerContext(gameConfiguration);
        if (!loginThrottle.TryBeginAttempt(loginName, DateTime.UtcNow))
        {
            return new WebsiteLoginResult(WebsiteLoginStatus.LockedOut, null);
        }

        AccountState? state;
        Account? account;
        try
        {
            state = await context.AuthenticateAsync(loginName, password, cancellationToken).ConfigureAwait(false);
            account = state is null ? null : await context.GetAccountByLoginNameAsync(loginName, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            // The password wasn't checked, so the reserved check doesn't count as a failed login.
            loginThrottle.CancelAttempt(loginName);
            throw;
        }

        var succeeded = state is not null && account is { IsTemplate: false };
        if (loginThrottle.CompleteAttempt(loginName, succeeded, DateTime.UtcNow))
        {
            logger.LogWarning("The website login of account {LoginName} is locked out after {Failures} failed logins through API key {ApiKey}.", loginName, WebsiteLoginThrottle.MaximumFailures, actor.Name);
        }

        if (!succeeded)
        {
            return new WebsiteLoginResult(WebsiteLoginStatus.Failed, null);
        }

        var characters = account!.Characters
            .OrderBy(character => character.CharacterSlot)
            .Select(character => new WebsiteCharacterSummary(
                character.Name,
                character.CharacterClass?.Name.ValueInNeutralLanguage ?? string.Empty,
                (int)GetAttribute(character, Stats.Level),
                (int)GetAttribute(character, Stats.MasterLevel),
                (int)GetAttribute(character, Stats.Resets),
                character.Inventory?.Money ?? 0,
                character.CurrentMap?.Number,
                character.CurrentMap?.Name.ValueInNeutralLanguage,
                character.PositionX,
                character.PositionY))
            .ToList();
        var isOnline = (await loginServer.GetSnapshotAsync().ConfigureAwait(false)).Keys.Contains(account.LoginName, StringComparer.OrdinalIgnoreCase);

        // A game server which doesn't answer must not prevent the login.
        var isOfflineLeveling = await this.IsOfflineLevelingAsync(account.LoginName, cancellationToken).ConfigureAwait(false) ?? false;
        return new WebsiteLoginResult(WebsiteLoginStatus.Succeeded, new WebsiteAccount(account.LoginName, state!.Value.ToString(), isOnline, isOfflineLeveling, characters));
    }

    /// <summary>Changes the password of a game account, after checking its current password.</summary>
    /// <param name="loginName">The login name.</param>
    /// <param name="currentPassword">The current password; it's never logged or audited.</param>
    /// <param name="newPassword">The new password, which follows the rules of the registration; it's never logged or audited.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the change.</returns>
    /// <remarks>
    /// A wrong current password counts as a failed login of the <see cref="WebsiteLoginThrottle"/>. The change is audited with the name of the key,
    /// once the current password is checked; an invalid request or a locked out account isn't audited.
    /// The account has to be offline and none of its characters may be offline leveling, because the game server
    /// would overwrite the new password when it saves the account later. If a game server can't tell whether a character
    /// of the account is offline leveling, the password isn't changed either.
    /// </remarks>
    [AdminOperation("WebManager", ReadOnly = false)]
    [NotExposedToAgents("Players change their own password on websites; agents never handle player credentials.")]
    public async ValueTask<WebsitePasswordChangeStatus> ChangePasswordAsync(string loginName, string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        await executor.RequireOperationAsync(AdminRoles.WebManager, WebsitePermissions.ChangePassword, cancellationToken).ConfigureAwait(false);
        if (!IsValidPassword(newPassword))
        {
            throw new AdminOperationException($"The new password must contain {AccountRegistration.MinimumLength}-{AccountRegistration.PasswordMaximumLength} printable ASCII characters.", AdminOperationErrorKind.InvalidInput);
        }

        if (!IsValidLogin(loginName, currentPassword))
        {
            return WebsitePasswordChangeStatus.Failed;
        }

        if (!loginThrottle.TryBeginAttempt(loginName, DateTime.UtcNow))
        {
            return WebsitePasswordChangeStatus.LockedOut;
        }

        var isChecked = false;
        try
        {
            return await executor.ExecuteAsync(
                AdminRoles.WebManager,
                WebsitePermissions.ChangePassword,
                new { loginName },
                async _ =>
                {
                    using var context = persistence.CreateNewPlayerContext(await configurations.GetOwnerAsync(Guid.Empty, cancellationToken).ConfigureAwait(false));
                    var account = await context.AuthenticateAsync(loginName, currentPassword, cancellationToken).ConfigureAwait(false) is null
                        ? null
                        : await context.GetAccountByLoginNameAsync(loginName, cancellationToken).ConfigureAwait(false);
                    isChecked = true;
                    if (account is not { IsTemplate: false })
                    {
                        loginThrottle.CompleteAttempt(loginName, false, DateTime.UtcNow);
                        return WebsitePasswordChangeStatus.Failed;
                    }

                    loginThrottle.CompleteAttempt(loginName, true, DateTime.UtcNow);
                    if ((await loginServer.GetSnapshotAsync().ConfigureAwait(false)).Keys.Contains(account.LoginName, StringComparer.OrdinalIgnoreCase))
                    {
                        return WebsitePasswordChangeStatus.AccountOnline;
                    }

                    var isOfflineLeveling = await this.IsOfflineLevelingAsync(account.LoginName, cancellationToken).ConfigureAwait(false);
                    if (isOfflineLeveling is null)
                    {
                        // Fail closed: if a character is offline leveling there, its game server would overwrite the new password later.
                        return WebsitePasswordChangeStatus.GameServerUnavailable;
                    }

                    if (isOfflineLeveling.Value)
                    {
                        return WebsitePasswordChangeStatus.OfflineLeveling;
                    }

                    account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    if (!await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false))
                    {
                        throw new AdminOperationException("The password change could not be saved.");
                    }

                    return WebsitePasswordChangeStatus.Changed;
                },
                cancellationToken,
                status => status == WebsitePasswordChangeStatus.Changed).ConfigureAwait(false);
        }
        finally
        {
            if (!isChecked)
            {
                // The current password wasn't checked, so the reserved check doesn't count as a failed login.
                loginThrottle.CancelAttempt(loginName);
            }
        }
    }

    /// <summary>Grants cash shop coins to an account, e.g. after a payment on the website.</summary>
    /// <param name="loginName">The exact login name.</param>
    /// <param name="coinType">The coin type.</param>
    /// <param name="amount">The positive amount.</param>
    /// <param name="reason">The optional reason, which is shown in the admin panel.</param>
    /// <param name="reference">The unique reference of the grant, e.g. the identifier of the payment; reuse it when retrying the same grant.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The status of the grant, and the grant if it exists.</returns>
    /// <remarks>
    /// Unlike the cash shop API of a payment provider, a website can only add coins and has to name a reference,
    /// so a repeated request never grants the coins twice. The game server applies the grant when the player opens the cash shop.
    /// </remarks>
    [AdminOperation("WebManager", ReadOnly = false)]
    [NotExposedToAgents("Payment flow of websites; agents use cashshop_grant.")]
    public async ValueTask<(CashShopCoinGrantStatus Status, CashShopCoinGrantInfo? Grant)> GrantCashShopCoinsAsync(string loginName, CashShopCoinType coinType, int amount, string? reason, string reference, CancellationToken cancellationToken)
    {
        if (amount <= 0 || !Enum.IsDefined(coinType) || string.IsNullOrWhiteSpace(reference))
        {
            throw new AdminOperationException($"Supply a known coin type, a positive {nameof(amount)} and a unique {nameof(reference)}.", AdminOperationErrorKind.InvalidInput);
        }

        var result = await executor.ExecuteAsync(
            AdminRoles.WebManager,
            WebsitePermissions.CashShopGrant,
            new { loginName, coinType = coinType.ToString(), amount, reason, reference },
            async actor =>
            {
                var (status, grant) = await coins.GrantAsync(loginName, coinType, amount, reason, reference, actor.Name, cancellationToken).ConfigureAwait(false);
                if (status == CashShopCoinGrantStatus.AlreadyGranted && (grant?.Amount != amount || grant.CoinType != coinType))
                {
                    // A retry has to repeat the same grant; otherwise the reference is reused by mistake.
                    throw new AdminOperationException($"The {nameof(reference)} was already used with a different {nameof(amount)} or coin type. No additional coins were granted.");
                }

                return new { Status = status, Grant = grant };
            },
            cancellationToken,
            value => value.Status is CashShopCoinGrantStatus.Created or CashShopCoinGrantStatus.AlreadyGranted).ConfigureAwait(false);
        return (result.Status, result.Grant);
    }

    private static float GetAttribute(Character character, AttributeDefinition definition)
        => character.Attributes.FirstOrDefault(a => a.Definition.Id == definition.Id)?.Value ?? 0;

    /// <summary>
    /// Determines whether a login name and a password can belong to an account at all, before the password is checked.
    /// </summary>
    /// <param name="loginName">The login name.</param>
    /// <param name="password">The password.</param>
    /// <returns><c>true</c>, if both are present and not longer than the game client allows.</returns>
    private static bool IsValidLogin(string? loginName, string? password)
        => !string.IsNullOrEmpty(loginName) && loginName.Length <= AccountRegistration.LoginNameMaximumLength
           && !string.IsNullOrEmpty(password) && password.Length <= AccountRegistration.PasswordMaximumLength;

    /// <summary>
    /// Determines whether a password follows the rules of the registration, see <see cref="AccountRegistration.Password"/>.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <returns><c>true</c>, if the password is valid.</returns>
    private static bool IsValidPassword(string? password)
        => Validator.TryValidateProperty(password, new ValidationContext(new AccountRegistration()) { MemberName = nameof(AccountRegistration.Password) }, null);

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

    /// <summary>
    /// Determines whether a character of the account is offline leveling on one of the started game servers.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <c>true</c>, if a game server runs an offline leveling character of the account; <c>false</c>, if no game server does;
    /// <c>null</c>, if none of the game servers which answered does, but at least one couldn't be asked.
    /// </returns>
    private async ValueTask<bool?> IsOfflineLevelingAsync(string loginName, CancellationToken cancellationToken)
    {
        var isUnknown = false;
        foreach (var server in servers.Servers.OfType<IGameServer>().Where(s => s.ServerState == ServerState.Started).OfType<IGameServerAdministration>())
        {
            try
            {
                if (await server.IsOfflineLevelingAsync(loginName, cancellationToken).ConfigureAwait(false))
                {
                    return true;
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // The callers decide whether an unknown answer is acceptable, so a failing game server is only logged here.
                logger.LogWarning(ex, "Could not check the offline leveling of account {LoginName} for the website.", loginName);
                isUnknown = true;
            }
        }

        return isUnknown ? null : false;
    }
}

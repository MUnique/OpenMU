// <copyright file="PlayerPortalService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.PlayerPortal.Api;

using System.Threading;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.API;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Implements player-only operations over OpenMU account persistence.</summary>
public sealed class PlayerPortalService(
    IPersistenceContextProvider persistence,
    IDataSource<GameConfiguration> configurations,
    AccountRegistrationService registrations,
    PlayerSessionStore sessions,
    PlayerLoginAttemptLimiter attempts)
{
    private static readonly string DummyPasswordHash = BCrypt.Net.BCrypt.HashPassword("not-a-real-player-password", workFactor: 11);
    private static readonly Guid LevelDefinitionId = Stats.Level.Id;
    private static readonly Guid MasterLevelDefinitionId = Stats.MasterLevel.Id;

    /// <summary>Creates an account through the same validation, storage, and BCrypt path as the existing OpenMU admin operation.</summary>
    /// <param name="registration">The new player account registration.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public ValueTask<AccountRegistrationStatus> RegisterAsync(AccountRegistration registration, CancellationToken cancellationToken = default)
        => registrations.RegisterAsync(registration, cancellationToken);

    /// <summary>Verifies native OpenMU credentials and creates a fixed-expiry owner-bound portal session.</summary>
    /// <param name="request">The submitted game-compatible credentials.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async ValueTask<(PlayerLoginStatus Status, PlayerSessionDescriptor? Session, TimeSpan RetryAfter)> LoginAsync(PlayerLoginRequest request, CancellationToken cancellationToken = default)
    {
        if (!AccountRegistrationService.IsValid(new AccountRegistration { LoginName = request.LoginName, Password = request.Password }))
        {
            _ = BCrypt.Net.BCrypt.Verify(request.Password, DummyPasswordHash);
            return (PlayerLoginStatus.InvalidCredentials, null, TimeSpan.Zero);
        }

        if (!attempts.TryConsume(request.LoginName, out var retryAfter))
        {
            return (PlayerLoginStatus.RateLimited, null, retryAfter);
        }

        var configuration = await configurations.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        using var context = persistence.CreateNewPlayerContext(configuration);
        var state = await context.AuthenticateAsync(request.LoginName, request.Password, cancellationToken).ConfigureAwait(false);
        if (state is null)
        {
            _ = BCrypt.Net.BCrypt.Verify(request.Password, DummyPasswordHash);
            return (PlayerLoginStatus.InvalidCredentials, null, TimeSpan.Zero);
        }

        if (state != AccountState.Normal)
        {
            return (PlayerLoginStatus.InvalidCredentials, null, TimeSpan.Zero);
        }

        // Authentication only needs the account identifier. Loading the complete account graph here
        // would require the portal database role to read character, storage, and grant tables.
        var accountId = await context.GetAccountIdByLoginNameAsync(request.LoginName, cancellationToken).ConfigureAwait(false);
        if (accountId is null)
        {
            return (PlayerLoginStatus.InvalidCredentials, null, TimeSpan.Zero);
        }

        // Recheck credentials and state after resolving the ID, so a concurrent account block or
        // password change cannot create a session from stale authentication state.
        var finalState = await context.AuthenticateAsync(request.LoginName, request.Password, cancellationToken).ConfigureAwait(false);
        if (finalState != AccountState.Normal)
        {
            return (PlayerLoginStatus.InvalidCredentials, null, TimeSpan.Zero);
        }

        try
        {
            return (PlayerLoginStatus.Authenticated, sessions.Create(accountId.Value, TimeSpan.FromMinutes(30)), TimeSpan.Zero);
        }
        catch (InvalidOperationException)
        {
            return (PlayerLoginStatus.SessionCapacityReached, null, TimeSpan.Zero);
        }
    }

    /// <summary>Returns only the session owner's persisted account and character summary.</summary>
    /// <param name="ownerAccountId">The account identifier stored in the authenticated session.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async ValueTask<PlayerAccountSnapshot?> GetOwnAccountAsync(Guid ownerAccountId, CancellationToken cancellationToken = default)
    {
        var configuration = await configurations.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        using var context = persistence.CreateNewPlayerContext(configuration);
        var account = await context.GetAccountOverviewAsync(
            ownerAccountId,
            LevelDefinitionId,
            MasterLevelDefinitionId,
            cancellationToken).ConfigureAwait(false);
        if (account is null || account.State != AccountState.Normal)
        {
            return null;
        }

        var characterClasses = configuration.CharacterClasses?.ToDictionary(characterClass => characterClass.GetId()) ?? new Dictionary<Guid, CharacterClass>();
        var characters = account.Characters
            .Select(character =>
            {
                var characterClass = character.CharacterClassId is { } classId && characterClasses.TryGetValue(classId, out var configuredClass)
                    ? configuredClass
                    : null;
                return new PlayerCharacterSnapshot(
                    character.Name,
                    characterClass?.Name.ToString(),
                    characterClass?.Number,
                    ReadStat(character.Level),
                    ReadStat(character.MasterLevel));
            })
            .ToArray();

        return new PlayerAccountSnapshot(account.LoginName, characters);
    }

    private static int ReadStat(float value) => float.IsFinite(value) ? (int)Math.Clamp(value, 0, int.MaxValue) : 0;
}

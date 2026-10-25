// <copyright file="CashShopCoinService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Shows and grants the cash shop coins of accounts.
/// </summary>
/// <remarks>
/// The coins are never changed directly, because the game server may hold the account in memory.
/// Instead, a <see cref="CashShopCoinGrant"/> is added, which the game server applies.
/// </remarks>
public class CashShopCoinService
{
    /// <summary>
    /// The maximum number of grants which are shown in a summary.
    /// </summary>
    internal const int SummaryGrantCount = 50;

    private readonly IPersistenceContextProvider _persistenceContextProvider;
    private readonly IDataSource<GameConfiguration> _gameConfigurationSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="CashShopCoinService"/> class.
    /// </summary>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="gameConfigurationSource">The game configuration source.</param>
    public CashShopCoinService(IPersistenceContextProvider persistenceContextProvider, IDataSource<GameConfiguration> gameConfigurationSource)
    {
        this._persistenceContextProvider = persistenceContextProvider;
        this._gameConfigurationSource = gameConfigurationSource;
    }

    /// <summary>
    /// Gets the cash shop coins of an account.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The coins of the account; <c>null</c>, if the account doesn't exist.</returns>
    public async ValueTask<CashShopCoinSummary?> GetSummaryAsync(string loginName, CancellationToken cancellationToken = default)
    {
        using var context = await this.CreateContextAsync(cancellationToken).ConfigureAwait(false);
        if (await context.GetAccountByLoginNameAsync(loginName, cancellationToken).ConfigureAwait(false) is not { } account)
        {
            return null;
        }

        var grants = await context.GetLatestCashShopCoinGrantsAsync(account.GetId(), SummaryGrantCount, cancellationToken).ConfigureAwait(false);
        return new CashShopCoinSummary(
            account.LoginName,
            account.WCoinC,
            account.WCoinP,
            account.GoblinPoints,
            grants.Select(CashShopCoinGrantInfo.From).ToList());
    }

    /// <summary>
    /// Grants cash shop coins to an account.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    /// <param name="coinType">The type of the granted coins.</param>
    /// <param name="amount">The amount. A negative amount takes coins, but not below zero.</param>
    /// <param name="reason">The reason.</param>
    /// <param name="reference">The optional reference of the granting system, which can only be granted once.</param>
    /// <param name="grantedBy">The name of the administrator or API client which grants the coins.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The status, and the created grant or the existing grant with the same reference.</returns>
    public async ValueTask<(CashShopCoinGrantStatus Status, CashShopCoinGrantInfo? Grant)> GrantAsync(
        string loginName,
        CashShopCoinType coinType,
        int amount,
        string? reason,
        string? reference,
        string? grantedBy,
        CancellationToken cancellationToken = default)
    {
        // The game server couldn't apply a grant of an unknown coin type, and it would stay pending.
        if (!Enum.IsDefined(coinType))
        {
            throw new ArgumentOutOfRangeException(nameof(coinType), coinType, "The coin type is unknown.");
        }

        reference = string.IsNullOrWhiteSpace(reference) ? null : reference.Trim();
        using var context = await this.CreateContextAsync(cancellationToken).ConfigureAwait(false);
        if (await context.GetAccountByLoginNameAsync(loginName, cancellationToken).ConfigureAwait(false) is not { } account)
        {
            return (CashShopCoinGrantStatus.AccountNotFound, null);
        }

        var accountId = account.GetId();
        if (reference is not null && await this.GetExistingGrantAsync(reference, accountId, cancellationToken).ConfigureAwait(false) is { } existing)
        {
            return existing;
        }

        var grant = context.CreateNew<CashShopCoinGrant>();
        grant.AccountId = accountId;
        grant.CoinType = coinType;
        grant.Amount = amount;
        grant.Reason = reason;
        grant.Reference = reference;
        grant.GrantedBy = grantedBy;
        grant.CreatedAt = DateTime.UtcNow;
        try
        {
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch when (reference is not null)
        {
            // Another request may have granted the same reference between the check above and this
            // save, in which case the unique reference rejects this one.
            if (await this.GetExistingGrantAsync(reference, accountId, cancellationToken).ConfigureAwait(false) is { } concurrent)
            {
                return concurrent;
            }

            throw;
        }

        return (CashShopCoinGrantStatus.Created, CashShopCoinGrantInfo.From(grant));
    }

    private async ValueTask<(CashShopCoinGrantStatus Status, CashShopCoinGrantInfo? Grant)?> GetExistingGrantAsync(string reference, Guid accountId, CancellationToken cancellationToken)
    {
        using var context = await this.CreateContextAsync(cancellationToken).ConfigureAwait(false);
        if (await context.GetCashShopCoinGrantByReferenceAsync(reference, cancellationToken).ConfigureAwait(false) is not { } grant)
        {
            return null;
        }

        return grant.AccountId == accountId
            ? (CashShopCoinGrantStatus.AlreadyGranted, CashShopCoinGrantInfo.From(grant))
            : (CashShopCoinGrantStatus.ReferenceUsedByOtherAccount, null);
    }

    private async ValueTask<IPlayerContext> CreateContextAsync(CancellationToken cancellationToken)
    {
        var gameConfiguration = await this._gameConfigurationSource.GetOwnerAsync(Guid.Empty, cancellationToken).ConfigureAwait(false);
        return this._persistenceContextProvider.CreateNewPlayerContext(gameConfiguration);
    }
}

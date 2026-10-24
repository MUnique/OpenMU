// <copyright file="CashShopOperations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Text.Json.Nodes;
using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>Reads balances and issues idempotent grants through the existing cash shop service.</summary>
/// <param name="coins">The shared coin service.</param>
/// <param name="executor">The authorization and audit executor.</param>
public sealed class CashShopOperations(CashShopCoinService coins, AdminOperationExecutor executor)
{
    /// <summary>Reads saved balances and pending grants.</summary>
    /// <param name="loginName">The exact login name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The saved balance and recent grants.</returns>
    [AdminOperation("CashShop")]
    public async ValueTask<JsonObject> GetBalanceAsync(string loginName, CancellationToken cancellationToken)
    {
        await executor.RequireAsync(AdminRoles.CashShop, cancellationToken).ConfigureAwait(false);
        return AdminReadResult.From(await coins.GetSummaryAsync(loginName, cancellationToken).ConfigureAwait(false)
            ?? throw new AdminOperationException("Account not found.", AdminOperationErrorKind.InvalidInput));
    }

    /// <summary>Queues a coin grant with a unique reference to prevent duplicate credits on retries.</summary>
    /// <param name="loginName">The exact login name.</param>
    /// <param name="coinType">The coin type.</param>
    /// <param name="amount">A nonzero signed amount.</param>
    /// <param name="reason">The audit reason.</param>
    /// <param name="reference">The unique granting reference; reuse it when retrying the same grant.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The grant status and identifier.</returns>
    [AdminOperation("CashShop", ReadOnly = false)]
    public ValueTask<JsonObject> GrantAsync(string loginName, CashShopCoinType coinType, int amount, string reason, string reference, CancellationToken cancellationToken)
        => executor.ExecuteAsync(
            AdminRoles.CashShop,
            "cashshop_grant",
            new { loginName, coinType = coinType.ToString(), amount, reason, reference },
            async actor =>
            {
                AccountWriteOperations.ValidateReason(reason);
                if (amount == 0 || string.IsNullOrWhiteSpace(reference) || reference.Length > 100 || reason.Length > 200)
                {
                    throw new AdminOperationException($"Supply a nonzero {nameof(amount)} and a unique {nameof(reference)} of at most 100 characters and a {nameof(reason)} of at most 200 characters.", AdminOperationErrorKind.InvalidInput);
                }

                var (status, grant) = await coins.GrantAsync(loginName, coinType, amount, reason, reference, actor.Name, cancellationToken).ConfigureAwait(false);
                if (status is CashShopCoinGrantStatus.AccountNotFound or CashShopCoinGrantStatus.ReferenceUsedByOtherAccount)
                {
                    throw new AdminOperationException($"Account not found, or the {nameof(reference)} belongs to another account.");
                }

                if (status == CashShopCoinGrantStatus.AlreadyGranted && (grant?.Amount != amount || grant.CoinType != coinType || grant.Reason != reason))
                {
                    throw new AdminOperationException($"The {nameof(reference)} was already used with different grant arguments. No additional coins were granted.");
                }

                return AdminReadResult.From(new { status = status.ToString(), grant });
            },
            cancellationToken);
}

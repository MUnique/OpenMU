// <copyright file="AccountWriteOperations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.API;

/// <summary>Creates accounts and changes offline accounts without exposing credentials.</summary>
/// <param name="persistence">The persistence provider.</param>
/// <param name="configurations">The shared configuration source.</param>
/// <param name="login">The login server.</param>
/// <param name="executor">The authorization and audit service.</param>
public sealed class AccountWriteOperations(IPersistenceContextProvider persistence, IDataSource<GameConfiguration> configurations, ILoginServer login, AdminOperationExecutor executor)
{
    /// <summary>Creates a normal account using the public registration API's validation.</summary>
    /// <param name="loginName">The new login name.</param>
    /// <param name="password">The initial password; never recorded in the audit trail.</param>
    /// <param name="email">The optional email address.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Whether the account was created or already exists.</returns>
    [AdminOperation(AdminRole.Operator, ReadOnly = false)]
    public ValueTask<JsonObject> CreateAsync(string loginName, string password, string? email, CancellationToken cancellationToken)
        => executor.ExecuteAsync(
            AdminRoles.Operator,
            "accounts_create",
            new { loginName, email },
            async _ =>
            {
                var registration = new AccountRegistration { LoginName = loginName, Password = password, EMail = email };
                var validation = new List<ValidationResult>();
                if (!Validator.TryValidateObject(registration, new ValidationContext(registration), validation, true))
                {
                    throw new AdminOperationException($"Use a printable ASCII login of 3-10 characters, a printable ASCII {nameof(password)} of 3-20 characters and a valid optional email address.", AdminOperationErrorKind.InvalidInput);
                }

                var config = await configurations.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                using var context = persistence.CreateNewPlayerContext(config);
                if (await context.GetAccountByLoginNameAsync(loginName, cancellationToken).ConfigureAwait(false) is not null)
                {
                    return AdminReadResult.From(new { status = "already_exists", loginName });
                }

                var account = context.CreateNew<Account>();
                account.LoginName = loginName;
                account.EMail = email ?? string.Empty;
                account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                account.State = AccountState.Normal;
                account.RegistrationDate = DateTime.UtcNow;
                try
                {
                    await SaveAsync(context, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    using var check = persistence.CreateNewPlayerContext(config);
                    if (await check.GetAccountByLoginNameAsync(loginName, cancellationToken).ConfigureAwait(false) is not null)
                    {
                        return AdminReadResult.From(new { status = "already_exists", loginName });
                    }

                    throw;
                }

                return AdminReadResult.From(new { status = "created", loginName });
            },
            cancellationToken);

    /// <summary>Changes an offline account's state with a mandatory audit reason.</summary>
    /// <param name="loginName">The exact login name.</param>
    /// <param name="state">The requested state.</param>
    /// <param name="reason">The audit reason.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The new state.</returns>
    [AdminOperation(AdminRole.Operator, ReadOnly = false)]
    public ValueTask<JsonObject> SetStateAsync(string loginName, AccountState state, string reason, CancellationToken cancellationToken)
        => executor.ExecuteAsync(
            AdminRoles.Operator,
            "accounts_set_state",
            new { loginName, state = state.ToString(), reason },
            async _ =>
            {
                ValidateReason(reason);
                if (!Enum.IsDefined(state))
                {
                    throw new AdminOperationException("Unknown account state.", AdminOperationErrorKind.InvalidInput);
                }

                using var context = persistence.CreateNewPlayerContext(await configurations.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(false));
                var account = await context.GetAccountByLoginNameAsync(loginName, cancellationToken).ConfigureAwait(false)
                    ?? throw new AdminOperationException("Account not found.", AdminOperationErrorKind.InvalidInput);
                await this.RequireOfflineAsync(account.LoginName).ConfigureAwait(false);
                if (IsPrivileged(account.State) || IsPrivileged(state))
                {
                    await executor.RequireAsync(AdminRoles.Administrator, cancellationToken).ConfigureAwait(false);
                }

                account.State = state;
                await SaveAsync(context, cancellationToken).ConfigureAwait(false);
                return AdminReadResult.From(new { loginName = account.LoginName, state = state.ToString() });
            },
            cancellationToken);

    /// <summary>Resets an offline account's password without returning or auditing the secret.</summary>
    /// <param name="loginName">The exact login name.</param>
    /// <param name="password">The new password.</param>
    /// <param name="reason">The audit reason.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A success status with no credentials.</returns>
    [AdminOperation(AdminRole.Operator, ReadOnly = false)]
    public ValueTask<JsonObject> ResetPasswordAsync(string loginName, string password, string reason, CancellationToken cancellationToken)
        => executor.ExecuteAsync(
            AdminRoles.Operator,
            "accounts_reset_password",
            new { loginName, reason },
            async _ =>
            {
                ValidateReason(reason);
                if (password.Length is < 3 or > 20 || password.Any(c => c is < ' ' or > '~'))
                {
                    throw new AdminOperationException($"The {nameof(password)} must contain 3-20 printable ASCII characters.", AdminOperationErrorKind.InvalidInput);
                }

                using var context = persistence.CreateNewPlayerContext(await configurations.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(false));
                var account = await context.GetAccountByLoginNameAsync(loginName, cancellationToken).ConfigureAwait(false)
                    ?? throw new AdminOperationException("Account not found.", AdminOperationErrorKind.InvalidInput);
                await this.RequireOfflineAsync(account.LoginName).ConfigureAwait(false);
                if (IsPrivileged(account.State))
                {
                    await executor.RequireAsync(AdminRoles.Administrator, cancellationToken).ConfigureAwait(false);
                }

                account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                await SaveAsync(context, cancellationToken).ConfigureAwait(false);
                return AdminReadResult.From(new { loginName = account.LoginName, status = "password_reset" });
            },
            cancellationToken);

    /// <summary>Validates that an operational audit reason is bounded and nonempty.</summary>
    /// <param name="reason">The reason.</param>
    internal static void ValidateReason(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason) || reason.Length > 1000)
        {
            throw new AdminOperationException($"An audit {nameof(reason)} of 1-1000 characters is required.", AdminOperationErrorKind.InvalidInput);
        }
    }

    private static bool IsPrivileged(AccountState state) => state is not (AccountState.Normal or AccountState.Banned or AccountState.TemporarilyBanned);

    private static async ValueTask SaveAsync(IContext context, CancellationToken cancellationToken)
    {
        if (!await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false))
        {
            throw new AdminOperationException("The account change could not be saved.");
        }
    }

    private async ValueTask RequireOfflineAsync(string loginName)
    {
        if ((await login.GetSnapshotAsync().ConfigureAwait(false)).Keys.Contains(loginName, StringComparer.OrdinalIgnoreCase))
        {
            throw new AdminOperationException("The account is online. Use players_disconnect first, or players_ban to ban a live character.");
        }
    }
}

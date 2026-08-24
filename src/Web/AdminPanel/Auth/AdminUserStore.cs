// <copyright file="AdminUserStore.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Security.Cryptography;
using System.Threading;
using Microsoft.AspNetCore.Identity;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// An ASP.NET Core Identity user store which is backed by the <see cref="IAdminUserRepository"/>
/// and by the optionally configured bootstrap user.
/// </summary>
/// <remarks>
/// Only the parts of Identity which are actually needed are implemented, so the whole user
/// management fits into one table instead of the eight tables of the Identity EF store.
/// </remarks>
public class AdminUserStore :
    IUserStore<AdminUser>,
    IUserPasswordStore<AdminUser>,
    IUserSecurityStampStore<AdminUser>,
    IUserTwoFactorStore<AdminUser>,
    IUserAuthenticatorKeyStore<AdminUser>,
    IUserTwoFactorRecoveryCodeStore<AdminUser>,
    IUserLockoutStore<AdminUser>,
    IUserRoleStore<AdminUser>
{
    private const char RecoveryCodeSeparator = ';';

    private readonly IAdminUserRepository _repository;
    private readonly BootstrapAdminUserProvider _bootstrapUserProvider;
    private readonly AdminUserSecretProtector _secretProtector;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminUserStore"/> class.
    /// </summary>
    /// <param name="repository">The repository of the stored users.</param>
    /// <param name="bootstrapUserProvider">The provider of the bootstrap user.</param>
    /// <param name="secretProtector">The protector of the user secrets.</param>
    public AdminUserStore(
        IAdminUserRepository repository,
        BootstrapAdminUserProvider bootstrapUserProvider,
        AdminUserSecretProtector secretProtector)
    {
        this._repository = repository;
        this._bootstrapUserProvider = bootstrapUserProvider;
        this._secretProtector = secretProtector;
    }

    /// <inheritdoc />
    public Task<string> GetUserIdAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.Id.ToString());

    /// <inheritdoc />
    public Task<string?> GetUserNameAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult<string?>(user.LoginName);

    /// <inheritdoc />
    public Task SetUserNameAsync(AdminUser user, string? userName, CancellationToken cancellationToken)
    {
        user.LoginName = userName ?? string.Empty;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<string?> GetNormalizedUserNameAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult<string?>(user.NormalizedLoginName);

    /// <inheritdoc />
    public Task SetNormalizedUserNameAsync(AdminUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedLoginName = normalizedName ?? string.Empty;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<IdentityResult> CreateAsync(AdminUser user, CancellationToken cancellationToken)
    {
        if (user.Id == Guid.Empty)
        {
            user.Id = Guid.NewGuid();
        }

        await this._repository.AddAsync(user, cancellationToken).ConfigureAwait(false);
        return IdentityResult.Success;
    }

    /// <inheritdoc />
    public async Task<IdentityResult> UpdateAsync(AdminUser user, CancellationToken cancellationToken)
    {
        if (BootstrapAdminUserProvider.IsBootstrapUser(user))
        {
            // The bootstrap user only exists in the configuration - its state is kept in memory.
            return IdentityResult.Success;
        }

        await this._repository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        return IdentityResult.Success;
    }

    /// <inheritdoc />
    public async Task<IdentityResult> DeleteAsync(AdminUser user, CancellationToken cancellationToken)
    {
        if (BootstrapAdminUserProvider.IsBootstrapUser(user))
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "BootstrapUserNotDeletable",
                Description = "The bootstrap user is defined by the configuration and can't be deleted here.",
            });
        }

        await this._repository.DeleteAsync(user, cancellationToken).ConfigureAwait(false);
        return IdentityResult.Success;
    }

    /// <inheritdoc />
    public async Task<AdminUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(userId, out var id))
        {
            return null;
        }

        if (this._bootstrapUserProvider.User is { } bootstrapUser && bootstrapUser.Id == id)
        {
            return bootstrapUser;
        }

        return await this._repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<AdminUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        if (this._bootstrapUserProvider.User is { } bootstrapUser
            && string.Equals(bootstrapUser.NormalizedLoginName, normalizedUserName, StringComparison.Ordinal))
        {
            return bootstrapUser;
        }

        return await this._repository.GetByNormalizedLoginNameAsync(normalizedUserName, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task SetPasswordHashAsync(AdminUser user, string? passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash ?? string.Empty;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<string?> GetPasswordHashAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult<string?>(user.PasswordHash);

    /// <inheritdoc />
    public Task<bool> HasPasswordAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));

    /// <inheritdoc />
    public Task SetSecurityStampAsync(AdminUser user, string stamp, CancellationToken cancellationToken)
    {
        user.SecurityStamp = stamp;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<string?> GetSecurityStampAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult<string?>(user.SecurityStamp);

    /// <inheritdoc />
    public Task SetTwoFactorEnabledAsync(AdminUser user, bool enabled, CancellationToken cancellationToken)
    {
        user.IsTwoFactorEnabled = enabled;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<bool> GetTwoFactorEnabledAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.IsTwoFactorEnabled);

    /// <inheritdoc />
    public Task SetAuthenticatorKeyAsync(AdminUser user, string key, CancellationToken cancellationToken)
    {
        user.ProtectedAuthenticatorKey = this._secretProtector.Protect(key);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<string?> GetAuthenticatorKeyAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult(this._secretProtector.Unprotect(user.ProtectedAuthenticatorKey));

    /// <inheritdoc />
    public Task ReplaceCodesAsync(AdminUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
    {
        var hashes = recoveryCodes.Select(HashRecoveryCode);
        user.RecoveryCodeHashes = string.Join(RecoveryCodeSeparator, hashes);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<bool> RedeemCodeAsync(AdminUser user, string code, CancellationToken cancellationToken)
    {
        var hashes = SplitRecoveryCodeHashes(user).ToList();
        var codeHash = HashRecoveryCode(code);
        var expected = Encoding.ASCII.GetBytes(codeHash);
        var index = hashes.FindIndex(hash =>
        {
            var actual = Encoding.ASCII.GetBytes(hash);
            return actual.Length == expected.Length && CryptographicOperations.FixedTimeEquals(actual, expected);
        });
        if (index < 0)
        {
            return Task.FromResult(false);
        }

        hashes.RemoveAt(index);
        user.RecoveryCodeHashes = string.Join(RecoveryCodeSeparator, hashes);
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public Task<int> CountCodesAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult(SplitRecoveryCodeHashes(user).Count());

    /// <inheritdoc />
    public Task<DateTimeOffset?> GetLockoutEndDateAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.LockoutEnd);

    /// <inheritdoc />
    public Task SetLockoutEndDateAsync(AdminUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
    {
        user.LockoutEnd = lockoutEnd;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<int> IncrementAccessFailedCountAsync(AdminUser user, CancellationToken cancellationToken)
    {
        user.AccessFailedCount++;
        return Task.FromResult(user.AccessFailedCount);
    }

    /// <inheritdoc />
    public Task ResetAccessFailedCountAsync(AdminUser user, CancellationToken cancellationToken)
    {
        user.AccessFailedCount = 0;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<int> GetAccessFailedCountAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.AccessFailedCount);

    /// <inheritdoc />
    public Task<bool> GetLockoutEnabledAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult(true);

    /// <inheritdoc />
    public Task SetLockoutEnabledAsync(AdminUser user, bool enabled, CancellationToken cancellationToken)
    {
        // Lockout is always enabled for admin panel users.
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task AddToRoleAsync(AdminUser user, string roleName, CancellationToken cancellationToken)
    {
        var roles = SplitRoles(user).ToList();
        if (!roles.Contains(roleName, StringComparer.OrdinalIgnoreCase))
        {
            roles.Add(roleName);
            user.Roles = string.Join(',', roles);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveFromRoleAsync(AdminUser user, string roleName, CancellationToken cancellationToken)
    {
        var roles = SplitRoles(user).Where(r => !string.Equals(r, roleName, StringComparison.OrdinalIgnoreCase));
        user.Roles = string.Join(',', roles);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IList<string>> GetRolesAsync(AdminUser user, CancellationToken cancellationToken)
        => Task.FromResult<IList<string>>(SplitRoles(user).ToList());

    /// <inheritdoc />
    public Task<bool> IsInRoleAsync(AdminUser user, string roleName, CancellationToken cancellationToken)
        => Task.FromResult(SplitRoles(user).Contains(roleName, StringComparer.OrdinalIgnoreCase));

    /// <inheritdoc />
    public async Task<IList<AdminUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var users = await this._repository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        if (this._bootstrapUserProvider.User is { } bootstrapUser)
        {
            users.Add(bootstrapUser);
        }

        return users.Where(u => SplitRoles(u).Contains(roleName, StringComparer.OrdinalIgnoreCase)).ToList();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Nothing to dispose - the repository is managed by the dependency injection container.
        GC.SuppressFinalize(this);
    }

    private static string HashRecoveryCode(string code)
    {
        var normalized = code.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpperInvariant();
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(normalized)));
    }

    private static IEnumerable<string> SplitRecoveryCodeHashes(AdminUser user)
        => (user.RecoveryCodeHashes ?? string.Empty).Split(RecoveryCodeSeparator, StringSplitOptions.RemoveEmptyEntries);

    private static IEnumerable<string> SplitRoles(AdminUser user)
        => (user.Roles ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

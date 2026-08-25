// <copyright file="BootstrapAdminUserProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// Provides the configured bootstrap user, which exists without a database.
/// </summary>
/// <remarks>
/// Changes to this user (lockout counters, recovery codes, a newly set up authenticator)
/// are only kept in memory and are lost when the process restarts, because there is no
/// storage for them by definition. It's meant to create the first real user and to get
/// back in when that's not possible anymore.
/// </remarks>
public class BootstrapAdminUserProvider
{
    /// <summary>
    /// The identifier of the bootstrap user. It's fixed, so it can be recognized in the store.
    /// </summary>
    public static readonly Guid BootstrapUserId = new("00000000-0000-0000-0000-00000000B007");

    private readonly ILogger<BootstrapAdminUserProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BootstrapAdminUserProvider"/> class.
    /// </summary>
    /// <param name="options">The authentication options.</param>
    /// <param name="passwordHasher">The password hasher.</param>
    /// <param name="secretProtector">The protector for the authenticator key.</param>
    /// <param name="logger">The logger.</param>
    public BootstrapAdminUserProvider(
        IOptions<AdminPanelAuthOptions> options,
        Microsoft.AspNetCore.Identity.IPasswordHasher<AdminUser> passwordHasher,
        AdminUserSecretProtector secretProtector,
        ILogger<BootstrapAdminUserProvider> logger)
    {
        this._logger = logger;
        var configured = options.Value.BootstrapUser;
        if (configured is null
            || string.IsNullOrWhiteSpace(configured.LoginName)
            || string.IsNullOrWhiteSpace(configured.Password))
        {
            return;
        }

        this.User = new AdminUser
        {
            Id = BootstrapUserId,
            LoginName = configured.LoginName,
            NormalizedLoginName = configured.LoginName.ToUpperInvariant(),
            Roles = AdminRoles.Administrator,
            SecurityStamp = Guid.NewGuid().ToString("N"),
            CreatedAt = DateTime.UtcNow,
        };

        this.User.PasswordHash = passwordHasher.HashPassword(this.User, configured.Password);
        if (!string.IsNullOrWhiteSpace(configured.AuthenticatorKey))
        {
            this.User.ProtectedAuthenticatorKey = secretProtector.Protect(configured.AuthenticatorKey.Replace(" ", string.Empty).ToUpperInvariant());
            this.User.IsTwoFactorEnabled = true;
        }

        this._logger.LogInformation(
            "A bootstrap admin panel user '{LoginName}' is configured. Two factor authentication is {State}.",
            this.User.LoginName,
            this.User.IsTwoFactorEnabled ? "enabled" : "disabled");
    }

    /// <summary>
    /// Gets the bootstrap user, if one is configured.
    /// </summary>
    public AdminUser? User { get; }

    /// <summary>
    /// Determines whether the specified user is the bootstrap user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns><c>true</c>, if the specified user is the bootstrap user; otherwise, <c>false</c>.</returns>
    public static bool IsBootstrapUser(AdminUser user) => user.Id == BootstrapUserId;
}

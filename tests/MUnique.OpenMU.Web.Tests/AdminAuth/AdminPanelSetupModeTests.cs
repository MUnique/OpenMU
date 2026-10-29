// <copyright file="AdminPanelSetupModeTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// Tests the <see cref="AdminPanelSetupMode"/>.
/// </summary>
[TestFixture]
public sealed class AdminPanelSetupModeTests
{
    private const string TestPassword = "a long enough test password";

    /// <summary>
    /// Tests that the admin panel is in its setup mode without any user.
    /// </summary>
    [Test]
    public async Task ActiveWithoutAUserAsync()
    {
        var setupMode = CreateSetupMode(new InMemoryAdminUserRepository());

        Assert.That(await setupMode.IsActiveAsync().ConfigureAwait(false), Is.True);
    }

    /// <summary>
    /// Tests that a stored user ends the setup mode.
    /// </summary>
    [Test]
    public async Task NotActiveWithAStoredUserAsync()
    {
        var users = new InMemoryAdminUserRepository();
        await users.AddAsync(CreateUser("admin")).ConfigureAwait(false);
        var setupMode = CreateSetupMode(users);

        Assert.That(await setupMode.IsActiveAsync().ConfigureAwait(false), Is.False);
    }

    /// <summary>
    /// Tests that a bootstrap user ends the setup mode, although it isn't stored.
    /// </summary>
    [Test]
    public async Task NotActiveWithABootstrapUserAsync()
    {
        var setupMode = CreateSetupMode(
            new InMemoryAdminUserRepository(),
            new BootstrapAdminUserOptions { LoginName = "bootstrap", Password = TestPassword });

        Assert.That(await setupMode.IsActiveAsync().ConfigureAwait(false), Is.False);
    }

    /// <summary>
    /// Creates the setup mode; the environment variables of the bootstrap user are not read here.
    /// </summary>
    /// <param name="users">The stored users.</param>
    /// <param name="bootstrapUser">The bootstrap user, if there is one.</param>
    /// <returns>The setup mode.</returns>
    internal static AdminPanelSetupMode CreateSetupMode(IAdminUserRepository users, BootstrapAdminUserOptions? bootstrapUser = null)
    {
        var bootstrapUserProvider = new BootstrapAdminUserProvider(
            Options.Create(new AdminPanelAuthOptions { BootstrapUser = bootstrapUser }),
            new BCryptPasswordHasher(),
            new AdminUserSecretProtector(new EphemeralDataProtectionProvider(), NullLogger<AdminUserSecretProtector>.Instance),
            NullLogger<BootstrapAdminUserProvider>.Instance);
        return new AdminPanelSetupMode(users, bootstrapUserProvider);
    }

    /// <summary>
    /// Creates a stored user.
    /// </summary>
    /// <param name="loginName">The login name.</param>
    /// <returns>The user.</returns>
    internal static AdminUser CreateUser(string loginName)
    {
        return new AdminUser
        {
            Id = Guid.NewGuid(),
            LoginName = loginName,
            NormalizedLoginName = loginName.ToUpperInvariant(),
            Roles = AdminRoles.Administrator,
        };
    }
}

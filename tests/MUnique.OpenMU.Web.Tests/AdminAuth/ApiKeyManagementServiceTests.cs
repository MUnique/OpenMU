// <copyright file="ApiKeyManagementServiceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

/// <summary>
/// Tests when the <see cref="ApiKeyManagementService"/> creates an API key.
/// </summary>
[TestFixture]
public sealed class ApiKeyManagementServiceTests
{
    /// <summary>
    /// Tests that no key is created while the admin panel has no user: in that setup mode, anybody
    /// who reaches the admin panel could create one, and it would keep working after the first
    /// user exists.
    /// </summary>
    [Test]
    public async Task NoKeyWithoutAUserAsync()
    {
        var keys = new InMemoryApiKeyRepository();
        var modals = new TestModalService();
        var toasts = new Mock<IToastService>();
        var service = new ApiKeyManagementService(keys, modals, toasts.Object, CreateUserAvailability(new InMemoryAdminUserRepository()));

        var createdKey = await service.CreateNewInModalDialogAsync().ConfigureAwait(false);

        Assert.That(createdKey, Is.Null);
        Assert.That(modals.ShownDialogs, Is.Empty);
        Assert.That(await keys.GetAllAsync().ConfigureAwait(false), Is.Empty);
        toasts.Verify(toast => toast.ShowError(It.IsAny<string>(), It.IsAny<string?>()), Times.Once);
    }

    /// <summary>
    /// Tests that a key is created once the admin panel has a user.
    /// </summary>
    [Test]
    public async Task KeyWithAUserAsync()
    {
        var users = new InMemoryAdminUserRepository();
        await users.AddAsync(new AdminUser { Id = Guid.NewGuid(), LoginName = "admin", NormalizedLoginName = "ADMIN", Roles = AdminRoles.Administrator }).ConfigureAwait(false);
        var keys = new InMemoryApiKeyRepository();
        var modals = new TestModalService();
        var service = new ApiKeyManagementService(keys, modals, Mock.Of<IToastService>(), CreateUserAvailability(users));

        var createdKey = await service.CreateNewInModalDialogAsync().ConfigureAwait(false);

        Assert.That(createdKey, Is.Not.Null);
        Assert.That(modals.ShownDialogs, Has.Count.EqualTo(1));
        Assert.That(await keys.GetAllAsync().ConfigureAwait(false), Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Creates the service which knows whether the admin panel has a user, without a bootstrap user:
    /// the environment variables of the bootstrap user are not read here.
    /// </summary>
    private static AdminUserAvailabilityService CreateUserAvailability(IAdminUserRepository users)
    {
        var bootstrapUser = new BootstrapAdminUserProvider(
            Options.Create(new AdminPanelAuthOptions()),
            new BCryptPasswordHasher(),
            new AdminUserSecretProtector(new EphemeralDataProtectionProvider(), NullLogger<AdminUserSecretProtector>.Instance),
            NullLogger<BootstrapAdminUserProvider>.Instance);
        return new AdminUserAvailabilityService(users, bootstrapUser);
    }
}

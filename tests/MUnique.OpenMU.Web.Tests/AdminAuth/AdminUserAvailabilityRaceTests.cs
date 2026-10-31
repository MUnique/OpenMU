// <copyright file="AdminUserAvailabilityRaceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using System.Threading;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// Tests that the <see cref="AdminUserAvailabilityService"/> doesn't answer "no user exists" while users
/// exist, when a check of the storage is still running.
/// </summary>
/// <remarks>
/// "No user exists" is the initial setup mode, in which the panel is open without a login. Each of these
/// checks would otherwise let anonymous requests in with full access, while users exist.
/// </remarks>
[TestFixture]
public class AdminUserAvailabilityRaceTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Tests that a check during the first probe after the start, e.g. while the migrations run,
    /// doesn't grant the initial setup mode when users exist.
    /// </summary>
    [Test]
    public async Task CheckDuringFirstProbeDoesNotGrantSetupModeAsync()
    {
        var repository = new GatedAdminUserRepository();
        await repository.AddAsync(CreateUser("admin")).ConfigureAwait(false);
        var service = CreateService(repository);

        repository.StorageGate.Close();
        var probingCall = Task.Run(async () => await service.AnyUserExistsAsync().ConfigureAwait(false));
        await repository.StorageGate.Entered.WaitAsync(Timeout).ConfigureAwait(false);

        var concurrentCall = service.AnyUserExistsAsync();
        Assert.That(concurrentCall.IsCompleted, Is.True, "A check must not wait for a probe which is already running.");
        Assert.That(await concurrentCall.ConfigureAwait(false), Is.True, "The setup mode was granted while a user exists.");

        repository.StorageGate.Open();
        Assert.That(await probingCall.WaitAsync(Timeout).ConfigureAwait(false), Is.True);
    }

    /// <summary>
    /// Tests that a check while the user count is asked again after the first user has been created
    /// doesn't grant the initial setup mode anymore.
    /// </summary>
    [Test]
    public async Task CheckAfterUserCreationDoesNotGrantSetupModeAsync()
    {
        var repository = new GatedAdminUserRepository();
        var service = CreateService(repository);
        Assert.That(await service.AnyUserExistsAsync().ConfigureAwait(false), Is.False, "Without a user, the panel is in its setup mode.");

        // What AdminUserManagementService does after it created a user.
        await repository.AddAsync(CreateUser("admin")).ConfigureAwait(false);
        service.Invalidate();

        await AssertConcurrentCheckRequiresLoginAsync(service, repository).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that a check while the user count is asked again after a user has been deleted
    /// doesn't grant the initial setup mode, when other users remain.
    /// </summary>
    [Test]
    public async Task CheckAfterUserDeletionDoesNotGrantSetupModeAsync()
    {
        var repository = new GatedAdminUserRepository();
        var deletedUser = CreateUser("deleted");
        await repository.AddAsync(CreateUser("admin")).ConfigureAwait(false);
        await repository.AddAsync(deletedUser).ConfigureAwait(false);
        var service = CreateService(repository);
        Assert.That(await service.AnyUserExistsAsync().ConfigureAwait(false), Is.True);

        // What AdminUserManagementService does after it deleted a user.
        await repository.DeleteAsync(deletedUser).ConfigureAwait(false);
        service.Invalidate();

        await AssertConcurrentCheckRequiresLoginAsync(service, repository).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that a caller which gives up during the first probe doesn't make it look like
    /// the storage is unavailable, which would grant the initial setup mode while users exist.
    /// </summary>
    /// <remarks>
    /// The reverse proxy authentication passes the token of its request, so a client which closes its
    /// connection cancels it. The repository treats a probe which didn't finish as "not available".
    /// </remarks>
    [Test]
    public async Task CancelledFirstProbeDoesNotGrantSetupModeAsync()
    {
        var repository = new GatedAdminUserRepository();
        await repository.AddAsync(CreateUser("admin")).ConfigureAwait(false);
        var service = CreateService(repository);

        repository.StorageGate.Close();
        using var cancellation = new CancellationTokenSource();
        var cancelledCall = service.AnyUserExistsAsync(cancellation.Token).AsTask();
        await repository.StorageGate.Entered.WaitAsync(Timeout).ConfigureAwait(false);
        await cancellation.CancelAsync().ConfigureAwait(false);
        try
        {
            await cancelledCall.WaitAsync(Timeout).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Giving up is fine for the cancelled caller itself.
        }

        Assert.That(await service.AnyUserExistsAsync().ConfigureAwait(false), Is.True, "The setup mode was granted while a user exists.");

        repository.StorageGate.Open();
        Assert.That(await service.AnyUserExistsAsync().ConfigureAwait(false), Is.True);
    }

    /// <summary>
    /// Tests that a created user ends the initial setup mode right away, without asking the storage again.
    /// </summary>
    [Test]
    public async Task CreatedUserEndsSetupModeWithoutCheckAsync()
    {
        var repository = new GatedAdminUserRepository();
        var service = CreateService(repository);
        Assert.That(await service.AnyUserExistsAsync().ConfigureAwait(false), Is.False, "Without a user, the panel is in its setup mode.");

        await repository.AddAsync(CreateUser("admin")).ConfigureAwait(false);
        service.NotifyUserCreated();
        repository.StorageGate.Close();
        repository.CountGate.Close();

        var check = service.AnyUserExistsAsync();
        Assert.That(check.IsCompleted, Is.True, "The answer is known after a user has been created.");
        Assert.That(await check.ConfigureAwait(false), Is.True);
        Assert.That(repository.StorageGate.Entered.IsCompleted || repository.CountGate.Entered.IsCompleted, Is.False);
    }

    /// <summary>
    /// Tests that a check which counted the users before the first user was created doesn't
    /// bring the initial setup mode back when it finishes afterwards.
    /// </summary>
    [Test]
    public async Task OutdatedCheckDoesNotGrantSetupModeAfterUserCreationAsync()
    {
        var repository = new GatedAdminUserRepository { CountsBeforeGate = true };
        var service = CreateService(repository);

        repository.CountGate.Close();
        var outdatedCall = Task.Run(async () => await service.AnyUserExistsAsync().ConfigureAwait(false));
        await repository.CountGate.Entered.WaitAsync(Timeout).ConfigureAwait(false);

        await repository.AddAsync(CreateUser("admin")).ConfigureAwait(false);
        service.NotifyUserCreated();
        repository.CountGate.Open();

        Assert.That(await outdatedCall.WaitAsync(Timeout).ConfigureAwait(false), Is.True);
        Assert.That(await service.AnyUserExistsAsync().ConfigureAwait(false), Is.True, "The outdated count overwrote the created user.");
    }

    private static async Task AssertConcurrentCheckRequiresLoginAsync(AdminUserAvailabilityService service, GatedAdminUserRepository repository)
    {
        repository.CountGate.Close();
        var probingCall = Task.Run(async () => await service.AnyUserExistsAsync().ConfigureAwait(false));
        await repository.CountGate.Entered.WaitAsync(Timeout).ConfigureAwait(false);

        var concurrentCall = service.AnyUserExistsAsync();
        Assert.That(concurrentCall.IsCompleted, Is.True, "A check must not wait for a probe which is already running.");
        Assert.That(await concurrentCall.ConfigureAwait(false), Is.True, "The setup mode was granted while a user exists.");

        repository.CountGate.Open();
        Assert.That(await probingCall.WaitAsync(Timeout).ConfigureAwait(false), Is.True);
        Assert.That(await service.AnyUserExistsAsync().ConfigureAwait(false), Is.True);
    }

    private static AdminUser CreateUser(string loginName) => new()
    {
        Id = Guid.NewGuid(),
        LoginName = loginName,
        NormalizedLoginName = loginName.ToUpperInvariant(),
    };

    private static AdminUserAvailabilityService CreateService(IAdminUserRepository repository)
        => new(
            repository,
            new BootstrapAdminUserProvider(
                Options.Create(new AdminPanelAuthOptions()),
                new BCryptPasswordHasher(),
                new AdminUserSecretProtector(new EphemeralDataProtectionProvider(), NullLogger<AdminUserSecretProtector>.Instance),
                NullLogger<BootstrapAdminUserProvider>.Instance));
}

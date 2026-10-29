// <copyright file="AdminPanelSetupModeTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// Tests the <see cref="AdminPanelSetupMode"/>, and that the first user of the admin panel
/// disables the API keys which were created while it had no user.
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
        var setupMode = CreateSetupMode(new InMemoryAdminUserRepository(), new InMemoryApiKeyRepository());

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
        var setupMode = CreateSetupMode(users, new InMemoryApiKeyRepository());

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
            new InMemoryApiKeyRepository(),
            new BootstrapAdminUserOptions { LoginName = "bootstrap", Password = TestPassword });

        Assert.That(await setupMode.IsActiveAsync().ConfigureAwait(false), Is.False);
    }

    /// <summary>
    /// Tests that the oldest stored user stands for the first user.
    /// </summary>
    [Test]
    public async Task FirstUserCreatedAtIsTheOldestUserAsync()
    {
        var users = new InMemoryAdminUserRepository();
        var oldest = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var younger = CreateUser("younger");
        younger.CreatedAt = oldest.AddDays(3);
        var older = CreateUser("older");
        older.CreatedAt = oldest;
        await users.AddAsync(younger).ConfigureAwait(false);
        await users.AddAsync(older).ConfigureAwait(false);
        var setupMode = CreateSetupMode(users, new InMemoryApiKeyRepository());

        Assert.That(await setupMode.GetFirstUserCreatedAtAsync().ConfigureAwait(false), Is.EqualTo(oldest));
    }

    /// <summary>
    /// Tests that there is no first user without a stored user.
    /// </summary>
    [Test]
    public async Task NoFirstUserCreatedAtWithoutAUserAsync()
    {
        var setupMode = CreateSetupMode(new InMemoryAdminUserRepository(), new InMemoryApiKeyRepository());

        Assert.That(await setupMode.GetFirstUserCreatedAtAsync().ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// Tests that disabling the keys of the setup mode disables every enabled key and counts only those.
    /// </summary>
    [Test]
    public async Task DisableApiKeysDisablesTheEnabledKeysAsync()
    {
        var keys = new InMemoryApiKeyRepository();
        await keys.AddAsync(CreateKey("launcher", isDisabled: false)).ConfigureAwait(false);
        await keys.AddAsync(CreateKey("website", isDisabled: false)).ConfigureAwait(false);
        await keys.AddAsync(CreateKey("old", isDisabled: true)).ConfigureAwait(false);
        var setupMode = CreateSetupMode(new InMemoryAdminUserRepository(), keys);

        var disabled = await setupMode.DisableApiKeysAsync().ConfigureAwait(false);

        Assert.That(disabled, Is.EqualTo(2));
        Assert.That(await keys.GetAllAsync().ConfigureAwait(false), Has.All.Matches<ApiKey>(apiKey => apiKey.IsDisabled));
    }

    /// <summary>
    /// Tests that the first user, which ends the setup mode, disables the keys which exist then,
    /// and tells so.
    /// </summary>
    [Test]
    public async Task FirstUserDisablesTheKeysOfTheSetupModeAsync()
    {
        var users = new InMemoryAdminUserRepository();
        var keys = new InMemoryApiKeyRepository();
        await keys.AddAsync(CreateKey("launcher", isDisabled: false)).ConfigureAwait(false);
        var toasts = new Mock<IToastService>();
        using var services = BuildServices(users, keys, toasts.Object, "first");

        using var scope = services.CreateScope();

        var created = await scope.ServiceProvider.GetRequiredService<AdminUserManagementService>().CreateNewInModalDialogAsync().ConfigureAwait(false);

        Assert.That(created, Is.True);
        Assert.That(await users.GetCountAsync().ConfigureAwait(false), Is.EqualTo(1));
        Assert.That((await keys.GetAllAsync().ConfigureAwait(false)).Single().IsDisabled, Is.True);
        toasts.Verify(toast => toast.ShowWarning(It.IsAny<string>(), It.IsAny<string?>()), Times.Once);
    }

    /// <summary>
    /// Tests that a user who isn't the first one leaves the keys alone.
    /// </summary>
    [Test]
    public async Task LaterUserLeavesTheKeysEnabledAsync()
    {
        var users = new InMemoryAdminUserRepository();
        await users.AddAsync(CreateUser("admin")).ConfigureAwait(false);
        var keys = new InMemoryApiKeyRepository();
        await keys.AddAsync(CreateKey("launcher", isDisabled: false)).ConfigureAwait(false);
        var toasts = new Mock<IToastService>();
        using var services = BuildServices(users, keys, toasts.Object, "second");

        using var scope = services.CreateScope();

        var created = await scope.ServiceProvider.GetRequiredService<AdminUserManagementService>().CreateNewInModalDialogAsync().ConfigureAwait(false);

        Assert.That(created, Is.True);
        Assert.That(await users.GetCountAsync().ConfigureAwait(false), Is.EqualTo(2));
        Assert.That((await keys.GetAllAsync().ConfigureAwait(false)).Single().IsDisabled, Is.False);
        toasts.Verify(toast => toast.ShowWarning(It.IsAny<string>(), It.IsAny<string?>()), Times.Never);
    }

    /// <summary>
    /// Creates the setup mode; the environment variables of the bootstrap user are not read here.
    /// </summary>
    /// <param name="users">The stored users.</param>
    /// <param name="keys">The stored API keys.</param>
    /// <param name="bootstrapUser">The bootstrap user, if there is one.</param>
    /// <returns>The setup mode.</returns>
    internal static AdminPanelSetupMode CreateSetupMode(IAdminUserRepository users, IApiKeyRepository keys, BootstrapAdminUserOptions? bootstrapUser = null)
    {
        var bootstrapUserProvider = new BootstrapAdminUserProvider(
            Options.Create(new AdminPanelAuthOptions { BootstrapUser = bootstrapUser }),
            new BCryptPasswordHasher(),
            new AdminUserSecretProtector(new EphemeralDataProtectionProvider(), NullLogger<AdminUserSecretProtector>.Instance),
            NullLogger<BootstrapAdminUserProvider>.Instance);
        return new AdminPanelSetupMode(users, keys, bootstrapUserProvider, NullLogger<AdminPanelSetupMode>.Instance);
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

    private static ApiKey CreateKey(string name, bool isDisabled)
    {
        return new ApiKey
        {
            Id = Guid.NewGuid(),
            Name = name,
            KeyHash = name,
            KeyPrefix = name,
            IsDisabled = isDisabled,
        };
    }

    private static ServiceProvider BuildServices(InMemoryAdminUserRepository users, IApiKeyRepository keys, IToastService toasts, string newLoginName)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
        services.AddDataProtection();
        services.AddSingleton<IAdminUserRepository>(users);
        services.AddSingleton(keys);
        services.AddSingleton(Options.Create(new AdminPanelAuthOptions()));
        services.AddSingleton<AdminUserSecretProtector>();
        services.AddSingleton<IPasswordHasher<AdminUser>, BCryptPasswordHasher>();
        services.AddSingleton<BootstrapAdminUserProvider>();
        services.AddSingleton<SignInTicketService>();
        services.AddSingleton<AdminUserAvailabilityService>();
        services.AddSingleton<AdminPanelSetupMode>();
        services.AddSingleton<IModalService>(new UserDialog(newLoginName));
        services.AddSingleton(toasts);
        services.AddScoped<AdminUserStore>();
        services.AddScoped<IUserStore<AdminUser>>(provider => provider.GetRequiredService<AdminUserStore>());
        services.AddScoped<AuthenticatorSetupService>();
        services.AddScoped<AdminUserManagementService>();
        services.AddIdentityCore<AdminUser>(options =>
            {
                options.Password.RequiredLength = 12;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddDefaultTokenProviders();
        return services.BuildServiceProvider();
    }

    /// <summary>
    /// The dialog which creates a user: it fills in the login name and the password and confirms.
    /// </summary>
    private sealed class UserDialog : IModalService
    {
        private readonly string _loginName;

        public UserDialog(string loginName)
        {
            this._loginName = loginName;
        }

        public IModalReference Show<TComponent>(string title, ModalParameters? parameters = null, ModalOptions? options = null)
            where TComponent : class, IComponent
        {
            return this.Show(typeof(TComponent), title, parameters, options);
        }

        public IModalReference Show(Type componentType, string title, ModalParameters? parameters = null, ModalOptions? options = null)
        {
            var input = parameters?.FirstOrDefault(parameter => parameter.Key == "Item").Value
                        ?? throw new InvalidOperationException("The dialog has no item to fill in.");
            input.GetType().GetProperty("LoginName")!.SetValue(input, this._loginName);
            input.GetType().GetProperty("Password")!.SetValue(input, TestPassword);
            return new Reference(ModalResult.Ok(true));
        }

        private sealed class Reference : IModalReference
        {
            public Reference(ModalResult result)
            {
                this.Result = Task.FromResult(result);
            }

            public Task<ModalResult> Result { get; }
        }
    }
}

// <copyright file="AdminAuthenticationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// Tests for the authentication of the admin panel.
/// </summary>
[TestFixture]
public class AdminAuthenticationTests
{
    private const string TestPassword = "a-very-long-test-password";

    private ServiceProvider _serviceProvider = null!;
    private InMemoryAdminUserRepository _repository = null!;

    /// <summary>
    /// Sets a fresh service provider up for each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._repository = new InMemoryAdminUserRepository();
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
        services.AddDataProtection();
        services.AddSingleton<IAdminUserRepository>(this._repository);
        services.AddSingleton(Options.Create(new AdminPanelAuthOptions()));
        services.AddSingleton<AdminUserSecretProtector>();
        services.AddSingleton<IPasswordHasher<AdminUser>, BCryptPasswordHasher>();
        services.AddSingleton<BootstrapAdminUserProvider>();
        services.AddSingleton<SignInTicketService>();
        services.AddScoped<IUserStore<AdminUser>, AdminUserStore>();
        services.AddScoped<AdminLoginService>();
        services.AddScoped<AuthenticatorSetupService>();
        services.AddIdentityCore<AdminUser>(options =>
            {
                options.Password.RequiredLength = 12;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddDefaultTokenProviders();

        this._serviceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Disposes the service provider.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        this._serviceProvider.Dispose();
    }

    /// <summary>
    /// Tests that a user without a second factor can log in with its password.
    /// </summary>
    [Test]
    public async Task LoginWithoutSecondFactorSucceedsAsync()
    {
        await this.CreateUserAsync("tester").ConfigureAwait(false);

        var result = await this.GetLoginService().CheckPasswordAsync("tester", TestPassword, false).ConfigureAwait(false);

        Assert.That(result.Status, Is.EqualTo(AdminLoginStatus.Succeeded));
        Assert.That(result.Ticket, Is.Not.Null);
        Assert.That(result.Claims, Is.Not.Null);
    }

    /// <summary>
    /// Tests that a wrong password is rejected.
    /// </summary>
    [Test]
    public async Task LoginWithWrongPasswordFailsAsync()
    {
        await this.CreateUserAsync("tester").ConfigureAwait(false);

        var result = await this.GetLoginService().CheckPasswordAsync("tester", "wrong-password-here", false).ConfigureAwait(false);

        Assert.That(result.Status, Is.EqualTo(AdminLoginStatus.Failed));
        Assert.That(result.Ticket, Is.Null);
    }

    /// <summary>
    /// Tests that an unknown login name is rejected.
    /// </summary>
    [Test]
    public async Task LoginWithUnknownUserFailsAsync()
    {
        var result = await this.GetLoginService().CheckPasswordAsync("nobody", TestPassword, false).ConfigureAwait(false);

        Assert.That(result.Status, Is.EqualTo(AdminLoginStatus.Failed));
    }

    /// <summary>
    /// Tests that a disabled user can't log in, even with the correct password.
    /// </summary>
    [Test]
    public async Task LoginOfDisabledUserFailsAsync()
    {
        var user = await this.CreateUserAsync("tester").ConfigureAwait(false);
        user.IsDisabled = true;
        await this._repository.UpdateAsync(user).ConfigureAwait(false);

        var result = await this.GetLoginService().CheckPasswordAsync("tester", TestPassword, false).ConfigureAwait(false);

        Assert.That(result.Status, Is.EqualTo(AdminLoginStatus.Failed));
    }

    /// <summary>
    /// Tests that the user gets locked out after too many failed attempts.
    /// </summary>
    [Test]
    public async Task RepeatedFailedAttemptsCauseLockoutAsync()
    {
        await this.CreateUserAsync("tester").ConfigureAwait(false);
        var loginService = this.GetLoginService();

        for (var i = 0; i < 3; i++)
        {
            await loginService.CheckPasswordAsync("tester", "wrong-password-here", false).ConfigureAwait(false);
        }

        var result = await loginService.CheckPasswordAsync("tester", TestPassword, false).ConfigureAwait(false);

        Assert.That(result.Status, Is.EqualTo(AdminLoginStatus.LockedOut));
    }

    /// <summary>
    /// Tests that the second factor is asked for and that a valid authenticator code completes the login.
    /// </summary>
    [Test]
    public async Task LoginWithSecondFactorSucceedsAsync()
    {
        var (_, key) = await this.CreateUserWithAuthenticatorAsync("tester").ConfigureAwait(false);
        var loginService = this.GetLoginService();

        var passwordResult = await loginService.CheckPasswordAsync("tester", TestPassword, false).ConfigureAwait(false);
        Assert.That(passwordResult.Status, Is.EqualTo(AdminLoginStatus.TwoFactorRequired));
        Assert.That(passwordResult.Ticket, Is.Null, "No ticket may be issued before the second factor was checked.");

        var codeResult = await loginService.CheckTwoFactorAsync(TestTotpGenerator.Generate(key), false).ConfigureAwait(false);

        Assert.That(codeResult.Status, Is.EqualTo(AdminLoginStatus.Succeeded));
        Assert.That(codeResult.Ticket, Is.Not.Null);
    }

    /// <summary>
    /// Tests that a wrong authenticator code is rejected.
    /// </summary>
    [Test]
    public async Task LoginWithWrongSecondFactorFailsAsync()
    {
        await this.CreateUserWithAuthenticatorAsync("tester").ConfigureAwait(false);
        var loginService = this.GetLoginService();
        await loginService.CheckPasswordAsync("tester", TestPassword, false).ConfigureAwait(false);

        var codeResult = await loginService.CheckTwoFactorAsync("000000", false).ConfigureAwait(false);

        Assert.That(codeResult.Status, Is.EqualTo(AdminLoginStatus.Failed));
    }

    /// <summary>
    /// Tests that the same authenticator code can't be used a second time within its time step.
    /// </summary>
    [Test]
    public async Task ReplayedAuthenticatorCodeIsRejectedAsync()
    {
        var (_, key) = await this.CreateUserWithAuthenticatorAsync("tester").ConfigureAwait(false);
        var code = TestTotpGenerator.Generate(key);

        var firstLogin = this.GetLoginService();
        await firstLogin.CheckPasswordAsync("tester", TestPassword, false).ConfigureAwait(false);
        var firstResult = await firstLogin.CheckTwoFactorAsync(code, false).ConfigureAwait(false);
        Assert.That(firstResult.Status, Is.EqualTo(AdminLoginStatus.Succeeded));

        var secondLogin = this.GetLoginService();
        await secondLogin.CheckPasswordAsync("tester", TestPassword, false).ConfigureAwait(false);
        var secondResult = await secondLogin.CheckTwoFactorAsync(code, false).ConfigureAwait(false);

        Assert.That(secondResult.Status, Is.EqualTo(AdminLoginStatus.Failed));
    }

    /// <summary>
    /// Tests that a recovery code works exactly once.
    /// </summary>
    [Test]
    public async Task RecoveryCodeCanBeUsedOnceAsync()
    {
        var (user, key) = await this.CreateUserWithAuthenticatorAsync("tester").ConfigureAwait(false);
        using var scope = this._serviceProvider.CreateScope();
        var setupService = scope.ServiceProvider.GetRequiredService<AuthenticatorSetupService>();
        var recoveryCodes = await setupService.GenerateRecoveryCodesAsync(user).ConfigureAwait(false);
        Assert.That(recoveryCodes, Is.Not.Empty);
        var recoveryCode = recoveryCodes[0];

        var firstLogin = this.GetLoginService();
        await firstLogin.CheckPasswordAsync("tester", TestPassword, false).ConfigureAwait(false);
        var firstResult = await firstLogin.CheckTwoFactorAsync(recoveryCode, true).ConfigureAwait(false);
        Assert.That(firstResult.Status, Is.EqualTo(AdminLoginStatus.Succeeded));

        var secondLogin = this.GetLoginService();
        await secondLogin.CheckPasswordAsync("tester", TestPassword, false).ConfigureAwait(false);
        var secondResult = await secondLogin.CheckTwoFactorAsync(recoveryCode, true).ConfigureAwait(false);
        Assert.That(secondResult.Status, Is.EqualTo(AdminLoginStatus.Failed));

        // Suppress the unused variable warning for the key, which is only needed for the setup.
        Assert.That(key, Is.Not.Empty);
    }

    /// <summary>
    /// Tests that the second factor is only enabled after the user proved that its app produces valid codes.
    /// </summary>
    [Test]
    public async Task SecondFactorIsOnlyEnabledAfterConfirmationAsync()
    {
        var user = await this.CreateUserAsync("tester").ConfigureAwait(false);
        using var scope = this._serviceProvider.CreateScope();
        var setupService = scope.ServiceProvider.GetRequiredService<AuthenticatorSetupService>();

        var setup = await setupService.BeginSetupAsync(user).ConfigureAwait(false);
        Assert.That(user.IsTwoFactorEnabled, Is.False, "The second factor must not be active before it was confirmed.");
        Assert.That(setup.QrCodeSvg, Does.Contain("<svg"));
        Assert.That(setup.AuthenticatorUri, Does.StartWith("otpauth://totp/"));
        Assert.That(setup.AuthenticatorUri, Does.Contain("digits=6"));
        Assert.That(setup.AuthenticatorUri, Does.Contain("period=30"));
        Assert.That(setup.AuthenticatorUri, Does.Contain("algorithm=SHA1"));

        var wrongResult = await setupService.ConfirmSetupAsync(user, "000000").ConfigureAwait(false);
        Assert.That(wrongResult, Is.Null);
        Assert.That(user.IsTwoFactorEnabled, Is.False);

        var key = setup.SharedKey.Replace(" ", string.Empty);
        var recoveryCodes = await setupService.ConfirmSetupAsync(user, TestTotpGenerator.Generate(key)).ConfigureAwait(false);
        Assert.That(recoveryCodes, Is.Not.Null);
        Assert.That(user.IsTwoFactorEnabled, Is.True);
    }

    /// <summary>
    /// Tests that the authenticator key is not stored in plain text.
    /// </summary>
    [Test]
    public async Task AuthenticatorKeyIsProtectedAsync()
    {
        var (user, key) = await this.CreateUserWithAuthenticatorAsync("tester").ConfigureAwait(false);

        Assert.That(user.ProtectedAuthenticatorKey, Is.Not.Null);
        Assert.That(user.ProtectedAuthenticatorKey, Does.Not.Contain(key));
    }

    /// <summary>
    /// Tests that a sign in ticket can only be redeemed once.
    /// </summary>
    [Test]
    public void SignInTicketIsSingleUse()
    {
        var ticketService = new SignInTicketService();
        var ticketValue = ticketService.Issue(Array.Empty<System.Security.Claims.Claim>(), false);

        Assert.That(ticketService.TryRedeem(ticketValue, out var ticket), Is.True);
        Assert.That(ticket, Is.Not.Null);
        Assert.That(ticketService.TryRedeem(ticketValue, out _), Is.False);
        Assert.That(ticketService.TryRedeem("some-other-value", out _), Is.False);
    }

    /// <summary>
    /// Tests that the roles build up on each other.
    /// </summary>
    [Test]
    public void EffectiveRolesIncludeTheLessPrivilegedOnes()
    {
        Assert.That(AdminRoles.GetEffectiveRoles(AdminRoles.Administrator), Is.EquivalentTo(new[] { AdminRoles.Viewer, AdminRoles.Operator, AdminRoles.Administrator }));
        Assert.That(AdminRoles.GetEffectiveRoles(AdminRoles.Operator), Is.EquivalentTo(new[] { AdminRoles.Viewer, AdminRoles.Operator }));
        Assert.That(AdminRoles.GetEffectiveRoles(AdminRoles.Viewer), Is.EquivalentTo(new[] { AdminRoles.Viewer }));
        Assert.That(AdminRoles.GetEffectiveRoles("Unknown"), Is.Empty);
    }

    /// <summary>
    /// Tests that the role names and the role enum stay in sync, since the roles are stored
    /// and compared as strings, but selected as an enum in the user interface.
    /// </summary>
    [Test]
    public void RoleNamesMatchTheRoleEnum()
    {
        Assert.That(AdminRoles.All, Is.EqualTo(Enum.GetNames<AdminRole>()).AsCollection);
        Assert.That(AdminRoles.Viewer, Is.EqualTo(AdminRole.Viewer.ToString()));
        Assert.That(AdminRoles.Operator, Is.EqualTo(AdminRole.Operator.ToString()));
        Assert.That(AdminRoles.Administrator, Is.EqualTo(AdminRole.Administrator.ToString()));
    }

    /// <summary>
    /// Tests that the claims of an administrator contain the implied roles.
    /// </summary>
    [Test]
    public async Task ClaimsContainTheEffectiveRolesAsync()
    {
        var user = await this.CreateUserAsync("tester").ConfigureAwait(false);

        var claims = AdminLoginService.CreateClaims(user, true);
        var roles = claims
            .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        Assert.That(roles, Is.EquivalentTo(new[] { AdminRoles.Viewer, AdminRoles.Operator, AdminRoles.Administrator }));
        Assert.That(
            claims.Any(c => c.Type == AdminAuthenticationDefaults.AuthenticationMethodClaimType
                            && c.Value == AdminAuthenticationDefaults.MultiFactorAuthenticationMethod),
            Is.True);
    }

    private AdminLoginService GetLoginService()
    {
        return this._serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AdminLoginService>();
    }

    private async Task<AdminUser> CreateUserAsync(string loginName)
    {
        using var scope = this._serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AdminUser>>();
        var user = new AdminUser
        {
            LoginName = loginName,
            Roles = AdminRoles.Administrator,
        };

        var result = await userManager.CreateAsync(user, TestPassword).ConfigureAwait(false);
        Assert.That(result.Succeeded, Is.True, string.Join(' ', result.Errors.Select(e => e.Description)));
        return user;
    }

    private async Task<(AdminUser User, string Key)> CreateUserWithAuthenticatorAsync(string loginName)
    {
        var user = await this.CreateUserAsync(loginName).ConfigureAwait(false);
        using var scope = this._serviceProvider.CreateScope();
        var setupService = scope.ServiceProvider.GetRequiredService<AuthenticatorSetupService>();
        var setup = await setupService.BeginSetupAsync(user).ConfigureAwait(false);
        var key = setup.SharedKey.Replace(" ", string.Empty);
        var recoveryCodes = await setupService.ConfirmSetupAsync(user, TestTotpGenerator.Generate(key)).ConfigureAwait(false);
        Assert.That(recoveryCodes, Is.Not.Null, "The generated code should have been accepted.");
        return (user, key);
    }
}

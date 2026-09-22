// <copyright file="AuthenticatorSetupService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using Microsoft.AspNetCore.Identity;
using MUnique.OpenMU.Persistence.AdminAuth;
using QRCoder;

/// <summary>
/// The data which is needed to set an authenticator app up.
/// </summary>
/// <param name="SharedKey">The shared key, formatted in groups of four characters for manual entry.</param>
/// <param name="AuthenticatorUri">The otpauth uri which is encoded in the QR code.</param>
/// <param name="QrCodeSvg">The QR code as inline SVG.</param>
public record AuthenticatorSetup(string SharedKey, string AuthenticatorUri, string QrCodeSvg);

/// <summary>
/// Sets the time based one time password (TOTP) second factor of an admin panel user up.
/// </summary>
/// <remarks>
/// The parameters are deliberately kept at the defaults of SHA-1, 6 digits and a period of
/// 30 seconds. The Microsoft Authenticator app ignores deviating values in the otpauth uri and
/// calculates the default anyway, so a "stronger" configuration would just produce codes which
/// never validate.
/// </remarks>
public class AuthenticatorSetupService
{
    private const string Issuer = "OpenMU AdminPanel";
    private const int RecoveryCodeCount = 10;

    private readonly UserManager<AdminUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticatorSetupService"/> class.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    public AuthenticatorSetupService(UserManager<AdminUser> userManager)
    {
        this._userManager = userManager;
    }

    /// <summary>
    /// Creates a new authenticator key for the specified user and returns the data to set it up.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>The data which is needed to set the authenticator app up.</returns>
    /// <remarks>
    /// The second factor is not enabled yet - that only happens after the user proved with
    /// <see cref="ConfirmSetupAsync"/> that its authenticator app produces valid codes.
    /// Otherwise a mistake while scanning would lock the user out of its own panel.
    /// </remarks>
    public async Task<AuthenticatorSetup> BeginSetupAsync(AdminUser user)
    {
        await this._userManager.ResetAuthenticatorKeyAsync(user).ConfigureAwait(false);
        var key = await this._userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false)
                  ?? throw new InvalidOperationException("The authenticator key could not be created.");

        var uri = CreateAuthenticatorUri(user.LoginName, key);
        return new AuthenticatorSetup(FormatKey(key), uri, CreateQrCodeSvg(uri));
    }

    /// <summary>
    /// Verifies the specified code and enables the second factor if it's correct.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="code">The code of the authenticator app.</param>
    /// <returns>The generated recovery codes, if the code was correct; otherwise, <c>null</c>.</returns>
    public async Task<IReadOnlyList<string>?> ConfirmSetupAsync(AdminUser user, string code)
    {
        var normalizedCode = code.Replace(" ", string.Empty).Replace("-", string.Empty);
        var isValid = await this._userManager
            .VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, normalizedCode)
            .ConfigureAwait(false);
        if (!isValid)
        {
            return null;
        }

        await this._userManager.SetTwoFactorEnabledAsync(user, true).ConfigureAwait(false);
        var recoveryCodes = await this._userManager
            .GenerateNewTwoFactorRecoveryCodesAsync(user, RecoveryCodeCount)
            .ConfigureAwait(false);
        await this._userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);

        return recoveryCodes?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// Disables the second factor of the specified user and removes its authenticator key.
    /// </summary>
    /// <param name="user">The user.</param>
    public async Task DisableAsync(AdminUser user)
    {
        await this._userManager.SetTwoFactorEnabledAsync(user, false).ConfigureAwait(false);
        user.ProtectedAuthenticatorKey = null;
        user.RecoveryCodeHashes = null;
        user.LastAcceptedTotpStep = 0;
        await this._userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
    }

    /// <summary>
    /// Generates a new set of recovery codes for the specified user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>The new recovery codes.</returns>
    public async Task<IReadOnlyList<string>> GenerateRecoveryCodesAsync(AdminUser user)
    {
        var codes = await this._userManager
            .GenerateNewTwoFactorRecoveryCodesAsync(user, RecoveryCodeCount)
            .ConfigureAwait(false);
        return codes?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// Gets the number of recovery codes which are still available.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>The number of recovery codes which are still available.</returns>
    public Task<int> GetRemainingRecoveryCodeCountAsync(AdminUser user)
        => this._userManager.CountRecoveryCodesAsync(user);

    private static string CreateAuthenticatorUri(string loginName, string key)
    {
        var escapedIssuer = Uri.EscapeDataString(Issuer);
        var escapedLogin = Uri.EscapeDataString(loginName);

        // The issuer has to appear in the label as well as in the query, because the authenticator
        // apps use it to group and to name the entry.
        return $"otpauth://totp/{escapedIssuer}:{escapedLogin}?secret={key}&issuer={escapedIssuer}&algorithm=SHA1&digits=6&period=30";
    }

    private static string CreateQrCodeSvg(string uri)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
        var svgQrCode = new SvgQRCode(data);
        return svgQrCode.GetGraphic(4, "#000000", "#ffffff", drawQuietZones: true);
    }

    private static string FormatKey(string key)
    {
        var result = new StringBuilder();
        for (var i = 0; i < key.Length; i += 4)
        {
            result.Append(key.AsSpan(i, Math.Min(4, key.Length - i))).Append(' ');
        }

        return result.ToString().Trim();
    }
}

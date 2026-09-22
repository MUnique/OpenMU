// <copyright file="AdminUserSecretProtector.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

/// <summary>
/// Protects the secrets of an admin user, so they are not readable in a database dump.
/// </summary>
/// <remarks>
/// The authenticator key is password equivalent - whoever knows it can generate valid codes.
/// Note that the data protection key ring must be persisted, otherwise protected values
/// become unreadable after a restart and the affected users have to set their second factor up again.
/// </remarks>
public class AdminUserSecretProtector
{
    private readonly IDataProtector _protector;
    private readonly ILogger<AdminUserSecretProtector> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminUserSecretProtector"/> class.
    /// </summary>
    /// <param name="dataProtectionProvider">The data protection provider.</param>
    /// <param name="logger">The logger.</param>
    public AdminUserSecretProtector(IDataProtectionProvider dataProtectionProvider, ILogger<AdminUserSecretProtector> logger)
    {
        this._protector = dataProtectionProvider.CreateProtector("MUnique.OpenMU.AdminPanel.AdminUserSecrets.v1");
        this._logger = logger;
    }

    /// <summary>
    /// Protects the specified plain text value.
    /// </summary>
    /// <param name="plainText">The plain text value.</param>
    /// <returns>The protected value.</returns>
    public string Protect(string plainText) => this._protector.Protect(plainText);

    /// <summary>
    /// Unprotects the specified protected value.
    /// </summary>
    /// <param name="protectedValue">The protected value.</param>
    /// <returns>The plain text value; <c>null</c>, if it could not be unprotected.</returns>
    public string? Unprotect(string? protectedValue)
    {
        if (string.IsNullOrEmpty(protectedValue))
        {
            return null;
        }

        try
        {
            return this._protector.Unprotect(protectedValue);
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(
                ex,
                "A protected admin user secret could not be read. This usually means that the data protection key ring changed - the affected user has to set up its authenticator again.");
            return null;
        }
    }
}

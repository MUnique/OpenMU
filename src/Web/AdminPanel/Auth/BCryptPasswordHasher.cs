// <copyright file="BCryptPasswordHasher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using Microsoft.AspNetCore.Identity;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// An <see cref="IPasswordHasher{TUser}"/> which uses BCrypt, like the rest of this project does.
/// </summary>
public class BCryptPasswordHasher : IPasswordHasher<AdminUser>
{
    /// <inheritdoc />
    public string HashPassword(AdminUser user, string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <inheritdoc />
    public PasswordVerificationResult VerifyHashedPassword(AdminUser user, string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword))
        {
            return PasswordVerificationResult.Failed;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword)
                ? PasswordVerificationResult.Success
                : PasswordVerificationResult.Failed;
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return PasswordVerificationResult.Failed;
        }
    }
}

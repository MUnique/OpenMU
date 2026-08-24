// <copyright file="AdminUser.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// A user which is allowed to log into the admin panel.
/// </summary>
/// <remarks>
/// This is deliberately not related to <see cref="DataModel.Entities.Account"/>:
/// A game account password is typed into the game client and travels over the game protocol,
/// while an admin panel user can restart servers, edit the whole game configuration and read logs.
/// Sharing one secret between both would mean that a leaked game password grants server administration.
/// Additionally, the admin panel must be usable before the game database has been initialized,
/// which wouldn't be possible if the credentials were stored in the game data schema.
/// </remarks>
public class AdminUser
{
    /// <summary>
    /// Gets or sets the identifier of this user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the login name.
    /// </summary>
    public string LoginName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the normalized (upper case, invariant) login name which is used for lookups.
    /// </summary>
    public string NormalizedLoginName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hash of the password.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the security stamp which changes whenever a security relevant property changes.
    /// It's used to invalidate all existing sessions of this user.
    /// </summary>
    public string SecurityStamp { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the roles of this user, as a comma separated list.
    /// </summary>
    /// <seealso cref="AdminRoles"/>
    public string Roles { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the two factor authentication is enabled for this user.
    /// </summary>
    public bool IsTwoFactorEnabled { get; set; }

    /// <summary>
    /// Gets or sets the data protected authenticator (TOTP) key of this user.
    /// </summary>
    /// <remarks>
    /// The key is password equivalent, so it's never stored in plain text.
    /// </remarks>
    public string? ProtectedAuthenticatorKey { get; set; }

    /// <summary>
    /// Gets or sets the hashes of the still unused recovery codes of this user, separated by semicolons.
    /// </summary>
    /// <remarks>
    /// Only the hashes are stored, so a database dump doesn't hand out usable second factors.
    /// The codes themselves are random and long enough to make a fast hash sufficient here.
    /// </remarks>
    public string? RecoveryCodeHashes { get; set; }

    /// <summary>
    /// Gets or sets the last TOTP time step which was accepted for this user.
    /// </summary>
    /// <remarks>
    /// A time based one time password stays valid for a whole validation window.
    /// Remembering the last accepted step prevents that an observed code can be replayed within that window.
    /// </remarks>
    public long LastAcceptedTotpStep { get; set; }

    /// <summary>
    /// Gets or sets the number of failed login attempts since the last successful one.
    /// </summary>
    public int AccessFailedCount { get; set; }

    /// <summary>
    /// Gets or sets the date and time until which this user is locked out.
    /// </summary>
    public DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this user is disabled and therefore can't log in.
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this user has been created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time of the last successful login.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <inheritdoc />
    public override string ToString() => this.LoginName;
}

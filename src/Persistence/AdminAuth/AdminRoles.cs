// <copyright file="AdminRoles.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// The names of the <see cref="AdminRole"/>s, as they are stored and used in the claims.
/// </summary>
public static class AdminRoles
{
    /// <summary>
    /// Gets the role which is allowed to see the state of the servers and the game data, but can't change anything.
    /// </summary>
    public static string Viewer => nameof(AdminRole.Viewer);

    /// <summary>
    /// Gets the role which is additionally allowed to operate the servers, e.g. start and stop them,
    /// disconnect players and edit accounts.
    /// </summary>
    public static string Operator => nameof(AdminRole.Operator);

    /// <summary>
    /// Gets the role which is additionally allowed to change the game configuration, install updates,
    /// set the database up and manage the admin panel users.
    /// </summary>
    public static string Administrator => nameof(AdminRole.Administrator);

    /// <summary>
    /// Gets the role which is allowed to grant cash shop coins to accounts.
    /// </summary>
    /// <remarks>
    /// It's not one of the <see cref="AdminRole"/>s which build up on each other, so that it can be given
    /// alone, e.g. to the API key of a payment provider. An <see cref="Administrator"/> has it implicitly.
    /// </remarks>
    public static string CashShop => "CashShop";

    /// <summary>
    /// Gets the role of the API key of a server website, which may only use the website functions
    /// which an administrator selected for the key.
    /// </summary>
    /// <remarks>
    /// Like <see cref="CashShop"/>, it's not one of the <see cref="AdminRole"/>s which build up on each other.
    /// Unlike it, an <see cref="Administrator"/> doesn't have it implicitly: it includes checking the passwords
    /// of game accounts, which only a key that was created for this purpose should be able to do.
    /// </remarks>
    public static string WebManager => "WebManager";

    /// <summary>
    /// Gets the role which is allowed to issue login tickets, with which a game client logs into an account without its password.
    /// </summary>
    /// <remarks>
    /// It's meant for the API key of a website which signs the players in, e.g. with Discord.
    /// Like <see cref="CashShop"/>, it's given alone. Not even an <see cref="Administrator"/> has it implicitly,
    /// because it allows to log into every account.
    /// </remarks>
    public static string LoginTicket => "LoginTicket";

    /// <summary>
    /// Gets all defined roles which build up on each other, from the least to the most privileged one.
    /// </summary>
    public static IReadOnlyList<string> All { get; } = Enum.GetNames<AdminRole>();

    /// <summary>
    /// Gets the roles which are implied by the specified role, including the role itself.
    /// </summary>
    /// <param name="role">The role.</param>
    /// <returns>The role itself and all roles which are implied by it.</returns>
    /// <remarks>
    /// The roles build up on each other, so an <see cref="Administrator"/> is implicitly
    /// an <see cref="Operator"/> and a <see cref="Viewer"/> as well, and has the <see cref="CashShop"/> role.
    /// </remarks>
    public static IEnumerable<string> GetEffectiveRoles(string role)
    {
        if (string.Equals(role, CashShop, StringComparison.OrdinalIgnoreCase))
        {
            yield return CashShop;
            yield break;
        }

        if (string.Equals(role, WebManager, StringComparison.OrdinalIgnoreCase))
        {
            yield return WebManager;
            yield break;
        }

        if (string.Equals(role, LoginTicket, StringComparison.OrdinalIgnoreCase))
        {
            yield return LoginTicket;
            yield break;
        }

        if (!Enum.TryParse<AdminRole>(role, out var parsedRole) || !Enum.IsDefined(parsedRole))
        {
            yield break;
        }

        foreach (var candidate in Enum.GetValues<AdminRole>())
        {
            if (candidate <= parsedRole)
            {
                yield return candidate.ToString();
            }
        }

        if (parsedRole == AdminRole.Administrator)
        {
            yield return CashShop;
        }
    }
}

// <copyright file="AdminRoles.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// The roles which can be assigned to an <see cref="AdminUser"/>.
/// </summary>
public static class AdminRoles
{
    /// <summary>
    /// The role which is allowed to see the state of the servers and the game data, but can't change anything.
    /// </summary>
    public const string Viewer = "Viewer";

    /// <summary>
    /// The role which is additionally allowed to operate the servers, e.g. start and stop them,
    /// disconnect players and edit accounts.
    /// </summary>
    public const string Operator = "Operator";

    /// <summary>
    /// The role which is additionally allowed to change the game configuration, install updates,
    /// set the database up and manage the admin panel users.
    /// </summary>
    public const string Administrator = "Administrator";

    /// <summary>
    /// Gets all defined roles, from the least to the most privileged one.
    /// </summary>
    public static IReadOnlyList<string> All { get; } = new[] { Viewer, Operator, Administrator };

    /// <summary>
    /// Gets the roles which are implied by the specified role, including the role itself.
    /// </summary>
    /// <param name="role">The role.</param>
    /// <returns>The role itself and all roles which are implied by it.</returns>
    /// <remarks>
    /// The roles build up on each other, so an <see cref="Administrator"/> is implicitly
    /// an <see cref="Operator"/> and a <see cref="Viewer"/> as well.
    /// </remarks>
    public static IEnumerable<string> GetEffectiveRoles(string role)
    {
        var index = All.ToList().IndexOf(role);
        if (index < 0)
        {
            yield break;
        }

        for (var i = 0; i <= index; i++)
        {
            yield return All[i];
        }
    }
}

// <copyright file="AdminRole.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// The roles which can be assigned to an <see cref="AdminUser"/>, from the least to the most
/// privileged one. The roles build up on each other.
/// </summary>
public enum AdminRole
{
    /// <summary>
    /// May see the state of the servers and the game data, but can't change anything.
    /// </summary>
    Viewer,

    /// <summary>
    /// May additionally operate the servers, e.g. start and stop them,
    /// disconnect players and edit accounts.
    /// </summary>
    Operator,

    /// <summary>
    /// May additionally change the game configuration, install updates,
    /// set the database up and manage the admin panel users.
    /// </summary>
    Administrator,
}

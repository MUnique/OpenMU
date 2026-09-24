// <copyright file="GuildRoleAssignType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// The assignment type of a guild role assignment request (C1 E1), as sent by the client.
/// All known types are validated against the role limits.
/// No behavioral difference between the two standard types is known.
/// </summary>
public enum GuildRoleAssignType : byte
{
    /// <summary>
    /// Role assignment validated against the role limits.
    /// </summary>
    Standard = 1,

    /// <summary>
    /// Role assignment validated against the role limits.
    /// No behavioral difference from <see cref="Standard"/> is known.
    /// </summary>
    StandardAlternate = 2,

    /// <summary>
    /// Role assignment which the server forwards without validating the role limits.
    /// </summary>
    Direct = 3,
}

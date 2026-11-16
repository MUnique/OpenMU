// <copyright file="AllianceCreationResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// Defines the result of an alliance creation attempt.
/// </summary>
public enum AllianceCreationResult
{
    /// <summary>
    /// The alliance creation failed for an unspecified reason.
    /// </summary>
    Failed,

    /// <summary>
    /// The alliance was created successfully.
    /// </summary>
    Success,

    /// <summary>
    /// The master guild could not be found.
    /// </summary>
    MasterGuildNotFound,

    /// <summary>
    /// The target guild could not be found.
    /// </summary>
    TargetGuildNotFound,

    /// <summary>
    /// The target guild is already a member of an alliance.
    /// </summary>
    TargetGuildAlreadyInAlliance,

    /// <summary>
    /// The maximum number of guilds allowed in an alliance has been reached.
    /// </summary>
    MaximumAllianceSizeReached,

    /// <summary>
    /// The guild could not be found in the target context.
    /// </summary>
    GuildNotFoundInTargetContext,

    /// <summary>
    /// An unexpected error occurred during alliance creation.
    /// </summary>
    Error,
}

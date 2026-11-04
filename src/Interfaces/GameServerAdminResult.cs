// <copyright file="GameServerAdminResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>A confirmed result from the game server which owns the live state.</summary>
/// <param name="Succeeded">Whether the action completed successfully.</param>
/// <param name="Error">Safe operator guidance when the action was refused.</param>
public sealed record GameServerAdminResult(bool Succeeded, string? Error = null)
{
    /// <summary>Gets a successful result.</summary>
    public static GameServerAdminResult Success { get; } = new(true);
}

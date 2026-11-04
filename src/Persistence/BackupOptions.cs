// <copyright file="BackupOptions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

/// <summary>
/// Options which define what a backup contains.
/// </summary>
public sealed record BackupOptions
{
    /// <summary>
    /// Gets the default options, which include everything.
    /// </summary>
    public static BackupOptions Default { get; } = new();

    /// <summary>
    /// Gets a value indicating whether the accounts are included in the backup.
    /// Exporting the accounts of a running server takes the most time, so it can be
    /// skipped when only the configuration should be transferred.
    /// </summary>
    public bool IncludeAccounts { get; init; } = true;
}

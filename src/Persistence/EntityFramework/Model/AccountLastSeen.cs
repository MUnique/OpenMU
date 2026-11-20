// <copyright file="AccountLastSeen.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Model;

// ReSharper disable UnusedAutoPropertyAccessor.Local They're used by the entity framework core (reflection).
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Subset of an <see cref="Account"/>, used to query when it was last seen in the game.
/// </summary>
[Table(nameof(Account), Schema = "data")]
public class AccountLastSeen
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the time when the account was last seen in the game.
    /// </summary>
    public DateTime LastSeenDate { get; private set; }
}

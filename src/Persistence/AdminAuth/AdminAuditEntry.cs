// <copyright file="AdminAuditEntry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>A durable record of an attempted administrative action. Pending means its outcome is unknown.</summary>
public sealed class AdminAuditEntry
{
    /// <summary>Gets or sets the audit identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the UTC start time.</summary>
    public DateTime StartedAt { get; set; }

    /// <summary>Gets or sets the UTC completion time.</summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>Gets or sets the stable principal identifier.</summary>
    public string PrincipalId { get; set; } = string.Empty;

    /// <summary>Gets or sets the display name.</summary>
    public string PrincipalName { get; set; } = string.Empty;

    /// <summary>Gets or sets the initiating front end.</summary>
    public string Frontend { get; set; } = string.Empty;

    /// <summary>Gets or sets the operation name.</summary>
    public string Operation { get; set; } = string.Empty;

    /// <summary>Gets or sets the sanitized arguments.</summary>
    public string ArgumentsJson { get; set; } = "{}";

    /// <summary>Gets or sets the sanitized result projection.</summary>
    public string ResultJson { get; set; } = "{}";

    /// <summary>Gets or sets the action outcome, without exception details.</summary>
    public string Outcome { get; set; } = "Pending";
}

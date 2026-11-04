// <copyright file="AdminAuditSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Host-owned audit policy for the explicit in-memory demo deployment.</summary>
public sealed class AdminAuditSettings
{
    /// <summary>Gets a value indicating whether panel actions use a non-durable log audit in demo mode.</summary>
    public bool IsDemo { get; init; }
}

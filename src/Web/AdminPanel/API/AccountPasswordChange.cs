// <copyright file="AccountPasswordChange.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.ComponentModel.DataAnnotations;

/// <summary>An offline password reset; the operations layer enforces the game's password limits.</summary>
public sealed class AccountPasswordChange
{
    /// <summary>Gets or sets the new password, which is never returned or audited.</summary>
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    /// <summary>Gets or sets the audit reason.</summary>
    [Required]
    [StringLength(1000)]
    public string Reason { get; set; } = string.Empty;
}

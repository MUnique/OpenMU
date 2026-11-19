// <copyright file="WebsitePasswordChangeRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.ComponentModel.DataAnnotations;

/// <summary>A password change of a player at the website; the passwords are never returned, logged or audited.</summary>
public sealed class WebsitePasswordChangeRequest
{
    /// <summary>Gets or sets the current password of the game account.</summary>
    [Required]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>Gets or sets the new password of the game account.</summary>
    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;
}

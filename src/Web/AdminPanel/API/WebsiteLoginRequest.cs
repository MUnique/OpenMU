// <copyright file="WebsiteLoginRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.ComponentModel.DataAnnotations;

/// <summary>The login of a player at the website, with the login name and password of the game account.</summary>
public sealed class WebsiteLoginRequest
{
    /// <summary>Gets or sets the login name of the game account.</summary>
    [Required]
    public string LoginName { get; set; } = string.Empty;

    /// <summary>Gets or sets the password, which is never returned, logged or audited.</summary>
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

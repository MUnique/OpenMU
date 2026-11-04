// <copyright file="PlayerLoginRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.PlayerPortal.Api;

using System.ComponentModel.DataAnnotations;

/// <summary>Native game credentials accepted by the player portal.</summary>
public sealed class PlayerLoginRequest
{
    /// <summary>Gets or sets the OpenMU login name, limited to the Season 6 login packet size.</summary>
    [Required]
    [MinLength(3)]
    [MaxLength(10)]
    [RegularExpression(@"^[\x21-\x7E]+$")]
    public string LoginName { get; set; } = string.Empty;

    /// <summary>Gets or sets the OpenMU password, limited to the Season 6 login packet size.</summary>
    [Required]
    [MinLength(3)]
    [MaxLength(20)]
    [RegularExpression(@"^[\x20-\x7E]+$")]
    public string Password { get; set; } = string.Empty;
}

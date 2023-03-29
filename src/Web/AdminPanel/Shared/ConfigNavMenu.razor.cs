// <copyright file="ConfigNavMenu.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Shared;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Navigation menu of the admin panel.
/// </summary>
public partial class ConfigNavMenu
{
    private bool _expandGameConfig;

    /// <summary>
    /// Gets or sets the game configuration identifier.
    /// </summary>
    [Parameter]
    [Required]
    public Guid GameConfigurationId { get; set; }

    private void ToggleGameConfig()
    {
        this._expandGameConfig = !this._expandGameConfig;
    }
}
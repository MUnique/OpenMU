// <copyright file="ConfigNavMenu.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Layout;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Navigation menu of the admin panel.
/// </summary>
public partial class ConfigNavMenu
{
    private bool _expandGameConfig;
    
    [Inject]
    private NavigationHistory NavigationHistory { get; set; } = null!;

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
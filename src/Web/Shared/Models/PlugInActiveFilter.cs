// <copyright file="PlugInActiveFilter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Models;

/// <summary>
/// The filter options for the active state of plugins.
/// </summary>
public enum PlugInActiveFilter
{
    /// <summary>
    /// Shows all plugins, regardless of their active state.
    /// </summary>
    All,

    /// <summary>
    /// Shows only active plugins.
    /// </summary>
    Active,

    /// <summary>
    /// Shows only inactive plugins.
    /// </summary>
    Inactive,
}

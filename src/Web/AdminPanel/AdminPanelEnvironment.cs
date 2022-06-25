// <copyright file="AdminPanelEnvironment.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel;

/// <summary>
/// Environment information about the admin panel.
/// </summary>
public static class AdminPanelEnvironment
{
    /// <summary>
    /// Gets a value indicating whether the hosting of the admin panel is embedded into an all-in-one deployment.
    /// </summary>
    public static bool IsHostingEmbedded { get; internal set; }
}
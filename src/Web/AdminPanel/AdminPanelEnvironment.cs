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

    /// <summary>
    /// Gets a value indicating whether Grafana is available under <c>/grafana/</c> to look at the logs, metrics and traces.
    /// </summary>
    /// <remarks>
    /// The distributed deployment always comes with it. The all-in-one deployment only
    /// when it exports its telemetry, see the docker compose file for monitoring.
    /// </remarks>
    public static bool IsGrafanaAvailable { get; internal set; } = true;
}
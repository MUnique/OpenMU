// <copyright file="ApiKeyToolGroupResources.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using MUnique.OpenMU.Web.AdminPanel.Properties;

/// <summary>Localized caption for the optional MCP tool restriction field.</summary>
public static class ApiKeyToolGroupResources
{
    /// <summary>Gets the localized allowlist caption used by generated forms and the key table.</summary>
    public static string Caption => Resources.ResourceManager.GetString("AllowedMcpTools", Resources.Culture) ?? nameof(Caption);
}

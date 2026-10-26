// <copyright file="RestrictedTestTools.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>A test-only tool which verifies the SDK's per-tool authorization filters.</summary>
[McpServerToolType]
public sealed class RestrictedTestTools
{
    private RestrictedTestTools()
    {
    }

    /// <summary>Returns a harmless constant for administrators.</summary>
    /// <returns>The test result.</returns>
    [McpServerTool(Name = "test_admin_only", ReadOnly = true)]
    [Authorize(Roles = nameof(AdminRole.Administrator))]
    public static string Read() => "authorized";
}

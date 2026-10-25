// <copyright file="OperationParityTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Mcp;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Ensures admin operations stay reachable by agents with consistent authorization.</summary>
[TestFixture]
public class OperationParityTests
{
    /// <summary>Every public operation must have a tool, or a documented reason for its exclusion.</summary>
    [Test]
    public void AllOperationsAreExposedWithAtLeastTheirRequiredRole()
    {
        var tools = typeof(AdminReadTools).Assembly.GetTypes().SelectMany(t => t.GetMethods())
            .Where(m => m.GetCustomAttribute<McpServerToolAttribute>() is not null).ToArray();
        var operations = typeof(AccountReadOperations).Assembly.GetTypes().Where(t => t.Name.EndsWith("Operations", StringComparison.Ordinal))
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
        foreach (var operation in operations)
        {
            if (operation.GetCustomAttribute<NotExposedToAgentsAttribute>() is { } excluded)
            {
                Assert.That(excluded.Reason, Is.Not.Empty, operation.Name);
                continue;
            }

            var role = operation.GetCustomAttribute<AdminOperationAttribute>();
            Assert.That(role, Is.Not.Null, $"Missing operation role: {operation}");
            var matching = tools.Where(t => t.GetCustomAttributes<ExposesOperationAttribute>().Any(e => e.ServiceType == operation.DeclaringType && e.MethodName == operation.Name)).ToArray();
            Assert.That(matching, Is.Not.Empty, $"No MCP tool for {operation}");
            foreach (var tool in matching)
            {
                var authorizedRoles = tool.GetCustomAttributes<AuthorizeAttribute>().SelectMany(a => (a.Roles ?? string.Empty).Split(','));
                Assert.That(authorizedRoles.All(r => AdminRoles.GetEffectiveRoles(r).Contains(role!.Role)), Is.True, tool.Name);
                Assert.That(authorizedRoles, Is.Not.Empty, tool.Name);
                Assert.That(tool.GetCustomAttribute<McpServerToolAttribute>()!.ReadOnly, Is.EqualTo(role!.ReadOnly), tool.Name);
            }
        }

        foreach (var tool in tools)
        {
            foreach (var exposure in tool.GetCustomAttributes<ExposesOperationAttribute>())
            {
                Assert.That(exposure.ServiceType.GetMethod(exposure.MethodName), Is.Not.Null, tool.Name);
            }
        }
    }
}

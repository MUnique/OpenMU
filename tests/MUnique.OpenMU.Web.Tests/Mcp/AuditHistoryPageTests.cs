// <copyright file="AuditHistoryPageTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.AdminPanel.Pages;

/// <summary>Verifies audit history rendering and server-side role checks behind the page.</summary>
[TestFixture]
public sealed class AuditHistoryPageTests
{
    /// <summary>Untrusted audit arguments are displayed as encoded text.</summary>
    [Test]
    public async Task EncodesAuditArgumentsAsync()
    {
        using var context = new BunitContext();
        context.Services.AddLocalization();
        var audit = new InMemoryAdminAuditRepository();
        await audit.AddAsync(new AdminAuditEntry { Id = Guid.NewGuid(), StartedAt = DateTime.UtcNow,
            PrincipalName = "agent", Operation = "server_send_message", ArgumentsJson = "<script>not executable</script>" }).ConfigureAwait(false);
        context.Services.AddSingleton(new AuditReadOperations(AdminOperationTestServices.CreateExecutor(audit: audit), audit));
        var page = context.Render<AuditHistory>();
        page.WaitForAssertion(() => Assert.That(page.Markup, Does.Contain("server_send_message")));
        Assert.That(page.FindAll("script"), Is.Empty);
        Assert.That(page.Markup, Does.Contain("&lt;script&gt;"));
    }

    /// <summary>A caller without Administrator cannot obtain audit data even if the component is directly rendered.</summary>
    [Test]
    public async Task ViewerCannotReadAuditHistoryAsync()
    {
        using var context = new BunitContext();
        context.Services.AddLocalization();
        var audit = new InMemoryAdminAuditRepository();
        await audit.AddAsync(new AdminAuditEntry { Id = Guid.NewGuid(), ArgumentsJson = "private-audit-data" }).ConfigureAwait(false);
        var executor = AdminOperationTestServices.CreateExecutor(audit: audit, actor: () => new AdminActor("viewer-id", "viewer", "panel", [AdminRoles.Viewer]));
        context.Services.AddSingleton(new AuditReadOperations(executor, audit));
        var page = context.Render<AuditHistory>();
        page.WaitForAssertion(() => Assert.That(page.FindAll(".alert-warning"), Has.Count.EqualTo(1)));
        Assert.That(page.Markup, Does.Not.Contain("private-audit-data"));
    }
}

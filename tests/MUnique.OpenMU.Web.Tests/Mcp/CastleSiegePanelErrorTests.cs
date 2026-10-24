// <copyright file="CastleSiegePanelErrorTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Reflection;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;
using MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

/// <summary>Operational audit errors remain visible in the Castle Siege panel.</summary>
[TestFixture]
public sealed class CastleSiegePanelErrorTests
{
    /// <summary>The workflow shows both pre-action failures and uncertain outcomes without replacing their guidance.</summary>
    [TestCase(false, "Audit storage is unavailable")]
    [TestCase(true, "Check the current state before retrying")]
    public async Task DisplaysSafeOperationalGuidanceAsync(bool uncertainOutcome, string guidance)
    {
        using var context = new BunitContext();
        var audit = new InMemoryAdminAuditRepository { RejectWrites = !uncertainOutcome, RejectCompletion = uncertainOutcome };
        var provider = new Mock<IServerProvider>();
        provider.SetupGet(p => p.Servers).Returns(new List<IManageableServer>());
        var toast = new Mock<IToastService>();
        context.Services.AddLogging();
        context.Services.AddSingleton<NavigationHistory>();
        context.Services.AddSingleton(new CastleSiegeManagementService(provider.Object));
        context.Services.AddSingleton<IToastService>(toast.Object);
        context.Services.AddSingleton<IModalService>(new TestModalService());
        context.Services.AddSingleton(AdminOperationTestServices.CreateExecutor(audit: audit));
        var component = context.Render<CastleSiegeManagement>();
        var executed = false;
        Func<ValueTask<CastleSiegeAdministrationResult>> operation = () =>
        {
            executed = true;
            return ValueTask.FromResult(CastleSiegeAdministrationResult.Succeeded);
        };
        // Exercise the page's common action workflow without constructing an unrelated live siege event.
        var workflow = typeof(CastleSiegeManagement).GetMethod("RunOperationAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;
        await component.InvokeAsync(() => (Task)workflow.Invoke(component.Instance, [operation, "Succeeded", null, "ForceStateAsync"])!).ConfigureAwait(false);
        toast.Verify(t => t.ShowError(It.Is<string>(message => message.Contains(guidance, StringComparison.Ordinal)), null), Times.Once);
        toast.Verify(t => t.ShowSuccess(It.IsAny<string>(), It.IsAny<string?>()), Times.Never);
        Assert.That(executed, Is.EqualTo(uncertainOutcome));
        Assert.That(component.Find("h1"), Is.Not.Null);
        if (uncertainOutcome)
        {
            Assert.That((await audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single().Outcome, Is.EqualTo("Pending"));
        }
    }
}

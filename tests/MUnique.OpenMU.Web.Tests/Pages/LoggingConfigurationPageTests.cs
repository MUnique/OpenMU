// <copyright file="LoggingConfigurationPageTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Pages;

using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MUnique.OpenMU.Web.AdminPanel;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// Tests for the <see cref="LoggingConfiguration"/> page.
/// </summary>
/// <remarks>
/// The page loads the configuration file asynchronously. When the tests interact with it right after
/// the content appeared, the page may still finish its initialization, and the event is handled after
/// <c>Click</c> or <c>Change</c> returned. So the tests wait for the result of each interaction.
/// </remarks>
[TestFixture]
[NonParallelizable]
public class LoggingConfigurationPageTests
{
    private BunitContext _context = null!;
    private Mock<IToastService> _toastService = null!;
    private CultureInfo _previousUiCulture = null!;
    private bool _previousIsHostingEmbedded;
    private string _filePath = string.Empty;

    /// <summary>
    /// Sets up the test context with a configuration file.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
        this._previousIsHostingEmbedded = AdminPanelEnvironment.IsHostingEmbedded;
        AdminPanelEnvironment.IsHostingEmbedded = true;

        this._filePath = Path.GetTempFileName();
        File.WriteAllText(this._filePath, """{ "Serilog": { "MinimumLevel": { "Default": "Information", "Override": { "Microsoft": "Fatal" } } } }""");

        this._toastService = new Mock<IToastService>();
        this._context = new BunitContext();
        this._context.Services.AddSingleton(new LoggingConfigurationService(this._filePath));
        this._context.Services.AddSingleton(this._toastService.Object);
        this._context.Services.AddSingleton(Mock.Of<ILogger<LoggingConfiguration>>());
        this._context.ComponentFactories.AddStub<MUnique.OpenMU.Web.Shared.Components.Breadcrumb>();
    }

    /// <summary>
    /// Restores the environment and deletes the configuration file.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        this._context.Dispose();
        File.Delete(this._filePath);
        AdminPanelEnvironment.IsHostingEmbedded = this._previousIsHostingEmbedded;
        CultureInfo.CurrentUICulture = this._previousUiCulture;
    }

    /// <summary>
    /// The levels which are changed in the page are saved to the file.
    /// </summary>
    [Test]
    public void SavesChangedLevels()
    {
        var cut = this._context.Render<LoggingConfiguration>();
        cut.WaitForState(() => cut.FindAll("#defaultLogLevel").Count > 0);
        Assert.That(cut.Find("#defaultLogLevel").GetAttribute("value"), Is.EqualTo("Information"));

        cut.Find("#defaultLogLevel").Change("Debug");
        cut.WaitForState(() => cut.Find("#defaultLogLevel").GetAttribute("value") == "Debug");
        cut.FindAll("button").First(b => b.TextContent.Contains(Resources.AddNew)).Click();
        cut.WaitForState(() => cut.FindAll("tbody input").Count == 2);
        cut.FindAll("tbody input")[1].Change("MUnique.OpenMU.GameLogic");
        cut.WaitForState(() => cut.FindAll("tbody input")[1].GetAttribute("value") == "MUnique.OpenMU.GameLogic");
        cut.FindAll("tbody select")[1].Change("Verbose");
        cut.WaitForState(() => cut.FindAll("tbody select")[1].GetAttribute("value") == "Verbose");
        cut.FindAll("button").First(b => b.TextContent.Contains(Resources.Save)).Click();

        cut.WaitForAssertion(() => this._toastService.Verify(t => t.ShowSuccess(Resources.LoggingConfigurationSaved, null)));
        var minimumLevel = JsonNode.Parse(File.ReadAllText(this._filePath))!["Serilog"]!["MinimumLevel"]!;
        Assert.That(minimumLevel["Default"]?.ToString(), Is.EqualTo("Debug"));
        Assert.That(minimumLevel["Override"]?["Microsoft"]?.ToString(), Is.EqualTo("Fatal"));
        Assert.That(minimumLevel["Override"]?["MUnique.OpenMU.GameLogic"]?.ToString(), Is.EqualTo("Verbose"));
    }

    /// <summary>
    /// An override without a source is not saved.
    /// </summary>
    [Test]
    public void DoesNotSaveOverrideWithoutSource()
    {
        var content = File.ReadAllText(this._filePath);
        var cut = this._context.Render<LoggingConfiguration>();
        cut.WaitForState(() => cut.FindAll("tbody tr").Count == 1);

        cut.FindAll("button").First(b => b.TextContent.Contains(Resources.AddNew)).Click();
        cut.WaitForState(() => cut.FindAll("tbody tr").Count == 2);
        cut.FindAll("button").First(b => b.TextContent.Contains(Resources.Save)).Click();

        cut.WaitForAssertion(() => this._toastService.Verify(t => t.ShowError(Resources.LogLevelOverridesInvalid, null)));
        Assert.That(File.ReadAllText(this._filePath), Is.EqualTo(content));
    }

    /// <summary>
    /// Invalid JSON in the advanced editor is not saved.
    /// </summary>
    [Test]
    public void DoesNotSaveInvalidJson()
    {
        var content = File.ReadAllText(this._filePath);
        var cut = this._context.Render<LoggingConfiguration>();
        cut.WaitForState(() => cut.FindAll("button.nav-link").Count == 2);

        cut.FindAll("button.nav-link")[1].Click();
        cut.WaitForElement("textarea");
        cut.Find("textarea").Change("{ \"MinimumLevel\": ");
        cut.WaitForState(() => cut.Find("textarea").GetAttribute("value") == "{ \"MinimumLevel\": ");
        cut.FindAll("button").First(b => b.TextContent.Contains(Resources.Save)).Click();

        cut.WaitForAssertion(() => this._toastService.Verify(t => t.ShowError(It.Is<string>(m => m.StartsWith("The JSON is invalid")), null)));
        Assert.That(File.ReadAllText(this._filePath), Is.EqualTo(content));
    }
}

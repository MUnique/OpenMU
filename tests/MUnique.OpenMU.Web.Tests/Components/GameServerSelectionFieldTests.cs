// <copyright file="GameServerSelectionFieldTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Components;

using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.Shared.Components.Form;
using MUnique.OpenMU.Web.Shared.Services;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

/// <summary>
/// Tests for the <see cref="GameServerSelectionField"/> component.
/// </summary>
[TestFixture]
public class GameServerSelectionFieldTests
{
    private BunitContext _context = null!;

    /// <summary>
    /// Seeds two game servers and wires the component services.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        var provider = new InMemoryPersistenceContextProvider();
        using (var context = provider.CreateNewConfigurationContext())
        {
            var normal = context.CreateNew<GameServerDefinition>();
            normal.ServerID = 0;
            normal.Description = "Normal";
            normal.PvpEnabled = true;

            var peaceful = context.CreateNew<GameServerDefinition>();
            peaceful.ServerID = 1;
            peaceful.Description = "Peaceful";
            peaceful.PvpEnabled = false;
        }

        this._context = new BunitContext();
        this._context.Services.AddSingleton<IChangeNotificationService, ChangeNotificationService>();
        this._context.Services.AddLogging();
        this._context.Services.AddSingleton<IPersistenceContextProvider>(provider);
    }

    /// <summary>
    /// Releases the renderer.
    /// </summary>
    [TearDown]
    public void TearDown() => this._context.Dispose();

    /// <summary>
    /// Every configured server renders as a checkbox with its id, description and PvP flag.
    /// </summary>
    [Test]
    public void Servers_RenderAsLabeledCheckboxes()
    {
        var component = this.RenderField(new SelectionModel());

        component.WaitForAssertion(() => Assert.That(component.FindAll("input[type='checkbox']"), Has.Count.EqualTo(2)));
        var labels = component.FindAll(".form-check-label")
            .Select(label => Regex.Replace(label.TextContent, @"\s+", " ").Trim());
        Assert.That(labels, Is.EquivalentTo(new[] { "#0 - Normal (PvP)", "#1 - Peaceful (PvE)" }));
    }

    /// <summary>
    /// Selected servers render checked, the others unchecked.
    /// </summary>
    [Test]
    public void SelectedServers_RenderChecked()
    {
        var component = this.RenderField(new SelectionModel { Selection = new List<byte> { 1 } });

        component.WaitForAssertion(() => Assert.That(component.FindAll("input[type='checkbox']"), Has.Count.EqualTo(2)));
        var checkboxes = component.FindAll("input[type='checkbox']");
        Assert.That(checkboxes[0].HasAttribute("checked"), Is.False);
        Assert.That(checkboxes[1].HasAttribute("checked"), Is.True);
    }

    /// <summary>
    /// Toggling a checkbox updates the selection.
    /// </summary>
    [Test]
    public void TogglingCheckbox_UpdatesSelection()
    {
        var model = new SelectionModel();
        var component = this.RenderField(model);

        component.WaitForAssertion(() => Assert.That(component.FindAll("input[type='checkbox']"), Has.Count.EqualTo(2)));
        component.FindAll("input[type='checkbox']")[0].Change(true);
        Assert.That(model.Selection, Is.EqualTo(new byte[] { 0 }));

        component.FindAll("input[type='checkbox']")[0].Change(false);
        Assert.That(model.Selection, Is.Empty);
    }

    /// <summary>
    /// Select-all ticks every server, clear empties the selection (empty means all servers).
    /// </summary>
    [Test]
    public void SelectAllAndClear_UpdateSelection()
    {
        var model = new SelectionModel();
        var component = this.RenderField(model);

        component.WaitForAssertion(() => Assert.That(component.FindAll("input[type='checkbox']"), Has.Count.EqualTo(2)));
        var buttons = component.FindAll("button");
        buttons.Single(button => button.TextContent.Contains("Select all", StringComparison.Ordinal)).Click();
        Assert.That(model.Selection, Is.EqualTo(new byte[] { 0, 1 }));

        component.FindAll("button").Single(button => button.TextContent.Contains("Clear", StringComparison.Ordinal)).Click();
        Assert.That(model.Selection, Is.Empty);
    }

    /// <summary>
    /// Selected servers which no longer exist render as removable rows instead of silently disabling the event.
    /// </summary>
    [Test]
    public void StaleSelection_RendersRemovableRow()
    {
        var model = new SelectionModel { Selection = new List<byte> { 1, 5 } };
        var component = this.RenderField(model);

        component.WaitForAssertion(() => Assert.That(component.FindAll("input[type='checkbox']"), Has.Count.EqualTo(3)));
        var labels = component.FindAll(".form-check-label")
            .Select(label => Regex.Replace(label.TextContent, @"\s+", " ").Trim());
        Assert.That(labels, Does.Contain("Server 5 (no longer available)"));

        component.FindAll("input[type='checkbox']")[2].Change(false);
        Assert.That(model.Selection, Is.EqualTo(new byte[] { 1 }));
    }

    private IRenderedComponent<EditForm> RenderField(SelectionModel model)
    {
        return this._context.Render<EditForm>(parameters => parameters
            .Add(form => form.Model, model)
            .Add(form => form.ChildContent, (RenderFragment<EditContext>)(_ => builder =>
            {
                builder.OpenComponent<GameServerSelectionField>(0);
                builder.AddAttribute(1, nameof(GameServerSelectionField.Value), model.Selection);
                builder.AddAttribute(2, nameof(GameServerSelectionField.ValueExpression), (Expression<Func<IList<byte>>>)(() => model.Selection));
                builder.AddAttribute(3, nameof(GameServerSelectionField.ValueChanged), EventCallback.Factory.Create<IList<byte>>(this, value => model.Selection = value));
                builder.CloseComponent();
            })));
    }

    /// <summary>
    /// A form model which holds the edited server selection.
    /// </summary>
    private sealed class SelectionModel
    {
        /// <summary>
        /// Gets or sets the selected server ids; empty means all servers.
        /// </summary>
        public IList<byte> Selection { get; set; } = new List<byte>();
    }
}

// <copyright file="LookupFieldTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.Shared.Components.Form;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Tests for lookup components used to discover and select available options.
/// </summary>
[TestFixture]
public class LookupFieldTests
{
    private static readonly TestOption[] AvailableOptions =
    [
        new("Option A"),
        new("Option B"),
        new("Option C"),
    ];

    /// <summary>
    /// All available options are displayed when a multi-lookup field receives focus and discovery is enabled.
    /// </summary>
    [Test]
    public void MultiLookupField_ShowAllOnFocus_ShowsAvailableOptions()
    {
        using var context = CreateContext();
        var model = new TestModel();
        var component = RenderMultiLookupField(context, model, true);

        component.Find("input").Focus();

        component.WaitForAssertion(() =>
            Assert.That(GetDisplayedOptionNames(component),
                Is.EquivalentTo(new[] { "Option A", "Option B", "Option C" })));
    }

    /// <summary>
    /// The default behavior does not display options when the field receives focus.
    /// </summary>
    [Test]
    public void MultiLookupField_WithoutShowAllOnFocus_DoesNotLoadOptions()
    {
        using var context = CreateContext();
        var model = new TestModel();
        var controller = new TestLookupController(AvailableOptions);
        var component = RenderMultiLookupField(context, model, false, controller);

        component.Find("input").Focus();

        Assert.That(controller.SearchCount, Is.Zero);
        Assert.That(component.FindAll(".multi-lookup-field__dropdown"), Is.Empty);
    }

    /// <summary>
    /// Typing a search query filters the available options.
    /// </summary>
    [Test]
    public async Task MultiLookupField_SearchInput_FiltersOptions()
    {
        using var context = CreateContext();
        var model = new TestModel();
        var component = RenderMultiLookupField(context, model, true);

        component.Find("input").Focus();
        await component.Find("input").InputAsync("Option B");

        Assert.That(
            GetDisplayedOptionNames(component),
            Is.EqualTo(new[] { "Option B" }));
    }

    /// <summary>
    /// Clearing the search query restores all available options.
    /// </summary>
    [Test]
    public async Task MultiLookupField_ClearingSearch_RestoresAllOptions()
    {
        using var context = CreateContext();
        var model = new TestModel();
        var component = RenderMultiLookupField(context, model, true);

        component.Find("input").Focus();
        await component.Find("input").InputAsync("Option A");

        Assert.That(
            GetDisplayedOptionNames(component),
            Is.EqualTo(new[] { "Option A" }));

        await component.Find("input").InputAsync(string.Empty);

        Assert.That(
            GetDisplayedOptionNames(component),
            Is.EquivalentTo(new[] { "Option A", "Option B", "Option C" }));
    }

    /// <summary>
    /// Selecting and removing options updates the selected items.
    /// </summary>
    [Test]
    public void MultiLookupField_SelectingAndRemovingOptions_UpdatesSelection()
    {
        using var context = CreateContext();
        var model = new TestModel();
        var component = RenderMultiLookupField(context, model, true);

        component.Find("input").Focus();

        var option = component.FindAll(".multi-lookup-field__dropdown-item")
            .Single(item => item.TextContent.Contains("Option A", StringComparison.Ordinal));

        option.MouseDown();

        Assert.That(model.SelectedOptions, Contains.Item(AvailableOptions[0]));

        component.WaitForAssertion(() =>
            Assert.That(component.FindAll(".multi-lookup-field__tag"), Has.Count.EqualTo(1)));

        component.Find(".multi-lookup-field__tag-remove").Click();

        Assert.That(model.SelectedOptions, Is.Empty);
    }

    /// <summary>
    /// Typeahead displays suggestions when focused if discovery is enabled.
    /// </summary>
    [Test]
    public void Typeahead_ShowDropDownOnFocus_ShowsSuggestions()
    {
        using var context = CreateContext();
        var searchCount = 0;

        var component = context.Render<Typeahead<TestOption, TestOption>>(parameters => parameters
            .Add(field => field.SearchMethod, (_, _) =>
            {
                searchCount++;
                return Task.FromResult<IEnumerable<TestOption>>(AvailableOptions);
            })
            .Add(field => field.ShowDropDownOnFocus, true)
            .Add(field => field.TextSelector, option => option.Name));

        component.Find("input").Focus();

        component.WaitForAssertion(() =>
        {
            Assert.That(searchCount, Is.EqualTo(1));
            Assert.That(component.FindAll(".typeahead-result"), Has.Count.EqualTo(3));
        });
    }

    /// <summary>
    /// Typeahead retains its original behavior when focus-based discovery is disabled.
    /// </summary>
    [Test]
    public void Typeahead_WithoutShowDropDownOnFocus_DoesNotLoadSuggestions()
    {
        using var context = CreateContext();
        var searchCount = 0;

        var component = context.Render<Typeahead<TestOption, TestOption>>(parameters => parameters
            .Add(field => field.SearchMethod, (_, _) =>
            {
                searchCount++;
                return Task.FromResult<IEnumerable<TestOption>>(AvailableOptions);
            })
            .Add(field => field.TextSelector, option => option.Name));

        component.Find("input").Focus();

        Assert.That(searchCount, Is.Zero);
        Assert.That(component.FindAll(".typeahead-results"), Is.Empty);
    }

    /// <summary>
    /// Typeahead clears the existing search text when ClearSearchOnFocus is enabled.
    /// </summary>
    [Test]
    public void Typeahead_ClearSearchOnFocus_SearchesWithoutPreviousValue()
    {
        using var context = CreateContext();
        var selectedOption = AvailableOptions[0];
        string? searchedText = null;

        var component = context.Render<Typeahead<TestOption, TestOption>>(parameters => parameters
            .Add(field => field.Value, selectedOption)
            .Add(field => field.SearchMethod, (text, _) =>
            {
                searchedText = text;
                return Task.FromResult<IEnumerable<TestOption>>(AvailableOptions);
            })
            .Add(field => field.ShowDropDownOnFocus, true)
            .Add(field => field.ClearSearchOnFocus, true)
            .Add(field => field.TextSelector, option => option.Name));

        component.Find(".typeahead-selected-value").Click();

        component.WaitForAssertion(() =>
        {
            Assert.That(searchedText, Is.Empty);
            Assert.That(component.FindAll(".typeahead-result"), Has.Count.EqualTo(3));
        });
    }
    
    /// <summary>
    /// An older focus lookup response must not replace results from a newer filtered search.
    /// </summary>
    [Test]
    public async Task MultiLookupField_IgnoresStaleSearchResponse_WhenRequestsCompleteOutOfOrder()
    {
        using var context = CreateContext();
        var controller = new ControlledLookupController();
        var component = RenderMultiLookupField(
            context,
            new TestModel(),
            true,
            controller);

        var focusTask = component.Find("input")
            .TriggerEventAsync("onfocus", new FocusEventArgs());

        await controller.WaitForSearchAsync(string.Empty);

        var filteredTask = component.Find("input").InputAsync("Option B");

        await controller.WaitForSearchAsync("Option B");

        controller.CompleteSearch("Option B", [AvailableOptions[1]]);
        await filteredTask;

        Assert.That(
            GetDisplayedOptionNames(component),
            Is.EqualTo(new[] { "Option B" }));

        controller.CompleteSearch(string.Empty, AvailableOptions);
        await focusTask;

        Assert.That(
            GetDisplayedOptionNames(component),
            Is.EqualTo(new[] { "Option B" }));
    }

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddSingleton<IChangeNotificationService, ChangeNotificationService>();
        context.Services.AddSingleton<ILookupController>(
            new TestLookupController(AvailableOptions));
        context.Services.AddLogging();
        context.Services.AddLocalization();
        return context;
    }

    private static IRenderedComponent<CascadingValue<EditContext>> RenderMultiLookupField(
        BunitContext context,
        TestModel model,
        bool showAllOnFocus,
        ILookupController? controller = null)
    {
        controller ??= new TestLookupController(AvailableOptions);

        return context.Render<CascadingValue<EditContext>>(parameters => parameters
            .Add(component => component.Value, new EditContext(model))
            .Add(component => component.ChildContent, builder =>
            {
                builder.OpenComponent<MultiLookupField<TestOption>>(0);
                builder.AddAttribute(1, nameof(MultiLookupField<TestOption>.Value), model.SelectedOptions);
                builder.AddAttribute(2, nameof(MultiLookupField<TestOption>.ValueExpression),
                    (System.Linq.Expressions.Expression<Func<IList<TestOption>>>)(() => model.SelectedOptions));
                builder.AddAttribute(3, nameof(MultiLookupField<TestOption>.ShowAllOnFocus), showAllOnFocus);
                builder.AddAttribute(4, nameof(MultiLookupField<TestOption>.CaptionFactory),
                    (Func<TestOption, string>)(option => option.Name));
                builder.AddAttribute(5, nameof(MultiLookupField<TestOption>.ExplicitLookupController), controller);
                builder.CloseComponent();
            }));
    }

    private static string[] GetDisplayedOptionNames(
        IRenderedComponent<CascadingValue<EditContext>> component)
    {
        return component.FindAll(".multi-lookup-field__dropdown-item span")
            .Select(element => element.TextContent.Trim())
            .ToArray();
    }

    private sealed class TestModel
    {
        public IList<TestOption> SelectedOptions { get; } = new List<TestOption>();
    }

    private sealed class TestOption(string name)
    {
        public string Name { get; } = name;

        public override string ToString() => this.Name;
    }

    
    /// <inheritdoc />
    private sealed class TestLookupController(IEnumerable<TestOption> options) : ILookupController
    {
        public int SearchCount { get; private set; }
        
        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Major Code Smell",
            "S1172:Unused method parameters should be removed",
            Justification = "Required by ILookupController; this test implementation does not use a persistence context.")]
        public Task<IEnumerable<T>> GetSuggestionsAsync<T>(string text, IContext? persistenceContext)
            where T : class
        {
            this.SearchCount++;

            var results = options.Where(option =>
                string.IsNullOrWhiteSpace(text)
                || option.Name.Contains(text, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(results.OfType<T>().AsEnumerable());
        }
    }
    
    private sealed class ControlledLookupController : ILookupController
    {
        private readonly Dictionary<string, TaskCompletionSource<IEnumerable<TestOption>>> _responses =
            new(StringComparer.Ordinal)
            {
                [string.Empty] = new(TaskCreationOptions.RunContinuationsAsynchronously),
                ["Option B"] = new(TaskCreationOptions.RunContinuationsAsynchronously),
            };

        private readonly Dictionary<string, TaskCompletionSource<bool>> _started =
            new(StringComparer.Ordinal)
            {
                [string.Empty] = new(TaskCreationOptions.RunContinuationsAsynchronously),
                ["Option B"] = new(TaskCreationOptions.RunContinuationsAsynchronously),
            };

        public Task WaitForSearchAsync(string text) => this._started[text].Task;

        public void CompleteSearch(string text, IEnumerable<TestOption> results)
        {
            this._responses[text].SetResult(results);
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Major Code Smell",
            "S1172:Unused method parameters should be removed",
            Justification = "Required by ILookupController; this test implementation does not use a persistence context.")]
        public async Task<IEnumerable<T>> GetSuggestionsAsync<T>(
            string text,
            IContext? persistenceContext)
            where T : class
        {
            this._started[text].TrySetResult(true);

            var results = await this._responses[text].Task.ConfigureAwait(false);
            return results.OfType<T>().ToArray();
        }
    }
}

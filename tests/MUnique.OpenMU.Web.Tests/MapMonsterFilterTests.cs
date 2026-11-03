// <copyright file="MapMonsterFilterTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests;

using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MUnique.OpenMU.Web.Map.Components;
using MUnique.OpenMU.Web.Map.Map;

/// <summary>
/// Tests for the <see cref="MapMonsterFilter"/>, which filters the monsters of the live map by name.
/// </summary>
[TestFixture]
public class MapMonsterFilterTests
{
    private const int ServerId = 2;
    private const string SetFilterFunction = "SetMapMonsterFilter";
    private const string GetMatchesFunction = "GetMapMonsterFilterMatches";

    private static readonly Guid MapId = new("6dbd2e42-1d6b-4c43-9b32-0d1b8d3f1b2a");

    private static readonly MonsterFilterMatch[] Matches =
    [
        new(1108, "Red Dragon", 47, 150),
        new(1120, "Golden Dragon", 201, 33),
    ];

    private BunitContext _context = null!;

    /// <summary>
    /// Sets up the context with the map scripts, which return <see cref="Matches"/>.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._context = new BunitContext();
        this._context.JSInterop.Mode = JSRuntimeMode.Strict;
        this._context.JSInterop.SetupVoid(SetFilterFunction, _ => true).SetVoidResult();
        this._context.JSInterop.Setup<MonsterFilterMatch[]>(GetMatchesFunction, _ => true).SetResult(Matches);
        this._context.Services.AddSingleton(Mock.Of<ILogger<MapMonsterFilter>>());
    }

    /// <summary>
    /// Disposes the context.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        this._context.Dispose();
    }

    /// <summary>
    /// Tests that typing sets the filter of the map with the default mode, "contains", and lists the
    /// matching monsters with their position.
    /// </summary>
    [Test]
    public void TypingFiltersAndListsTheMatches()
    {
        var filter = this.RenderFilter();

        filter.Find("input.monster-filter-text").Input("dragon");

        this.AssertLastFilter(MonsterFilterMode.Contains, "dragon");
        var rows = filter.FindAll("tr.monster-filter-match");
        var cells = rows.Select(row => string.Join(" | ", row.QuerySelectorAll("td").Select(cell => cell.TextContent.Trim())));
        Assert.That(cells, Is.EqualTo(new[] { "Red Dragon | (47, 150)", "Golden Dragon | (201, 33)" }));
    }

    /// <summary>
    /// Tests that the chosen mode is passed to the map.
    /// </summary>
    /// <param name="mode">The mode.</param>
    [TestCase(MonsterFilterMode.StartsWith)]
    [TestCase(MonsterFilterMode.EndsWith)]
    public void TheModeIsPassedToTheMap(MonsterFilterMode mode)
    {
        var filter = this.RenderFilter();

        filter.Find("input.monster-filter-text").Input("gold");
        filter.Find("select.monster-filter-mode").Change(((int)mode).ToString(CultureInfo.InvariantCulture));

        this.AssertLastFilter(mode, "gold");
    }

    /// <summary>
    /// Tests that clicking a matching monster selects it on the map.
    /// </summary>
    [Test]
    public void ClickingAMatchSelectsTheMonster()
    {
        int? selectedId = null;
        var filter = this.RenderFilter(EventCallback.Factory.Create<int>(this, id => selectedId = id));

        filter.Find("input.monster-filter-text").Input("dragon");
        filter.FindAll("tr.monster-filter-match")[1].Click();

        Assert.That(selectedId, Is.EqualTo(1120));
    }

    /// <summary>
    /// Tests that clearing the filter shows all monsters again and empties the list.
    /// </summary>
    [Test]
    public void ClearingShowsAllMonstersAgain()
    {
        var filter = this.RenderFilter();
        filter.Find("input.monster-filter-text").Input("dragon");

        filter.Find("button.monster-filter-clear").Click();

        this.AssertLastFilter(MonsterFilterMode.Contains, string.Empty);
        Assert.That(filter.FindAll("tr.monster-filter-match"), Is.Empty);
        Assert.That(filter.Find("input.monster-filter-text").GetAttribute("value"), Is.Empty);
    }

    private IRenderedComponent<MapMonsterFilter> RenderFilter(EventCallback<int> onSelectObject = default)
    {
        return this._context.Render<MapMonsterFilter>(p => p
            .Add(f => f.ServerId, ServerId)
            .Add(f => f.MapId, MapId)
            .Add(f => f.OnSelectObject, onSelectObject));
    }

    private void AssertLastFilter(MonsterFilterMode mode, string text)
    {
        var invocation = this._context.JSInterop.Invocations[SetFilterFunction].Last();
        Assert.That(invocation.Arguments, Is.EqualTo(new object?[] { ServerId, MapId, (int)mode, text }));
    }
}

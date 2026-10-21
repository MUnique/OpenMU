// <copyright file="DataTableTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Components;

using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Web.Shared.Components.Form;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Tests for the <see cref="DataTable{TItem}"/> component.
/// </summary>
[TestFixture]
public class DataTableTests
{
    /// <summary>
    /// A refresh of a page which doesn't exist anymore, e.g. after a filter reduced the results,
    /// starts over from the first page instead of showing an empty table.
    /// </summary>
    [Test]
    public void RefreshOfEmptyPage_StartsOverFromFirstPage()
    {
        using var context = new BunitContext();
        var dataService = new TestDataService(Enumerable.Range(1, 45).Select(i => new Item(i)));
        context.Services.AddSingleton<IDataService<Item>>(dataService);
        var component = context.Render<DataTable<Item>>(parameters => parameters
            .Add(table => table.TableHeader, (RenderFragment)(_ => { }))
            .Add(table => table.ItemTemplate, item => builder => builder.AddContent(0, item.Number)));

        // The buttons are: previous page, page number, next page.
        component.FindAll("button")[2].Click();
        component.FindAll("button")[2].Click();
        component.WaitForAssertion(() => Assert.That(component.Instance.Page, Is.EqualTo(3)));

        dataService.SetItems(Enumerable.Range(1, 3).Select(i => new Item(i)));

        component.WaitForAssertion(() =>
        {
            Assert.That(component.Instance.Page, Is.EqualTo(1));
            Assert.That(component.FindAll("tbody tr"), Has.Count.EqualTo(3));
        });
    }

    /// <summary>
    /// An item of the table.
    /// </summary>
    /// <param name="Number">The number of the item.</param>
    public sealed record Item(int Number);

    private sealed class TestDataService : IDataService<Item>, ISupportDataChangedNotification
    {
        private List<Item> _items;

        public TestDataService(IEnumerable<Item> items)
        {
            this._items = items.ToList();
        }

        public event EventHandler? DataChanged;

        public Task<List<Item>> GetAsync(int offset, int count)
        {
            return Task.FromResult(this._items.Skip(offset).Take(count).ToList());
        }

        public void SetItems(IEnumerable<Item> items)
        {
            this._items = items.ToList();
            this.DataChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

// <copyright file="PageWindowTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Components;

using MUnique.OpenMU.Web.Shared.Components;

/// <summary>
/// Tests for the <see cref="PageWindow"/>.
/// </summary>
[TestFixture]
public class PageWindowTests
{
    /// <summary>
    /// If all pages fit into the slots, all of them are shown.
    /// </summary>
    /// <param name="totalPages">The total number of pages.</param>
    [TestCase(1)]
    [TestCase(3)]
    [TestCase(7)]
    public void FewPages_ShowsAllPages(int totalPages)
    {
        var expected = Enumerable.Range(1, totalPages).Select(p => (int?)p).ToArray();
        for (var current = 1; current <= totalPages; current++)
        {
            Assert.That(PageWindow.Create(current, totalPages, false), Is.EqualTo(expected));
        }
    }

    /// <summary>
    /// The layout depends on the position of the current page.
    /// </summary>
    /// <param name="current">The current page.</param>
    /// <param name="expected">The expected entries, where 0 is a gap.</param>
    [TestCase(1, new[] { 1, 2, 3, 4, 5, 0, 12 })]
    [TestCase(4, new[] { 1, 2, 3, 4, 5, 0, 12 })]
    [TestCase(5, new[] { 1, 0, 4, 5, 6, 0, 12 })]
    [TestCase(8, new[] { 1, 0, 7, 8, 9, 0, 12 })]
    [TestCase(9, new[] { 1, 0, 8, 9, 10, 11, 12 })]
    [TestCase(12, new[] { 1, 0, 8, 9, 10, 11, 12 })]
    public void ManyPages_UsesLayoutOfCurrentPosition(int current, int[] expected)
    {
        var result = PageWindow.Create(current, 12, false);
        Assert.That(result.Select(p => p ?? 0), Is.EqualTo(expected));
    }

    /// <summary>
    /// The first and the last page are always visible and the amount of entries never changes, wherever the current page is.
    /// </summary>
    [Test]
    public void ManyPages_KeepsEntryCountAndShowsFirstAndLastPage()
    {
        for (var current = 1; current <= 50; current++)
        {
            var result = PageWindow.Create(current, 50, false);
            Assert.That(result, Has.Count.EqualTo(PageWindow.DefaultSlotCount), $"page {current}");
            Assert.That(result[0], Is.EqualTo(1), $"page {current}");
            Assert.That(result[^1], Is.EqualTo(50), $"page {current}");
            Assert.That(result, Does.Contain(current), $"page {current}");
        }
    }

    /// <summary>
    /// Without a known total, the pages are shown up to the last known one, and the amount of entries stops growing at the slot count.
    /// </summary>
    [Test]
    public void UnknownTotal_ShowsKnownPagesWithConstantEntryCount()
    {
        Assert.That(PageWindow.Create(1, null, true), Is.EqualTo(new int?[] { 1, 2 }));
        Assert.That(PageWindow.Create(3, null, false), Is.EqualTo(new int?[] { 1, 2, 3 }));
        Assert.That(PageWindow.Create(6, null, true), Is.EqualTo(new int?[] { 1, 2, 3, 4, 5, 6, 7 }));
        Assert.That(PageWindow.Create(7, null, true), Is.EqualTo(new int?[] { 1, null, 4, 5, 6, 7, 8 }));
        Assert.That(PageWindow.Create(20, null, false), Is.EqualTo(new int?[] { 1, null, 16, 17, 18, 19, 20 }));
    }

    /// <summary>
    /// Fewer than 5 slots can't show the first page, a gap, the current page and its neighbors.
    /// </summary>
    [Test]
    public void TooFewSlots_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PageWindow.Create(1, 10, false, 4));
    }
}

// <copyright file="KanturuBarrierAreaHelperTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// Tests for <see cref="KanturuBarrierAreaHelper"/>.
/// </summary>
[TestFixture]
public class KanturuBarrierAreaHelperTests
{
    /// <summary>
    /// Tests that a single cell area returns exactly that cell.
    /// </summary>
    [Test]
    public void EnumerateCells_SingleCell_ReturnsIt()
    {
        var areas = new List<KanturuTerrainArea> { new() { StartX = 73, StartY = 144, EndX = 73, EndY = 144 } };

        var cells = KanturuBarrierAreaHelper.EnumerateCells(areas).ToList();

        Assert.That(cells, Has.Count.EqualTo(1));
        Assert.That(cells[0], Is.EqualTo(((byte)73, (byte)144)));
    }

    /// <summary>
    /// Tests that a rectangle is enumerated inclusively.
    /// </summary>
    [Test]
    public void EnumerateCells_Rectangle_IsInclusive()
    {
        var areas = new List<KanturuTerrainArea> { new() { StartX = 0, StartY = 0, EndX = 1, EndY = 2 } };

        var cells = KanturuBarrierAreaHelper.EnumerateCells(areas).ToList();

        Assert.That(cells, Has.Count.EqualTo(6));
    }

    /// <summary>
    /// Tests that the default barrier area contains the expected cell count.
    /// </summary>
    [Test]
    public void EnumerateCells_DefaultBarrierArea_MatchesExpectedCount()
    {
        // X=73-90 (18), Y=144-195 (52) => 936 cells.
        var areas = new List<KanturuTerrainArea> { new() { StartX = 73, StartY = 144, EndX = 90, EndY = 195 } };

        Assert.That(KanturuBarrierAreaHelper.EnumerateCells(areas).Count(), Is.EqualTo(18 * 52));
    }

    /// <summary>
    /// Tests that no areas return no cells.
    /// </summary>
    [Test]
    public void EnumerateCells_Empty_ReturnsNone()
    {
        Assert.That(KanturuBarrierAreaHelper.EnumerateCells(new List<KanturuTerrainArea>()), Is.Empty);
    }

    /// <summary>
    /// Tests that an area touching the map border terminates instead of wrapping around.
    /// </summary>
    [Test, Timeout(5000)]
    public void EnumerateCells_MaxBoundary_Terminates()
    {
        var areas = new List<KanturuTerrainArea> { new() { StartX = 254, StartY = 254, EndX = 255, EndY = 255 } };

        var cells = KanturuBarrierAreaHelper.EnumerateCells(areas).ToList();

        Assert.That(cells, Has.Count.EqualTo(4));
    }
}

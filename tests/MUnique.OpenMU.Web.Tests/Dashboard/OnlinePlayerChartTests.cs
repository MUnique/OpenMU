// <copyright file="OnlinePlayerChartTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Dashboard;

using System.Globalization;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Tests for the <see cref="OnlinePlayerChart"/>.
/// </summary>
[TestFixture]
public class OnlinePlayerChartTests
{
    private static readonly DateTime Now = new(2026, 10, 18, 12, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// A line needs at least two samples.
    /// </summary>
    [Test]
    public void NoChartWithSingleSample()
    {
        Assert.That(OnlinePlayerChart.Create([new OnlinePlayerSample(Now, 5)], Now), Is.Null);
    }

    /// <summary>
    /// The current, peak and low values are taken from the samples.
    /// </summary>
    [Test]
    public void ValuesAreTakenFromSamples()
    {
        var chart = OnlinePlayerChart.Create(
            [
                new OnlinePlayerSample(Now.AddHours(-3), 20),
                new OnlinePlayerSample(Now.AddHours(-2), 80),
                new OnlinePlayerSample(Now.AddHours(-1), 5),
                new OnlinePlayerSample(Now, 30),
            ],
            Now);

        Assert.That(chart, Is.Not.Null);
        Assert.That((chart!.Current, chart.Peak, chart.Low), Is.EqualTo((30, 80, 5)));
    }

    /// <summary>
    /// The points are placed by their time within the window, and are inside the coordinate system.
    /// Samples which are older than the window are not drawn.
    /// </summary>
    [Test]
    public void PointsAreInsideOfCoordinateSystem()
    {
        var chart = OnlinePlayerChart.Create(
            [
                new OnlinePlayerSample(Now - OnlinePlayerHistory.Window - TimeSpan.FromHours(1), 1000),
                new OnlinePlayerSample(Now - (OnlinePlayerHistory.Window / 2), 0),
                new OnlinePlayerSample(Now, 50),
            ],
            Now);

        var points = chart!.LinePoints.Split(' ')
            .Select(p => p.Split(','))
            .Select(p => (X: double.Parse(p[0], CultureInfo.InvariantCulture), Y: double.Parse(p[1], CultureInfo.InvariantCulture)))
            .ToList();
        Assert.Multiple(() =>
        {
            Assert.That(chart.Peak, Is.EqualTo(50));
            Assert.That(points, Has.Count.EqualTo(2));
            Assert.That(points[0], Is.EqualTo((OnlinePlayerChart.Width / 2.0, (double)OnlinePlayerChart.Height)));
            Assert.That(points[1].X, Is.EqualTo(OnlinePlayerChart.Width));
            Assert.That(points[1].Y, Is.InRange(0, OnlinePlayerChart.Height));
        });
    }
}

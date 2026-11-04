// <copyright file="OnlinePlayerChart.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Globalization;
using System.Text;

/// <summary>
/// The data of the chart of the online players, which is drawn as SVG.
/// </summary>
/// <param name="LinePoints">The points of the line, in the format of the SVG points attribute.</param>
/// <param name="AreaPoints">The points of the area below the line, in the format of the SVG points attribute.</param>
/// <param name="Current">The current number of online players, i.e. of the latest sample.</param>
/// <param name="Peak">The highest number of online players.</param>
/// <param name="Low">The lowest number of online players.</param>
public sealed record OnlinePlayerChart(string LinePoints, string AreaPoints, int Current, int Peak, int Low)
{
    /// <summary>
    /// The width of the coordinate system of the chart.
    /// </summary>
    public const int Width = 600;

    /// <summary>
    /// The height of the coordinate system of the chart.
    /// </summary>
    public const int Height = 160;

    /// <summary>
    /// The minimum value of the y-axis, so that small numbers are not drawn as huge spikes.
    /// </summary>
    private const int MinimumScale = 10;

    /// <summary>
    /// Creates the chart of the samples of the <see cref="OnlinePlayerHistory.Window"/> which ends at the specified time.
    /// </summary>
    /// <param name="samples">The samples, ordered by their timestamp.</param>
    /// <param name="now">The end of the time window (UTC).</param>
    /// <returns>The chart; or <c>null</c>, if there are not enough samples to draw a line.</returns>
    public static OnlinePlayerChart? Create(IReadOnlyList<OnlinePlayerSample> samples, DateTime now)
    {
        var windowStart = now - OnlinePlayerHistory.Window;
        var visibleSamples = samples.Where(s => s.Timestamp >= windowStart).ToList();
        if (visibleSamples.Count < 2)
        {
            return null;
        }

        var peak = visibleSamples.Max(s => s.PlayerCount);
        var low = visibleSamples.Min(s => s.PlayerCount);

        // Leave some space above the peak, so the line doesn't touch the top border.
        var scale = Math.Max(peak, MinimumScale) * 1.1;
        var line = new StringBuilder();
        foreach (var sample in visibleSamples)
        {
            var x = (sample.Timestamp - windowStart) / OnlinePlayerHistory.Window * Width;
            var y = Height - (sample.PlayerCount / scale * Height);
            AppendPoint(line, x, y);
        }

        var firstX = (visibleSamples[0].Timestamp - windowStart) / OnlinePlayerHistory.Window * Width;
        var lastX = (visibleSamples[^1].Timestamp - windowStart) / OnlinePlayerHistory.Window * Width;
        var area = new StringBuilder();
        AppendPoint(area, firstX, Height);
        area.Append(line);
        AppendPoint(area, lastX, Height);

        return new OnlinePlayerChart(line.ToString().TrimEnd(), area.ToString().TrimEnd(), visibleSamples[^1].PlayerCount, peak, low);
    }

    private static void AppendPoint(StringBuilder builder, double x, double y)
    {
        builder.Append(x.ToString("0.#", CultureInfo.InvariantCulture))
            .Append(',')
            .Append(y.ToString("0.#", CultureInfo.InvariantCulture))
            .Append(' ');
    }
}

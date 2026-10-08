// <copyright file="IntervalAsTimeSpanJsonConverterTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Text.Json;
using MUnique.OpenMU.Persistence.EntityFramework.Json;

/// <summary>
/// Tests for the <see cref="IntervalAsTimeSpanJsonConverter"/>.
/// The expected values are the seconds which postgres extracts from the same intervals.
/// </summary>
[TestFixture]
public class IntervalAsTimeSpanJsonConverterTests
{
    /// <summary>
    /// Tests that an interval in the json output of postgres is read as <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="interval">The interval, as postgres provides it in json.</param>
    /// <param name="expectedSeconds">The expected seconds.</param>
    [TestCase("00:00:00", 0)]
    [TestCase("00:00:30", 30)]
    [TestCase("1 day", 86400)]
    [TestCase("30 days", 2592000)]
    [TestCase("1 day 02:03:04.5", 93784.5)]
    [TestCase("-1 days", -86400)]
    [TestCase("1 day -02:00:00", 79200)]
    [TestCase("36:00:00", 129600)]
    [TestCase("-02:00:00", -7200)]
    [TestCase("1 year 2 mons", 36741600)]
    public void IntervalIsRead(string interval, double expectedSeconds)
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new IntervalAsTimeSpanJsonConverter());

        var result = JsonSerializer.Deserialize<TimeSpan>($"\"{interval}\"", options);

        Assert.That(result, Is.EqualTo(TimeSpan.FromSeconds(expectedSeconds)));
    }

    /// <summary>
    /// Tests that a nullable <see cref="TimeSpan"/> is read, too.
    /// </summary>
    [Test]
    public void NullableIntervalIsRead()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new IntervalAsTimeSpanJsonConverter());

        Assert.That(JsonSerializer.Deserialize<TimeSpan?>("\"7 days\"", options), Is.EqualTo(TimeSpan.FromDays(7)));
        Assert.That(JsonSerializer.Deserialize<TimeSpan?>("null", options), Is.Null);
    }

    /// <summary>
    /// Tests that an interval in an unknown format is reported as <see cref="JsonException"/>.
    /// </summary>
    [Test]
    public void UnknownFormatThrowsJsonException()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new IntervalAsTimeSpanJsonConverter());

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TimeSpan>("\"3 fortnights\"", options));
    }
}

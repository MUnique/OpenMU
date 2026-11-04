// <copyright file="IntervalAsTimeSpanJsonConverter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Json;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// This converter converts a postgres interval to a <see cref="TimeSpan"/>.
/// Postgres provides intervals of at least one day in its own format, e.g. "7 days" or "1 day 02:00:00",
/// which the default converter can't parse.
/// </summary>
public class IntervalAsTimeSpanJsonConverter : JsonConverter<TimeSpan>
{
    /// <summary>
    /// The number of days postgres counts for a year, e.g. when it extracts the epoch of an interval.
    /// </summary>
    private const double DaysPerYear = 365.25;

    /// <summary>
    /// The number of days postgres counts for a month.
    /// </summary>
    private const int DaysPerMonth = 30;

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("c", CultureInfo.InvariantCulture));
    }

    /// <inheritdoc />
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token parsing an interval. Expected String, got {reader.TokenType}.");
        }

        var text = reader.GetString()!;
        if (TimeSpan.TryParseExact(text, "c", CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        return ParseInterval(text);
    }

    /// <summary>
    /// Parses an interval in the postgres output format, e.g. "1 year 2 mons -3 days +04:05:06.5".
    /// </summary>
    /// <param name="text">The interval text.</param>
    /// <returns>The parsed interval.</returns>
    private static TimeSpan ParseInterval(string text)
    {
        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var result = TimeSpan.Zero;
        var index = 0;
        while (index < parts.Length)
        {
            var part = parts[index];
            if (part.Contains(':'))
            {
                result += ParseTime(part, text);
                index++;
                continue;
            }

            if (index + 1 >= parts.Length || !int.TryParse(part, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var amount))
            {
                throw new JsonException($"The interval '{text}' has an unsupported format.");
            }

            result += parts[index + 1] switch
            {
                "day" or "days" => TimeSpan.FromDays(amount),
                "mon" or "mons" => TimeSpan.FromDays(amount * DaysPerMonth),
                "year" or "years" => TimeSpan.FromDays(amount * DaysPerYear),
                _ => throw new JsonException($"The interval '{text}' has an unsupported unit '{parts[index + 1]}'."),
            };
            index += 2;
        }

        return result;
    }

    private static TimeSpan ParseTime(string time, string text)
    {
        var isNegative = time.StartsWith('-');
        var segments = time.TrimStart('+', '-').Split(':');
        if (segments.Length != 3
            || !int.TryParse(segments[0], NumberStyles.None, CultureInfo.InvariantCulture, out var hours)
            || !int.TryParse(segments[1], NumberStyles.None, CultureInfo.InvariantCulture, out var minutes)
            || !decimal.TryParse(segments[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var seconds))
        {
            throw new JsonException($"The interval '{text}' has an unsupported time '{time}'.");
        }

        var result = TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes) + TimeSpan.FromTicks((long)(seconds * TimeSpan.TicksPerSecond));
        return isNegative ? result.Negate() : result;
    }
}

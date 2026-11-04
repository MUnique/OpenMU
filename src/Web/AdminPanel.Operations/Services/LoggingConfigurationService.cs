// <copyright file="LoggingConfigurationService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using Nito.AsyncEx;

/// <summary>
/// Reads and writes the logging configuration of the server process, which is the
/// <c>Serilog</c> section of its <c>appsettings.json</c>.
/// </summary>
/// <remarks>
/// The configuration file is watched by the server. When it's saved, Serilog applies the changed
/// minimum levels at once. Everything else, including added or removed overrides, takes effect
/// after a restart of the server.
/// </remarks>
public class LoggingConfigurationService
{
    /// <summary>
    /// The level which is used when nothing else is configured.
    /// </summary>
    public static readonly string DefaultLevel = "Information";

    /// <summary>
    /// The name of the section which holds the logging configuration.
    /// </summary>
    public static readonly string SectionName = "Serilog";

    private const string MinimumLevelKey = "MinimumLevel";
    private const string DefaultKey = "Default";
    private const string OverrideKey = "Override";

    private static readonly JsonDocumentOptions DocumentOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
    };

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,

        // The file is edited by humans, too - so we don't escape characters like '+' or '<'.
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private readonly AsyncLock _lock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingConfigurationService"/> class.
    /// </summary>
    /// <param name="filePath">The path of the configuration file.</param>
    public LoggingConfigurationService(string filePath)
    {
        this.FilePath = filePath;
    }

    /// <summary>
    /// Gets the available log levels of Serilog, ordered from the most to the least detailed one.
    /// </summary>
    public static IReadOnlyList<string> Levels { get; } = ["Verbose", "Debug", "Information", "Warning", "Error", "Fatal"];

    /// <summary>
    /// Gets the path of the configuration file.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Loads the logging settings from the configuration file.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The loaded settings.</returns>
    /// <exception cref="IOException">When the file couldn't be read.</exception>
    /// <exception cref="JsonException">When the file doesn't contain a valid JSON object.</exception>
    public async ValueTask<LoggingSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        using var l = await this._lock.LockAsync(cancellationToken).ConfigureAwait(false);
        var root = await this.ReadRootAsync(cancellationToken).ConfigureAwait(false);
        var section = root[SectionName] as JsonObject ?? new JsonObject();

        var settings = new LoggingSettings
        {
            SectionJson = section.ToJsonString(SerializerOptions),
        };

        switch (section[MinimumLevelKey])
        {
            case JsonValue value:
                settings.DefaultLevel = NormalizeLevel(value.ToString());
                break;
            case JsonObject minimumLevel:
                settings.DefaultLevel = NormalizeLevel(minimumLevel[DefaultKey]?.ToString());
                if (minimumLevel[OverrideKey] is JsonObject overrides)
                {
                    settings.Overrides.AddRange(overrides.Select(o => new LogLevelOverride
                    {
                        Source = o.Key,
                        Level = NormalizeLevel(o.Value?.ToString()),
                    }));
                }

                break;
            default:
                // Nothing configured, so Serilog uses its default.
                break;
        }

        return settings;
    }

    /// <summary>
    /// Saves the minimum levels into the configuration file, and keeps the rest of it unchanged.
    /// </summary>
    /// <param name="defaultLevel">The default minimum level.</param>
    /// <param name="overrides">The minimum levels of specific sources.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentException">When a level is unknown, or a source is empty or defined twice.</exception>
    /// <exception cref="IOException">When the file couldn't be read or written.</exception>
    /// <exception cref="JsonException">When the file doesn't contain a valid JSON object.</exception>
    public async ValueTask SaveMinimumLevelsAsync(string defaultLevel, IReadOnlyCollection<LogLevelOverride> overrides, CancellationToken cancellationToken = default)
    {
        if (!Levels.Contains(defaultLevel))
        {
            throw new ArgumentException($"Unknown log level '{defaultLevel}'.", nameof(defaultLevel));
        }

        if (overrides.FirstOrDefault(o => string.IsNullOrWhiteSpace(o.Source) || !Levels.Contains(o.Level)) is { } invalid)
        {
            throw new ArgumentException($"Invalid override '{invalid.Source}' with level '{invalid.Level}'.", nameof(overrides));
        }

        if (overrides.GroupBy(o => o.Source.Trim()).FirstOrDefault(g => g.Count() > 1) is { } duplicate)
        {
            throw new ArgumentException($"The source '{duplicate.Key}' is defined more than once.", nameof(overrides));
        }

        using var l = await this._lock.LockAsync(cancellationToken).ConfigureAwait(false);
        var root = await this.ReadRootAsync(cancellationToken).ConfigureAwait(false);
        if (root[SectionName] is not JsonObject section)
        {
            section = new JsonObject();
            root[SectionName] = section;
        }

        if (section[MinimumLevelKey] is JsonValue && overrides.Count == 0)
        {
            // Serilog watches the value at the place where it was at the start,
            // so we keep the short form when it's possible.
            section[MinimumLevelKey] = defaultLevel;
        }
        else
        {
            if (section[MinimumLevelKey] is not JsonObject minimumLevel)
            {
                minimumLevel = new JsonObject();
                section[MinimumLevelKey] = minimumLevel;
            }

            minimumLevel[DefaultKey] = defaultLevel;
            var overridesObject = new JsonObject();
            foreach (var levelOverride in overrides)
            {
                overridesObject[levelOverride.Source.Trim()] = levelOverride.Level;
            }

            minimumLevel[OverrideKey] = overridesObject;
        }

        await this.WriteRootAsync(root, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Replaces the whole logging section of the configuration file, and keeps the rest of it unchanged.
    /// </summary>
    /// <param name="sectionJson">The JSON of the new section.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="JsonException">When <paramref name="sectionJson"/> or the file don't contain a valid JSON object.</exception>
    /// <exception cref="IOException">When the file couldn't be read or written.</exception>
    [MUnique.OpenMU.Web.AdminPanel.Operations.NotExposedToAgents("Full logging sink configuration can contain credentials; MCP exposes minimum levels only.")]
    public async ValueTask SaveSectionAsync(string sectionJson, CancellationToken cancellationToken = default)
    {
        var section = ParseObject(sectionJson);

        using var l = await this._lock.LockAsync(cancellationToken).ConfigureAwait(false);
        var root = await this.ReadRootAsync(cancellationToken).ConfigureAwait(false);
        root[SectionName] = section;
        await this.WriteRootAsync(root, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Changes only the default minimum level under the shared configuration lock.</summary>
    /// <param name="level">The validated Serilog level.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal async ValueTask SaveDefaultLevelAsync(string level, CancellationToken cancellationToken)
    {
        if (!Levels.Contains(level))
        {
            throw new ArgumentException("Unknown logging level.", nameof(level));
        }

        using var scope = await this._lock.LockAsync(cancellationToken).ConfigureAwait(false);
        var root = await this.ReadRootAsync(cancellationToken).ConfigureAwait(false);
        if (root[SectionName] is not JsonObject section)
        {
            section = new JsonObject();
            root[SectionName] = section;
        }

        if (section[MinimumLevelKey] is JsonObject minimum)
        {
            minimum[DefaultKey] = level;
        }
        else
        {
            section[MinimumLevelKey] = level;
        }

        await this.WriteRootAsync(root, cancellationToken).ConfigureAwait(false);
    }

    private static string NormalizeLevel(string? level)
    {
        return Levels.FirstOrDefault(l => string.Equals(l, level, StringComparison.OrdinalIgnoreCase)) ?? DefaultLevel;
    }

    private static JsonObject ParseObject(string json)
    {
        return JsonNode.Parse(json, documentOptions: DocumentOptions) as JsonObject
               ?? throw new JsonException("The JSON is not an object.");
    }

    private async ValueTask<JsonObject> ReadRootAsync(CancellationToken cancellationToken)
    {
        var json = await File.ReadAllTextAsync(this.FilePath, cancellationToken).ConfigureAwait(false);
        return ParseObject(json);
    }

    private async ValueTask WriteRootAsync(JsonObject root, CancellationToken cancellationToken)
    {
        // We overwrite the file in place instead of replacing it with a new one,
        // because it may be a single file which is mounted into a docker container.
        var json = root.ToJsonString(SerializerOptions) + Environment.NewLine;
        await File.WriteAllTextAsync(this.FilePath, json, cancellationToken).ConfigureAwait(false);
    }
}

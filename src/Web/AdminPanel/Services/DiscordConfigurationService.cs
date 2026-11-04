// <copyright file="DiscordConfigurationService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using MUnique.OpenMU.Discord;
using Nito.AsyncEx;

/// <summary>
/// Reads and writes the settings of the Discord integration, which are the
/// <see cref="DiscordSettings.SectionName"/> section of the <c>appsettings.json</c> of the server process.
/// </summary>
/// <remarks>
/// The configuration file is watched by the server, and the <see cref="DiscordIntegration"/> restarts
/// itself with the saved settings. Settings which are also defined by environment variables or other
/// configuration sources take their values from there; they can't be changed in the file.
/// </remarks>
public class DiscordConfigurationService
{
    private static readonly JsonDocumentOptions DocumentOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
    };

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

        // The computed properties, like DiscordBotSettings.IsEnabled, are not settings.
        IgnoreReadOnlyProperties = true,
        Converters = { new JsonStringEnumConverter() },

        // The file is edited by humans, too - so we don't escape characters like '+' or '<'.
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private readonly AsyncLock _lock = new();
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordConfigurationService"/> class.
    /// </summary>
    /// <param name="filePath">The path of the configuration file.</param>
    /// <param name="configuration">The configuration of the server process, which includes the file.</param>
    public DiscordConfigurationService(string filePath, IConfiguration configuration)
    {
        this.FilePath = filePath;
        this._configuration = configuration;
    }

    /// <summary>
    /// Gets the path of the configuration file.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Loads the settings which are stored in the configuration file.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The settings of the file.</returns>
    /// <exception cref="IOException">When the file couldn't be read.</exception>
    /// <exception cref="JsonException">When the file doesn't contain a valid JSON object.</exception>
    public async ValueTask<DiscordSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        using var l = await this._lock.LockAsync(cancellationToken).ConfigureAwait(false);
        var root = await this.ReadRootAsync(cancellationToken).ConfigureAwait(false);
        if (root[DiscordSettings.SectionName] is not JsonObject section)
        {
            return new DiscordSettings();
        }

        // The settings are bound like the server binds them, so that both understand the file in the same way.
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(new JsonObject { [DiscordSettings.SectionName] = section.DeepClone() }.ToJsonString()));
        return new ConfigurationBuilder()
                   .AddJsonStream(stream)
                   .Build()
                   .GetSection(DiscordSettings.SectionName)
                   .Get<DiscordSettings>()
               ?? new DiscordSettings();
    }

    /// <summary>
    /// Saves the settings into the configuration file, and keeps the rest of it unchanged.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="IOException">When the file couldn't be read or written.</exception>
    /// <exception cref="JsonException">When the file doesn't contain a valid JSON object.</exception>
    public async ValueTask SaveAsync(DiscordSettings settings, CancellationToken cancellationToken = default)
    {
        RemoveEmptyEntries(settings);
        using (await this._lock.LockAsync(cancellationToken).ConfigureAwait(false))
        {
            var root = await this.ReadRootAsync(cancellationToken).ConfigureAwait(false);
            var updated = JsonSerializer.SerializeToNode(settings, SerializerOptions)!.AsObject();
            this.CopyManagedValues(updated, root[DiscordSettings.SectionName] as JsonObject ?? new JsonObject());
            root[DiscordSettings.SectionName] = updated;

            // We overwrite the file in place instead of replacing it with a new one,
            // because it may be a single file which is mounted into a docker container.
            var json = root.ToJsonString(SerializerOptions) + Environment.NewLine;
            await File.WriteAllTextAsync(this.FilePath, json, cancellationToken).ConfigureAwait(false);
        }

        // The file is watched, too, but this way the changes take effect without its delay.
        (this._configuration as IConfigurationRoot)?.Reload();
    }

    /// <summary>
    /// Determines if a setting is defined by another source than the configuration file, e.g. an environment
    /// variable, which takes precedence over the file.
    /// </summary>
    /// <param name="key">The key of the setting, relative to the section, e.g. <c>Bot:Token</c>.</param>
    /// <returns><c>true</c>, if the setting is defined by another source.</returns>
    public bool IsDefinedElsewhere(string key)
    {
        if (this._configuration is not IConfigurationRoot root)
        {
            return false;
        }

        var fullKey = ConfigurationPath.Combine(DiscordSettings.SectionName, key);
        var fileName = Path.GetFileName(this.FilePath);
        foreach (var provider in root.Providers.Reverse())
        {
            if (provider.TryGet(fullKey, out _)
                || provider.TryGet(ConfigurationPath.Combine(fullKey, "0"), out _)
                || provider.GetChildKeys([], fullKey).Any())
            {
                return provider is not JsonConfigurationProvider { Source.Path: { } path }
                       || !string.Equals(Path.GetFileName(path), fileName, StringComparison.OrdinalIgnoreCase);
            }
        }

        return false;
    }

    /// <summary>
    /// Loads editable file settings with effective values for fields managed elsewhere.
    /// Saving restores those fields from the original file, so environment secrets are never copied into it.
    /// </summary>
    /// <returns>The settings as they should be displayed.</returns>
    public ValueTask<DiscordSettings> LoadForEditingAsync() => this.LoadForEditingAsync(CancellationToken.None);

    /// <summary>
    /// Loads editable settings using the supplied cancellation token.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The settings as they should be displayed.</returns>
    public async ValueTask<DiscordSettings> LoadForEditingAsync(CancellationToken cancellationToken)
    {
        var fileSettings = await this.LoadAsync(cancellationToken).ConfigureAwait(false);
        var effective = this._configuration.GetSection(DiscordSettings.SectionName).Get<DiscordSettings>() ?? new DiscordSettings();
        var editable = JsonSerializer.SerializeToNode(fileSettings, SerializerOptions)!.AsObject();
        this.CopyManagedValues(editable, JsonSerializer.SerializeToNode(effective, SerializerOptions)!.AsObject());
        return editable.Deserialize<DiscordSettings>(SerializerOptions)!;
    }

    /// <summary>
    /// Gets the configuration key an operator can use to change a managed field.
    /// </summary>
    /// <param name="key">The key relative to the Discord section.</param>
    /// <returns>The environment-variable form of the key.</returns>
    public string GetEnvironmentKey(string key) => ConfigurationPath.Combine(DiscordSettings.SectionName, key).Replace(":", "__", StringComparison.Ordinal);

    private static void RemoveEmptyEntries(DiscordSettings settings)
    {
        foreach (var category in settings.Webhooks.Where(entry => string.IsNullOrWhiteSpace(entry.Value)).Select(entry => entry.Key).ToList())
        {
            settings.Webhooks.Remove(category);
        }

        if (string.IsNullOrWhiteSpace(settings.Bot.Token))
        {
            settings.Bot.Token = null;
        }

        if (string.IsNullOrWhiteSpace(settings.Bot.LayoutFile))
        {
            settings.Bot.LayoutFile = null;
        }
    }

    private static JsonObject ParseObject(string json)
    {
        return JsonNode.Parse(json, documentOptions: DocumentOptions) as JsonObject
               ?? throw new JsonException("The JSON is not an object.");
    }

    private void CopyManagedValues(JsonObject target, JsonObject source, string prefix = "")
    {
        foreach (var name in target.Select(entry => entry.Key).Union(source.Select(entry => entry.Key)).ToList())
        {
            var key = string.IsNullOrEmpty(prefix) ? name : ConfigurationPath.Combine(prefix, name);
            if ((target[name] ?? source[name]) is JsonObject)
            {
                if (target[name] is not JsonObject child)
                {
                    child = new JsonObject();
                    target[name] = child;
                }

                this.CopyManagedValues(child, source[name] as JsonObject ?? new JsonObject(), key);
                continue;
            }

            if (!this.IsDefinedElsewhere(key))
            {
                continue;
            }

            if (source[name] is { } value)
            {
                target[name] = value.DeepClone();
            }
            else
            {
                target.Remove(name);
            }
        }
    }

    private async ValueTask<JsonObject> ReadRootAsync(CancellationToken cancellationToken)
    {
        var json = await File.ReadAllTextAsync(this.FilePath, cancellationToken).ConfigureAwait(false);
        return ParseObject(json);
    }
}

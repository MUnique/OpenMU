// <copyright file="DiscordServerLayout.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.Provisioning;

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// The layout of a Discord server for a game server: its roles, categories and channels.
/// The channels are identified by their keys, so that the notifications find their channels
/// without configuring the identifiers of the channels.
/// </summary>
public sealed class DiscordServerLayout
{
    /// <summary>
    /// The key of the role which the bot gives to the Discord users who are linked to a game account.
    /// </summary>
    public const string LinkedRoleKey = "linked";

    /// <summary>
    /// The key of the category in which the bot creates the channels for the chats of guilds.
    /// </summary>
    public const string GuildsCategoryKey = "guilds";

    private const string DefaultLayoutResourceName = "MUnique.OpenMU.Discord.Provisioning.DefaultLayout.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>
    /// Gets or sets the roles.
    /// </summary>
    public List<DiscordRoleLayout> Roles { get; set; } = new();

    /// <summary>
    /// Gets or sets the categories with their channels.
    /// </summary>
    public List<DiscordCategoryLayout> Categories { get; set; } = new();

    /// <summary>
    /// Gets all channels.
    /// </summary>
    [JsonIgnore]
    public IEnumerable<DiscordChannelLayout> Channels => this.Categories.SelectMany(category => category.Channels);

    /// <summary>
    /// Loads the default layout, which is part of this assembly.
    /// </summary>
    /// <returns>The default layout.</returns>
    public static DiscordServerLayout LoadDefault()
    {
        using var stream = typeof(DiscordServerLayout).Assembly.GetManifestResourceStream(DefaultLayoutResourceName)
                           ?? throw new InvalidOperationException($"The resource {DefaultLayoutResourceName} wasn't found.");
        return Load(stream);
    }

    /// <summary>
    /// Loads a layout from a JSON file.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <returns>The layout.</returns>
    public static DiscordServerLayout Load(string path)
    {
        using var stream = File.OpenRead(path);
        return Load(stream);
    }

    /// <summary>
    /// Loads a layout from a JSON stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>The layout.</returns>
    /// <exception cref="InvalidDataException">If the layout isn't valid.</exception>
    public static DiscordServerLayout Load(Stream stream)
    {
        var layout = JsonSerializer.Deserialize<DiscordServerLayout>(stream, SerializerOptions)
                     ?? throw new InvalidDataException("The Discord server layout is empty.");
        var errors = layout.Validate();
        if (errors.Count > 0)
        {
            throw new InvalidDataException("The Discord server layout is invalid: " + string.Join(" ", errors));
        }

        return layout;
    }

    /// <summary>
    /// Validates the layout.
    /// </summary>
    /// <returns>The errors; empty, if the layout is valid.</returns>
    public IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();
        var roleKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var role in this.Roles)
        {
            ValidateItem(errors, "role", role.Key, role.Name, roleKeys);
        }

        var categoryKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var channelKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var category in this.Categories)
        {
            ValidateItem(errors, "category", category.Key, category.Name, categoryKeys);
            foreach (var roleKey in category.VisibleTo.Where(roleKey => !roleKeys.Contains(roleKey)))
            {
                errors.Add($"The category '{category.Key}' refers to the unknown role '{roleKey}'.");
            }

            foreach (var channel in category.Channels)
            {
                ValidateItem(errors, "channel", channel.Key, channel.Name, channelKeys);
            }
        }

        return errors;
    }

    private static void ValidateItem(List<string> errors, string kind, string key, string name, HashSet<string> keys)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(name))
        {
            errors.Add($"Every {kind} needs a key and a name.");
        }
        else if (!keys.Add(key))
        {
            errors.Add($"The {kind} key '{key}' isn't unique.");
        }
    }
}

// <copyright file="DiscordPublisherOptions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.ComponentModel.DataAnnotations;
using System.Reflection;
using MUnique.OpenMU.Discord;
using MUnique.OpenMU.GameLogic.PlugIns.GameEvents;

/// <summary>
/// Provides publisher option metadata for the Discord configuration form.
/// </summary>
internal static class DiscordPublisherOptions
{
    private static readonly IReadOnlyDictionary<string, PropertyInfo> Properties = typeof(GameEventPublisherConfiguration)
        .GetProperties().ToDictionary(property => property.Name);

    /// <summary>
    /// Gets the localized label of a publisher setting.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <returns>The label.</returns>
    internal static string GetLabel(string name) => Properties[name].GetCustomAttribute<DisplayAttribute>()?.GetName() ?? name;

    /// <summary>
    /// Gets the event switches associated with a delivery category.
    /// </summary>
    /// <param name="category">The delivery category.</param>
    /// <returns>The property names of the switches.</returns>
    internal static IEnumerable<string> GetOptions(DiscordChannelCategory category) => category switch
    {
        DiscordChannelCategory.Events => [nameof(GameEventPublisherConfiguration.PublishMiniGameEvents), nameof(GameEventPublisherConfiguration.PublishInvasionEvents)],
        DiscordChannelCategory.CastleSiege => [nameof(GameEventPublisherConfiguration.PublishCastleSiegeEvents)],
        DiscordChannelCategory.Notices => [nameof(GameEventPublisherConfiguration.PublishGlobalNotices)],
        DiscordChannelCategory.WorldNews =>
        [
            nameof(GameEventPublisherConfiguration.PublishExcellentItemDrops),
            nameof(GameEventPublisherConfiguration.PublishAncientItemDrops),
            nameof(GameEventPublisherConfiguration.PublishBossKills),
            nameof(GameEventPublisherConfiguration.PublishLevelMilestones),
        ],
        _ => [],
    };

    /// <summary>
    /// Reads an event switch.
    /// </summary>
    /// <param name="settings">The publisher settings.</param>
    /// <param name="name">The Boolean property name.</param>
    /// <returns>Whether the switch is enabled.</returns>
    internal static bool GetFlag(GameEventPublisherConfiguration? settings, string name) => settings is not null && (bool)Properties[name].GetValue(settings)!;

    /// <summary>
    /// Stages an event switch.
    /// </summary>
    /// <param name="settings">The publisher settings.</param>
    /// <param name="name">The Boolean property name.</param>
    /// <param name="value">The new value.</param>
    internal static void SetFlag(GameEventPublisherConfiguration? settings, string name, bool value)
    {
        if (settings is not null)
        {
            Properties[name].SetValue(settings, value);
        }
    }
}

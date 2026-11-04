// <copyright file="ChatCommandTypeExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Globalization;
using System.Reflection;
using System.Resources;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Extension methods regarding chat command types.
/// </summary>
public static class ChatCommandTypeExtensions
{
    /// <summary>
    /// Gets the available chat commands of the player.
    /// Deactivated command plugins are not included, because they can't be
    /// executed either - the <see cref="PlugInManager"/> only resolves active
    /// plugins when a command is dispatched.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The available chat commands of the player.</returns>
    public static IEnumerable<ChatCommandHelpAttribute> GetAvailableChatCommands(this Player player)
    {
        return GetAvailableCommands(player).Select(command => command.Help);
    }

    /// <summary>
    /// Gets the description of the available chat commands of the player,
    /// in the language of the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The described chat commands which are available to the player.</returns>
    public static IEnumerable<ChatCommandInfo> GetAvailableChatCommandInfos(this Player player)
    {
        return GetAvailableCommands(player)
            .Select(command => CreateChatCommandInfo(command.PlugInType, command.Help, player.Culture));
    }

    /// <summary>
    /// Creates the description of the chat command which is implemented by the specified plugin type.
    /// </summary>
    /// <param name="plugInType">The type of the chat command plugin.</param>
    /// <param name="culture">The language in which name and description should be returned. If it's <see langword="null"/>, the language of the current thread is used.</param>
    /// <returns>The described chat command, if the type is a chat command plugin; Otherwise, <see langword="null"/>.</returns>
    public static ChatCommandInfo? TryCreateChatCommandInfo(Type plugInType, CultureInfo? culture = null)
    {
        if (plugInType.GetCustomAttribute<ChatCommandHelpAttribute>() is not { } help)
        {
            return null;
        }

        return CreateChatCommandInfo(plugInType, help, culture);
    }

    private static IEnumerable<(Type PlugInType, ChatCommandHelpAttribute Help)> GetAvailableCommands(Player player)
    {
        var plugInManager = player.GameContext?.PlugInManager;
        if (plugInManager is null)
        {
            return [];
        }

        return plugInManager
            .GetKnownPlugInsOf<IChatCommandPlugIn>()
            .Where(plugInManager.IsPlugInActive)
            .Select(plugInType => (PlugInType: plugInType, Help: plugInType.GetCustomAttribute<ChatCommandHelpAttribute>()))
            .Where(command => command.Help is { })
            .Where(command => player.SelectedCharacter?.CharacterStatus >= command.Help!.MinimumCharacterStatus)
            .Select(command => (command.PlugInType, Help: command.Help!));
    }

    private static ChatCommandInfo CreateChatCommandInfo(Type plugInType, ChatCommandHelpAttribute help, CultureInfo? culture)
    {
        var display = plugInType.GetCustomAttribute<DisplayAttribute>();
        IReadOnlyList<ChatCommandParameterInfo> parameters = help.ArgumentsType is { } argumentsType
            ? CommandExtensions.GetParameterInfos(argumentsType).ToList()
            : [];

        return new ChatCommandInfo(
            help.Command,
            GetLocalizedValue(display, display?.Name, () => display?.GetName(), culture) ?? plugInType.Name,
            GetLocalizedValue(display, display?.Description, () => display?.GetDescription(), culture) ?? help.Description ?? string.Empty,
            help.MinimumCharacterStatus,
            help.Usage,
            parameters);
    }

    /// <summary>
    /// Gets the value of a <see cref="DisplayAttribute"/> in the requested language.
    /// </summary>
    /// <param name="display">The attribute.</param>
    /// <param name="rawValue">The raw value of the requested property, which is a resource key when a <see cref="DisplayAttribute.ResourceType"/> is set.</param>
    /// <param name="defaultResolver">The resolver of the attribute itself, which uses the language of the current thread.</param>
    /// <param name="culture">The requested language.</param>
    /// <returns>The value in the requested language, if available.</returns>
    private static string? GetLocalizedValue(DisplayAttribute? display, string? rawValue, Func<string?> defaultResolver, CultureInfo? culture)
    {
        if (display is null || string.IsNullOrEmpty(rawValue))
        {
            return null;
        }

        if (culture is { }
            && display.ResourceType is { } resourceType
            && resourceType.GetProperty(nameof(PlugInResources.ResourceManager), BindingFlags.Public | BindingFlags.Static)?.GetValue(null) is ResourceManager resourceManager
            && resourceManager.GetString(rawValue, culture) is { } localizedValue)
        {
            return localizedValue;
        }

        // Either no specific language was requested, or the value is a literal which
        // can't be translated anyway - the attribute knows how to resolve both.
        try
        {
            return defaultResolver();
        }
        catch (InvalidOperationException)
        {
            // The attribute refers to a resource which doesn't exist. A single
            // misconfigured plugin shouldn't break the whole command list.
            return null;
        }
    }
}

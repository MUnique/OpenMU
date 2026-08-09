// <copyright file="PlayerMessageExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.Properties;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Extensions to show (localized) messages to a <see cref="Player"/>.
/// </summary>
public static class PlayerMessageExtensions
{
    /// <summary>
    /// Gets the localized message.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="resourceName">Name of the resource.</param>
    /// <param name="formatArguments">The format arguments.</param>
    /// <returns>The localized message.</returns>
    public static string GetLocalizedMessage(this Player player, string resourceName, params ReadOnlySpan<object?> formatArguments)
    {
        if (formatArguments.Length > 0)
        {
            return string.Format(PlayerMessage.ResourceManager.GetString(resourceName, player.Culture) ?? string.Empty, formatArguments);
        }

        return PlayerMessage.ResourceManager.GetString(resourceName, player.Culture) ?? string.Empty;
    }

    /// <summary>
    /// Easier way to show a localized blue message to the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="messageKey">The message resource key.</param>
    /// <param name="arguments">The parameters for the message.</param>
    public static ValueTask ShowLocalizedBlueMessageAsync(this Player player, string messageKey, params ReadOnlySpan<object?> arguments)
    {
        var message = player.GetLocalizedMessage(messageKey, arguments);
        return player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal));
    }

    /// <summary>
    /// Easier way to show a localized golden message to the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="messageKey">The message resource key.</param>
    /// <param name="arguments">The parameters for the message.</param>
    public static ValueTask ShowLocalizedGoldenMessageAsync(this Player player, string messageKey, params ReadOnlySpan<object?> arguments)
    {
        var message = player.GetLocalizedMessage(messageKey, arguments);
        return player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.GoldenCenter));
    }

    /// <summary>
    /// Easier way to show a blue message to the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="message">The message.</param>
    public static ValueTask ShowBlueMessageAsync(this Player player, string message)
    {
        return player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal));
    }
}

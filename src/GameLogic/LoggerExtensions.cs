// <copyright file="LoggerExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Extensions for <see cref="ILogger"/>s.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="values">The key value pairs which describe the scope.</param>
    /// <returns>An <see cref="T:System.IDisposable" /> that ends the logical operation scope on dispose.</returns>
    public static IDisposable? BeginScope(this ILogger logger, params (string Key, object Value)[] values)
    {
        return logger.BeginScope(values.Select(pair => new KeyValuePair<string, object>(pair.Key, pair.Value)));
    }

    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="gameContext">The game context.</param>
    /// <returns>An <see cref="T:System.IDisposable" /> that ends the logical operation scope on dispose.</returns>
    public static IDisposable? BeginScope(this ILogger logger, IGameContext gameContext)
    {
        if (gameContext is IGameServerContext gameServerContext)
        {
            return logger.BeginScope("GameServer {id}", gameServerContext.Id);
        }

        return null;
    }
}
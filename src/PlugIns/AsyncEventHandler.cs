// <copyright file="AsyncEventHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

/// <summary>
/// Event handler which is awaitable.
/// </summary>
/// <typeparam name="T">The type of the event args.</typeparam>
/// <param name="eventArgs">The event arguments.</param>
public delegate ValueTask AsyncEventHandler<T>(T eventArgs);

/// <summary>
/// Event handler without arguments which is awaitable.
/// </summary>
/// <typeparam name="T">The type of the event args.</typeparam>
public delegate ValueTask AsyncEventHandler();

/// <summary>
/// Extensions for <see cref="AsyncEventHandler"/>.
/// </summary>
public static class EventExtensions
{
    /// <summary>
    /// Invokes an event if any event handler is registered. If none is registered, nothing happens.
    /// </summary>
    /// <typeparam name="T">The type of the event args.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <param name="argsFactory">The arguments factory.</param>
    public static ValueTask SafeInvokeAsync<T>(this AsyncEventHandler<T>? handler, Func<T> argsFactory)
    {
        if (handler is null)
        {
            return ValueTask.CompletedTask;
        }

        return handler.Invoke(argsFactory());
    }

    /// <summary>
    /// Invokes an event if any event handler is registered. If none is registered, nothing happens.
    /// </summary>
    /// <typeparam name="T">The type of the event args.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <param name="args">The arguments.</param>
    public static ValueTask SafeInvokeAsync<T>(this AsyncEventHandler<T>? handler, T args)
    {
        if (handler is null)
        {
            return ValueTask.CompletedTask;
        }

        return handler.Invoke(args);
    }

    /// <summary>
    /// Invokes an event if any event handler is registered. If none is registered, nothing happens.
    /// </summary>
    /// <param name="handler">The handler.</param>
    public static ValueTask SafeInvokeAsync(this AsyncEventHandler? handler)
    {
        if (handler is null)
        {
            return ValueTask.CompletedTask;
        }

        return handler.Invoke();
    }
}
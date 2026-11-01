// <copyright file="EventExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

using System.Runtime.ExceptionServices;

/// <summary>
/// Extensions for <see cref="AsyncEventHandler"/>.
/// </summary>
/// <remarks>
/// Invoking a multicast <see cref="ValueTask"/> delegate directly returns only the task of its
/// last subscriber, so the other subscribers would run unobserved, and their exceptions would get
/// lost. That's why these methods invoke the subscribers one after another and await each of them.
/// </remarks>
public static class EventExtensions
{
    /// <summary>
    /// Invokes an event if any event handler is registered. If none is registered, nothing happens.
    /// The subscribers are invoked one after another, each after the previous one completed.
    /// </summary>
    /// <typeparam name="T">The type of the event args.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <param name="argsFactory">The arguments factory.</param>
    /// <returns>The task which completes after all subscribers completed.</returns>
    /// <remarks>
    /// When a subscriber throws, the remaining subscribers are still invoked, and the exception is
    /// thrown afterwards; an <see cref="AggregateException"/> when several subscribers threw.
    /// </remarks>
    public static ValueTask SafeInvokeAsync<T>(this AsyncEventHandler<T>? handler, Func<T> argsFactory)
    {
        if (handler is null)
        {
            return ValueTask.CompletedTask;
        }

        return handler.SafeInvokeAsync(argsFactory());
    }

    /// <summary>
    /// Invokes an event if any event handler is registered. If none is registered, nothing happens.
    /// The subscribers are invoked one after another, each after the previous one completed.
    /// </summary>
    /// <typeparam name="T">The type of the event args.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>The task which completes after all subscribers completed.</returns>
    /// <remarks>
    /// When a subscriber throws, the remaining subscribers are still invoked, and the exception is
    /// thrown afterwards; an <see cref="AggregateException"/> when several subscribers threw.
    /// </remarks>
    public static ValueTask SafeInvokeAsync<T>(this AsyncEventHandler<T>? handler, T args)
    {
        if (handler is null)
        {
            return ValueTask.CompletedTask;
        }

        if (handler.HasSingleTarget)
        {
            return handler.Invoke(args);
        }

        return InvokeOneByOneAsync(handler, args);
    }

    /// <summary>
    /// Invokes an event if any event handler is registered. If none is registered, nothing happens.
    /// The subscribers are invoked one after another, each after the previous one completed.
    /// </summary>
    /// <param name="handler">The handler.</param>
    /// <returns>The task which completes after all subscribers completed.</returns>
    /// <remarks>
    /// When a subscriber throws, the remaining subscribers are still invoked, and the exception is
    /// thrown afterwards; an <see cref="AggregateException"/> when several subscribers threw.
    /// </remarks>
    public static ValueTask SafeInvokeAsync(this AsyncEventHandler? handler)
    {
        if (handler is null)
        {
            return ValueTask.CompletedTask;
        }

        if (handler.HasSingleTarget)
        {
            return handler.Invoke();
        }

        return InvokeOneByOneAsync(handler);
    }

    private static async ValueTask InvokeOneByOneAsync<T>(AsyncEventHandler<T> handler, T args)
    {
        List<Exception>? exceptions = null;
        foreach (var subscriber in handler.GetInvocationList())
        {
            try
            {
                await ((AsyncEventHandler<T>)subscriber).Invoke(args).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptions ??= new();
                exceptions.Add(ex);
            }
        }

        ThrowIfAny(exceptions);
    }

    private static async ValueTask InvokeOneByOneAsync(AsyncEventHandler handler)
    {
        List<Exception>? exceptions = null;
        foreach (var subscriber in handler.GetInvocationList())
        {
            try
            {
                await ((AsyncEventHandler)subscriber).Invoke().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptions ??= new();
                exceptions.Add(ex);
            }
        }

        ThrowIfAny(exceptions);
    }

    private static void ThrowIfAny(List<Exception>? exceptions)
    {
        if (exceptions is null)
        {
            return;
        }

        if (exceptions.Count == 1)
        {
            ExceptionDispatchInfo.Capture(exceptions[0]).Throw();
        }

        throw new AggregateException(exceptions);
    }
}

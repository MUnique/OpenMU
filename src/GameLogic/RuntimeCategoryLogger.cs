// <copyright file="RuntimeCategoryLogger.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// An <see cref="ILogger{T}"/> which files its entries under the category of the owner's actual
/// runtime type, while still satisfying an <see cref="ILoggerOwner{T}"/> contract declared for a
/// base type.
/// </summary>
/// <remarks>
/// <see cref="Player"/> is subclassed and a logger built with <c>CreateLogger&lt;Player&gt;()</c> puts
/// every subclass under the same <c>...GameLogic.Player</c> category, so their output cannot be told
/// apart by log configuration. This adapter resolves the category from the runtime type instead
/// (e.g. <c>...GameLogic.Bots.BotPlayer</c>), which Serilog's prefix-based <c>MinimumLevel.Override</c>
/// can then address individually. The wrapper is needed because the <see cref="ILoggerOwner{T}"/>
/// contract requires an <see cref="ILogger{T}"/>, while <c>CreateLogger(Type)</c> only returns a
/// non-generic <see cref="ILogger"/>.
/// It is currently used to give bots their own log category, leaving real players on the
/// <see cref="Player"/> category untouched.
/// </remarks>
/// <typeparam name="T">The type whose <see cref="ILogger{T}"/> contract is being satisfied.</typeparam>
internal sealed class RuntimeCategoryLogger<T> : ILogger<T>
{
    private readonly ILogger _inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeCategoryLogger{T}"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="owner">The instance whose runtime type provides the log category.</param>
    public RuntimeCategoryLogger(ILoggerFactory loggerFactory, object owner)
    {
        this._inner = loggerFactory.CreateLogger(owner.GetType());
    }

    /// <inheritdoc />
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => this._inner.BeginScope(state);

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) => this._inner.IsEnabled(logLevel);

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        => this._inner.Log(logLevel, eventId, state, exception, formatter);
}

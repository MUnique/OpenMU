// <copyright file="CapturingLoggerFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging;

/// <summary>
/// A logger factory which keeps the log entries of all its loggers, so that a test can check them.
/// </summary>
internal sealed class CapturingLoggerFactory : ILoggerFactory
{
    private readonly List<(LogLevel Level, string Message)> _entries = new();

    /// <summary>
    /// Gets a copy of the log entries which were written so far.
    /// </summary>
    public IReadOnlyList<(LogLevel Level, string Message)> Entries
    {
        get
        {
            lock (this._entries)
            {
                return this._entries.ToList();
            }
        }
    }

    /// <inheritdoc />
    public ILogger CreateLogger(string categoryName) => new CapturingLogger(this);

    /// <inheritdoc />
    public void AddProvider(ILoggerProvider provider)
    {
        // Not needed, the entries are kept by the factory itself.
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Nothing to dispose.
    }

    private void Add(LogLevel level, string message)
    {
        lock (this._entries)
        {
            this._entries.Add((level, message));
        }
    }

    private sealed class CapturingLogger : ILogger
    {
        private readonly CapturingLoggerFactory _factory;

        public CapturingLogger(CapturingLoggerFactory factory)
        {
            this._factory = factory;
        }

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
            => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            this._factory.Add(logLevel, formatter(state, exception));
        }
    }
}

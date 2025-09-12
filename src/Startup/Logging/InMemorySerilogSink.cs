namespace MUnique.OpenMU.Startup.Logging;

using Serilog.Core;
using Serilog.Events;
using System.Globalization;

/// <summary>
/// Serilog sink which writes formatted log events into an <see cref="InMemoryLogBuffer"/>.
/// </summary>
internal sealed class InMemorySerilogSink : ILogEventSink
{
    private readonly InMemoryLogBuffer _buffer;

    public InMemorySerilogSink(InMemoryLogBuffer buffer)
    {
        _buffer = buffer;
    }

    public void Emit(LogEvent logEvent)
    {
        try
        {
            var timestamp = logEvent.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            var level = logEvent.Level.ToString().ToUpperInvariant().PadRight(7);
            var message = logEvent.RenderMessage(CultureInfo.InvariantCulture);
            var line = $"{timestamp} [{level}] {message}";
            if (logEvent.Exception is { } ex)
            {
                line += $" | {ex.GetType().Name}: {ex.Message}";
            }

            _buffer.Add(line);
        }
        catch
        {
            // ignore
        }
    }
}


namespace MUnique.OpenMU.Startup.Logging;

/// <summary>
/// Static access to the shared in-memory log buffer for simple registration.
/// </summary>
internal static class ConsoleLog
{
    public static InMemoryLogBuffer Buffer { get; set; } = new();
}


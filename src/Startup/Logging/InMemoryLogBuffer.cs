namespace MUnique.OpenMU.Startup.Logging;

using System.Collections.Concurrent;

/// <summary>
/// In-memory ring buffer for recent log lines.
/// </summary>
internal sealed class InMemoryLogBuffer
{
    private readonly ConcurrentQueue<string> _queue = new();
    private readonly int _capacity;

    public InMemoryLogBuffer(int capacity = 2000)
    {
        _capacity = Math.Max(100, capacity);
    }

    public void Add(string line)
    {
        _queue.Enqueue(line);
        while (_queue.Count > _capacity && _queue.TryDequeue(out _))
        {
            // drop old
        }
    }

    public IReadOnlyList<string> Tail(int count = 200)
    {
        if (count <= 0)
        {
            return Array.Empty<string>();
        }

        return _queue.Reverse().Take(count).Reverse().ToList();
    }
}


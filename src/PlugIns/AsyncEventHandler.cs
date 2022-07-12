namespace MUnique.OpenMU.PlugIns;

public delegate ValueTask AsyncEventHandler<T>(T eventArgs);

public delegate ValueTask AsyncEventHandler();

public static class EventExtensions
{
    public static ValueTask SafeInvoke<T>(this AsyncEventHandler<T>? handler, Func<T> argsFactory)
    {
        if (handler is null)
        {
            return ValueTask.CompletedTask;
        }

        return handler.Invoke(argsFactory());
    }

    public static ValueTask SafeInvoke<T>(this AsyncEventHandler<T>? handler, T args)
    {
        if (handler is null)
        {
            return ValueTask.CompletedTask;
        }

        return handler.Invoke(args);
    }

    public static ValueTask SafeInvoke(this AsyncEventHandler? handler)
    {
        if (handler is null)
        {
            return ValueTask.CompletedTask;
        }

        return handler.Invoke();
    }
}
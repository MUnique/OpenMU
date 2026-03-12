// <copyright file="NullViewProxy.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using System.Reflection;

/// <summary>
/// A dynamic proxy that returns completed tasks for async methods
/// and default values for all other return types.
/// </summary>
internal class NullViewProxy : DispatchProxy
{
    private static readonly MethodInfo TaskFromResultMethod =
        typeof(Task).GetMethod(nameof(Task.FromResult))!;

    private static readonly MethodInfo ValueTaskFromResultMethod =
        typeof(ValueTask).GetMethod(nameof(ValueTask.FromResult))!;

    /// <inheritdoc/>
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod is null)
        {
            return null;
        }

        var returnType = targetMethod.ReturnType;

        if (returnType == typeof(Task))
        {
            return Task.CompletedTask;
        }

        if (returnType == typeof(ValueTask))
        {
            return ValueTask.CompletedTask;
        }

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
        {
            var arg = returnType.GetGenericArguments()[0];
            return ValueTaskFromResultMethod.MakeGenericMethod(arg)
                .Invoke(null, [GetDefaultValue(arg)]);
        }

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var arg = returnType.GetGenericArguments()[0];
            return TaskFromResultMethod.MakeGenericMethod(arg)
                .Invoke(null, [GetDefaultValue(arg)]);
        }

        return GetDefaultValue(returnType);
    }

    private static object? GetDefaultValue(Type type)
        => type.IsValueType ? Activator.CreateInstance(type) : null;
}
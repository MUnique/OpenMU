// <copyright file="LocalizableExceptionBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

using System.Resources;

/// <summary>
/// Base class for localizable exceptions sharing a common resource manager cache.
/// </summary>
public abstract class LocalizableExceptionBase : Exception
{
    /// <summary>
    /// The cached resource managers shared across all localizable exceptions.
    /// </summary>
    private static readonly Dictionary<Type, ResourceManager> ResourceManagers = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizableExceptionBase"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="resourceKey">The resource key.</param>
    /// <param name="formatArguments">The format arguments.</param>
    protected LocalizableExceptionBase(string? message, Exception? innerException, string resourceKey, ReadOnlySpan<object?> formatArguments)
        : base(message, innerException)
    {
        this.FormatArguments = formatArguments.ToArray();
        this.ResourceKey = resourceKey;
    }

    /// <summary>
    /// Gets the format arguments.
    /// </summary>
    public object?[] FormatArguments { get; }

    /// <summary>
    /// Gets the resource key.
    /// </summary>
    public string ResourceKey { get; }

    /// <summary>
    /// Resolves (and caches) the <see cref="ResourceManager"/> for the specified resource type.
    /// </summary>
    /// <param name="resourceType">The resource type.</param>
    /// <returns>The resource manager.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resource type does not expose a public static ResourceManager property.
    /// </exception>
    protected static ResourceManager ResolveResourceManager(Type resourceType)
    {
        if (!ResourceManagers.TryGetValue(resourceType, out var resourceManager))
        {
            var resourceManagerProperty = resourceType.GetProperty(
                "ResourceManager",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

            resourceManager = resourceManagerProperty?.GetValue(null) as ResourceManager
                              ?? throw new InvalidOperationException(
                                  $"The type {resourceType.FullName} does not have a public static ResourceManager property.");

            ResourceManagers[resourceType] = resourceManager;
        }

        return resourceManager;
    }
}
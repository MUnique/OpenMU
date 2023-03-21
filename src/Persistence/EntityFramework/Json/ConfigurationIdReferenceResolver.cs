// <copyright file="ConfigurationIdReferenceResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Json;

using System.Text.Json.Serialization;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// A reference resolver which resolves them by looking at the objects occurring in the <see cref="GameConfiguration" />.
/// The cache is maintained by instances of the <see cref="ConfigurationTypeRepository{T}" />.
/// </summary>
/// <remarks>TODO: I don't like it as a singleton, but I keep it until I find a cleaner solution.</remarks>
internal class ConfigurationIdReferenceResolver : ReferenceResolver
{
    /// <summary>
    /// The singleton instance.
    /// </summary>
    private static readonly ConfigurationIdReferenceResolver InstanceValue = new();

    private readonly IDictionary<Guid, IIdentifiable> _cache = new Dictionary<Guid, IIdentifiable>();

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationIdReferenceResolver"/> class.
    /// </summary>
    protected ConfigurationIdReferenceResolver()
    {
    }

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static ConfigurationIdReferenceResolver Instance => InstanceValue;

    /// <inheritdoc />
    public override object ResolveReference(string referenceId)
    {
        var id = new Guid(referenceId);
        if (id == Guid.Empty)
        {
            return null!;
        }

        var success = this._cache.TryGetValue(id, out var obj);
        if (!success)
        {
            return null!;
        }

        return obj!;
    }

    /// <inheritdoc/>
    public override string GetReference(object value, out bool alreadyExists)
    {
        var p = (IIdentifiable)value;
        alreadyExists = this._cache.ContainsKey(p.Id);
        return p.Id.ToString();
    }

    /// <inheritdoc/>
    public override void AddReference(string referenceId, object value)
    {
        var id = new Guid(referenceId);
        this._cache[id] = (IIdentifiable)value;
    }

    /// <summary>
    /// Adds the reference to the cache of the resolver.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AddReference(IIdentifiable value)
    {
        this._cache[value.Id] = value;
    }

    /// <summary>
    /// Removes the reference from the cache of the resolver.
    /// </summary>
    /// <param name="key">The key.</param>
    public void RemoveReference(Guid key)
    {
        this._cache.Remove(key);
    }
}
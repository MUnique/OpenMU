// <copyright file="IdReferenceResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using Newtonsoft.Json.Serialization;

/// <summary>
/// A reference resolver, which resolves based on $id references which are values of <see cref="IIdentifiable.Id" />.
/// </summary>
public class IdReferenceResolver : IReferenceResolver
{
    private readonly IDictionary<Guid, IIdentifiable> _objects = new Dictionary<Guid, IIdentifiable>();

    /// <inheritdoc />
    public object ResolveReference(object context, string reference)
    {
        var identity = new Guid(reference);
        if (identity == Guid.Empty)
        {
            return null!;
        }

        var success = this._objects.TryGetValue(identity, out var obj);
        if (!success)
        {
            return null!; // we're handling null returns
        }

        return obj!;
    }

    /// <inheritdoc/>
    public string GetReference(object context, object value)
    {
        if (value is IIdentifiable identifiable)
        {
            this._objects[identifiable.Id] = identifiable;
            return identifiable.Id.ToString();
        }

        return null!; // we're handling null returns
    }

    /// <inheritdoc/>
    public bool IsReferenced(object context, object value)
    {
        return value is IIdentifiable identifiable && this._objects.ContainsKey(identifiable.Id);
    }

    /// <inheritdoc/>
    public void AddReference(object context, string reference, object value)
    {
        if (value is IIdentifiable identifiable)
        {
            var identity = new Guid(reference);
            this._objects[identity] = identifiable;
        }
    }
}
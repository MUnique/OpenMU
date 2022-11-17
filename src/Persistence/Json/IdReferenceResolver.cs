// <copyright file="IdReferenceResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using System.Text.Json.Serialization;

/// <summary>
/// A reference resolver, which resolves based on $id references which are values of <see cref="IIdentifiable.Id" />.
/// </summary>
public class IdReferenceResolver : ReferenceResolver
{
    private readonly IDictionary<Guid, IIdentifiable> _objects = new Dictionary<Guid, IIdentifiable>();

    /// <inheritdoc />
    public override object ResolveReference(string referenceId)
    {
        var identity = new Guid(referenceId);
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
    public override string GetReference(object value, out bool alreadyExists)
    {
        if (value is IIdentifiable identifiable)
        {
            alreadyExists = !this._objects.TryAdd(identifiable.Id, identifiable);
            return identifiable.Id.ToString();
        }

        throw new InvalidOperationException("value must implement IIdentifiable");
    }

    /// <inheritdoc/>
    public override void AddReference(string reference, object value)
    {
        if (value is IIdentifiable identifiable)
        {
            var identity = new Guid(reference);
            this._objects[identity] = identifiable;
        }
    }
}
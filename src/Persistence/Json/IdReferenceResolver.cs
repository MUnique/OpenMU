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
    private readonly Dictionary<Guid, IIdentifiable> _objects = new();

    private readonly Dictionary<object, string> _objectToReferenceIdMap = new();
    private uint _referenceCount;

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

        if (this._objectToReferenceIdMap.TryGetValue(value, out string? referenceId))
        {
            alreadyExists = true;
        }
        else
        {
            this._referenceCount++;
            referenceId = this._referenceCount.ToString();
            this._objectToReferenceIdMap.Add(value, referenceId);
            alreadyExists = false;
        }

        return referenceId;
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
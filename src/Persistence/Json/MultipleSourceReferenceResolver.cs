// <copyright file="MultipleSourceReferenceResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using System.Text.Json.Serialization;

/// <summary>
/// A reference resolver which can resolve references by using multiple reference resolvers itself.
/// </summary>
public class MultipleSourceReferenceResolver : ReferenceResolver
{
    private readonly IList<ReferenceResolver> _resolvers;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleSourceReferenceResolver"/> class.
    /// </summary>
    /// <param name="mainResolver">The main resolver, used to add references.</param>
    /// <param name="fallbackResolvers">The fallback resolvers.</param>
    public MultipleSourceReferenceResolver(ReferenceResolver mainResolver, params ReferenceResolver[] fallbackResolvers)
    {
        this._resolvers = new List<ReferenceResolver> { mainResolver };
        foreach (var resolver in fallbackResolvers)
        {
            this._resolvers.Add(resolver);
        }
    }

    /// <inheritdoc />
    public override object ResolveReference(string referenceId)
    {
        foreach (var resolver in this._resolvers)
        {
            var resolved = resolver.ResolveReference(referenceId);
            if (resolved is { })
            {
                return resolved;
            }
        }

        return null!; // we're handling null returns
    }

    /// <inheritdoc />
    public override string GetReference(object value, out bool alreadyExists)
    {
        return this._resolvers.First().GetReference(value, out alreadyExists);
    }

    /// <inheritdoc />
    public override void AddReference(string referenceId, object value)
    {
        this._resolvers.First().AddReference(referenceId, value);
    }
}
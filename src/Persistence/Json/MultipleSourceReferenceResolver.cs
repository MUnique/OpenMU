// <copyright file="MultipleSourceReferenceResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using Newtonsoft.Json.Serialization;

/// <summary>
/// A reference resolver which can resolve references by using multiple reference resolvers itself.
/// </summary>
public class MultipleSourceReferenceResolver : IReferenceResolver
{
    private readonly IList<IReferenceResolver> _resolvers;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleSourceReferenceResolver"/> class.
    /// </summary>
    /// <param name="mainResolver">The main resolver, used to add references.</param>
    /// <param name="fallbackResolvers">The fallback resolvers.</param>
    public MultipleSourceReferenceResolver(IReferenceResolver mainResolver, params IReferenceResolver[] fallbackResolvers)
    {
        this._resolvers = new List<IReferenceResolver> { mainResolver };
        foreach (var resolver in fallbackResolvers)
        {
            this._resolvers.Add(resolver);
        }
    }

    /// <inheritdoc />
    public object ResolveReference(object context, string reference)
    {
        foreach (var resolver in this._resolvers)
        {
            var resolved = resolver.ResolveReference(context, reference);
            if (resolved != null!)
            {
                return resolved;
            }
        }

        return null!; // we're handling null returns
    }

    /// <inheritdoc />
    public string GetReference(object context, object value)
    {
        return this._resolvers.First().GetReference(context, value);
    }

    /// <inheritdoc />
    public bool IsReferenced(object context, object value)
    {
        return this._resolvers.Any(resolver => resolver.IsReferenced(context, value));
    }

    /// <inheritdoc />
    public void AddReference(object context, string reference, object value)
    {
        this._resolvers.First().AddReference(context, reference, value);
    }
}
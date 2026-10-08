// <copyright file="CachingReferenceHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Json;

using System.Text.Json.Serialization;
using MUnique.OpenMU.Persistence.Json;

/// <summary>
/// A reference handler which considers cached configuration objects.
/// This is basically needed when loading account data. They can have references
/// to configuration.
/// </summary>
public class CachingReferenceHandler : ReferenceHandler, IIdReferenceHandler
{
    private readonly ReferenceResolver? _configurationResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingReferenceHandler"/> class.
    /// </summary>
    /// <param name="configurationResolver">The resolver of the cached configuration objects. If it's <c>null</c>, only the loaded objects are resolved.</param>
    public CachingReferenceHandler(ReferenceResolver? configurationResolver = null)
    {
        this._configurationResolver = configurationResolver;
    }

    /// <summary>
    /// Gets the currently used resolver.
    /// </summary>
    public ReferenceResolver? Current { get; private set; }

    /// <inheritdoc />
    public override ReferenceResolver CreateResolver()
    {
        return this.Current ??= this._configurationResolver is { } configurationResolver
            ? new MultipleSourceReferenceResolver(new IdReferenceResolver(), configurationResolver)
            : new IdReferenceResolver();
    }
}
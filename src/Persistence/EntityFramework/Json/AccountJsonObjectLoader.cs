// <copyright file="AccountJsonObjectLoader.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Json;

using System.Text.Json.Serialization;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// A json object loader for <see cref="Account"/>s.
/// </summary>
public class AccountJsonObjectLoader : JsonObjectLoader
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccountJsonObjectLoader"/> class.
    /// </summary>
    /// <param name="configurationResolver">The resolver of the cached configuration objects, which are referenced by the account data.</param>
    public AccountJsonObjectLoader(ReferenceResolver? configurationResolver = null)
        : base(new JsonQueryBuilder(), new JsonObjectDeserializer(), new CachingReferenceHandler(configurationResolver))
    {
    }
}
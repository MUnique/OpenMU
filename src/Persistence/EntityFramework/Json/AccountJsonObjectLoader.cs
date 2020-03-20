// <copyright file="AccountJsonObjectLoader.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Json
{
    using MUnique.OpenMU.Persistence.Json;

    /// <summary>
    /// A json object loader for <see cref="Account"/>s.
    /// </summary>
    public class AccountJsonObjectLoader : JsonObjectLoader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountJsonObjectLoader"/> class.
        /// </summary>
        public AccountJsonObjectLoader()
            : base(new JsonQueryBuilder(), new JsonObjectDeserializer(), new MultipleSourceReferenceResolver(new IdReferenceResolver(), ConfigurationIdReferenceResolver.Instance))
        {
        }
    }
}
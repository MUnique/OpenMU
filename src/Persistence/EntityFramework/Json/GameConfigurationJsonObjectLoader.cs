// <copyright file="GameConfigurationJsonObjectLoader.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Json
{
    using MUnique.OpenMU.Persistence.Json;

    /// <summary>
    /// Object loader for <see cref="GameConfiguration"/> objects.
    /// </summary>
    public class GameConfigurationJsonObjectLoader : JsonObjectLoader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameConfigurationJsonObjectLoader"/> class.
        /// </summary>
        public GameConfigurationJsonObjectLoader()
            : base(
                  new GameConfigurationJsonQueryBuilder(),
                  new JsonObjectDeserializer { AreCircularReferencesExpected = true },
                  new IdReferenceResolver())
        {
        }
    }
}
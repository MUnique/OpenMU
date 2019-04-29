// <copyright file="ConnectServerDefinitionDto.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// A <see cref="MUnique.OpenMU.Persistence.BasicModel.ConnectServerDefinition"/> with an added game client property, because
    /// otherwise the deserialization expects a json property name starting with a big letter.
    /// </summary>
    public class ConnectServerDefinitionDto : Persistence.BasicModel.ConnectServerDefinition
    {
        /// <summary>
        /// Gets or sets the game client.
        /// </summary>
        [JsonProperty("gameClient")]
        public Persistence.BasicModel.GameClientDefinition GameClient
        {
            get => this.RawClient;
            set => this.RawClient = value;
        }
    }
}
// <copyright file="MuBotData.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities
{
    using System;
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// Information about mu data.
    /// </summary>
    [AggregateRoot]
    public class MuBotData
    {
        /// <summary>
        /// Gets or sets the identifier. Should be the same id as the character id to which it belongs.
        /// </summary>
        public Guid CharacterId { get; set; }

        /// <summary>
        /// Gets or sets the current data of mu bot.
        /// </summary>
        public byte[] Data { get; set; }
    }
}
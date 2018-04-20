// <copyright file="LetterBodyRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory
{
    using System;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// In-Memory repository for <see cref="LetterBody"/>s.
    /// </summary>
    internal class LetterBodyRepository : MemoryRepository<LetterBody>, ILetterBodyRepository<LetterBody>
    {
        /// <inheritdoc />
        public LetterBody GetBodyByHeaderId(Guid headerId) => this.GetAll().FirstOrDefault(body => body.Header.Id == headerId);
    }
}

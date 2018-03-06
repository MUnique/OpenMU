// <copyright file="LetterBodyRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Linq;

    /// <summary>
    /// Repository which is able to load <see cref="LetterBody"/>s for a specific letter header.
    /// </summary>
    internal class LetterBodyRepository : GenericRepository<LetterBody>, ILetterBodyRepository<LetterBody>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LetterBodyRepository"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public LetterBodyRepository(IRepositoryManager manager)
            : base(manager)
        {
        }

        /// <inheritdoc/>
        public LetterBody GetBodyByHeaderId(Guid headerId)
        {
            using (var context = this.GetContext())
            {
                var letterBody = context.Context.Set<LetterBody>().FirstOrDefault(body => body.HeaderId == headerId);
                if (letterBody != null)
                {
                    this.LoadDependentData(letterBody, context.Context);
                }

                return letterBody;
            }
        }
    }
}

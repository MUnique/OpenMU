// <copyright file="LetterBodyRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.EntityFramework.Model;
using System.Threading;

/// <summary>
/// Repository which is able to load <see cref="LetterBody"/>s for a specific letter header.
/// </summary>
internal class LetterBodyRepository : CachingGenericRepository<LetterBody>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LetterBodyRepository" /> class.
    /// </summary>
    /// <param name="repositoryProvider">The repository provider.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public LetterBodyRepository(IContextAwareRepositoryProvider repositoryProvider, ILoggerFactory loggerFactory)
        : base(repositoryProvider, loggerFactory)
    {
    }

    /// <summary>
    /// Gets the letter body by the id of its header.
    /// </summary>
    /// <param name="headerId">The id of its header.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The body of the header.
    /// </returns>
    public async ValueTask<LetterBody?> GetBodyByHeaderIdAsync(Guid headerId, CancellationToken cancellationToken = default)
    {
        using var context = this.GetContext();
        var letterBody = await context.Context.Set<LetterBody>().FirstOrDefaultAsync(body => body.HeaderId == headerId, cancellationToken).ConfigureAwait(false);
        if (letterBody is not null)
        {
            await this.LoadDependentDataAsync(letterBody, context.Context, cancellationToken).ConfigureAwait(false);
        }

        return letterBody;
    }
}
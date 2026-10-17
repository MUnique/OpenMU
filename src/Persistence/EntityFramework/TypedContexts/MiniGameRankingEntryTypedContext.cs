// <copyright file="MiniGameRankingEntryTypedContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.TypedContexts;

/// <summary>
/// The <see cref="TypedContext"/> for <see cref="MUnique.OpenMU.DataModel.Statistics.MiniGameRankingEntry"/>, which uses a compiled model.
/// </summary>
internal sealed class MiniGameRankingEntryTypedContext : TypedContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MiniGameRankingEntryTypedContext"/> class.
    /// </summary>
    public MiniGameRankingEntryTypedContext()
        : base(typeof(MUnique.OpenMU.DataModel.Statistics.MiniGameRankingEntry))
    {
    }
}

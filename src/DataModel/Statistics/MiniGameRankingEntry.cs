// <copyright file="MiniGameRankingEntry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Statistics;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// A ranking entry for a finished mini game. May be useful for statistical purposes.
/// </summary>
public class MiniGameRankingEntry
{
    /// <summary>
    /// Gets or sets an id to be able to identify the same game.
    /// </summary>
    public Guid GameInstanceId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the game has finished.
    /// </summary>
    public DateTime? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the score which the player has reached.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Gets or sets the rank which the player has reached within his game.
    /// </summary>
    public int Rank { get; set; }

    /// <summary>
    /// Gets or sets the character which played the mini game.
    /// </summary>
    [Required]
    public virtual Character? Character { get; set; }

    /// <summary>
    /// Gets or sets the definition of the mini game.
    /// </summary>
    [Required]
    public virtual MiniGameDefinition? MiniGame { get; set; }
}
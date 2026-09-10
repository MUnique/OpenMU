// <copyright file="GuildPositionComparer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Models;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Comparer for <see cref="GuildPosition"/> and <see cref="GuildMemberViewItem"/>, ordering by hierarchy:
/// GuildMaster (0) -> AssistantMaster (1) -> BattleMaster (2) -> NormalMember (3) -> others.
/// </summary>
public class GuildPositionComparer : IComparer<GuildPosition>, IComparer<GuildMemberViewItem>
{
    /// <summary>
    /// Gets the singleton instance of <see cref="GuildPositionComparer"/>.
    /// </summary>
    public static GuildPositionComparer Instance { get; } = new();

    /// <inheritdoc/>
    public int Compare(GuildPosition x, GuildPosition y)
    {
        return GetRank(x).CompareTo(GetRank(y));
    }

    /// <inheritdoc/>
    public int Compare(GuildMemberViewItem? x, GuildMemberViewItem? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (x is null)
        {
            return -1;
        }

        if (y is null)
        {
            return 1;
        }

        var positionComparison = this.Compare(x.Position, y.Position);
        if (positionComparison != 0)
        {
            return positionComparison;
        }

        return StringComparer.OrdinalIgnoreCase.Compare(x.CharacterName, y.CharacterName);
    }

    /// <summary>
    /// Gets the numerical rank for ordering purposes.
    /// </summary>
    /// <param name="position">The guild position.</param>
    /// <returns>A lower integer for higher-ranking positions.</returns>
    public static int GetRank(GuildPosition position)
    {
        return position switch
        {
            GuildPosition.GuildMaster => 0,
            GuildPosition.AssistantMaster => 1,
            GuildPosition.BattleMaster => 2,
            _ => 3,
        };
    }
}

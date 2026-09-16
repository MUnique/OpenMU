// <copyright file="GuildPositionComparer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// Comparer for <see cref="GuildPosition"/> and <see cref="GuildListEntry"/>, ordering by hierarchy:
/// guild master, assistant master, battle master, normal members, anything else; ties are broken by name.
/// </summary>
public sealed class GuildPositionComparer : IComparer<GuildPosition>, IComparer<GuildListEntry>
{
    /// <summary>
    /// Gets the shared instance of the <see cref="GuildPositionComparer"/>.
    /// </summary>
    public static GuildPositionComparer Instance { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildPositionComparer"/> class.
    /// </summary>
    private GuildPositionComparer()
    {
    }

    /// <inheritdoc/>
    public int Compare(GuildPosition x, GuildPosition y)
    {
        return GetRank(x).CompareTo(GetRank(y));
    }

    /// <inheritdoc/>
    public int Compare(GuildListEntry? x, GuildListEntry? y)
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

        var rankComparison = this.Compare(x.PlayerPosition, y.PlayerPosition);
        if (rankComparison != 0)
        {
            return rankComparison;
        }

        return StringComparer.OrdinalIgnoreCase.Compare(x.PlayerName, y.PlayerName);
    }

    /// <summary>
    /// Gets the numerical rank for ordering purposes. A lower value means a higher rank.
    /// </summary>
    /// <param name="position">The guild position.</param>
    /// <returns>The rank of the position.</returns>
    public static int GetRank(GuildPosition position)
    {
        return position switch
        {
            GuildPosition.GuildMaster => 0,
            GuildPosition.AssistantMaster => 1,
            GuildPosition.BattleMaster => 2,
            GuildPosition.NormalMember => 3,
            _ => 4,
        };
    }
}

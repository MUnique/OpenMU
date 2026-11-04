// <copyright file="GuildWarContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.GuildWar;

/// <summary>
/// Holds the information about an ongoing guild war.
/// </summary>
public class GuildWarContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GuildWarContext"/> class.
    /// </summary>
    /// <param name="warType">Type of the war.</param>
    /// <param name="score">The score.</param>
    /// <param name="team">The team.</param>
    /// <param name="requester">The requester.</param>
    public GuildWarContext(GuildWarType warType, GuildWarScore score, GuildWarTeam team, Player? requester)
    {
        this.WarType = warType;
        this.Score = score;
        this.Team = team;
        this.Requester = requester;
    }

    /// <summary>
    /// Gets the type of the war.
    /// </summary>
    public GuildWarType WarType { get; }

    /// <summary>
    /// Gets the score.
    /// </summary>
    public GuildWarScore Score { get; }

    /// <summary>
    /// Gets the team.
    /// </summary>
    public GuildWarTeam Team { get; }

    /// <summary>
    /// Gets the initial requester.
    /// </summary>
    public Player? Requester { get; }

    /// <summary>
    /// Gets or sets the state.
    /// </summary>
    public GuildWarState State { get; set; }

    /// <summary>
    /// Gets the name of the enemy team.
    /// </summary>
    public string EnemyTeamName => this.Team == GuildWarTeam.First ? this.Score.SecondGuildName : this.Score.FirstGuildName;

    /// <summary>
    /// Gets the score for this guild.
    /// </summary>
    public byte ThisScore => this.Team == GuildWarTeam.First ? this.Score.FirstGuildScore : this.Score.SecondGuildScore;

    /// <summary>
    /// Gets the score of the enemy guild.
    /// </summary>
    public byte EnemyScore => this.Team == GuildWarTeam.First ? this.Score.SecondGuildScore : this.Score.FirstGuildScore;
}
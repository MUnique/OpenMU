// <copyright file="GuildWarScore.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.GuildWar;

using System.ComponentModel;
using System.Runtime.CompilerServices;

/// <summary>
/// The score of a guild war.
/// </summary>
/// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
public class GuildWarScore : INotifyPropertyChanged
{
    private readonly object _syncRoot = new();

    private uint _firstGuildScore;

    private uint _secondGuildScore;

    private bool _endedEarly;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the name of the first guild.
    /// </summary>
    public string FirstGuildName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name of the second guild.
    /// </summary>
    public string SecondGuildName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the score of the first guild.
    /// </summary>
    public byte FirstGuildScore => (byte)Math.Min(byte.MaxValue, this._firstGuildScore);

    /// <summary>
    /// Gets the score of the second guild.
    /// </summary>
    public byte SecondGuildScore => (byte)Math.Min(byte.MaxValue, this._secondGuildScore);

    /// <summary>
    /// Gets a value indicating whether the guild war has ended.
    /// </summary>
    public bool HasEnded => this._endedEarly || this._firstGuildScore >= this.MaximumScore || this._secondGuildScore >= this.MaximumScore;

    /// <summary>
    /// Gets the maximum score.
    /// </summary>
    public byte MaximumScore { get; init; }

    /// <summary>
    /// Gets the winners of the guild war. It's <c>null</c> when the war ended without winners, see <see cref="Cancel"/>.
    /// </summary>
    public GuildWarTeam? Winners { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the time ran out with a draw, so that the next score decides the war.
    /// </summary>
    public bool IsSuddenDeath { get; private set; }

    /// <summary>
    /// Increases the score of the first guild.
    /// </summary>
    /// <param name="value">The value.</param>
    public void IncreaseFirstGuildScore(byte value = 1)
    {
        this.IncreaseScore(GuildWarTeam.First, value);
    }

    /// <summary>
    /// Increases the score of the second guild.
    /// </summary>
    /// <param name="value">The value.</param>
    public void IncreaseSecondGuildScore(byte value = 1)
    {
        this.IncreaseScore(GuildWarTeam.Second, value);
    }

    /// <summary>
    /// Ends the war because its time ran out: the guild with the higher score wins.
    /// On a draw, the war goes on in <see cref="IsSuddenDeath"/>, until the next score.
    /// </summary>
    public void EndByTime()
    {
        GuildWarTeam? winners;
        lock (this._syncRoot)
        {
            if (this.HasEnded || this.IsSuddenDeath)
            {
                return;
            }

            if (this._firstGuildScore == this._secondGuildScore)
            {
                this.IsSuddenDeath = true;
                winners = null;
            }
            else
            {
                winners = this._firstGuildScore > this._secondGuildScore ? GuildWarTeam.First : GuildWarTeam.Second;
                this._endedEarly = true;
                this.Winners = winners;
            }
        }

        if (winners is null)
        {
            this.RaisePropertyChanged(nameof(this.IsSuddenDeath));
        }
        else
        {
            this.RaiseEnded();
        }
    }

    /// <summary>
    /// Ends the war without winners, e.g. because nobody of both guilds is left on the battle field.
    /// </summary>
    public void Cancel()
    {
        lock (this._syncRoot)
        {
            if (this.HasEnded)
            {
                return;
            }

            this._endedEarly = true;
            this.Winners = null;
        }

        this.RaiseEnded();
    }

    private void IncreaseScore(GuildWarTeam team, byte value)
    {
        bool hasEnded;
        lock (this._syncRoot)
        {
            if (this.HasEnded)
            {
                return;
            }

            if (team == GuildWarTeam.First)
            {
                this._firstGuildScore += value;
            }
            else
            {
                this._secondGuildScore += value;
            }

            if (this.IsSuddenDeath)
            {
                this._endedEarly = true;
            }

            hasEnded = this.HasEnded;
            if (hasEnded)
            {
                this.Winners = team;
            }
        }

        this.RaisePropertyChanged(team == GuildWarTeam.First ? nameof(this.FirstGuildScore) : nameof(this.SecondGuildScore));
        if (hasEnded)
        {
            this.RaiseEnded();
        }
    }

    private void RaiseEnded()
    {
        this.RaisePropertyChanged(nameof(this.HasEnded));
        this.PropertyChanged = null;
    }

    private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

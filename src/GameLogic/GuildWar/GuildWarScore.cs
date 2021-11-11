// <copyright file="GuildWarScore.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.GuildWar;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

/// <summary>
/// The score of a guild war.
/// </summary>
/// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
public class GuildWarScore : INotifyPropertyChanged
{
    private uint _firstGuildScore;

    private uint _secondGuildScore;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the name of the first guild.
    /// </summary>
    public string FirstGuildName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the second guild.
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
    public bool HasEnded => this._firstGuildScore >= MaximumScore || this._secondGuildScore >= MaximumScore;

    /// <summary>
    /// Gets or sets the maximum score.
    /// </summary>
    public byte MaximumScore { get; init; }

    /// <summary>
    /// Gets the winners of the guild war.
    /// </summary>
    public GuildWarTeam? Winners { get; private set; }

    /// <summary>
    /// Increases the score of the first guild.
    /// </summary>
    /// <param name="value">The value.</param>
    public void IncreaseFirstGuildScore(byte value = 1)
    {
        if (!this.HasEnded)
        {
            Interlocked.Add(ref this._firstGuildScore, value);
            this.RaisePropertyChanged(nameof(this.FirstGuildScore));
            if (this.HasEnded)
            {
                this.Winners = GuildWarTeam.First;
                this.RaisePropertyChanged(nameof(this.HasEnded));
                this.PropertyChanged = null;
            }
        }
    }

    /// <summary>
    /// Increases the score of the first guild.
    /// </summary>
    /// <param name="value">The value.</param>
    public void IncreaseSecondGuildScore(byte value = 1)
    {
        if (!this.HasEnded)
        {
            Interlocked.Add(ref this._secondGuildScore, value);
            this.RaisePropertyChanged(nameof(this.SecondGuildScore));
            if (this.HasEnded)
            {
                this.Winners = GuildWarTeam.Second;
                this.RaisePropertyChanged(nameof(this.HasEnded));
                this.PropertyChanged = null;
            }
        }
    }

    private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
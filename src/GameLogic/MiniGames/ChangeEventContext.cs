// <copyright file="ChangeEventContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System.Threading;

/// <summary>
/// The context of the <see cref="MiniGameChangeEvent"/>.
/// </summary>
public sealed class ChangeEventContext
{
    private int _actualKills;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChangeEventContext"/> class.
    /// </summary>
    /// <param name="definition">The definition.</param>
    /// <param name="playerCount">The player count.</param>
    public ChangeEventContext(MiniGameChangeEvent definition, int playerCount)
    {
        this.Definition = definition;
        this.RequiredKills = definition.NumberOfKills;
        if (this.Definition.MultiplyKillsByPlayers)
        {
            this.RequiredKills *= playerCount;
        }
    }

    /// <summary>
    /// Gets the definition of the change event.
    /// </summary>
    public MiniGameChangeEvent Definition { get; }

    /// <summary>
    /// Gets the required kills.
    /// </summary>
    public int RequiredKills { get; }

    /// <summary>
    /// Gets the actual kills.
    /// </summary>
    public int ActualKills => this._actualKills;

    /// <summary>
    /// Registers a kill and returns if the target has been achieved.
    /// </summary>
    /// <returns>True, if the target has been achieved just right now.</returns>
    public bool RegisterKill()
    {
        if (this._actualKills == this.RequiredKills)
        {
            // Already achieved.
            return false;
        }

        return Interlocked.Increment(ref this._actualKills) == this.RequiredKills;
    }
}

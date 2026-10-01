// <copyright file="GensConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Gens;

using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// The configuration of the gens system.
/// </summary>
public class GensConfiguration
{
    /// <summary>
    /// Gets or sets the minimum level of a character to join a gens.
    /// </summary>
    public int MinimumLevel { get; set; } = 50;

    /// <summary>
    /// Gets or sets the contribution points which a character gets when it joins a gens.
    /// </summary>
    public int StartingContribution { get; set; } = 10;

    /// <summary>
    /// Gets or sets the rank which a character gets when it joins a gens.
    /// The game client knows the ranks 1 (highest, Grand Duke) to 14 (lowest, Private).
    /// </summary>
    public byte StartingRank { get; set; } = 14;

    /// <summary>
    /// Gets or sets the time which a character has to wait after leaving a gens, until it can join a gens again.
    /// </summary>
    public TimeSpan RejoinWaitTime { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Gets or sets the number of the npc of the Duprian gens.
    /// </summary>
    public short DuprianNpcNumber { get; set; } = 543;

    /// <summary>
    /// Gets or sets the number of the npc of the Vanert gens.
    /// </summary>
    public short VanertNpcNumber { get; set; } = 544;

    /// <summary>
    /// Gets the gens of the npc with the specified number.
    /// </summary>
    /// <param name="npcNumber">The number of the npc.</param>
    /// <returns>The gens of the npc; <see cref="GensType.None"/>, if it's not a gens npc.</returns>
    public GensType GetGensOfNpc(short npcNumber)
    {
        if (npcNumber == this.DuprianNpcNumber)
        {
            return GensType.Duprian;
        }

        if (npcNumber == this.VanertNpcNumber)
        {
            return GensType.Vanert;
        }

        return GensType.None;
    }
}

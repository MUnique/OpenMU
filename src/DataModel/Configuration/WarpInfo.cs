// <copyright file="WarpInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Defines a warp list entry.
/// </summary>
[Cloneable]
public partial class WarpInfo
{
    /// <summary>
    /// Gets or sets the index.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public LocalizedString Name { get; set; }

    /// <summary>
    /// Gets or sets the warp costs.
    /// </summary>
    public int Costs { get; set; }

    /// <summary>
    /// Gets or sets the level requirement which a character needs to fulfill so that it can warp to the <see cref="Gate"/>.
    /// </summary>
    public int LevelRequirement { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether players can use this warp.
    /// </summary>
    /// <remarks>
    /// When it's <c>false</c>, the warp is only available to game masters: it isn't sent to the warp list of
    /// the game client, and the warp request of a player is refused. It doesn't affect other ways to get to
    /// the map, e.g. walking or the move command of a game master. That's the case of the warps which the
    /// original client doesn't offer in its warp window, like Crywolf, which is reached by walking.
    /// </remarks>
    public bool IsWarpable { get; set; } = true;

    /// <summary>
    /// Gets or sets the gate.
    /// </summary>
    [Required]
    public virtual ExitGate? Gate { get; set; }

    /// <inheritdoc />
    public override string? ToString()
    {
        return this.Name.ToString();
    }
}
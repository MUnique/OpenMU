// <copyright file="AttackerSurrogate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Pet;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A surrogate for an <see cref="Player"/> which exposes the attack attributes
/// of another <see cref="IAttributeSystem"/>.
/// </summary>
public class AttackerSurrogate : IAttacker, IWorldObserver, IPlayerSurrogate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AttackerSurrogate" /> class.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="attributeSystem">The attribute system.</param>
    public AttackerSurrogate(Player owner, IAttributeSystem attributeSystem)
    {
        this.Owner = owner;
        this.Attributes = attributeSystem;
    }

    /// <inheritdoc />
    public Player Owner { get; }

    /// <inheritdoc />
    public ushort Id => this.Owner.Id;

    /// <inheritdoc />
    public GameMap? CurrentMap => this.Owner.CurrentMap;

    /// <inheritdoc />
    public Point Position
    {
        get => this.Owner.Position;
        set => this.Owner.Position = value;
    }

    /// <inheritdoc />
    public IAttributeSystem Attributes { get; }

    /// <inheritdoc />
    public ILogger Logger => this.Owner.Logger;

    /// <inheritdoc />
    public ICustomPlugInContainer<IViewPlugIn> ViewPlugIns => this.Owner.ViewPlugIns;
}
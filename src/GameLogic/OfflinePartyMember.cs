// <copyright file="OfflinePartyMember.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;

/// <summary>
/// A party member who is currently offline.
/// </summary>
public sealed class OfflinePartyMember : IPartyMember
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OfflinePartyMember"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public OfflinePartyMember(IPartyMember player)
    {
        this.Name = player.Name;
        this.Id = player.Id;
        this.CharacterId = player.CharacterId;
        this.CurrentHealth = player.CurrentHealth;
        this.MaximumHealth = player.MaximumHealth;
        this.CurrentMap = player.CurrentMap;
        this.Position = player.Position;
        this.Logger = (player as ILoggerOwner)?.Logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    }

    /// <inheritdoc />
    public Party? Party { get; set; }

    /// <inheritdoc />
    public IPartyMember? LastPartyRequester { get; set; }

    /// <inheritdoc />
    public uint MaximumHealth { get; }

    /// <inheritdoc />
    public uint CurrentHealth { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public Guid CharacterId { get; }

    /// <inheritdoc />
    public bool IsConnected => false;

    /// <inheritdoc />
    public ushort Id { get; }

    /// <inheritdoc />
    public Point Position { get; set; }

    /// <inheritdoc />
    public GameMap? CurrentMap { get; }

    /// <inheritdoc />
    public ICustomPlugInContainer<IViewPlugIn> ViewPlugIns { get; } = new DummyViewPlugInContainer();

    /// <inheritdoc />
    public AsyncReaderWriterLock ObserverLock { get; } = new();

    /// <inheritdoc />
    public ISet<IWorldObserver> Observers { get; } = new HashSet<IWorldObserver>();

    /// <inheritdoc />
    public ILogger Logger { get; }

    /// <inheritdoc />
    public ValueTask AddObserverAsync(IWorldObserver observer) => ValueTask.CompletedTask;

    /// <inheritdoc />
    public ValueTask RemoveObserverAsync(IWorldObserver observer) => ValueTask.CompletedTask;

    private class DummyViewPlugInContainer : ICustomPlugInContainer<IViewPlugIn>
    {
        public T? GetPlugIn<T>()
            where T : class, IViewPlugIn => null;
    }
}

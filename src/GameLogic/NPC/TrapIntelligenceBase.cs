// <copyright file="TrapIntelligenceBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

using MUnique.OpenMU.Pathfinding;
using System.Diagnostics;
using System.Threading;

/// <summary>
/// An abstract trap AI.
/// </summary>
public abstract class TrapIntelligenceBase : INpcIntelligence, IDisposable
{
    private Timer? _aiTimer;
    private Trap? _trap;

    /// <inheritdoc />
    public bool CanWalkOnSafezone => false;

    /// <summary>
    /// CanWalkOn?
    /// </summary>
    public virtual bool CanWalkOn(Point target) => false;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrapIntelligenceBase"/> class.
    /// </summary>
    /// <param name="map">The map.</param>
    protected TrapIntelligenceBase(GameMap map)
    {
        this.Map = map;
    }

    /// <summary>
    /// Gets or sets the trap.
    /// </summary>
    public Trap Trap
    {
        get => this._trap ?? throw new InvalidOperationException("Instance is not initialized with a Trap yet");
        set => this._trap = value;
    }

    /// <inheritdoc/>
    public NonPlayerCharacter Npc
    {
        get => this.Trap;
        set => this.Trap = (Trap)value;
    }

    /// <summary>
    /// Gets the map.
    /// </summary>
    protected GameMap Map { get; }

    /// <summary>
    /// Gets all possible targets.
    /// </summary>
    protected IEnumerable<IAttackable> PossibleTargets
    {
        get
        {
            List<IWorldObserver> tempObservers;
            using (this.Trap.ObserverLock.ReaderLock()) // todo: async?
            {
                tempObservers = new List<IWorldObserver>(this.Trap.Observers);
            }

            return tempObservers.OfType<IAttackable>();
        }
    }

    /// <inheritdoc/>
    public void RegisterHit(IAttacker attacker)
    {
        throw new NotImplementedException("A trap can't be attacked");
    }

    /// <inheritdoc/>
    public void Start()
    {
        var startDelay = this.Npc.Definition.AttackDelay + TimeSpan.FromMilliseconds(Rand.NextInt(0, 1000));
        this._aiTimer ??= new Timer(_ => this.SafeTick(), null, startDelay, this.Npc.Definition.AttackDelay);
    }

    /// <inheritdoc/>
    public void Pause()
    {
        this._aiTimer?.Dispose();
        this._aiTimer = null;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this._aiTimer?.Dispose();
        this._aiTimer = null;
    }

    /// <summary>
    /// Function which is executed in an interval.
    /// </summary>
    protected abstract ValueTask TickAsync();

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void SafeTick()
    {
        try
        {
            await this.TickAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
    }
}
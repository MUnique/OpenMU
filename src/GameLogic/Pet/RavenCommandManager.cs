// <copyright file="RavenCommandManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Pet;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views.Pet;
using Nito.AsyncEx;

/// <summary>
/// Implementation of the <see cref="IPetCommandManager"/> for the dark raven pet.
/// </summary>
public class RavenCommandManager : Disposable, IPetCommandManager
{
    private const int AttackRange = 7;
    private const int MaxRangeHits = 3;

    private readonly Player _owner;
    private readonly Item _pet;
    private readonly IAttacker _petAttackerSurrogate;
    private readonly List<IAttackable> _targetBuffer = new(MaxRangeHits);
    private readonly AsyncLock _rangeAttackLock = new();

    private CancellationTokenSource? _attackCts;
    private PetBehaviour _currentBehaviour;

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenCommandManager" /> class.
    /// </summary>
    /// <param name="owner">The owner of the pet.</param>
    /// <param name="pet">The pet.</param>
    public RavenCommandManager(Player owner, Item pet)
    {
        this._owner = owner;
        this._pet = pet;
        this._petAttackerSurrogate = new AttackerSurrogate(owner, new RavenAttributeSystem(owner));
    }

    private TimeSpan AttackDelay => TimeSpan.FromMilliseconds(Math.Max(100, 1500 - (this._petAttackerSurrogate.Attributes[Stats.AttackSpeed] * 10)));

    /// <summary>
    /// Sets the behaviour.
    /// </summary>
    /// <param name="newBehaviour">The new behaviour.</param>
    /// <param name="target">The target.</param>
    public async ValueTask SetBehaviourAsync(PetBehaviour newBehaviour, IAttackable? target)
    {
        await (this._attackCts?.CancelAsync() ?? Task.CompletedTask).ConfigureAwait(false);
        this._attackCts?.Dispose();
        this._attackCts = null;

        if (this._pet.Durability == 0.0)
        {
            this._currentBehaviour = PetBehaviour.Idle;
        }

        this._currentBehaviour = newBehaviour;

        await this._owner.InvokeViewPlugInAsync<IPetBehaviourChangedViewPlugIn>(p => p.PetBehaviourChanged(this._pet, this._currentBehaviour, target)).ConfigureAwait(false);
        if (newBehaviour == PetBehaviour.Idle)
        {
            return;
        }

        this._attackCts = new();
        switch (newBehaviour)
        {
            case PetBehaviour.AttackRandom:
                _ = this.AttackRandomAsync(this._attackCts.Token);
                break;
            case PetBehaviour.AttackTarget when target is { }:
                _ = this.AttackTargetUntilDeathAsync(target, this._attackCts.Token);
                break;
            case PetBehaviour.AttackWithOwner:
                _ = this.AttackSameAsOwnerAsync(this._attackCts.Token);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newBehaviour), $"Unknown behavior: {newBehaviour}");
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        this._attackCts?.Cancel();
        this._attackCts?.Dispose();
        this._attackCts = null;
    }

    private async ValueTask AttackAsync(IAttackable target, CancellationToken cancellationToken)
    {
        if (this._owner.Logger.IsEnabled(LogLevel.Debug))
        {
            this._owner.Logger.LogDebug($"Pet attacks target: {target}");
        }

        if (this._owner.IsAtSafezone())
        {
            return;
        }

        var attackType = Rand.NextRandomBool(0.3) ? PetAttackType.RangeAttack : PetAttackType.SingleTarget;

        await this._owner.ForEachWorldObserverAsync<IPetAttackViewPlugIn>(
            p => p.ShowPetAttackAnimation(this._owner, this._pet, target, attackType),
            true)
            .ConfigureAwait(false);

        if (attackType == PetAttackType.SingleTarget)
        {
            await target.AttackByAsync(this._petAttackerSurrogate, null, false).ConfigureAwait(false);
        }
        else
        {
            _ = this.RangeAttackAsync(target, cancellationToken);
        }
    }

    /// <summary>
    /// Does a range attack on the main target and other targets in the same area, with up to 3 hits.
    /// </summary>
    /// <param name="mainTarget">The main target which was selected to be attacked.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    private async ValueTask RangeAttackAsync(IAttackable mainTarget, CancellationToken cancellationToken)
    {
        try
        {
            using var l = await this._rangeAttackLock.LockAsync(cancellationToken);
            var delay = this.AttackDelay / 4;
            this._targetBuffer.Clear();
            this._targetBuffer.Add(mainTarget);
            var targets = mainTarget
                .CurrentMap!
                .GetAttackablesInRange(mainTarget.Position, 3)
                .OfType<AttackableNpcBase>()
                .Where(t => t != mainTarget)
                .Where(this.IsValidTarget)
                .Take(2);
            this._targetBuffer.AddRange(targets);
            for (int i = 0; i < 3; i++)
            {
                var target = this._targetBuffer[i % this._targetBuffer.Count];
                await this._owner.ForEachWorldObserverAsync<IPetAttackViewPlugIn>(
                        p => p.ShowPetAttackAnimation(this._owner, this._pet, target, PetAttackType.RangeAttack),
                        true)
                    .ConfigureAwait(false);

                await target.AttackByAsync(this._petAttackerSurrogate, null, false).ConfigureAwait(false);
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // We can ignore that.
        }
        catch (Exception ex)
        {
            this._owner.Logger.LogError(ex, "Unexpected error in range attack of the pet.");
        }
    }

    private async Task AttackTargetUntilDeathAsync(IAttackable target, CancellationToken cancellationToken)
    {
        this._owner.Logger.LogDebug($"Starting to attack {target} with the pet, until it's dead.");
        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!target.IsAlive)
                {
                    break;
                }

                if (!this._owner.IsAtSafezone() && this.IsValidTarget(target))
                {
                    await this.AttackAsync(target, cancellationToken).ConfigureAwait(false);
                }

                await Task.Delay(this.AttackDelay, cancellationToken).ConfigureAwait(false);
            }

            this._currentBehaviour = PetBehaviour.Idle;
            await this._owner.InvokeViewPlugInAsync<IPetBehaviourChangedViewPlugIn>(p => p.PetBehaviourChanged(this._pet, this._currentBehaviour, null)).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            this._owner.Logger.LogDebug("Cancelled attack with the pet.");
        }
        catch (Exception ex)
        {
            this._owner.Logger.LogError(ex, "Unexpected error in attack loop of the pet.");
        }

        this._owner.Logger.LogDebug("Ending attack with the pet.");
    }

    private async Task AttackRandomAsync(CancellationToken cancellationToken)
    {
        this._owner.Logger.LogDebug("Starting random attack with the pet.");
        try
        {
            IAttackable? currentTarget = null;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!this._owner.IsAtSafezone())
                {
                    if (!this.IsValidTarget(currentTarget))
                    {
                        var attackablesInRange = this._owner.CurrentMap?
                            .GetAttackablesInRange(this._owner.Position, AttackRange)
                            .OfType<AttackableNpcBase>()
                            .Where(this.IsValidTarget);
                        currentTarget = attackablesInRange?.SelectRandom();
                        if (this._owner.Logger.IsEnabled(LogLevel.Debug))
                        {
                            this._owner.Logger.LogDebug($"Pet selected target: {currentTarget}");
                        }
                    }

                    if (currentTarget is { IsAlive: true })
                    {
                        await this.AttackAsync(currentTarget, cancellationToken).ConfigureAwait(false);
                    }
                }

                await Task.Delay(this.AttackDelay, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            this._owner.Logger.LogDebug("Cancelled random attack with the pet.");
        }
        catch (Exception ex)
        {
            this._owner.Logger.LogError(ex, "Unexpected error in random attack loop of the pet.");
        }

        this._owner.Logger.LogDebug("Ending random attack with the pet.");
    }

    private async Task AttackSameAsOwnerAsync(CancellationToken cancellationToken)
    {
        this._owner.Logger.LogDebug("Starting attack with the pet.");
        try
        {
            IAttackable? currentTarget = null;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (this._owner.LastAttackedTarget.TryGetTarget(out var lastTarget)
                    && lastTarget != currentTarget)
                {
                    currentTarget = lastTarget;
                    if (this._owner.Logger.IsEnabled(LogLevel.Debug))
                    {
                        this._owner.Logger.LogDebug($"Pet selected last target of player: {currentTarget}");
                    }
                }

                if (this.IsValidTarget(currentTarget))
                {
                    await this.AttackAsync(currentTarget, cancellationToken).ConfigureAwait(false);
                }

                await Task.Delay(this.AttackDelay, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            this._owner.Logger.LogDebug("Cancelled attack with the pet.");
        }
        catch (Exception ex)
        {
            this._owner.Logger.LogError(ex, "Unexpected error in attack loop of the pet.");
        }

        this._owner.Logger.LogDebug("Ending attack with the pet.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsValidTarget([NotNullWhen(true)] IAttackable? target)
    {
        return target is not null 
               && target is not Monster { Definition.ObjectKind: NpcObjectKind.Guard }
               && target.IsActive()
               && target.IsInRange(this._owner.Position, AttackRange);
    }
}
// <copyright file="MagicEffectsList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// The list of magic effects of a player instance. Automatically applies the power-ups of the effects to the player.
/// </summary>
public class MagicEffectsList : AsyncDisposable
{
    private const byte InvisibleEffectStartIndex = 200;
    private readonly BitArray _contains = new(0x100);
    private readonly IAttackable _owner;
    private readonly Lock _sync = new();
    private readonly SortedList<short, MagicEffect> _activeEffects = new(6);

    /// <summary>
    /// Initializes a new instance of the <see cref="MagicEffectsList"/> class.
    /// </summary>
    /// <param name="owner">The attackable which owns this list.</param>
    public MagicEffectsList(IAttackable owner)
    {
        this._owner = owner;
    }

    /// <summary>
    /// Gets the active visible effect ids.
    /// </summary>
    public IList<MagicEffect> VisibleEffects
    {
        get
        {
            lock (this._sync)
            {
                return this._activeEffects.Values.Where(me => me.Definition.InformObservers).ToList();
            }
        }
    }

    /// <summary>
    /// Determines whether an effect with the specified identifier is active.
    /// </summary>
    /// <param name="effectId">The effect identifier.</param>
    /// <returns>True, if the effect is active.</returns>
    public bool ContainsEffect(short effectId)
    {
        lock (this._sync)
        {
            return this._activeEffects.ContainsKey(effectId);
        }
    }

    /// <summary>
    /// Determines whether any of the specified effects is active.
    /// </summary>
    /// <param name="effectIds">The effect identifiers.</param>
    /// <returns>True, if any of the effects is active.</returns>
    public bool ContainsAnyEffect(params short[] effectIds)
    {
        lock (this._sync)
        {
            for (int i = 0; i < effectIds.Length; i++)
            {
                if (this._activeEffects.ContainsKey(effectIds[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Determines whether the given magic effect is active.
    /// </summary>
    /// <param name="effectDefinition">The magic effect definition.</param>
    /// <returns>True, if the effect is active.</returns>
    public bool HasEffect(MagicEffectDefinition effectDefinition)
    {
        lock (this._sync)
        {
            var values = this._activeEffects.Values;
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].Definition == effectDefinition)
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Tries to get the active effect with the specified identifier.
    /// </summary>
    /// <param name="effectId">The effect identifier.</param>
    /// <param name="effect">The effect, if found.</param>
    /// <returns>True, if the effect is active.</returns>
    public bool TryGetEffect(short effectId, [NotNullWhen(true)] out MagicEffect? effect)
    {
        lock (this._sync)
        {
            return this._activeEffects.TryGetValue(effectId, out effect);
        }
    }

    /// <summary>
    /// Tries to get the active effect with the specified definition.
    /// </summary>
    /// <param name="effectDefinition">The magic effect definition.</param>
    /// <param name="effect">The effect, if found.</param>
    /// <returns>True, if the effect is active.</returns>
    public bool TryGetEffect(MagicEffectDefinition effectDefinition, [NotNullWhen(true)] out MagicEffect? effect)
    {
        lock (this._sync)
        {
            foreach (var candidate in this._activeEffects.Values)
            {
                if (candidate.Definition == effectDefinition)
                {
                    effect = candidate;
                    return true;
                }
            }

            effect = null;
            return false;
        }
    }

    /// <summary>
    /// Gets a snapshot of the active effects.
    /// </summary>
    /// <returns>The active effects at the time the snapshot was taken.</returns>
    public IReadOnlyList<MagicEffect> GetActiveEffectsSnapshot()
    {
        lock (this._sync)
        {
            return this._activeEffects.Values.ToList();
        }
    }

    /// <summary>
    /// Adds the effect and applies the power ups.
    /// </summary>
    /// <param name="effect">The effect.</param>
    public async ValueTask AddEffectAsync(MagicEffect effect)
    {
        bool added = false;
        lock (this._sync)
        {
            if (effect.IsDisposed || effect.IsDisposing)
            {
                // The effect is already dying before it was ever published:
                // there is nothing to track, and subscribing to the corpse
                // would leave a stuck entry behind. This narrows the race to
                // the guard-to-subscribe instruction gap; closing it fully
                // would need the lock on MagicEffect's side, which reads and
                // clears the event with no synchronization of its own.
                return;
            }

            if (this._contains[effect.Id])
            {
                added = this.UpdateEffect(effect);
            }
            else
            {
                added = true;
                this.AddFreshEffect(effect);
            }
        }

        if (added)
        {
            if (effect.Id < InvisibleEffectStartIndex && this._owner is IWorldObserver observer)
            {
                await observer.InvokeViewPlugInAsync<IActivateMagicEffectPlugIn>(p => p.ActivateMagicEffectAsync(effect, this._owner)).ConfigureAwait(false);
            }

            if (effect.Id < InvisibleEffectStartIndex && effect.Definition.InformObservers && this._owner is IObservable observable)
            {
                await observable.ForEachWorldObserverAsync<IActivateMagicEffectPlugIn>(p => p.ActivateMagicEffectAsync(effect, this._owner), false).ConfigureAwait(false);
            }
        }
        else
        {
            effect.Dispose();
        }
    }

    /// <summary>
    /// Clears all active effects.
    /// </summary>
    public async ValueTask ClearAllEffectsAsync()
    {
        while (true)
        {
            MagicEffect? first;
            lock (this._sync)
            {
                if (this._activeEffects.Count == 0)
                {
                    break;
                }

                first = this._activeEffects.Values[0];
            }

            await first.DisposeAsync().ConfigureAwait(false);

            // Disposal normally removes the entry through OnEffectTimeOutAsync.
            // If it didn't (the effect died before it was ever subscribed),
            // force the removal so we can't spin on the same element forever.
            List<MagicEffect.ElementWithTarget>? orphanedPowerUps = null;
            lock (this._sync)
            {
                if (this._activeEffects.Count > 0 && ReferenceEquals(this._activeEffects.Values[0], first))
                {
                    this._activeEffects.RemoveAt(0);
                    this._contains[first.Id] = false;
                    orphanedPowerUps = first.PowerUpElements.ToList();
                }
            }

            if (orphanedPowerUps is not null)
            {
                foreach (var powerUp in orphanedPowerUps)
                {
                    this._owner.Attributes.RemoveElement(powerUp.Element, powerUp.Target);
                }
            }
        }
    }

    /// <summary>
    /// Clear the effects that produce a specific stat.
    /// </summary>
    /// <param name="stat">The stat produced by effect.</param>
    public async ValueTask ClearAllEffectsProducingSpecificStatAsync(AttributeDefinition stat)
    {
        var effects = this.GetActiveEffectsSnapshot();

        foreach (var effect in effects)
        {
            if (effect.PowerUpElements.Any(p => p.Target == stat))
            {
                await effect.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Clears the effects after death of the player.
    /// </summary>
    public async ValueTask ClearEffectsAfterDeathAsync()
    {
        var effectsToRemove = this.GetActiveEffectsSnapshot().Where(effect => effect.Definition.StopByDeath).ToList();
        foreach (var effect in effectsToRemove)
        {
            await effect.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Tries to get the currently active effect of the specified <see cref="MagicEffectDefinition.SubType"/>.
    /// </summary>
    /// <param name="subType">The <see cref="MagicEffectDefinition.SubType"/>.</param>
    /// <returns>The effect, if found.</returns>
    public MagicEffect? TryGetActiveEffectOfSubType(byte subType)
    {
        lock (this._sync)
        {
            return this._activeEffects.Values.FirstOrDefault(e => e.Definition.SubType == subType);
        }
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        await this.ClearAllEffectsAsync().ConfigureAwait(false);
        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    private async ValueTask OnEffectTimeOutAsync(MagicEffect effect)
    {
        lock (this._sync)
        {
            // Identity-aware: the entry may have been replaced since this
            // effect started expiring (see UpdateEffect). Only tear down
            // what is ours — a replacement owns the id now, its power-ups
            // stay applied, and no deactivate is sent for it. Whoever
            // replaced or force-removed the entry already did its cleanup.
            if (!this._activeEffects.TryGetValue(effect.Id, out var current) || !ReferenceEquals(current, effect))
            {
                return;
            }

            this._activeEffects.Remove(effect.Id);
            this._contains[effect.Id] = false;
        }

        foreach (var powerUp in effect.PowerUpElements)
        {
            this._owner.Attributes.RemoveElement(powerUp.Element, powerUp.Target);
        }

        if (effect.Id >= InvisibleEffectStartIndex)
        {
            return;
        }

        (this._owner as IWorldObserver)?.InvokeViewPlugInAsync<IDeactivateMagicEffectPlugIn>(p => p.DeactivateMagicEffectAsync(effect, this._owner));
        if (effect.Definition.InformObservers && this._owner.IsAlive)
        {
            (this._owner as IObservable)?.ForEachWorldObserverAsync<IDeactivateMagicEffectPlugIn>(p => p.DeactivateMagicEffectAsync(effect, this._owner), false);
        }
    }

    /// <summary>
    /// Tracks a new effect which is not yet in the list.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <remarks>Caller must hold <see cref="_sync"/>.</remarks>
    private void AddFreshEffect(MagicEffect effect)
    {
        this._activeEffects.Add(effect.Id, effect);
        this._contains[effect.Id] = true;

        // Subscribe while still holding the lock, so a concurrently
        // firing expiry timer blocks on _sync until the add is fully
        // published and observes a subscribed entry.
        effect.EffectTimeOut += this.OnEffectTimeOutAsync;

        // The attribute changes stay inside the lock, mirroring the
        // previous AsyncLock scope: membership and applied power-ups
        // change atomically, so a concurrent expiry can't observe or
        // tear down a half-applied effect. The remove path only needs
        // the lock for the entry itself, because readers stop matching
        // the effect the moment it is gone.
        foreach (var powerUp in effect.PowerUpElements)
        {
            this._owner.Attributes.AddElement(powerUp.Element, powerUp.Target);
        }
    }

    /// <summary>
    /// Updates the effect.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <returns>True, if <paramref name="effect"/> is now tracked and needs activation; false, if it was merged into (or rejected by) the existing entry.</returns>
    /// <remarks>Caller must hold <see cref="_sync"/>.</remarks>
    private bool UpdateEffect(MagicEffect effect)
    {
        MagicEffect magicEffect = this._activeEffects[effect.Id];
        if (magicEffect.IsDisposed || magicEffect.IsDisposing)
        {
            // Stale corpse entry from a publish race: drop it with its
            // bookkeeping and track the fresh effect instead of calling
            // ResetTimer() on a dead timer, which would throw.
            this._activeEffects.Remove(magicEffect.Id);
            this._contains[magicEffect.Id] = false;
            foreach (var powerUp in magicEffect.PowerUpElements)
            {
                this._owner.Attributes.RemoveElement(powerUp.Element, powerUp.Target);
            }

            this.AddFreshEffect(effect);
            return true;
        }

        if (magicEffect.Value > effect.Value)
        {
            // no de-buffing allowed
            return false;
        }

        //// GMO behaviour would be: RemoveEffect(magicEffect.Id); AddEffectAsync(effect);
        //// I change the existing Timer and Buff Value, without removing the effect itself.
        //// This doesn't only save traffic, it also looks better in game.
        magicEffect.Duration = effect.Duration;
        magicEffect.ResetTimer();

        if (magicEffect.PowerUpElements.Select(e => e.Element)
            .SequenceEqual(effect.PowerUpElements.Select(e => e.Element)))
        {
            // if the effect power ups are the same, we can leave it like that
            return false;
        }

        foreach (var powerUp in magicEffect.PowerUpElements)
        {
            this._owner.Attributes.RemoveElement(powerUp.Element, powerUp.Target);
        }

        magicEffect.PowerUpElements = effect.PowerUpElements;
        foreach (var powerUp in magicEffect.PowerUpElements)
        {
            this._owner.Attributes.AddElement(powerUp.Element, powerUp.Target);
        }

        return false;
    }
}

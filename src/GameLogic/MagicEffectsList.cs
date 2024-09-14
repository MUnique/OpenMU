// <copyright file="MagicEffectsList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Collections;
using Nito.AsyncEx;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// The list of magic effects of a player instance. Automatically applies the power-ups of the effects to the player.
/// </summary>
public class MagicEffectsList : AsyncDisposable
{
    private const byte InvisibleEffectStartIndex = 200;
    private readonly BitArray _contains = new(0x100);
    private readonly IAttackable _owner;
    private readonly AsyncLock _addLock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MagicEffectsList"/> class.
    /// </summary>
    /// <param name="owner">The attackable which owns this list.</param>
    public MagicEffectsList(IAttackable owner)
    {
        this._owner = owner;
        this.ActiveEffects = new SortedList<short, MagicEffect>(6);
    }

    /// <summary>
    /// Gets the active effects.
    /// </summary>
    public IDictionary<short, MagicEffect> ActiveEffects { get; }

    /// <summary>
    /// Gets the active visible effect ids.
    /// </summary>
    public IList<MagicEffect> VisibleEffects => this.ActiveEffects.Values.Where(me => me.Definition.InformObservers).ToList();

    /// <summary>
    /// Adds the effect and applies the power ups.
    /// </summary>
    /// <param name="effect">The effect.</param>
    public async ValueTask AddEffectAsync(MagicEffect effect)
    {
        bool added = false;
        using (await this._addLock.LockAsync())
        {
            if (this._contains[effect.Id])
            {
                this.UpdateEffect(effect);
            }
            else
            {
                added = true;
                this.ActiveEffects.Add(effect.Id, effect);
                this._contains[effect.Id] = true;
                foreach (var powerUp in effect.PowerUpElements)
                {
                    this._owner.Attributes.AddElement(powerUp.Element, powerUp.Target);
                }
            }
        }

        if (added)
        {
            effect.EffectTimeOut += this.OnEffectTimeOutAsync;
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
        while (this.ActiveEffects.Any())
        {
            await this.ActiveEffects.Values.First().DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Clear the effects that produce a specific stat.
    /// </summary>
    /// <param name="stat">The stat produced by effect</param>
    public async ValueTask ClearAllEffectsProducingSpecificStatAsync(AttributeDefinition stat)
    {
        var effects = this.ActiveEffects.Values.ToArray();

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
        var effectsToRemove = this.ActiveEffects.Values.Where(effect => effect.Definition.StopByDeath).ToList();
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
    public async ValueTask<MagicEffect?> TryGetActiveEffectOfSubTypeAsync(byte subType)
    {
        using var l = await this._addLock.LockAsync();
        return this.ActiveEffects.Values.FirstOrDefault(e => e.Definition.SubType == subType);
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        await this.ClearAllEffectsAsync().ConfigureAwait(false);
        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    private async ValueTask OnEffectTimeOutAsync(MagicEffect effect)
    {
        using (await this._addLock.LockAsync())
        {
            this.ActiveEffects.Remove(effect.Id);
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
    /// Updates the effect.
    /// </summary>
    /// <param name="effect">The effect.</param>
    private void UpdateEffect(MagicEffect effect)
    {
        MagicEffect magicEffect = this.ActiveEffects[effect.Id];
        if (magicEffect.Value > effect.Value)
        {
            // no de-buffing allowed
            return;
        }

        //// GMO behaviour would be: RemoveEffect(magicEffect.Id); AddEffectAsync(effect);
        //// I change the existing Timer and Buff Value, without removing the effect itself.
        //// This doesn't only save traffic, it also looks better in game.
        magicEffect.Duration = effect.Duration;
        magicEffect.ResetTimer();
        foreach (var powerUp in magicEffect.PowerUpElements)
        {
            this._owner.Attributes.RemoveElement(powerUp.Element, powerUp.Target);
        }

        magicEffect.PowerUpElements = effect.PowerUpElements;
        foreach (var powerUp in magicEffect.PowerUpElements)
        {
            this._owner.Attributes.AddElement(powerUp.Element, powerUp.Target);
        }
    }
}
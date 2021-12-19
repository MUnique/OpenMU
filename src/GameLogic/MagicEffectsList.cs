// <copyright file="MagicEffectsList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// The list of magic effects of a player instance. Automatically applies the power-ups of the effects to the player.
/// </summary>
public class MagicEffectsList : Disposable
{
    private readonly BitArray _contains = new (0x100);
    private readonly IAttackable _owner;
    private readonly object _addLock = new ();

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
    public void AddEffect(MagicEffect effect)
    {
        bool added = false;
        lock (this._addLock)
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
            effect.EffectTimeOut += this.OnEffectTimeOut;
            (this._owner as Player)?.ViewPlugIns.GetPlugIn<IActivateMagicEffectPlugIn>()?.ActivateMagicEffect(effect, this._owner);
            if (effect.Definition.InformObservers)
            {
                (this._owner as IObservable)?.ForEachObservingPlayer(p => p.ViewPlugIns.GetPlugIn<IActivateMagicEffectPlugIn>()?.ActivateMagicEffect(effect, this._owner), false);
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
    public void ClearAllEffects()
    {
        while (this.ActiveEffects.Any())
        {
            this.ActiveEffects.Values.First().Dispose();
        }
    }

    /// <summary>
    /// Tries to get the currently active effect of the specified <see cref="MagicEffectDefinition.SubType"/>.
    /// </summary>
    /// <param name="subType">The <see cref="MagicEffectDefinition.SubType"/>.</param>
    /// <param name="effect">The effect, if found.</param>
    /// <returns><see langword="true"/>, if found.</returns>
    public bool TryGetActiveEffectOfSubType(byte subType, [MaybeNullWhen(false)] out MagicEffect effect)
    {
        lock (this._addLock)
        {
            effect = this.ActiveEffects.Values.FirstOrDefault(e => e.Definition.SubType == subType);
            return effect is not null;
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        this.ClearAllEffects();
    }

    private void OnEffectTimeOut(object? sender, EventArgs args)
    {
        var effect = sender as MagicEffect;
        if (effect is null)
        {
            return;
        }

        lock (this._addLock)
        {
            this.ActiveEffects.Remove(effect.Id);
            this._contains[effect.Id] = false;
        }

        foreach (var powerUp in effect.PowerUpElements)
        {
            this._owner.Attributes.RemoveElement(powerUp.Element, powerUp.Target);
        }

        (this._owner as Player)?.ViewPlugIns.GetPlugIn<IDeactivateMagicEffectPlugIn>()?.DeactivateMagicEffect(effect, this._owner);
        if (effect.Definition.InformObservers && this._owner.IsAlive)
        {
            (this._owner as IObservable)?.ForEachObservingPlayer(p => p.ViewPlugIns.GetPlugIn<IDeactivateMagicEffectPlugIn>()?.DeactivateMagicEffect(effect, this._owner), false);
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

        //// GMO behaviour would be: RemoveEffect(magicEffect.Id); AddEffect(effect);
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
// <copyright file="CrywolfEffectDisplay.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

using MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// Shows an effect of the crywolf event at an NPC to the players which observe it.
/// </summary>
/// <remarks>
/// The effects are sent separately, because the packet which adds the NPCs to the scope of a player
/// doesn't contain their effects. The effects which a player has seen are remembered, because the client
/// removes all effects of the event from an object, when it gets an effect which the object already has.
/// When the NPC gets out of the scope of the player, the client forgets its effects as well.
/// </remarks>
internal sealed class CrywolfEffectDisplay
{
    private readonly Dictionary<Player, CrywolfEffect> _shownEffects = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CrywolfEffectDisplay"/> class.
    /// </summary>
    /// <param name="npc">The NPC.</param>
    public CrywolfEffectDisplay(NonPlayerCharacter npc)
    {
        this.Npc = npc;
    }

    /// <summary>
    /// Gets the NPC.
    /// </summary>
    public NonPlayerCharacter Npc { get; }

    /// <summary>
    /// Gets or sets the effect which should be shown, or <c>null</c> if no effect should be shown.
    /// </summary>
    public CrywolfEffect? Effect { get; set; }

    /// <summary>
    /// Shows the current effect to the players which observe the NPC and haven't seen it yet.
    /// </summary>
    public async ValueTask UpdateAsync()
    {
        List<Player> observers;
        using (await this.Npc.ObserverLock.ReaderLockAsync())
        {
            observers = this.Npc.Observers.OfType<Player>().ToList();
        }

        foreach (var player in this._shownEffects.Keys.Except(observers).ToList())
        {
            this._shownEffects.Remove(player);
        }

        var effect = this.Effect;
        foreach (var player in observers)
        {
            var isShown = this._shownEffects.TryGetValue(player, out var shownEffect);
            if (isShown && shownEffect == effect)
            {
                continue;
            }

            if (isShown)
            {
                await player.InvokeViewPlugInAsync<ICrywolfEventViewPlugIn>(p => p.ShowEffectAsync(this.Npc, shownEffect, false)).ConfigureAwait(false);
                this._shownEffects.Remove(player);
            }

            if (effect is { } newEffect)
            {
                await player.InvokeViewPlugInAsync<ICrywolfEventViewPlugIn>(p => p.ShowEffectAsync(this.Npc, newEffect, true)).ConfigureAwait(false);
                this._shownEffects[player] = newEffect;
            }
        }
    }
}

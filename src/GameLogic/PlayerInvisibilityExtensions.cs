// <copyright file="PlayerInvisibilityExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Extensions to add and remove the invisibility effect of a <see cref="Player"/>.
/// </summary>
public static class PlayerInvisibilityExtensions
{
    /// <summary>
    /// Adds the invisible effect.
    /// </summary>
    /// <param name="player">The player.</param>
    public static async ValueTask AddInvisibleEffectAsync(this Player player)
    {
        var invisibleEffect = player.GameContext.Configuration.MagicEffects.FirstOrDefault(e => e.PowerUpDefinitions.Any(e => e.TargetAttribute == Stats.IsInvisible));
        if (invisibleEffect is null)
        {
            player.Logger.LogError("Invisible effect not found!");
        }
        else
        {
            var (duration, powerUps) = player.CreateMagicEffectPowerUp(invisibleEffect);
            var magicEffect = new MagicEffect(TimeSpan.FromSeconds(duration.Value), invisibleEffect, powerUps.Select(p => new MagicEffect.ElementWithTarget(p.BuffPowerUp, p.Target)).ToArray());
            await player.MagicEffectList.AddEffectAsync(magicEffect).ConfigureAwait(false);

            if (player.CurrentMap is { } currentMap)
            {
                await currentMap.RespawnAsync(player).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Removes the invisible effect.
    /// </summary>
    /// <param name="player">The player.</param>
    public static async ValueTask RemoveInvisibleEffectAsync(this Player player)
    {
        var invisibleEffect = player.GameContext.Configuration.MagicEffects.FirstOrDefault(e => e.PowerUpDefinitions.Any(e => e.TargetAttribute == Stats.IsInvisible));
        if (invisibleEffect is null)
        {
            return;
        }

        var activeEffect = player.MagicEffectList.ActiveEffects.Values.FirstOrDefault(e => e.Definition == invisibleEffect);
        if (activeEffect is null)
        {
            return;
        }

        await activeEffect.DisposeAsync().ConfigureAwait(false);
        if (player.CurrentMap is { } currentMap)
        {
            await currentMap.RespawnAsync(player).ConfigureAwait(false);
        }
    }
}

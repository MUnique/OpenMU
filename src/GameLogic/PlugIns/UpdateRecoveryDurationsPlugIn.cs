// <copyright file="UpdateRecoveryDurationsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Keeps the recovery duration attributes of the players up-to-date.
/// </summary>
/// <remarks>
/// The durations are the input of the recovery ramp bonuses (see <see cref="Stats.RestingRecoveryBonus"/>
/// and <see cref="Stats.ShieldRecoveryRampBonus"/>), which are calculated by attribute relationships.
/// That way, the recovery rates rise the longer a character rests or recovers its shield,
/// without any hard-coded steps in the game logic.
/// </remarks>
[PlugIn]
[Display(Name = "Update recovery durations", Description = "Updates the resting and shield recovery durations of the players, from which the recovery ramp bonuses are calculated.")]
[Guid("E189EFA3-E26F-409D-B75E-30A17C1691F8")]
public class UpdateRecoveryDurationsPlugIn : IPeriodicTaskPlugIn, IAttackableGotHitPlugIn
{
    private readonly ConditionalWeakTable<ItemAwareAttributeSystem, RecoveryDurations> _durations = new();

    private DateTime _lastExecution = DateTime.UtcNow;

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var now = DateTime.UtcNow;
        var elapsed = now - this._lastExecution;
        this._lastExecution = now;
        if (elapsed <= TimeSpan.Zero)
        {
            return;
        }

        var elapsedSeconds = (float)elapsed.TotalSeconds;
        var players = await gameContext.GetPlayersAsync().ConfigureAwait(false);
        foreach (var player in players)
        {
            try
            {
                this.UpdateDurations(player, elapsedSeconds);
            }
            catch (Exception ex)
            {
                player.Logger.LogError(ex, "Unexpected error when updating the recovery durations.");
            }
        }
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        // do nothing.
    }

    /// <inheritdoc />
    public void AttackableGotHit(IAttackable attackable, IAttacker attacker, HitInfo hitInfo)
    {
        if (attackable is Player { Attributes: { } attributes }
            && this._durations.TryGetValue(attributes, out var durations))
        {
            // Getting hit interrupts the shield recovery, so it has to ramp up again.
            durations.ShieldRecovery.Value = 0;
        }
    }

    private void UpdateDurations(Player player, float elapsedSeconds)
    {
        if (player.Attributes is not { } attributes
            || player.SelectedCharacter is null
            || player.PlayerState.CurrentState.IsDisconnectedOrFinished())
        {
            return;
        }

        // The flag is also updated when the player moves; we refresh it here as well, because the
        // shield recovery depends on it, and a player can enter or leave a safezone without moving itself.
        attributes.SetStatAttribute(Stats.IsInSafezone, player.IsAtSafezone() ? 1.0f : 0.0f);

        var durations = this._durations.GetValue(attributes, static system => new RecoveryDurations(system));

        durations.Resting.Value = attributes[Stats.IsResting] > 0
            ? Math.Min(durations.Resting.Value + elapsedSeconds, durations.RestingMaximum)
            : 0;

        var isShieldRecovering = attributes[Stats.ShieldRecoveryActive] >= 1
                                 && attributes[Stats.CurrentShield] < attributes[Stats.MaximumShield];
        durations.ShieldRecovery.Value = isShieldRecovering
            ? Math.Min(durations.ShieldRecovery.Value + elapsedSeconds, durations.ShieldRecoveryMaximum)
            : 0;
    }

    /// <summary>
    /// Holds the elements which provide the duration values of one attribute system.
    /// They are not persisted - they just live as long as the attribute system of the player does.
    /// </summary>
    private sealed class RecoveryDurations
    {
        private const float DefaultMaximumDuration = 60f;

        public RecoveryDurations(ItemAwareAttributeSystem attributeSystem)
        {
            (this.Resting, this.RestingMaximum) = AddDurationElement(attributeSystem, Stats.RestingDuration);
            (this.ShieldRecovery, this.ShieldRecoveryMaximum) = AddDurationElement(attributeSystem, Stats.ShieldRecoveryDuration);
        }

        public SimpleElement Resting { get; }

        public float RestingMaximum { get; }

        public SimpleElement ShieldRecovery { get; }

        public float ShieldRecoveryMaximum { get; }

        private static (SimpleElement Element, float Maximum) AddDurationElement(ItemAwareAttributeSystem attributeSystem, AttributeDefinition definition)
        {
            var element = new SimpleElement(0, AggregateType.AddRaw);
            var attribute = attributeSystem.GetComposableAttribute(definition);
            if (attribute is null)
            {
                attributeSystem.AddElement(element, definition);
            }
            else
            {
                attribute.AddElement(element);
            }

            // We keep the raw value within the limit of the definition, so that it doesn't grow indefinitely.
            var maximum = attribute?.Definition.MaximumValue ?? definition.MaximumValue ?? DefaultMaximumDuration;
            return (element, maximum);
        }
    }
}

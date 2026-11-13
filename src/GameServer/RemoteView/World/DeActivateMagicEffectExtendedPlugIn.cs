// <copyright file="DeActivateMagicEffectExtendedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Buffers;
using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The extended implementation of the <see cref="IActivateMagicEffectPlugIn"/>, <see cref="IDeactivateMagicEffectPlugIn"/>
/// and <see cref="IUpdateMagicEffectPlugIn"/>: the own player gets the values of its effects
/// (<see cref="MagicEffectStatusExtended"/>), at activation and whenever they change, and their time left
/// if their definition sends it (<see cref="DataModel.Configuration.MagicEffectDefinition.SendDuration"/>), as for the original clients.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DeActivateMagicEffectExtendedPlugIn_Name), Description = nameof(PlugInResources.DeActivateMagicEffectExtendedPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("6B19E506-66BF-4B7D-A2CA-1420880FF09A")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class DeActivateMagicEffectExtendedPlugIn : IActivateMagicEffectPlugIn, IDeactivateMagicEffectPlugIn, IUpdateMagicEffectPlugIn
{
    /// <summary>
    /// The most values which fit into the packet, whose length is a single byte; taken from the packet's layout, so a new field can't overflow it.
    /// </summary>
    internal static readonly int MaximumValues = (byte.MaxValue - MagicEffectStatusExtendedRef.GetRequiredSize(0)) / MagicEffectStatusExtendedRef.EffectValueRef.Length;

    /// <summary>
    /// The time an effect needs to have left, so that its owner gets an update: MuMain counts down the last second
    /// by itself, and an update so close to the end could arrive after the deactivation.
    /// It can't come from the configuration: it's a timing margin of this view's protocol with MuMain, not game data.
    /// </summary>
    private static readonly TimeSpan MinimumTimeLeftForUpdate = TimeSpan.FromSeconds(1);

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeActivateMagicEffectExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public DeActivateMagicEffectExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ActivateMagicEffectAsync(MagicEffect effect, IAttackable affectedObject)
    {
        if (!DeActivateMagicEffectPlugIn.TryGetConnection(this._player, effect, out var connection))
        {
            return;
        }

        if (affectedObject != this._player)
        {
            await DeActivateMagicEffectPlugIn.SendShortMagicEffectStatusAsync(connection, this._player, effect, affectedObject, true).ConfigureAwait(false);
            return;
        }

        // Clients which read the time of consumed items only from this packet keep getting it.
        await DeActivateMagicEffectPlugIn.SendEffectItemConsumptionAsync(connection, effect, effect.Definition.SendDuration ? effect.Duration : TimeSpan.Zero).ConfigureAwait(false);
        await SendExtendedStatusAsync(connection, effect, affectedObject.GetId(this._player), false).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask DeactivateMagicEffectAsync(MagicEffect effect, IAttackable affectedObject)
    {
        if (DeActivateMagicEffectPlugIn.TryGetConnection(this._player, effect, out var connection))
        {
            await DeActivateMagicEffectPlugIn.SendShortMagicEffectStatusAsync(connection, this._player, effect, affectedObject, false).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask UpdateMagicEffectAsync(MagicEffect effect, IAttackable affectedObject)
    {
        if (affectedObject != this._player
            || !DeActivateMagicEffectPlugIn.TryGetConnection(this._player, effect, out var connection))
        {
            return;
        }

        await SendExtendedStatusAsync(connection, effect, affectedObject.GetId(this._player), true).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the seconds until the effect ends, rounded up, as they are sent; 0 if the definition of the effect
    /// doesn't send its duration. Which effects show their time is game data: the admin decides it per effect.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <returns>The seconds until the effect ends, or 0.</returns>
    private static uint GetRemainingSeconds(MagicEffect effect)
    {
        if (!effect.Definition.SendDuration)
        {
            return 0;
        }

        return (uint)Math.Min(Math.Ceiling(effect.TimeLeft.TotalSeconds), uint.MaxValue);
    }

    private static ValueTask SendExtendedStatusAsync(IConnection connection, MagicEffect effect, ushort playerId, bool isUpdate)
    {
        return connection.SendAsync(() => WriteExtendedStatus(connection.Output, effect, playerId, isUpdate));
    }

    private static int WriteExtendedStatus(IBufferWriter<byte> output, MagicEffect effect, ushort playerId, bool isUpdate)
    {
        // Runs under the output lock and reads the time and the values here, so two updates
        // close together are written in the order of their values.
        if (effect.IsDisposing || effect.IsDisposed)
        {
            // The effect ends: its deactivation is (or will be) written under the same output lock,
            // because the disposal starts before it is sent. Writing the status now could show the effect again after it.
            return 0;
        }

        if (isUpdate && effect.TimeLeft < MinimumTimeLeftForUpdate)
        {
            // Its deactivation follows within the second. An activation is always sent, also for a short effect.
            return 0;
        }

        var values = effect.PowerUpElements as MagicEffect.ElementWithTarget[] ?? effect.PowerUpElements.ToArray();
        var valueCount = 0;
        for (var i = 0; i < values.Length; i++)
        {
            if (IsFirstOfItsAttribute(values, i))
            {
                valueCount++;
            }
        }

        valueCount = Math.Min(valueCount, MaximumValues);
        var size = MagicEffectStatusExtendedRef.GetRequiredSize(valueCount);
        var packet = new MagicEffectStatusExtendedRef(output.GetSpan(size)[..size])
        {
            IsActive = true,
            PlayerId = playerId,
            EffectId = (byte)effect.Id,
            RemainingSeconds = GetRemainingSeconds(effect),
            ValueCount = (byte)valueCount,
        };

        var written = 0;
        for (var i = 0; i < values.Length && written < valueCount; i++)
        {
            if (!IsFirstOfItsAttribute(values, i))
            {
                continue;
            }

            var value = packet[written];
            var (kind, finishedValue) = GetFinishedValue(values, i);
            values[i].Target.Id.TryWriteBytes(value.AttributeId, bigEndian: true, out _);
            value.Kind = kind;
            value.Value = finishedValue;
            written++;
        }

        return size;
    }

    /// <summary>
    /// Determines whether the value at <paramref name="index"/> is the first one of its attribute.
    /// </summary>
    /// <remarks>
    /// A scan instead of a grouping: an effect has a handful of values at most, and this way writing the packet allocates nothing.
    /// </remarks>
    /// <param name="values">The values of the effect.</param>
    /// <param name="index">The index of the value.</param>
    /// <returns><c>true</c>, if no value before it has the same attribute.</returns>
    private static bool IsFirstOfItsAttribute(MagicEffect.ElementWithTarget[] values, int index)
    {
        for (var i = 0; i < index; i++)
        {
            if (values[i].Target == values[index].Target)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Combines the values of the effect for the attribute of the value at <paramref name="first"/>, as the attribute system does,
    /// see <see cref="ElementAggregate"/>.
    /// </summary>
    /// <param name="values">The values of the effect.</param>
    /// <param name="first">The index of the first value of the attribute.</param>
    /// <returns>The kind and the finished value.</returns>
    private static (MagicEffectStatusExtended.ValueKind Kind, float Value) GetFinishedValue(MagicEffect.ElementWithTarget[] values, int first)
    {
        var target = values[first].Target;
        var aggregate = default(ElementAggregate);
        for (var i = first; i < values.Length; i++)
        {
            if (values[i].Target == target)
            {
                aggregate.Add(values[i].Element);
            }
        }

        var hasRaw = aggregate.Contains(AggregateType.AddRaw);
        var hasFinal = aggregate.Contains(AggregateType.AddFinal);
        var hasMaximum = aggregate.Contains(AggregateType.Maximum);
        var kind = (hasRaw, hasFinal, hasMaximum, aggregate.Contains(AggregateType.Multiplicate)) switch
        {
            (false, false, false, _) => MagicEffectStatusExtended.ValueKind.Multiplicate,
            (false, false, true, false) => MagicEffectStatusExtended.ValueKind.Maximum,
            _ when hasRaw || hasMaximum => MagicEffectStatusExtended.ValueKind.AddRaw,
            _ => MagicEffectStatusExtended.ValueKind.AddFinal,
        };

        // Limited to the attribute's maximum, e.g. Soul Barrier's damage decrement to 0.7.
        return (kind, aggregate.GetValue(target));
    }
}
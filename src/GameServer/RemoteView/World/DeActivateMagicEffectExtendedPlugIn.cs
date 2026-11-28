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

        // More values than fit are cut by whole attributes: the two parts of an attribute are only sent together.
        var valueCount = 0;
        for (var i = 0; i < values.Length; i++)
        {
            if (!IsFirstOfItsAttribute(values, i))
            {
                continue;
            }

            var count = GetValueCount(values, i);
            if (valueCount + count <= MaximumValues)
            {
                valueCount += count;
            }
        }

        var size = MagicEffectStatusExtendedRef.GetRequiredSize(valueCount);
        var packet = new MagicEffectStatusExtendedRef(output.GetSpan(size)[..size])
        {
            IsActive = true,
            PlayerId = playerId,
            EffectId = (byte)effect.Id,
            RemainingSeconds = GetRemainingSeconds(effect),
            ValueCount = (byte)valueCount,
        };

        // The same attributes as counted above: each one is written if it still fits as a whole.
        var written = 0;
        for (var i = 0; i < values.Length && written < valueCount; i++)
        {
            if (!IsFirstOfItsAttribute(values, i) || written + GetValueCount(values, i) > valueCount)
            {
                continue;
            }

            if (HasRawAndFinalParts(values, i))
            {
                // The final part first: MuMain's buff tooltip (since sven-n/MuMain#1212) reads only the first value of an
                // attribute, so it keeps showing Increase Block's bonus until it reads both.
                var (raw, final) = GetRawAndFinalParts(values, i);
                WriteValue(packet[written++], values[i].Target, MagicEffectStatusExtended.ValueKind.AddFinal, final);
                WriteValue(packet[written++], values[i].Target, MagicEffectStatusExtended.ValueKind.AddRaw, raw);
                continue;
            }

            var (kind, finishedValue) = GetFinishedValue(values, i);
            WriteValue(packet[written++], values[i].Target, kind, finishedValue);
        }

        return size;
    }

    private static int GetValueCount(MagicEffect.ElementWithTarget[] values, int first) => HasRawAndFinalParts(values, first) ? 2 : 1;

    private static void WriteValue(MagicEffectStatusExtendedRef.EffectValueRef value, AttributeDefinition attribute, MagicEffectStatusExtended.ValueKind kind, float finishedValue)
    {
        attribute.Id.TryWriteBytes(value.AttributeId, bigEndian: true, out _);
        value.Kind = kind;
        value.Value = finishedValue;
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
    /// Determines whether the effect adds values to the attribute of the value at <paramref name="first"/> both before and
    /// after the factors, which are then sent as two values, so a client can tell them apart. E.g. Increase Block with its
    /// master skill: the master skill's raw part is multiplied by the defense rate of the character class, the bonus isn't.
    /// </summary>
    /// <param name="values">The values of the effect.</param>
    /// <param name="first">The index of the first value of the attribute.</param>
    /// <returns><c>true</c>, if the effect has a value added before the factors (or a maximum) and one added after them.</returns>
    private static bool HasRawAndFinalParts(MagicEffect.ElementWithTarget[] values, int first)
    {
        var target = values[first].Target;
        var hasRaw = false;
        var hasFinal = false;
        for (var i = first; i < values.Length; i++)
        {
            if (values[i].Target != target)
            {
                continue;
            }

            switch (values[i].Element.AggregateType)
            {
                case AggregateType.AddRaw:
                case AggregateType.Maximum:
                    hasRaw = true;
                    break;
                case AggregateType.AddFinal:
                    hasFinal = true;
                    break;
                default:
                    // Factors belong to the raw part; they don't make a part of their own.
                    break;
            }
        }

        return hasRaw && hasFinal;
    }

    /// <summary>
    /// Combines the values of the effect for the attribute of the value at <paramref name="first"/> into its raw part
    /// (the values added before the factors, the maximum and the factors) and its final part (the values added after them),
    /// as the attribute system does. Together they are the effect's whole value, which the attribute's maximum limits once,
    /// as the attribute system does; the raw part keeps it on its own too, the final part is what is left of the whole.
    /// </summary>
    /// <param name="values">The values of the effect.</param>
    /// <param name="first">The index of the first value of the attribute.</param>
    /// <returns>The raw and the final part.</returns>
    private static (float Raw, float Final) GetRawAndFinalParts(MagicEffect.ElementWithTarget[] values, int first)
    {
        var target = values[first].Target;
        var all = default(ElementAggregate);
        var raw = default(ElementAggregate);
        var final = default(ElementAggregate);
        for (var i = first; i < values.Length; i++)
        {
            if (values[i].Target != target)
            {
                continue;
            }

            // Read once: the elements are live (e.g. 2 + the caster's energy / 10), so the parts and the whole come
            // from the same reading.
            var type = values[i].Element.AggregateType;
            var value = values[i].Element.Value;
            all.Add(type, value);
            if (type == AggregateType.AddFinal)
            {
                final.Add(type, value);
            }
            else
            {
                raw.Add(type, value);
            }
        }

        var total = all.GetValue(target);
        var rawValue = raw.GetValue(target);
        var finalValue = final.GetValue();

        // Unlimited, the whole is the sum of the parts as they are; else the final part is what the maximum leaves of it.
        // A negative final part counts too: a raw part of 1.0 and a final of -0.2 at a maximum of 0.7 is 0.7 in all.
        return (rawValue, total == rawValue + finalValue ? finalValue : total - rawValue);
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
// <copyright file="DeActivateMagicEffectExtendedPlugInTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.IO;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.GameServer.RemoteView.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// Tests for the <see cref="DeActivateMagicEffectExtendedPlugIn"/>, which tells the own player the time left and the values of its effects.
/// </summary>
[TestFixture]
public class DeActivateMagicEffectExtendedPlugInTests
{
    private const short GreaterDamage = 28;

    private readonly List<GameServer.RemoteView.RemotePlayer> _players = new();

    /// <summary>
    /// Disposes the players of the test.
    /// </summary>
    [TearDown]
    public async ValueTask DisposePlayersAsync()
    {
        foreach (var player in this._players)
        {
            await player.DisposeAsync().ConfigureAwait(false);
        }

        this._players.Clear();
    }

    /// <summary>
    /// The own player gets the effect with its time left and a value for each attribute it changes, found by the attribute id.
    /// </summary>
    [Test]
    public async ValueTask OwnEffectIsSentWithTimeAndValuesAsync()
    {
        var (player, output) = this.CreatePlayer();
        await using var effect = CreateEffect(GreaterDamage, TimeSpan.FromSeconds(60), 42.5f, 7f);

        await new DeActivateMagicEffectExtendedPlugIn(player).ActivateMagicEffectAsync(effect, player).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data, Has.Length.EqualTo(MagicEffectStatusExtendedRef.GetRequiredSize(2)));
        var packet = new MagicEffectStatusExtended(data);
        var sentValues = Enumerable.Range(0, packet.ValueCount).ToDictionary(i => new Guid(packet[i].AttributeId, bigEndian: true), i => packet[i].Value);
        Assert.Multiple(() =>
        {
            Assert.That(packet.Header.Code, Is.EqualTo(MagicEffectStatus.Code));
            Assert.That(packet.IsActive, Is.True);
            Assert.That(packet.PlayerId, Is.EqualTo(player.GetId(player)));
            Assert.That(packet.EffectId, Is.EqualTo((byte)GreaterDamage));
            Assert.That(packet.RemainingSeconds, Is.InRange(59u, 60u));
            Assert.That(packet.ValueCount, Is.EqualTo(2));
            foreach (var powerUp in effect.PowerUpElements)
            {
                Assert.That(sentValues, Contains.Key(powerUp.Target.Id).WithValue(powerUp.Element.Value));
            }

            for (var i = 0; i < packet.ValueCount; i++)
            {
                Assert.That(packet[i].Kind, Is.EqualTo(MagicEffectStatusExtended.ValueKind.AddRaw));
            }
        });

        // A client which only knows MagicEffectStatus reads the same first fields.
        var asOld = new MagicEffectStatus(data.AsMemory(0, MagicEffectStatus.Length));
        Assert.Multiple(() =>
        {
            Assert.That(asOld.IsActive, Is.True);
            Assert.That(asOld.PlayerId, Is.EqualTo(packet.PlayerId));
            Assert.That(asOld.EffectId, Is.EqualTo((byte)GreaterDamage));
        });
    }

    /// <summary>
    /// An effect whose definition doesn't send its duration comes without its time (0), but with its values:
    /// which effects show their time is up to the admin, as for the original clients.
    /// </summary>
    [Test]
    public async ValueTask EffectWithoutSendDurationComesWithoutTimeAsync()
    {
        var (player, output) = this.CreatePlayer();
        var definition = new Persistence.BasicModel.MagicEffectDefinition { Number = GreaterDamage, SendDuration = false };
        await using var effect = new MagicEffect(TimeSpan.FromSeconds(60), definition, new MagicEffect.ElementWithTarget(new ConstantElement(103f), Stats.GreaterDamageBonus));
        var plugIn = new DeActivateMagicEffectExtendedPlugIn(player);

        await plugIn.ActivateMagicEffectAsync(effect, player).ConfigureAwait(false);
        await plugIn.UpdateMagicEffectAsync(effect, player).ConfigureAwait(false);

        var data = output.ToArray();
        var size = MagicEffectStatusExtendedRef.GetRequiredSize(1);
        Assert.That(data, Has.Length.EqualTo(2 * size));
        var activation = new MagicEffectStatusExtended(data.AsMemory(0, size));
        var update = new MagicEffectStatusExtended(data.AsMemory(size));
        Assert.Multiple(() =>
        {
            Assert.That(activation.RemainingSeconds, Is.Zero);
            Assert.That(update.RemainingSeconds, Is.Zero);
            Assert.That(activation[0].Value, Is.EqualTo(103f));
            Assert.That(update[0].Value, Is.EqualTo(103f));
        });
    }

    /// <summary>
    /// An update after the effect was shortened (e.g. by a skill which decreases its timer) comes without its time, too,
    /// if the definition doesn't send its duration, but still with its values.
    /// </summary>
    [Test]
    public async ValueTask ShortenedEffectWithoutSendDurationComesWithoutTimeAsync()
    {
        var (player, output) = this.CreatePlayer();
        var definition = new Persistence.BasicModel.MagicEffectDefinition { Number = GreaterDamage, SendDuration = false };
        await using var effect = new MagicEffect(TimeSpan.FromSeconds(60), definition, new MagicEffect.ElementWithTarget(new ConstantElement(103f), Stats.GreaterDamageBonus));

        effect.DecreaseTimer(30);
        await new DeActivateMagicEffectExtendedPlugIn(player).UpdateMagicEffectAsync(effect, player).ConfigureAwait(false);

        var update = new MagicEffectStatusExtended(output.ToArray());
        Assert.Multiple(() =>
        {
            Assert.That(update.RemainingSeconds, Is.Zero);
            Assert.That(update.ValueCount, Is.EqualTo(1));
            Assert.That(update[0].Value, Is.EqualTo(103f));
        });
    }

    /// <summary>
    /// The effect of a consumed item whose definition doesn't send its duration gets its item packet with 0 seconds,
    /// as the base plug-in sends it to the original clients, and the extended status without time, but with its values.
    /// </summary>
    [Test]
    public async ValueTask ConsumedItemEffectWithoutSendDurationComesWithoutTimeAsync()
    {
        var (player, output) = this.CreatePlayer();
        var definition = new Persistence.BasicModel.MagicEffectDefinition
        {
            Number = 44,
            SendDuration = false,
            PowerUpDefinitions = { new Persistence.BasicModel.PowerUpDefinition { TargetAttribute = Stats.MaximumHealth } },
        };
        await using var effect = new MagicEffect(TimeSpan.FromMinutes(30), definition, new MagicEffect.ElementWithTarget(new ConstantElement(100), Stats.MaximumHealth));

        await new DeActivateMagicEffectExtendedPlugIn(player).ActivateMagicEffectAsync(effect, player).ConfigureAwait(false);

        var data = output.ToArray();
        var itemPacketLength = EffectItemConsumption.Length;
        Assert.That(data, Has.Length.EqualTo(itemPacketLength + MagicEffectStatusExtendedRef.GetRequiredSize(1)));
        var itemPacket = new EffectItemConsumption(data.AsMemory(0, itemPacketLength));
        var status = new MagicEffectStatusExtended(data.AsMemory(itemPacketLength));
        Assert.Multiple(() =>
        {
            Assert.That(itemPacket.RemainingSeconds, Is.Zero);
            Assert.That(status.RemainingSeconds, Is.Zero);
            Assert.That(status[0].Value, Is.EqualTo(100f));
        });
    }

    /// <summary>
    /// Greater Damage has two values on one attribute, the energy bonus and the factor of its master skills:
    /// the packet carries one finished value, as the attribute system combines them.
    /// </summary>
    [Test]
    public async ValueTask ValuesOfOneAttributeAreSentAsOneFinishedValueAsync()
    {
        var (player, output) = this.CreatePlayer();
        var masterFactor = new CombinedElement(new ConstantElement(1f, AggregateType.Multiplicate), new ConstantElement(0.32f, AggregateType.Multiplicate));
        await using var effect = CreateEffect(
            GreaterDamage,
            TimeSpan.FromSeconds(60),
            new MagicEffect.ElementWithTarget(new ConstantElement(103f), Stats.GreaterDamageBonus),
            new MagicEffect.ElementWithTarget(masterFactor, Stats.GreaterDamageBonus));

        await new DeActivateMagicEffectExtendedPlugIn(player).ActivateMagicEffectAsync(effect, player).ConfigureAwait(false);

        var packet = new MagicEffectStatusExtended(output.ToArray());
        Assert.Multiple(() =>
        {
            Assert.That(packet.ValueCount, Is.EqualTo(1));
            Assert.That(new Guid(packet[0].AttributeId, bigEndian: true), Is.EqualTo(Stats.GreaterDamageBonus.Id));
            Assert.That(packet[0].Kind, Is.EqualTo(MagicEffectStatusExtended.ValueKind.AddRaw));
            Assert.That(packet[0].Value, Is.EqualTo(103f * 1.32f).Within(0.001f));
        });
    }

    /// <summary>
    /// An attribute which the effect only changes by a factor gets the factor, marked as such.
    /// </summary>
    [Test]
    public async ValueTask FactorOnlyIsSentAsFactorAsync()
    {
        var (player, output) = this.CreatePlayer();
        await using var effect = CreateEffect(
            GreaterDamage,
            TimeSpan.FromSeconds(60),
            new MagicEffect.ElementWithTarget(new ConstantElement(1.1f, AggregateType.Multiplicate), Stats.MaximumHealth));

        await new DeActivateMagicEffectExtendedPlugIn(player).ActivateMagicEffectAsync(effect, player).ConfigureAwait(false);

        var packet = new MagicEffectStatusExtended(output.ToArray());
        Assert.Multiple(() =>
        {
            Assert.That(packet[0].Kind, Is.EqualTo(MagicEffectStatusExtended.ValueKind.Multiplicate));
            Assert.That(packet[0].Value, Is.EqualTo(1.1f));
        });
    }

    /// <summary>
    /// The kind of the sent value follows the kinds which the effect has for the attribute; several kinds without a value
    /// added both before and after the factors become one value, the effect's own: its factors only count for its own values.
    /// </summary>
    /// <param name="first">The aggregate type of the first value.</param>
    /// <param name="firstValue">The first value.</param>
    /// <param name="second">The aggregate type of the second value, or <c>null</c> for none.</param>
    /// <param name="secondValue">The second value.</param>
    /// <param name="expectedKind">The expected kind.</param>
    /// <param name="expectedValue">The expected value.</param>
    [TestCase(AggregateType.AddFinal, 50f, null, 0f, MagicEffectStatusExtended.ValueKind.AddFinal, 50f)]
    [TestCase(AggregateType.Maximum, 30f, null, 0f, MagicEffectStatusExtended.ValueKind.Maximum, 30f)]
    [TestCase(AggregateType.Maximum, 30f, AggregateType.Maximum, 40f, MagicEffectStatusExtended.ValueKind.Maximum, 40f)]
    [TestCase(AggregateType.Multiplicate, 1.1f, AggregateType.AddFinal, 50f, MagicEffectStatusExtended.ValueKind.AddFinal, 50f)]
    [TestCase(AggregateType.Maximum, 30f, AggregateType.Multiplicate, 2f, MagicEffectStatusExtended.ValueKind.AddRaw, 60f)]
    [TestCase(AggregateType.AddRaw, 10f, AggregateType.Multiplicate, 2f, MagicEffectStatusExtended.ValueKind.AddRaw, 20f)]
    public async ValueTask KindsOfOneAttributeBecomeOneValueAsync(AggregateType first, float firstValue, AggregateType? second, float secondValue, MagicEffectStatusExtended.ValueKind expectedKind, float expectedValue)
    {
        var (player, output) = this.CreatePlayer();
        var values = new List<MagicEffect.ElementWithTarget> { new(new ConstantElement(firstValue, first), Stats.MaximumHealth) };
        if (second is { } secondType)
        {
            values.Add(new(new ConstantElement(secondValue, secondType), Stats.MaximumHealth));
        }

        await using var effect = CreateEffect(GreaterDamage, TimeSpan.FromSeconds(60), values.ToArray());

        await new DeActivateMagicEffectExtendedPlugIn(player).ActivateMagicEffectAsync(effect, player).ConfigureAwait(false);

        var packet = new MagicEffectStatusExtended(output.ToArray());
        Assert.Multiple(() =>
        {
            Assert.That(packet.ValueCount, Is.EqualTo(1));
            Assert.That(packet[0].Kind, Is.EqualTo(expectedKind));
            Assert.That(packet[0].Value, Is.EqualTo(expectedValue).Within(0.001f));
        });
    }

    /// <summary>
    /// A value added before the factors and one added after them are sent as two values of the attribute, so the client can
    /// tell them apart: Increase Block with its master skill adds 0.44 (which the character class multiplies with the defense
    /// rate) and a bonus of 52 to the same attribute. The factors belong to the raw part. The final part comes first, which
    /// a client reading only the first value of an attribute shows.
    /// </summary>
    /// <param name="factor">A factor of the effect for the attribute, or <c>null</c> for none.</param>
    /// <param name="expectedRaw">The expected raw part.</param>
    [TestCase(null, 0.44f)]
    [TestCase(2f, 0.88f)]
    public async ValueTask RawAndFinalPartsAreSentAsTwoValuesAsync(float? factor, float expectedRaw)
    {
        var (player, output) = this.CreatePlayer();
        var values = new List<MagicEffect.ElementWithTarget>
        {
            new(new ConstantElement(52f, AggregateType.AddFinal), Stats.IncreaseBlockBonus),
            new(new ConstantElement(0.44f, AggregateType.AddRaw), Stats.IncreaseBlockBonus),
        };
        if (factor is { } factorValue)
        {
            values.Add(new(new ConstantElement(factorValue, AggregateType.Multiplicate), Stats.IncreaseBlockBonus));
        }

        await using var effect = CreateEffect(GreaterDamage, TimeSpan.FromSeconds(60), values.ToArray());

        await new DeActivateMagicEffectExtendedPlugIn(player).ActivateMagicEffectAsync(effect, player).ConfigureAwait(false);

        var packet = new MagicEffectStatusExtended(output.ToArray());
        Assert.Multiple(() =>
        {
            Assert.That(packet.ValueCount, Is.EqualTo(2));
            Assert.That(new Guid(packet[0].AttributeId, bigEndian: true), Is.EqualTo(Stats.IncreaseBlockBonus.Id));
            Assert.That(packet[0].Kind, Is.EqualTo(MagicEffectStatusExtended.ValueKind.AddFinal));
            Assert.That(packet[0].Value, Is.EqualTo(52f));
            Assert.That(new Guid(packet[1].AttributeId, bigEndian: true), Is.EqualTo(Stats.IncreaseBlockBonus.Id));
            Assert.That(packet[1].Kind, Is.EqualTo(MagicEffectStatusExtended.ValueKind.AddRaw));
            Assert.That(packet[1].Value, Is.EqualTo(expectedRaw).Within(0.0001f));
        });
    }

    /// <summary>
    /// The two parts of an attribute together are what the attribute system gives for the effect's values, limited by the
    /// attribute's maximum once; the raw part keeps the maximum on its own too, the final part is what is left.
    /// </summary>
    /// <param name="maximum">The attribute's maximum.</param>
    /// <param name="rawValue">The value added before the factors.</param>
    /// <param name="finalValue">The value added after the factors.</param>
    /// <param name="expectedRaw">The expected raw part.</param>
    /// <param name="expectedFinal">The expected final part.</param>
    [TestCase(30f, 20f, 15f, 20f, 10f)]
    [TestCase(30f, 40f, 5f, 30f, 0f)]
    [TestCase(0.7f, 1.0f, -0.2f, 0.7f, 0f)]
    [TestCase(0.7f, 0.5f, -0.2f, 0.5f, -0.2f)]
    public async ValueTask RawAndFinalPartsKeepTheMaximumTogetherAsync(float maximum, float rawValue, float finalValue, float expectedRaw, float expectedFinal)
    {
        var (player, output) = this.CreatePlayer();
        var limited = new AttributeDefinition(Guid.NewGuid(), "Limited", string.Empty) { MaximumValue = maximum };
        var values = new[]
        {
            new MagicEffect.ElementWithTarget(new ConstantElement(rawValue, AggregateType.AddRaw), limited),
            new MagicEffect.ElementWithTarget(new ConstantElement(finalValue, AggregateType.AddFinal), limited),
        };
        var expectedWhole = GetWholeValue(values, limited);
        await using var effect = CreateEffect(GreaterDamage, TimeSpan.FromSeconds(60), values);

        await new DeActivateMagicEffectExtendedPlugIn(player).ActivateMagicEffectAsync(effect, player).ConfigureAwait(false);

        var packet = new MagicEffectStatusExtended(output.ToArray());
        Assert.Multiple(() =>
        {
            Assert.That(packet.ValueCount, Is.EqualTo(2));
            Assert.That(packet[0].Kind, Is.EqualTo(MagicEffectStatusExtended.ValueKind.AddFinal));
            Assert.That(packet[0].Value, Is.EqualTo(expectedFinal).Within(0.0001f));
            Assert.That(packet[1].Kind, Is.EqualTo(MagicEffectStatusExtended.ValueKind.AddRaw));
            Assert.That(packet[1].Value, Is.EqualTo(expectedRaw).Within(0.0001f));
            Assert.That(packet[0].Value + packet[1].Value, Is.EqualTo(expectedWhole).Within(0.0001f));
        });
    }

    /// <summary>
    /// The value keeps the maximum of its attribute, as the attribute system does: Soul Barrier's own value is already limited
    /// to 0.7, but its Strengthener master skill adds to it in a combined element without that limit, e.g. 0.7 + 0.05.
    /// </summary>
    [Test]
    public async ValueTask ValueKeepsTheMaximumOfItsAttributeAsync()
    {
        var (player, output) = this.CreatePlayer();
        var decrement = new AttributeDefinition(Guid.NewGuid(), "Soul Barrier Damage Receive Decrement", string.Empty) { MaximumValue = 0.7f };
        await using var effect = CreateEffect(GreaterDamage, TimeSpan.FromSeconds(60), new MagicEffect.ElementWithTarget(new CombinedElement(new ConstantElement(0.7f), new ConstantElement(0.05f)), decrement));

        await new DeActivateMagicEffectExtendedPlugIn(player).ActivateMagicEffectAsync(effect, player).ConfigureAwait(false);

        var packet = new MagicEffectStatusExtended(output.ToArray());
        Assert.That(packet[0].Value, Is.EqualTo(0.7f));
    }

    /// <summary>
    /// The values are grouped by their attribute before they are cut to what fits: many values of a few attributes all count.
    /// </summary>
    [Test]
    public async ValueTask ValuesAreGroupedBeforeTheyAreCutAsync()
    {
        var (player, output) = this.CreatePlayer();
        var values = Enumerable.Range(0, DeActivateMagicEffectExtendedPlugIn.MaximumValues + 1)
            .Select(i => new MagicEffect.ElementWithTarget(new ConstantElement(1f), i % 2 == 0 ? Stats.MaximumHealth : Stats.MaximumMana))
            .ToArray();
        await using var effect = CreateEffect(GreaterDamage, TimeSpan.FromSeconds(60), values);

        await new DeActivateMagicEffectExtendedPlugIn(player).ActivateMagicEffectAsync(effect, player).ConfigureAwait(false);

        var packet = new MagicEffectStatusExtended(output.ToArray());
        var sentValues = Enumerable.Range(0, packet.ValueCount).ToDictionary(i => new Guid(packet[i].AttributeId, bigEndian: true), i => packet[i].Value);
        Assert.Multiple(() =>
        {
            Assert.That(packet.ValueCount, Is.EqualTo(2));
            Assert.That(sentValues, Contains.Key(Stats.MaximumHealth.Id).WithValue(values.Count(v => v.Target == Stats.MaximumHealth)));
            Assert.That(sentValues, Contains.Key(Stats.MaximumMana.Id).WithValue(values.Count(v => v.Target == Stats.MaximumMana)));
        });
    }

    /// <summary>
    /// The effect of a consumed item still comes with its time in <see cref="EffectItemConsumption"/>, before the extended status.
    /// </summary>
    [Test]
    public async ValueTask ConsumedItemEffectKeepsItsItemPacketFirstAsync()
    {
        var (player, output) = this.CreatePlayer();
        var definition = new Persistence.BasicModel.MagicEffectDefinition
        {
            Number = 44,
            SendDuration = true,
            PowerUpDefinitions = { new Persistence.BasicModel.PowerUpDefinition { TargetAttribute = Stats.MaximumHealth } },
        };
        await using var effect = new MagicEffect(TimeSpan.FromMinutes(30), definition, new MagicEffect.ElementWithTarget(new ConstantElement(100), Stats.MaximumHealth));

        await new DeActivateMagicEffectExtendedPlugIn(player).ActivateMagicEffectAsync(effect, player).ConfigureAwait(false);

        var data = output.ToArray();
        var itemPacketLength = EffectItemConsumption.Length;
        Assert.That(data, Has.Length.EqualTo(itemPacketLength + MagicEffectStatusExtendedRef.GetRequiredSize(1)));
        var itemPacket = new EffectItemConsumption(data.AsMemory(0, itemPacketLength));
        var status = new MagicEffectStatusExtended(data.AsMemory(itemPacketLength));
        Assert.Multiple(() =>
        {
            Assert.That(itemPacket.Header.Code, Is.EqualTo(EffectItemConsumption.Code));
            Assert.That(itemPacket.RemainingSeconds, Is.EqualTo(1800u));
            Assert.That(status.EffectId, Is.EqualTo(44));
            Assert.That(status[0].Value, Is.EqualTo(100f));
            Assert.That(new Guid(status[0].AttributeId, bigEndian: true), Is.EqualTo(Stats.MaximumHealth.Id));
        });
    }

    /// <summary>
    /// Effects of other players and every deactivation keep the short <see cref="MagicEffectStatus"/>.
    /// </summary>
    [Test]
    public async ValueTask OtherPlayersAndDeactivationGetTheShortStatusAsync()
    {
        var (player, output) = this.CreatePlayer();
        var (otherPlayer, _) = this.CreatePlayer();
        await using var effect = CreateEffect(GreaterDamage, TimeSpan.FromSeconds(60), 42.5f);
        var plugIn = new DeActivateMagicEffectExtendedPlugIn(player);

        await plugIn.ActivateMagicEffectAsync(effect, otherPlayer).ConfigureAwait(false);
        await plugIn.DeactivateMagicEffectAsync(effect, player).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data, Has.Length.EqualTo(2 * MagicEffectStatus.Length));
        var activated = new MagicEffectStatus(data.AsMemory(0, MagicEffectStatus.Length));
        var deactivated = new MagicEffectStatus(data.AsMemory(MagicEffectStatus.Length));
        Assert.Multiple(() =>
        {
            Assert.That(activated.IsActive, Is.True);
            Assert.That(deactivated.IsActive, Is.False);
            Assert.That(deactivated.EffectId, Is.EqualTo((byte)GreaterDamage));
        });
    }

    /// <summary>
    /// An update of the own effect sends its new time and values; an update of another player's effect sends nothing.
    /// </summary>
    [Test]
    public async ValueTask UpdateSendsTheOwnEffectOnlyAsync()
    {
        var (player, output) = this.CreatePlayer();
        var (otherPlayer, _) = this.CreatePlayer();
        await using var effect = CreateEffect(GreaterDamage, TimeSpan.FromSeconds(90), 12f);
        var plugIn = new DeActivateMagicEffectExtendedPlugIn(player);

        await plugIn.UpdateMagicEffectAsync(effect, otherPlayer).ConfigureAwait(false);
        Assert.That(output.ToArray(), Is.Empty);

        await plugIn.UpdateMagicEffectAsync(effect, player).ConfigureAwait(false);
        var packet = new MagicEffectStatusExtended(output.ToArray());
        Assert.Multiple(() =>
        {
            Assert.That(packet.RemainingSeconds, Is.InRange(89u, 90u));
            Assert.That(packet[0].Value, Is.EqualTo(12f));
        });
    }

    /// <summary>
    /// An update of an effect which ends meanwhile (e.g. removed by an antidote) writes nothing,
    /// so it can't show the effect again after its deactivation.
    /// </summary>
    [Test]
    public async ValueTask UpdateOfAnEndingEffectWritesNothingAsync()
    {
        var (player, output) = this.CreatePlayer();
        var effect = CreateEffect(GreaterDamage, TimeSpan.FromSeconds(90), 12f);
        await effect.DisposeAsync().ConfigureAwait(false);

        await new DeActivateMagicEffectExtendedPlugIn(player).UpdateMagicEffectAsync(effect, player).ConfigureAwait(false);

        Assert.That(output.ToArray(), Is.Empty);
    }

    /// <summary>
    /// An update which is written while the effect is being disposed (its deactivation is about to be sent) writes nothing.
    /// </summary>
    [Test]
    public async ValueTask UpdateDuringTheDisposalWritesNothingAsync()
    {
        var (player, output) = this.CreatePlayer();
        var plugIn = new DeActivateMagicEffectExtendedPlugIn(player);
        var effect = CreateEffect(GreaterDamage, TimeSpan.FromSeconds(90), 12f);
        var wasDisposing = false;
        effect.EffectTimeOut += async e =>
        {
            wasDisposing = e.IsDisposing;
            await plugIn.UpdateMagicEffectAsync(e, player).ConfigureAwait(false);
        };

        await effect.DisposeAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(wasDisposing, Is.True);
            Assert.That(output.ToArray(), Is.Empty);
        });
    }

    /// <summary>
    /// An update of an effect with less than a second left writes nothing: the client counts down the last second by itself,
    /// and the update could arrive after the deactivation. The activation of such a short effect is still sent.
    /// </summary>
    [Test]
    public async ValueTask UpdateUnderASecondBeforeTheEndWritesNothingAsync()
    {
        var (player, output) = this.CreatePlayer();
        var plugIn = new DeActivateMagicEffectExtendedPlugIn(player);
        await using var effect = CreateEffect(GreaterDamage, TimeSpan.FromSeconds(0.99), 12f);

        await plugIn.UpdateMagicEffectAsync(effect, player).ConfigureAwait(false);
        Assert.That(output.ToArray(), Is.Empty);

        await plugIn.ActivateMagicEffectAsync(effect, player).ConfigureAwait(false);
        Assert.That(output.ToArray(), Has.Length.EqualTo(MagicEffectStatusExtendedRef.GetRequiredSize(1)));
    }

    /// <summary>
    /// More values than fit into a packet of one length byte are cut.
    /// </summary>
    [Test]
    public async ValueTask ValuesAreCutToWhatFitsAsync()
    {
        var (player, output) = this.CreatePlayer();
        var values = Enumerable.Range(1, DeActivateMagicEffectExtendedPlugIn.MaximumValues + 5).Select(v => (float)v).ToArray();
        await using var effect = CreateEffect(GreaterDamage, TimeSpan.FromSeconds(60), values);

        await new DeActivateMagicEffectExtendedPlugIn(player).ActivateMagicEffectAsync(effect, player).ConfigureAwait(false);

        var data = output.ToArray();
        var packet = new MagicEffectStatusExtended(data);
        Assert.Multiple(() =>
        {
            Assert.That(data.Length, Is.LessThanOrEqualTo(byte.MaxValue));
            Assert.That(packet.Header.Length, Is.EqualTo(data.Length));
            Assert.That(packet.ValueCount, Is.EqualTo(DeActivateMagicEffectExtendedPlugIn.MaximumValues));
        });
    }

    /// <summary>
    /// The open source client (106.3) gets the extended plug-in for activations, deactivations and updates;
    /// the original client keeps the base plug-in and gets no updates.
    /// </summary>
    [Test]
    public async ValueTask OnlyTheOpenSourceClientGetsTheExtendedPlugInAsync()
    {
        var (openSourceClient, _) = this.CreatePlayerWithEffectPlugIns(new ClientVersion(106, 3, ClientLanguage.English));
        var (originalClient, _) = this.CreatePlayerWithEffectPlugIns(new ClientVersion(6, 3, ClientLanguage.English));

        Assert.Multiple(() =>
        {
            Assert.That(openSourceClient.ViewPlugIns.GetPlugIn<IActivateMagicEffectPlugIn>(), Is.InstanceOf<DeActivateMagicEffectExtendedPlugIn>());
            Assert.That(openSourceClient.ViewPlugIns.GetPlugIn<IDeactivateMagicEffectPlugIn>(), Is.InstanceOf<DeActivateMagicEffectExtendedPlugIn>());
            Assert.That(openSourceClient.ViewPlugIns.GetPlugIn<IUpdateMagicEffectPlugIn>(), Is.InstanceOf<DeActivateMagicEffectExtendedPlugIn>());
            Assert.That(originalClient.ViewPlugIns.GetPlugIn<IActivateMagicEffectPlugIn>(), Is.InstanceOf<DeActivateMagicEffectPlugIn>());
            Assert.That(originalClient.ViewPlugIns.GetPlugIn<IDeactivateMagicEffectPlugIn>(), Is.InstanceOf<DeActivateMagicEffectPlugIn>());
            Assert.That(originalClient.ViewPlugIns.GetPlugIn<IUpdateMagicEffectPlugIn>(), Is.Null);
        });
    }

    private (GameServer.RemoteView.RemotePlayer Player, MemoryStream Output) CreatePlayerWithEffectPlugIns(ClientVersion clientVersion)
    {
        var (player, output) = this.CreatePlayer(clientVersion);
        player.GameContext.PlugInManager.RegisterPlugIn<IViewPlugIn, DeActivateMagicEffectPlugIn>();
        player.GameContext.PlugInManager.RegisterPlugIn<IViewPlugIn, DeActivateMagicEffectExtendedPlugIn>();
        return (player, output);
    }

    private (GameServer.RemoteView.RemotePlayer Player, MemoryStream Output) CreatePlayer(ClientVersion clientVersion = default)
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer(clientVersion);
        this._players.Add(player);
        return (player, output);
    }

    private static float GetWholeValue(MagicEffect.ElementWithTarget[] values, AttributeDefinition attribute)
    {
        var whole = default(ElementAggregate);
        foreach (var value in values)
        {
            whole.Add(value.Element);
        }

        return whole.GetValue(attribute);
    }

    private static MagicEffect CreateEffect(short number, TimeSpan duration, params MagicEffect.ElementWithTarget[] values)
    {
        var definition = new Persistence.BasicModel.MagicEffectDefinition { Number = number, SendDuration = true };
        return new MagicEffect(duration, definition, values);
    }

    private static MagicEffect CreateEffect(short number, TimeSpan duration, params float[] values)
    {
        var definition = new Persistence.BasicModel.MagicEffectDefinition { Number = number, SendDuration = true };
        return new MagicEffect(duration, definition, values.Select(v => new MagicEffect.ElementWithTarget(new ConstantElement(v), new AttributeDefinition(Guid.NewGuid(), "Test", string.Empty))).ToArray());
    }
}
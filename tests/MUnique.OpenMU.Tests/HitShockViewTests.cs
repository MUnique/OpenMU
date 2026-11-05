// <copyright file="HitShockViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer.RemoteView.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using Moq;

/// <summary>
/// Tests hit-shock encoding in hit packets.
/// </summary>
[TestFixture]
public class HitShockViewTests
{
    private const ushort TargetId = 0x1234;

    /// <summary>
    /// The standard hit packet encodes the hit-shock flag in the high bit of the target id.
    /// </summary>
    /// <param name="triggersHitShock">Whether the hit is hit-shocking.</param>
    /// <param name="expectedTargetId">The encoded target id.</param>
    [TestCase(false, TargetId)]
    [TestCase(true, (ushort)(TargetId | 0x8000))]
    public async Task StandardHitPacketEncodesHitShockAsync(bool triggersHitShock, ushort expectedTargetId)
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var target = CreateTarget();

        await new ShowHitPlugIn(player)
            .ShowHitAsync(target.Object, new HitInfo(10, 0, DamageAttributes.Undefined, TriggersHitShock: triggersHitShock))
            .ConfigureAwait(false);

        var packet = new ObjectHitRef(output.ToArray());
        Assert.That(packet.ObjectId, Is.EqualTo(expectedTargetId));
    }

    /// <summary>
    /// The extended hit packet encodes the hit-shock flag in the high bit of the target id.
    /// </summary>
    /// <param name="triggersHitShock">Whether the hit is hit-shocking.</param>
    /// <param name="expectedTargetId">The encoded target id.</param>
    [TestCase(false, TargetId)]
    [TestCase(true, (ushort)(TargetId | 0x8000))]
    public async Task ExtendedHitPacketEncodesHitShockAsync(bool triggersHitShock, ushort expectedTargetId)
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var target = CreateTarget();

        await new ShowHitExtendedPlugIn(player)
            .ShowHitAsync(target.Object, new HitInfo(10, 0, DamageAttributes.Undefined, TriggersHitShock: triggersHitShock))
            .ConfigureAwait(false);

        var packet = new ObjectHitExtendedRef(output.ToArray());
        Assert.That(packet.ObjectId, Is.EqualTo(expectedTargetId));
    }

    private static Mock<IAttackable> CreateTarget()
    {
        var attributes = new Mock<IAttributeSystem>();
        attributes.Setup(a => a[It.IsAny<AttributeDefinition>()]).Returns(0);

        var target = new Mock<IAttackable>();
        target.Setup(t => t.Id).Returns(TargetId);
        target.Setup(t => t.Attributes).Returns(attributes.Object);
        return target;
    }
}

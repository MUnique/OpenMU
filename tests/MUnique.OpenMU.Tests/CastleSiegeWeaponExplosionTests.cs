// <copyright file="CastleSiegeWeaponExplosionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;
using MUnique.OpenMU.Network.Packets.ClientToServer;

/// <summary>
/// Tests the weapon explosion request of Castle Siege warfare machines.
/// </summary>
[TestFixture]
public class CastleSiegeWeaponExplosionTests
{
    /// <summary>
    /// Verifies the handler's packet group and subcode.
    /// </summary>
    [Test]
    public void HandlerUsesExpectedSubcode()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CastleSiegeMachineGroupHandlerPlugIn.GroupKey, Is.EqualTo(0xB7));
            Assert.That(new CastleSiegeWeaponExplosionHandlerPlugIn().Key, Is.EqualTo(0x04));
        });
    }

    /// <summary>
    /// Verifies that the request, including a truncated one, has no effect: the server applies the impact itself.
    /// </summary>
    /// <param name="empty">Whether the packet is empty instead of complete.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async ValueTask RequestHasNoEffectAsync(bool empty)
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var packet = new byte[empty ? 0 : WeaponExplosionRequest.Length];
        if (!empty)
        {
            _ = new WeaponExplosionRequest(packet) { CatapultId = 0x1234 };
        }

        await new CastleSiegeWeaponExplosionHandlerPlugIn().HandlePacketAsync(player, packet).ConfigureAwait(false);

        Assert.That(output.Length, Is.Zero);
    }
}

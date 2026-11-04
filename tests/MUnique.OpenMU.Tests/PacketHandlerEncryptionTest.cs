// <copyright file="PacketHandlerEncryptionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Reflection;
using MUnique.OpenMU.GameServer.MessageHandler;
using MUnique.OpenMU.Network.Packets.ClientToServer;

/// <summary>
/// Tests that the packet handlers only expect encryption for packets which are defined as encrypted.
/// </summary>
/// <remarks>
/// The <see cref="PacketHandlerPlugInContainer{THandler}"/> drops packets with a C1 or C2 header
/// when the handler expects encryption. Clients which send the packets as defined (C1 or C2), like the
/// open source client, would then never reach the handler.
/// </remarks>
[TestFixture]
public class PacketHandlerEncryptionTest
{
    /// <summary>
    /// Tests that each main packet handler which expects encryption handles a packet code
    /// which is defined with a C3 or C4 header.
    /// </summary>
    [Test]
    public void HandlersExpectEncryptionOnlyForEncryptedPackets()
    {
        var headerTypesByCode = typeof(MagicEffectCancelRequest).Assembly.GetTypes()
            .Where(t => t is { IsValueType: true, IsByRefLike: false } && t.Namespace == typeof(MagicEffectCancelRequest).Namespace)
            .Where(t => GetStaticByte(t, "Code") is not null && GetStaticByte(t, "HeaderType") is not null)
            .Where(t => GetStaticByte(t, "SubCode") is null)
            .GroupBy(t => GetStaticByte(t, "Code")!.Value)
            .ToDictionary(g => g.Key, g => g.Select(t => GetStaticByte(t, "HeaderType")!.Value).ToList());

        var handlers = typeof(IPacketHandlerPlugIn).Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IPacketHandlerPlugIn).IsAssignableFrom(t))
            .Where(t => t.GetConstructor(Type.EmptyTypes) is not null)
            .Select(t => (IPacketHandlerPlugIn)Activator.CreateInstance(t)!)
            .Where(handler => handler.IsEncryptionExpected);

        var violations = handlers
            .Where(handler => headerTypesByCode.TryGetValue(handler.Key, out var headerTypes) && headerTypes.All(headerType => headerType < 0xC3))
            .Select(handler => $"{handler.GetType().Name} (code {handler.Key:X2})")
            .ToList();

        Assert.That(violations, Is.Empty, "These handlers expect encryption, but their packets are defined with a C1 or C2 header.");
    }

    private static byte? GetStaticByte(Type type, string propertyName)
    {
        return type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as byte?;
    }
}

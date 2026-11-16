// <copyright file="GuildRelationshipChangeResultConversionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameServer.RemoteView.Guild;
using PacketResultType = MUnique.OpenMU.Network.Packets.ServerToClient.GuildRelationshipChangeResult.GuildRelationshipChangeResultType;
using ViewResultType = MUnique.OpenMU.GameLogic.Views.Guild.GuildRelationshipChangeResultType;

/// <summary>
/// Tests the conversion of the guild relationship change result to the packet value the client shows.
/// </summary>
[TestFixture]
public class GuildRelationshipChangeResultConversionTest
{
    /// <summary>
    /// A missing guild is sent as the generic failure, because the client reads its packet value as the castle siege case.
    /// </summary>
    [Test]
    public void GuildNotFoundIsSentAsFailed()
    {
        Assert.That(ViewResultType.GuildNotFound.Convert(), Is.EqualTo(PacketResultType.Failed));
    }

    /// <summary>
    /// The castle siege result keeps its packet value.
    /// </summary>
    [Test]
    public void FailedDuringCastleSiegeKeepsItsValue()
    {
        Assert.That(ViewResultType.FailedDuringCastleSiege.Convert(), Is.EqualTo(PacketResultType.FailedDuringCastleSiege));
    }
}

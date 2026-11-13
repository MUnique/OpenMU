// <copyright file="WarpListTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Globalization;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Properties;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.GameServer.RemoteView.World;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// Tests for sending the warp list of the server to the open source client.
/// </summary>
[TestFixture]
public class WarpListTests
{
    /// <summary>
    /// Tests that the values of the warps are written into the fields of the packet, with the name
    /// in the language of the player.
    /// </summary>
    [Test]
    public async Task WarpListIsWrittenAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        player.Culture = CultureInfo.GetCultureInfo("de");
        var view = new ShowWarpListExtendedPlugIn(player);
        WarpInfo[] warps =
        [
            CreateWarp(26, "Crywolf", 34, 190, 10000),
            CreateWarp(5, "Lost Tower||de=Verlorener Turm", 4, 50, 5000),
        ];

        await view.ShowWarpListAsync(warps).ConfigureAwait(false);

        var data = output.ToArray();
        var packet = new WarpListExtendedRef(data);
        Assert.That(data.Length, Is.EqualTo(WarpListExtendedRef.GetRequiredSize(2)));
        Assert.That(packet.Header.Type, Is.EqualTo(0xC2));
        Assert.That(packet.Header.Length, Is.EqualTo(data.Length));
        Assert.That(packet.Header.Code, Is.EqualTo(0xF5));
        Assert.That(packet.Header.SubCode, Is.EqualTo(0x09));
        Assert.That(packet.Count, Is.EqualTo(2));
        Assert.That(
            (packet[0].Index, packet[0].MapNumber, packet[0].LevelRequirement, packet[0].Costs, packet[0].Name),
            Is.EqualTo(((ushort)26, (ushort)34, (ushort)190, 10000u, "Crywolf")));
        Assert.That(
            (packet[1].Index, packet[1].MapNumber, packet[1].LevelRequirement, packet[1].Costs, packet[1].Name),
            Is.EqualTo(((ushort)5, (ushort)4, (ushort)50, 5000u, "Verlorener Turm")));
    }

    /// <summary>
    /// Tests that a name which doesn't fit into the name field is shortened to the whole characters which fit,
    /// instead of failing to write the packet.
    /// </summary>
    [Test]
    public void LongNameIsShortenedToWholeCharacters()
    {
        var thirtyOne = new string('a', 31);

        Assert.That(ShowWarpListExtendedPlugIn.FitIntoNameField("Lorencia"), Is.EqualTo("Lorencia"));
        Assert.That(ShowWarpListExtendedPlugIn.FitIntoNameField(new string('a', 32)), Is.EqualTo(new string('a', 32)));
        Assert.That(ShowWarpListExtendedPlugIn.FitIntoNameField(thirtyOne + "é"), Is.EqualTo(thirtyOne), "'é' takes two bytes, which don't fit anymore");
        Assert.That(ShowWarpListExtendedPlugIn.FitIntoNameField(new string('洛', 11)), Is.EqualTo(new string('洛', 10)), "each takes three bytes");
    }

    /// <summary>
    /// Tests that a long name doesn't break the packet.
    /// </summary>
    [Test]
    public async Task WarpListWithLongNameIsWrittenAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new ShowWarpListExtendedPlugIn(player);

        await view.ShowWarpListAsync([CreateWarp(1, new string('x', 40), 0, 10, 2000)]).ConfigureAwait(false);

        var packet = new WarpListExtendedRef(output.ToArray());
        Assert.That(packet[0].Name, Is.EqualTo(new string('x', 32)));
    }

    /// <summary>
    /// Tests that only the open source client gets the warp list.
    /// </summary>
    /// <param name="season">The season of the client version.</param>
    /// <param name="expected">If set to <c>true</c>, the client is expected to get the warp list.</param>
    [TestCase(6, false)]
    [TestCase(106, true)]
    public void OnlyTheOpenSourceClientGetsTheWarpList(byte season, bool expected)
    {
        var (player, _) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        player.GameContext.PlugInManager.RegisterPlugIn<IViewPlugIn, ShowWarpListExtendedPlugIn>();
        player.ClientVersion = new ClientVersion(season, 3, ClientLanguage.English);

        Assert.That(player.ViewPlugIns.GetPlugIn<IShowWarpListPlugIn>() is not null, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that the selectable warps are ordered by their index, that warps without a map are left out,
    /// and that of warps with the same index only the first one is included.
    /// </summary>
    [Test]
    public void SelectableWarps()
    {
        var first = CreateWarp(3, "First", 2, 10, 2000);
        var second = CreateWarp(3, "Second", 2, 10, 2000);
        var withoutMap = CreateWarp(1, "Without map", 0, 10, 2000);
        withoutMap.Gate!.Map = null;
        var lowerIndex = CreateWarp(2, "Lower index", 1, 10, 2000);

        var warps = ShowWarpListAfterEnterWorldPlugIn.GetSelectableWarps([first, withoutMap, second, lowerIndex]);

        Assert.That(warps, Is.EqualTo(new[] { lowerIndex, first }));
    }

    /// <summary>
    /// Tests that warps which players can't use are left out, also when they're the first one of their index.
    /// </summary>
    [Test]
    public void WarpsWhichArentWarpableAreLeftOut()
    {
        var warpable = CreateWarp(34, "Raklion", 57, 280, 15000);
        var notWarpable = CreateWarp(48, "LaCleon", 57, 280, 15000);
        notWarpable.IsWarpable = false;
        var notWarpableFirst = CreateWarp(5, "Not warpable first", 4, 10, 2000);
        notWarpableFirst.IsWarpable = false;
        var warpableSecond = CreateWarp(5, "Warpable second", 4, 10, 2000);

        var warps = ShowWarpListAfterEnterWorldPlugIn.GetSelectableWarps([warpable, notWarpable, notWarpableFirst, warpableSecond]);

        Assert.That(warps, Is.EqualTo(new[] { warpable }), "the warp request of index 5 would use the first one");
    }

    /// <summary>
    /// Tests that a player can't use a warp which isn't warpable: it's refused with a message, and the costs aren't paid.
    /// </summary>
    [Test]
    public async Task PlayerCantUseWarpWhichIsntWarpableAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Money = 100000;
        var warp = CreateWarp(26, "Crywolf", 0, 1, 10000);
        warp.IsWarpable = false;

        await new WarpAction().WarpToAsync(player, warp).ConfigureAwait(false);

        var message = player.GetLocalizedMessage(nameof(PlayerMessage.WarpOnlyForGameMasters));
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!);
        view.Verify(v => v.ShowMessageAsync(message, MessageType.BlueNormal), Times.Once);
        Assert.That(player.Money, Is.EqualTo(100000));
    }

    /// <summary>
    /// Tests that a game master isn't refused a warp which isn't warpable.
    /// </summary>
    [Test]
    public async Task GameMasterCanUseWarpWhichIsntWarpableAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.SelectedCharacter!.CharacterStatus = DataModel.Entities.CharacterStatus.GameMaster;
        var warp = CreateWarp(26, "Crywolf", 0, 1, 10000);
        warp.IsWarpable = false;

        await new WarpAction().WarpToAsync(player, warp).ConfigureAwait(false);

        var message = player.GetLocalizedMessage(nameof(PlayerMessage.WarpOnlyForGameMasters));
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!);
        view.Verify(v => v.ShowMessageAsync(message, It.IsAny<MessageType>()), Times.Never);
    }

    /// <summary>
    /// Tests that the warp list is shown when the character entered the game, and not when the player
    /// gets back into the world after a map change.
    /// </summary>
    /// <param name="previous">The name of the previous state.</param>
    /// <param name="expected">If set to <c>true</c>, the warp list is expected to be shown.</param>
    [TestCase(nameof(PlayerState.CharacterSelection), true)]
    [TestCase(nameof(PlayerState.ChangingMap), false)]
    public async Task WarpListIsShownAfterEnteringTheGameAsync(string previous, bool expected)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var warp = CreateWarp(26, "Crywolf", 34, 190, 10000);
        Mock.Get(player.GameContext.Configuration).Setup(c => c.WarpList).Returns(new List<WarpInfo> { warp });
        var previousState = (State)typeof(PlayerState).GetProperty(previous)!.GetValue(null)!;
        var plugIn = new ShowWarpListAfterEnterWorldPlugIn();

        await plugIn.PlayerStateChangedAsync(player, previousState, PlayerState.EnteredWorld).ConfigureAwait(false);

        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IShowWarpListPlugIn>()!);
        view.Verify(
            v => v.ShowWarpListAsync(It.Is<IReadOnlyCollection<WarpInfo>>(warps => warps.Single() == warp)),
            expected ? Times.Once() : Times.Never());
    }

    private static WarpInfo CreateWarp(int index, string name, short mapNumber, int level, int costs)
    {
        return new WarpInfo
        {
            Index = index,
            Name = name,
            LevelRequirement = level,
            Costs = costs,
            Gate = new ExitGate { Map = new Persistence.BasicModel.GameMapDefinition { Number = mapNumber } },
        };
    }
}

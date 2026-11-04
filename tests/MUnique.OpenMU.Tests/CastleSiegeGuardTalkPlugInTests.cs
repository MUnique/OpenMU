// <copyright file="CastleSiegeGuardTalkPlugInTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.GameServer.RemoteView.NPC;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;
using NpcWindowResponse = MUnique.OpenMU.Network.Packets.ServerToClient.NpcWindowResponse;

/// <summary>
/// Tests the talk plug-ins of the Castle Siege guards: the guardsmen at the bridge open the registration window,
/// the guard at the Land of Trials entrance opens the Land of Trials dialog.
/// </summary>
[TestFixture]
public class CastleSiegeGuardTalkPlugInTests
{
    /// <summary>
    /// Verifies the default NPCs: the guardsman (224) for the registration and the guard (220) for the Land of Trials.
    /// </summary>
    [Test]
    public void DefaultNpcs()
    {
        Assert.Multiple(() =>
        {
            Assert.That(new CastleSiegeRegistrationGuardsmanTalkPlugIn().DefaultNpcNumber, Is.EqualTo(224));
            Assert.That(new CastleSiegeLandOfTrialsGuardTalkPlugIn().DefaultNpcNumber, Is.EqualTo(220));
        });
    }

    /// <summary>
    /// Verifies that a guardsman on the Castle Siege map opens the registration window and keeps the dialog open.
    /// </summary>
    [Test]
    public async ValueTask GuardsmanOpensRegistrationWindowAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var eventArgs = new NpcTalkEventArgs();

        await fixture.PlugIn.PlayerTalksToNpcAsync(fixture.Player, fixture.Guardsman, eventArgs).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(eventArgs.HasBeenHandled, Is.True);
            Assert.That(eventArgs.LeavesDialogOpen, Is.True);
        });
        fixture.WindowView.Verify(view => view.OpenNpcWindowAsync(NpcWindow.CastleSiegeGuardsman), Times.Once);
    }

    /// <summary>
    /// Verifies that guardsmen on other maps, e.g. in Crywolf Fortress, aren't handled.
    /// </summary>
    [Test]
    public async ValueTask GuardsmanOnOtherMapIsNotHandledAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Player.SetCurrentMapSilently(new GameMap(new BasicModel.GameMapDefinition { Number = 34 }, TimeSpan.FromMinutes(1), 8));
        var eventArgs = new NpcTalkEventArgs();

        await fixture.PlugIn.PlayerTalksToNpcAsync(fixture.Player, fixture.Guardsman, eventArgs).ConfigureAwait(false);

        Assert.That(eventArgs.HasBeenHandled, Is.False);
        fixture.WindowView.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Verifies that the guardsman isn't handled when Castle Siege is disabled or not available.
    /// </summary>
    [Test]
    public async ValueTask GuardsmanWithoutActiveCastleSiegeIsNotHandledAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        fixture.Context.Configuration.Enabled = false;
        var disabledArgs = new NpcTalkEventArgs();
        var unavailableArgs = new NpcTalkEventArgs();
        var unavailablePlugIn = new CastleSiegeRegistrationGuardsmanTalkPlugIn(_ => null) { Configuration = fixture.PlugIn.Configuration };

        await fixture.PlugIn.PlayerTalksToNpcAsync(fixture.Player, fixture.Guardsman, disabledArgs).ConfigureAwait(false);
        await unavailablePlugIn.PlayerTalksToNpcAsync(fixture.Player, fixture.Guardsman, unavailableArgs).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(disabledArgs.HasBeenHandled, Is.False);
            Assert.That(unavailableArgs.HasBeenHandled, Is.False);
        });
        fixture.WindowView.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Verifies that the registration window is sent as NPC window 0x0D, on which MuMain requests the Castle Siege status.
    /// </summary>
    [Test]
    public async ValueTask SerializeRegistrationWindowAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();

        await new OpenNpcWindowPlugIn(player).OpenNpcWindowAsync(NpcWindow.CastleSiegeGuardsman).ConfigureAwait(false);

        var packet = (NpcWindowResponse)output.ToArray().AsMemory();
        Assert.That((byte)packet.Window, Is.EqualTo(0x0D));
    }

    private static async ValueTask<TestFixture> CreateFixtureAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var configuration = new BasicModel.CastleSiegeConfiguration
        {
            Enabled = true,
            CastleSiegeMapDefinition = gameContext.Configuration.Maps.First(),
        };
        configuration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry { State = CastleSiegeState.RegisterGuild, DayOfWeek = DayOfWeek.Monday });
        configuration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry { State = CastleSiegeState.RegisterMark, DayOfWeek = DayOfWeek.Tuesday });
        var context = new CastleSiegeContext(gameContext, configuration);
        var map = new GameMap(configuration.CastleSiegeMapDefinition, TimeSpan.FromMinutes(1), 8);

        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        player.SetCurrentMapSilently(map);

        var guardsmanDefinition = new BasicModel.MonsterDefinition { Number = CastleSiegeRegistrationGuardsmanTalkPlugIn.GuardsmanNumber };
        var guardsman = new NonPlayerCharacter(
            new BasicModel.MonsterSpawnArea { MonsterDefinition = guardsmanDefinition, GameMap = map.Definition },
            guardsmanDefinition,
            map);
        var plugIn = new CastleSiegeRegistrationGuardsmanTalkPlugIn(_ => context)
        {
            Configuration = new NpcTalkPlugInConfiguration { Npc = guardsmanDefinition },
        };

        var windowView = Mock.Get(player.ViewPlugIns.GetPlugIn<IOpenNpcWindowPlugIn>()!);
        return new(context, player, guardsman, plugIn, windowView);
    }

    private sealed record TestFixture(
        CastleSiegeContext Context,
        Player Player,
        NonPlayerCharacter Guardsman,
        CastleSiegeRegistrationGuardsmanTalkPlugIn PlugIn,
        Mock<IOpenNpcWindowPlugIn> WindowView);
}

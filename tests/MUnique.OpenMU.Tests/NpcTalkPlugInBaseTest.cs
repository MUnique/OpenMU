// <copyright file="NpcTalkPlugInBaseTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Resets;

/// <summary>
/// Tests for the <see cref="NpcTalkPlugInBase"/>.
/// </summary>
[TestFixture]
public class NpcTalkPlugInBaseTest
{
    private const short ConfiguredNpcNumber = 233;

    /// <summary>
    /// Tests that a plugin without a configured NPC doesn't handle its default NPC.
    /// </summary>
    [Test]
    public async ValueTask PlugInWithoutConfiguredNpcHandlesNothingAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = new GatekeeperNpcPlugin();
        var eventArgs = new NpcTalkEventArgs();

        await plugIn.PlayerTalksToNpcAsync(player, CreateNpc(player, plugIn.DefaultNpcNumber), eventArgs).ConfigureAwait(false);

        Assert.That(eventArgs.HasBeenHandled, Is.False);
    }

    /// <summary>
    /// Tests that the plugins handle their configured NPC and ignore their default NPC.
    /// </summary>
    /// <param name="plugInType">The type of the plugin.</param>
    [TestCase(typeof(GatekeeperNpcPlugin))]
    [TestCase(typeof(ResetCharacterNpcPlugin))]
    public async ValueTask PlugInHandlesConfiguredNpcAsync(Type plugInType)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = (NpcTalkPlugInBase)Activator.CreateInstance(plugInType)!;
        plugIn.Configuration = new NpcTalkPlugInConfiguration { Npc = new MonsterDefinition { Number = ConfiguredNpcNumber } };
        var defaultArgs = new NpcTalkEventArgs();
        var configuredArgs = new NpcTalkEventArgs();

        await plugIn.PlayerTalksToNpcAsync(player, CreateNpc(player, plugIn.DefaultNpcNumber), defaultArgs).ConfigureAwait(false);
        await plugIn.PlayerTalksToNpcAsync(player, CreateNpc(player, ConfiguredNpcNumber), configuredArgs).ConfigureAwait(false);

        Assert.That(defaultArgs.HasBeenHandled, Is.False);
        Assert.That(configuredArgs.HasBeenHandled, Is.True);
    }

    /// <summary>
    /// Tests that <see cref="NpcTalkPlugInBase.IsNpcOf{TPlugIn}"/> recognizes the configured NPC of an active plugin.
    /// </summary>
    [Test]
    public async ValueTask IsNpcOfRecognizesConfiguredNpcOfActivePlugInAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = new GatekeeperNpcPlugin
        {
            Configuration = new NpcTalkPlugInConfiguration { Npc = new MonsterDefinition { Number = ConfiguredNpcNumber } },
        };

        Assert.That(NpcTalkPlugInBase.IsNpcOf<GatekeeperNpcPlugin>(player.GameContext, CreateNpc(player, ConfiguredNpcNumber)), Is.False, "The plugin isn't active yet.");

        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerTalkToNpcPlugIn>(plugIn);

        Assert.That(NpcTalkPlugInBase.IsNpcOf<GatekeeperNpcPlugin>(player.GameContext, CreateNpc(player, ConfiguredNpcNumber)), Is.True);
        Assert.That(NpcTalkPlugInBase.IsNpcOf<GatekeeperNpcPlugin>(player.GameContext, CreateNpc(player, plugIn.DefaultNpcNumber)), Is.False);
        Assert.That(NpcTalkPlugInBase.IsNpcOf<ResetCharacterNpcPlugin>(player.GameContext, CreateNpc(player, ConfiguredNpcNumber)), Is.False);
        Assert.That(NpcTalkPlugInBase.IsNpcOf<GatekeeperNpcPlugin>(player.GameContext, null), Is.False);
    }

    /// <summary>
    /// Tests that the default configuration references the monster with the default NPC number of the plugin.
    /// </summary>
    [Test]
    public void DefaultConfigurationReferencesDefaultNpc()
    {
        var plugIn = new GatekeeperNpcPlugin();
        var gatekeeper = new MonsterDefinition { Number = plugIn.DefaultNpcNumber };
        var gameConfiguration = new Mock<GameConfiguration>();
        gameConfiguration.Setup(c => c.Monsters).Returns(new List<MonsterDefinition> { new() { Number = ConfiguredNpcNumber }, gatekeeper });

        var configuration = plugIn.CreateDefaultConfig(gameConfiguration.Object);

        Assert.That(configuration.Npc, Is.SameAs(gatekeeper));
    }

    private static NonPlayerCharacter CreateNpc(Player player, short number)
    {
        return new Mock<NonPlayerCharacter>(new MonsterSpawnArea(), new MonsterDefinition { Number = number }, player.CurrentMap!).Object;
    }
}

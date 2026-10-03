// <copyright file="TalkNpcNumberConfigurationTest.cs" company="MUnique">
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
/// Tests that the NPC talk plugins use their configured NPC numbers.
/// </summary>
[TestFixture]
public class TalkNpcNumberConfigurationTest
{
    private const short OtherNpcNumber = 233;

    /// <summary>
    /// Tests that the gatekeeper plugin handles its default NPC number without a configuration.
    /// </summary>
    [Test]
    public async ValueTask GatekeeperHandlesDefaultNumberAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var eventArgs = new NpcTalkEventArgs();

        await new GatekeeperNpcPlugin()
            .PlayerTalksToNpcAsync(player, CreateNpc(player, GatekeeperNpcPluginConfiguration.DefaultGatekeeperNumber), eventArgs)
            .ConfigureAwait(false);

        Assert.That(eventArgs.HasBeenHandled, Is.True);
    }

    /// <summary>
    /// Tests that the gatekeeper plugin uses the configured NPC number instead of the default one.
    /// </summary>
    [Test]
    public async ValueTask GatekeeperUsesConfiguredNumberAsync()
    {
        var plugIn = new GatekeeperNpcPlugin
        {
            Configuration = new GatekeeperNpcPluginConfiguration { GatekeeperNumber = OtherNpcNumber },
        };

        var (defaultHandled, configuredHandled) = await TalkToDefaultAndConfiguredAsync(plugIn, GatekeeperNpcPluginConfiguration.DefaultGatekeeperNumber).ConfigureAwait(false);

        Assert.That(defaultHandled, Is.False);
        Assert.That(configuredHandled, Is.True);
    }

    /// <summary>
    /// Tests that the reset plugin uses the configured NPC number instead of the default one.
    /// </summary>
    [Test]
    public async ValueTask ResetNpcUsesConfiguredNumberAsync()
    {
        var plugIn = new ResetCharacterNpcPlugin
        {
            Configuration = new ResetCharacterNpcPluginConfiguration { ResetNpcNumber = OtherNpcNumber },
        };

        var (defaultHandled, configuredHandled) = await TalkToDefaultAndConfiguredAsync(plugIn, ResetCharacterNpcPluginConfiguration.DefaultResetNpcNumber).ConfigureAwait(false);

        Assert.That(defaultHandled, Is.False);
        Assert.That(configuredHandled, Is.True);
    }

    private static async ValueTask<(bool DefaultHandled, bool ConfiguredHandled)> TalkToDefaultAndConfiguredAsync(IPlayerTalkToNpcPlugIn plugIn, short defaultNumber)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var defaultArgs = new NpcTalkEventArgs();
        var configuredArgs = new NpcTalkEventArgs();

        await plugIn.PlayerTalksToNpcAsync(player, CreateNpc(player, defaultNumber), defaultArgs).ConfigureAwait(false);
        await plugIn.PlayerTalksToNpcAsync(player, CreateNpc(player, OtherNpcNumber), configuredArgs).ConfigureAwait(false);

        return (defaultArgs.HasBeenHandled, configuredArgs.HasBeenHandled);
    }

    private static NonPlayerCharacter CreateNpc(Player player, short number)
    {
        return new Mock<NonPlayerCharacter>(new MonsterSpawnArea(), new MonsterDefinition { Number = number }, player.CurrentMap!).Object;
    }
}

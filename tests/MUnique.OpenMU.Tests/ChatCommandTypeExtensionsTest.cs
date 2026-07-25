// <copyright file="ChatCommandTypeExtensionsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for the <see cref="ChatCommandTypeExtensions"/>.
/// </summary>
[TestFixture]
public class ChatCommandTypeExtensionsTest
{
    private static readonly Guid ListCommandId = new("a5b0a3e5-bb2a-4287-821a-cd97714fe209");

    private static readonly Guid HelpCommandId = new("EFE9399A-9A14-4B94-BBC1-20718584C4C2");

    /// <summary>
    /// Tests that an active chat command plugin is listed as available.
    /// </summary>
    [Test]
    public async ValueTask ActiveCommandIsAvailableAsync()
    {
        var commands = await GetAvailableCommandsAsync(listCommandActive: true).ConfigureAwait(false);

        Assert.That(commands, Has.Member("/list"));
    }

    /// <summary>
    /// Tests that a deactivated chat command plugin is not listed as available.
    /// It couldn't be executed anyway, because the dispatching only considers active plugins.
    /// </summary>
    [Test]
    public async ValueTask DeactivatedCommandIsNotAvailableAsync()
    {
        var commands = await GetAvailableCommandsAsync(listCommandActive: false).ConfigureAwait(false);

        Assert.That(commands, Has.No.Member("/list"));

        // A sanity check, so that we know the list isn't just empty for another reason.
        Assert.That(commands, Has.Member("/help"));
    }

    private static async ValueTask<List<string>> GetAvailableCommandsAsync(bool listCommandActive)
    {
        var gameContext = GameContextTestHelper.CreateGameContext(
        [
            new PlugInConfiguration { TypeId = ListCommandId, IsActive = listCommandActive },
            new PlugInConfiguration { TypeId = HelpCommandId, IsActive = true },
        ]);

        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        player.SelectedCharacter!.CharacterStatus = CharacterStatus.GameMaster;

        return player.GetAvailableChatCommands().Select(command => command.Command).ToList();
    }
}

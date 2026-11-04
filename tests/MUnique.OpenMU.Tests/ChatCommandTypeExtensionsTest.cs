// <copyright file="ChatCommandTypeExtensionsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Globalization;
using System.Reflection;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.GameLogic.Resets;
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

    /// <summary>
    /// Tests that the parameters of a command are described with their short names,
    /// their requirement and their accepted values.
    /// </summary>
    [Test]
    public void CommandInfoDescribesParameters()
    {
        var info = ChatCommandTypeExtensions.TryCreateChatCommandInfo(typeof(ItemChatCommandPlugIn), CultureInfo.InvariantCulture);

        Assert.That(info, Is.Not.Null);
        Assert.That(info!.Command, Is.EqualTo("/item"));
        Assert.That(info.MinimumCharacterStatus, Is.EqualTo(CharacterStatus.GameMaster));

        var group = info.Parameters.FirstOrDefault(parameter => parameter.Name == nameof(ItemChatCommandArgs.Group));
        Assert.That(group, Is.Not.Null);
        Assert.That(group!.ShortName, Is.EqualTo("group"));
        Assert.That(group.IsRequired, Is.True);

        var level = info.Parameters.FirstOrDefault(parameter => parameter.Name == nameof(ItemChatCommandArgs.Level));
        Assert.That(level, Is.Not.Null);
        Assert.That(level!.ShortName, Is.EqualTo("lvl"));
        Assert.That(level.IsRequired, Is.False);

        var ancient = info.Parameters.FirstOrDefault(parameter => parameter.Name == nameof(ItemChatCommandArgs.Ancient));
        Assert.That(ancient, Is.Not.Null);
        Assert.That(ancient!.ValidValues, Is.EquivalentTo(new[] { "0", "1", "2" }));

        var skill = info.Parameters.FirstOrDefault(parameter => parameter.Name == nameof(ItemChatCommandArgs.Skill));
        Assert.That(skill, Is.Not.Null);
        Assert.That(skill!.ValidValues, Is.EquivalentTo(new[] { "0", "1" }));
    }

    /// <summary>
    /// Tests that name and description are resolved to their text, and not to the
    /// resource key which is defined by the display attribute.
    /// </summary>
    [Test]
    public void CommandInfoResolvesTextOfRequestedLanguage()
    {
        var info = ChatCommandTypeExtensions.TryCreateChatCommandInfo(typeof(ItemChatCommandPlugIn), CultureInfo.InvariantCulture);

        Assert.That(info, Is.Not.Null);
        Assert.That(info!.Name, Is.Not.Empty);
        Assert.That(info.Name, Does.Not.Contain("ItemChatCommandPlugIn_"));
        Assert.That(info.Description, Is.Not.Empty);
        Assert.That(info.Description, Does.Not.Contain("ItemChatCommandPlugIn_"));
    }

    /// <summary>
    /// Tests that the reset info command, whose texts were hard coded in English before,
    /// is described by its translatable resources.
    /// </summary>
    [Test]
    public void ResetInfoCommandIsDescribedByResources()
    {
        var info = ChatCommandTypeExtensions.TryCreateChatCommandInfo(typeof(ResetInfoChatCommandPlugIn), CultureInfo.InvariantCulture);

        Assert.That(info, Is.Not.Null);
        Assert.That(info!.Command, Is.EqualTo("/resetinfo"));
        Assert.That(info.Name, Is.Not.Empty);
        Assert.That(info.Name, Does.Not.Contain("ResetInfoChatCommandPlugIn_"));
        Assert.That(info.Description, Is.Not.Empty);
        Assert.That(info.Description, Does.Not.Contain("ResetInfoChatCommandPlugIn_"));
    }

    /// <summary>
    /// Tests that every chat command plugin can be described, so that none of them is
    /// missing from the command list, the help command or the admin panel.
    /// </summary>
    [Test]
    public void EveryChatCommandCanBeDescribed()
    {
        var commandTypes = GetChatCommandTypes();

        Assert.That(commandTypes, Is.Not.Empty);

        Assert.Multiple(() =>
        {
            foreach (var commandType in commandTypes)
            {
                var info = ChatCommandTypeExtensions.TryCreateChatCommandInfo(commandType, CultureInfo.InvariantCulture);
                Assert.That(info, Is.Not.Null, $"{commandType.Name} has no {nameof(ChatCommandHelpAttribute)}, so it's not listed anywhere.");
                Assert.That(info?.Name, Is.Not.Empty, $"{commandType.Name} has no name.");
                Assert.That(info?.Description, Is.Not.Empty, $"{commandType.Name} has no description.");
            }
        });
    }

    /// <summary>
    /// Tests that the command and the required character status of the help attribute are
    /// the ones the plugin actually uses. If they differ, a command is offered to players
    /// who can't execute it, or hidden from players who can.
    /// </summary>
    [Test]
    public void DescriptionMatchesWhatThePlugInRequires()
    {
        Assert.Multiple(() =>
        {
            foreach (var commandType in GetChatCommandTypes())
            {
                if (commandType.GetCustomAttribute<ChatCommandHelpAttribute>() is not { } help)
                {
                    continue;
                }

                var plugIn = (IChatCommandPlugIn)Activator.CreateInstance(commandType)!;
                Assert.That(help.Command, Is.EqualTo(plugIn.Key), $"{commandType.Name} is documented as '{help.Command}', but listens to '{plugIn.Key}'.");
                Assert.That(
                    help.MinimumCharacterStatus,
                    Is.EqualTo(plugIn.MinCharacterStatusRequirement),
                    $"{commandType.Name} is documented for {help.MinimumCharacterStatus}, but requires {plugIn.MinCharacterStatusRequirement}.");
            }
        });
    }

    /// <summary>
    /// Tests that a type which is no chat command plugin isn't described as one.
    /// </summary>
    [Test]
    public void NonCommandTypeIsNotDescribed()
    {
        Assert.That(ChatCommandTypeExtensions.TryCreateChatCommandInfo(typeof(ChatCommandTypeExtensionsTest)), Is.Null);
    }

    /// <summary>
    /// Tests that the described commands of a player are the same as its available
    /// commands, so that the two ways to get them can't drift apart.
    /// </summary>
    [Test]
    public async ValueTask CommandInfosMatchAvailableCommandsAsync()
    {
        var player = await CreateGameMasterAsync(listCommandActive: false).ConfigureAwait(false);

        var commands = player.GetAvailableChatCommands().Select(command => command.Command).ToList();
        var describedCommands = player.GetAvailableChatCommandInfos().Select(info => info.Command).ToList();

        Assert.That(describedCommands, Is.EquivalentTo(commands));
        Assert.That(describedCommands, Has.No.Member("/list"));
    }

    private static List<Type> GetChatCommandTypes()
    {
        return typeof(ListCommand).Assembly.GetTypes()
            .Where(type => type.GetCustomAttribute<PlugInAttribute>() is { })
            .Where(type => typeof(IChatCommandPlugIn).IsAssignableFrom(type))
            .ToList();
    }

    private static async ValueTask<List<string>> GetAvailableCommandsAsync(bool listCommandActive)
    {
        var player = await CreateGameMasterAsync(listCommandActive).ConfigureAwait(false);

        return player.GetAvailableChatCommands().Select(command => command.Command).ToList();
    }

    private static async ValueTask<Player> CreateGameMasterAsync(bool listCommandActive)
    {
        var gameContext = GameContextTestHelper.CreateGameContext(
        [
            new PlugInConfiguration { TypeId = ListCommandId, IsActive = listCommandActive },
            new PlugInConfiguration { TypeId = HelpCommandId, IsActive = true },
        ]);

        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        player.SelectedCharacter!.CharacterStatus = CharacterStatus.GameMaster;

        return player;
    }
}

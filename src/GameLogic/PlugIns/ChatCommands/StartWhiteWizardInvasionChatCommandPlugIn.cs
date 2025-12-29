// <copyright file="StartWhiteWizardInvasionChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the startww command.
/// Starts the (custom) White Wizard invasion at the next possible time.
/// </summary>
[Guid("55C53E9B-7D8B-4B83-9A87-0AC7A6B53E5E")]
[PlugIn(nameof(StartWhiteWizardInvasionChatCommandPlugIn), "Handles the chat command '/startww'. Starts the White Wizard invasion at the next possible time.")]
[ChatCommandHelp(Command, "Starts the White Wizard invasion at the next possible time.", CharacterStatus.GameMaster)]
public class StartWhiteWizardInvasionChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/startww";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc/>
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        // Get the periodic task plugin point container to enumerate actual instances
        var pluginPoint = player.GameContext.PlugInManager.GetPlugInPoint<IPeriodicTaskPlugIn>();
        if (pluginPoint is IPlugInContainer<IPeriodicTaskPlugIn> container)
        {
            var ww = container.ActivePlugIns.FirstOrDefault(p => p is WhiteWizardInvasionPlugIn);
            if (ww is object)
            {
                // ForceStart is defined on PeriodicTaskBasePlugIn; use reflection to avoid generic constraints
                ww.GetType().GetMethod("ForceStart")?.Invoke(ww, Array.Empty<object>());
                var message = player.GetLocalizedMessage("Chat_Message_WhiteWizardStarting", "White Wizard invasion will start shortly.");
                await player.ShowMessageAsync(message).ConfigureAwait(false);
                return;
            }
        }

        var notFound = player.GetLocalizedMessage("Chat_Message_WhiteWizardUnavailable", "White Wizard invasion plug-in not found or inactive.");
        await player.ShowMessageAsync(notFound).ConfigureAwait(false);
    }
}

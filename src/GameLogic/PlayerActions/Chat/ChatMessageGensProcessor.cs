// <copyright file="ChatMessageGensProcessor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Chat;

using System.ComponentModel;
using MUnique.OpenMU.GameLogic.Gens;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Chat message processor for the messages to the members of the own gens (prefix <c>$</c>).
/// The message reaches the members of the gens on the same game server.
/// </summary>
public class ChatMessageGensProcessor : BannableChatMessageBaseProcessor
{
    /// <inheritdoc/>
    public override async ValueTask SubclassProcessMessageAsync(Player sender, (string Message, string PlayerName) content)
    {
        var gens = sender.GetGens();
        if (gens == GensType.None || GensFeaturePlugIn.GetConfiguration(sender.GameContext) is null)
        {
            return;
        }

        var eventArgs = new CancelEventArgs();
        sender.GameContext.PlugInManager.GetPlugInPoint<IChatMessageReceivedPlugIn>()?.ChatMessageReceived(sender, content.Message, eventArgs);
        if (eventArgs.Cancel)
        {
            return;
        }

        var senderName = sender.SelectedCharacter!.Name;
        await sender.GameContext.ForEachPlayerAsync(player => player.GetGens() == gens
                ? player.InvokeViewPlugInAsync<IChatViewPlugIn>(p => p.ChatMessageAsync(content.Message, senderName, ChatMessageType.Gens)).AsTask()
                : Task.CompletedTask)
            .ConfigureAwait(false);
    }
}

// <copyright file="IPostMessageSentPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called after a player posted a message to the whole game server with
/// the <c>/post</c> command.
/// </summary>
/// <remarks>
/// A post goes out as a <see cref="Views.ChatMessageType.Gens"/> message, so it can't be told
/// apart from the gens chat through the <see cref="IChatMessageSentPlugIn"/>; this plugin point
/// reports the posts alone.
/// </remarks>
[Guid("CCBECE06-7FDA-42F2-BA16-97CF1ACD1EAF")]
[PlugInPoint("Post message sent", "Plugins which are called after a player posted a message to the whole game server with /post.")]
public interface IPostMessageSentPlugIn
{
    /// <summary>
    /// Is called after the player posted the message.
    /// </summary>
    /// <param name="sender">The posting player.</param>
    /// <param name="message">The posted message, without the command.</param>
    /// <returns>The task.</returns>
    ValueTask PostMessageSentAsync(Player sender, string message);
}

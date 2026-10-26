// <copyright file="GuildChatBridgeNoticePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tells players when they enter the game, if the chat of their guild or alliance is mirrored
/// to a channel of an external service like Discord. So nobody's chat is mirrored without knowing it.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.GuildChatBridgeNoticePlugIn_Name), Description = nameof(PlugInResources.GuildChatBridgeNoticePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("7FE145F7-54D4-4295-A43A-E4A812EF5F29")]
public class GuildChatBridgeNoticePlugIn : IPlayerStateChangedPlugIn
{
    /// <inheritdoc />
    public async ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
    {
        if (previousState != PlayerState.CharacterSelection
            || currentState != PlayerState.EnteredWorld
            || player.SelectedCharacter is not { } character)
        {
            return;
        }

        try
        {
            // The guild status of the player isn't known yet, so the membership is loaded from the database.
            using var guildContext = player.GameContext.PersistenceContextProvider.CreateNewGuildContext();
            if (await guildContext.GetGuildMembershipAsync(character.Name).ConfigureAwait(false) is not { } membership)
            {
                return;
            }

            using var context = player.GameContext.PersistenceContextProvider.CreateNewPlayerContext(player.GameContext.Configuration);
            var allianceId = membership.AllianceMasterGuildId ?? membership.GuildId;
            var bindings = await context.GetGuildChatBindingsAsync(membership.GuildId, allianceId).ConfigureAwait(false);
            foreach (var binding in bindings)
            {
                var message = binding.Scope == GuildChatScope.Guild ? nameof(PlayerMessage.GuildChatMirrored) : nameof(PlayerMessage.AllianceChatMirrored);
                await player.ShowLocalizedBlueMessageAsync(message).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            player.Logger.LogWarning(ex, "The bindings of the guild chat couldn't be checked.");
        }
    }
}

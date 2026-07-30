// <copyright file="PartyRequestHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MuHelper;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Handles auto-accepting incoming party requests from friends and guild members
/// based on the player's MU Helper settings flags.
/// </summary>
public static class PartyRequestHandler
{
    /// <summary>
    /// Automatically accepts a party request if the receiver has the relevant flag enabled.
    /// </summary>
    /// <param name="receiver">The player receiving the party request.</param>
    /// <param name="requester">The player who sent the party request.</param>
    /// <returns>True if the criteria matched (auto-accept was attempted, regardless of success); false if no criteria matched.</returns>
    public static async ValueTask<bool> TryAutoAcceptPartyRequestAsync(Player receiver, Player requester)
    {
        var settings = receiver.MuHelperSettings;
        if (settings is null)
        {
            return false;
        }

        if (settings.AutoAcceptGuild && AreGuildMembers(receiver, requester))
        {
            await AcceptPartyRequestAsync(receiver, requester).ConfigureAwait(false);
            return true;
        }

        if (settings.AutoAcceptFriend && await AreFriendsAsync(receiver, requester).ConfigureAwait(false))
        {
            await AcceptPartyRequestAsync(receiver, requester).ConfigureAwait(false);
            return true;
        }

        if (settings.AutoAcceptAnyone && await Bots.BotPartyHandler.TryScheduleAcceptAsync(receiver, requester).ConfigureAwait(false))
        {
            // The actual accept happens shortly afterwards in the bot's own tick (a human-like delay);
            // all bot-specific safeguards live in the handler, so this stays a thin criteria branch.
            return true;
        }

        return false;
    }

    private static bool AreGuildMembers(Player receiver, Player requester)
    {
        return receiver.GuildStatus?.GuildId != null
            && receiver.GuildStatus.GuildId == requester.GuildStatus?.GuildId;
    }

    private static async ValueTask<bool> AreFriendsAsync(Player receiver, Player requester)
    {
        if (receiver.SelectedCharacter is null || requester.SelectedCharacter is null)
        {
            return false;
        }

        var friendServer = (receiver.GameContext as IGameServerContext)?.FriendServer;
        if (friendServer is null)
        {
            return false;
        }

        return await friendServer.IsFriendAsync(receiver.SelectedCharacter.Name, requester.SelectedCharacter.Name).ConfigureAwait(false);
    }

    private static async ValueTask<bool> AcceptPartyRequestAsync(Player receiver, Player requester)
    {
        bool success = false;
        try
        {
            if (receiver.Party != null)
            {
                if (requester.Party == null)
                {
                    // Receiver is the offline party master; add the solo requester to the existing party.
                    success = await receiver.Party.AddAsync(requester).ConfigureAwait(false);
                }
            }
            else if (requester.Party != null)
            {
                // Requester already has a party; add the offline receiver to it.
                success = await requester.Party.AddAsync(receiver).ConfigureAwait(false);
            }
            else
            {
                // Neither side has a party; create a new one.
                var party = receiver.GameContext.PartyManager.CreateParty();
                success = await party.AddAsync(requester).ConfigureAwait(false)
                    && await party.AddAsync(receiver).ConfigureAwait(false);
            }
        }
        finally
        {
            await receiver.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
            receiver.LastPartyRequester = null;
        }

        return success;
    }
}

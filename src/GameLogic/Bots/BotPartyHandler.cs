// <copyright file="BotPartyHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// Lets a server-side bot party up with players who invite it (enabled by
/// <see cref="BotMuHelperSettings.AutoAcceptAnyone"/>): the invitation is accepted after a short
/// human-like delay, and the bot then follows the leader like any party member (see the follow logic
/// in <see cref="BotNavigator"/>). A bot stays in the party with its human companion for as long as
/// the party exists and the companion is reachable - a player who groups a bot keeps a companion for
/// the whole session, and the bot only leaves on the engine's own terms: the party disbands, it is
/// kicked, it can no longer legally follow the leader to another map, every human companion has been
/// disconnected without reconnecting for longer than the grace period, or before its own logout, as
/// when the presence rotation or a fault restart stops it. Safeguards keep it believable: no acceptance while
/// the bot is on an errand (shopping trip) or has unfinished business (revenge), and
/// the invitation is re-validated when the delay has passed - the inviter may have joined another
/// party or left. There is no level gate, matching OpenMU's own party action: it is the player who
/// invites.
/// </summary>
internal static class BotPartyHandler
{
    /// <summary>Lower bound of the human-like delay before the bot answers an invitation.</summary>
    private static readonly TimeSpan MinAcceptDelay = TimeSpan.FromSeconds(2);

    /// <summary>Upper bound of the human-like delay before the bot answers an invitation.</summary>
    private static readonly TimeSpan MaxAcceptDelay = TimeSpan.FromSeconds(5);

    /// <summary>
    /// How long a bot waits in a dead party before leaving (grace for reconnect/off-level).
    /// </summary>
    private static readonly TimeSpan PartyDisconnectGracePeriod = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Schedules the acceptance of a party invitation to a bot, if the bot is available for it.
    /// Called from the auto-accept criteria of <see cref="MuHelper.PartyRequestHandler"/>.
    /// </summary>
    /// <param name="receiver">The invited player; only server-side bots schedule an accept.</param>
    /// <param name="requester">The player who sent the party request.</param>
    /// <param name="acceptDelay">Overrides the human-like random delay (used by tests).</param>
    /// <returns>True, if the invitation was taken and will be answered; false, if no criteria matched.</returns>
    internal static async ValueTask<bool> TryScheduleAcceptAsync(Player receiver, Player requester, TimeSpan? acceptDelay = null)
    {
        if (receiver is not OfflinePlayer bot
            || bot.Account?.IsBot != true
            || HasHumanCompanion(bot)
            || bot.PendingPartyInvite is not null)
        {
            return false;
        }

        if (bot.IsOnShoppingTrip || bot.IsOnBuffTrip || bot.HasRevengeIntent || bot.CurrentMiniGame is not null)
        {
            // Busy - a player in the middle of an errand, a grudge or an event would not group up either.
            return false;
        }

        if (!IsRequesterEligible(requester))
        {
            return false;
        }

        var delay = acceptDelay
            ?? MinAcceptDelay + TimeSpan.FromMilliseconds(Rand.NextInt(0, (int)(MaxAcceptDelay - MinAcceptDelay).TotalMilliseconds + 1));

        // Blocks a second concurrent inviter (the request action treats a set requester like a busy
        // player) and is cleared again when the invitation is answered or dropped.
        bot.LastPartyRequester = requester;
        bot.PendingPartyInvite = new PendingPartyInvite(requester, DateTime.UtcNow + delay);

        // The same feedback a human invitee's request flow gives, so the inviter knows it went out.
        await requester.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.RequestedPlayerForParty), bot.Name).ConfigureAwait(false);
        bot.Logger.LogDebug("Bot '{Name}' accepts the party invitation of '{Requester}' in {Delay}.", bot.Name, requester.Name, delay);
        return true;
    }

    /// <summary>
    /// Drives the bot's party behavior; called from the bot's regular evaluation tick. Answers a
    /// pending invitation once its delay passed, and walks the bot out of a dead party once the
    /// disconnect grace period has elapsed.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <param name="utcNow">Overrides the current time used for the grace-period check (used by tests).</param>
    internal static async ValueTask ProcessAsync(OfflinePlayer bot, DateTime? utcNow = null)
    {
        var now = utcNow ?? DateTime.UtcNow;

        if (bot.PendingPartyInvite is { } invite && now >= invite.AcceptAtUtc)
        {
            bot.PendingPartyInvite = null;
            try
            {
                await AcceptInvitationAsync(bot, invite.Requester).ConfigureAwait(false);
            }
            finally
            {
                bot.LastPartyRequester = null;
            }
        }

        // No live human is left in the party: every human slot still present is an offline snapshot,
        // kept by the engine to reserve the spot for reconnection. Wait out the grace period (they may
        // reconnect or be off-leveling) before leaving. This is independent of who currently holds the
        // master slot, since mastership moves to a live bot when the previous master leaves properly.
        // A bot-only party carries no such snapshot, since bots kick themselves before stopping - which
        // is also the common case this loop must stay cheap for, so it avoids allocating an iterator or
        // sorting the list just to find out there is nothing to find.
        if (bot.Party is { } party && !HasHumanCompanion(bot))
        {
            DateTime? mostRecentDisconnect = null;
            string? mostRecentlyDisconnectedMemberName = null;
            foreach (var member in party.PartyList)
            {
                if (member is OfflinePartyMember snapshot
                    && (mostRecentDisconnect is null || snapshot.DisconnectedAtUtc > mostRecentDisconnect))
                {
                    mostRecentDisconnect = snapshot.DisconnectedAtUtc;
                    mostRecentlyDisconnectedMemberName = snapshot.Name;
                }
            }

            if (mostRecentDisconnect is null || now - mostRecentDisconnect < PartyDisconnectGracePeriod)
            {
                return;
            }

            bot.Logger.LogDebug(
                "Bot '{Name}' leaves the party of the disconnected player '{Player}'.",
                bot.Name,
                mostRecentlyDisconnectedMemberName);
            await party.KickMySelfAsync(bot).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Determines whether the bot's party contains a human player (any live member which is not a
    /// server-side <see cref="OfflinePlayer"/>).
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <returns>True, if a human player is in the bot's party.</returns>
    internal static bool HasHumanCompanion(Player bot)
    {
        return bot.Party is { } party
            && party.PartyList.OfType<Player>().Any(member => member is not OfflinePlayer);
    }

    private static async ValueTask AcceptInvitationAsync(OfflinePlayer bot, Player requester)
    {
        // Re-validate: between the invitation and this answer, the bot may have joined a human's party
        // and the inviter may have died, left the game or joined another party.
        if (HasHumanCompanion(bot) || !IsRequesterEligible(requester))
        {
            bot.Logger.LogDebug("Bot '{Name}' dropped the party invitation of '{Requester}' - the situation changed.", bot.Name, requester.Name);
            return;
        }

        await LeaveBotPartyAsync(bot).ConfigureAwait(false);
        if (bot.Party is not null)
        {
            bot.Logger.LogDebug("Bot '{Name}' could not leave its bot party for '{Requester}'.", bot.Name, requester.Name);
            return;
        }

        bool success;
        if (requester.Party is { } requesterParty)
        {
            if (!Equals(requesterParty.PartyMaster, requester))
            {
                // The inviter joined another party as a plain member in the meantime; it can no
                // longer take the bot in.
                return;
            }

            success = await requesterParty.AddAsync(bot).ConfigureAwait(false);
        }
        else
        {
            // Like the regular party response: the requester becomes the master of the new party.
            var party = bot.GameContext.PartyManager.CreateParty();
            success = await party.AddAsync(requester).ConfigureAwait(false)
                && await party.AddAsync(bot).ConfigureAwait(false);
        }

        if (success)
        {
            bot.Logger.LogDebug("Bot '{Name}' joined the party of '{Requester}'.", bot.Name, requester.Name);
        }
    }

    /// <summary>
    /// Lets the bot leave the bot-only party it hunts in, so it can join the player who invited it: a
    /// living player takes precedence over the bot's own company. When the bot LEADS that party, the
    /// group is broken up instead - the engine does not hand the mastership over to another member when
    /// the master leaves (it only removes them from the member list), which would leave the remaining
    /// bots following a leader who is not in their party anymore. Their next hourly re-formation groups
    /// them again (see <see cref="BotManager"/>).
    /// </summary>
    private static async ValueTask LeaveBotPartyAsync(OfflinePlayer bot)
    {
        if (bot.Party is not { } party)
        {
            return;
        }

        if (Equals(party.PartyMaster, bot))
        {
            bot.Logger.LogDebug("Bot '{Name}' breaks up its bot party to join a player.", bot.Name);
            foreach (var member in party.PartyList.ToList())
            {
                await party.KickMySelfAsync(member).ConfigureAwait(false);
            }

            return;
        }

        await party.KickMySelfAsync(bot).ConfigureAwait(false);
    }

    private static bool IsRequesterEligible(Player requester)
    {
        return requester.IsAlive && requester.PlayerState.CurrentState == PlayerState.EnteredWorld;
    }
}

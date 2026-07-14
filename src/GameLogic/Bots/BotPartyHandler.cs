// <copyright file="BotPartyHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// Lets a server-side bot party up with players who invite it (enabled by
/// <see cref="BotMuHelperSettings.AutoAcceptAnyone"/>): the invitation is accepted after a short
/// human-like delay, and the bot then follows the leader like any party member (see the follow logic
/// in <see cref="BotNavigator"/>) until it gets bored and politely leaves. Safeguards keep it
/// believable and abuse-free: no grouping across an absurd level gap, no acceptance while the bot is
/// on an errand (shopping trip) or has unfinished business (revenge), and the invitation is
/// re-validated when the delay has passed - the inviter may have joined another party or left.
/// </summary>
internal static class BotPartyHandler
{
    /// <summary>
    /// The maximum difference of the reset-aware effective level (see
    /// <see cref="BotResetHandler.GetEffectiveLevel"/>) between the bot and the inviter. Within one
    /// reset worth of levels plus some slack, hunting together still makes sense for both; grouping a
    /// fresh character with a 15-resets veteran would only be a power-leveling service. On servers
    /// without the reset feature the plain levels always lie within this bound, matching OpenMU's own
    /// party action, which has no level gate at all.
    /// </summary>
    private const int MaxEffectiveLevelGap = 500;

    /// <summary>Lower bound of the human-like delay before the bot answers an invitation.</summary>
    private static readonly TimeSpan MinAcceptDelay = TimeSpan.FromSeconds(2);

    /// <summary>Upper bound of the human-like delay before the bot answers an invitation.</summary>
    private static readonly TimeSpan MaxAcceptDelay = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Lower bound of the time the bot stays in a party with a human before it gets bored and leaves.
    /// A player who groups a bot gets a companion for a decent hunting session, but not a permanent
    /// follower - the bot has its own goals (its resets, its shopping, its own pace).
    /// </summary>
    private static readonly TimeSpan MinPartyDuration = TimeSpan.FromMinutes(10);

    /// <summary>Upper bound of the time the bot stays in a party with a human, see <see cref="MinPartyDuration"/>.</summary>
    private static readonly TimeSpan MaxPartyDuration = TimeSpan.FromMinutes(20);

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

        if (bot.IsOnShoppingTrip || bot.HasRevengeIntent || bot.CurrentMiniGame is not null)
        {
            // Busy - a player in the middle of an errand, a grudge or an event would not group up either.
            return false;
        }

        if (!IsRequesterEligible(bot, requester))
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
        bot.Logger.LogInformation("Bot '{Name}' accepts the party invitation of '{Requester}' in {Delay}.", bot.Name, requester.Name, delay);
        return true;
    }

    /// <summary>
    /// Drives the bot's party behavior; called from the bot's regular evaluation tick. Answers a
    /// pending invitation once its delay passed, and leaves the party again when the bot got bored
    /// of grouping with a human (bot-only parties are exempt - they are managed by the hourly
    /// re-formation of <see cref="BotManager"/>).
    /// </summary>
    /// <param name="bot">The bot.</param>
    internal static async ValueTask ProcessAsync(OfflinePlayer bot)
    {
        if (bot.PendingPartyInvite is { } invite && DateTime.UtcNow >= invite.AcceptAtUtc)
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

        if (bot.Party is { } party && HasHumanCompanion(bot))
        {
            bot.PartyBoredomAtUtc ??= DateTime.UtcNow + MinPartyDuration
                + TimeSpan.FromSeconds(Rand.NextInt(0, (int)(MaxPartyDuration - MinPartyDuration).TotalSeconds + 1));
            if (DateTime.UtcNow >= bot.PartyBoredomAtUtc)
            {
                bot.PartyBoredomAtUtc = null;
                bot.Logger.LogInformation("Bot '{Name}' got bored and leaves its party.", bot.Name);
                await party.KickMySelfAsync(bot).ConfigureAwait(false);
            }
        }
        else
        {
            bot.PartyBoredomAtUtc = null;
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
        if (HasHumanCompanion(bot) || !IsRequesterEligible(bot, requester))
        {
            bot.Logger.LogInformation("Bot '{Name}' dropped the party invitation of '{Requester}' - the situation changed.", bot.Name, requester.Name);
            return;
        }

        await LeaveBotPartyAsync(bot).ConfigureAwait(false);
        if (bot.Party is not null)
        {
            bot.Logger.LogInformation("Bot '{Name}' could not leave its bot party for '{Requester}'.", bot.Name, requester.Name);
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
            bot.Logger.LogInformation("Bot '{Name}' joined the party of '{Requester}'.", bot.Name, requester.Name);
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
            bot.Logger.LogInformation("Bot '{Name}' breaks up its bot party to join a player.", bot.Name);
            foreach (var member in party.PartyList.ToList())
            {
                await party.KickMySelfAsync(member).ConfigureAwait(false);
            }

            return;
        }

        await party.KickMySelfAsync(bot).ConfigureAwait(false);
    }

    private static bool IsRequesterEligible(OfflinePlayer bot, Player requester)
    {
        if (!requester.IsAlive || requester.PlayerState.CurrentState != PlayerState.EnteredWorld)
        {
            return false;
        }

        var levelGap = Math.Abs(BotResetHandler.GetEffectiveLevel(bot) - BotResetHandler.GetEffectiveLevel(requester));
        return levelGap <= MaxEffectiveLevelGap;
    }
}

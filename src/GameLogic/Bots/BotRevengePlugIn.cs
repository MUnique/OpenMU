// <copyright file="BotRevengePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Notes when a (human) player kills a server-side bot, so the bot can march back to the place of
/// its death after respawning and take revenge on the killer. A bot which shrugs off being killed
/// and calmly heads for the next hunting ground is an obvious bot giveaway - a real player comes
/// back angry. The return march happens in the <see cref="BotNavigator"/>; the counter-attack in
/// the offline <see cref="CombatHandler"/>, whose re-armed aggressor memory keeps the killer
/// prioritized - struck only once the game's own rules make it legal (see <see cref="BotPvpRules"/>).
/// </summary>
[PlugIn]
[Display(Name = "Bot revenge", Description = "Makes server-side bots return to their death site and take revenge on the player who killed them.")]
[Guid("29B871B0-FBCF-44D4-A677-8A9832AAC193")]
public class BotRevengePlugIn : IAttackableGotKilledPlugIn
{
    /// <inheritdoc />
    public ValueTask AttackableGotKilledAsync(IAttackable killed, IAttacker? killer)
    {
        if (killed is OfflinePlayer bot
            && bot.Account?.IsBot == true
            && bot.CurrentMiniGame is null // a death in an event (Chaos Castle) is part of the game, not a wrong to avenge
            && killer is Player killerPlayer
            && killerPlayer is not OfflinePlayer
            && !ReferenceEquals(killerPlayer, killed))
        {
            bot.RegisterDeathByPlayer(killerPlayer);
        }

        return ValueTask.CompletedTask;
    }
}

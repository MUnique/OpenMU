// <copyright file="BotPartyComposer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// Groups bot candidates into hunting parties.
/// Each party holds exactly one buffer and one of each build at most.
/// Members and buffer all sit within the level gap above the leader,
/// so party span never exceeds it.
/// </summary>
internal static class BotPartyComposer
{
    /// <summary>
    /// Composes parties from level-ordered candidates.
    /// </summary>
    /// <param name="candidates">The bots without a party, ordered by effective level.</param>
    internal static Formation Compose(IReadOnlyList<OfflinePlayer> candidates)
    {
        var remaining = candidates.ToList();
        var parties = new List<List<OfflinePlayer>>();
        var waitingBuffers = new List<OfflinePlayer>();

        while (remaining.Count > 0)
        {
            var leader = remaining[0];
            remaining.RemoveAt(0);

            // Buffers always group up. Others may stay solo for variety.
            if (!BotBuild.IsSupportElf(leader) && Rand.NextInt(0, 100) >= BotPartyPolicy.PartiedSharePercent)
            {
                continue;
            }

            var targetSize = Rand.NextInt(BotPartyPolicy.MinPartySize, BotPartyPolicy.MaxPartySize + 1);
            var members = new List<OfflinePlayer> { leader };
            var picked = PickMembers(remaining, leader, targetSize);

            if (BotBuild.IsSupportElf(leader))
            {
                members.AddRange(picked);
                if (members.Count >= BotPartyPolicy.MinPartySize)
                {
                    RemoveAll(remaining, picked);
                    parties.Add(members);
                }
                else
                {
                    waitingBuffers.Add(leader);
                }

                continue;
            }

            var buffer = FindBuffer(remaining, leader);
            if (buffer is null)
            {
                leader.Logger.LogDebug("Bot '{Name}' stays solo: no buffer in range.", leader.Name);
                continue;
            }

            members.AddRange(picked);
            members.Add(buffer);
            RemoveAll(remaining, picked);
            remaining.Remove(buffer);
            parties.Add(members);
        }

        return new Formation(parties, waitingBuffers);
    }

    private static List<OfflinePlayer> PickMembers(List<OfflinePlayer> remaining, OfflinePlayer leader, int targetSize)
    {
        var picked = new List<OfflinePlayer>();
        var leaderLevel = BotResetHandler.GetEffectiveLevel(leader);
        var openSlots = (BotBuild.IsSupportElf(leader) ? targetSize : targetSize - 1) - 1;
        var buildKeys = new HashSet<(byte Line, int Slot)> { BotBuild.GetBuildKey(leader) };

        foreach (var candidate in remaining)
        {
            if (picked.Count >= openSlots)
            {
                break;
            }

            if (BotResetHandler.GetEffectiveLevel(candidate) - leaderLevel > BotPartyPolicy.MaxLevelGap)
            {
                break;
            }

            // Buffers join through FindBuffer. One of each build at most.
            if (BotBuild.IsSupportElf(candidate) || !buildKeys.Add(BotBuild.GetBuildKey(candidate)))
            {
                continue;
            }

            picked.Add(candidate);
        }

        return picked;
    }

    private static OfflinePlayer? FindBuffer(List<OfflinePlayer> remaining, OfflinePlayer leader)
    {
        var leaderLevel = BotResetHandler.GetEffectiveLevel(leader);
        return remaining
            .Where(BotBuild.IsSupportElf)
            .Where(b => Math.Abs(BotResetHandler.GetEffectiveLevel(b) - leaderLevel) <= BotPartyPolicy.MaxLevelGap)
            .MinBy(b => Math.Abs(BotResetHandler.GetEffectiveLevel(b) - leaderLevel));
    }

    private static void RemoveAll(List<OfflinePlayer> remaining, List<OfflinePlayer> picked)
    {
        foreach (var member in picked)
        {
            remaining.Remove(member);
        }
    }

    /// <summary>
    /// The outcome of one formation pass.
    /// </summary>
    /// <param name="Parties">The formed parties.</param>
    /// <param name="UnplacedBuffers">Buffers which found no party and hunt solo.</param>
    internal sealed record Formation(
        IReadOnlyList<IReadOnlyList<OfflinePlayer>> Parties,
        IReadOnlyList<OfflinePlayer> UnplacedBuffers);
}

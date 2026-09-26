// <copyright file="WeeklyQuestListViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IWeeklyQuestListViewPlugIn"/> which sends
/// one <see cref="WeeklyQuestEntry"/> message per weekly quest to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.WeeklyQuestListViewPlugIn_Name), Description = nameof(PlugInResources.WeeklyQuestListViewPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("C4F1A7B2-6E3D-4B89-9A0C-5D2E8F7B1A36")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class WeeklyQuestListViewPlugIn : IWeeklyQuestListViewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeeklyQuestListViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public WeeklyQuestListViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowWeeklyQuestsAsync(WeeklyQuestOverview overview)
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        var seconds = GetSecondsUntil(overview.NextResetUtc);

        // The client can't count more than a byte, and nobody reads that many quests anyway.
        var entries = overview.Entries.Take(byte.MaxValue).ToList();
        if (entries.Count == 0)
        {
            // An empty list is sent as one message without a quest, so that the client forgets the previous ones.
            await this.SendAsync(connection, null, 0, 0, false, seconds).ConfigureAwait(false);
            return;
        }

        for (var i = 0; i < entries.Count; i++)
        {
            await this.SendAsync(connection, entries[i], (byte)i, (byte)entries.Count, false, seconds).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask UpdateWeeklyQuestAsync(WeeklyQuestOverviewEntry entry, DateTime nextResetUtc)
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        await this.SendAsync(connection, entry, 0, 1, true, GetSecondsUntil(nextResetUtc)).ConfigureAwait(false);
    }

    private static uint GetSecondsUntil(DateTime utc)
    {
        return (uint)Math.Clamp((utc - DateTime.UtcNow).TotalSeconds, 0, uint.MaxValue);
    }

    private ValueTask SendAsync(IConnection connection, WeeklyQuestOverviewEntry? entry, byte index, byte count, bool isUpdate, uint secondsUntilReset)
    {
        var culture = this._player.Culture;

        int Write()
        {
            var size = WeeklyQuestEntryRef.Length;
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new WeeklyQuestEntryRef(span)
            {
                Index = index,
                Count = count,
                IsUpdate = isUpdate,
                SecondsUntilReset = secondsUntilReset,
            };

            if (entry is not null)
            {
                packet.IsCompleted = entry.IsCompleted;
                packet.IsRewarded = entry.IsRewarded;
                packet.CurrentCount = (uint)Math.Max(0, entry.Count);
                packet.RequiredCount = (uint)Math.Max(0, entry.Quest.RequiredCount);
                packet.Id = entry.Quest.Id;
                packet.Name = entry.Quest.Name;
                packet.Description = entry.Quest.Description;
                packet.Rewards = entry.Quest.GetRewardsText(culture);
            }

            return size;
        }

        return connection.SendAsync(Write);
    }
}

// <copyright file="WeeklyQuestListViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Runtime.InteropServices;
using System.Text;
using MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IWeeklyQuestListViewPlugIn"/> which sends
/// one <see cref="WeeklyQuestEntry"/> message per quest to the game client, each followed by
/// a <see cref="QuestDetails"/> message with its category and objectives.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.WeeklyQuestListViewPlugIn_Name), Description = nameof(PlugInResources.WeeklyQuestListViewPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("C4F1A7B2-6E3D-4B89-9A0C-5D2E8F7B1A36")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class WeeklyQuestListViewPlugIn : IWeeklyQuestListViewPlugIn
{
    /// <summary>
    /// The maximum number of objectives which are sent per quest. More wouldn't fit into the window anyway.
    /// </summary>
    private const int MaximumObjectives = 16;

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
            await this.SendEntryAsync(connection, null, 0, 0, false, seconds).ConfigureAwait(false);
            return;
        }

        for (var i = 0; i < entries.Count; i++)
        {
            await this.SendEntryAsync(connection, entries[i], (byte)i, (byte)entries.Count, false, seconds).ConfigureAwait(false);
            await this.SendDetailsAsync(connection, entries[i], overview.NextResetUtc, overview.NextDailyResetUtc).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask UpdateWeeklyQuestAsync(WeeklyQuestOverviewEntry entry, DateTime nextResetUtc, DateTime nextDailyResetUtc)
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        await this.SendEntryAsync(connection, entry, 0, 1, true, GetSecondsUntil(nextResetUtc)).ConfigureAwait(false);
        await this.SendDetailsAsync(connection, entry, nextResetUtc, nextDailyResetUtc).ConfigureAwait(false);
    }

    private static uint GetSecondsUntil(DateTime utc)
    {
        return (uint)Math.Clamp((utc - DateTime.UtcNow).TotalSeconds, 0, uint.MaxValue);
    }

    /// <summary>
    /// Cuts the text, so that it fits into a string field of the specified size, without cutting a character in half.
    /// </summary>
    private static string Fit(string? text, int maxBytes)
    {
        text ??= string.Empty;
        if (Encoding.UTF8.GetByteCount(text) <= maxBytes)
        {
            return text;
        }

        var length = Math.Min(text.Length, maxBytes);
        while (length > 0 && Encoding.UTF8.GetByteCount(text.AsSpan(0, length)) > maxBytes)
        {
            length--;
        }

        if (length > 0 && char.IsHighSurrogate(text[length - 1]))
        {
            length--;
        }

        return text[..length];
    }

    private ValueTask SendEntryAsync(IConnection connection, WeeklyQuestOverviewEntry? entry, byte index, byte count, bool isUpdate, uint secondsUntilReset)
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
                packet.RequiredCount = (uint)Math.Max(0, entry.Required);
                packet.Id = Fit(entry.Quest.Id, 64);
                packet.Name = Fit(entry.Quest.Name, 48);
                packet.Description = Fit(entry.Quest.Description, 256);
                packet.Rewards = Fit(entry.Quest.GetRewardsText(culture), 128);
            }

            return size;
        }

        return connection.SendAsync(Write);
    }

    private ValueTask SendDetailsAsync(IConnection connection, WeeklyQuestOverviewEntry entry, DateTime nextResetUtc, DateTime nextDailyResetUtc)
    {
        var culture = this._player.Culture;
        var objectives = entry.Objectives.Take(MaximumObjectives).ToList();
        var secondsUntilReset = entry.Quest.Period switch
        {
            QuestPeriod.Once => 0u,
            QuestPeriod.Daily => GetSecondsUntil(nextDailyResetUtc),
            _ => GetSecondsUntil(nextResetUtc),
        };

        int Write()
        {
            var size = QuestDetailsRef.GetRequiredSize(objectives.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new QuestDetailsRef(span)
            {
                Category = (byte)entry.Quest.Category,
                Period = (byte)entry.Quest.Period,
                CurrentStep = (byte)Math.Clamp(entry.CurrentStep, 0, objectives.Count),
                ObjectiveCount = (byte)objectives.Count,
                IsSequential = entry.Quest.SequentialObjectives,
                SecondsUntilReset = secondsUntilReset,
                Id = Fit(entry.Quest.Id, 64),
            };

            for (var i = 0; i < objectives.Count; i++)
            {
                var objective = objectives[i];
                var target = packet[i];
                target.CurrentCount = (uint)Math.Max(0, objective.Count);
                target.RequiredCount = (uint)Math.Max(0, objective.Required);
                target.IsDone = objective.IsDone;
                target.Text = Fit(objective.Objective.GetDisplayText(culture), 64);
            }

            return size;
        }

        return connection.SendAsync(Write);
    }
}

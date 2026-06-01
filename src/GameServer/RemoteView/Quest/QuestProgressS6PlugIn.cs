// <copyright file="QuestProgressS6PlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest;

using System.Buffers.Binary;
using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Season 6-compatible implementation of <see cref="IQuestProgressPlugIn"/>.
/// Sends GC[0xF6][0x0C] in the format the S6 GMO client expects:
/// PMSG_NPC_QUESTEXP_INFO header followed by 5 NPC_QUESTEXP_REQUEST_INFO slots
/// and 5 NPC_QUESTEXP_REWARD_INFO slots (all with default MSVC alignment).
/// </summary>
[PlugIn]
[Display(Name = "QuestProgressS6PlugIn", Description = "Season 6 quest progress plugin sending F6 0C in S6 client format.")]
[Guid("B10A7CBD-3C5F-4FF3-8CAC-EE712E20A6C1")]
[MinimumClient(6, 0, ClientLanguage.Invariant)]
public class QuestProgressS6PlugIn : IQuestProgressPlugIn
{
    // S6 QUEST_REQUEST_TYPE values
    private const byte RequestTypeNone = 0;
    private const byte RequestTypeMonster = 1;
    private const byte RequestTypeItem = 3;
    private const byte RequestTypeZen = 15;

    // S6 QUEST_REWARD_TYPE values
    private const byte RewardTypeNone = 0;
    private const byte RewardTypeExp = 1;
    private const byte RewardTypeZen = 2;
    private const byte RewardTypeItem = 4;

    // Packet layout constants.
    // sizeof(PMSG_NPC_QUESTEXP_INFO) = PWMSG_HEADER(4) + SubCode(1) + RequestCount(1)
    //   + RewardCount(1) + RandRewardCount(1) + QuestIndex DWORD(4) = 12 bytes.
    private const int HeaderSize = 12;

    private const int MaxSlots = 5;
    private const int ItemInfoLength = 15;

    // sizeof(NPC_QUESTEXP_REQUEST_INFO) with default MSVC alignment (/Zp8):
    //   BYTE(1) + pad(1) + WORD(2) + DWORD(4) + DWORD(4) + BYTE[15](15) + pad(1) = 28 bytes.
    private const int RequestSlotSize = 28;

    // sizeof(NPC_QUESTEXP_REWARD_INFO) with default MSVC alignment (/Zp8):
    //   BYTE(1) + pad(1) + WORD(2) + DWORD(4) + BYTE[15](15) + pad(1) = 24 bytes.
    private const int RewardSlotSize = 24;

    private const int TotalPacketSize =
        HeaderSize + (MaxSlots * RequestSlotSize) + (MaxSlots * RewardSlotSize); // 272

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestProgressS6PlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public QuestProgressS6PlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc/>
    public async ValueTask ShowQuestProgressAsync(QuestDefinition quest, bool wasProgressionRequested)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        var questState = this._player.SelectedCharacter?.QuestStates
            .FirstOrDefault(q => q.Group == quest.Group);

        int Write()
        {
            var span = connection.Output.GetSpan(TotalPacketSize)[..TotalPacketSize];
            span.Clear();

            // C2 header: [0xC2, sizeH, sizeL, 0xF6]
            span[0] = 0xC2;
            span[1] = (byte)(TotalPacketSize >> 8);
            span[2] = (byte)(TotalPacketSize & 0xFF);
            span[3] = 0xF6;

            // SubCode
            span[4] = 0x0C;

            var monsterKills = quest.RequiredMonsterKills?.ToList() ?? new();
            var itemReqs = quest.RequiredItems?.ToList() ?? new();
            var rewards = quest.Rewards?
                .Where(r => r.RewardType is QuestRewardType.Experience
                    or QuestRewardType.Money
                    or QuestRewardType.Item)
                .Take(MaxSlots)
                .ToList() ?? new();

            int requestCount = Math.Min(monsterKills.Count + itemReqs.Count, MaxSlots);
            int rewardCount = rewards.Count;

            span[5] = (byte)requestCount;
            span[6] = (byte)rewardCount;
            span[7] = 0; // RandRewardCount

            // QuestIndex = (Group << 16) | Number, little-endian DWORD
            uint questIndex = ((uint)(ushort)quest.Group << 16) | (uint)(ushort)quest.Number;
            BinaryPrimitives.WriteUInt32LittleEndian(span[8..12], questIndex);

            // Request slots (monster kills first, then item requirements)
            int slot = 0;
            foreach (var kill in monsterKills.Take(MaxSlots))
            {
                this.WriteMonsterKillSlot(span, slot, kill, questState);
                slot++;
            }

            foreach (var itemReq in itemReqs.Take(MaxSlots - slot))
            {
                this.WriteItemReqSlot(span, slot, itemReq);
                slot++;
            }

            // Reward slots
            for (int r = 0; r < rewards.Count; r++)
            {
                this.WriteRewardSlot(span, r, rewards[r]);
            }

            return TotalPacketSize;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    private void WriteMonsterKillSlot(
        Span<byte> packet,
        int slotIndex,
        QuestMonsterKillRequirement kill,
        CharacterQuestState? questState)
    {
        int b = HeaderSize + (slotIndex * RequestSlotSize);

        // [+0] type; [+1] alignment padding (cleared)
        packet[b + 0] = RequestTypeMonster;

        // [+2..3] m_wIndex = monster number (WORD LE)
        BinaryPrimitives.WriteUInt16LittleEndian(packet[(b + 2)..(b + 4)], (ushort)(kill.Monster?.Number ?? 0));

        // [+4..7] m_dwValue = required kill count (DWORD LE)
        BinaryPrimitives.WriteUInt32LittleEndian(packet[(b + 4)..(b + 8)], (uint)kill.MinimumNumber);

        // [+8..11] m_wCurValue = current kill count (DWORD LE)
        uint curCount = questState is null
            ? 0u
            : (uint)(questState.RequirementStates
                .FirstOrDefault(s => s.Requirement != null && s.Requirement.Equals(kill))
                ?.KillCount ?? 0);
        BinaryPrimitives.WriteUInt32LittleEndian(packet[(b + 8)..(b + 12)], curCount);

        // [+12..26] m_byItemInfo[15] = zeros (already cleared); [+27] struct padding
    }

    private void WriteItemReqSlot(
        Span<byte> packet,
        int slotIndex,
        QuestItemRequirement itemReq)
    {
        if (itemReq.Item is not { } item)
        {
            return;
        }

        int b = HeaderSize + (slotIndex * RequestSlotSize);

        packet[b + 0] = RequestTypeItem;

        // m_wIndex = item type (group << 9 | number)
        ushort itemType = (ushort)(((ushort)(item.Group << 9)) | (ushort)item.Number);
        BinaryPrimitives.WriteUInt16LittleEndian(packet[(b + 2)..(b + 4)], itemType);

        // m_dwValue = required count
        BinaryPrimitives.WriteUInt32LittleEndian(packet[(b + 4)..(b + 8)], (uint)itemReq.MinimumNumber);

        // m_wCurValue = current count in inventory
        uint curCount = (uint)(this._player.Inventory?.Items.Count(i => Equals(i.Definition, item)) ?? 0);
        BinaryPrimitives.WriteUInt32LittleEndian(packet[(b + 8)..(b + 12)], curCount);

        // Serialize item definition into m_byItemInfo[15]
        var temporaryItem = new TemporaryItem { Definition = item };
        temporaryItem.Durability = temporaryItem.GetMaximumDurabilityOfOnePiece();
        this._player.ItemSerializer.SerializeItem(packet[(b + 12)..(b + 27)], temporaryItem);
    }

    private void WriteRewardSlot(
        Span<byte> packet,
        int slotIndex,
        QuestReward reward)
    {
        int b = HeaderSize + (MaxSlots * RequestSlotSize) + (slotIndex * RewardSlotSize);

        byte rewardType = reward.RewardType switch
        {
            QuestRewardType.Experience => RewardTypeExp,
            QuestRewardType.Money => RewardTypeZen,
            QuestRewardType.Item => RewardTypeItem,
            _ => RewardTypeNone,
        };
        packet[b + 0] = rewardType;

        if (reward.RewardType == QuestRewardType.Item && reward.ItemReward?.Definition is { } itemDef)
        {
            // m_wIndex = item type
            ushort itemType = (ushort)(((ushort)(itemDef.Group << 9)) | (ushort)itemDef.Number);
            BinaryPrimitives.WriteUInt16LittleEndian(packet[(b + 2)..(b + 4)], itemType);

            // m_dwValue = reward quantity
            BinaryPrimitives.WriteUInt32LittleEndian(packet[(b + 4)..(b + 8)], (uint)reward.Value);

            // Serialize item into m_byItemInfo[15]
            this._player.ItemSerializer.SerializeItem(packet[(b + 8)..(b + 23)], reward.ItemReward);
        }
        else
        {
            // EXP or ZEN: m_wIndex = 0, m_dwValue = amount
            BinaryPrimitives.WriteUInt32LittleEndian(packet[(b + 4)..(b + 8)], (uint)reward.Value);
        }
    }
}

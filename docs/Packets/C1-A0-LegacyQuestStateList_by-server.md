# C1 A0 - LegacyQuestStateList (by server)

## Is sent when

After the player entered the game with his character.

## Causes the following actions on the client side

The game client updates the quest state for the quest dialog accordingly.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xA0  | Packet header - packet type identifier |
| 3 | 1 | Byte | 6 | QuestCount |
| 4 | LegacyQuestState.Length *  | Array of LegacyQuestState |  | States |
| 4 << 0 | 2 bit | LegacyQuestState | LegacyQuestState.Inactive | ScrollOfEmperorState |
| 4 << 2 | 2 bit | LegacyQuestState | LegacyQuestState.Inactive | ThreeTreasuresOfMuState |
| 4 << 4 | 2 bit | LegacyQuestState | LegacyQuestState.Inactive | GainHeroStatusState |
| 4 << 6 | 2 bit | LegacyQuestState | LegacyQuestState.Inactive | SecretOfDarkStoneState |
| 5 << 0 | 2 bit | LegacyQuestState | LegacyQuestState.Inactive | CertificateOfStrengthState |
| 5 << 2 | 2 bit | LegacyQuestState | LegacyQuestState.Inactive | InfiltrationOfBarrackState |
| 5 << 4 | 2 bit | LegacyQuestState | LegacyQuestState.Inactive | InfiltrationOfRefugeState |
| 5 << 6 | 2 bit | LegacyQuestState | LegacyQuestState.Undefined | UnusedQuestState |
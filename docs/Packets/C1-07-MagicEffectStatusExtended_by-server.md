# C1 07 - MagicEffectStatusExtended (by server)

## Is sent when

A magic effect was added to the own player, or its time or values changed (a recast or a shortened duration).

## Causes the following actions on the client side

The user interface shows the effect as icon at the top of the interface, with the time left and the values of the effect. The first fields are the same as of MagicEffectStatus, so a client which only knows that one reads it as such.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x07  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | IsActive |
| 4 | 2 | ShortBigEndian |  | PlayerId |
| 6 | 1 | Byte |  | EffectId |
| 7 | 4 | IntegerLittleEndian |  | RemainingSeconds; The seconds until the effect ends, rounded up; 0 if the effect's definition doesn't send its duration (SendDuration), so the client shows no time. |
| 11 | 1 | Byte |  | ValueCount |
| 12 | EffectValue.Length * ValueCount | Array of EffectValue |  | Values; The finished value of the effect for each attribute it changes, one per attribute; their order has no meaning. It's the effect's own value: the server combines the power-ups of the effect for one attribute as the attribute system does, as if the effect were the attribute's only source, and limits it to the attribute's maximum (e.g. Greater Damage: the energy bonus times the factor of its master skills; Soul Barrier at most 0.7). Other sources of the attribute, like other effects, items or stats, aren't part of it. Only the power-ups which the effect has are counted, e.g. a stun has no value for its stun chance, poison and bleeding only their first. |

### EffectValue Structure

The finished value of a magic effect for one attribute.

Length: 21 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 16 | Binary |  | AttributeId; The id of the attribute which the value applies to (the Id of its AttributeDefinition), as the 16 bytes of the Guid in the order of its text form (big endian). |
| 16 | 1 | MagicEffectStatusExtended.ValueKind |  | Kind |
| 17 | 4 | Float |  | Value |

### ValueKind Enum

Defines how a value changes its attribute (the aggregate type of the attribute system). When the effect has several kinds for one attribute, they are combined into one value: AddRaw if the effect adds a value before the factors or has a maximum, else AddFinal. Its factors are then part of the value and only count for its own values, e.g. a factor with only a finally added value gives that value.

| Value | Name | Description |
|-------|------|-------------|
| 0 | AddRaw | The value is added to the attribute, before the factors apply. |
| 1 | Multiplicate | The value is a factor of the attribute, e.g. 1.1 for +10 %. Only when the effect has nothing but factors for this attribute. |
| 2 | AddFinal | The value is added to the attribute, after the factors apply. |
| 3 | Maximum | Only the highest value of this kind counts for the attribute (e.g. jewelry element resistance). |
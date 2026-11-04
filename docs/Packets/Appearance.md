# Appearance

The appearance of characters is serialized as follows. 
Webzen did a "good" job saving one bit here and there, so it's a bit complicated
sometimes (e.g. wings and pets).
It seems like the structure is historically grown.

## Structure

Please read this table like a stream of bits. E.g. if there are 8 bits of 1 byte
are specified in a row, the highest bits come first.

|  Byte index | Length | Data type | Description |
|----------|---------|-------------|------------|
| 0 | 4 | bit  | Character class |
| 0 | 4 | bit  | Character pose, see below |
| 1 | 1 | byte | Left hand item index. 0xFF if empty. |
| 2 | 1 | byte | Right hand item index. 0xFF if empty. |
| 3 | 4 | bit | Helm item index (lower 4 bits). See byte index 9 and 13 for the rest. |
| 3 | 4 | bit | Armor item index (lower 4 bits). See byte index 9 and 14 for the rest. |
| 4 | 4 | bit | Pants item index (lower 4 bits). See byte index 9 and 14 for the rest. |
| 4 | 4 | bit | Gloves item index (lower 4 bits). See byte index 9 and 15 for the rest. |
| 5 | 4 | bit | Boots item index (lower 4 bits). See byte index 9 and 15 for the rest. |
| 5 | 4 | bit | Pets and wings flags, see below |
| 6 | 3 | bit | Left hand item level |
| 6 | 3 | bit | Right hand item level |
| 6~7 | 3 | bit | Helm item level |
| 7 | 3 | bit | Armor item level |
| 7 | 3 | bit | Pants item level |
| 7~8 | 3 | bit | Gloves item level |
| 8 | 3 | bit | Boots item level |
| 8 | 3 | bit | unused |
| 9 | 1 | bit | Helm item index (5th bit) |
| 9 | 1 | bit | Armor item index (5th bit) |
| 9 | 1 | bit | Pants item index (5th bit) |
| 9 | 1 | bit | Gloves item index (5th bit) |
| 9 | 1 | bit | Boots item index (5th bit) |
| 9 | 3 | bit | Wing item index (see table below) |
| 10 | 1 | bit | Helm excellent option flag |
| 10 | 1 | bit | Armor excellent option flag |
| 10 | 1 | bit | Pants excellent option flag |
| 10 | 1 | bit | Gloves excellent option flag |
| 10 | 1 | bit | Boots excellent option flag |
| 10 | 1 | bit | Left hand item excellent option flag |
| 10 | 1 | bit | Right hand item excellent option flag |
| 10 | 1 | bit | Dinorant flag |
| 11 | 1 | bit | Helm ancient option flag |
| 11 | 1 | bit | Armor ancient option flag |
| 11 | 1 | bit | Pants ancient option flag |
| 11 | 1 | bit | Gloves ancient option flag |
| 11 | 1 | bit | Boots ancient option flag |
| 11 | 1 | bit | Left hand item ancient option flag |
| 11 | 1 | bit | Right hand item ancient option flag |
| 11 | 1 | bit | Full ancient set flag |
| 12 | 3 | bit | Left hand item group. 111 = empty |
| 12 | 1 | bit | unused or empty flag? |
| 12 | 1 | bit | unused |
| 12 | 1 | bit | Fenrir flag |
| 12 | 1 | bit | unused |
| 12 | 1 | bit | Dark horse flag |
| 13 | 3 | bit | Right hand item group. 111 = empty |
| 13 | 1 | bit | unused or empty flag? |
| 13 | 4 | bit | Helm item index (6-9th bit). 0xF = empty |
| 14 | 4 | bit | Armor item index (6-9th bit). 0xF = empty |
| 14 | 4 | bit | Pants item index (6-9th bit). 0xF = empty |
| 15 | 4 | bit | Gloves item index (6-9th bit). 0xF = empty |
| 15 | 4 | bit | Boots item index (6-9th bit). 0xF = empty |
| 16 | 6 | bit | Pet item index, see below |
| 16 | 1 | bit | Blue fenrir flag |
| 16 | 1 | bit | Black fenrir flag |
| 17 | 4 | bit | Small Wing item index, see below |
| 17 | 3 | bit | unused |
| 17 | 1 | bit | Gold fenrir flag |

### Character pose

| Value | Meaning |
|-------|---------|
|   0   | Standing (default) |
|   1   | unused  |
|   2   | Sitting (e.g. on a trunk) |
|   3   | Leaning (e.g. against a wall) |
|   4   | Hanging (at these strange things in Noria) |

Note: At Season 10 and above, the values are different.
Sitting is at 1, Leaning 2, Hanging 3. So, they removed the unused value.

### Item level calculation

The item levels are calculated with the following formula: ([Item Level] - 1) / 2.

This works until item level of 15, because it fits into 4 bits.

### Item indexes

The bytes are represented in binary format. X means the bit is used by something
else (see table above).

#### Pet items

| Item                  | 5th byte | 10th byte | 12th byte |
|-----------------------|----------|-----------|-----------|
| Guardian Angel        | xxxxxx00 |
| Imp                   | xxxxxx01 |
| Unicorn               | xxxxxx10 |
| Dinorant              | xxxxxx11 | xxxxxxx1  ||
| Fenrir                | xxxxxx11 |           | xxxxxx1x  |
| None                  | xxxxxx11 | xxxxxxx0  |

And some other pets:

| Item                  | 16th byte |
|-----------------------|-----------|
| Pet Panda             | 111000xx  |
| Pet Unicorn           | 101000xx  |
| Skeleton              | 011000xx  |
| Rudolph               | 100000xx  |
| Spirit of Guardian    | 010000xx  |
| Demon                 | 001000xx  |

#### Wings

| Item                  | Character Class | 5th byte | 9th byte |
|-----------------------|-----------------|----------|----------|
| Wings of Elf          | Fairy Elf       | xxxx01xx | xxxxx001 |
| Wings of Heaven       | Dark Wizard     | xxxx01xx | xxxxx010 |
| Wings of Satan        | Dark Knight     | xxxx01xx | xxxxx011 |
| Wings of Mistery      | Summoner        | xxxx01xx | xxxxx100 |
| Wings of Spirit       | Muse Elf        | xxxx10xx | xxxxx001 |
| Wings of Soul         | Soul Master     | xxxx10xx | xxxxx010 |
| Wings of Dragon       | Blade Knight    | xxxx10xx | xxxxx011 |
| Wings of Darkness     | Magic Gladiator | xxxx10xx | xxxxx100 |
| Cape of Lord          | Dark Lord       | xxxx10xx | xxxxx101 |
| Wings of Despair      | Bloody Summoner | xxxx10xx | xxxxx110 |
| Cape of Fighter       | Rage Fighter    | xxxx10xx | xxxxx111 |
| Wing of Storm         | Blade Master    | xxxx11xx | xxxxx001 |
| Wing of Eternal       | Grand Master    | xxxx11xx | xxxxx010 |
| Wing of Illusion      | High Elf        | xxxx11xx | xxxxx011 |
| Wing of Ruin          | Duel Master     | xxxx11xx | xxxxx100 |
| Cape of Emperor       | Lord Emperor    | xxxx11xx | xxxxx101 |
| Wing of Dimension     | Dimension Master| xxxx11xx | xxxxx110 |
| Cape of Overrule      | Fist Master     | xxxx11xx | xxxxx111 |
| None                  |                 | xxxx00xx | xxxxx000 |

#### Small Wings

| Wing Item             | 5th byte | 17th byte |
|-----------------------|----------|-------|
| Small Cape of Lord    | xxxx11xx |  0x20 |
| Small Wings of Mistery| xxxx11xx |  0x40 |
| Small Wings of Elf    | xxxx11xx |  0x60 |
| Small Wings of Heaven | xxxx11xx |  0x80 |
| Small Wings of Satan  | xxxx11xx |  0xA0 |
| Small Cloak of Warrior| xxxx11xx |  0xC0 |
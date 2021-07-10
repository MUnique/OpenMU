# Master System

## What is it?

When a character reached a certain level (usually level 400) and completes a quest,
its character class changes to a *master character class*.
This unlocks the *Master Skill Tree* which allows to distribute points to master
skills.
Points are given by each master level the player reaches (just like before, by
gaining experience).

## Types of master skills

Basically, there are two types of master skills in a skill tree:

### Passive skills

When these skills are learned, they give certain power-ups passively. For example,
there are master skills to increase the maximum health or to increase the
attack damage.

### Active skills

When learned, these skills appear in the skill list. Most skills of this type replace
previously existing skills. The replaced skills are staying in the background,
are not visible in the game client, but their internal value still apply to the
master skill as well.
The master skill just defines how much damage or buff power is added to the
replaced skill.

## Structure of the master skill tree

The master skill tree consists of three roots.
The skills are put into rows which defines the *rank* of a skill.
By default, there 5 ranks available. However, the game client would support up
to 9 ranks. Each skill usually can have up to 20 levels, some only have 10. The
client probably supports more than that.

A skill can be defined to depend on a skill of the same or the previous rank of
the same root.

Each character class is able to learn different skills, which are sometimes
exclusive to the character class. So, one skill can be available for multiple
character classes.
The root and rank for a skill is always the same for all character classes.
The visual appearance on the client may differ (more on that later).

## Requisitions of learning a skill

To learn a skill, the following checks are done by the server (and client).

### Character class

The skill must be defined for the class of the character.

### Rank

The skill must either be in the first rank, or a skill in the previous rank of
the same root must at least of level 10.

### Required Skill

If the skill has required skills defined (it's optional), these skills must be
at least of level 10.

## Client implementation

Some short notes about the client:

* There is a message for the master level etc. (F3 50)

  * It contains health and mana, too. If this packet isn't sent, a master
    character appears without health/mana.

* There is a message for the learned master skills (F3 53)

  * The information about each skill contains:

    * Skill Number

    * Skill Index

      * Defines were the skill is located in the clients interface

      * I wonder why they need this in the message, since the client already
        knows the index of a skill itself

      * May differ between character classes

    * Current Level

    * Value of its effect at the current level

    * Value of its effect at the next level

  * Even if no skill was learned, this message needs to be sent - otherwise it
    may still contain the master information about the previously played master
    character.

## Server implementation

### Adding Points

* For code, see [AddMasterPointAction](https://github.com/MUnique/OpenMU/tree/master/src/GameLogic/PlayerActions/Character/AddMasterPointAction.cs).
* Request Packet: [C1F352 - Add Master Skill Point](Packets/C1-F3-52-AddMasterSkillPoint_by-client.md)
* Response Packet: [C1F352 - Master skill level update](Packets/C1-F3-52-MasterSkillLevelUpdate_by-server.md)

### Sending Master Stats (F3 50)

* For code, see [UpdateMasterStatsPlugIn](https://github.com/MUnique/OpenMU/tree/master/src/GameServer/RemoteView/Character/UpdateMasterStatsPlugIn.cs).
* Request Packet: [C2F350 - Master Stats Update](Packets/C1-F3-50-MasterStatsUpdate_by-server.md)

### Sending Master Skills (F3 53)

* For code, see [UpdateMasterSkillsPlugIn](https://github.com/MUnique/OpenMU/tree/master/src/GameServer/RemoteView/Character/UpdateMasterSkillsPlugIn.cs).
* Packet: [C2F353 - Master Skill List](Packets/C2-F3-53-MasterSkillList_by-server.md)

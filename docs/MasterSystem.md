# Master System / Sistema Maestro

*Read this document in [English](#english) or [Español](#espanol).* 

<a id="english"></a>
## English

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

<a id="espanol"></a>
## Español

## ¿Qué es?

Cuando un personaje alcanza cierto level (usualmente level 400) y completa una
quest, su character class cambia a una *master character class*. Esto desbloquea
el *Master Skill Tree*, que permite distribuir points a master skills. Los points
se otorgan por cada master level que el player alcanza (igual que antes, ganando
experience).

## Tipos de master skills

Básicamente hay dos tipos de master skills en un skill tree:

### Passive skills

Cuando se aprenden, estas skills otorgan ciertos power-ups de forma pasiva. Por
ejemplo, hay master skills para incrementar la máxima health o para aumentar el
attack damage.

### Active skills

Al aprenderlas, estas skills aparecen en la skill list. La mayoría de este tipo
reemplaza skills existentes. Las skills reemplazadas permanecen en segundo plano
y no son visibles en el game client, pero su valor interno sigue aplicándose a
la master skill. La master skill solo define cuánto damage o buff se agrega a la
skill reemplazada.

## Estructura del master skill tree

El master skill tree consta de tres roots. Las skills se colocan en filas que
definen el *rank* de una skill. Por defecto hay 5 ranks disponibles; sin embargo,
el game client soporta hasta 9 ranks. Cada skill usualmente puede tener hasta 20
levels, algunas solo 10; el client probablemente soporta más.

Una skill puede depender de una skill del mismo rank o del rank previo del mismo
root.

Cada character class puede aprender diferentes skills, que a veces son exclusivas
de la clase. Una skill puede estar disponible para múltiples character classes,
pero la root y el rank de una skill siempre son los mismos para todas las
character classes. La apariencia visual en el client puede diferir.

## Requisitos para aprender una skill

Para aprender una skill, el server (y el client) realiza las siguientes
verificaciones:

### Character class

La skill debe estar definida para la clase del character.

### Rank

La skill debe estar en el primer rank o una skill del rank previo del mismo root
debe estar al menos en level 10.

### Required Skill

Si la skill tiene required skills definidas (es opcional), estas skills deben
estar al menos en level 10.

## Client implementation

Algunas notas breves sobre el client:

* Existe un message para el master level, etc. (F3 50)
  * Contiene health y mana. Si este packet no se envía, un master character
    aparece sin health/mana.
* Existe un message para las master skills aprendidas (F3 53)
  * La información sobre cada skill contiene:
    * Skill Number
    * Skill Index
      * Define dónde se ubica la skill en la interface del client
      * Me pregunto por qué necesitan esto en el message, ya que el client ya
        conoce el index de una skill
      * Puede diferir entre character classes
    * Current Level
    * Valor de su efecto en el current level
    * Valor de su efecto en el next level
  * Incluso si no se aprendió ninguna skill, este message debe enviarse; de lo
    contrario, puede contener la información master del master character jugado
    anteriormente.

## Server implementation

### Adding Points

* Para código, ver [AddMasterPointAction](https://github.com/MUnique/OpenMU/tree/master/src/GameLogic/PlayerActions/Character/AddMasterPointAction.cs).
* Request Packet: [C1F352 - Add Master Skill Point](Packets/C1-F3-52-AddMasterSkillPoint_by-client.md)
* Response Packet: [C1F352 - Master skill level update](Packets/C1-F3-52-MasterSkillLevelUpdate_by-server.md)

### Sending Master Stats (F3 50)

* Para código, ver [UpdateMasterStatsPlugIn](https://github.com/MUnique/OpenMU/tree/master/src/GameServer/RemoteView/Character/UpdateMasterStatsPlugIn.cs).
* Request Packet: [C2F350 - Master Stats Update](Packets/C1-F3-50-MasterStatsUpdate_by-server.md)

### Sending Master Skills (F3 53)

* Para código, ver [UpdateMasterSkillsPlugIn](https://github.com/MUnique/OpenMU/tree/master/src/GameServer/RemoteView/Character/UpdateMasterSkillsPlugIn.cs).
* Packet: [C2F353 - Master Skill List](Packets/C2-F3-53-MasterSkillList_by-server.md)

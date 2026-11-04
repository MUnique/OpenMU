# Attribute System

This project contains all what's required to create a so-called "Attribute System".
I'm sure it could be helpful also for other games, even other kind of games,
because it's pretty generic and not bound to MU Online or RPGs in general.

## What is it?

In OpenMU, every object which attacks and is attackable (interface IAttackable),
has an instance of an Attribute System. This Attribute System contains all attributes
of this entity.

## And how is it used?

For example a attribute system of a player can contain the following attributes:
  
* Strength
* Agility
* Vitality
* Health
* Level
* Physical Damage (min)
* Physical Damage (max)

Attributes can depend on each other and attributes values are calculated automatically.
For example we can define that Health is calculated by multiplying Vitality by
2 and adding the Level. Or that the minimum physical damage is Strength divided
by 6.
We can define such relationships by AttributeRelationship objects.

To make this work there need to be attributes which don't depend on other ones
and can be influenced by the player. These are called "StatAttributes".
StatAttributes are for example:

* Level (increasable by gaining experience)
* Base Strength (increasable by distributing free level up points)

Attribute values can get increased by equipping items. For example, there are
ancient items which increase Strength when they are equipped. Or more classical,
Swords which would increase the minimum and maximum physical damage attributes.

E.g. for Strength we usually have another attribute which is called "Total Strength",
which is basically the "Base Strength", but it can be a target attribute of an
AttributeRelationship. This "Total Strength" is then used for item requirements
or the further damage calculation. The "Base Strength" is the value which is
usually persisted to the database for a specific player.

In case of this server project, these relationships are all defined the
configuration for each character class. For some examples you can have a look
at the CharacterClassInitialization of the MUnique.OpenMU.Persistence.Initialization
project.

## Tests

This project also includes some unit tests for AttributeSystem, so check it out
if you want to find out how that works as a whole.

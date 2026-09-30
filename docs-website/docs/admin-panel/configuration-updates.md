---
title: Configuration updates
sidebar_position: 3
description: Apply the configuration data updates which ship with new OpenMU versions.
---

# Configuration updates

**Navigation:** *Updates* — route `/config-updates`

OpenMU is under active development, and new versions regularly bring changes to
the **game configuration data**: a corrected monster stat, a new drop item group,
a new plugin configuration, a fixed quest.

Your database keeps the configuration you initialized (and possibly customized),
so those changes cannot simply be applied by updating the software. Instead they
come as *configuration updates*, which you apply here.

The navigation menu shows a badge with the number of available updates, so you
notice them after an upgrade.

## Applying updates

The page lists every update which has not been applied to your database yet, with
its description.

* **Mandatory updates** are always applied and cannot be deselected — the server
  requires them.
* **Optional updates** can be selected or deselected individually, which lets you
  keep your own customizations where an update would overwrite them.

After the run, each update is marked as applied or, when something went wrong,
as failed.

:::warning[A restart is required]
The updates take effect after a restart of the server process. Until then, the
running server still works with the configuration it loaded at startup.
:::

## Preconditions

If the database is not installed yet, or a schema update is pending, this page
refers you to the [Setup page](setup.md) first — configuration updates are
applied on top of a current schema.

## Good practice

* Take a backup of your PostgreSQL database before applying updates to a server
  which has players on it.
* Read the descriptions. If an update touches something you customized (drop
  rates, monster stats), you may want to deselect it and merge the change
  yourself in the [game configuration](game-configuration.md).
* Apply updates during a maintenance window, since a restart is needed anyway.

## Simplified Chinese configuration names

The optional **Add Simplified Chinese configuration names** update adds sourced
Chinese names for built-in classes, maps, merchants, monsters and NPCs. It is
available for 0.75, 0.95d and Season 6 configurations. Freshly initialized databases
already include these names.

Back up the database, apply the update on `/config-updates`, then restart the
application. The update fills missing Chinese translations and replaces English
copies and explicitly listed legacy translations. Other names and languages are
preserved. A customized name identical to a listed legacy translation is also
replaced, so review the mapping before applying it. Gameplay settings are unchanged.
Reapplying the mapping does not create duplicate translations.

Coverage is partial: unverified names retain their existing values. These are
server configuration names; the game's own client language files are unaffected.
The [mapping and sources](https://github.com/MUnique/OpenMU/blob/master/docs/ChineseConfigurationNames.md)
list included names and distinguish derived variant labels from official names.

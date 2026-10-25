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
its description. An update can also reappear with a new version (shown as
`v1 → v2`) when a previously installed update was extended — applying it brings
your database to the current state without duplicating anything.

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

## On a new database

A new database already contains the changes of OpenMU's own updates, because the
initialization creates the current configuration. It therefore marks them as
applied, and this page starts empty.

Plugins can bring configuration updates too, for example one which adds the
plugin's own item. Their changes are not part of OpenMU's initialization, so such
an update declares `RunsOnNewDatabase`, and a new database applies it right away,
after the updates it depends on. On an existing database it is listed here like
any other update.

An optional update of that kind is not applied on a new database: it is listed
here as pending, as on an existing database, so you decide whether to apply it.
The same goes for the updates which would be applied and depend on it. Their
dependencies are checked when you apply them, so an optional plugin update with a
missing dependency doesn't stop the creation of a new database.

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

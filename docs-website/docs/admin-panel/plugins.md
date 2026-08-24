---
title: Plugins
sidebar_position: 8
description: Activate, deactivate and configure the plugins which implement the game features.
---

# Plugins

**Navigation:** *Configuration → Plugins* — route `/plugins`

Large parts of OpenMU are implemented as plugins: packet handlers, view plugins,
chat commands, periodic tasks, item crafting, quest logic, and much more. Each
plugin implements an **extension point** (a plugin interface), and the server
picks the plugins which are active for the client version it is serving.

This page lists every plugin the server knows and lets you turn it on and off.

## The list

| Column | Meaning |
|---|---|
| Extension point | The interface this plugin implements. Hover for its description. |
| Plugin name | The display name. Hover for the plugin's description. |
| Plugin type | The .NET type. Hover for its type id (a GUID). |
| Action | Activate/Deactivate and, if the plugin has one, its configuration |

Above the list you can filter by extension point, by name and by type — useful,
because there are hundreds of plugins.

## Activating and deactivating

**Deactivate** disables a plugin, **Activate** enables it again. This is how you

* switch off a feature you do not want on your server,
* replace a built-in behaviour by your own plugin: deactivate the original,
  activate your extended or modified version.

:::warning[Deactivate carefully]
Many plugins are not optional features but the implementation of a game
mechanic — a packet handler or a view plugin. Deactivating one can make the
client behave strangely or disconnect. If in doubt, note the type name before you
change anything, so you can turn it back on.
:::

## Plugin configuration

Plugins with their own configuration have a ⚙ button which opens it. The
configuration is stored per plugin and, where applicable, per client version, so
the same plugin can behave differently for different clients.

The custom plugin configuration can also contain a *custom plugin source*, which
is how a plugin can be adapted without rebuilding the server.

## When do changes take effect?

Activation state is stored in the configuration. Depending on the extension
point, the change is picked up by the running server or requires a reload — use
**Reload configuration and restart all game servers** on the
[Servers page](servers.md) if you don't see an effect.

## Feature plugins

Some plugins are not a single mechanic but a whole feature with its own
configuration, grouped in the *Feature Plugins* extension point. They are
documented separately:

* [Server-side AI bots](../server-features/bots.md)

## Writing your own plugin

Implementing a plugin is documented next to the code, in the
[MUnique.OpenMU.PlugIns readme](https://github.com/MUnique/OpenMU/blob/master/src/PlugIns/Readme.md).

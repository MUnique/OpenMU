---
title: Plugins
sidebar_position: 3
description: How the plugin system works and how to write your own plugin.
---

# Plugins

Large parts of OpenMU are implemented as plugins, which is what makes the server
extendable without changing its code. The building blocks live in
[`MUnique.OpenMU.PlugIns`](https://github.com/MUnique/OpenMU/tree/master/src/PlugIns),
which has no dependencies to other OpenMU projects and comes with unit tests that
are a good way to understand the mechanics.

Once your plugin exists, it is activated, deactivated and configured in the
[admin panel](../admin-panel/plugins.md).

## The plugin manager

The plugin manager discovers plugins and offers methods to retrieve, activate,
deactivate and manually register them.

## Kinds of plugins

### Regular plugins

The manager collects all plugins of the same type into a dynamically created
proxy object, which iterates through all active plugins when a method is called:

```csharp
// Executes "ExecuteSomeMethod" of all active plugins implementing "ISomePlugIn":
manager.GetPlugInPoint<ISomePlugIn>()?.ExecuteSomeMethod("example parameter");
```

The `?.` operator is needed because there might be no plugin at all.

### Regular plugins with a custom plugin container

A custom container collects all plugins of a common interface and decides which
implementation is currently *effective*. This is how client versions are handled:
the `ViewPlugInContainer` provides the view plugins which fit the connected
client best.

```csharp
var container = new ViewPlugInContainer(manager);
container.GetPlugIn<IChatViewPlugIn>()?.ShowMessage("Bob", "Hello World");
```

### Strategy plugins

Sometimes exactly one plugin should handle one specific case — the typical
example is a chat command:

```csharp
manager.GetStrategy<IChatCommandPlugIn>("/post")?.HandleCommand("/post Hello World");
```

If the key is not a string, its type has to be given explicitly:

```csharp
manager.GetStrategy<long, IAnotherStrategyPlugIn>(1337)?.DoStuff();
```

## Defining a plugin point

A plugin point is an interface with two attributes:

```csharp
/// <summary>
/// Example interface for a plugin.
/// </summary>
[Guid("34AEED37-9D62-4AE1-9320-91BB620B39C2")]
[PlugInPoint("Example PlugIn Point", "This plugin point is an example.")]
public interface IExamplePlugIn
{
    void DoStuff(Player player, string text);
}
```

* **`Guid`** — every plugin interface needs a unique, *fixed* identifier, so it
  can be referenced safely later. Without the attribute it would be some random
  number.
* **`PlugInPoint`** — name, description, and the marker that the plugin manager
  should pick it up.

For a strategy plugin point, the interface additionally extends
`IStrategyPlugIn<TKey>`:

```csharp
[Guid("1E68B14C-9156-448A-A6AB-90E423A8E91C")]
[PlugInPoint("Strategy Plugin Test Interface", "A strategy plugin test interface")]
public interface IExampleStrategyPlugIn : IStrategyPlugIn<string>
{
    void HandleCommand(string command);
}
```

For a custom plugin container, the *common* interface is marked with
`CustomPlugInContainer` instead, and the specialized interfaces extend it:

```csharp
[Guid("D6A56A13-AC5B-442B-B185-857587C59A32")]
[CustomPlugInContainer("Example Custom PlugIn Container", "This plugin container is an example.")]
public interface IViewPlugIn
{
}

public interface IChatViewPlugIn : IViewPlugIn
{
    void ShowMessage(string sender, string message);
}
```

## Implementing a plugin

```csharp
/// <summary>
/// The implementation of the <see cref="IExamplePlugIn"/>.
/// </summary>
[Guid("9FCA692F-2BD5-4310-8755-E20761F94180")]
[PlugIn]
[Display(Name = nameof(ExamplePlugIn), Description = "Just an example plugin.")]
internal class ExamplePlugIn : IExamplePlugIn
{
    /// <inheritdoc />
    public void DoStuff(Player player, string text)
    {
        // Stuff is done here
    }
}
```

Again two attributes are required:

* **`Guid`** — a fixed unique id, so the plugin can be referenced in
  configurations.
* **`PlugIn`** — name, description, and the marker for the plugin manager.

## Configuration and custom plugins

The plugin manager can be initialized with a list of plugin configurations. It
searches for plugins in all loaded assemblies, registers them, and then walks
through the configurations to find the plugin with the corresponding id. If it
can't find one, it assumes a custom/external plugin, loads its assembly and
rediscovers. Finally it applies the `IsActive` flag — which is what the
[Plugins page](../admin-panel/plugins.md) of the admin panel edits.

There are two ways to load custom plugins:

1. By specifying the name of an external assembly which is available in a
   `plugins` subfolder of the server.
2. By putting the **source code** of the plugin into the configuration. It is
   compiled at runtime with Roslyn.

The second option is preferable from a compatibility point of view, because the
source always references the currently loaded assemblies: compile errors show up
at server start and can be fixed quickly.

## Where to look for examples

* Packet handlers: `MUnique.OpenMU.GameServer.MessageHandler`
* View plugins: `MUnique.OpenMU.GameServer.RemoteView`
* Chat commands: the `IChatCommandPlugIn` implementations in
  `MUnique.OpenMU.GameLogic`
* The unit tests in
  [`tests/MUnique.OpenMU.PlugIns.Tests`](https://github.com/MUnique/OpenMU/tree/master/tests/MUnique.OpenMU.PlugIns.Tests)

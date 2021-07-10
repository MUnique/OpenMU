# MUnique.OpenMU.PlugIns

This project contains the building blocks for the plugin system of OpenMU and
doesn't have any dependencies to other OpenMU projects - so it's reuseable, if
required.
It also contains unit tests with almost complete test coverage, which can help
to understand how this all works.

## Plugin Manager

A plugin manager takes care of discovering plugins and offers methods to retrieve,
activate, deactivate and to manually register plugins.

## Plugin Types

The system supports the following kind of plugins. They are used in different
situations.

### Regular Plugins

The plugin manager collects plugins of the same type and puts them into a
dynamically created proxy object which iterates through all active plugins of
the same type when a method of a plugin is executed.

Example:

```csharp
  // The following call would execute the function "ExecuteSomeMethod" of
  // all active plugins which implement "ISomePlugIn":
  manager.GetPlugInPoint<ISomePlugIn>()?.ExecuteSomeMethod("example parameter");
```

Note, that I use the ?. operator here, because if no plugin is defined, we might
get null.

### Regular Plugins, managed by custom plugin containers

In this case, there is a custom plugin container which collects all plugins of
a common plugin interface type. Additionally, there are specific plugin
interfaces which derive from this common interface. A custom plugin container
decides which one of the actual implementations are currently *effective*.

Example:

* We have a ```ViewPlugInContainer : ICustomPlugInContainer<IViewPlugIn>```
  which collects and manages all *IViewPlugIns*.
  Depending of the client version, it provides the plugins which fit best.

* We have a plugin interface ```IChatViewPlugIn : IViewPlugIn```.

* We have several implementations for the *IChatViewPlugIn*, e.g. for different
  client versions.

```csharp
  // assume that we have a manager which has already some active implementations
  // for IChatViewPlugIn available
  var container = new ViewPlugInContainer(manager);
  container.GetPlugIn<IChatViewPlugIn>()?.ShowMessage("Bob", "Hello World");
```

Note, that I use the ?. operator here, because if no *IChatViewPlugIn* is active,
we might get null.

### Strategy Plugins

Sometimes we just want to execute one specific plugin for one specific case.
A typical example are plugins which do something for a specific chat command.

Example:
```csharp
  // The following call would execute the function "HandleCommand" of
  // the active plugin which implements "IChatCommandPlugIn" and is responsible
  // for the command "/post":
  manager.GetStrategy<IChatCommandPlugIn>("/post")?.HandleCommand("/post Hello World");
```

Note, that I use the ?. operator here, because if no strategy plugin is defined
for the key, we might get null.

In this example, there is also some syntactic sugar used. When the key is
something else as a string (or any non-considered type in the future), you must
specify it's type explicitly:
```csharp
  manager.GetStrategy<long, IAnotherStrategyPlugIn>(1337)?.DoStuff();
```

## Defining Plugin Points / Interfaces

To define a plugin point (= interface), we simply add a new interface for it.

I copied this example from the unit tests:

```csharp
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Example interface for a plugin.
/// </summary>
[Guid("34AEED37-9D62-4AE1-9320-91BB620B39C2")]
[PlugInPoint("Example PlugIn Point", "This plugin point is an example.")]
public interface IExamplePlugIn
{
    /// <summary>
    /// Does some stuff.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="text">The text.</param>
    void DoStuff(Player player, string text);
}
```

As you can see, there are two additional attributes at it:

* **Guid**: Every plugin interface needs a unique identifier.
            This id is compiled into ```typeof(IExamplePlugIn).GUID``` -
            when it's missing, it's just some random number. We want fixed GUID,
            so we can safely reference it later. They need to be unique for
            every interface, of course.

* **PlugInPoint**: This one defines the name and description and that it
                   should be picked up by the plugin manager.

### Defining Strategy Plugins

The same applies here, too. We just add, that the plugin interface extends
```IStrategyPlugIn<TKey>```.

Example:

```csharp
using System.Runtime.InteropServices;

/// <summary>
/// Interface for an example strategy plugin, where the strategy key is a string.
/// </summary>
[Guid("1E68B14C-9156-448A-A6AB-90E423A8E91C")]
[PlugInPoint("Strategy Plugin Test Interface", "A strategy plugin test interface")]
public interface IExampleStrategyPlugIn : IStrategyPlugIn<string>
{
    /// <summary>
    /// Handles the command.
    /// </summary>
    /// <param name="command">The command.</param>
    void HandleCommand(string command);
}
```

### Defining custom plugin containers and plugins

This works slightly different from what you have seen above. Instead of using
the ```PlugInPointAttribute```, the common interface has to be marked with
the ```CustomPlugInContainerAttribute```.
The more specialized plugin interfaces have to extend this interface.

I'd like to pick up the previously mentioned example:

```csharp
using System.Runtime.InteropServices;

/// <summary>
/// Common interface for all plugins of a custom plugin container.
/// </summary>
[Guid("D6A56A13-AC5B-442B-B185-857587C59A32")]
[CustomPlugInContainer(
  "Example Custom PlugIn Container",
  "This plugin container is an example.")]
public interface IViewPlugIn
{
}

// we don't need additional attributes here, instead we extend IViewPlugIn.
public interface IChatViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the message in the game client.
    /// </summary>
    /// <param name="sender">The name of the sender.</param>
    /// <param name="message">The message.</param>
    void ShowMessage(string sender, string message);
}
```

## Implementing Plugins

To implement the actual plugins, we just implement the previously defined interfaces.

Example:

```csharp
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The implementation of the <see cref="IExamplePlugIn"/>.
/// </summary>
/// <seealso cref="IExamplePlugIn" />
[Guid("9FCA692F-2BD5-4310-8755-E20761F94180")]
[PlugIn(nameof(ExamplePlugIn), "Just an example plugin.")]
internal class ExamplePlugIn : IExamplePlugIn
{
    /// <inheritdoc />
    public void DoStuff(Player player, string text)
    {
        // Stuff is done here
    }
}
```

Again, there are two additionally required attributes:

* **Guid**: Every plugin needs a unique identifier.
            This id is compiled into ```typeof(ExamplePlugIn).GUID``` -
            when it's missing, it's just some random number. We want fixed GUIDs,
            so we can safely reference it in configurations. They need to be
            unique for every implemented plugin, of course.

* **PlugIn**: This one defines the name and description and that it should be
              picked up by the plugin manager.

## Configuration

It's possible to initialize the plugin manager with a list of plugin configurations
by passing them into the constructor.
When this is done, it automatically searches for plugins in all currently
loaded assemblies and registers them. Then it iterates through all given
configurations, and tries to find the plugin with the corresponding id. If it
can't find it, it looks if it's a custom/external plugin and tries to load
their assemblies and rediscovers them.
If it's then available, it deactivates the plugin based on the IsActive flag.

So, that's a simple mechanism to configure existing plugins, and to extend it
by custom ones. Currently, there are two ways to load custom plugins:

* By specifying the name of an external assembly which is available in a "plugins"
  subfolder of the server.

* By adding the source code of the plugin into the configuration. It's compiled
  at runtime with Roslyn.

From a compatibility point of view, the last option is to prefer, because the
source is always referencing the currently loaded assemblies. Compile errors
would come up on the start of the server and could be fixed in a short time.

// <copyright file="PlugInManagerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns.Tests;

using System.ComponentModel.Design;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Tests;

/// <summary>
/// Tests for the <see cref="PlugInManager"/>.
/// </summary>
[TestFixture]
public class PlugInManagerTest
{
    /// <summary>
    /// Tests if registering a plugin type creates a proxy for it.
    /// </summary>
    [Test]
    public void RegisteringPlugInCreatesProxy()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), this.CreateServiceProvider(), null);
        manager.RegisterPlugIn<IExamplePlugIn, ExamplePlugIn>();

        var point = manager.GetPlugInPoint<IExamplePlugIn>();
        Assert.That(point, Is.InstanceOf<IExamplePlugIn>());
        Assert.That(point, Is.InstanceOf<IPlugInContainer<IExamplePlugIn>>());
    }

    /// <summary>
    /// Tests if registered plugins are active by default.
    /// </summary>
    [Test]
    public async ValueTask RegisteredPlugInsActiveByDefaultAsync()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), this.CreateServiceProvider(), null);
        var plugIn = new ExamplePlugIn();
        manager.RegisterPlugInAtPlugInPoint<IExamplePlugIn>(plugIn);

        var player = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var command = "test";
        var args = new MyEventArgs();

        var point = manager.GetPlugInPoint<IExamplePlugIn>();
        point!.DoStuff(player, command, args);
        Assert.That(plugIn.WasExecuted, Is.True);
    }

    /// <summary>
    /// Tests if plugins can be deactivated and are not executed if they are.
    /// </summary>
    [Test]
    public async ValueTask DeactivatingPlugInsAsync()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), this.CreateServiceProvider(), null);
        var plugIn = new ExamplePlugIn();
        manager.RegisterPlugInAtPlugInPoint<IExamplePlugIn>(plugIn);
        manager.DeactivatePlugIn<ExamplePlugIn>();

        var player = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var command = "test";
        var args = new MyEventArgs();

        var point = manager.GetPlugInPoint<IExamplePlugIn>();
        point!.DoStuff(player, command, args);
        Assert.That(plugIn.WasExecuted, Is.False);
    }

    /// <summary>
    /// Tests if deactivating a deactivated plugin doesn't cause issues.
    /// </summary>
    [Test]
    public async ValueTask DeactivatingDeactivatedPlugInAsync()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), this.CreateServiceProvider(), null);
        var plugIn = new ExamplePlugIn();
        manager.RegisterPlugInAtPlugInPoint<IExamplePlugIn>(plugIn);
        manager.DeactivatePlugIn<ExamplePlugIn>();
        manager.DeactivatePlugIn<ExamplePlugIn>();

        var player = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var command = "test";
        var args = new MyEventArgs();

        var point = manager.GetPlugInPoint<IExamplePlugIn>();
        point!.DoStuff(player, command, args);
        Assert.That(plugIn.WasExecuted, Is.False);
    }

    /// <summary>
    /// Tests if activating an activated plugin doesn't cause issues.
    /// </summary>
    [Test]
    public async ValueTask ActivatingActivatedPlugInAsync()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), this.CreateServiceProvider(), null);
        var plugIn = new ExamplePlugIn();
        manager.RegisterPlugInAtPlugInPoint<IExamplePlugIn>(plugIn);
        manager.ActivatePlugIn<ExamplePlugIn>();
        manager.ActivatePlugIn<ExamplePlugIn>();

        var player = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var command = "test";
        var args = new MyEventArgs();

        var point = manager.GetPlugInPoint<IExamplePlugIn>();
        point!.DoStuff(player, command, args);
        Assert.That(plugIn.WasExecuted, Is.True);
    }

    /// <summary>
    /// Tests if deactivating a plugin doesn't affect another plugin.
    /// </summary>
    [Test]
    public async ValueTask DeactivatingOnePlugInDoesntAffectOthersAsync()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), this.CreateServiceProvider(), null);
        var plugIn = new ExamplePlugIn();
        manager.RegisterPlugInAtPlugInPoint<IExamplePlugIn>(plugIn);
        manager.RegisterPlugIn<IExamplePlugIn, ExamplePlugIn.NestedPlugIn>();
        manager.DeactivatePlugIn<ExamplePlugIn.NestedPlugIn>();
        manager.ActivatePlugIn<ExamplePlugIn.NestedPlugIn>();
        manager.DeactivatePlugIn<ExamplePlugIn.NestedPlugIn>();

        var player = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var command = "test";
        var args = new MyEventArgs();

        var point = manager.GetPlugInPoint<IExamplePlugIn>();
        point!.DoStuff(player, command, args);
        Assert.That(plugIn.WasExecuted, Is.True);
    }

    /// <summary>
    /// Tests if the plugins are in the correct active-state if they got created with a <see cref="PlugInConfiguration"/>.
    /// </summary>
    /// <param name="active">If set to <c>true</c>, the <see cref="PlugInConfiguration"/> is configured to be active.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async ValueTask CreatedAndActiveByConfigurationAsync(bool active)
    {
        var configuration = new PlugInConfiguration
        {
            TypeId = typeof(ExamplePlugIn).GUID,
            IsActive = active,
        };
        var manager = new PlugInManager(new List<PlugInConfiguration> { configuration }, new NullLoggerFactory(), this.CreateServiceProvider(), null);
        var player = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var command = "test";
        var args = new MyEventArgs();

        var point = manager.GetPlugInPoint<IExamplePlugIn>();
        point!.DoStuff(player, command, args);
        Assert.That(args.WasExecuted, Is.EqualTo(active));
    }

    /// <summary>
    /// Tests if a custom plugin in a non-existing assembly is not created and throws no errors.
    /// </summary>
    [Test]
    public void CustomPlugInByExternalAssemblyNotFoundDoesntThrowError()
    {
        var configuration = new PlugInConfiguration
        {
            TypeId = new Guid("D88B1ACA-42B7-4A89-B3E0-3C97AA4C8578"),
            IsActive = true,
            ExternalAssemblyName = "DoesNotExist.dll",
        };
        _ = new PlugInManager(new List<PlugInConfiguration> { configuration }, new NullLoggerFactory(), this.CreateServiceProvider(), null);
    }

    /// <summary>
    /// Tests if an unknown plugin in the configuration doesn't cause exceptions.
    /// </summary>
    [Test]
    public void UnknownPlugInByConfigurationDoesntThrowError()
    {
        var configuration = new PlugInConfiguration
        {
            TypeId = new Guid("A9BDA3E2-4EB6-45C3-B234-37C1819C0CB6"),
            IsActive = true,
        };
        _ = new PlugInManager(new List<PlugInConfiguration> { configuration }, new NullLoggerFactory(), this.CreateServiceProvider(), null);
    }

    /// <summary>
    /// Tests if activating an unknown plugin doesn't cause exceptions.
    /// </summary>
    [Test]
    public void ActivatingUnknownPlugInDoesNotThrowError()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), this.CreateServiceProvider(), null);
        manager.ActivatePlugIn(new Guid("4C38A813-F9BF-428A-8EA1-A6C90A87E583"));
    }

    /// <summary>
    /// Tests if deactivating an unknown plugin doesn't cause exceptions.
    /// </summary>
    [Test]
    public void DeactivatingUnknownPlugInDoesNotThrowError()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), this.CreateServiceProvider(), null);
        manager.ActivatePlugIn(new Guid("4C38A813-F9BF-428A-8EA1-A6C90A87E583"));
    }

    /// <summary>
    /// Tests if plugins can be activated and are executed if they are.
    /// </summary>
    [Test]
    public async ValueTask ActivatingPlugInsAsync()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), this.CreateServiceProvider(), null);
        var plugIn = new ExamplePlugIn();
        manager.RegisterPlugInAtPlugInPoint<IExamplePlugIn>(plugIn);
        manager.DeactivatePlugIn<ExamplePlugIn>();
        manager.ActivatePlugIn<ExamplePlugIn>();

        var player = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var command = "test";
        var args = new MyEventArgs();

        var point = manager.GetPlugInPoint<IExamplePlugIn>();
        point!.DoStuff(player, command, args);
        Assert.That(plugIn.WasExecuted, Is.True);
    }

    /// <summary>
    /// Tests the automatic discovery of plugins of the loaded assemblies.
    /// </summary>
    [Test]
    public void AutoDiscovery()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), this.CreateServiceProvider(), null);
        manager.DiscoverAndRegisterPlugIns();
        var examplePlugInPoint = manager.GetPlugInPoint<IExamplePlugIn>();
        Assert.That(examplePlugInPoint, Is.InstanceOf<IExamplePlugIn>());
    }

    /// <summary>
    /// Tests if registering a plug in without a unique identifier throws an error.
    /// </summary>
    [Test]
    public void RegisteringPlugInWithoutGuidThrowsError()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), this.CreateServiceProvider(), null);
        Assert.Throws<ArgumentException>(() => manager.RegisterPlugIn<IExamplePlugIn, ExamplePlugIn.NestedWithoutGuid>());
    }

    /// <summary>
    /// Tests if the strategy provider is created for registered strategy plug in.
    /// </summary>
    [Test]
    public void StrategyProviderCreatedForRegisteredStrategyPlugIn()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), this.CreateServiceProvider(), null);
        manager.RegisterPlugIn<IExampleStrategyPlugIn, ExampleStrategyPlugIn>();

        var strategyProvider = manager.GetStrategyProvider<string, IExampleStrategyPlugIn>();
        Assert.That(strategyProvider, Is.Not.Null);
    }

    /// <summary>
    /// Tests if the strategy provider is not created when there is no registered strategy plug in yet.
    /// </summary>
    [Test]
    public void StrategyProviderNotCreatedWithoutRegisteredStrategyPlugIn()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), this.CreateServiceProvider(), null);

        var strategyProvider = manager.GetStrategyProvider<string, IExampleStrategyPlugIn>();
        Assert.That(strategyProvider, Is.Null);
    }

    /// <summary>
    /// Tests if the registered strategy plug in is available.
    /// </summary>
    [Test]
    public void RegisteredStrategyPlugInAvailable()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), this.CreateServiceProvider(), null);
        manager.RegisterPlugIn<IExampleStrategyPlugIn, ExampleStrategyPlugIn>();

        var strategy = manager.GetStrategy<IExampleStrategyPlugIn>(ExampleStrategyPlugIn.CommandKey);
        Assert.That(strategy, Is.Not.Null);
        Assert.That(strategy, Is.TypeOf<ExampleStrategyPlugIn>());
    }

    /// <summary>
    /// Tests if the registered, but deactivated strategy plug in is not available.
    /// </summary>
    [Test]
    public void DeactivatedStrategyPlugInNotAvailable()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), this.CreateServiceProvider(), null);
        manager.RegisterPlugIn<IExampleStrategyPlugIn, ExampleStrategyPlugIn>();
        manager.DeactivatePlugIn<ExampleStrategyPlugIn>();
        var strategy = manager.GetStrategy<IExampleStrategyPlugIn>(ExampleStrategyPlugIn.CommandKey);
        Assert.That(strategy, Is.Null);
    }

    /// <summary>
    /// Tests if registering an already registered strategy plug in does not throw an error.
    /// </summary>
    [Test]
    public void RegisteringRegisteredStrategyPlugInDoesntThrowError()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), this.CreateServiceProvider(), null);
        manager.RegisterPlugIn<IExampleStrategyPlugIn, ExampleStrategyPlugIn>();
        manager.RegisterPlugIn<IExampleStrategyPlugIn, ExampleStrategyPlugIn>();
    }

    /// <summary>
    /// Tests if no plug in point is created and returned for strategy plug ins.
    /// </summary>
    [Test]
    public void NoPlugInPointForStrategyPlugIn()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), this.CreateServiceProvider(), null);
        manager.RegisterPlugIn<IExampleStrategyPlugIn, ExampleStrategyPlugIn>();
        Assert.That(manager.GetPlugInPoint<IExampleStrategyPlugIn>(), Is.Null);
    }

    private IServiceProvider CreateServiceProvider()
    {
        var provider = new ServiceContainer();
        provider.AddService(typeof(ILoggerFactory), new NullLoggerFactory());
        return provider;
    }
}
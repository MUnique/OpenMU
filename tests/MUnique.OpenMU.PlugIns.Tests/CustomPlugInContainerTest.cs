// <copyright file="CustomPlugInContainerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns.Tests;

using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

/// <summary>
/// Tests for the <see cref="PlugInManager"/>.
/// </summary>
[TestFixture]
public class CustomPlugInContainerTest
{
    /// <summary>
    /// Tests if creating the plugin container with an interface without a <see cref="CustomPlugInContainerAttribute"/> throws an exception.
    /// </summary>
    [Test]
    public void CreatingContainerWithNonMarkedTypeThrowsException()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        var mock = new Mock<CustomPlugInContainerBase<ITestCustomPlugIn>>(manager);
        var exception = Assert.Throws<TargetInvocationException>(() =>
        {
            _ = mock.Object;
        });

        Assert.That(exception?.InnerException, Is.InstanceOf<ArgumentException>());
    }

    /// <summary>
    /// Tests if a plugin can be retrieved from the custom container, when the plugin has been registered after the container was created.
    /// </summary>
    [Test]
    public void GetPlugInFromCustomContainerWithRegisteredPlugInAfterRegistration()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        var container = new CustomTestPlugInContainer(manager);
        manager.RegisterPlugIn<ITestCustomPlugIn, TestCustomPlugIn>();

        var plugIn = container.GetPlugIn<ITestCustomPlugIn>();
        Assert.That(plugIn, Is.Not.Null);
    }

    /// <summary>
    /// Tests if a plugin can be retrieved from the custom container, when the plugin has been registered before the container was created.
    /// </summary>
    [Test]
    public void GetPlugInFromCustomContainerWithInitiallyRegisteredPlugIn()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<ITestCustomPlugIn, TestCustomPlugIn>();
        var container = new CustomTestPlugInContainer(manager);
        var plugIn = container.GetPlugIn<ITestCustomPlugIn>();
        Assert.That(plugIn, Is.Not.Null);
    }

    /// <summary>
    /// Tests if a plugin can't be retrieved from the custom container when the plugin has been deactivated before.
    /// </summary>
    [Test]
    public void DontGetPlugInFromCustomContainerAfterDeactivation()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<ITestCustomPlugIn, TestCustomPlugIn>();
        var container = new CustomTestPlugInContainer(manager);
        manager.DeactivatePlugIn<TestCustomPlugIn>();
        var plugIn = container.GetPlugIn<ITestCustomPlugIn>();
        Assert.That(plugIn, Is.Null);
    }

    /// <summary>
    /// Tests if a plugin can't be retrieved from the custom container when the plugin isn't suitable for the container and therefore not effective.
    /// </summary>
    [Test]
    public void DontGetPlugInFromCustomContainerIfItDoesntSuit()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        var container = new CustomTestPlugInContainer(manager) { CreateNewPlugIns = false };
        manager.RegisterPlugIn<ITestCustomPlugIn, TestCustomPlugIn>();
        var plugIn = container.GetPlugIn<ITestCustomPlugIn>();
        Assert.That(plugIn, Is.Null);
    }

    /// <summary>
    /// Tests if the custom container replaces the plugin implementation if a new (more suitable) plugin is registered.
    /// </summary>
    [Test]
    public void ReplacePlugInAtCustomContainer()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<ITestCustomPlugIn, TestCustomPlugIn>();
        var container = new CustomTestPlugInContainer(manager);
        manager.RegisterPlugIn<ITestCustomPlugIn, TestCustomPlugIn2>();
        var plugIn = container.GetPlugIn<ITestCustomPlugIn>();
        Assert.That(plugIn, Is.InstanceOf<TestCustomPlugIn2>());
    }

    /// <summary>
    /// Tests if a plugin which got deactivated by another plugin, gets reactivated as soon as the other plugin gets deactivated.
    /// </summary>
    [Test]
    public void ReactivatePlugInAtCustomContainer()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<ITestCustomPlugIn, TestCustomPlugIn>();
        var container = new CustomTestPlugInContainer(manager);
        manager.RegisterPlugIn<ITestCustomPlugIn, TestCustomPlugIn2>();
        manager.DeactivatePlugIn<TestCustomPlugIn2>();
        var plugIn = container.GetPlugIn<ITestCustomPlugIn>();
        Assert.That(plugIn, Is.InstanceOf<TestCustomPlugIn>());
    }

    /// <summary>
    /// Tests if a plugin can be retrieved with both of its implemented interfaces.
    /// </summary>
    [Test]
    public void GetPlugInFromCustomContainerWithAllImplementedInterfaces()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        var container = new CustomTestPlugInContainer(manager);
        container.AddPlugIn(new TestCustomPlugIn2(), true);
        Assert.That(container.GetPlugIn<ITestCustomPlugIn>(), Is.Not.Null);
        Assert.That(container.GetPlugIn<IAnotherCustomPlugIn>(), Is.Not.Null);
    }
}
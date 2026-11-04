// <copyright file="GeneratedPlugInProxyTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns.Tests;

using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.Tests;

/// <summary>
/// Tests for the plugin proxies, which are generated at compile time by the <c>PlugInProxyGenerator</c>.
/// </summary>
[TestFixture]
public class GeneratedPlugInProxyTest
{
    /// <summary>
    /// Tests that every plugin point of the OpenMU assemblies has a generated proxy,
    /// so that no proxy needs to be compiled at runtime.
    /// </summary>
    /// <param name="assembly">The assembly which contains plugin points.</param>
    [TestCaseSource(nameof(AssembliesWithPlugInPoints))]
    public void AllPlugInPointsHaveGeneratedProxies(Assembly assembly)
    {
        var plugInPoints = assembly.GetTypes()
            .Where(t => t.IsInterface && t.GetCustomAttribute<PlugInPointAttribute>() is not null)
            .Where(t => !t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IStrategyPlugIn<>)))
            .ToList();
        Assert.That(plugInPoints, Is.Not.Empty);

        var manager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        var tryCreateMethod = typeof(PlugInProxyRegistry).GetMethod(nameof(PlugInProxyRegistry.TryCreate), BindingFlags.Static | BindingFlags.NonPublic)!;
        var missing = plugInPoints
            .Where(plugInPoint => tryCreateMethod.MakeGenericMethod(plugInPoint).Invoke(null, [manager, null]) is not true)
            .Select(plugInPoint => plugInPoint.FullName)
            .ToList();

        Assert.That(missing, Is.Empty);
    }

    /// <summary>
    /// Tests that the <see cref="PlugInManager"/> uses the generated proxy instead of compiling one at runtime.
    /// </summary>
    [Test]
    public void PlugInManagerUsesGeneratedProxy()
    {
        var manager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        manager.RegisterPlugIn<IExamplePlugIn, ExamplePlugIn>();

        var point = manager.GetPlugInPoint<IExamplePlugIn>();

        Assert.That(point, Is.Not.Null);
        Assert.That(point!.GetType().Assembly, Is.SameAs(typeof(IExamplePlugIn).Assembly));
        Assert.That(point.GetType().Assembly.IsDynamic, Is.False);
    }

    /// <summary>
    /// Tests that all active plugins are executed by the generated proxy.
    /// </summary>
    [Test]
    public async ValueTask MultiplePlugInsAreExecutedAsync()
    {
        var proxy = CreateProxy<IExamplePlugIn>();
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var args = new MyEventArgs();
        var firstMock = new Mock<IExamplePlugIn>();
        var secondMock = new Mock<IExamplePlugIn>();
        firstMock.Setup(p => p.DoStuff(player, "test", args)).Verifiable();
        secondMock.Setup(p => p.DoStuff(player, "test", args)).Verifiable();
        proxy.AddPlugIn(firstMock.Object, true);
        proxy.AddPlugIn(secondMock.Object, true);

        ((IExamplePlugIn)proxy).DoStuff(player, "test", args);

        firstMock.VerifyAll();
        secondMock.VerifyAll();
    }

    /// <summary>
    /// Tests that all active plugins are executed by the generated proxy of an asynchronous method.
    /// </summary>
    [Test]
    public async ValueTask MultipleAsyncPlugInsAreExecutedAsync()
    {
        var proxy = CreateProxy<PlugInProxyTypeGeneratorTest.IAsyncPlugIn>();
        var firstMock = new Mock<PlugInProxyTypeGeneratorTest.IAsyncPlugIn>();
        var secondMock = new Mock<PlugInProxyTypeGeneratorTest.IAsyncPlugIn>();
        firstMock.Setup(p => p.MyMethodAsync()).Verifiable();
        secondMock.Setup(p => p.MyMethodAsync()).Verifiable();
        proxy.AddPlugIn(firstMock.Object, true);
        proxy.AddPlugIn(secondMock.Object, true);

        await ((PlugInProxyTypeGeneratorTest.IAsyncPlugIn)proxy).MyMethodAsync().ConfigureAwait(false);

        firstMock.VerifyAll();
        secondMock.VerifyAll();
    }

    /// <summary>
    /// Tests that inactive plugins are not executed by the generated proxy.
    /// </summary>
    [Test]
    public async ValueTask InactivePlugInsAreNotExecutedAsync()
    {
        var proxy = CreateProxy<IExamplePlugIn>();
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var args = new MyEventArgs();
        var firstMock = new Mock<IExamplePlugIn>();
        var secondMock = new Mock<IExamplePlugIn>();
        secondMock.Setup(p => p.DoStuff(player, "test", args)).Verifiable();
        proxy.AddPlugIn(firstMock.Object, false);
        proxy.AddPlugIn(secondMock.Object, true);

        ((IExamplePlugIn)proxy).DoStuff(player, "test", args);

        firstMock.VerifyNoOtherCalls();
        secondMock.VerifyAll();
    }

    /// <summary>
    /// Tests that the generated proxy stops calling plugins after one of them cancelled the event.
    /// </summary>
    [Test]
    public async ValueTask CancelEventArgsAreRespectedAsync()
    {
        var proxy = CreateProxy<IExamplePlugIn>();
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var args = new MyEventArgs();
        var firstMock = new Mock<IExamplePlugIn>();
        var secondMock = new Mock<IExamplePlugIn>();
        firstMock.Setup(p => p.DoStuff(player, "test", args)).Callback(() => args.Cancel = true).Verifiable();
        proxy.AddPlugIn(firstMock.Object, true);
        proxy.AddPlugIn(secondMock.Object, true);

        ((IExamplePlugIn)proxy).DoStuff(player, "test", args);

        firstMock.VerifyAll();
        secondMock.VerifyNoOtherCalls();
    }

    private static IEnumerable<Assembly> AssembliesWithPlugInPoints()
    {
        // The other assemblies just contain strategy plugin points, which don't need a proxy.
        yield return typeof(GameLogic.GameContext).Assembly;
    }

    private static IPlugInContainer<TPlugIn> CreateProxy<TPlugIn>()
        where TPlugIn : class
    {
        Assert.That(PlugInProxyRegistry.TryCreate<TPlugIn>(new PlugInManager(null, NullLoggerFactory.Instance, null, null), out var proxy), Is.True);
        return proxy!;
    }
}

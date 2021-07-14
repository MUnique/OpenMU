// <copyright file="PlugInProxyTypeGeneratorTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns.Tests
{
    using System;
    using System.ComponentModel;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using MUnique.OpenMU.Tests;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="PlugInProxyTypeGenerator"/>.
    /// </summary>
    [TestFixture]
    public class PlugInProxyTypeGeneratorTest
    {
        /// <summary>
        /// An interface with an unsupported method signature.
        /// </summary>
        [PlugInPoint("Foo", "Bar")]
        internal interface IUnsupportedPlugIn
        {
            /// <summary>
            /// Unsupported method.
            /// </summary>
            /// <returns>Some boolean.</returns>
            bool UnsupportedMethod();
        }

        /// <summary>
        /// Tests the proxy creation for <see cref="IExamplePlugIn"/>.
        /// </summary>
        [Test]
        public void ProxyIsCreated()
        {
            var generator = new PlugInProxyTypeGenerator();
            var proxy = generator.GenerateProxy<IExamplePlugIn>(new PlugInManager(null, new NullLoggerFactory(), null));

            Assert.That(proxy, Is.Not.Null);
        }

        /// <summary>
        /// Tests if multiple plugins are executed.
        /// </summary>
        [Test]
        public void MultiplePlugInsAreExecuted()
        {
            var generator = new PlugInProxyTypeGenerator();
            var proxy = generator.GenerateProxy<IExamplePlugIn>(new PlugInManager(null, NullLoggerFactory.Instance, null));

            var player = TestHelper.CreatePlayer();
            var command = "test";
            var args = new MyEventArgs();
            var firstMock = new Mock<IExamplePlugIn>();
            var secondMock = new Mock<IExamplePlugIn>();

            firstMock.Setup(p => p.DoStuff(player, command, args)).Verifiable();
            secondMock.Setup(p => p.DoStuff(player, command, args)).Verifiable();
            proxy.AddPlugIn(firstMock.Object, true);
            proxy.AddPlugIn(secondMock.Object, true);

            (proxy as IExamplePlugIn)?.DoStuff(player, command, args);

            firstMock.VerifyAll();
            secondMock.VerifyAll();
        }

        /// <summary>
        /// Tests if inactive plugins are not executed.
        /// </summary>
        [Test]
        public void InactivePlugInsAreNotExecuted()
        {
            var generator = new PlugInProxyTypeGenerator();
            var proxy = generator.GenerateProxy<IExamplePlugIn>(new PlugInManager(null, NullLoggerFactory.Instance, null));

            var player = TestHelper.CreatePlayer();
            var command = "test";
            var args = new MyEventArgs();
            var firstMock = new Mock<IExamplePlugIn>();
            var secondMock = new Mock<IExamplePlugIn>();

            secondMock.Setup(p => p.DoStuff(player, command, args)).Verifiable();
            proxy.AddPlugIn(firstMock.Object, false);
            proxy.AddPlugIn(secondMock.Object, true);

            (proxy as IExamplePlugIn)?.DoStuff(player, command, args);

            firstMock.VerifyAll();
            firstMock.VerifyNoOtherCalls();
            secondMock.VerifyAll();
        }

        /// <summary>
        /// Tests if parameters of <see cref="CancelEventArgs"/> are respected, so that when <see cref="CancelEventArgs.Cancel"/> is <c>true</c>,
        /// next plugins are not executed anymore.
        /// </summary>
        [Test]
        public void CancelEventArgsAreRespected()
        {
            var generator = new PlugInProxyTypeGenerator();
            var proxy = generator.GenerateProxy<IExamplePlugIn>(new PlugInManager(null, NullLoggerFactory.Instance, null));

            var player = TestHelper.CreatePlayer();
            var command = "test";
            var args = new MyEventArgs();
            var firstMock = new Mock<IExamplePlugIn>();
            var secondMock = new Mock<IExamplePlugIn>();

            firstMock.Setup(p => p.DoStuff(player, command, args)).Callback(() => args.Cancel = true).Verifiable();
            proxy.AddPlugIn(firstMock.Object, true);
            proxy.AddPlugIn(secondMock.Object, true);

            (proxy as IExamplePlugIn)?.DoStuff(player, command, args);
            firstMock.VerifyAll();
            secondMock.VerifyAll();
            secondMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests if the proxy creation fails when a class is passed as proxy interface type.
        /// </summary>
        [Test]
        public void ErrorForClasses()
        {
            var generator = new PlugInProxyTypeGenerator();
            Assert.Throws<ArgumentException>(() => generator.GenerateProxy<ExamplePlugIn>(new PlugInManager(null, new NullLoggerFactory(), null)));
        }

        /// <summary>
        /// Tests if the proxy creation fails when an interface without <see cref="PlugInPointAttribute"/> is passed as proxy interface type.
        /// </summary>
        [Test]
        public void ErrorForInterfaceWithoutAttribute()
        {
            var generator = new PlugInProxyTypeGenerator();
            Assert.Throws<ArgumentException>(() => generator.GenerateProxy<ICloneable>(new PlugInManager(null, new NullLoggerFactory(), null)));
        }

        /// <summary>
        /// Tests if the proxy creation fails when an interface with an unsupported method signature is passed as proxy interface type.
        /// </summary>
        [Test]
        public void ErrorForInterfaceWithUnsupportedMethodSignature()
        {
            var generator = new PlugInProxyTypeGenerator();
            Assert.Throws<ArgumentException>(() => generator.GenerateProxy<IUnsupportedPlugIn>(new PlugInManager(null, new NullLoggerFactory(), null)));
        }
    }
}

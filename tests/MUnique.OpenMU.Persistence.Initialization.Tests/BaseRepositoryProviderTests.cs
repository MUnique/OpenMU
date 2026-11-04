// <copyright file="BaseRepositoryProviderTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Threading;
using Moq;

/// <summary>
/// Tests for the <see cref="BaseRepositoryProvider"/>.
/// </summary>
[TestFixture]
public class BaseRepositoryProviderTests
{
    /// <summary>
    /// Tests that concurrent first accesses initialize the provider only once.
    /// Previously, each of them ran the initialization, which failed when registering
    /// the same repository again, e.g. when players logged in at the same time.
    /// </summary>
    [Test]
    public async Task ConcurrentFirstAccessesInitializeOnceAsync()
    {
        var provider = new SlowlyInitializingRepositoryProvider();
        using var start = new ManualResetEventSlim();
        var accesses = Enumerable.Range(0, 8)
            .Select(_ => Task.Run(() =>
            {
                start.Wait();
                return provider.GetRepository(typeof(string));
            }))
            .ToList();

        start.Set();
        var repositories = await Task.WhenAll(accesses).ConfigureAwait(false);

        Assert.That(provider.InitializationCount, Is.EqualTo(1));
        Assert.That(repositories, Has.All.Not.Null);
    }

    private sealed class SlowlyInitializingRepositoryProvider : BaseRepositoryProvider
    {
        private int _initializationCount;

        public int InitializationCount => this._initializationCount;

        protected override void Initialize()
        {
            Interlocked.Increment(ref this._initializationCount);

            // Gives the other accesses the time to arrive while the initialization is running.
            Thread.Sleep(100);
            this.RegisterRepository(typeof(string), new Mock<IRepository>().Object);
            base.Initialize();
        }
    }
}

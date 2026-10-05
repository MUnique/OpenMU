// <copyright file="CachedRepositoryTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Threading;
using Moq;
using MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// Tests for the <see cref="CachedRepository{T}"/>.
/// </summary>
[TestFixture]
public class CachedRepositoryTests
{
    /// <summary>
    /// Tests that concurrent calls load the data of the base repository only once.
    /// </summary>
    [Test]
    public async Task ConcurrentCallsLoadOnceAsync()
    {
        var item = new IdentifiableObject { Id = Guid.NewGuid() };
        var loader = new GatedLoader(item);
        var repository = new CachedRepository<IdentifiableObject>(loader.CreateRepository());

        var calls = Enumerable.Range(0, 8).Select(_ => repository.GetAllAsync().AsTask()).ToList();
        loader.Gate.SetResult();
        var results = await Task.WhenAll(calls).ConfigureAwait(false);

        Assert.That(loader.LoadCount, Is.EqualTo(1));
        Assert.That(results, Has.All.EquivalentTo(new[] { item }));
    }

    /// <summary>
    /// Tests that a caller which waited for a failing load doesn't get an empty result, but loads again.
    /// Previously, it got the empty cache.
    /// </summary>
    [Test]
    public async Task WaitingCallerLoadsAgainWhenTheLoadFailedAsync()
    {
        var item = new IdentifiableObject { Id = Guid.NewGuid() };
        var loader = new GatedLoader(item) { FailFirstLoad = true };
        var repository = new CachedRepository<IdentifiableObject>(loader.CreateRepository());

        var failingCall = repository.GetAllAsync().AsTask();
        var waitingCall = repository.GetByIdAsync(item.Id).AsTask();
        loader.Gate.SetResult();

        Assert.ThrowsAsync<InvalidOperationException>(() => failingCall);
        Assert.That(await waitingCall.ConfigureAwait(false), Is.SameAs(item));
        Assert.That(loader.LoadCount, Is.EqualTo(2));
    }

    /// <summary>
    /// A simple identifiable object.
    /// </summary>
    public sealed class IdentifiableObject : IIdentifiable
    {
        /// <inheritdoc />
        public Guid Id { get; set; }
    }

    /// <summary>
    /// Loads the item for a mocked base repository after the gate is opened.
    /// </summary>
    private sealed class GatedLoader
    {
        private readonly IdentifiableObject _item;

        private int _loadCount;

        public GatedLoader(IdentifiableObject item)
        {
            this._item = item;
        }

        public TaskCompletionSource Gate { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public bool FailFirstLoad { get; init; }

        public int LoadCount => this._loadCount;

        public IRepository<IdentifiableObject> CreateRepository()
        {
            var repository = new Mock<IRepository<IdentifiableObject>>();
            repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).Returns(() => this.LoadAsync());
            return repository.Object;
        }

        private async ValueTask<IEnumerable<IdentifiableObject>> LoadAsync()
        {
            var loadCount = Interlocked.Increment(ref this._loadCount);
            await this.Gate.Task.ConfigureAwait(false);
            if (this.FailFirstLoad && loadCount == 1)
            {
                throw new InvalidOperationException("The first load failed.");
            }

            return [this._item];
        }
    }
}

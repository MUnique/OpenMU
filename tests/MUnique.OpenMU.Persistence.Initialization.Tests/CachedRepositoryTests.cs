// <copyright file="CachedRepositoryTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Collections;
using System.Threading;
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
        var baseRepository = new GatedRepository(item);
        var repository = new CachedRepository<IdentifiableObject>(baseRepository);

        var calls = Enumerable.Range(0, 8).Select(_ => repository.GetAllAsync().AsTask()).ToList();
        baseRepository.Gate.SetResult();
        var results = await Task.WhenAll(calls).ConfigureAwait(false);

        Assert.That(baseRepository.LoadCount, Is.EqualTo(1));
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
        var baseRepository = new GatedRepository(item) { FailFirstLoad = true };
        var repository = new CachedRepository<IdentifiableObject>(baseRepository);

        var failingCall = repository.GetAllAsync().AsTask();
        var waitingCall = repository.GetByIdAsync(item.Id).AsTask();
        baseRepository.Gate.SetResult();

        Assert.ThrowsAsync<InvalidOperationException>(() => failingCall);
        Assert.That(await waitingCall.ConfigureAwait(false), Is.SameAs(item));
        Assert.That(baseRepository.LoadCount, Is.EqualTo(2));
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
    /// A repository which returns its item after the gate is opened.
    /// </summary>
    private sealed class GatedRepository : IRepository<IdentifiableObject>
    {
        private readonly IdentifiableObject _item;

        private int _loadCount;

        public GatedRepository(IdentifiableObject item)
        {
            this._item = item;
        }

        public TaskCompletionSource Gate { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public bool FailFirstLoad { get; init; }

        public int LoadCount => this._loadCount;

        public async ValueTask<IEnumerable<IdentifiableObject>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var loadCount = Interlocked.Increment(ref this._loadCount);
            await this.Gate.Task.ConfigureAwait(false);
            if (this.FailFirstLoad && loadCount == 1)
            {
                throw new InvalidOperationException("The first load failed.");
            }

            return [this._item];
        }

        public ValueTask<IdentifiableObject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public ValueTask<bool> DeleteAsync(object obj) => throw new NotSupportedException();

        public ValueTask<bool> DeleteAsync(Guid id) => throw new NotSupportedException();

        async ValueTask<IEnumerable> IRepository.GetAllAsync(CancellationToken cancellationToken) => await this.GetAllAsync(cancellationToken).ConfigureAwait(false);

        ValueTask<object?> IRepository.GetByIdAsync(Guid id, CancellationToken cancellationToken) => throw new NotSupportedException();
    }
}

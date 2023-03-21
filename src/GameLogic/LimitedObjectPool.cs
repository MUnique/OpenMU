// <copyright file="LimitedObjectPool.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Threading;
using Microsoft.Extensions.ObjectPool;

/// <summary>
/// An <see cref="IObjectPool{T}"/> which limits the amounts of created
/// <typeparamref name="T"/> to the maximum retained objects.
/// </summary>
/// <typeparam name="T">The type of objects to pool.</typeparam>
internal sealed class LimitedObjectPool<T> : DefaultObjectPool<T>, IObjectPool<T>
    where T : class
{
    private readonly SemaphoreSlim _semaphore;

    /// <summary>
    /// Initializes a new instance of the <see cref="LimitedObjectPool{T}"/> class.
    /// </summary>
    /// <param name="policy">The pooling policy to use.</param>
    /// <param name="maximumRetained">The maximum number of objects to create and retain in the pool.</param>
    public LimitedObjectPool(IPooledObjectPolicy<T> policy, int maximumRetained)
        : base(policy, maximumRetained)
    {
        this._semaphore = new SemaphoreSlim(maximumRetained, maximumRetained);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LimitedObjectPool{T}"/> class.
    /// </summary>
    /// <param name="policy">The pooling policy to use.</param>
    public LimitedObjectPool(IPooledObjectPolicy<T> policy)
        : base(policy, MaximumRetainedDefault)
    {
        this._semaphore = new SemaphoreSlim(MaximumRetainedDefault, MaximumRetainedDefault);
    }

    /// <summary>
    /// Gets the default number of maximum number of pooled objects.
    /// </summary>
    public static int MaximumRetainedDefault => Environment.ProcessorCount * 2;

    /// <inheritdoc />
    public async ValueTask<T> GetAsync(CancellationToken cancellationToken = default)
    {
        await this._semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        return base.Get();
    }

    /// <inheritdoc />
    public override T Get()
    {
        this._semaphore.Wait();
        return base.Get();
    }

    /// <inheritdoc />
    public override void Return(T obj)
    {
        base.Return(obj);
        this._semaphore.Release();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._semaphore.Dispose();
    }
}
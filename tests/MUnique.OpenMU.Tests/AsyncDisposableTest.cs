// <copyright file="AsyncDisposableTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;

/// <summary>
/// Tests for the <see cref="AsyncDisposable"/>.
/// </summary>
[TestFixture]
public class AsyncDisposableTest
{
    /// <summary>
    /// Tests that an instance is marked as disposed after <see cref="AsyncDisposable.DisposeAsync"/>.
    /// </summary>
    [Test]
    public async Task IsDisposedAfterDisposeAsync()
    {
        var disposable = new TestDisposable();

        await disposable.DisposeAsync().ConfigureAwait(false);

        Assert.That(disposable.IsDisposed, Is.True);
        Assert.That(disposable.IsDisposing, Is.False);
    }

    /// <summary>
    /// Tests that disposing an instance a second time doesn't release its resources again.
    /// </summary>
    [Test]
    public async Task SecondDisposeAsyncDoesNothingAsync()
    {
        var disposable = new TestDisposable();

        await disposable.DisposeAsync().ConfigureAwait(false);
        await disposable.DisposeAsync().ConfigureAwait(false);

        Assert.That(disposable.AsyncDisposeCount, Is.EqualTo(1));
        Assert.That(disposable.DisposeCount, Is.EqualTo(1));
    }

    private sealed class TestDisposable : AsyncDisposable
    {
        public int AsyncDisposeCount { get; private set; }

        public int DisposeCount { get; private set; }

        protected override ValueTask DisposeAsyncCore()
        {
            this.AsyncDisposeCount++;
            return base.DisposeAsyncCore();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DisposeCount++;
            }

            base.Dispose(disposing);
        }
    }
}

// <copyright file="EntityFrameworkRetryTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.EntityFrameworkCore;
using MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// Tests the retry policy of <see cref="EntityFrameworkContextBase"/>: only transient conflicts caused
/// by a concurrent entity mutation racing a save may be retried. A disposed context instance can never
/// succeed on retry (e.g. a save racing the disposal of its own context during a server shutdown), so
/// retrying it only delays the shutdown with misleading warnings.
/// </summary>
[TestFixture]
public class EntityFrameworkRetryTests
{
    /// <summary>
    /// A disposed context is not transient - retrying it could never succeed. This needs an explicit
    /// exclusion because <see cref="ObjectDisposedException"/> derives from
    /// <see cref="InvalidOperationException"/>, which otherwise IS retried.
    /// </summary>
    [Test]
    public void DisposedContextIsNotRetried()
    {
        Assert.That(EntityFrameworkContextBase.IsTransientConcurrencyConflict(new ObjectDisposedException("AccountContext")), Is.False);
    }

    /// <summary>
    /// The actual concurrency conflicts stay retryable.
    /// </summary>
    /// <param name="exception">The exception thrown by the save.</param>
    [TestCaseSource(nameof(TransientExceptions))]
    public void TransientConflictsAreRetried(Exception exception)
    {
        Assert.That(EntityFrameworkContextBase.IsTransientConcurrencyConflict(exception), Is.True);
    }

    private static IEnumerable<TestCaseData> TransientExceptions()
    {
        yield return new TestCaseData(new DbUpdateConcurrencyException()).SetName("DbUpdateConcurrencyException");
        yield return new TestCaseData(new InvalidOperationException("Collection was modified.")).SetName("InvalidOperationException");
        yield return new TestCaseData(new ArgumentNullException("key")).SetName("ArgumentNullException");
        yield return new TestCaseData(new NullReferenceException()).SetName("NullReferenceException");
        yield return new TestCaseData(new IndexOutOfRangeException()).SetName("IndexOutOfRangeException");
        yield return new TestCaseData(new KeyNotFoundException()).SetName("KeyNotFoundException");
    }
}

// <copyright file="InMemoryRepositoryProviderTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the <see cref="InMemoryRepositoryProvider"/>.
/// </summary>
[TestFixture]
public class InMemoryRepositoryProviderTest
{
    /// <summary>
    /// Tests that concurrent contexts which access a repository the first time get the same repository.
    /// The game servers of the demo mode did that at startup, which corrupted the dictionary of the
    /// repositories, or failed because a repository was registered twice.
    /// </summary>
    [Test]
    public async Task ConcurrentFirstAccessCreatesOneRepositoryAsync()
    {
        for (var i = 0; i < 20; i++)
        {
            var provider = new InMemoryRepositoryProvider();
            using var start = new ManualResetEventSlim();
            var tasks = Enumerable.Range(0, 8)
                .Select(_ => Task.Run(() =>
                {
                    start.Wait();
                    return provider.GetRepository(typeof(Guild));
                }))
                .ToList();
            start.Set();

            var repositories = await Task.WhenAll(tasks).ConfigureAwait(false);

            Assert.That(repositories.Distinct().Count(), Is.EqualTo(1));
        }
    }
}

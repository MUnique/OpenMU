// <copyright file="ContextStackTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the <see cref="ContextStack"/>.
/// </summary>
[TestFixture]
public class ContextStackTests
{
    /// <summary>
    /// Tests that a context which is used by a forked flow doesn't become the current context of the flow it was forked from.
    /// Previously, both flows shared the same stack instance.
    /// </summary>
    [Test]
    public async Task ForkedFlowDoesNotChangeTheCurrentContextAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        using var outerContext = contextProvider.CreateNewContext();
        using var forkedContext = contextProvider.CreateNewContext();
        var contextStack = new ContextStack();
        var forkedContextUsed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var parentChecked = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        using (contextStack.UseContext(outerContext))
        {
            var forkedFlow = Task.Run(async () =>
            {
                using (contextStack.UseContext(forkedContext))
                {
                    forkedContextUsed.SetResult();
                    await parentChecked.Task.ConfigureAwait(false);
                    Assert.That(contextStack.GetCurrentContext(), Is.SameAs(forkedContext));
                }

                Assert.That(contextStack.GetCurrentContext(), Is.SameAs(outerContext));
            });

            await forkedContextUsed.Task.ConfigureAwait(false);
            Assert.That(contextStack.GetCurrentContext(), Is.SameAs(outerContext));
            parentChecked.SetResult();
            await forkedFlow.ConfigureAwait(false);
            Assert.That(contextStack.GetCurrentContext(), Is.SameAs(outerContext));
        }

        Assert.That(contextStack.GetCurrentContext(), Is.Null);
    }

    /// <summary>
    /// Tests that nested usages are ended in the reverse order.
    /// </summary>
    [Test]
    public void NestedUsagesAreEndedInReverseOrder()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        using var outerContext = contextProvider.CreateNewContext();
        using var innerContext = contextProvider.CreateNewContext();
        var contextStack = new ContextStack();

        using (contextStack.UseContext(outerContext))
        {
            using (contextStack.UseContext(innerContext))
            {
                Assert.That(contextStack.GetCurrentContext(), Is.SameAs(innerContext));
            }

            Assert.That(contextStack.GetCurrentContext(), Is.SameAs(outerContext));
        }

        Assert.That(contextStack.GetCurrentContext(), Is.Null);
    }
}

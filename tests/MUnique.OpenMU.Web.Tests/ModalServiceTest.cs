// <copyright file="ModalServiceTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests;

using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Tests for <see cref="ModalService"/>.
/// </summary>
[TestFixture]
public class ModalServiceTest
{
    /// <summary>
    /// Tests that a modal which is shown from within another one doesn't close the other one,
    /// so that e.g. an object which is being created isn't discarded when a field of it opens a selection.
    /// </summary>
    [Test]
    public void ShowingAnotherModalKeepsThePreviousOneOpen()
    {
        using var service = new ModalService();
        var first = service.Show<ModalMessage>("first");
        var second = service.Show<ModalMessage>("second");

        Assert.That(first.Result.IsCompleted, Is.False);
        Assert.That(second.Result.IsCompleted, Is.False);
    }

    /// <summary>
    /// Tests that disposing the service cancels all open modals.
    /// </summary>
    [Test]
    public void DisposeCancelsAllModals()
    {
        var service = new ModalService();
        var first = service.Show<ModalMessage>("first");
        var second = service.Show<ModalMessage>("second");

        service.Dispose();

        Assert.That(first.Result.IsCompletedSuccessfully && first.Result.Result.Cancelled, Is.True);
        Assert.That(second.Result.IsCompletedSuccessfully && second.Result.Result.Cancelled, Is.True);
    }
}

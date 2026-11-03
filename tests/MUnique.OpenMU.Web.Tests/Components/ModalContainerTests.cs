// <copyright file="ModalContainerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Components;

using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Tests for the <see cref="ModalContainer"/> component.
/// </summary>
[TestFixture]
public class ModalContainerTests
{
    /// <summary>
    /// Showing and closing a modal doesn't render the content of the container again. In the admin panel, the content
    /// is the routed page, whose authorization would run again and could recreate the page (see MUnique/OpenMU#1338).
    /// </summary>
    [Test]
    public async Task ShowingAndClosingAModal_DoesNotRenderTheContentAgainAsync()
    {
        using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var modalService = new ModalService();
        context.Services.AddSingleton(modalService);
        var container = context.Render<ModalContainer>(parameters => parameters
            .Add(c => c.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<RenderCounter>(0);
                builder.AddComponentParameter(1, nameof(RenderCounter.ChildContent), (RenderFragment)(_ => { }));
                builder.CloseComponent();
            })));
        var counter = container.FindComponent<RenderCounter>().Instance;
        var rendersBefore = counter.Renders;

        await container.InvokeAsync(() => modalService.Show<ModalMessage>("Title", new ModalParameters { { nameof(ModalMessage.Text), "Text" } })).ConfigureAwait(false);
        container.WaitForState(() => container.FindAll(".modal").Count == 1);
        Assert.That(counter.Renders, Is.EqualTo(rendersBefore));

        await container.Find(".btn-close").ClickAsync(new()).ConfigureAwait(false);
        container.WaitForState(() => container.FindAll(".modal").Count == 0);
        Assert.That(counter.Renders, Is.EqualTo(rendersBefore));
    }

    /// <summary>
    /// Counts its renders. Like the <see cref="Microsoft.AspNetCore.Components.Authorization.AuthorizeRouteView"/>, it
    /// has a parameter of a render fragment, which counts as changed whenever its parent renders.
    /// </summary>
    private sealed class RenderCounter : ComponentBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        public int Renders { get; private set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            this.Renders++;
            builder.AddContent(0, this.Renders);
        }
    }
}

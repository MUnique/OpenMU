// <copyright file="EditorInvalidationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Pages;

using System.Threading;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;
using MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

/// <summary>Exercises a real generic editor which outlives a background configuration-cache invalidation.</summary>
[TestFixture]
public sealed class EditorInvalidationTests
{
    /// <summary>A clean open editor requests a reload instead of saving its disposed context; refreshing restores saving.</summary>
    [Test]
    public async Task CleanEditorSurvivesInvalidationAndRefreshAsync()
    {
        using var context = new BunitContext();
        var original = new GameConfiguration { ExperienceRate = 3 };
        var current = new GameConfiguration { ExperienceRate = 6 };
        var oldContext = CreateContext(original);
        var newContext = CreateContext(current);
        oldContext.Setup(c => c.Dispose()).Callback(() => oldContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new ObjectDisposedException("old context")));
        var provider = new Mock<IPersistenceContextProvider>();
        provider.SetupSequence(p => p.CreateNewContext()).Returns(oldContext.Object).Returns(newContext.Object);
        using IDataSource<GameConfiguration> source = new GameConfigurationDataSource(NullLogger<GameConfigurationDataSource>.Instance, provider.Object);
        var toast = Configure(context, source, provider.Object);
        context.JSInterop.Setup<bool>("window.confirm", _ => true).SetResult(true);
        var editor = context.Render<EditConfig>(p => p.Add(e => e.TypeString, typeof(GameConfiguration).FullName!));
        editor.WaitForAssertion(() => Assert.That(editor.Find("#ExperienceRate").GetAttribute("value"), Is.EqualTo("3")));
        await source.InvalidateAsync().ConfigureAwait(false);
        oldContext.Verify(c => c.Dispose(), Times.Once);
        await editor.Find("#ExperienceRate").ChangeAsync(new Microsoft.AspNetCore.Components.ChangeEventArgs { Value = "7" }).ConfigureAwait(false);
        await editor.Find("form").SubmitAsync().ConfigureAwait(false);
        var message = Resources.ResourceManager.GetString("EditorContextChanged", Resources.Culture)!;
        toast.Verify(t => t.ShowError(message, null), Times.Once);
        oldContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        newContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        newContext.SetupGet(c => c.HasChanges).Returns(true);
        await editor.Find("button.btn-secondary").ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()).ConfigureAwait(false);
        editor.WaitForAssertion(() => Assert.That(editor.Find("#ExperienceRate").GetAttribute("value"), Is.EqualTo("6")));
        newContext.Verify(c => c.Dispose(), Times.Never);
        await editor.Find("#ExperienceRate").ChangeAsync(new Microsoft.AspNetCore.Components.ChangeEventArgs { Value = "7" }).ConfigureAwait(false);
        await editor.Find("form").SubmitAsync().ConfigureAwait(false);
        newContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        toast.Verify(t => t.ShowSuccess(Resources.SavedChanges, null), Times.Once);
    }

    /// <summary>Closing a stale page never discards another admin's pending changes in the replacement context.</summary>
    [Test]
    public async Task ClosingStaleEditorPreservesReplacementChangesAsync()
    {
        using var context = new BunitContext();
        var original = new GameConfiguration { ExperienceRate = 3 };
        var current = new GameConfiguration { ExperienceRate = 6 };
        var oldContext = CreateContext(original);
        var newContext = CreateContext(current);
        var provider = new Mock<IPersistenceContextProvider>();
        provider.SetupSequence(p => p.CreateNewContext()).Returns(oldContext.Object).Returns(newContext.Object);
        using IDataSource<GameConfiguration> source = new GameConfigurationDataSource(NullLogger<GameConfigurationDataSource>.Instance, provider.Object);
        Configure(context, source, provider.Object);
        var editor = context.Render<EditConfig>(p => p.Add(e => e.TypeString, typeof(GameConfiguration).FullName!));
        editor.WaitForAssertion(() => Assert.That(editor.FindAll("form"), Has.Count.EqualTo(1)));
        await source.InvalidateAsync().ConfigureAwait(false);
        Assert.That(await source.GetOwnerAsync().ConfigureAwait(false), Is.SameAs(current));
        newContext.SetupGet(c => c.HasChanges).Returns(true);
        await editor.Instance.DisposeAsync().ConfigureAwait(false);
        newContext.Verify(c => c.Dispose(), Times.Never);
        Assert.That(await source.GetOwnerAsync().ConfigureAwait(false), Is.SameAs(current));
    }

    /// <summary>Leaving a stale editor prompts for unsaved data without touching replacement-context changes.</summary>
    [Test]
    public async Task NavigationFromStaleEditorPreservesReplacementChangesAsync()
    {
        using var context = new BunitContext();
        var oldContext = CreateContext(new GameConfiguration { ExperienceRate = 3 });
        var current = new GameConfiguration { ExperienceRate = 6 };
        var newContext = CreateContext(current);
        var provider = new Mock<IPersistenceContextProvider>();
        provider.SetupSequence(p => p.CreateNewContext()).Returns(oldContext.Object).Returns(newContext.Object);
        using IDataSource<GameConfiguration> source = new GameConfigurationDataSource(NullLogger<GameConfigurationDataSource>.Instance, provider.Object);
        Configure(context, source, provider.Object);
        // Use the shared page base without a query-bound form, so a fake navigation cannot reload the page.
        var editor = context.Render<NavigationEditor>(p => p.Add(e => e.TypeString, typeof(GameConfiguration).FullName!));
        editor.WaitForAssertion(() => Assert.That(editor.Markup, Is.EqualTo("ready")));
        await source.InvalidateAsync().ConfigureAwait(false);
        Assert.That(await source.GetOwnerAsync().ConfigureAwait(false), Is.SameAs(current));
        newContext.SetupGet(c => c.HasChanges).Returns(true);
        var confirmation = context.JSInterop.Setup<bool>("window.confirm", _ => true);
        confirmation.SetResult(true);
        var navigation = context.Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
        navigation.NavigateTo("/other");
        editor.WaitForAssertion(() => Assert.That(navigation.Uri, Does.EndWith("/other")));
        Assert.That(confirmation.Invocations, Has.Count.EqualTo(1));
        await editor.Instance.DisposeAsync().ConfigureAwait(false);
        newContext.Verify(c => c.Dispose(), Times.Never);
        Assert.That(await source.GetOwnerAsync().ConfigureAwait(false), Is.SameAs(current));
    }

    private static Mock<IContext> CreateContext(GameConfiguration configuration)
    {
        var context = new Mock<IContext>();
        context.Setup(c => c.GetAsync<GameConfiguration>(It.IsAny<CancellationToken>())).ReturnsAsync([configuration]);
        context.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
        return context;
    }

    private static Mock<IToastService> Configure(BunitContext context, IDataSource<GameConfiguration> source, IPersistenceContextProvider provider)
    {
        var toast = new Mock<IToastService>();
        context.Services.AddSingleton(source);
        context.Services.AddSingleton(provider);
        context.Services.AddSingleton(toast.Object);
        context.Services.AddSingleton<IModalService>(new TestModalService(true));
        context.Services.AddSingleton<LoadingOverlayService>();
        context.Services.AddSingleton<NavigationHistory>();
        context.Services.AddSingleton<IChangeNotificationService, ChangeNotificationService>();
        context.Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>()
            .NavigateTo("/edit-config/" + typeof(GameConfiguration).FullName + "?search=ExperienceRate");
        return toast;
    }

    /// <summary>Minimal page for exercising navigation independently of query-parameter updates in a routerless fixture.</summary>
    public sealed class NavigationEditor : EditBase
    {
        /// <inheritdoc />
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
        {
            if (this.Model is not null)
            {
                builder.AddContent(0, "ready");
            }
        }
    }
}

// <copyright file="ConfigurationReviewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Web.Shared.Components.Form;
using MUnique.OpenMU.Web.Shared.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Regression coverage for cache invalidation, independent previews and sensitive plugin changes.</summary>
[TestFixture]
public sealed class ConfigurationReviewTests
{
    /// <summary>Invalidation refreshes clean data, but retains dirty editors until they are saved.</summary>
    [Test]
    public async Task InvalidationPreservesPendingEditsAsync()
    {
        var provider = new Mock<IPersistenceContextProvider>();
        var first = new Mock<IContext>();
        var second = new Mock<IContext>();
        var old = new GameConfiguration { ExperienceRate = 3 };
        var current = new GameConfiguration { ExperienceRate = 6 };
        first.Setup(c => c.GetAsync<GameConfiguration>(It.IsAny<CancellationToken>())).ReturnsAsync([old]);
        second.Setup(c => c.GetAsync<GameConfiguration>(It.IsAny<CancellationToken>())).ReturnsAsync([current]);
        provider.SetupSequence(p => p.CreateNewContext()).Returns(first.Object).Returns(second.Object);
        using IDataSource<GameConfiguration> source = new GameConfigurationDataSource(NullLogger<GameConfigurationDataSource>.Instance, provider.Object);
        Assert.That(await source.GetOwnerAsync().ConfigureAwait(false), Is.SameAs(old));
        first.SetupGet(c => c.HasChanges).Returns(true);
        await source.InvalidateAsync().ConfigureAwait(false);
        Assert.That(await source.GetOwnerAsync().ConfigureAwait(false), Is.SameAs(old));
        first.Verify(c => c.Dispose(), Times.Never);
        first.SetupGet(c => c.HasChanges).Returns(false);
        Assert.That(await source.GetOwnerAsync().ConfigureAwait(false), Is.SameAs(current));
        first.Verify(c => c.Dispose(), Times.Once);
    }

    /// <summary>The graphical editor masks secret strings while preserving normal editing and saving.</summary>
    [Test]
    public void PanelMasksSecretStrings()
    {
        using var context = new BunitContext();
        context.Services.AddSingleton<IChangeNotificationService, ChangeNotificationService>();
        var settings = new ReviewSettings();
        var component = context.Render<CascadingValue<EditContext>>(parameters => parameters
            .Add(cascade => cascade.Value, new EditContext(settings))
            .AddChildContent<TextField>(fields => fields
                .Add(field => field.Value, settings.Access)
                .Add(field => field.ValueExpression, () => settings.Access)
                .Add(field => field.ValueChanged, value => settings.Access = value)));
        Assert.That(component.Find("input").GetAttribute("type"), Is.EqualTo("password"));
        component.Find("input").Change("updated-secret");
        Assert.That(settings.Access, Is.EqualTo("updated-secret"));
    }

    /// <summary>Secrets are omitted recursively by attribute and harmless numeric token settings remain auditable.</summary>
    [Test]
    public void AuditUsesSecretMetadata()
    {
        var json = AdminAuditSanitizer.Serialize(new { nested = new ReviewSettings() });
        Assert.That(json, Does.Not.Contain("marked-private-value").And.Not.Contain("7319").And.Contain("maxAttackTokens"));
        Assert.That(ConfigurationPatch.Fields(typeof(ReviewSettings), pluginSettings: true).Select(p => p.Name), Does.Contain("MaxAttackTokens").And.Not.Contain("Pin").And.Not.Contain("Access"));
    }

    /// <summary>Known rollback failures complete audit as Failed; unknown commit failures remain Pending.</summary>
    [TestCase(true)]
    [TestCase(false)]
    public async Task SaveFailureClassificationAsync(bool rejected)
    {
        var context = new Mock<IContext>();
        Exception failure = rejected ? new PersistenceConflictException(new ValidationException()) : new IOException("synthetic transport failure");
        context.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(failure);
        var audit = new InMemoryAdminAuditRepository();
        var executor = AdminOperationTestServices.CreateExecutor(audit: audit);
        var error = Assert.ThrowsAsync<AdminOperationException>(async () =>
            await executor.ExecuteAsync(AdminRoles.Administrator, "config_update", new { }, async _ =>
            {
                await AdminChangeExecution.SaveAsync(context.Object, default).ConfigureAwait(false);
                return true;
            }, default).ConfigureAwait(false));
        Assert.That(error!.Kind, Is.EqualTo(rejected ? AdminOperationErrorKind.Conflict : AdminOperationErrorKind.Uncertain));
        Assert.That((await audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single().Outcome, Is.EqualTo(rejected ? "Failed" : "Pending"));
    }

    /// <summary>Unmarked strings are writable without disclosing either value; marked settings are refused.</summary>
    [Test]
    public async Task PluginStringChangesNeverDiscloseValuesAsync()
    {
        var provider = new InMemoryPersistenceContextProvider();
        using var setup = provider.CreateNewContext();
        var config = setup.CreateNew<GameConfiguration>();
        var plugin = setup.CreateNew<PlugInConfiguration>();
        plugin.TypeId = typeof(ReviewPlugin).GUID;
        plugin.CustomConfiguration = "{\"Endpoint\":\"old-private-value\",\"Access\":\"marked-private-value\",\"MaxAttackTokens\":5}";
        config.PlugInConfigurations.Add(plugin);
        await setup.SaveChangesAsync().ConfigureAwait(false);
        var source = new Mock<IDataSource<GameConfiguration>>();
        source.Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(config);
        var manager = new PlugInManager([], NullLoggerFactory.Instance, null, null);
        manager.RegisterPlugIn<MUnique.OpenMU.GameLogic.PlugIns.IPeriodicTaskPlugIn, ReviewPlugin>();
        var audit = new InMemoryAdminAuditRepository();
        var confirmations = new AdminChangeConfirmation(TimeProvider.System);
        var writes = new PluginWriteOperations(provider, source.Object, manager, AdminOperationTestServices.CreateExecutor(audit: audit), confirmations, new AdminWriteSettings { ReadOnly = false });
        const string patch = "{\"Endpoint\":\"new-private-value\",\"MaxAttackTokens\":7}";
        using (await confirmations.Gate.LockAsync().ConfigureAwait(false))
        {
            // A preview must complete while an apply owns the host write gate.
            var previewTask = writes.UpdateConfigurationAsync(plugin.TypeId, patch, false, null, default).AsTask();
            var preview = await previewTask.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(preview.ToJsonString(), Does.Not.Contain("private-value"));
            Assert.That(preview["change"]!["revertAvailable"]!.GetValue<bool>(), Is.False);
        }

        var confirmed = await writes.UpdateConfigurationAsync(plugin.TypeId, patch, false, null, default).ConfigureAwait(false);
        var result = await writes.UpdateConfigurationAsync(plugin.TypeId, patch, true, confirmed["confirmationToken"]!.GetValue<string>(), default).ConfigureAwait(false);
        Assert.That(result.ToJsonString(), Does.Not.Contain("private-value"));
        var entry = (await audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single();
        Assert.That(entry.ArgumentsJson + entry.ResultJson, Does.Not.Contain("private-value"));
        Assert.That(plugin.CustomConfiguration, Does.Contain("new-private-value").And.Contain("marked-private-value"));
        Assert.ThrowsAsync<AdminOperationException>(async () => await writes.RevertFromAuditAsync(entry, false, null, default).ConfigureAwait(false));
        Assert.ThrowsAsync<AdminOperationException>(async () => await writes.UpdateConfigurationAsync(plugin.TypeId, "{\"Access\":\"replacement\"}", false, null, default).ConfigureAwait(false));
        source.Verify(s => s.InvalidateAsync(), Times.Once);
    }

    /// <summary>Represents an external plugin with both marked and legacy unmarked sensitive fields.</summary>
    [System.Runtime.InteropServices.Guid("a55e453b-96f6-4093-8f1c-7631647c9179")]
    public sealed class ReviewPlugin : MUnique.OpenMU.GameLogic.PlugIns.IPeriodicTaskPlugIn, ISupportCustomConfiguration<ReviewSettings>
    {
        /// <inheritdoc />
        public ReviewSettings? Configuration { get; set; }

        /// <inheritdoc />
        public ValueTask ExecuteTaskAsync(MUnique.OpenMU.GameLogic.GameContext gameContext) => ValueTask.CompletedTask;

        /// <inheritdoc />
        public void ForceStart()
        {
            // This test plugin has no periodic work to schedule.
        }
    }

    /// <summary>Sample configuration for disclosure regression tests.</summary>
    public sealed class ReviewSettings
    {
        /// <summary>Gets or sets an unmarked legacy string which must never be disclosed.</summary>
        public string Endpoint { get; set; } = "old-private-value";

        /// <summary>Gets or sets a marked string.</summary>
        [Secret]
        public string Access { get; set; } = "marked-private-value";

        /// <summary>Gets or sets a marked numeric credential.</summary>
        [Secret]
        public int Pin { get; set; } = 7319;

        /// <summary>Gets or sets a harmless token bucket limit.</summary>
        public double MaxAttackTokens { get; set; } = 5;
    }
}

// <copyright file="EffectTimeOperationsTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Threading;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>
/// Tests the <see cref="EffectTimeOperations"/>, which the panel and the agents share.
/// </summary>
[TestFixture]
public sealed class EffectTimeOperationsTests
{
    private InMemoryPersistenceContextProvider _persistence = null!;
    private GameConfiguration _config = null!;
    private InMemoryAdminAuditRepository _audit = null!;
    private MagicEffectDefinition _greaterDamage = null!;
    private MagicEffectDefinition _infiniteArrow = null!;
    private MagicEffectDefinition _soulBarrier = null!;

    /// <summary>
    /// Creates a configuration with three effects, one of which sends its time.
    /// </summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._persistence = new InMemoryPersistenceContextProvider();
        using var context = this._persistence.CreateNewContext();
        this._config = context.CreateNew<GameConfiguration>();
        this._greaterDamage = this.AddEffect(context, 1, "Greater Damage", false);
        this._infiniteArrow = this.AddEffect(context, 6, "Infinite Arrow", true);
        this._soulBarrier = this.AddEffect(context, 4, "Soul Barrier", false);
        await context.SaveChangesAsync().ConfigureAwait(false);
        this._audit = new InMemoryAdminAuditRepository();
    }

    /// <summary>
    /// A preview names the changing effects and changes nothing; the confirmed apply switches them and is audited.
    /// </summary>
    [Test]
    public async Task SetSendsTimePreviewsAndAppliesAsync()
    {
        var operations = this.CreateOperations();
        Guid[] send = [this._greaterDamage.GetId(), this._soulBarrier.GetId()];
        Guid[] hide = [this._infiniteArrow.GetId()];

        var preview = await operations.SetSendsTimeAsync(send, hide, false, null, default).ConfigureAwait(false);

        Assert.That(preview["affectedObjects"]!.GetValue<int>(), Is.EqualTo(3));
        Assert.That(preview["change"]!["send"]!.AsArray().Select(n => n!.GetValue<string>()), Is.EqualTo(new[] { "1 Greater Damage", "4 Soul Barrier" }));
        Assert.That(preview["change"]!["hide"]!.AsArray().Select(n => n!.GetValue<string>()), Is.EqualTo(new[] { "6 Infinite Arrow" }));
        Assert.That(this._greaterDamage.SendDuration, Is.False);
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Total, Is.Zero);

        await operations.SetSendsTimeAsync(send, hide, true, preview["confirmationToken"]!.GetValue<string>(), default).ConfigureAwait(false);

        Assert.That(this._greaterDamage.SendDuration, Is.True);
        Assert.That(this._soulBarrier.SendDuration, Is.True);
        Assert.That(this._infiniteArrow.SendDuration, Is.False);
        var entry = (await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single();
        Assert.That(entry.Operation, Is.EqualTo("effect_times_set"));
        Assert.That(entry.Outcome, Is.EqualTo("Succeeded"));
    }

    /// <summary>
    /// Effects which already have the requested state aren't part of the change.
    /// </summary>
    [Test]
    public async Task UnchangedEffectsAreNoChangeAsync()
    {
        var operations = this.CreateOperations();

        var preview = await operations.SetSendsTimeAsync([this._infiniteArrow.GetId()], [this._greaterDamage.GetId()], false, null, default).ConfigureAwait(false);

        Assert.That(preview["affectedObjects"]!.GetValue<int>(), Is.Zero);
    }

    /// <summary>
    /// A confirmation doesn't apply when the state changed since the preview.
    /// </summary>
    [Test]
    public async Task ChangedStateRefusesApplyAsync()
    {
        var operations = this.CreateOperations();
        var preview = await operations.SetSendsTimeAsync([this._greaterDamage.GetId()], null, false, null, default).ConfigureAwait(false);
        this._greaterDamage.SendDuration = true;

        Assert.ThrowsAsync<AdminOperationException>(async () => await operations.SetSendsTimeAsync([this._greaterDamage.GetId()], null, true, preview["confirmationToken"]!.GetValue<string>(), default).ConfigureAwait(false));
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Total, Is.Zero);
    }

    /// <summary>
    /// Invalid identifiers are rejected.
    /// </summary>
    [Test]
    public void RejectsInvalidInput()
    {
        var operations = this.CreateOperations();
        var effect = this._greaterDamage.GetId();

        Assert.That(Assert.ThrowsAsync<AdminOperationException>(async () => await operations.SetSendsTimeAsync([effect], [effect], false, null, default).ConfigureAwait(false))!.Kind, Is.EqualTo(AdminOperationErrorKind.InvalidInput));
        Assert.That(Assert.ThrowsAsync<AdminOperationException>(async () => await operations.SetSendsTimeAsync([], null, false, null, default).ConfigureAwait(false))!.Kind, Is.EqualTo(AdminOperationErrorKind.InvalidInput));
        Assert.That(Assert.ThrowsAsync<AdminOperationException>(async () => await operations.SetSendsTimeAsync([this._config.GetId()], null, false, null, default).ConfigureAwait(false))!.Kind, Is.EqualTo(AdminOperationErrorKind.InvalidInput));
    }

    /// <summary>
    /// A viewer may list the effects, but not change them.
    /// </summary>
    [Test]
    public async Task ViewerCanOnlyListAsync()
    {
        var operations = this.CreateOperations(AdminRoles.Viewer);

        var list = await operations.ListAsync(sendsTime: true, string.Empty, 0, 10, default).ConfigureAwait(false);

        Assert.That(list["total"]!.GetValue<int>(), Is.EqualTo(1));
        Assert.That(list["items"]!.AsArray().Single()!["name"]!.GetValue<string>(), Is.EqualTo("Infinite Arrow"));
        Assert.That(list["sendingCount"]!.GetValue<int>(), Is.EqualTo(1));
        Assert.That(Assert.ThrowsAsync<AdminOperationException>(async () => await operations.SetSendsTimeAsync([this._greaterDamage.GetId()], null, false, null, default).ConfigureAwait(false))!.Kind, Is.EqualTo(AdminOperationErrorKind.Forbidden));
    }

    private EffectTimeOperations CreateOperations(string? role = null)
    {
        var source = new Mock<IDataSource<GameConfiguration>>();
        source.Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(this._config);
        var executor = AdminOperationTestServices.CreateExecutor(audit: this._audit, actor: () => new AdminActor("test-id", "test", "panel", [role ?? AdminRoles.Administrator]));
        return new EffectTimeOperations(this._persistence, source.Object, executor, new AdminChangeConfirmation(TimeProvider.System), new AdminWriteSettings { ReadOnly = false });
    }

    private MagicEffectDefinition AddEffect(IContext context, short number, string name, bool sendDuration)
    {
        var effect = context.CreateNew<MagicEffectDefinition>();
        effect.Number = number;
        effect.Name = name;
        effect.SendDuration = sendDuration;
        this._config.MagicEffects.Add(effect);
        return effect;
    }
}

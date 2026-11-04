// <copyright file="ContentActivationOperationsTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Threading;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>
/// Tests the <see cref="ContentActivationOperations"/>, which the panel and the agents share.
/// </summary>
[TestFixture]
public sealed class ContentActivationOperationsTests
{
    private InMemoryPersistenceContextProvider _persistence = null!;
    private GameConfiguration _config = null!;
    private InMemoryAdminAuditRepository _audit = null!;
    private GameMapDefinition _lorencia = null!;
    private GameMapDefinition _karutan = null!;
    private ItemDefinition _unknownItem = null!;
    private MonsterDefinition _monster = null!;

    /// <summary>
    /// Creates a configuration with content of different versions.
    /// </summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._persistence = new InMemoryPersistenceContextProvider();
        using var context = this._persistence.CreateNewContext();
        this._config = context.CreateNew<GameConfiguration>();
        this._lorencia = this.AddMap(context, 0, "Lorencia", GameVersion.Version029);
        this._lorencia.IsActive = false;
        this._karutan = this.AddMap(context, 80, "Karutan 1", GameVersion.Season6Episode1);
        this._unknownItem = context.CreateNew<ItemDefinition>();
        this._unknownItem.Group = 14;
        this._unknownItem.Number = 200;
        this._unknownItem.Name = "Unknown item";
        this._config.Items.Add(this._unknownItem);
        this._monster = context.CreateNew<MonsterDefinition>();
        this._monster.Number = 459;
        this._monster.Designation = "Selupan";
        this._monster.IntroducedIn = GameVersion.Season4Episode1;
        this._config.Monsters.Add(this._monster);
        await context.SaveChangesAsync().ConfigureAwait(false);
        this._audit = new InMemoryAdminAuditRepository();
    }

    /// <summary>
    /// A preview changes nothing; the confirmed apply activates the content up to the version,
    /// deactivates the later content, keeps content of an unknown version and is audited.
    /// </summary>
    [Test]
    public async Task RestrictToGameVersionPreviewsAndAppliesAsync()
    {
        var operations = this.CreateOperations();

        var preview = await operations.RestrictToGameVersionAsync(GameVersion.Season2, false, null, default).ConfigureAwait(false);

        Assert.That(preview["affectedObjects"]!.GetValue<int>(), Is.EqualTo(3));
        Assert.That(preview["change"]!["activated"]!.GetValue<int>(), Is.EqualTo(1));
        Assert.That(preview["change"]!["deactivated"]!.GetValue<int>(), Is.EqualTo(2));
        Assert.That(preview["change"]!["types"]!["Maps"]!["deactivate"]!.AsArray().Select(k => k!.GetValue<string>()), Is.EquivalentTo(new[] { "80" }));
        Assert.That(this._lorencia.IsActive, Is.False);
        Assert.That(this._karutan.IsActive, Is.True);
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Total, Is.Zero);

        await operations.RestrictToGameVersionAsync(GameVersion.Season2, true, preview["confirmationToken"]!.GetValue<string>(), default).ConfigureAwait(false);

        Assert.That(this._lorencia.IsActive, Is.True);
        Assert.That(this._karutan.IsActive, Is.False);
        Assert.That(this._monster.IsActive, Is.False);
        Assert.That(this._unknownItem.IsActive, Is.True);
        var entry = (await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single();
        Assert.That(entry.Operation, Is.EqualTo("content_restrict_to_version"));
        Assert.That(entry.Outcome, Is.EqualTo("Succeeded"));
        Assert.That(entry.ArgumentsJson, Does.Contain("Season2"));
    }

    /// <summary>
    /// Explicit entries can be activated and deactivated, including content of an unknown version.
    /// </summary>
    [Test]
    public async Task SetActiveChangesExplicitEntriesAsync()
    {
        var operations = this.CreateOperations();
        Guid[] deactivate = [this._unknownItem.GetId(), this._karutan.GetId()];
        Guid[] activate = [this._lorencia.GetId()];

        var preview = await operations.SetActiveAsync(activate, deactivate, false, null, default).ConfigureAwait(false);
        await operations.SetActiveAsync(activate, deactivate, true, preview["confirmationToken"]!.GetValue<string>(), default).ConfigureAwait(false);

        Assert.That(this._lorencia.IsActive, Is.True);
        Assert.That(this._karutan.IsActive, Is.False);
        Assert.That(this._unknownItem.IsActive, Is.False);
        Assert.That(this._monster.IsActive, Is.True);
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single().Operation, Is.EqualTo("content_set_active"));
    }

    /// <summary>
    /// A confirmation doesn't apply when the state changed since the preview, or with other arguments.
    /// </summary>
    /// <param name="changeArguments">If set to <c>true</c>, the arguments are changed; otherwise the state.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async Task ChangedStateOrArgumentsRefuseApplyAsync(bool changeArguments)
    {
        var operations = this.CreateOperations();
        var preview = await operations.RestrictToGameVersionAsync(GameVersion.Season2, false, null, default).ConfigureAwait(false);
        var token = preview["confirmationToken"]!.GetValue<string>();
        if (!changeArguments)
        {
            this._monster.IsActive = false;
        }

        var target = changeArguments ? GameVersion.Season1 : GameVersion.Season2;
        Assert.ThrowsAsync<AdminOperationException>(async () => await operations.RestrictToGameVersionAsync(target, true, token, default).ConfigureAwait(false));

        Assert.That(this._karutan.IsActive, Is.True);
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Total, Is.Zero);
    }

    /// <summary>
    /// Invalid identifiers and versions are rejected.
    /// </summary>
    [Test]
    public void RejectsInvalidInput()
    {
        var operations = this.CreateOperations();
        var map = this._karutan.GetId();

        Assert.That(Assert.ThrowsAsync<AdminOperationException>(async () => await operations.SetActiveAsync([map], [map], false, null, default).ConfigureAwait(false))!.Kind, Is.EqualTo(AdminOperationErrorKind.InvalidInput));
        Assert.That(Assert.ThrowsAsync<AdminOperationException>(async () => await operations.SetActiveAsync([], null, false, null, default).ConfigureAwait(false))!.Kind, Is.EqualTo(AdminOperationErrorKind.InvalidInput));
        Assert.That(Assert.ThrowsAsync<AdminOperationException>(async () => await operations.SetActiveAsync([this._config.GetId()], null, false, null, default).ConfigureAwait(false))!.Kind, Is.EqualTo(AdminOperationErrorKind.InvalidInput));
        Assert.That(Assert.ThrowsAsync<AdminOperationException>(async () => await operations.RestrictToGameVersionAsync(GameVersion.Unknown, false, null, default).ConfigureAwait(false))!.Kind, Is.EqualTo(AdminOperationErrorKind.InvalidInput));
    }

    /// <summary>
    /// A viewer may list the content, but not change it.
    /// </summary>
    [Test]
    public async Task ViewerCanOnlyListAsync()
    {
        var operations = this.CreateOperations(AdminRoles.Viewer);

        var list = await operations.ListAsync(ContentActivationType.Maps, isActive: true, string.Empty, 0, 10, default).ConfigureAwait(false);

        Assert.That(list["total"]!.GetValue<int>(), Is.EqualTo(1));
        Assert.That(list["items"]!.AsArray().Single()!["name"]!.GetValue<string>(), Is.EqualTo("Karutan 1"));
        Assert.That(list["items"]!.AsArray().Single()!["introducedIn"]!.GetValue<string>(), Is.EqualTo(nameof(GameVersion.Season6Episode1)));
        Assert.That(Assert.ThrowsAsync<AdminOperationException>(async () => await operations.RestrictToGameVersionAsync(GameVersion.Season2, false, null, default).ConfigureAwait(false))!.Kind, Is.EqualTo(AdminOperationErrorKind.Forbidden));
    }

    /// <summary>
    /// A change of many entries omits their keys in the diff, so that it still fits into the audit.
    /// </summary>
    [Test]
    public async Task LargeChangeFitsIntoAuditAsync()
    {
        using (var context = this._persistence.CreateNewContext())
        {
            for (short number = 0; number < 2000; number++)
            {
                var item = context.CreateNew<ItemDefinition>();
                item.Group = 7;
                item.Number = number;
                item.Name = $"Item {number}";
                item.IntroducedIn = GameVersion.Season6Episode1;
                this._config.Items.Add(item);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        var operations = this.CreateOperations();
        var preview = await operations.RestrictToGameVersionAsync(GameVersion.Version029, false, null, default).ConfigureAwait(false);
        await operations.RestrictToGameVersionAsync(GameVersion.Version029, true, preview["confirmationToken"]!.GetValue<string>(), default).ConfigureAwait(false);

        Assert.That(preview["change"]!["keysOmitted"]!.GetValue<bool>(), Is.True);
        Assert.That(preview["change"]!["types"]!["Items"]!["deactivate"]!.GetValue<int>(), Is.EqualTo(2000));
        Assert.That(this._config.Items.Where(i => i.Group == 7), Has.All.Property(nameof(ItemDefinition.IsActive)).False);
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single().Outcome, Is.EqualTo("Succeeded"));
    }

    private ContentActivationOperations CreateOperations(string? role = null)
    {
        var source = new Mock<IDataSource<GameConfiguration>>();
        source.Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(this._config);
        var executor = AdminOperationTestServices.CreateExecutor(audit: this._audit, actor: () => new AdminActor("test-id", "test", "panel", [role ?? AdminRoles.Administrator]));
        return new ContentActivationOperations(this._persistence, source.Object, executor, new AdminChangeConfirmation(TimeProvider.System), new AdminWriteSettings { ReadOnly = false });
    }

    private GameMapDefinition AddMap(IContext context, short number, string name, GameVersion introducedIn)
    {
        var map = context.CreateNew<GameMapDefinition>();
        map.Number = number;
        map.Name = name;
        map.IntroducedIn = introducedIn;
        this._config.Maps.Add(map);
        return map;
    }
}

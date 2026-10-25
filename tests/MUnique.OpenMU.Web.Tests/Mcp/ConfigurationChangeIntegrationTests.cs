// <copyright file="ConfigurationChangeIntegrationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.Tests.AdminAuth;

/// <summary>Exercises preview, apply, audit, revert and restrictions through the official HTTP MCP client.</summary>
[TestFixture]
public sealed class ConfigurationChangeIntegrationTests
{
    private McpIntegrationTests _host = null!;
    private McpClient _client = null!;
    private GameConfiguration _config = null!;
    private IPersistenceContextProvider _persistence = null!;
    private InMemoryAdminAuditRepository _audit = null!;

    /// <summary>Starts the enabled host and an administrator client.</summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._host = new McpIntegrationTests { EnableWrites = true };
        await this._host.SetUpAsync().ConfigureAwait(false);
        this._client = await this._host.ConnectAsync("omu_mcp-admin-test").ConfigureAwait(false);
        this._config = await this._host.App.Services.GetRequiredService<IDataSource<GameConfiguration>>().GetOwnerAsync().ConfigureAwait(false);
        this._persistence = this._host.App.Services.GetRequiredService<IPersistenceContextProvider>();
        this._audit = (InMemoryAdminAuditRepository)this._host.App.Services.GetRequiredService<IAdminAuditRepository>();
    }

    /// <summary>Disposes the client and HTTP server.</summary>
    [TearDown]
    public async Task TearDownAsync()
    {
        await this._client.DisposeAsync().ConfigureAwait(false);
        await this._host.TearDownAsync().ConfigureAwait(false);
    }

    /// <summary>A preview has no effect or audit write; one confirmed apply stores a reversible diff.</summary>
    [Test]
    public async Task PreviewApplyAndRevertAsync()
    {
        var args = this.Patch("{\"ExperienceRate\":6}");
        var preview = await this.CallAsync("config_update", args).ConfigureAwait(false);
        Assert.That(this._config.ExperienceRate, Is.EqualTo(3));
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Total, Is.Zero);
        Confirm(args, preview);
        await this.CallAsync("config_update", args).ConfigureAwait(false);
        Assert.That(this._config.ExperienceRate, Is.EqualTo(6));
        var entry = (await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single();
        Assert.That(entry.Outcome, Is.EqualTo("Succeeded"));
        Assert.That(entry.ArgumentsJson, Does.Contain("ExperienceRate").And.Not.Contain("confirmationToken"));
        Assert.That((await this._client.CallToolAsync("config_update", args).ConfigureAwait(false)).IsError, Is.True);
        var revert = new Dictionary<string, object?> { ["auditId"] = entry.Id };
        Confirm(revert, await this.CallAsync("audit_revert", revert).ConfigureAwait(false));
        await this.CallAsync("audit_revert", revert).ConfigureAwait(false);
        Assert.That(this._config.ExperienceRate, Is.EqualTo(3));
    }

    /// <summary>A token cannot authorize different arguments or an intervening edit.</summary>
    [TestCase(true)]
    [TestCase(false)]
    public async Task ChangedArgumentsOrStateRefuseApplyAsync(bool changeArguments)
    {
        var args = this.Patch("{\"ExperienceRate\":6}");
        Confirm(args, await this.CallAsync("config_update", args).ConfigureAwait(false));
        if (changeArguments)
        {
            args["patch"] = "{\"ExperienceRate\":12}";
        }
        else
        {
            this._config.ExperienceRate = 4;
        }

        var result = await this._client.CallToolAsync("config_update", args).ConfigureAwait(false);
        Assert.That(result.IsError, Is.True);
        Assert.That(this._config.ExperienceRate, Is.EqualTo(changeArguments ? 3 : 4));
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Total, Is.Zero);
    }

    /// <summary>Tokens cannot cross key identities, even when both keys are administrators.</summary>
    [Test]
    public async Task CallerBindingAsync()
    {
        var keys = this._host.App.Services.GetRequiredService<IApiKeyRepository>();
        await keys.AddAsync(new ApiKey { Id = Guid.NewGuid(), Name = "other", Roles = AdminRoles.Administrator, KeyHash = ApiKeyGenerator.Hash("other-config-admin") }).ConfigureAwait(false);
        var args = this.Patch("{\"ExperienceRate\":6}");
        Confirm(args, await this.CallAsync("config_update", args).ConfigureAwait(false));
        await using var other = await this._host.ConnectAsync("other-config-admin").ConfigureAwait(false);
        Assert.That((await other.CallToolAsync("config_update", args).ConfigureAwait(false)).IsError, Is.True);
        Assert.That(this._config.ExperienceRate, Is.EqualTo(3));
    }

    /// <summary>Failing durable audit prevents configuration mutation.</summary>
    [Test]
    public async Task AuditFailurePreventsMutationAsync()
    {
        var args = this.Patch("{\"ExperienceRate\":6}");
        Confirm(args, await this.CallAsync("config_update", args).ConfigureAwait(false));
        this._audit.RejectWrites = true;
        Assert.That((await this._client.CallToolAsync("config_update", args).ConfigureAwait(false)).IsError, Is.True);
        Assert.That(this._config.ExperienceRate, Is.EqualTo(3));
    }

    /// <summary>Identity fields, unknown fields, collection replacement and invalid scalar conversions are rejected.</summary>
    [TestCase("{\"Id\":\"00000000-0000-0000-0000-000000000000\"}")]
    [TestCase("{\"PlugInConfigurations\":[]}")]
    [TestCase("{\"ExperienceRate\":\"bad\"}")]
    [TestCase("{\"MaximumLevel\":999999}")]
    [TestCase("{\"ExperienceRate\":null}")]
    [TestCase("{\"ExperienceRate\":-1}")]
    public async Task InvalidPatchNeverChangesStateAsync(string patch)
    {
        Assert.That((await this._client.CallToolAsync("config_update", this.Patch(patch)).ConfigureAwait(false)).IsError, Is.True);
        Assert.That(this._config.ExperienceRate, Is.EqualTo(3));
    }

    /// <summary>Reference-checked deletion and structural revert preserve the identifier and owning collection.</summary>
    [Test]
    public async Task CreatesDeletesAndRevertsLeafAsync()
    {
        var create = new Dictionary<string, object?> { ["parentType"] = "GameConfiguration", ["parentId"] = this._config.GetId(), ["collection"] = "DropItemGroups", ["values"] = "{\"Chance\":0.5,\"Description\":\"Event drops||de=Ereignis\"}" };
        var initialCount = this._config.DropItemGroups.Count;
        Confirm(create, await this.CallAsync("config_create", create).ConfigureAwait(false));
        var result = await this.CallAsync("config_create", create).ConfigureAwait(false);
        var id = Guid.Parse(result["change"]!["id"]!.GetValue<string>());
        Assert.That(this._config.DropItemGroups.Count, Is.EqualTo(initialCount + 1));
        var delete = new Dictionary<string, object?> { ["parentType"] = "GameConfiguration", ["parentId"] = this._config.GetId(), ["collection"] = "DropItemGroups", ["id"] = id };
        Confirm(delete, await this.CallAsync("config_delete", delete).ConfigureAwait(false));
        await this.CallAsync("config_delete", delete).ConfigureAwait(false);
        Assert.That(this._config.DropItemGroups.Count, Is.EqualTo(initialCount));
        var deletion = (await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.First(e => e.Operation == "config_delete");
        var revert = new Dictionary<string, object?> { ["auditId"] = deletion.Id };
        Confirm(revert, await this.CallAsync("audit_revert", revert).ConfigureAwait(false));
        await this.CallAsync("audit_revert", revert).ConfigureAwait(false);
        var restored = this._config.DropItemGroups.Single(g => g.GetId() == id);
        Assert.That(restored.Chance, Is.EqualTo(0.5));
        Assert.That(restored.Description.Value, Is.EqualTo("Event drops||de=Ereignis"));
    }

    /// <summary>Revert refuses an object edited again after the original change.</summary>
    [Test]
    public async Task RevertRefusesInterveningEditAsync()
    {
        var args = this.Patch("{\"ExperienceRate\":6}");
        Confirm(args, await this.CallAsync("config_update", args).ConfigureAwait(false));
        await this.CallAsync("config_update", args).ConfigureAwait(false);
        var entry = (await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single();
        this._config.MaximumLevel = 300;
        Assert.That((await this._client.CallToolAsync("audit_revert", new Dictionary<string, object?> { ["auditId"] = entry.Id }).ConfigureAwait(false)).IsError, Is.True);
        Assert.That(this._config.ExperienceRate, Is.EqualTo(6));
    }

    /// <summary>Group restrictions hide disallowed reads and writes, and reject direct tool calls.</summary>
    [Test]
    public async Task ToolGroupsFilterDiscoveryAndCallsAsync()
    {
        var keys = (InMemoryApiKeyRepository)this._host.App.Services.GetRequiredService<IApiKeyRepository>();
        var key = new ApiKey { Id = Guid.NewGuid(), Name = "rates", Roles = AdminRoles.Administrator, KeyHash = ApiKeyGenerator.Hash("rates-only"), AllowedToolGroups = "rates_*" };
        await keys.AddAsync(key).ConfigureAwait(false);
        await using var client = await this._host.ConnectAsync("rates-only").ConfigureAwait(false);
        Assert.That((await client.ListToolsAsync().ConfigureAwait(false)).Select(t => t.Name), Is.EquivalentTo(new[] { "rates_get", "rates_set" }));
        Assert.That((await client.CallToolAsync("config_update", this.Patch("{\"ExperienceRate\":5}")).ConfigureAwait(false)).IsError, Is.True);
        var args = new Dictionary<string, object?> { ["rate"] = "experience", ["value"] = 5 };
        var preview = await client.CallToolAsync("rates_set", args).ConfigureAwait(false);
        Assert.That(preview.IsError, Is.Not.True);
        Confirm(args, JsonSerializer.SerializeToNode(preview.StructuredContent)!.AsObject());
        Assert.That((await client.CallToolAsync("rates_set", args).ConfigureAwait(false)).IsError, Is.Not.True);
        Assert.That(this._config.ExperienceRate, Is.EqualTo(5));
        key.AllowedToolGroups = "invalid*";
        await keys.UpdateAsync(key).ConfigureAwait(false);
        Assert.That(await client.ListToolsAsync().ConfigureAwait(false), Is.Empty);
    }

    /// <summary>Installed plugin activation and safe scalar configuration edits are previewed and audited.</summary>
    [Test]
    public async Task PluginChangesPreserveSecretFieldsAsync()
    {
        var manager = this._host.App.Services.GetRequiredService<PlugInManager>();
        manager.RegisterPlugInAtPlugInPoint<IPeriodicTaskPlugIn>(new PeriodicSaveProgressPlugIn());
        using var context = this._persistence.CreateNewContext();
        var plugin = context.CreateNew<PlugInConfiguration>();
        plugin.TypeId = typeof(PeriodicSaveProgressPlugIn).GUID;
        plugin.CustomConfiguration = "{\"Interval\":\"00:01:00\",\"Secret\":\"never-return-this\"}";
        this._config.PlugInConfigurations.Add(plugin);
        var active = new Dictionary<string, object?> { ["pluginId"] = plugin.TypeId, ["active"] = true };
        Confirm(active, await this.CallAsync("plugins_set_active", active).ConfigureAwait(false));
        await this.CallAsync("plugins_set_active", active).ConfigureAwait(false);
        Assert.That(plugin.IsActive, Is.True);
        var update = new Dictionary<string, object?> { ["pluginId"] = plugin.TypeId, ["patch"] = "{\"Interval\":\"00:02:00\"}" };
        Confirm(update, await this.CallAsync("plugins_update_configuration", update).ConfigureAwait(false));
        await this.CallAsync("plugins_update_configuration", update).ConfigureAwait(false);
        Assert.That(plugin.CustomConfiguration, Does.Contain("00:02:00").And.Contain("never-return-this"));
        var records = JsonSerializer.Serialize((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items);
        Assert.That(records, Does.Not.Contain("never-return-this"));
        var entry = (await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.First(e => e.Operation == "plugins_update_configuration");
        var revert = new Dictionary<string, object?> { ["auditId"] = entry.Id };
        Confirm(revert, await this.CallAsync("audit_revert", revert).ConfigureAwait(false));
        await this.CallAsync("audit_revert", revert).ConfigureAwait(false);
        Assert.That(plugin.CustomConfiguration, Does.Contain("00:01:00").And.Contain("never-return-this"));
    }

    /// <summary>Offline writes reserve through save and always release; an online account is refused.</summary>
    [Test]
    public async Task OfflineCharacterReservationAndItemPlacementAsync()
    {
        var login = Mock.Get(this._host.App.Services.GetRequiredService<ILoginServer>());
        login.Setup(l => l.TryReserveAccountAsync("tester", It.IsAny<int>())).ReturnsAsync(true);
        using var context = this._persistence.CreateNewPlayerContext(this._config);
        var account = await context.GetAccountByLoginNameAsync("tester").ConfigureAwait(false);
        var character = account!.Characters.Single();
        this._config.MaximumInventoryMoney = 100000;
        character.Inventory = context.CreateNew<ItemStorage>();
        var strength = context.CreateNew<AttributeDefinition>(Stats.BaseStrength.Id, "Strength", string.Empty);
        strength.MaximumValue = 100;
        character.Attributes.Add(context.CreateNew<StatAttribute>(strength, 10f));
        var invalidStats = new Dictionary<string, object?> { ["characterName"] = "Hero", ["patch"] = "{\"strength\":101}" };
        Assert.That((await this._client.CallToolAsync("characters_update", invalidStats).ConfigureAwait(false)).IsError, Is.True);
        Assert.That(character.Attributes.Single().Value, Is.EqualTo(10));
        var item = this._config.Items.First();
        item.Width = 1;
        item.Height = 1;
        item.MaximumItemLevel = 15;
        item.Durability = 20;
        var args = new Dictionary<string, object?> { ["characterName"] = "Hero", ["patch"] = "{\"money\":100}" };
        Confirm(args, await this.CallAsync("characters_update", args).ConfigureAwait(false));
        Assert.That(character.Inventory.Money, Is.Zero);
        await this.CallAsync("characters_update", args).ConfigureAwait(false);
        Assert.That(character.Inventory.Money, Is.EqualTo(100));
        login.Verify(l => l.ReleaseAccountReservationAsync("tester", It.IsAny<int>()), Times.Exactly(3));
        var add = new Dictionary<string, object?> { ["characterName"] = "Hero", ["itemId"] = item.GetId(), ["level"] = 1 };
        Confirm(add, await this.CallAsync("characters_add_item", add).ConfigureAwait(false));
        Assert.That(character.Inventory.Items, Is.Empty);
        await this.CallAsync("characters_add_item", add).ConfigureAwait(false);
        Assert.That(character.Inventory.Items.Single().ItemSlot, Is.GreaterThanOrEqualTo(12));
        login.Setup(l => l.TryReserveAccountAsync("tester", It.IsAny<int>())).ReturnsAsync(false);
        Assert.That((await this._client.CallToolAsync("characters_update", new Dictionary<string, object?> { ["characterName"] = "Hero", ["patch"] = "{\"money\":200}" }).ConfigureAwait(false)).IsError, Is.True);
        Assert.That(character.Inventory.Money, Is.EqualTo(100));
    }

    /// <summary>Drop edits can add one item and deletion refuses another object's reference.</summary>
    [Test]
    public async Task DropReferencesAreValidatedAsync()
    {
        using var context = this._persistence.CreateNewContext();
        var group = context.CreateNew<DropItemGroup>();
        group.Chance = 0.1;
        this._config.DropItemGroups.Add(group);
        var item = this._config.Items.First();
        var args = new Dictionary<string, object?> { ["groupId"] = group.GetId(), ["chance"] = 0.2, ["addItemId"] = item.GetId() };
        Confirm(args, await this.CallAsync("drops_update", args).ConfigureAwait(false));
        await this.CallAsync("drops_update", args).ConfigureAwait(false);
        Assert.That(group.Chance, Is.EqualTo(0.2));
        Assert.That(group.PossibleItems.Single().GetId(), Is.EqualTo(item.GetId()));
        this._config.Maps.First().DropItemGroups.Add(group);
        var delete = new Dictionary<string, object?> { ["parentType"] = "GameConfiguration", ["parentId"] = this._config.GetId(), ["collection"] = "DropItemGroups", ["id"] = group.GetId() };
        Assert.That((await this._client.CallToolAsync("config_delete", delete).ConfigureAwait(false)).IsError, Is.True);
        Assert.That(this._config.DropItemGroups, Does.Contain(group));
    }

    /// <summary>Money rates use the configured immutable attribute's assignment path and are reversible.</summary>
    [Test]
    public async Task MoneyRateCanBeChangedAndRevertedAsync()
    {
        using var context = this._persistence.CreateNewContext();
        var definition = context.CreateNew<AttributeDefinition>(Stats.MoneyAmountRate.Id, "Money amount", string.Empty);
        this._config.Attributes.Add(definition);
        var attribute = context.CreateNew<ConstValueAttribute>(2f, definition, AggregateType.AddRaw);
        this._config.GlobalBaseAttributeValues.Add(attribute);
        var args = new Dictionary<string, object?> { ["rate"] = "money", ["value"] = 4 };
        Confirm(args, await this.CallAsync("rates_set", args).ConfigureAwait(false));
        await this.CallAsync("rates_set", args).ConfigureAwait(false);
        Assert.That(attribute.Value, Is.EqualTo(4));
        var entry = (await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single();
        var revert = new Dictionary<string, object?> { ["auditId"] = entry.Id };
        Confirm(revert, await this.CallAsync("audit_revert", revert).ConfigureAwait(false));
        await this.CallAsync("audit_revert", revert).ConfigureAwait(false);
        Assert.That(attribute.Value, Is.EqualTo(2));
    }

    /// <summary>Spawn model validation rejects inverted rectangles before mutation.</summary>
    [TestCase("{\"X1\":20,\"X2\":10}")]
    [TestCase("{\"Quantity\":-1}")]
    public async Task SpawnBoundsAreValidatedAsync(string invalidPatch)
    {
        using var context = this._persistence.CreateNewContext();
        var spawn = context.CreateNew<MonsterSpawnArea>();
        spawn.GameMap = this._config.Maps.First();
        spawn.MonsterDefinition = this._config.Monsters.First();
        spawn.GameMap.MonsterSpawns.Add(spawn);
        var args = new Dictionary<string, object?> { ["spawnId"] = spawn.GetId(), ["patch"] = invalidPatch };
        Assert.That((await this._client.CallToolAsync("spawns_update", args).ConfigureAwait(false)).IsError, Is.True);
        Assert.That(spawn.X1, Is.Zero);
        args["patch"] = "{\"X2\":10,\"Quantity\":5}";
        Confirm(args, await this.CallAsync("spawns_update", args).ConfigureAwait(false));
        await this.CallAsync("spawns_update", args).ConfigureAwait(false);
        Assert.That(spawn.Quantity, Is.EqualTo(5));
    }

    /// <summary>Runtime read-only mode blocks even previews before audit or mutation.</summary>
    [Test]
    public async Task ReadOnlyRejectsConfigurationPreviewAsync()
    {
        this._host.App.Services.GetRequiredService<MUnique.OpenMU.Web.AdminPanel.Mcp.AdminMcpOptions>().ReadOnly = true;
        var result = await this._client.CallToolAsync("config_update", this.Patch("{\"ExperienceRate\":5}")).ConfigureAwait(false);
        Assert.That(result.IsError, Is.True);
        Assert.That(JsonSerializer.Serialize(result), Does.Contain("writes are disabled"));
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Total, Is.Zero);
    }

    /// <summary>Opaque terrain changes also invalidate a configuration preview without exposing the terrain.</summary>
    [Test]
    public async Task BinaryStateChangesInvalidatePreviewAsync()
    {
        var map = this._config.Maps.First();
        map.SafezoneMap = map;
        map.TerrainData = [1, 2, 3];
        var args = new Dictionary<string, object?> { ["type"] = "GameMapDefinition", ["id"] = map.GetId(), ["patch"] = "{\"Name\":\"New name\"}" };
        Confirm(args, await this.CallAsync("config_update", args).ConfigureAwait(false));
        map.TerrainData = [1, 2, 4];
        Assert.That((await this._client.CallToolAsync("config_update", args).ConfigureAwait(false)).IsError, Is.True);
        Assert.That(map.Name.Value, Is.Not.EqualTo("New name"));
    }

    /// <summary>Content activation uses the shared preview, confirmation and audit, and agents read the result back.</summary>
    [Test]
    public async Task ContentActivationPreviewAndApplyAsync()
    {
        var map = this._config.Maps.Single();
        map.IntroducedIn = GameVersion.Season6Episode1;
        var args = new Dictionary<string, object?> { ["target"] = nameof(GameVersion.Season2) };
        var preview = await this.CallAsync("content_restrict_to_version", args).ConfigureAwait(false);
        Assert.That(map.IsActive, Is.True);
        Assert.That(preview["affectedObjects"]!.GetValue<int>(), Is.EqualTo(1));
        Confirm(args, preview);
        await this.CallAsync("content_restrict_to_version", args).ConfigureAwait(false);
        Assert.That(map.IsActive, Is.False);

        var inactive = await this.CallAsync("content_list", new() { ["type"] = "Maps", ["isActive"] = false }).ConfigureAwait(false);
        Assert.That(inactive["items"]!.AsArray().Single()!["id"]!.GetValue<Guid>(), Is.EqualTo(map.GetId()));

        var activate = new Dictionary<string, object?> { ["activateIds"] = new[] { map.GetId() } };
        Confirm(activate, await this.CallAsync("content_set_active", activate).ConfigureAwait(false));
        await this.CallAsync("content_set_active", activate).ConfigureAwait(false);
        Assert.That(map.IsActive, Is.True);
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Select(e => e.Operation), Is.EquivalentTo(new[] { "content_restrict_to_version", "content_set_active" }));
    }

    private static void Confirm(Dictionary<string, object?> arguments, JsonObject preview)
    {
        arguments["apply"] = true;
        arguments["confirmationToken"] = preview["confirmationToken"]!.GetValue<string>();
    }

    private Dictionary<string, object?> Patch(string jsonPatch) => new() { ["type"] = "GameConfiguration", ["id"] = this._config.GetId(), ["patch"] = jsonPatch };

    private async ValueTask<JsonObject> CallAsync(string tool, Dictionary<string, object?> arguments)
    {
        var result = await this._client.CallToolAsync(tool, arguments).ConfigureAwait(false);
        Assert.That(result.IsError, Is.Not.True, JsonSerializer.Serialize(result));
        return JsonSerializer.SerializeToNode(result.StructuredContent)!.AsObject();
    }
}

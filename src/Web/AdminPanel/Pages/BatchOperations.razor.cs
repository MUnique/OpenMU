// <copyright file="BatchOperations.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// MU Nueva Era batch configuration cockpit.
/// Provides preview-first bulk operations over the native OpenMU configuration graph.
/// </summary>
public partial class BatchOperations : ComponentBase, IAsyncDisposable
{
    private readonly HashSet<Guid> _selectedMapIds = [];
    private readonly HashSet<Guid> _targetMerchantIds = [];

    private GameConfiguration? _gameConfiguration;
    private IContext? _gameContext;
    private IContext? _serverContext;

    private List<GameServerDefinition> _servers = [];
    private List<ServerRateEdit> _serverEdits = [];
    private List<GameMapDefinition> _maps = [];
    private List<MonsterDefinition> _monsters = [];
    private List<ItemDefinition> _items = [];
    private List<DropItemGroup> _dropGroups = [];
    private List<ItemDropItemGroup> _dropContentGroups = [];
    private List<ItemSetGroup> _itemSets = [];
    private List<DropChanceEdit> _dropChanceEdits = [];
    private List<MonsterDefinition> _merchants = [];

    private EconomyEdit _economy = new();

    private string _dropScope = "maps";
    private string _dropAction = "add";
    private Guid? _selectedDropGroupId;
    private string _monsterFilter = string.Empty;
    private int? _monsterMinLevel;
    private int? _monsterMaxLevel;
    private bool _monsterOnlySelectedMaps;

    private int? _batchMaximumItemDrops;
    private double? _batchRespawnSeconds;

    private string _itemFilter = string.Empty;
    private int? _itemGroup;
    private int? _itemCurrentMinDropLevel;
    private int? _itemCurrentMaxDropLevel;
    private int? _newItemDropLevel;
    private bool _setItemMaximumDropLevel;
    private int? _newItemMaximumDropLevel;
    private bool _setDropsFromMonsters;
    private bool _newDropsFromMonsters = true;

    private Guid? _contentDropGroupId;
    private string _contentAction = "add";
    private string _contentFilter = string.Empty;
    private int? _contentGroup;
    private Guid? _contentSetId;
    private DropGroupItemCategory _contentCategory = DropGroupItemCategory.Any;
    private bool _contentConfirmed;

    private Guid? _sourceMerchantId;

    private bool _restartAllAfterEconomyApply = true;
    private bool _isLoading = true;
    private bool _isApplying;

    private string _previewTitle = "Sin vista previa";
    private string _previewSummary = "Configura una operación y pulsa Vista previa.";
    private List<string> _previewLines = [];
    private Func<Task>? _undoAction;

    /// <summary>
    /// Gets or sets the game configuration data source.
    /// </summary>
    [Inject]
    public IDataSource<GameConfiguration> DataSource { get; set; } = null!;

    /// <summary>
    /// Gets or sets the persistence context provider.
    /// </summary>
    [Inject]
    public IPersistenceContextProvider ContextProvider { get; set; } = null!;

    /// <summary>
    /// Gets or sets the game server instance manager.
    /// </summary>
    [Inject]
    public IGameServerInstanceManager ServerInstanceManager { get; set; } = null!;

    /// <summary>
    /// Gets or sets the toast service.
    /// </summary>
    [Inject]
    public IToastService ToastService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the loading overlay service.
    /// </summary>
    [Inject]
    public LoadingOverlayService LoadingService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [Inject]
    public ILogger<BatchOperations> Logger { get; set; } = null!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        using var loading = this.LoadingService.ShowLoadingIndicator();
        try
        {
            this._gameContext = await this.DataSource.GetContextAsync().ConfigureAwait(true);
            this._gameConfiguration = await this.DataSource.GetOwnerAsync().ConfigureAwait(true);

            this._maps = this.DataSource.GetAll<GameMapDefinition>()
                .OrderBy(m => m.Number)
                .ThenBy(m => m.Name.ToString())
                .ToList();

            this._monsters = this.DataSource.GetAll<MonsterDefinition>()
                .Where(m => m.ObjectKind == NpcObjectKind.Monster)
                .OrderBy(GetMonsterLevel)
                .ThenBy(m => m.Designation.ToString())
                .ToList();

            this._items = this.DataSource.GetAll<ItemDefinition>()
                .OrderBy(i => i.Group)
                .ThenBy(i => i.Number)
                .ToList();

            this._dropGroups = this.DataSource.GetAll<DropItemGroup>()
                .OrderBy(g => g.ItemType)
                .ThenBy(g => g.Description.ToString())
                .ToList();

            this._dropContentGroups = this.DataSource.GetAll<ItemDropItemGroup>()
                .OrderBy(g => g.Description.ToString())
                .ToList();

            this._itemSets = this.DataSource.GetAll<ItemSetGroup>()
                .OrderBy(set => set.Name.ToString())
                .ToList();

            this._merchants = this.DataSource.GetAll<MonsterDefinition>()
                .Where(m => m is { ObjectKind: NpcObjectKind.PassiveNpc, MerchantStore: not null })
                .OrderBy(m => m.Designation.ToString())
                .ToList();

            this._economy = new EconomyEdit
            {
                ExperienceRate = this._gameConfiguration.ExperienceRate,
                MasterExperienceRate = this._gameConfiguration.MasterExperienceRate,
                MaximumInventoryMoney = this._gameConfiguration.MaximumInventoryMoney,
                MaximumVaultMoney = this._gameConfiguration.MaximumVaultMoney,
                ShouldDropMoney = this._gameConfiguration.ShouldDropMoney,
                ItemDropDurationSeconds = this._gameConfiguration.ItemDropDuration.TotalSeconds,
            };

            this._dropChanceEdits = this._dropGroups
                .Where(IsCommonEconomyDropGroup)
                .Select(group => new DropChanceEdit(group))
                .ToList();

            this._serverContext = this.ContextProvider.CreateNewTypedContext(
                typeof(GameServerDefinition),
                true,
                this._gameConfiguration);

            this._servers = (await this._serverContext.GetAsync<GameServerDefinition>().ConfigureAwait(true))
                .OrderBy(s => s.ServerID)
                .ToList();

            this._serverEdits = this._servers.Select(server => new ServerRateEdit(server)).ToList();
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Failed to load batch cockpit.");
            this.ToastService.ShowError($"No se pudo cargar Operaciones por lote: {ex.Message}");
        }
        finally
        {
            this._isLoading = false;
        }
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        this._serverContext?.Dispose();
        this._serverContext = null;
        return ValueTask.CompletedTask;
    }

    private void SelectAllMaps(bool selected)
    {
        this._selectedMapIds.Clear();
        if (selected)
        {
            foreach (var map in this._maps)
            {
                this._selectedMapIds.Add(map.GetId());
            }
        }
    }

    private void ToggleMap(Guid id, bool selected)
    {
        if (selected)
        {
            this._selectedMapIds.Add(id);
        }
        else
        {
            this._selectedMapIds.Remove(id);
        }
    }

    private void ToggleMerchant(Guid id, bool selected)
    {
        if (selected)
        {
            this._targetMerchantIds.Add(id);
        }
        else
        {
            this._targetMerchantIds.Remove(id);
        }
    }

    private void PreviewEconomy()
    {
        if (this._gameConfiguration is null)
        {
            return;
        }

        var lines = new List<string>();
        AddDiff(lines, "EXP global", this._gameConfiguration.ExperienceRate, this._economy.ExperienceRate);
        AddDiff(lines, "Master EXP global", this._gameConfiguration.MasterExperienceRate, this._economy.MasterExperienceRate);
        AddDiff(lines, "Zen máximo inventario", this._gameConfiguration.MaximumInventoryMoney, this._economy.MaximumInventoryMoney);
        AddDiff(lines, "Zen máximo baúl", this._gameConfiguration.MaximumVaultMoney, this._economy.MaximumVaultMoney);
        AddDiff(lines, "Zen cae al suelo", this._gameConfiguration.ShouldDropMoney, this._economy.ShouldDropMoney);
        AddDiff(lines, "Duración drops (s)", this._gameConfiguration.ItemDropDuration.TotalSeconds, this._economy.ItemDropDurationSeconds);

        foreach (var edit in this._serverEdits)
        {
            var server = this._servers.First(s => s.GetId() == edit.Id);
            AddDiff(lines, $"Canal #{server.ServerID} {server.Description} · EXP", server.ExperienceRate, edit.ExperienceRate);
            AddDiff(lines, $"Canal #{server.ServerID} {server.Description} · Zen", server.MoneyRate, edit.MoneyRate);
            AddDiff(lines, $"Canal #{server.ServerID} {server.Description} · PvP", server.PvpEnabled, edit.PvpEnabled);
        }

        foreach (var edit in this._dropChanceEdits)
        {
            var group = this._dropGroups.First(g => g.GetId() == edit.Id);
            AddDiff(lines, $"Drop {group.Description}", group.Chance * 100.0, edit.Percent, "%");
        }

        this.SetPreview(
            "Economía y canales",
            lines,
            lines.Count == 0 ? "No hay cambios pendientes." : $"{lines.Count} cambios listos para aplicar.");
    }

    private async Task ApplyEconomyAsync()
    {
        if (this._gameConfiguration is null || this._gameContext is null || this._serverContext is null)
        {
            return;
        }

        this.PreviewEconomy();
        if (this._previewLines.Count == 0)
        {
            this.ToastService.ShowSuccess("No hay cambios de economía para guardar.");
            return;
        }

        if (this._economy.ExperienceRate < 0
            || this._economy.MasterExperienceRate < 0
            || this._economy.ItemDropDurationSeconds < 0
            || this._serverEdits.Any(e => e.ExperienceRate < 0 || e.MoneyRate < 0)
            || this._dropChanceEdits.Any(e => e.Percent is < 0 or > 100))
        {
            this.ToastService.ShowError("Hay valores fuera de rango. EXP/Zen no pueden ser negativos y los drops deben estar entre 0% y 100%.");
            return;
        }

        var gameSnapshot = new EconomyEdit
        {
            ExperienceRate = this._gameConfiguration.ExperienceRate,
            MasterExperienceRate = this._gameConfiguration.MasterExperienceRate,
            MaximumInventoryMoney = this._gameConfiguration.MaximumInventoryMoney,
            MaximumVaultMoney = this._gameConfiguration.MaximumVaultMoney,
            ShouldDropMoney = this._gameConfiguration.ShouldDropMoney,
            ItemDropDurationSeconds = this._gameConfiguration.ItemDropDuration.TotalSeconds,
        };

        var serverSnapshot = this._servers.ToDictionary(
            s => s.GetId(),
            s => new ServerSnapshot(s.ExperienceRate, s.MoneyRate, s.PvpEnabled));

        var dropSnapshot = this._dropChanceEdits.ToDictionary(
            e => e.Id,
            e => this._dropGroups.First(g => g.GetId() == e.Id).Chance);

        await this.RunApplyAsync(async () =>
        {
            this._gameConfiguration.ExperienceRate = this._economy.ExperienceRate;
            this._gameConfiguration.MasterExperienceRate = this._economy.MasterExperienceRate;
            this._gameConfiguration.MaximumInventoryMoney = this._economy.MaximumInventoryMoney;
            this._gameConfiguration.MaximumVaultMoney = this._economy.MaximumVaultMoney;
            this._gameConfiguration.ShouldDropMoney = this._economy.ShouldDropMoney;
            this._gameConfiguration.ItemDropDuration = TimeSpan.FromSeconds(this._economy.ItemDropDurationSeconds);

            foreach (var edit in this._serverEdits)
            {
                var server = this._servers.First(s => s.GetId() == edit.Id);
                server.ExperienceRate = edit.ExperienceRate;
                server.MoneyRate = edit.MoneyRate;
                server.PvpEnabled = edit.PvpEnabled;
            }

            foreach (var edit in this._dropChanceEdits)
            {
                var group = this._dropGroups.First(g => g.GetId() == edit.Id);
                group.Chance = edit.Percent / 100.0;
            }

            await this.SaveContextsAsync().ConfigureAwait(true);

            this._undoAction = async () =>
            {
                this._gameConfiguration.ExperienceRate = gameSnapshot.ExperienceRate;
                this._gameConfiguration.MasterExperienceRate = gameSnapshot.MasterExperienceRate;
                this._gameConfiguration.MaximumInventoryMoney = gameSnapshot.MaximumInventoryMoney;
                this._gameConfiguration.MaximumVaultMoney = gameSnapshot.MaximumVaultMoney;
                this._gameConfiguration.ShouldDropMoney = gameSnapshot.ShouldDropMoney;
                this._gameConfiguration.ItemDropDuration = TimeSpan.FromSeconds(gameSnapshot.ItemDropDurationSeconds);

                foreach (var server in this._servers)
                {
                    var snapshot = serverSnapshot[server.GetId()];
                    server.ExperienceRate = snapshot.ExperienceRate;
                    server.MoneyRate = snapshot.MoneyRate;
                    server.PvpEnabled = snapshot.PvpEnabled;
                }

                foreach (var pair in dropSnapshot)
                {
                    this._dropGroups.First(g => g.GetId() == pair.Key).Chance = pair.Value;
                }

                await this.SaveContextsAsync().ConfigureAwait(true);
            };

            if (this._restartAllAfterEconomyApply)
            {
                await this.ServerInstanceManager.RestartAllAsync(false).ConfigureAwait(true);
            }
        }, "Economía y canales guardados.").ConfigureAwait(true);
    }

    private void PreviewDropAssignment()
    {
        var group = this.GetSelectedDropGroup();
        if (group is null)
        {
            this.SetPreview("Drops masivos", [], "Selecciona un grupo de drop.");
            return;
        }

        var lines = new List<string>();
        if (this._dropScope == "maps")
        {
            foreach (var map in this.GetSelectedMaps())
            {
                var contains = map.DropItemGroups.Contains(group);
                if ((this._dropAction == "add" && !contains) || (this._dropAction == "remove" && contains))
                {
                    lines.Add($"{(this._dropAction == "add" ? "Agregar" : "Quitar")} '{group.Description}' {(this._dropAction == "add" ? "a" : "de")} mapa {map.Number} - {map.Name}");
                }
            }
        }
        else
        {
            foreach (var monster in this.GetMatchingMonsters())
            {
                var contains = monster.DropItemGroups.Contains(group);
                if ((this._dropAction == "add" && !contains) || (this._dropAction == "remove" && contains))
                {
                    lines.Add($"{(this._dropAction == "add" ? "Agregar" : "Quitar")} '{group.Description}' {(this._dropAction == "add" ? "a" : "de")} {monster.Designation} (lvl {GetMonsterLevel(monster)})");
                }
            }
        }

        this.SetPreview("Drops masivos", lines, $"{lines.Count} relaciones cambiarán.");
    }

    private async Task ApplyDropAssignmentAsync()
    {
        if (this._gameContext is null)
        {
            return;
        }

        var group = this.GetSelectedDropGroup();
        if (group is null)
        {
            this.ToastService.ShowError("Selecciona un grupo de drop.");
            return;
        }

        var snapshots = new List<DropMembershipSnapshot>();
        if (this._dropScope == "maps")
        {
            foreach (var map in this.GetSelectedMaps())
            {
                var had = map.DropItemGroups.Contains(group);
                if ((this._dropAction == "add" && !had) || (this._dropAction == "remove" && had))
                {
                    snapshots.Add(new DropMembershipSnapshot(map, null, group, had));
                }
            }
        }
        else
        {
            foreach (var monster in this.GetMatchingMonsters())
            {
                var had = monster.DropItemGroups.Contains(group);
                if ((this._dropAction == "add" && !had) || (this._dropAction == "remove" && had))
                {
                    snapshots.Add(new DropMembershipSnapshot(null, monster, group, had));
                }
            }
        }

        if (snapshots.Count == 0)
        {
            this.ToastService.ShowSuccess("No hay relaciones de drop para modificar.");
            return;
        }

        await this.RunApplyAsync(async () =>
        {
            foreach (var snapshot in snapshots)
            {
                SetDropMembership(snapshot.Map, snapshot.Monster, group, this._dropAction == "add");
            }

            await this.SaveGameContextAsync().ConfigureAwait(true);

            this._undoAction = async () =>
            {
                foreach (var snapshot in snapshots)
                {
                    SetDropMembership(snapshot.Map, snapshot.Monster, snapshot.Group, snapshot.HadGroup);
                }

                await this.SaveGameContextAsync().ConfigureAwait(true);
            };
        }, $"{snapshots.Count} relaciones de drop actualizadas.").ConfigureAwait(true);
    }

    private void PreviewMonsterBatch()
    {
        var targets = this.GetMatchingMonsters().ToList();
        var lines = new List<string>();

        foreach (var monster in targets)
        {
            var changes = new List<string>();
            if (this._batchMaximumItemDrops.HasValue && monster.NumberOfMaximumItemDrops != this._batchMaximumItemDrops.Value)
            {
                changes.Add($"max drops {monster.NumberOfMaximumItemDrops} → {this._batchMaximumItemDrops.Value}");
            }

            if (this._batchRespawnSeconds.HasValue
                && Math.Abs(monster.RespawnDelay.TotalSeconds - this._batchRespawnSeconds.Value) > 0.001)
            {
                changes.Add($"respawn {monster.RespawnDelay.TotalSeconds:0.##}s → {this._batchRespawnSeconds.Value:0.##}s");
            }

            if (changes.Count > 0)
            {
                lines.Add($"{monster.Designation} (lvl {GetMonsterLevel(monster)}): {string.Join(", ", changes)}");
            }
        }

        this.SetPreview("Monstruos por lote", lines, $"{lines.Count} monstruos cambiarán.");
    }

    private async Task ApplyMonsterBatchAsync()
    {
        if (this._gameContext is null)
        {
            return;
        }

        if (!this._batchMaximumItemDrops.HasValue && !this._batchRespawnSeconds.HasValue)
        {
            this.ToastService.ShowError("Define al menos un valor a modificar.");
            return;
        }

        if (this._batchMaximumItemDrops is < 0 || this._batchRespawnSeconds is < 0)
        {
            this.ToastService.ShowError("Max drops y respawn no pueden ser negativos.");
            return;
        }

        var targets = this.GetMatchingMonsters().ToList();
        var snapshots = targets.ToDictionary(
            m => m.GetId(),
            m => new MonsterBatchSnapshot(m.NumberOfMaximumItemDrops, m.RespawnDelay));

        await this.RunApplyAsync(async () =>
        {
            foreach (var monster in targets)
            {
                if (this._batchMaximumItemDrops.HasValue)
                {
                    monster.NumberOfMaximumItemDrops = this._batchMaximumItemDrops.Value;
                }

                if (this._batchRespawnSeconds.HasValue)
                {
                    monster.RespawnDelay = TimeSpan.FromSeconds(this._batchRespawnSeconds.Value);
                }
            }

            await this.SaveGameContextAsync().ConfigureAwait(true);

            this._undoAction = async () =>
            {
                foreach (var monster in targets)
                {
                    var snapshot = snapshots[monster.GetId()];
                    monster.NumberOfMaximumItemDrops = snapshot.MaximumItemDrops;
                    monster.RespawnDelay = snapshot.RespawnDelay;
                }

                await this.SaveGameContextAsync().ConfigureAwait(true);
            };
        }, $"{targets.Count} monstruos procesados.").ConfigureAwait(true);
    }

    private void PreviewItemBatch()
    {
        var targets = this.GetMatchingItems().ToList();
        var lines = new List<string>();
        foreach (var item in targets)
        {
            var changes = new List<string>();
            if (this._newItemDropLevel.HasValue && item.DropLevel != this._newItemDropLevel.Value)
            {
                changes.Add($"drop lvl {item.DropLevel} → {this._newItemDropLevel.Value}");
            }

            if (this._setItemMaximumDropLevel)
            {
                var newMax = this._newItemMaximumDropLevel.HasValue ? this._newItemMaximumDropLevel.Value.ToString(CultureInfo.InvariantCulture) : "sin límite";
                var oldMax = item.MaximumDropLevel?.ToString(CultureInfo.InvariantCulture) ?? "sin límite";
                if (oldMax != newMax)
                {
                    changes.Add($"max drop lvl {oldMax} → {newMax}");
                }
            }

            if (this._setDropsFromMonsters && item.DropsFromMonsters != this._newDropsFromMonsters)
            {
                changes.Add($"drop monstruos {item.DropsFromMonsters} → {this._newDropsFromMonsters}");
            }

            if (changes.Count > 0)
            {
                lines.Add($"[{item.Group},{item.Number}] {item.Name}: {string.Join(", ", changes)}");
            }
        }

        this.SetPreview("Items por lote", lines, $"{lines.Count} items cambiarán.");
    }

    private async Task ApplyItemBatchAsync()
    {
        if (this._gameContext is null)
        {
            return;
        }

        if (!this._newItemDropLevel.HasValue && !this._setItemMaximumDropLevel && !this._setDropsFromMonsters)
        {
            this.ToastService.ShowError("Define al menos un campo de item a modificar.");
            return;
        }

        if (!IsByteOrNull(this._newItemDropLevel) || !IsByteOrNull(this._newItemMaximumDropLevel))
        {
            this.ToastService.ShowError("Los niveles de drop deben estar entre 0 y 255.");
            return;
        }

        var targets = this.GetMatchingItems().ToList();
        var snapshots = targets.ToDictionary(
            i => i.GetId(),
            i => new ItemBatchSnapshot(i.DropLevel, i.MaximumDropLevel, i.DropsFromMonsters));

        await this.RunApplyAsync(async () =>
        {
            foreach (var item in targets)
            {
                if (this._newItemDropLevel.HasValue)
                {
                    item.DropLevel = (byte)this._newItemDropLevel.Value;
                }

                if (this._setItemMaximumDropLevel)
                {
                    item.MaximumDropLevel = this._newItemMaximumDropLevel.HasValue
                        ? (byte)this._newItemMaximumDropLevel.Value
                        : null;
                }

                if (this._setDropsFromMonsters)
                {
                    item.DropsFromMonsters = this._newDropsFromMonsters;
                }
            }

            await this.SaveGameContextAsync().ConfigureAwait(true);

            this._undoAction = async () =>
            {
                foreach (var item in targets)
                {
                    var snapshot = snapshots[item.GetId()];
                    item.DropLevel = snapshot.DropLevel;
                    item.MaximumDropLevel = snapshot.MaximumDropLevel;
                    item.DropsFromMonsters = snapshot.DropsFromMonsters;
                }

                await this.SaveGameContextAsync().ConfigureAwait(true);
            };
        }, $"{targets.Count} items procesados.").ConfigureAwait(true);
    }

    private ItemDropItemGroup? GetSelectedContentDropGroup()
        => this._contentDropGroupId.HasValue
            ? this._dropContentGroups.FirstOrDefault(group => group.GetId() == this._contentDropGroupId.Value)
            : null;

    private IReadOnlyList<ItemDefinition> GetMatchingContentItems()
        => DropGroupItemSelector.Filter(
            this._items,
            this._contentFilter,
            this._contentGroup,
            this._contentSetId,
            this._contentCategory);

    private async Task ApplyDropContentBatchAsync()
    {
        if (this._gameContext is null)
        {
            return;
        }

        var group = this.GetSelectedContentDropGroup();
        if (group is null)
        {
            this.ToastService.ShowError("Selecciona una caja o grupo de drop de items.");
            return;
        }

        if (!this._contentConfirmed)
        {
            this.ToastService.ShowError("Marca la confirmación para aplicar la edición directa.");
            return;
        }

        var targets = this.GetMatchingContentItems();
        if (targets.Count == 0)
        {
            this.ToastService.ShowError("Los filtros no encontraron items.");
            return;
        }

        if (this._contentAction is not ("add" or "remove" or "replace"))
        {
            this.ToastService.ShowError("La acción seleccionada no es válida.");
            return;
        }

        var original = group.PossibleItems.ToList();
        await this.RunApplyAsync(async () =>
        {
            if (this._contentAction == "replace")
            {
                foreach (var item in group.PossibleItems.ToList())
                {
                    group.PossibleItems.Remove(item);
                }
            }

            var targetIds = targets.Select(item => item.GetId()).ToHashSet();
            if (this._contentAction is "add" or "replace")
            {
                var existingIds = group.PossibleItems.Select(item => item.GetId()).ToHashSet();
                foreach (var item in targets.Where(item => !existingIds.Contains(item.GetId())))
                {
                    group.PossibleItems.Add(item);
                }
            }
            else
            {
                foreach (var item in group.PossibleItems.Where(item => targetIds.Contains(item.GetId())).ToList())
                {
                    group.PossibleItems.Remove(item);
                }
            }

            await this.SaveGameContextAsync().ConfigureAwait(true);
            this._contentConfirmed = false;
            var action = this._contentAction switch
            {
                "add" => "agregados",
                "remove" => "quitados",
                _ => "reemplazados",
            };
            var message = $"{targets.Count} items {action} en {group.Description}.";
            this.SetPreview("Edición directa aplicada", [], message);
            this._undoAction = async () =>
            {
                foreach (var item in group.PossibleItems.ToList())
                {
                    group.PossibleItems.Remove(item);
                }

                foreach (var item in original)
                {
                    group.PossibleItems.Add(item);
                }

                await this.SaveGameContextAsync().ConfigureAwait(true);
            };
        }, $"{targets.Count} items modificados en {group.Description}. Puedes deshacer la última operación.").ConfigureAwait(true);
    }
    private void PreviewMerchantClone()
    {
        var source = this.GetSourceMerchant();
        if (source?.MerchantStore is null)
        {
            this.SetPreview("Plantilla de tienda", [], "Selecciona una tienda origen.");
            return;
        }

        var targets = this.GetTargetMerchants(source.GetId()).ToList();
        var lines = targets
            .Select(target => $"{target.Designation}: {target.MerchantStore!.Items.Count} items → {source.MerchantStore.Items.Count} items")
            .ToList();

        this.SetPreview("Plantilla de tienda", lines, $"{lines.Count} tiendas serán reemplazadas por una copia de {source.Designation}.");
    }

    private async Task ApplyMerchantCloneAsync()
    {
        if (this._gameContext is null)
        {
            return;
        }

        var source = this.GetSourceMerchant();
        if (source?.MerchantStore is null)
        {
            this.ToastService.ShowError("Selecciona una tienda origen.");
            return;
        }

        var targets = this.GetTargetMerchants(source.GetId()).ToList();
        if (targets.Count == 0)
        {
            this.ToastService.ShowError("Selecciona al menos una tienda destino.");
            return;
        }

        var snapshots = targets.ToDictionary(
            m => m.GetId(),
            m => m.MerchantStore!.Items.Select(CreateShopItemSnapshot).ToList());

        var sourceSnapshots = source.MerchantStore.Items.Select(CreateShopItemSnapshot).ToList();

        await this.RunApplyAsync(async () =>
        {
            foreach (var target in targets)
            {
                await this.ReplaceMerchantItemsAsync(target, sourceSnapshots).ConfigureAwait(true);
            }

            await this.SaveGameContextAsync().ConfigureAwait(true);

            this._undoAction = async () =>
            {
                foreach (var target in targets)
                {
                    await this.ReplaceMerchantItemsAsync(target, snapshots[target.GetId()]).ConfigureAwait(true);
                }

                await this.SaveGameContextAsync().ConfigureAwait(true);
            };
        }, $"{targets.Count} tiendas clonadas. Puedes deshacer la última operación mientras permanezcas en esta pantalla.").ConfigureAwait(true);
    }

    private async Task UndoLastAsync()
    {
        if (this._undoAction is null)
        {
            return;
        }

        await this.RunApplyAsync(async () =>
        {
            var undo = this._undoAction;
            this._undoAction = null;
            await undo().ConfigureAwait(true);
        }, "Última operación revertida.").ConfigureAwait(true);
    }

    private async Task ReplaceMerchantItemsAsync(MonsterDefinition merchant, IReadOnlyList<ShopItemSnapshot> snapshots)
    {
        if (this._gameContext is null || merchant.MerchantStore is null)
        {
            return;
        }

        foreach (var oldItem in merchant.MerchantStore.Items.ToList())
        {
            merchant.MerchantStore.Items.Remove(oldItem);
            await this._gameContext.DeleteAsync(oldItem).ConfigureAwait(true);
        }

        foreach (var snapshot in snapshots)
        {
            merchant.MerchantStore.Items.Add(CreateItemFromSnapshot(this._gameContext, snapshot));
        }
    }

    private async Task RunApplyAsync(Func<Task> action, string successMessage)
    {
        if (this._isApplying)
        {
            return;
        }

        this._isApplying = true;
        using var loading = this.LoadingService.ShowLoadingIndicator();
        try
        {
            await action().ConfigureAwait(true);
            this.ToastService.ShowSuccess(successMessage);
            this._previewTitle = "Aplicado";
            this._previewSummary = successMessage;
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Batch operation failed.");
            this.ToastService.ShowError($"La operación falló: {ex.Message}");
        }
        finally
        {
            this._isApplying = false;
        }
    }

    private async Task SaveContextsAsync()
    {
        await this.SaveGameContextAsync().ConfigureAwait(true);
        if (this._serverContext?.HasChanges is true)
        {
            await this._serverContext.SaveChangesAsync().ConfigureAwait(true);
        }
    }

    private async Task SaveGameContextAsync()
    {
        if (this._gameContext?.HasChanges is true)
        {
            await this._gameContext.SaveChangesAsync().ConfigureAwait(true);
        }
    }

    private DropItemGroup? GetSelectedDropGroup()
        => this._selectedDropGroupId.HasValue
            ? this._dropGroups.FirstOrDefault(g => g.GetId() == this._selectedDropGroupId.Value)
            : null;

    private IEnumerable<GameMapDefinition> GetSelectedMaps()
        => this._maps.Where(m => this._selectedMapIds.Contains(m.GetId()));

    private IEnumerable<MonsterDefinition> GetMatchingMonsters()
    {
        IEnumerable<MonsterDefinition> result = this._monsters;

        if (!string.IsNullOrWhiteSpace(this._monsterFilter))
        {
            result = result.Where(m => (m.Designation.ToString() ?? string.Empty).Contains(this._monsterFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (this._monsterMinLevel.HasValue)
        {
            result = result.Where(m => GetMonsterLevel(m) >= this._monsterMinLevel.Value);
        }

        if (this._monsterMaxLevel.HasValue)
        {
            result = result.Where(m => GetMonsterLevel(m) <= this._monsterMaxLevel.Value);
        }

        if (this._monsterOnlySelectedMaps && this._selectedMapIds.Count > 0)
        {
            var ids = this.GetSelectedMaps()
                .SelectMany(m => m.MonsterSpawns)
                .Where(s => s.MonsterDefinition is not null)
                .Select(s => s.MonsterDefinition!.GetId())
                .ToHashSet();

            result = result.Where(m => ids.Contains(m.GetId()));
        }

        return result;
    }

    private IEnumerable<ItemDefinition> GetMatchingItems()
    {
        IEnumerable<ItemDefinition> result = this._items;

        if (!string.IsNullOrWhiteSpace(this._itemFilter))
        {
            result = result.Where(i => (i.Name.ToString() ?? string.Empty).Contains(this._itemFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (this._itemGroup.HasValue)
        {
            result = result.Where(i => i.Group == this._itemGroup.Value);
        }

        if (this._itemCurrentMinDropLevel.HasValue)
        {
            result = result.Where(i => i.DropLevel >= this._itemCurrentMinDropLevel.Value);
        }

        if (this._itemCurrentMaxDropLevel.HasValue)
        {
            result = result.Where(i => i.DropLevel <= this._itemCurrentMaxDropLevel.Value);
        }

        return result;
    }

    private MonsterDefinition? GetSourceMerchant()
        => this._sourceMerchantId.HasValue
            ? this._merchants.FirstOrDefault(m => m.GetId() == this._sourceMerchantId.Value)
            : null;

    private IEnumerable<MonsterDefinition> GetTargetMerchants(Guid sourceId)
        => this._merchants.Where(m => m.GetId() != sourceId && this._targetMerchantIds.Contains(m.GetId()));

    private void SetPreview(string title, IReadOnlyList<string> lines, string summary)
    {
        this._previewTitle = title;
        this._previewSummary = summary;
        this._previewLines = lines.Take(250).ToList();
        if (lines.Count > 250)
        {
            this._previewLines.Add($"… y {lines.Count - 250} cambios adicionales.");
        }
    }

    private static void SetDropMembership(GameMapDefinition? map, MonsterDefinition? monster, DropItemGroup group, bool shouldContain)
    {
        var collection = map?.DropItemGroups ?? monster?.DropItemGroups;
        if (collection is null)
        {
            return;
        }

        var contains = collection.Contains(group);
        if (shouldContain && !contains)
        {
            collection.Add(group);
        }
        else if (!shouldContain && contains)
        {
            collection.Remove(group);
        }
    }

    private static bool IsCommonEconomyDropGroup(DropItemGroup group)
        => group.Monster is null
           && !group.MinimumMonsterLevel.HasValue
           && !group.MaximumMonsterLevel.HasValue
           && group.ItemType is SpecialItemType.Money
               or SpecialItemType.RandomItem
               or SpecialItemType.Excellent
               or SpecialItemType.Jewel;

    private static int GetMonsterLevel(MonsterDefinition monster)
        => (int)(monster.Attributes.FirstOrDefault(a => a.AttributeDefinition == Stats.Level)?.Value ?? 0);

    private static bool IsByteOrNull(int? value)
        => !value.HasValue || value is >= byte.MinValue and <= byte.MaxValue;

    private static void AddDiff<T>(ICollection<string> lines, string label, T oldValue, T newValue, string suffix = "")
    {
        if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
        {
            return;
        }

        lines.Add($"{label}: {oldValue}{suffix} → {newValue}{suffix}");
    }

    private static ShopItemSnapshot CreateShopItemSnapshot(Item item)
        => new(
            item.ItemSlot,
            item.Definition,
            item.Durability,
            item.Level,
            item.HasSkill,
            item.SocketCount,
            item.StorePrice,
            item.PetExperience,
            item.ItemOptions.Select(option => new ShopOptionSnapshot(option.ItemOption, option.Level, option.Index)).ToList(),
            item.ItemSetGroups.ToList());

    private static Item CreateItemFromSnapshot(IContext context, ShopItemSnapshot snapshot)
    {
        var item = context.CreateNew<Item>();
        item.ItemSlot = snapshot.ItemSlot;
        item.Definition = snapshot.Definition;
        item.Durability = snapshot.Durability;
        item.Level = snapshot.Level;
        item.HasSkill = snapshot.HasSkill;
        item.SocketCount = snapshot.SocketCount;
        item.StorePrice = snapshot.StorePrice;
        item.PetExperience = snapshot.PetExperience;

        foreach (var optionSnapshot in snapshot.Options)
        {
            var option = context.CreateNew<ItemOptionLink>();
            option.ItemOption = optionSnapshot.ItemOption;
            option.Level = optionSnapshot.Level;
            option.Index = optionSnapshot.Index;
            item.ItemOptions.Add(option);
        }

        foreach (var setGroup in snapshot.ItemSetGroups)
        {
            item.ItemSetGroups.Add(setGroup);
        }

        return item;
    }

    private sealed class EconomyEdit
    {
        public float ExperienceRate { get; set; }

        public float MasterExperienceRate { get; set; }

        public int MaximumInventoryMoney { get; set; }

        public int MaximumVaultMoney { get; set; }

        public bool ShouldDropMoney { get; set; }

        public double ItemDropDurationSeconds { get; set; }
    }

    private sealed class ServerRateEdit
    {
        public ServerRateEdit(GameServerDefinition server)
        {
            this.Id = server.GetId();
            this.ServerId = server.ServerID;
            this.Description = server.Description;
            this.ExperienceRate = server.ExperienceRate;
            this.MoneyRate = server.MoneyRate;
            this.PvpEnabled = server.PvpEnabled;
        }

        public Guid Id { get; }

        public byte ServerId { get; }

        public string Description { get; }

        public float ExperienceRate { get; set; }

        public float MoneyRate { get; set; }

        public bool PvpEnabled { get; set; }
    }

    private sealed class DropChanceEdit
    {
        public DropChanceEdit(DropItemGroup group)
        {
            this.Id = group.GetId();
            this.Description = group.Description.ToString() ?? string.Empty;
            this.Type = group.ItemType;
            this.Percent = group.Chance * 100.0;
        }

        public Guid Id { get; }

        public string Description { get; }

        public SpecialItemType Type { get; }

        public double Percent { get; set; }
    }

    private sealed record ServerSnapshot(float ExperienceRate, float MoneyRate, bool PvpEnabled);

    private sealed record DropMembershipSnapshot(GameMapDefinition? Map, MonsterDefinition? Monster, DropItemGroup Group, bool HadGroup);

    private sealed record MonsterBatchSnapshot(int MaximumItemDrops, TimeSpan RespawnDelay);

    private sealed record ItemBatchSnapshot(byte DropLevel, byte? MaximumDropLevel, bool DropsFromMonsters);

    private sealed record ShopOptionSnapshot(IncreasableItemOption? ItemOption, int Level, int Index);

    private sealed record ShopItemSnapshot(
        byte ItemSlot,
        ItemDefinition? Definition,
        double Durability,
        byte Level,
        bool HasSkill,
        int SocketCount,
        int? StorePrice,
        int PetExperience,
        IReadOnlyList<ShopOptionSnapshot> Options,
        IReadOnlyList<ItemOfItemSet> ItemSetGroups);
}

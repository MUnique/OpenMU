// <copyright file="InvasionSpawnTable.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Form;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;
using MUnique.OpenMU.Persistence;

/// <summary>
/// A component which shows a collection of <see cref="InvasionSpawnConfiguration"/> in a table.
/// </summary>
public partial class InvasionSpawnTable : InputBase<IList<InvasionSpawnConfiguration>>
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private List<SpawnViewModel> _viewModels = new();
    private IEnumerable<MonsterDefinition>? _allMonsters;
    private IEnumerable<GameMapDefinition>? _allMaps;
    private bool _parsing;

    /// <summary>
    /// Gets or sets the label.
    /// </summary>
    [Parameter]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the persistence context.
    /// </summary>
    [CascadingParameter]
    public IContext PersistenceContext { get; set; } = null!;

    [Inject]
    private IDataSource<GameConfiguration> GameConfigurationSource { get; set; } = null!;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(false);
        if (this.Value is null)
        {
            this.Value = new List<InvasionSpawnConfiguration>();
        }

        await this.GameConfigurationSource.GetOwnerAsync().ConfigureAwait(false);
        this._allMonsters = this.GameConfigurationSource.GetAll<MonsterDefinition>();
        this._allMaps = this.GameConfigurationSource.GetAll<GameMapDefinition>();

        this._viewModels = this.Value.Select(v => new SpawnViewModel(v, this)).ToList();
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out IList<InvasionSpawnConfiguration> result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        if (this._parsing)
        {
            result = this.Value ?? new List<InvasionSpawnConfiguration>();
            validationErrorMessage = null;
            return true;
        }

        this._parsing = true;
        try
        {
            result = JsonSerializer.Deserialize<IList<InvasionSpawnConfiguration>>(value ?? "[]", JsonOptions) ?? new List<InvasionSpawnConfiguration>();
            validationErrorMessage = null;
            return true;
        }
        catch (Exception ex)
        {
            result = this.Value ?? new List<InvasionSpawnConfiguration>();
            validationErrorMessage = ex.Message;
            return true;
        }
        finally
        {
            this._parsing = false;
        }
    }

    /// <inheritdoc />
    protected override string FormatValueAsString(IList<InvasionSpawnConfiguration>? value)
    {
        return JsonSerializer.Serialize(value ?? new List<InvasionSpawnConfiguration>(), JsonOptions);
    }

    private void OnAddClick()
    {
        var newSpawn = new InvasionSpawnConfiguration();
        this.Value?.Add(newSpawn);
        this._viewModels.Add(new SpawnViewModel(newSpawn, this));
    }

    private void OnRemoveClick(SpawnViewModel viewModel)
    {
        this.Value?.Remove(viewModel.Model);
        this._viewModels.Remove(viewModel);
    }

    private MonsterDefinition? GetMonster(ushort monsterId)
    {
        if (monsterId == 0)
        {
            return null;
        }

        return this._allMonsters?.FirstOrDefault(m => m.Number == monsterId);
    }

    private IList<GameMapDefinition> GetMaps(IList<ushort> mapIds)
    {
        if (mapIds.Count == 0 || this._allMaps is null)
        {
            return new List<GameMapDefinition>();
        }

        return mapIds.Select(id => this._allMaps.FirstOrDefault(m => m.Number == (short)id))
            .Where(map => map is not null)
            .Cast<GameMapDefinition>()
            .ToList();
    }

    private void SetMaps(InvasionSpawnConfiguration spawn, IList<GameMapDefinition> maps)
    {
        spawn.MapIds = maps.Select(m => (ushort)m.Number).ToList();
    }

    /// <summary>
    /// View model for a single invasion spawn configuration to facilitate better Blazor data binding.
    /// </summary>
    public class SpawnViewModel
    {
        private readonly InvasionSpawnConfiguration _model;
        private readonly InvasionSpawnTable _parent;
        private SyncedMapList _cachedMaps;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnViewModel"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="parent">The parent component.</param>
        public SpawnViewModel(InvasionSpawnConfiguration model, InvasionSpawnTable parent)
        {
            this._model = model;
            this._parent = parent;
            var initialMaps = parent.GetMaps(model.MapIds);
            this._cachedMaps = new SyncedMapList(initialMaps, this);
        }

        /// <summary>
        /// Gets the underlying model.
        /// </summary>
        public InvasionSpawnConfiguration Model => this._model;

        /// <summary>
        /// Gets or sets the maps where the monster can spawn.
        /// </summary>
        public IList<GameMapDefinition> Maps
        {
            get => this._cachedMaps;
            set
            {
                this._cachedMaps = new SyncedMapList(value, this);
                this.SyncMapIdsToModel();
            }
        }

        /// <summary>
        /// Gets or sets the monster definition.
        /// </summary>
        public MonsterDefinition? Monster
        {
            get => this._parent.GetMonster(this._model.MonsterId);
            set => this._model.MonsterId = (ushort)(value?.Number ?? 0);
        }

        /// <summary>
        /// Gets or sets the quantity of monsters to spawn.
        /// </summary>
        public int Count
        {
            get => this._model.Count;
            set => this._model.Count = (byte)value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to spawn on all maps or just a random one.
        /// Delegates to <see cref="InvasionSpawnConfiguration.MapStrategy"/> via the
        /// <see cref="InvasionSpawnConfiguration.IsSpawnOnAllMaps"/> bridge property.
        /// </summary>
        public bool IsSpawnOnAllMaps
        {
            get => this._model.IsSpawnOnAllMaps;
            set => this._model.IsSpawnOnAllMaps = value;
        }

        /// <summary>
        /// Gets or sets the fixed X coordinate.
        /// </summary>
        public int? X
        {
            get => this._model.X;
            set => this._model.X = (byte?)value;
        }

        /// <summary>
        /// Gets or sets the fixed Y coordinate.
        /// </summary>
        public int? Y
        {
            get => this._model.Y;
            set => this._model.Y = (byte?)value;
        }

        private void SyncMapIdsToModel()
        {
            this._model.MapIds = this._cachedMaps
                .Select(m => (ushort)m.Number)
                .ToList();
        }

        /// <summary>
        /// A list wrapper that automatically syncs changes back to the model's MapIds.
        /// </summary>
        private class SyncedMapList : IList<GameMapDefinition>
        {
            private readonly List<GameMapDefinition> _inner;
            private readonly SpawnViewModel _owner;

            /// <summary>
            /// Initializes a new instance of the <see cref="SyncedMapList"/> class.
            /// </summary>
            /// <param name="items">The initial map items.</param>
            /// <param name="owner">The owning view model to sync back to.</param>
            public SyncedMapList(IEnumerable<GameMapDefinition> items, SpawnViewModel owner)
            {
                this._inner = new List<GameMapDefinition>(items);
                this._owner = owner;
            }

            /// <inheritdoc />
            public int Count => this._inner.Count;

            /// <inheritdoc />
            public bool IsReadOnly => false;

            /// <inheritdoc />
            public GameMapDefinition this[int index]
            {
                get => this._inner[index];
                set
                {
                    this._inner[index] = value;
                    this._owner.SyncMapIdsToModel();
                }
            }

            /// <inheritdoc />
            public void Add(GameMapDefinition item)
            {
                this._inner.Add(item);
                this._owner.SyncMapIdsToModel();
            }

            /// <inheritdoc />
            public bool Remove(GameMapDefinition item)
            {
                var result = this._inner.Remove(item);
                if (result)
                {
                    this._owner.SyncMapIdsToModel();
                }

                return result;
            }

            /// <inheritdoc />
            public void Clear()
            {
                this._inner.Clear();
                this._owner.SyncMapIdsToModel();
            }

            /// <inheritdoc />
            public void Insert(int index, GameMapDefinition item)
            {
                this._inner.Insert(index, item);
                this._owner.SyncMapIdsToModel();
            }

            /// <inheritdoc />
            public void RemoveAt(int index)
            {
                this._inner.RemoveAt(index);
                this._owner.SyncMapIdsToModel();
            }

            /// <inheritdoc />
            public bool Contains(GameMapDefinition item) => this._inner.Contains(item);

            /// <inheritdoc />
            public void CopyTo(GameMapDefinition[] array, int arrayIndex) => this._inner.CopyTo(array, arrayIndex);

            /// <inheritdoc />
            public int IndexOf(GameMapDefinition item) => this._inner.IndexOf(item);

            /// <inheritdoc />
            public IEnumerator<GameMapDefinition> GetEnumerator() => this._inner.GetEnumerator();

            /// <inheritdoc />
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();
        }
    }
}
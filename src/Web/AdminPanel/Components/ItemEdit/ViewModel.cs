// <copyright file="ViewModel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.ItemEdit;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence;
using Nito.AsyncEx.Synchronous;
using Nito.Disposables.Internals;

/// <summary>
/// The view model for an <see cref="Item"/>.
/// </summary>
public class ViewModel : INotifyPropertyChanged
{
    private readonly IContext _persistenceContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModel"/> class.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="persistenceContext">The persistence context.</param>
    public ViewModel(Item item, IContext persistenceContext)
    {
        this._persistenceContext = persistenceContext ?? throw new ArgumentNullException(nameof(persistenceContext));
        this.Item = item ?? throw new ArgumentNullException(nameof(item));
        var excellentOptions = new ItemOptionList(ItemOptionTypes.Excellent, item, persistenceContext);
        var wingOptions = new ItemOptionList(ItemOptionTypes.Wing, item, persistenceContext);
        this.Sockets = new List<SocketViewModel>(
            Enumerable.Range(0, this.Definition?.MaximumSockets ?? 0)
                .Select(i =>
                {
                    var socket = new SocketViewModel(item, this._persistenceContext, i, this.PossibleSocketOptions);
                    socket.OptionChanged += this.OnSocketOptionChanged;
                    return socket;
                }));

        excellentOptions.ListChanged += (_, _) => this.OnPropertyChanged(nameof(this.ExcellentOptions));
        wingOptions.ListChanged += (_, _) => this.OnPropertyChanged(nameof(this.WingOptions));

        this.ExcellentOptions = excellentOptions;
        this.WingOptions = wingOptions;
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the item.
    /// </summary>
    public Item Item { get; }

    /// <summary>
    /// Gets or sets the item slot.
    /// </summary>
    public byte ItemSlot
    {
        get => this.Item.ItemSlot;
        set
        {
            if (value == this.Item.ItemSlot)
            {
                return;
            }

            this.Item.ItemSlot = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the definition.
    /// </summary>
    public ItemDefinition? Definition
    {
        get => this.Item.Definition;
        set
        {
            if (this.Definition == value)
            {
                return;
            }

            this.Item.Definition = value;
            this.OnDefinitionChanged();
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the level.
    /// </summary>
    public byte Level
    {
        get => this.Item.Level;
        set
        {
            if (this.Item.Level == value)
            {
                return;
            }

            if (value > this.Definition?.MaximumItemLevel)
            {
                return;
            }

            this.Item.Level = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the amount for items which are not wearable.
    /// </summary>
    public byte Amount
    {
        get => (byte)this.Durability;
        set => this.Durability = value;
    }

    /// <summary>
    /// Gets or sets the durability.
    /// </summary>
    public double Durability
    {
        get => this.Item.Durability;
        set
        {
            if (Math.Abs(this.Item.Durability - value) < 0.0001)
            {
                return;
            }

            var maxDurability = this.Item.IsStackable() ? this.Definition?.Durability ?? 0 : this.Item.GetMaximumDurabilityOfOnePiece();

            this.Item.Durability = Math.Min(value, maxDurability);
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this item has skill.
    /// </summary>
    public bool HasSkill
    {
        get => this.Item.HasSkill;
        set
        {
            if (this.Item.HasSkill == value)
            {
                return;
            }

            this.Item.HasSkill = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this item has luck.
    /// </summary>
    public bool HasLuck
    {
        get => this.Item.ItemOptions.Any(io => io.ItemOption?.OptionType == ItemOptionTypes.Luck);
        set
        {
            if (this.HasLuck == value)
            {
                return;
            }

            if (!value)
            {
                if (this.Item.ItemOptions.FirstOrDefault(io => io.ItemOption?.OptionType == ItemOptionTypes.Luck) is { } optionLink)
                {
                    this.Item.ItemOptions.Remove(optionLink);
                    this._persistenceContext.DeleteAsync(optionLink).AsTask().WaitAndUnwrapException();
                }
            }
            else if (value && this.PossibleLuckOption is { } luckOption)
            {
                var optionLink = this._persistenceContext.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = luckOption;
                this.Item.ItemOptions.Add(optionLink);
            }
            else
            {
                // can't add luck when it's not possible.
            }

            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this item has guardian option.
    /// </summary>
    public bool HasGuardianOption
    {
        get => this.Item.ItemOptions.Any(io => io.ItemOption?.OptionType == ItemOptionTypes.GuardianOption);
        set
        {
            if (this.HasGuardianOption == value)
            {
                return;
            }

            foreach (var optionLink in this.Item.ItemOptions.Where(io => io.ItemOption?.OptionType == ItemOptionTypes.GuardianOption))
            {
                this.Item.ItemOptions.Remove(optionLink);
                this._persistenceContext.DeleteAsync(optionLink).AsTask().WaitAndUnwrapException();
            }

            if (value && this.PossibleGuardianOption is { } guardianOption)
            {
                foreach (var option in guardianOption.PossibleOptions)
                {
                    var optionLink = this._persistenceContext.CreateNew<ItemOptionLink>();
                    optionLink.ItemOption = option;
                    this.Item.ItemOptions.Add(optionLink);
                }
            }

            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets the possible luck option.
    /// </summary>
    public IncreasableItemOption? PossibleLuckOption
    {
        get
        {
            return this.Definition?.PossibleItemOptions
                .SelectMany(p => p.PossibleOptions)
                .FirstOrDefault(o => o.OptionType == ItemOptionTypes.Luck);
        }
    }

    /// <summary>
    /// Gets the normal option link, if available.
    /// </summary>
    public ItemOptionLink? NormalOptionLink => this.Item.ItemOptions.FirstOrDefault(io => io.ItemOption?.OptionType == ItemOptionTypes.Option);

    /// <summary>
    /// Gets or sets the normal option, if assigned.
    /// </summary>
    public IncreasableItemOption? NormalOption
    {
        get => this.NormalOptionLink?.ItemOption;
        set
        {
            if (this.NormalOption == value)
            {
                return;
            }

            if (value is null)
            {
                if (this.NormalOptionLink is { } optionLink)
                {
                    this.Item.ItemOptions.Remove(optionLink);
                    this._persistenceContext.DeleteAsync(optionLink).AsTask().WaitAndUnwrapException();
                }
            }
            else
            {
                var optionLink = this.NormalOptionLink;
                if (optionLink is null)
                {
                    optionLink = this._persistenceContext.CreateNew<ItemOptionLink>();
                    this.Item.ItemOptions.Add(optionLink);
                }

                optionLink.ItemOption = value;
            }
        }
    }

    /// <summary>
    /// Gets the normal option link, if available.
    /// </summary>
    public ItemOptionLink? HarmonyOptionLink => this.Item.ItemOptions.FirstOrDefault(io => io.ItemOption?.OptionType == ItemOptionTypes.HarmonyOption);

    /// <summary>
    /// Gets or sets the normal option, if assigned.
    /// </summary>
    public IncreasableItemOption? HarmonyOption
    {
        get => this.HarmonyOptionLink?.ItemOption;
        set
        {
            if (this.HarmonyOption == value)
            {
                return;
            }

            if (value is null)
            {
                if (this.HarmonyOptionLink is { } optionLink)
                {
                    this.Item.ItemOptions.Remove(optionLink);
                    this._persistenceContext.DeleteAsync(optionLink).AsTask().WaitAndUnwrapException();
                }
            }
            else
            {
                var optionLink = this.HarmonyOptionLink;
                if (optionLink is null)
                {
                    optionLink = this._persistenceContext.CreateNew<ItemOptionLink>();
                    this.Item.ItemOptions.Add(optionLink);
                }

                optionLink.ItemOption = value;
                optionLink.Level = value.LevelDependentOptions.Min(ldo => ldo.Level);
            }
        }
    }

    /// <summary>
    /// Gets or sets the socket bonus option.
    /// </summary>
    public IncreasableItemOption? SocketBonusOption
    {
        get => this.SocketBonusOptionLink?.ItemOption;
        set
        {
            if (this.SocketBonusOption == value)
            {
                return;
            }

            if (value is null)
            {
                if (this.SocketBonusOptionLink is { } optionLink)
                {
                    this.Item.ItemOptions.Remove(optionLink);
                    this._persistenceContext.DeleteAsync(optionLink).AsTask().WaitAndUnwrapException();
                }
            }
            else
            {
                var optionLink = this.SocketBonusOptionLink;
                if (optionLink is null)
                {
                    optionLink = this._persistenceContext.CreateNew<ItemOptionLink>();
                    this.Item.ItemOptions.Add(optionLink);
                }

                optionLink.ItemOption = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the fenrir option.
    /// </summary>
    public ItemOptionType? FenrirOption
    {
        get => this.FenrirOptionLinks.FirstOrDefault()?.ItemOption?.OptionType;
        set
        {
            if (this.FenrirOption == value)
            {
                return;
            }

            foreach (var optionLink in this.FenrirOptionLinks.ToList())
            {
                this.Item.ItemOptions.Remove(optionLink);
                this._persistenceContext.DeleteAsync(optionLink).AsTask().WaitAndUnwrapException();
            }

            foreach (var option in this.PossibleFenrirOptions.Where(o => o.OptionType == value))
            {
                var optionLink = this._persistenceContext.CreateNew<ItemOptionLink>();
                this.Item.ItemOptions.Add(optionLink);
                optionLink.ItemOption = option;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this item has a "normal" option.
    /// It comes to action when there is just one possible option.
    /// </summary>
    public bool HasOption
    {
        get => this.NormalOption is { };
        set
        {
            if (this.HasOption == value)
            {
                return;
            }

            this.NormalOption = value ? this.PossibleNormalOptions.FirstOrDefault() : null;

            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the socket count.
    /// </summary>
    public int SocketCount
    {
        get => this.Item.SocketCount;
        set
        {
            if (this.Item.SocketCount == value)
            {
                return;
            }

            var maximumSockets = this.Item.Definition?.MaximumSockets ?? 0;

            this.Item.SocketCount = Math.Min(value, maximumSockets);

            // Remove sockets above the max.
            while (this.Sockets.Count > maximumSockets)
            {
                var i = this.Sockets.Count - 1;
                this.Sockets[i].Option = null;
                this.Sockets[i].OptionChanged -= this.OnSocketOptionChanged;
                this.Sockets.RemoveAt(i);
            }

            // Add missing sockets.
            for (int i = this.Sockets.Count; i < maximumSockets; i++)
            {
                var socketViewModel = new SocketViewModel(this.Item, this._persistenceContext, i, this.PossibleSocketOptions);
                socketViewModel.OptionChanged += this.OnSocketOptionChanged;
                this.Sockets.Add(socketViewModel);
            }

            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the socket view items.
    /// </summary>
    public List<SocketViewModel> Sockets { get; set; }

    /// <summary>
    /// Gets or sets the pet experience.
    /// </summary>
    public int PetExperience
    {
        get => this.Item.PetExperience;
        set
        {
            if (this.Item.PetExperience == value)
            {
                return;
            }

            this.Item.PetExperience = this.Item.IsTrainablePet() ? value : 0;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the excellent options.
    /// </summary>
    public IList<IncreasableItemOption> ExcellentOptions { get; set; }

    /// <summary>
    /// Gets or sets the wing options.
    /// </summary>
    public IList<IncreasableItemOption> WingOptions { get; set; }

    /// <summary>
    /// Gets or sets the ancient set.
    /// </summary>
    public ItemSetGroup? AncientSet
    {
        get => this.ItemSetGroups.FirstOrDefault(io => io.AncientSetDiscriminator > 0)?.ItemSetGroup;
        set
        {
            if (this.AncientSet == value)
            {
                return;
            }

            if (this.AncientSet is not null)
            {
                foreach (var isg in this.ItemSetGroups.Where(g => g.AncientSetDiscriminator > 0).ToList())
                {
                    this.Item.ItemSetGroups.Remove(isg);
                }

                if (this.AncientBonus is { } optionLink)
                {
                    this.ItemOptions.Remove(optionLink);
                    this._persistenceContext.DeleteAsync(optionLink).AsTask().WaitAndUnwrapException();
                }
            }

            if (value is not null)
            {
                var itemOfSet = value.Items.First(ios => ios.ItemDefinition == this.Definition);
                this.ItemSetGroups.Add(itemOfSet);

                if (itemOfSet.BonusOption is { } bonusOption)
                {
                    var optionLink = this._persistenceContext.CreateNew<ItemOptionLink>();
                    optionLink.ItemOption = bonusOption;
                    optionLink.Level = 1;
                    this.ItemOptions.Add(optionLink);
                }
            }

            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets the ancient bonus.
    /// </summary>
    public ItemOptionLink? AncientBonus => this.ItemOptions.FirstOrDefault(io => io.ItemOption?.OptionType == ItemOptionTypes.AncientBonus);

    /// <summary>
    /// Gets the possible ancient sets for this item.
    /// </summary>
    [Browsable(false)]
    public IEnumerable<ItemSetGroup> PossibleAncientSets
    {
        get
        {
            return this.PossibleAncientSetItems
                .Select(isg => isg.ItemSetGroup)
                .WhereNotNull()
                .Where(isg => isg.Items.Any(it => it.ItemDefinition == this.Item.Definition));
        }
    }

    /// <summary>
    /// Gets the possible excellent options.
    /// </summary>
    public IEnumerable<ItemOptionDefinition> PossibleExcellentOptions
    {
        get
        {
            return this.Definition?.PossibleItemOptions
                .Where(iod => iod.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.Excellent))
                ?? Enumerable.Empty<ItemOptionDefinition>();
        }
    }

    /// <summary>
    /// Gets the possible wing options.
    /// </summary>
    public IEnumerable<ItemOptionDefinition> PossibleWingOptions
    {
        get
        {
            return this.Definition?.PossibleItemOptions
                       .Where(iod => iod.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.Wing))
                   ?? Enumerable.Empty<ItemOptionDefinition>();
        }
    }

    /// <summary>
    /// Gets the possible normal options.
    /// </summary>
    public IEnumerable<IncreasableItemOption> PossibleNormalOptions
    {
        get
        {
            return this.Definition?.PossibleItemOptions
                       .Where(iod => iod.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.Option))
                       .SelectMany(iod => iod.PossibleOptions)
                   ?? Enumerable.Empty<IncreasableItemOption>();
        }
    }

    /// <summary>
    /// Gets the possible socket options.
    /// </summary>
    public IEnumerable<SocketOptionViewModel> PossibleSocketOptions
    {
        get
        {
            return this.Definition?.PossibleItemOptions
                       .Where(iod => iod.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.SocketOption))
                       .SelectMany(iod => iod.PossibleOptions.Select(o => new SocketOptionViewModel(iod, o)))
                   ?? Enumerable.Empty<SocketOptionViewModel>();
        }
    }

    /// <summary>
    /// Gets the possible socket bonus options.
    /// </summary>
    public IEnumerable<IncreasableItemOption> PossibleSocketBonusOptions
    {
        get
        {
            return this.Definition?.PossibleItemOptions
                       .Where(iod => iod.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.SocketBonusOption))
                       .SelectMany(iod => iod.PossibleOptions)
                   ?? Enumerable.Empty<IncreasableItemOption>();
        }
    }

    /// <summary>
    /// Gets the possible fenrir options.
    /// </summary>
    public IEnumerable<IncreasableItemOption> PossibleFenrirOptions
    {
        get
        {
            return this.Definition?.PossibleItemOptions
                       .Where(iod => iod.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.BlackFenrir || po.OptionType == ItemOptionTypes.BlueFenrir || po.OptionType == ItemOptionTypes.GoldFenrir))
                       .SelectMany(iod => iod.PossibleOptions)
                   ?? Enumerable.Empty<IncreasableItemOption>();
        }
    }

    /// <summary>
    /// Gets the possible harmony bonus options.
    /// </summary>
    public IEnumerable<IncreasableItemOption> PossibleHarmonyOptions
    {
        get
        {
            return this.Definition?.PossibleItemOptions
                       .Where(iod => iod.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.HarmonyOption))
                       .SelectMany(iod => iod.PossibleOptions)
                   ?? Enumerable.Empty<IncreasableItemOption>();
        }
    }

    /// <summary>
    /// Gets the possible guardian bonus options.
    /// </summary>
    public ItemOptionDefinition? PossibleGuardianOption
    {
        get
        {
            return this.Definition?.PossibleItemOptions
                .FirstOrDefault(iod => iod.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.GuardianOption));
        }
    }

    /// <summary>
    /// Gets the item options.
    /// </summary>
    public ICollection<ItemOptionLink> ItemOptions => this.Item.ItemOptions;

    /// <summary>
    /// Gets the item set groups.
    /// </summary>
    public ICollection<ItemOfItemSet> ItemSetGroups => this.Item.ItemSetGroups;

    /// <summary>
    /// Gets the fenrir option types.
    /// </summary>
    internal static ItemOptionType[] FenrirOptions { get; } = { ItemOptionTypes.BlackFenrir, ItemOptionTypes.BlueFenrir, ItemOptionTypes.GoldFenrir };

    /// <summary>
    /// Gets the socket bonus option link, if available.
    /// </summary>
    private ItemOptionLink? SocketBonusOptionLink => this.Item.ItemOptions.FirstOrDefault(io => io.ItemOption?.OptionType == ItemOptionTypes.SocketBonusOption);

    /// <summary>
    /// Gets the socket bonus option link, if available.
    /// </summary>
    private IEnumerable<ItemOptionLink> FenrirOptionLinks => this.Item.ItemOptions.Where(io => io.ItemOption?.OptionType == ItemOptionTypes.BlackFenrir || io.ItemOption?.OptionType == ItemOptionTypes.BlueFenrir || io.ItemOption?.OptionType == ItemOptionTypes.GoldFenrir);

    /// <summary>
    /// Gets the possible ancient set items.
    /// </summary>
    private IEnumerable<ItemOfItemSet> PossibleAncientSetItems
    {
        get
        {
            return this.Definition?.PossibleItemSetGroups
                .SelectMany(isg => isg.Items)
                .Where(isi => isi.ItemDefinition == this.Definition)
                .Where(isi => isi.AncientSetDiscriminator > 0)
                ?? Enumerable.Empty<ItemOfItemSet>();
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Called when the <see cref="Definition"/> has been changed.
    /// </summary>
    private void OnDefinitionChanged()
    {
        if (this.Definition is not { } definition)
        {
            return;
        }

        if (definition.MaximumItemLevel < this.Level)
        {
            this.Level = 0;
        }

        this.AncientSet = null;

        // Reassigning the durability will limit it to the maximum value.
        this.Durability = this.Item.GetMaximumDurabilityOfOnePiece();

        var possibleOptions = this.Definition.PossibleItemOptions.SelectMany(pio => pio.PossibleOptions).ToHashSet();
        var impossibleOptions = this.ItemOptions.Where(iol => iol.ItemOption is null || possibleOptions.Contains(iol.ItemOption)).ToList();
        foreach (var optionLink in impossibleOptions)
        {
            this.Item.ItemOptions.Remove(optionLink);
            this._persistenceContext.DeleteAsync(optionLink).AsTask().WaitAndUnwrapException();
        }

        this.SocketCount = Math.Min(this.SocketCount, this.Definition.MaximumSockets);
        this.HasSkill &= this.Item.CanHaveSkill();
    }

    private void OnSocketOptionChanged(object? sender, EventArgs e)
    {
        this.OnPropertyChanged(nameof(this.Sockets));
    }
}
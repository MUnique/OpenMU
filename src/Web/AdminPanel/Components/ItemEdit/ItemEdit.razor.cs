// <copyright file="ItemEdit.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.ItemEdit;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Specialized editor for an item.
/// </summary>
public sealed partial class ItemEdit : IDisposable
{
    private ViewModel? _viewModel;

    /// <summary>
    /// Gets or sets the item.
    /// </summary>
    [Parameter]
    [Required]
    public Item Item { get; set; } = null!;

    /// <summary>
    /// Gets or sets the persistence context.
    /// </summary>
    [CascadingParameter]
    public IContext PersistenceContext { get; set; } = null!;

    /// <summary>
    /// Gets or sets the <see cref="EditForm.OnValidSubmit"/> event callback.
    /// </summary>
    [Parameter]
    public EventCallback OnValidSubmit { get; set; }

    /// <summary>
    /// Gets or sets the task which should be executed when the cancel button gets clicked. If null, no cancel button is shown.
    /// </summary>
    [Parameter]
    public EventCallback OnCancel { get; set; }

    /// <summary>
    /// Gets or sets the event callback which is raised when the item has been changed.
    /// </summary>
    [Parameter]
    public EventCallback ItemChanged { get; set; }

    private Func<IncreasableItemOption, string> CaptionFactory => obj => $"{(obj.PowerUpDefinition ?? obj.LevelDependentOptions.FirstOrDefault()?.PowerUpDefinition)?.TargetAttribute} ({obj.Number})";

    /// <inheritdoc />
    public void Dispose()
    {
        if (this._viewModel is { } oldModel)
        {
            oldModel.PropertyChanged -= this.OnItemPropertyChanged;
        }
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (this._viewModel is { } oldModel)
        {
            oldModel.PropertyChanged -= this.OnItemPropertyChanged;
        }

        this._viewModel = new ViewModel(this.Item, this.PersistenceContext);
        this._viewModel.PropertyChanged += this.OnItemPropertyChanged;
        base.OnParametersSet();
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (this.ItemChanged.HasDelegate)
        {
            _ = this.ItemChanged.InvokeAsync();
        }
    }
}
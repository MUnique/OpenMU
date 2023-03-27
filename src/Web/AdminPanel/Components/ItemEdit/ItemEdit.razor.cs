// <copyright file="ItemEdit.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.ItemEdit;

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;

public partial class ItemEdit
{
    private ViewModel? _viewModel;

    private Func<IncreasableItemOption, string> CaptionFactory => obj => $"{obj.PowerUpDefinition?.TargetAttribute} ({obj.Number})";

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
    public Task? OnCancel { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        this._viewModel = new ViewModel(this.Item, this.PersistenceContext);
        base.OnParametersSet();
    }
}
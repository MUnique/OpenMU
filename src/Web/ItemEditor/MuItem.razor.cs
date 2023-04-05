// <copyright file="MuItem.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.ItemEditor;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

/// <summary>
/// Component for an <see cref="Item"/>.
/// </summary>
public partial class MuItem
{
    /// <summary>
    /// Gets or sets the item data.
    /// </summary>
    [Parameter]
    [Required]
    public ItemViewModel Model { get; set; } = null!;

    /// <summary>
    /// Gets or sets the on click callback.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Gets or sets the callback which is called when the selected item moved.
    /// </summary>
    [Parameter]
    public EventCallback OnItemMoved { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is selected.
    /// </summary>
    [Parameter]
    public bool IsSelected { get; set; }

    private int TotalRows => this.Model.Parent.Rows;
    private int Height => this.Model.Item.Definition?.Height ?? byte.MaxValue;
    private int Width => this.Model.Item.Definition?.Width ?? byte.MaxValue;

    private bool CanMoveDown => this.TotalRows > this.Model.Row + this.Height;
    private bool CanMoveUp => this.Model.Row > 0;
    private bool CanMoveLeft => this.Model.Column > 0;
    private bool CanMoveRight => this.Model.Column + this.Width < 8;

    private async Task OnKeyPressAsync(KeyboardEventArgs obj)
    {
        if (this.IsSelected)
        {
            return;
        }

        switch (obj.Key)
        {
            case "w" when this.CanMoveUp:
                await this.MoveUpAsync().ConfigureAwait(true);
                break;
            case "s" when this.CanMoveDown:
                await this.MoveDownAsync().ConfigureAwait(true);
                break;
            case "d" when this.CanMoveRight:
                await this.MoveRightAsync().ConfigureAwait(true);
                break;
            case "a" when this.CanMoveLeft:
                await this.MoveLeftAsync().ConfigureAwait(true);
                break;
        }
    }

    private async Task MoveLeftAsync()
    {
        this.Model.Item.ItemSlot--;
        await this.RaiseOnItemMovedAsync().ConfigureAwait(true);

    }

    private async Task MoveRightAsync()
    {
        this.Model.Item.ItemSlot++;
        await this.RaiseOnItemMovedAsync().ConfigureAwait(true);
    }

    private async Task MoveUpAsync()
    {
        this.Model.Item.ItemSlot -= 8;
        await this.RaiseOnItemMovedAsync().ConfigureAwait(true);
    }

    private async Task MoveDownAsync()
    {
        this.Model.Item.ItemSlot += 8;
        await this.RaiseOnItemMovedAsync().ConfigureAwait(true);
    }

    private async Task RaiseOnItemMovedAsync()
    {
        if (this.OnItemMoved.HasDelegate)
        {
            await this.OnItemMoved.InvokeAsync().ConfigureAwait(true);
        }
    }
}

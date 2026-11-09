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
    private const int BoxSize = 42;

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
    /// Gets or sets the callback which is called when an item starts being dragged.
    /// </summary>
    [Parameter]
    public EventCallback<(Item Item, byte ColumnOffset, byte RowOffset)> OnDragStarted { get; set; }

    /// <summary>
    /// Gets or sets the callback which is called when dragging ends.
    /// </summary>
    [Parameter]
    public EventCallback OnDragEnded { get; set; }

    /// <summary>
    /// Gets or sets the callback which is called when the item should be deleted.
    /// </summary>
    [Parameter]
    public EventCallback OnDelete { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is selected.
    /// </summary>
    [Parameter]
    public bool IsSelected { get; set; }

    private int TotalRows => this.Model.Parent?.Rows ?? 0;
    private int Height => this.Model.Item.Definition?.Height ?? 1;
    private int Width => this.Model.Item.Definition?.Width ?? 1;
    private bool CanMoveDown => this.TotalRows > this.Model.Row + this.Height;
    private bool CanMoveUp => this.Model.Row > 0;
    private bool CanMoveLeft => this.Model.Column > 0;
    private bool CanMoveRight => this.Model.Column + this.Width < InventoryConstants.RowSize;
    private bool CanJumpDown => this.Model.Parent?.StorageType is StorageType.Inventory or StorageType.InventoryExtension;
    private bool CanJumpUp => this.Model.Parent?.StorageType is StorageType.InventoryExtension or StorageType.PersonalStore;

    private async Task OnDragStartAsync(DragEventArgs args)
    {
        var columnOffset = (byte)Math.Clamp((int)(args.OffsetX / BoxSize), 0, this.Width - 1);
        var rowOffset = (byte)Math.Clamp((int)(args.OffsetY / BoxSize), 0, this.Height - 1);
        await this.OnDragStarted.InvokeAsync((this.Model.Item, columnOffset, rowOffset)).ConfigureAwait(false);
    }

    private Task OnDragEndAsync()
    {
        return this.OnDragEnded.InvokeAsync();
    }

    private async Task OnKeyPressAsync(KeyboardEventArgs obj)
    {
        if (!this.IsSelected)
        {
            return;
        }

        switch (obj.Key)
        {
            case "w" when this.CanMoveUp:
            case "ArrowUp" when this.CanMoveUp:
                await this.MoveUpAsync().ConfigureAwait(false);
                break;
            case "s" when this.CanMoveDown:
            case "ArrowDown" when this.CanMoveDown:
                await this.MoveDownAsync().ConfigureAwait(false);
                break;
            case "w" when this.CanJumpUp:
            case "ArrowUp" when this.CanJumpUp:
                await this.JumpUpAsync().ConfigureAwait(false);
                break;
            case "s" when this.CanJumpDown:
            case "ArrowDown" when this.CanJumpDown:
                await this.JumpDownAsync().ConfigureAwait(false);
                break;
            case "d" when this.CanMoveRight:
            case "ArrowRight" when this.CanMoveRight:
                await this.MoveRightAsync().ConfigureAwait(false);
                break;
            case "a" when this.CanMoveLeft:
            case "ArrowLeft" when this.CanMoveLeft:
                await this.MoveLeftAsync().ConfigureAwait(false);
                break;
            case "Delete":
                await this.OnDelete.InvokeAsync().ConfigureAwait(false);
                break;
            default:
                break;
        }
    }

    private async Task MoveLeftAsync()
    {
        this.Model.Item.MoveLeft();
        await this.RaiseOnItemMovedAsync().ConfigureAwait(false);
    }

    private async Task MoveRightAsync()
    {
        this.Model.Item.MoveRight();
        await this.RaiseOnItemMovedAsync().ConfigureAwait(false);
    }

    private async Task MoveUpAsync()
    {
        this.Model.Item.MoveUp();
        await this.RaiseOnItemMovedAsync().ConfigureAwait(false);
    }

    private async Task MoveDownAsync()
    {
        this.Model.Item.MoveDown();
        await this.RaiseOnItemMovedAsync().ConfigureAwait(false);
    }

    private async Task JumpUpAsync()
    {
        var jumpRows = (this.Model.Item.Definition?.Height ?? 1) + (this.Model.Parent?.EmptyRowsToPreviousStorage ?? 0);

        for (var i = 0; i < jumpRows; i++)
        {
            this.Model.Item.MoveUp();
        }

        await this.RaiseOnItemMovedAsync().ConfigureAwait(false);
    }

    private async Task JumpDownAsync()
    {
        var jumpRows = (this.Model.Item.Definition?.Height ?? 1) + (this.Model.Parent?.EmptyRowsToNextStorage ?? 0);

        for (var i = 0; i < jumpRows; i++)
        {
            this.Model.Item.MoveDown();
        }

        await this.RaiseOnItemMovedAsync().ConfigureAwait(false);
    }

    private async Task RaiseOnItemMovedAsync()
    {
        if (this.OnItemMoved.HasDelegate)
        {
            await this.OnItemMoved.InvokeAsync().ConfigureAwait(false);
        }
    }
}

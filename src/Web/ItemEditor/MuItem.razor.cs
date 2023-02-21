// <copyright file="MuItem.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.DataModel.Entities;

namespace MUnique.OpenMU.Web.ItemEditor;

public partial class MuItem
{
    /// <summary>
    /// Gets or sets the item data.
    /// </summary>
    [Parameter]
    [Required]
    public ItemViewModel Model { get; set; } = null!;
}
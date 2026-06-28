// <copyright file="ModalObjectSelection.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Form.Modal;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// A component which allows to select an instance of <typeparamref name="TItem"/> through the <see cref="ILookupController"/>.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
/// <seealso cref="ComponentBase" />
public partial class ModalObjectSelection<TItem>
    where TItem : class
{
    /// <summary>
    /// Gets or sets the selected item.
    /// </summary>
    [Required]
    public TItem? Item { get; set; }

    /// <summary>
    /// Gets or sets the modal instance.
    /// </summary>
    [Parameter]
    public ModalInstance Modal { get; set; } = null!;

    /// <summary>
    /// Gets or sets the persistence context which should be used. It's required for lookups.
    /// </summary>
    [Parameter]
    public IContext PersistenceContext { get; set; } = null!;

    private Task SubmitAsync()
    {
        return this.Modal.CloseAsync(ModalResult.Ok(this.Item));
    }

    private Task CancelAsync()
    {
        return this.Modal.CancelAsync();
    }
}

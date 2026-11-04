// <copyright file="ModalObjectMultiSelection.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Form.Modal;

using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// A component which allows to select multiple instances of <typeparamref name="TItem"/> through the <see cref="ILookupController"/>.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
/// <seealso cref="ComponentBase" />
public partial class ModalObjectMultiSelection<TItem>
    where TItem : class
{
    /// <summary>
    /// Gets or sets the selected items.
    /// </summary>
    public IList<TItem> Items { get; set; } = new List<TItem>();

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
        return this.Modal.CloseAsync(ModalResult.Ok(this.Items));
    }

    private Task CancelAsync()
    {
        return this.Modal.CancelAsync();
    }
}

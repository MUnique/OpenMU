// <copyright file="ModalObjectSelection.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Form;

using System.ComponentModel.DataAnnotations;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Services;

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
    [CascadingParameter]
    public BlazoredModalInstance BlazoredModal { get; set; } = null!;

    /// <summary>
    /// Gets or sets the persistence context which should be used. It's required for lookups.
    /// </summary>
    [Parameter]
    public IContext PersistenceContext { get; set; } = null!;

    private Task SubmitAsync()
    {
        return this.BlazoredModal.CloseAsync(ModalResult.Ok(this.Item));
    }

    private Task CancelAsync()
    {
        return this.BlazoredModal.CancelAsync();
    }
}
// <copyright file="ModalObjectSelection.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Components.Form
{
    using System.ComponentModel.DataAnnotations;
    using Blazored.Modal;
    using Blazored.Modal.Services;
    using Microsoft.AspNetCore.Components;
    using MUnique.OpenMU.AdminPanel.Services;

    /// <summary>
    /// A component which allows to select an instance of <typeparamref name="TItem"/> through the <see cref="ILookupController"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <seealso cref="Microsoft.AspNetCore.Components.ComponentBase" />
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

        private void Submit()
        {
            this.BlazoredModal.Close(ModalResult.Ok(this.Item));
        }

        private void Cancel()
        {
            this.BlazoredModal.Cancel();
        }
    }
}

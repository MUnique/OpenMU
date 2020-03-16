// <copyright file="ModalObjectSelection.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Components.Form
{
    using MUnique.OpenMU.AdminPanel.Services;

    /// <summary>
    /// A component which allows to select an instance of <see cref="TItem"/> through the <see cref="ILookupController"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <seealso cref="Microsoft.AspNetCore.Components.ComponentBase" />
    public partial class ModalObjectSelection<TItem>
        where TItem : class
    {
    }
}

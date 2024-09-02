// <copyright file="EditAccount.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Threading;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Components.Form;
using MUnique.OpenMU.Web.AdminPanel.Components.ItemEdit;

/// <summary>
/// The edit page for account data.
/// </summary>
[Route("/edit-account/{accountId:guid}/{typeString}/{id:guid}")]
public partial class EditAccount : EditBase
{
    /// <summary>
    /// Gets or sets the identifier of the account which should be edited.
    /// </summary>
    [Parameter]
    public Guid AccountId { get; set; }

    /// <summary>
    /// Gets or sets the data source for account data.
    /// </summary>
    [Inject]
    public IDataSource<Account> AccountData { get; set; } = null!;

    /// <inheritdoc />
    protected override IDataSource EditDataSource => this.AccountData;

    /// <inheritdoc />
    protected override async ValueTask LoadOwnerAsync(CancellationToken cancellationToken)
    {
        await this.AccountData.GetOwnerAsync(this.AccountId, cancellationToken).ConfigureAwait(true);
    }

    /// <inheritdoc />
    protected override void AddFormToRenderTree(RenderTreeBuilder builder, ref int currentSequence)
    {
        if (this.Type == typeof(Item))
        {
            builder.OpenComponent(++currentSequence, typeof(ItemEdit));
            builder.AddAttribute(++currentSequence, nameof(ItemEdit.Item), this.Model);
            builder.AddAttribute(++currentSequence, nameof(ItemEdit.OnValidSubmit), EventCallback.Factory.Create(this, this.SaveChangesAsync));
            builder.CloseComponent();
        }
        else
        {
            // TODO: Instead of AutoForm, create more specialized components
            builder.OpenComponent(++currentSequence, typeof(AutoForm<>).MakeGenericType(this.Type!));
            builder.AddAttribute(++currentSequence, nameof(AutoForm<object>.Model), this.Model);
            builder.AddAttribute(++currentSequence, nameof(AutoForm<object>.OnValidSubmit), EventCallback.Factory.Create(this, this.SaveChangesAsync));
            builder.CloseComponent();
        }
    }
}

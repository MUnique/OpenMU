// <copyright file="EditAccount.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Threading;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.Shared.Components.Form;

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

    /// <summary>
    /// Gets the closed <see cref="AutoForm{T}"/> type for the current <see cref="EditBase.Type"/>.
    /// </summary>
    protected Type? AutoFormType => this.Type is null ? null : typeof(AutoForm<>).MakeGenericType(this.Type);

    /// <summary>
    /// Gets the parameters for the <see cref="AutoForm{T}"/> component.
    /// </summary>
    protected Dictionary<string, object?> AutoFormParameters => new()
    {
        [nameof(AutoForm<object>.Model)] = this.Model,
        [nameof(AutoForm<object>.OnValidSubmit)] = EventCallback.Factory.Create(this, this.SaveChangesAsync),
        [nameof(AutoForm<object>.OnRefresh)] = EventCallback.Factory.Create(this, this.RefreshAsync),
    };

    /// <inheritdoc />
    protected override async ValueTask LoadOwnerAsync(CancellationToken cancellationToken)
    {
        await this.AccountData.GetOwnerAsync(this.AccountId, cancellationToken).ConfigureAwait(true);
    }
}

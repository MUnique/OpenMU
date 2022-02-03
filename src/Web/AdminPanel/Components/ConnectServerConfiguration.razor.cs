// <copyright file="ConnectServerConfiguration.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Web.AdminPanel.Models;

/// <summary>
/// Edit component for the <see cref="ConnectServerDefinition"/>.
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Components.ComponentBase" />
public partial class ConnectServerConfiguration
{
    /// <summary>
    /// Gets or sets the <see cref="EditForm.OnValidSubmit"/> event callback.
    /// </summary>
    [Parameter]
    public EventCallback OnValidSubmit { get; set; }

    /// <summary>
    /// Gets or sets the task which should be executed when the cancel button gets clicked. If null, no cancel button is shown.
    /// </summary>
    [Parameter]
    public Task? OnCancel { get; set; }

    /// <summary>
    /// Gets or sets the model.
    /// </summary>
    [Parameter]
    public ConnectServerDefinition Model { get; set; } = null!;

    private ConnectServerConfigurationViewItem? Configuration { get; set; }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        this.Configuration = new ConnectServerConfigurationViewItem(this.Model);
    }
}
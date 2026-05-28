// <copyright file="Plugins.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Code-behind for the <see cref="Plugins"/> page.
/// </summary>
public partial class Plugins
{
    /// <summary>
    /// Gets or sets the plug-in identifier.
    /// </summary>
    [SupplyParameterFromQuery(Name = "id")]
    public string? PlugInId { get; set; }

    [Inject]
    private PlugInController PlugInController { get; set; } = null!;

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync().ConfigureAwait(true);

        if (Guid.TryParse(this.PlugInId, out var id))
        {
            var plugin = await this.PlugInController.GetByIdAsync(id).ConfigureAwait(true);

            if (plugin is { })
            {
                this.PlugInController.NameFilter = plugin.PlugInName ?? string.Empty;
                this.PlugInController.TypeFilter = string.Empty;
                this.PlugInController.PointFilter = Guid.Empty;

                if (plugin.ConfigurationType is { })
                {
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(100).ConfigureAwait(false);
                        await this.InvokeAsync(() => this.PlugInController.ShowPlugInConfigAsync(plugin)).ConfigureAwait(false);
                    });
                }
            }

            this.PlugInId = null;
        }
    }

    private void OnPlugInPointSelected(ChangeEventArgs args)
    {
        if (args.Value is string guidString && Guid.TryParse(guidString, out var result))
        {
            this.PlugInController.PointFilter = result;
        }
    }
}

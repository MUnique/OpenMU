// <copyright file="Setup.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using MUnique.OpenMU.Web.AdminPanel.Services;

public partial class Setup
{
    public bool ShowInstall { get; set; }

    [Inject] public SetupService SetupService { get; set; } = null!;

    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;

    public Task OnUpdateClickAsync()
    {
        return this.SetupService.InstallUpdatesAsync(default);
    }

    private void OnInstallClick()
    {
        this.ShowInstall = true;
    }

    private async Task OnReInstallClickAsync()
    {
        if (await this.JsRuntime.InvokeAsync<bool>("confirm", "Are you sure? All the current data is getting deleted and freshly installed."))
        {
            this.ShowInstall = true;
        }
    }

}
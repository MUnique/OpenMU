using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MUnique.OpenMU.Setup.Data;

namespace MUnique.OpenMU.Setup.Pages;

public partial class Index
{
    public bool ShowInstall { get; set; }

    [Inject]
    public SetupService SetupService { get; set; }

    [Inject]
    public IJSRuntime JsRuntime { get; set; }

    public Task OnUpdateClickAsync()
    {
        return this.SetupService.InstallUpdatesAsync(default);
    }

    private void OnInstallClick()
    {
        this.ShowInstall = true;
    }

    private async Task OnReInstallClick()
    {
        if (await this.JsRuntime.InvokeAsync<bool>("confirm", "Are you sure? All the current data is getting deleted and freshly installed."))
        {
            this.ShowInstall = true;
        }
    }

}
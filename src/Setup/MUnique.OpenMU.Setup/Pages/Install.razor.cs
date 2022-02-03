using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MUnique.OpenMU.Persistence.Initialization;
using MUnique.OpenMU.Setup.Data;

namespace MUnique.OpenMU.Setup.Pages
{
    public partial class Install
    {
        public IDataInitializationPlugIn? SelectedVersion { get; set; }

        public int GameServerCount { get; set; } = 2;

        public bool CreateTestAccounts { get; set; }

        public bool IsInstalling { get; private set; }

        public bool IsInstalled { get; private set; }

        [Parameter]
        public EventCallback InstallationFinished { get; set; }


        [Inject]
        public SetupService SetupService { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            this.SelectedVersion = this.SetupService.Versions.First();
        }
         
        private void OnSelectVersion(string key)
        {
            this.SelectedVersion = this.SetupService.Versions.First(v => v.Key == key);
        }

        public async Task StartInstallation()
        {
            var task = Task.Run(() =>
            {
                this.SetupService.CreateDatabase();
                this.SelectedVersion!.CreateInitialData((byte)this.GameServerCount, this.CreateTestAccounts);
            });

            this.IsInstalling = true;
            this.StateHasChanged();
            try
            {
                await task;
            }
            finally
            {
                this.IsInstalled = true;
                this.IsInstalling = false;
                await this.InstallationFinished.InvokeAsync();
            }
        }

        private void OnGameServerCountChange(ChangeEventArgs obj)
        {
            this.GameServerCount = int.Parse((string)obj.Value);
        }
        private void OnTestAccountsChange(ChangeEventArgs obj)
        {
            this.CreateTestAccounts = (bool)obj.Value;
        }

    }
}

using MUnique.OpenMU.Persistence.Initialization;
using MUnique.OpenMU.PlugIns;

namespace MUnique.OpenMU.Setup.Data
{
    using MUnique.OpenMU.Persistence.EntityFramework;

    public class SetupService
    {
        private readonly PersistenceContextProvider _contextProvider;

        private readonly PlugInManager _plugInManager;

        private ICollection<IDataInitializationPlugIn>? _availableInitializationPlugIns;

        public SetupService(PersistenceContextProvider contextProvider, PlugInManager plugInManager)
        {
            this._contextProvider = contextProvider;
            this._plugInManager = plugInManager;
        }

        public bool CanConnectToDatabase => this._contextProvider.CanConnectToDatabase();

        public bool IsInstalled => this._contextProvider.DatabaseExists();

        public bool IsUpdateRequired => !this._contextProvider.IsDatabaseUpToDate();

        public ICollection<IDataInitializationPlugIn> Versions => _availableInitializationPlugIns
            ??= this._plugInManager.GetStrategyProvider<string, IDataInitializationPlugIn>()!
                .AvailableStrategies
                .OrderByDescending(s => s.Caption)
                .ToList();

        public Task InstallUpdatesAsync(CancellationToken cancellationToken)
        {
            this._contextProvider.ApplyAllPendingUpdates();
            return this._contextProvider.WaitForUpdatedDatabase(cancellationToken);
        }

        public void CreateDatabase()
        {
            this._contextProvider.ReCreateDatabase();
        }
    }
}
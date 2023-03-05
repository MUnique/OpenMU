// <copyright file="DataInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence.Initialization.Version075.TestAccounts;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Data initialization plugin for Version 0.95d.
/// </summary>
[Guid("D24791DC-5808-42A9-AFEA-7C398C0D8C84")]
[PlugIn("Version 0.95d Initialization", "Provides initial data for Version 0.95d")]
public class DataInitialization : DataInitializationBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataInitialization" /> class.
    /// </summary>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public DataInitialization(IPersistenceContextProvider persistenceContextProvider, ILoggerFactory loggerFactory)
        : base(persistenceContextProvider, loggerFactory)
    {
    }

    /// <summary>
    /// Gets the identifier, by which the initialization is selected.
    /// </summary>
    public static string Id => "0.95d";

    /// <inheritdoc />
    public override string Caption => "0.95d";

    /// <inheritdoc />
    public override string Key => Id;

    /// <inheritdoc />
    protected override IInitializer GameConfigurationInitializer => new GameConfigurationInitializer(this.Context, this.GameConfiguration);

    /// <inheritdoc />
    protected override IGameMapsInitializer GameMapsInitializer => new GameMapsInitializer(this.Context, this.GameConfiguration);

    /// <inheritdoc />
    protected override IInitializer? TestAccountsInitializer => new TestAccountsInitialization(this.Context, this.GameConfiguration);

    /// <inheritdoc />
    protected override void CreateGameClientDefinition()
    {
        var version095Definition = this.Context.CreateNew<GameClientDefinition>();
        version095Definition.SetGuid(95);
        version095Definition.Season = 0;
        version095Definition.Episode = 95;
        version095Definition.Language = ClientLanguage.English;
        version095Definition.Version = new byte[] { 0x30, 0x39, 0x35, 0x30, 0x34 };
        version095Definition.Serial = Encoding.ASCII.GetBytes("4zYGWgYggf9ZENHc");
        version095Definition.Description = "Version 0.95d Client";
    }
}
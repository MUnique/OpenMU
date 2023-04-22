// <copyright file="DataInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence.Initialization.Version075.TestAccounts;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Data initialization plugin for Version 0.75.
/// </summary>
[Guid("420F8E50-0ACB-4A90-AF86-E1035D97F84D")]
[PlugIn("Version 0.75 Initialization", "Provides initial data for Version 0.75")]
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
    public static string Id => "0.75";

    /// <inheritdoc />
    public override string Key => Id;

    /// <inheritdoc />
    public override string Caption => "0.75";

    /// <inheritdoc />
    protected override IInitializer GameConfigurationInitializer => new GameConfigurationInitializer(this.Context, this.GameConfiguration);

    /// <inheritdoc />
    protected override IGameMapsInitializer GameMapsInitializer => new GameMapsInitializer(this.Context, this.GameConfiguration);

    /// <inheritdoc />
    protected override IInitializer? TestAccountsInitializer => new TestAccountsInitialization(this.Context, this.GameConfiguration);

    /// <inheritdoc />
    protected override void CreateGameClientDefinition()
    {
        var version075Definition = this.Context.CreateNew<GameClientDefinition>();
        version075Definition.SetGuid(75);
        version075Definition.Season = 0;
        version075Definition.Episode = 75;
        version075Definition.Language = ClientLanguage.Invariant; // it doesn't fit into any available category - maybe it's so old that it didn't have differences in the protocol yet.
        version075Definition.Version = new byte[] { 0x30, 0x37, 0x35, 0x30, 0x30 }; // the last two bytes are not relevant as te 0.75 does only use the first 3 bytes.
        version075Definition.Serial = Encoding.ASCII.GetBytes("sudv(*40ds7lkN2n");
        version075Definition.Description = "Version 0.75 Client";
    }
}
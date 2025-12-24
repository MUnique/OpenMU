// <copyright file="DataInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097d;

using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence.Initialization.Version075.TestAccounts;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Data initialization plugin for Version 0.97.
/// </summary>
[Guid("F5AAE1B9-9E73-4D6B-A756-DFA28DE2A130")]
[PlugIn("Version 0.97 Initialization", "Provides initial data for Version 0.97")]
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
    public static string Id => "0.97";

    /// <inheritdoc />
    public override string Caption => "0.97";

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
        var version097Definition = this.Context.CreateNew<GameClientDefinition>();
        version097Definition.SetGuid(97);
        version097Definition.Season = 0;
        version097Definition.Episode = 97;
        version097Definition.Language = ClientLanguage.English;
        version097Definition.Version = new byte[] { 0x30, 0x39, 0x37, 0x31, 0x31 };
        version097Definition.Serial = Encoding.ASCII.GetBytes("TbYehR2hFUPBKgZj");
        version097Definition.Description = "Version 0.97 Client";
    }
}

// <copyright file="DataInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.TestAccounts;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Provides initial data for Season 6 Episode 3.
/// </summary>
[Guid("9C21C359-F192-4AF5-8C05-1AC4AFD10897")]
[PlugIn("Season 6 Episode 3 Initialization", "Provides initial data for Season 6 Episode 3.")]
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
    public static string Id => "season6";

    /// <inheritdoc />
    public override string Key => Id;

    /// <inheritdoc />
    public override string Caption => "1.04d - Season 6 Episode 3";

    /// <inheritdoc/>
    protected override IInitializer TestAccountsInitializer => new TestAccountsInitialization(this.Context, this.GameConfiguration);

    /// <inheritdoc/>
    protected override IInitializer GameConfigurationInitializer => new GameConfigurationInitializer(this.Context, this.GameConfiguration);

    /// <inheritdoc/>
    protected override IGameMapsInitializer GameMapsInitializer => new GameMapsInitializer(this.Context, this.GameConfiguration);

    /// <inheritdoc />
    protected override void CreateGameClientDefinition()
    {
        var season6GmoClient = this.Context.CreateNew<GameClientDefinition>();
        season6GmoClient.SetGuid(0x104D);
        season6GmoClient.Season = 6;
        season6GmoClient.Episode = 3;
        season6GmoClient.Language = ClientLanguage.English;
        season6GmoClient.Version = [0x31, 0x30, 0x34, 0x30, 0x34];
        season6GmoClient.Serial = "k1Pk2jcET48mxL3b"u8.ToArray();
        season6GmoClient.Description = "Season 6 Episode 3 GMO Client";

        var season6OpenSource = this.Context.CreateNew<GameClientDefinition>();
        season6OpenSource.SetGuid(0x204D);
        season6OpenSource.Season = 106;
        season6OpenSource.Episode = 3;
        season6OpenSource.Language = ClientLanguage.English;
        season6OpenSource.Version = [0x32, 0x30, 0x34, 0x30, 0x34];
        season6OpenSource.Serial = "k1Pk2jcET48mxL3b"u8.ToArray();
        season6OpenSource.Description = "Season 6 Episode 3 Open Source Client";
    }
}
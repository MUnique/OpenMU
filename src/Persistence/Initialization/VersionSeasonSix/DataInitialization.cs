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
        var clientDefinition = this.Context.CreateNew<GameClientDefinition>();
        clientDefinition.SetGuid(0x104D);
        clientDefinition.Season = 6;
        clientDefinition.Episode = 3;
        clientDefinition.Language = ClientLanguage.English;
        clientDefinition.Version = new byte[] { 0x31, 0x30, 0x34, 0x30, 0x34 };
        clientDefinition.Serial = Encoding.ASCII.GetBytes("k1Pk2jcET48mxL3b");
        clientDefinition.Description = "Season 6 Episode 3 GMO Client";
    }
}
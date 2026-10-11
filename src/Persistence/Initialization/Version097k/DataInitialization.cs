// <copyright file="DataInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097k;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence.Initialization.Version075.TestAccounts;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Data initialization plugin for Version 0.97k.
/// </summary>
/// <remarks>
/// The content is the one of <see cref="Version095d"/>, extended by the second character classes,
/// the second wings and Blood Castle.
/// </remarks>
[Guid("E0F806BF-0A60-4264-811C-CB22C3856D16")]
[PlugIn]
[Display(Name = nameof(PlugInResources.DataInitialization097k_Name), Description = nameof(PlugInResources.DataInitialization097k_Description), ResourceType = typeof(PlugInResources))]
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
    public static string Id => "0.97k";

    /// <inheritdoc />
    public override string Caption => "0.97k";

    /// <inheritdoc />
    public override string Key => Id;

    /// <inheritdoc />
    protected override IInitializer GameConfigurationInitializer => new GameConfigurationInitializer(this.Context, this.GameConfiguration);

    /// <inheritdoc />
    protected override IGameMapsInitializer GameMapsInitializer => new GameMapsInitializer(this.Context, this.GameConfiguration);

    /// <inheritdoc />
    protected override IInitializer? TestAccountsInitializer => new TestAccountsInitialization(this.Context, this.GameConfiguration);

    /// <inheritdoc />
    /// <remarks>
    /// The client of version 0.97k (0.97.11) is a korean client. Its version is sent as "09711".
    /// It uses the same packet codes as the other clients before season 1, e.g. 0x15 for a hit,
    /// so it's defined as <see cref="ClientLanguage.Korean"/> to not get the packet handlers
    /// which are defined for english clients of version 0.97 and higher.
    /// </remarks>
    protected override void CreateGameClientDefinition()
    {
        var version097Definition = this.Context.CreateNew<GameClientDefinition>();
        version097Definition.SetGuid(97);
        version097Definition.Season = 0;
        version097Definition.Episode = 97;
        version097Definition.Language = ClientLanguage.Korean;
        version097Definition.Version = new byte[] { 0x30, 0x39, 0x37, 0x31, 0x31 };

        // The serial is the one of the reference client which is publicly available. It can be changed in the admin panel.
        version097Definition.Serial = Encoding.ASCII.GetBytes("TbYehR2hFUPBKgZj");
        version097Definition.Description = "Version 0.97k Client";
    }
}

// <copyright file="CaptionsPageTestInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Pages;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Properties;

/// <summary>
/// A minimal data initialization for the <see cref="CaptionsPageTests"/>, which creates
/// a configuration with one monster whose caption is linked to a source.
/// </summary>
[Guid("2B0C5E43-7A37-4D35-9A0B-29C8A1F5E8D4")]
internal sealed class CaptionsPageTestInitialization : IDataInitializationPlugIn
{
    /// <summary>
    /// The key of this initialization.
    /// </summary>
    public const string Id = "captions-page-test";

    /// <summary>
    /// The identifier of the monster.
    /// </summary>
    public static readonly Guid MonsterId = new("6E2D2F0A-91C4-4C3B-8F55-0D7E1C3A4B21");

    private readonly IPersistenceContextProvider _persistenceContextProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CaptionsPageTestInitialization"/> class.
    /// </summary>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public CaptionsPageTestInitialization(IPersistenceContextProvider persistenceContextProvider, ILoggerFactory loggerFactory)
    {
        this._persistenceContextProvider = persistenceContextProvider;
    }

    /// <inheritdoc />
    public string Key => Id;

    /// <inheritdoc />
    public string Caption => "Captions page test";

    /// <inheritdoc />
    public async Task CreateInitialDataAsync(byte numberOfGameServers, bool createTestAccounts)
    {
        using var context = this._persistenceContextProvider.CreateNewContext();
        var configuration = context.CreateNew<GameConfiguration>();
        var monster = context.CreateNew<MonsterDefinition>();
        ((IIdentifiable)monster).Id = MonsterId;
        monster.Designation = Resources.ResourceManager.GetLocalizedString(nameof(Resources.Captions));
        configuration.Monsters.Add(monster);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }
}

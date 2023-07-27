// <copyright file="ChatServerContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup;

using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// A container which takes care of the <see cref="Interfaces.IChatServer"/>.
/// It initializes and restarts it, as soon as the database is reinitialized.
/// </summary>
public class ChatServerContainer : ServerContainerBase
{
    private readonly ChatServer.ChatServer _chatServer;
    private readonly IPersistenceContextProvider _persistenceContextProvider;
    private readonly ILogger<ChatServerContainer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatServerContainer"/> class.
    /// </summary>
    /// <param name="chatServer">The chat server.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="setupService">The setup service.</param>
    /// <param name="logger">The logger.</param>
    public ChatServerContainer(ChatServer.ChatServer chatServer, IPersistenceContextProvider persistenceContextProvider, SetupService setupService, ILogger<ChatServerContainer> logger)
        : base(setupService, logger)
    {
        this._chatServer = chatServer;
        this._persistenceContextProvider = persistenceContextProvider;
        this._logger = logger;
    }

    /// <inheritdoc />
    protected override async Task StartInnerAsync(CancellationToken cancellationToken)
    {
        var definitions = await this._persistenceContextProvider.CreateNewConfigurationContext().GetAsync<ChatServerDefinition>().ConfigureAwait(false);
        if (definitions.FirstOrDefault() is { } definition)
        {
            definition.ConvertToSettings();
            this._chatServer.Initialize(definition.ConvertToSettings());
            await this._chatServer.StartAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            this._logger.LogWarning("No chat server configuration found.");
        }
    }

    /// <inheritdoc />
    protected override async Task StopInnerAsync(CancellationToken cancellationToken)
    {
        await this._chatServer.StopAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task StartListenersAsync(CancellationToken cancellationToken)
    {
        // listeners are always started...
        await this._chatServer.StartAsync(cancellationToken);
    }
}
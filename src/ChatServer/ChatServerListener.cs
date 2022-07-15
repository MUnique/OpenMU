// <copyright file="ChatServerListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer;

using System.ComponentModel;
using System.IO.Pipelines;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A listener which listens to the specified endpoint and provides the initialized <see cref="IConnection"/> by the event <see cref="ClientAccepted"/>.
/// </summary>
public class ChatServerListener
{
    private readonly ChatServerEndpoint _endpoint;
    private readonly PlugInManager _plugInManager;
    private readonly ILoggerFactory _loggerFactory;
    private Listener? _chatClientListener;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatServerListener" /> class.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="plugInManager">The plug in manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public ChatServerListener(ChatServerEndpoint endpoint, PlugInManager plugInManager, ILoggerFactory loggerFactory)
    {
        this._endpoint = endpoint;
        this._plugInManager = plugInManager;
        this._loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Occurs when a new client was accepted.
    /// </summary>
    public event AsyncEventHandler<ClientAcceptedEventArgs>? ClientAccepted;

    /// <summary>
    /// Occurs when a client has been accepted by the tcp listener, but before a <see cref="Connection"/> is created.
    /// </summary>
    public event AsyncEventHandler<CancelEventArgs>? ClientAccepting;

    /// <summary>
    /// Starts the tcp listener of this instance.
    /// </summary>
    public void Start()
    {
        this._chatClientListener = new Listener(this._endpoint.NetworkPort, this.CreateDecryptor, _ => null, this._loggerFactory);
        this._chatClientListener.ClientAccepted += async args => await this.ClientAccepted.SafeInvokeAsync(args).ConfigureAwait(false);
        this._chatClientListener.ClientAccepting += async args => await this.ClientAccepting.SafeInvokeAsync(args).ConfigureAwait(false);
        this._chatClientListener.Start();
    }

    /// <summary>
    /// Stops this instance.
    /// </summary>
    public void Stop()
    {
        this._chatClientListener?.Stop();
    }

    private IPipelinedDecryptor? CreateDecryptor(PipeReader pipeReader)
    {
        var encryptionFactoryPlugIn = this._plugInManager.GetStrategy<ClientVersion, INetworkEncryptionFactoryPlugIn>(this._endpoint.ClientVersion)
                                      ?? this._plugInManager.GetStrategy<ClientVersion, INetworkEncryptionFactoryPlugIn>(default);
        return encryptionFactoryPlugIn?.CreateDecryptor(pipeReader, DataDirection.ClientToServer);
    }
}
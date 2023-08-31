// <copyright file="Listener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Pipelines.Sockets.Unofficial;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A tcp listener which automatically creates instances of <see cref="Connection"/>s for accepted clients.
/// </summary>
public class Listener
{
    private readonly ILogger _logger;
    private readonly int _port;
    private readonly Func<PipeReader, IPipelinedDecryptor?>? _decryptorCreator;
    private readonly Func<PipeWriter, IPipelinedEncryptor?>? _encryptorCreator;
    private readonly ILoggerFactory _loggerFactory;
    private TcpListener? _clientListener;

    /// <summary>
    /// Initializes a new instance of the <see cref="Listener" /> class.
    /// </summary>
    /// <param name="port">The port on which the tcp listener should listen to.</param>
    /// <param name="decryptorCreator">The decryptor creator function.</param>
    /// <param name="encryptorCreator">The encryptor creator function.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public Listener(int port, Func<PipeReader, IPipelinedDecryptor?>? decryptorCreator, Func<PipeWriter, IPipelinedEncryptor?>? encryptorCreator, ILoggerFactory loggerFactory)
    {
        this._port = port;
        this._decryptorCreator = decryptorCreator;
        this._encryptorCreator = encryptorCreator;
        this._loggerFactory = loggerFactory;

        this._logger = this._loggerFactory.CreateLogger<Listener>();
    }

    /// <summary>
    /// Occurs when a client has been accepted by the tcp listener.
    /// </summary>
    public event AsyncEventHandler<ClientAcceptedEventArgs>? ClientAccepted;

    /// <summary>
    /// Occurs when a client has been accepted by the tcp listener, but before a <see cref="Connection"/> is created.
    /// </summary>
    public event AsyncEventHandler<ClientAcceptingEventArgs>? ClientAccepting;

    /// <summary>
    /// Gets a value indicating whether this listener is bound to a specific local port.
    /// </summary>
    public bool IsBound => this._clientListener?.Server.IsBound ?? false;

    /// <summary>
    /// Starts the tcp listener and begins to accept connections.
    /// </summary>
    /// <param name="backlog">The maximum length of the pending connections queue.</param>
    public void Start(int backlog = (int)SocketOptionName.MaxConnections)
    {
        this._clientListener = new TcpListener(IPAddress.Any, this._port);
        this._clientListener.Start(backlog);
        this._clientListener.BeginAcceptSocket(this.OnAccept, null);
    }

    /// <summary>
    /// Stops the tcp listener.
    /// </summary>
    public void Stop()
    {
        this._clientListener?.Stop();
    }

    /// <summary>
    /// Creates the decryptor for the specified reader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>The created decryptor.</returns>
    protected virtual IPipelinedDecryptor? CreateDecryptor(PipeReader reader)
    {
        return this._decryptorCreator?.Invoke(reader);
    }

    /// <summary>
    /// Creates the encryptor for the specified writer.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <returns>The created encryptor.</returns>
    protected virtual IPipelinedEncryptor? CreateEncryptor(PipeWriter writer)
    {
        return this._encryptorCreator?.Invoke(writer);
    }

    private IConnection CreateConnection(Socket clientSocket)
    {
        var socketConnection = SocketConnection.Create(clientSocket);
        return new Connection(socketConnection, this.CreateDecryptor(socketConnection.Input), this.CreateEncryptor(socketConnection.Output), this._loggerFactory.CreateLogger<Connection>());
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Exceptions are catched.")]
    private async void OnAccept(IAsyncResult result)
    {
        try
        {
            Socket socket;
            try
            {
                if (this._clientListener is null)
                {
                    return;
                }

                socket = this._clientListener.EndAcceptSocket(result);
            }
            catch (ObjectDisposedException)
            {
                // this exception is expected when the clientListener got disposed. In this case we don't want to spam the log.
                return;
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
            {
                this._logger.LogDebug(ex, "The listener was stopped.");
                return;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error accepting the client socket");
                return;
            }

            // Accept the next client:
            if (this._clientListener?.Server.IsBound ?? false)
            {
                // todo: refactor to use AcceptSocketAsync
                this._clientListener.BeginAcceptSocket(this.OnAccept, null);
            }

            ClientAcceptingEventArgs? cancel = null;
            if (this.ClientAccepting is { } clientAccepting)
            {
                cancel = new ClientAcceptingEventArgs(socket);
                await clientAccepting.Invoke(cancel).ConfigureAwait(false);
            }

            if (cancel is null || !cancel.Cancel)
            {
                socket.NoDelay = true; // todo: option?
                var connection = this.CreateConnection(socket);

                if (this.ClientAccepted is { } clientAccepted)
                {
                    await clientAccepted.Invoke(new ClientAcceptedEventArgs(connection)).ConfigureAwait(false);
                }
            }
            else
            {
                socket.Dispose();
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error in OnAccept.");
        }
    }
}
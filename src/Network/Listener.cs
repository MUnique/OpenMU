// <copyright file="Listener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
    using System.IO.Pipelines;
    using System.Net;
    using System.Net.Sockets;
    using Microsoft.Extensions.Logging;
    using Pipelines.Sockets.Unofficial;

    /// <summary>
    /// A tcp listener which automatically creates instances of <see cref="Connection"/>s for accepted clients.
    /// </summary>
    public class Listener
    {
        private readonly ILogger logger;
        private readonly int port;
        private readonly Func<PipeReader, IPipelinedDecryptor?>? decryptorCreator;
        private readonly Func<PipeWriter, IPipelinedEncryptor?>? encryptorCreator;
        private readonly ILoggerFactory loggerFactory;
        private TcpListener? clientListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="Listener" /> class.
        /// </summary>
        /// <param name="port">The port on which the tcp listener should listen to.</param>
        /// <param name="decryptorCreator">The decryptor creator function.</param>
        /// <param name="encryptorCreator">The encryptor creator function.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public Listener(int port, Func<PipeReader, IPipelinedDecryptor?>? decryptorCreator, Func<PipeWriter, IPipelinedEncryptor?>? encryptorCreator, ILoggerFactory loggerFactory)
        {
            this.port = port;
            this.decryptorCreator = decryptorCreator;
            this.encryptorCreator = encryptorCreator;
            this.loggerFactory = loggerFactory;

            this.logger = this.loggerFactory.CreateLogger<Listener>();
        }

        /// <summary>
        /// Occurs when a client has been accepted by the tcp listener.
        /// </summary>
        public event EventHandler<ClientAcceptedEventArgs>? ClientAccepted;

        /// <summary>
        /// Occurs when a client has been accepted by the tcp listener, but before a <see cref="Connection"/> is created.
        /// </summary>
        public event EventHandler<ClientAcceptingEventArgs>? ClientAccepting;

        /// <summary>
        /// Gets a value indicating whether this listener is bound to a specific local port.
        /// </summary>
        public bool IsBound => this.clientListener?.Server.IsBound ?? false;

        /// <summary>
        /// Starts the tcp listener and begins to accept connections.
        /// </summary>
        /// <param name="backlog">The maximum length of the pending connections queue.</param>
        public void Start(int backlog = (int)SocketOptionName.MaxConnections)
        {
            this.clientListener = new TcpListener(IPAddress.Any, this.port);
            this.clientListener.Start(backlog);
            this.clientListener.BeginAcceptSocket(this.OnAccept, null);
        }

        /// <summary>
        /// Stops the tcp listener.
        /// </summary>
        public void Stop()
        {
            this.clientListener?.Stop();
        }

        /// <summary>
        /// Creates the decryptor for the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The created decryptor.</returns>
        protected virtual IPipelinedDecryptor? CreateDecryptor(PipeReader reader)
        {
            return this.decryptorCreator?.Invoke(reader);
        }

        /// <summary>
        /// Creates the encryptor for the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <returns>The created encryptor.</returns>
        protected virtual IPipelinedEncryptor? CreateEncryptor(PipeWriter writer)
        {
            return this.encryptorCreator?.Invoke(writer);
        }

        private IConnection CreateConnection(Socket clientSocket)
        {
            var socketConnection = SocketConnection.Create(clientSocket);
            return new Connection(socketConnection, this.CreateDecryptor(socketConnection.Input), this.CreateEncryptor(socketConnection.Output), this.loggerFactory.CreateLogger<Connection>());
        }

        private void OnAccept(IAsyncResult result)
        {
            Socket socket;
            try
            {
                if (this.clientListener is null)
                {
                    return;
                }

                socket = this.clientListener.EndAcceptSocket(result);
            }
            catch (ObjectDisposedException)
            {
                // this exception is expected when the clientListener got disposed. In this case we don't want to spam the log.
                return;
            }
            catch (SocketException ex) when (ex.ErrorCode == (int)SocketError.OperationAborted)
            {
                this.logger.LogDebug(ex, "The listener was stopped.");
                return;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error accepting the client socket");
                return;
            }

            // Accept the next client:
            if (this.clientListener?.Server.IsBound ?? false)
            {
                this.clientListener.BeginAcceptSocket(this.OnAccept, null);
            }

            ClientAcceptingEventArgs? cancel = null;
            this.ClientAccepting?.Invoke(this, cancel = new ClientAcceptingEventArgs(socket));
            if (cancel is null || !cancel.Cancel)
            {
                socket.NoDelay = true; // todo: option?
                var connection = this.CreateConnection(socket);
                this.ClientAccepted?.Invoke(this, new ClientAcceptedEventArgs(connection));
            }
            else
            {
                socket.Dispose();
            }
        }
    }
}

// <copyright file="Listener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
    using System.ComponentModel;
    using System.IO.Pipelines;
    using System.Net;
    using System.Net.Sockets;
    using log4net;
    using Pipelines.Sockets.Unofficial;

    /// <summary>
    /// A tcp listener which automatically creates instances of <see cref="Connection"/>s for accepted clients.
    /// </summary>
    public class Listener
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Listener));
        private readonly int port;
        private readonly Func<PipeReader, IPipelinedDecryptor> decryptorCreator;
        private readonly Func<PipeWriter, IPipelinedEncryptor> encryptorCreator;
        private TcpListener clientListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="Listener" /> class.
        /// </summary>
        /// <param name="port">The port on which the tcp listener should listen to.</param>
        /// <param name="decryptorCreator">The decryptor creator function.</param>
        /// <param name="encryptorCreator">The encryptor creator function.</param>
        public Listener(int port, Func<PipeReader, IPipelinedDecryptor> decryptorCreator, Func<PipeWriter, IPipelinedEncryptor> encryptorCreator)
        {
            this.port = port;
            this.decryptorCreator = decryptorCreator;
            this.encryptorCreator = encryptorCreator;
        }

        /// <summary>
        /// Occurs when a client has been accepted by the tcp listener.
        /// </summary>
        public event EventHandler<ClientAcceptEventArgs> ClientAccepted;

        /// <summary>
        /// Occurs when a client has been accepted by the tcp listener, but before a <see cref="Connection"/> is created.
        /// </summary>
        public event CancelEventHandler ClientAccepting;

        /// <summary>
        /// Starts the tcp listener and begins to accept connections.
        /// </summary>
        public void Start()
        {
            this.clientListener = new TcpListener(IPAddress.Any, this.port);
            this.clientListener.Start();
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
        protected virtual IPipelinedDecryptor CreateDecryptor(PipeReader reader)
        {
            return this.decryptorCreator?.Invoke(reader);
        }

        /// <summary>
        /// Creates the encryptor for the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <returns>The created encryptor.</returns>
        protected virtual IPipelinedEncryptor CreateEncryptor(PipeWriter writer)
        {
            return this.encryptorCreator?.Invoke(writer);
        }

        private IConnection CreateConnection(Socket clientSocket)
        {
            var socketConnection = SocketConnection.Create(clientSocket);
            return new Connection(socketConnection, this.CreateDecryptor(socketConnection.Input), this.CreateEncryptor(socketConnection.Output));
        }

        private void OnAccept(IAsyncResult result)
        {
            Socket socket;
            try
            {
                socket = this.clientListener.EndAcceptSocket(result);
            }
            catch (ObjectDisposedException)
            {
                // this exception is expected when the clientListener got disposed. In this case we don't want to spam the log.
                return;
            }
            catch (Exception ex)
            {
                Log.Error("Error accepting the client socket", ex);
                return;
            }

            // Accept the next client:
            if (this.clientListener.Server.IsBound)
            {
                this.clientListener.BeginAcceptSocket(this.OnAccept, null);
            }

            var cancel = new CancelEventArgs();
            this.ClientAccepting?.Invoke(this, cancel);
            if (!cancel.Cancel)
            {
                var connection = this.CreateConnection(socket);
                this.ClientAccepted?.Invoke(this, new ClientAcceptEventArgs(connection));
            }
        }
    }
}

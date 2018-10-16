// <copyright file="Listener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
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

        private IConnection CreateConnection(Socket clientSocket)
        {
            var socketConnection = SocketConnection.Create(clientSocket);
            return new Connection(socketConnection, this.decryptorCreator(socketConnection.Input), this.encryptorCreator(socketConnection.Output));
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

            var connection = this.CreateConnection(socket);
            this.ClientAccepted?.Invoke(this, new ClientAcceptEventArgs(connection));
        }
    }
}

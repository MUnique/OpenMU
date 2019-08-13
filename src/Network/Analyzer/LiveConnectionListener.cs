// <copyright file="LiveConnectionListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer
{
    using System;
    using System.IO.Pipelines;
    using System.Net.Sockets;
    using System.Reflection;
    using log4net;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;
    using Pipelines.Sockets.Unofficial;

    /// <summary>
    /// A listener which creates <see cref="LiveConnection"/> instances for each connected client.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Network.Listener" />
    public class LiveConnectionListener : Listener
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly PlugInManager PlugInManager = CreatePlugInManager();

        private readonly Action<Delegate> invokeAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveConnectionListener" /> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="targetHost">The target server host.</param>
        /// <param name="targetPort">The target server port.</param>
        /// <param name="invokeAction">The invoke action to run an action on the UI thread.</param>
        public LiveConnectionListener(int port, string targetHost, int targetPort, Action<Delegate> invokeAction)
            : base(port, null, null)
        {
            this.TargetHost = targetHost;
            this.TargetPort = targetPort;
            this.invokeAction = invokeAction;

            this.ClientAccepted += this.OnClientAccepted;
        }

        /// <summary>
        /// Occurs when a client connected and the <see cref="LiveConnection"/> object has been created.
        /// </summary>
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;

        /// <summary>
        /// Gets or sets the client version.
        /// </summary>
        public ClientVersion ClientVersion { get; set; }

        /// <summary>
        /// Gets or sets the target server host.
        /// </summary>
        public string TargetHost { get; set; }

        /// <summary>
        /// Gets or sets the target server port.
        /// </summary>
        public int TargetPort { get; set; }

        private INetworkEncryptionFactoryPlugIn NetworkEncryptionPlugIn =>
            PlugInManager.GetStrategy<ClientVersion, INetworkEncryptionFactoryPlugIn>(this.ClientVersion)
            ?? PlugInManager.GetStrategy<ClientVersion, INetworkEncryptionFactoryPlugIn>(default);

        /// <inheritdoc />
        protected override IPipelinedDecryptor CreateDecryptor(PipeReader reader)
        {
            return this.GetDecryptor(reader, DataDirection.ClientToServer);
        }

        /// <inheritdoc />
        protected override IPipelinedEncryptor CreateEncryptor(PipeWriter writer)
        {
            return this.GetEncryptor(writer, DataDirection.ServerToClient);
        }

        private static PlugInManager CreatePlugInManager()
        {
            var plugInManager = new PlugInManager();
            plugInManager.DiscoverAndRegisterPlugIns();
            return plugInManager;
        }

        private IPipelinedEncryptor GetEncryptor(PipeWriter pipeWriter, DataDirection direction) => this.NetworkEncryptionPlugIn.CreateEncryptor(pipeWriter, direction);

        private IPipelinedDecryptor GetDecryptor(PipeReader pipeReader, DataDirection direction) => this.NetworkEncryptionPlugIn.CreateDecryptor(pipeReader, direction);

        private void OnClientAccepted(object sender, ClientAcceptEventArgs e)
        {
            var clientConnection = e.AcceptedConnection;
            try
            {
                var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Connect(this.TargetHost, this.TargetPort);
                var socketConnection = SocketConnection.Create(serverSocket);

                var decryptor = this.GetDecryptor(socketConnection.Input, DataDirection.ServerToClient);
                var encryptor = this.GetEncryptor(socketConnection.Output, DataDirection.ClientToServer);
                var serverConnection = new Connection(socketConnection, decryptor, encryptor);
                var proxy = new LiveConnection(clientConnection, serverConnection, this.invokeAction);

                this.ClientConnected?.Invoke(this, new ClientConnectedEventArgs(proxy));
            }
            catch (Exception exception)
            {
                Log.Error("Error while connecting to the server. Disconnecting the client.", exception);
                clientConnection.Disconnect();
            }
        }
    }
}
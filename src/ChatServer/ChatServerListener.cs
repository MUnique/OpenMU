// <copyright file="ChatServerListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer
{
    using System;
    using System.ComponentModel;
    using System.IO.Pipelines;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A listener which listens to the specified endpoint and provides the initialized <see cref="IConnection"/> by the event <see cref="ClientAccepted"/>.
    /// </summary>
    public class ChatServerListener
    {
        private readonly ChatServerEndpoint endpoint;
        private readonly PlugInManager plugInManager;
        private Listener chatClientListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatServerListener"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="plugInManager">The plug in manager.</param>
        public ChatServerListener(ChatServerEndpoint endpoint, PlugInManager plugInManager)
        {
            this.endpoint = endpoint;
            this.plugInManager = plugInManager;
        }

        /// <summary>
        /// Occurs when a new client was accepted.
        /// </summary>
        public event EventHandler<ClientAcceptEventArgs> ClientAccepted;

        /// <summary>
        /// Occurs when a client has been accepted by the tcp listener, but before a <see cref="Connection"/> is created.
        /// </summary>
        public event EventHandler<CancelEventArgs> ClientAccepting;

        /// <summary>
        /// Starts the tcp listener of this instance.
        /// </summary>
        public void Start()
        {
            this.chatClientListener = new Listener(this.endpoint.NetworkPort, this.CreateDecryptor, writer => null);
            this.chatClientListener.ClientAccepted += (sender, args) => this.ClientAccepted?.Invoke(sender, args);
            this.chatClientListener.ClientAccepting += (sender, args) => this.ClientAccepting?.Invoke(sender, args);
            this.chatClientListener.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.chatClientListener?.Stop();
        }

        private IPipelinedDecryptor CreateDecryptor(PipeReader pipeReader)
        {
            var encryptionFactoryPlugIn = this.plugInManager.GetStrategy<ClientVersion, INetworkEncryptionFactoryPlugIn>(this.endpoint.ClientVersion)
                                          ?? this.plugInManager.GetStrategy<ClientVersion, INetworkEncryptionFactoryPlugIn>(default);
            return encryptionFactoryPlugIn.CreateDecryptor(pipeReader, DataDirection.ClientToServer);
        }
    }
}

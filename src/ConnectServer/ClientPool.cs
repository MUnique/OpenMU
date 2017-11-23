// <copyright file="ClientPool.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System;
    using System.Collections.Concurrent;
    using System.Net;
    using System.Net.Sockets;

    using MUnique.OpenMU.ConnectServer.PacketHandler;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// A pool of client objects. With the help of this pool, client objects can be reused.
    /// This loweres the pressure on the garbage collector and improves the initialization of new clients.
    /// </summary>
    internal class ClientPool
    {
        private readonly IPacketHandler<Client> clientPacketHandler;

        private readonly ConcurrentQueue<Client> clientPool = new ConcurrentQueue<Client>();

        private readonly Settings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientPool"/> class.
        /// </summary>
        /// <param name="clientPacketHandler">The client packet handler.</param>
        /// <param name="settings">The settings.</param>
        public ClientPool(IPacketHandler<Client> clientPacketHandler, Settings settings)
        {
            this.settings = settings;
            this.clientPacketHandler = clientPacketHandler;
        }

        /// <summary>
        /// Gets a client from the pool.
        /// </summary>
        /// <param name="socket">The socket which holds the connection to the client.</param>
        /// <returns>The initialized client object.</returns>
        public Client GetClient(Socket socket)
        {
            if (this.clientPool.TryDequeue(out Client client))
            {
                client.Reset();
                var socketConnection = client.Connection as Connection;
                if (socketConnection == null)
                {
                    throw new InvalidOperationException(string.Format("Client connection must be of type {0}", typeof(Connection)));
                }

                socketConnection.Socket = socket;
            }
            else
            {
                client = new Client(new Connection(socket, null, null), this.settings.Timeout);
                client.Connection.PacketReceived += (sender, packet) => this.clientPacketHandler.HandlePacket(client, packet);
            }

            var ipEndpoint = (IPEndPoint)socket.RemoteEndPoint;
            client.Address = ipEndpoint.Address;
            client.Port = ipEndpoint.Port;
            client.Timeout = this.settings.Timeout;
            return client;
        }

        /// <summary>
        /// Adds the client to the pool, if the pool is not full yet. Otherwise, the client object will get disposed.
        /// </summary>
        /// <param name="client">The client.</param>
        public void Add(Client client)
        {
            if (this.clientPool.Count > this.settings.ClientPoolSize)
            {
                client.Dispose();
                return;
            }

            client.Reset();
            this.clientPool.Enqueue(client);
        }
    }
}

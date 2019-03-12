// <copyright file="ViewPlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System;
    using System.Linq;
    using System.Reflection;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.PlugIns;
    using Unity;
    using Unity.Lifetime;

    /// <summary>
    /// A plugin container which selects plugin based on the provided version/season metadata.
    /// </summary>
    /// <seealso cref="IViewPlugIn" />
    /// <remarks>
    /// Simplified example: View plugin container is meant for season 6.
    /// There are some IChatMessageViewPlugIns available for the seasons 1, 2, 6 and 7.
    /// Which one to choose?
    ///   In this case, it's easy, we just select the one which is equal.
    /// Now, if we remove the plugin for season 6, so that 1, 2 and 7 are available?
    ///   In this case, we assume that the last available season before the target season is the correct one, season 2.
    /// </remarks>
    internal class ViewPlugInContainer : CustomPlugInContainerBase<IViewPlugIn>
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewPlugInContainer"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="clientVersion">The client information.</param>
        /// <param name="manager">The manager.</param>
        public ViewPlugInContainer(RemotePlayer player, ClientVersion clientVersion, PlugInManager manager)
            : base(manager)
        {
            this.player = player;
            this.Client = clientVersion;

            this.Initialize();
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public ClientVersion Client { get; }

        /// <inheritdoc/>
        /// <remarks>We look if the activated plugin is rated at a higher client version than the current one.</remarks>
        protected override bool IsNewPlugInReplacingOld(IViewPlugIn currentEffectivePlugIn, IViewPlugIn activatedPlugIn)
        {
            var currentPlugInClient = currentEffectivePlugIn.GetType().GetCustomAttribute<ClientAttribute>()?.Client ?? default;
            var activatedPluginClient = activatedPlugIn.GetType().GetCustomAttribute<ClientAttribute>()?.Client ?? default;
            return currentPlugInClient.CompareTo(activatedPluginClient) < 0;
        }

        /// <inheritdoc />
        /// <remarks>We sort by version and choose the highest one.</remarks>
        protected override IViewPlugIn DetermineEffectivePlugIn(Type interfaceType)
        {
            return this.ActivePlugIns.OrderByDescending(p => p.GetType().GetCustomAttribute(typeof(ClientAttribute))).FirstOrDefault(interfaceType.IsInstanceOfType);
        }

        /// <inheritdoc />
        /// <remarks>We just take the plugins which have a equal or lower version than our target <see cref="Client"/>.</remarks>
        protected override void CreatePlugInIfSuitable(Type plugInType)
        {
            var clientAttribute = plugInType.GetCustomAttribute(typeof(ClientAttribute)) as ClientAttribute;
            if (clientAttribute == null // if the plugin doesn't specify a version, we use it as well
                || this.Client.CompareTo(clientAttribute.Client) >= 0)
            {
                using (var container = new UnityContainer())
                {
                    container.RegisterInstance(typeof(RemotePlayer), string.Empty, this.player, new ExternallyControlledLifetimeManager());
                    var plugIn = container.Resolve(plugInType) as IViewPlugIn;
                    this.AddPlugIn(plugIn, true);
                }
            }
        }
    }
}

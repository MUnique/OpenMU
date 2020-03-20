// <copyright file="ViewExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views
{
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// Extensions which are used by the remote views.
    /// </summary>
    public static class ViewExtensions
    {
        /// <summary>
        /// The constant player identifier which is sent to the player of a view to identify itself.
        /// </summary>
        public static readonly ushort ConstantPlayerId = 0x0200;

        /// <summary>
        /// Gets the <see cref="IIdentifiable.Id"/> of the <paramref name="identifiable"/> if it's not the same object
        /// instance as <paramref name="playerOfView"/>; Otherwise, the <see cref="ConstantPlayerId"/> is returned.
        /// </summary>
        /// <param name="identifiable">The identifiable.</param>
        /// <param name="playerOfView">The player of view.</param>
        /// <returns>
        /// The <see cref="IIdentifiable.Id"/> of the <paramref name="identifiable"/> if it's not the same object
        /// instance as <paramref name="playerOfView"/>; Otherwise, the <see cref="ConstantPlayerId"/> is returned.
        /// </returns>
        /// <remarks>
        /// This allows us to give players a dynamic <see cref="IIdentifiable.Id"/>, depending on the current game map.
        /// And the dynamic id allows us to add players of different game server instances to the same map instance
        /// without forcing the player client connect to another server.
        /// Examples for such shared maps are loren valley, crywolf, land of trials, loren market etc.
        /// </remarks>
        public static ushort GetId(this IIdentifiable identifiable, Player playerOfView)
        {
            if (playerOfView == identifiable)
            {
                return ConstantPlayerId;
            }

            return identifiable?.Id ?? 0;
        }

        /// <summary>
        /// Gets the object with the specified id which is on the same map as the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>The object with the specified id which is on the same map as the specified player.</returns>
        public static IIdentifiable GetObject(this Player player, ushort id)
        {
            if (id == ConstantPlayerId)
            {
                return player;
            }

            return player.CurrentMap.GetObject(id);
        }
    }
}
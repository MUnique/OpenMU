// <copyright file="IWorldView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// Interface for the world view. The world view shows the terrain, the objects and their visible (to other players) actions on the map.
    /// </summary>
    public interface IWorldView
    {
        /// <summary>
        /// An object got killed by another object.
        /// </summary>
        /// <param name="killedObject">The killed object.</param>
        /// <param name="killerObject">The object which killed the object.</param>
        void ObjectGotKilled(IAttackable killedObject, IAttackable killerObject);

        /// <summary>
        /// An object moved on the map.
        /// </summary>
        /// <param name="movedObject">The moved object.</param>
        /// <param name="moveType">Type of the move.</param>
        void ObjectMoved(ILocateable movedObject, MoveType moveType);

        /// <summary>
        /// Shows the dropped items.
        /// </summary>
        /// <param name="droppedItems">The dropped items.</param>
        /// <param name="freshDrops">if set to <c>true</c> this items are fresh drops; Otherwise they are already laying on the ground when reaching a newly discovered part of the map.</param>
        void ShowDroppedItems(IEnumerable<DroppedItem> droppedItems, bool freshDrops);

        /// <summary>
        /// The item drops disappeared from the ground.
        /// </summary>
        /// <param name="disappearedItemIds">The ids of the disappeared items.</param>
        void DroppedItemsDisappeared(IEnumerable<ushort> disappearedItemIds);

        /// <summary>
        /// Shows the animation.
        /// </summary>
        /// <param name="animatingObj">The animating object.</param>
        /// <param name="animation">The animation.</param>
        /// <param name="targetObj">The target object.</param>
        /// <param name="direction">The direction.</param>
        void ShowAnimation(IIdentifiable animatingObj, byte animation, IIdentifiable targetObj, byte direction);

        /// <summary>
        /// Will be called then the map got changed. The new map and coordinates are defined in the player.SelectedCharacter.CurrentMap.
        /// </summary>
        void MapChange();

        /// <summary>
        /// Objects are out of scope.
        /// </summary>
        /// <param name="objects">The objects.</param>
        void ObjectsOutOfScope(IEnumerable<IIdentifiable> objects);

        /// <summary>
        /// Shows the new players in scope.
        /// </summary>
        /// <param name="newObjects">The new objects.</param>
        void NewPlayersInScope(IEnumerable<Player> newObjects);

        /// <summary>
        /// Shows the new npcs in scope.
        /// </summary>
        /// <param name="newObjects">The new objects.</param>
        void NewNpcsInScope(IEnumerable<NonPlayerCharacter> newObjects);

        /// <summary>
        /// Updates the rotation of the own player.
        /// </summary>
        void UpdateRotation();

        /// <summary>
        /// Shows the skill animation.
        /// </summary>
        /// <param name="attackingPlayer">The attacking player.</param>
        /// <param name="target">The target.</param>
        /// <param name="skill">The skill.</param>
        void ShowSkillAnimation(Player attackingPlayer, IAttackable target, Skill skill);

        /// <summary>
        /// Shows the area skill animation.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="skill">The skill.</param>
        /// <param name="point">The coordinates.</param>
        /// <param name="rotation">The rotation.</param>
        void ShowAreaSkillAnimation(Player player, Skill skill, Point point, byte rotation);
    }
}

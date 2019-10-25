// <copyright file="CharacterStatusExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Network.Packets;
    using CharacterStatus = MUnique.OpenMU.Network.Packets.ServerToClient.CharacterStatus;

    /// <summary>
    /// Extension methods for <see cref="DataModel.Entities.CharacterStatus"/>.
    /// </summary>
    internal static class CharacterStatusExtensions
    {
        /// <summary>
        /// Converts the status of the data model to the status of the defined packets.
        /// </summary>
        /// <param name="status">The status of the character in the type of the data model.</param>
        /// <returns>The status of the character in the type of the packet definitions.</returns>
        public static CharacterStatus Convert(this DataModel.Entities.CharacterStatus status)
        {
            return status switch
            {
                DataModel.Entities.CharacterStatus.Normal => CharacterStatus.Normal,
                DataModel.Entities.CharacterStatus.Banned => CharacterStatus.Banned,
                DataModel.Entities.CharacterStatus.GameMaster => CharacterStatus.GameMaster,
                _ => throw new ArgumentException($"Unknown character status {status}"),
            };
        }

        public static CharacterHeroState Convert(this HeroState heroState)
        {
            return heroState switch
            {
                HeroState.Normal => CharacterHeroState.Normal,
                HeroState.Hero => CharacterHeroState.Hero,
                HeroState.LightHero => CharacterHeroState.LightHero,
                HeroState.New => CharacterHeroState.New,
                HeroState.PlayerKillWarning => CharacterHeroState.PlayerKillWarning,
                HeroState.PlayerKiller1stStage => CharacterHeroState.PlayerKiller1stStage,
                HeroState.PlayerKiller2ndStage => CharacterHeroState.PlayerKiller2ndStage,
                _ => throw new ArgumentException($"Unhandled case of {heroState}."),
            };
        }
    }
}
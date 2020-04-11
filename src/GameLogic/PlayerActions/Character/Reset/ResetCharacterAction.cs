// <copyright file="ResetCharacterAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character.Reset
{
    using System.Linq;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Login;

    /// <summary>
    /// Action to reset a character.
    /// </summary>
    public class ResetCharacterAction
    {
        private readonly Player player;
        private readonly LogoutAction logoutAction = new LogoutAction();

        // Reset System Configuration
        private readonly bool IsEnabled = true;
        private readonly int ResetLimit = -1;
        private readonly int RequiredLevel = 400;
        private readonly int RequiredZen = 1;
        private readonly bool MultiplyZenByResetCount = true;
        private readonly bool ResetStats = true;
        private readonly bool MultiplyPointsByResetCount = true;
        private readonly int PointsPerReset = 1500;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetCharacterAction"/> class.
        /// </summary>
        /// <param name="player">Player to reset.</param>
        public ResetCharacterAction(Player player)
        {
            this.player = player;
        }

        /// <summary>
        /// Reset specific character.
        /// </summary>
        public void ResetCharacter()
        {
            this.CheckEnabled();
            this.CheckLevel();
            this.CheckResetLimit();
            this.ConsumeZen();
            this.AddReset();
            this.ResetLevel();
            this.UpdateStats();
            this.UpdatePlayer();
        }

        private void CheckEnabled()
        {
            if (!this.IsEnabled)
            {
                throw new ResetCharacterActionException("Reset system is disabled");
            }
        }

        private void CheckLevel()
        {
            if (this.player.Level < this.RequiredLevel)
            {
                throw new ResetCharacterActionException($"[Reset System] Required level is {this.RequiredLevel}");
            }
        }

        private int GetResetCount()
        {
            return (int)this.player.Attributes[Stats.Resets];
        }

        private void CheckResetLimit()
        {
            if (this.ResetLimit > 0 && (this.GetResetCount() + 1) > this.ResetLimit)
            {
                throw new ResetCharacterActionException($"[Reset System] Max reset is {this.ResetLimit}");
            }
        }

        private void ConsumeZen()
        {
            var requiredZen = this.RequiredZen;
            if (this.MultiplyZenByResetCount)
            {
                requiredZen *= this.GetResetCount() + 1;
            }

            if (!this.player.TryRemoveMoney(requiredZen))
            {
                throw new ResetCharacterActionException($"[Reset System] You don't have enough Money, required zen is {requiredZen}");
            }
        }

        private void AddReset()
        {
            this.player.Attributes[Stats.Resets]++;
        }

        private void ResetLevel()
        {
            this.player.Attributes[Stats.Level] = 1;
            this.player.SelectedCharacter.Experience = 0;
        }

        private void UpdateStats()
        {
            var pointsPerReset = this.PointsPerReset;
            if (this.MultiplyPointsByResetCount)
            {
                pointsPerReset *= this.GetResetCount();
            }

            if (this.ResetStats)
            {
                this.player.SelectedCharacter.CharacterClass.StatAttributes.ForEach(stat => this.player.Attributes[stat.Attribute] = stat.BaseValue);
            }

            this.player.SelectedCharacter.LevelUpPoints = pointsPerReset;
        }

        private void UpdatePlayer()
        {
            var homeMap = this.player.SelectedCharacter.CharacterClass.HomeMap;
            var randomSpawn = homeMap.SafezoneMap.ExitGates.Where(g => g.IsSpawnGate).SelectRandom();
            this.player.SelectedCharacter.PositionX = (byte)Rand.NextInt(randomSpawn.X1, randomSpawn.X2);
            this.player.SelectedCharacter.PositionY = (byte)Rand.NextInt(randomSpawn.Y1, randomSpawn.Y2);
            this.player.SelectedCharacter.CurrentMap = randomSpawn.Map;
            this.player.Rotation = randomSpawn.Direction;
            this.logoutAction.Logout(this.player, LogoutType.BackToCharacterSelection);
        }
    }
}

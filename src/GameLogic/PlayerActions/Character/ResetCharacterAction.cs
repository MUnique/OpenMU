// <copyright file="ResetCharacterAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character
{
    using System.Linq;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Login;
    using MUnique.OpenMU.GameLogic.Views.NPC;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action to reset a character.
    /// </summary>
    public class ResetCharacterAction
    {
        /// <summary>
        /// Is reset system enabled.
        /// </summary>
        public static readonly bool IsEnabled = false;
        private readonly Player player;
        private readonly NonPlayerCharacter npc;
        private readonly LogoutAction logoutAction = new LogoutAction();

        /// <summary>
        /// Reset System Configuration.
        /// </summary>
        private readonly int resetLimit = -1;
        private readonly int requiredLevel = 400;
        private readonly int levelAfterReset = 10;
        private readonly int requiredMoney = 1;
        private readonly bool multiplyZenByResetCount = true;
        private readonly bool resetStats = true;
        private readonly bool multiplyPointsByResetCount = true;
        private readonly int pointsPerReset = 1500;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetCharacterAction"/> class.
        /// </summary>
        /// <param name="player">Player to reset.</param>
        /// <param name="npc">Npc thats call the reset action.</param>
        public ResetCharacterAction(Player player, NonPlayerCharacter npc = null)
        {
            this.player = player;
            this.npc = npc;
        }

        /// <summary>
        /// Reset specific character.
        /// </summary>
        public void ResetCharacter()
        {
            if (!IsEnabled)
            {
                this.ShowMessage("Reset is not enabled.");
                return;
            }

            if (this.player.PlayerState.CurrentState != PlayerState.EnteredWorld)
            {
                this.ShowMessage("Cannot do reset with any windows opened.");
                return;
            }

            if (this.player.Level < this.requiredLevel)
            {
                this.ShowMessage($"Required level for reset is {this.requiredLevel}.");
                return;
            }

            if (this.resetLimit > 0 && (this.GetResetCount() + 1) > this.resetLimit)
            {
                this.ShowMessage($"Maximum resets of {this.resetLimit} reached.");
                return;
            }

            if (!this.TryConsumeMoney())
            {
                return;
            }

            this.AddReset();
            this.ResetLevel();
            this.UpdateStats();
            this.MoveHome();
            this.Logout();
        }

        private void ShowMessage(string message)
        {
            if (this.npc is null)
            {
                this.player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage(message, MessageType.BlueNormal);
            }
            else
            {
                this.player.ViewPlugIns.GetPlugIn<IShowMessageOfObjectPlugIn>()?.ShowMessageOfObject(message, this.npc);
            }
        }

        private int GetResetCount()
        {
            return (int)this.player.Attributes[Stats.Resets];
        }

        private bool TryConsumeMoney()
        {
            var calculatedRequiredZen = this.requiredMoney;
            if (this.multiplyZenByResetCount)
            {
                calculatedRequiredZen *= this.GetResetCount() + 1;
            }

            if (!this.player.TryRemoveMoney(calculatedRequiredZen))
            {
                this.ShowMessage($"You don't have enough money for reset, required zen is {calculatedRequiredZen}");
                return false;
            }

            return true;
        }

        private void AddReset()
        {
            this.player.Attributes[Stats.Resets]++;
        }

        private void ResetLevel()
        {
            this.player.Attributes[Stats.Level] = this.levelAfterReset;
            this.player.SelectedCharacter.Experience = 0;
        }

        private void UpdateStats()
        {
            var calculatedPointsPerReset = this.pointsPerReset;
            if (this.multiplyPointsByResetCount)
            {
                calculatedPointsPerReset *= this.GetResetCount();
            }

            if (this.resetStats)
            {
                this.player.SelectedCharacter.CharacterClass.StatAttributes
                    .Where(s => s.IncreasableByPlayer)
                    .ForEach(s => this.player.Attributes[s.Attribute] = s.BaseValue);
            }

            this.player.SelectedCharacter.LevelUpPoints = calculatedPointsPerReset;
        }

        private void MoveHome()
        {
            var homeMap = this.player.SelectedCharacter.CharacterClass.HomeMap;
            var randomSpawn = homeMap.SafezoneMap.ExitGates.Where(g => g.IsSpawnGate).SelectRandom();
            this.player.SelectedCharacter.PositionX = (byte)Rand.NextInt(randomSpawn.X1, randomSpawn.X2);
            this.player.SelectedCharacter.PositionY = (byte)Rand.NextInt(randomSpawn.Y1, randomSpawn.Y2);
            this.player.SelectedCharacter.CurrentMap = randomSpawn.Map;
            this.player.Rotation = randomSpawn.Direction;
        }

        private void Logout()
        {
            this.logoutAction.Logout(this.player, LogoutType.BackToCharacterSelection);
        }
    }
}

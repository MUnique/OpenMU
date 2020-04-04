namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The pkclear command which clears players pk status.
    /// </summary>
    [Guid("8025961F-96B3-46B3-9EED-0CB05CDAC3E0")]
    [PlugIn("PKClear", "Clears players pk status")]

    public class PKClearCommand : IChatCommandPlugIn
    {
        public static string Command = "/pkclear";

        /// <inheritdoc />
        public string Key => Command;

        /// <inheritdoc />
        public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            try
            {
                int Cost = player.GameContext.Configuration.PKClearZenCost;

                if ((int)player.SelectedCharacter.State >= 4 && player.Money >= Cost)
                {
                    player.SelectedCharacter.State = HeroState.Normal;

                    if (Cost <= 0)
                    {
                        player.ViewPlugIns.GetPlugIn<IUpdateCharacterHeroStatePlugIn>()?.UpdateCharacterHeroState();
                        player.ShowMessage($"[PK System] Status cleard.");
                        return;
                    }

                    player.TryRemoveMoney(Cost);
                    player.ViewPlugIns.GetPlugIn<IUpdateCharacterHeroStatePlugIn>()?.UpdateCharacterHeroState();
                    player.ShowMessage($"[PK System] Status cleard, -{Cost} zen.");
                }
                else
                {
                    if ((int)player.SelectedCharacter.State <= 3 && player.Money >= Cost)
                    {
                        player.ShowMessage($"[PK System] Status cannot be cleard, your status is {player.SelectedCharacter.State.ToString().ToLower()}.");
                    }
                    else if ((int)player.SelectedCharacter.State >= 4 && player.Money < Cost)
                    {

                        player.ShowMessage($"[PK System] Status cannot be cleard, zen reqiuerd: {Cost}.");
                    }
                }
            }
            catch (ArgumentException e)
            {
                player.ShowMessage(e.Message);
            }
        }
    }
}
// <copyright file="MuBotSaveDataAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.MuBot
{
    using System;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views.MuBot;

    /// <summary>
    /// Action to update mu bot status.
    /// </summary>
    public class MuBotSaveDataAction
    {
        /// <summary>
        /// Toggle mu bot status.
        /// </summary>
        /// <param name="player">the player.</param>
        /// <param name="data">mu bot data to be saved.</param>
        public void SaveData(Player player, Span<byte> data)
        {
            try
            {
                var muBotData = player.PersistenceContext.CreateNew<MuBotData>();
                muBotData.Data = data.ToArray();
                muBotData.CharacterId = player.SelectedCharacter.Id;
                player.PersistenceContext.SaveChanges();
                player.ViewPlugIns.GetPlugIn<IMuBotSaveDataResponse>()?.SendMuBotSavedDataResponse(data);
            }
            catch (Exception e)
            {
                player.Logger.LogWarning($"Cannot save MuBotData => {e}");
            }
        }
    }
}
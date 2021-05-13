// <copyright file="TalkNpcAction075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.NPC;

    /// <summary>
    /// Action to talk to a npc for older versions.
    /// </summary>
    public class TalkNpcAction075 : TalkNpcAction
    {
        /// <inheritdoc />
        protected override bool AdvancePlayerState(NonPlayerCharacter npc)
        {
            switch (npc.Definition.NpcWindow)
            {
                case NpcWindow.ChaosMachine:
                case NpcWindow.VaultStorage:
                    return true;
                default:
                    return false;
            }
        }
    }
}
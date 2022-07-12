//namespace MUnique.OpenMU.PlugIns.Proxies
//{
//    using System;
//    using System.Collections.Generic;
//    using Nito.AsyncEx;
//    using MUnique.OpenMU.GameLogic.PlugIns;
//    using MUnique.OpenMU.GameLogic;
//    using MUnique.OpenMU.GameLogic.NPC;
//    using MUnique.OpenMU.GameLogic.PlugIns;

//    public class PlayerTalkToNpcPlugInProxy : PlugInContainerBase<IPlayerTalkToNpcPlugIn>, IPlayerTalkToNpcPlugIn
//    {
//        public PlayerTalkToNpcPlugInProxy(PlugInManager manager) : base(manager)
//        {
//        }

//        public async System.Threading.Tasks.ValueTask PlayerTalksToNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
//        {
//            using var l = await this.Lock.ReaderLockAsync();
//            {
//                foreach (IPlayerTalkToNpcPlugIn plugIn in this.ActivePlugIns)
//                {
//                    if (!eventArgs.Cancel)
//                    {
//                        await plugIn.PlayerTalksToNpcAsync(player, npc, eventArgs);
//                    }
//                }
//            }
//        }
//    }
//}
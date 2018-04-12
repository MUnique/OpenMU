// <copyright file="GuildInfoSerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// The default guild info serializer which serializes into the specific data packet format.
    /// </summary>
    internal class GuildInfoSerializer : GuildCache.IGuildInfoSerializer
    {
        /// <inheritdoc/>
        public byte[] Serialize(Guild guild, uint guildId)
        {
            /*
         *  C1 3C 66 00
            87 38 00 00 // guild number
            00  // guild type
            54 68 65 4F 6E 65 00 00 //TheOne - Maintain
            41 76 61 6C 6F 6E 00 2B //Avalon - Assistant
            18 88 88 81 18 66 66 81 18 61 16 81 18 61 16 81 18 66 66 81 18 61 16 81 18 61 16 81 18 61 16 81 //Guild Logo
            F9 96 7C //?
         */
            var result = new byte[0x3C];
            result.SetValues<byte>(0xC1, (byte)result.Length, 0x66);
            result.SetIntegerBigEndian(guildId, 4);
            Encoding.ASCII.GetBytes(guild.Name, 0, guild.Name.Length, result, 17);
            Buffer.BlockCopy(guild.Logo, 0, result, 25, 32);
            if (guild.AllianceGuild != null)
            {
                Encoding.ASCII.GetBytes(guild.AllianceGuild.Name, 0, guild.AllianceGuild.Name.Length, result, 9);
            }

            return result;
        }
    }
}

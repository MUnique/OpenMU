// <copyright file="ShowGuildInfoPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowGuildInfoPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowGuildInfoPlugIn", "The default implementation of the IShowGuildInfoPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("65f9d310-4adf-48b4-ace2-16779b254ecf")]
    public class ShowGuildInfoPlugIn : BaseGuildInfoPlugIn<ShowGuildInfoPlugIn>, IShowGuildInfoPlugIn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowGuildInfoPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowGuildInfoPlugIn(RemotePlayer player)
            : base(player)
        {
        }

        /// <inheritdoc/>
        public void ShowGuildInfo(uint guildId)
        {
            var data = this.GetGuildData(guildId);
            if (data == null)
            {
                return;
            }

            // guildInfo is the cached, serialized result of the GuildInformation-Class.
            using (var writer = this.Player.Connection.StartSafeWrite(data[0], data.Length))
            {
                data.CopyTo(writer.Span);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        protected override byte[] Serialize(Guild guild, uint guildId)
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
            Encoding.UTF8.GetBytes(guild.Name, 0, guild.Name.Length, result, 17);
            Buffer.BlockCopy(guild.Logo, 0, result, 25, 32);
            if (guild.AllianceGuild != null)
            {
                Encoding.UTF8.GetBytes(guild.AllianceGuild.Name, 0, guild.AllianceGuild.Name.Length, result, 9);
            }

            return result;
        }
    }
}
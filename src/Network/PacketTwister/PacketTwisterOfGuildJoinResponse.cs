// <copyright file="PacketTwisterOfGuildJoinResponse.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'GuildJoinResponse' type.
    /// </summary>
    internal class PacketTwisterOfGuildJoinResponse : IPacketTwister
    {
        /// <inheritdoc/>
        public void Twist(IList<byte> data)
        {
            if (data.Count >= 4)
            {
                if (data.Count >= 8)
                {
                    if (data.Count >= 16)
                    {
                        if (data.Count >= 32)
                        {
                            data[20] ^= 0x29;
                            var v10 = (byte)(data[13] >> 4);
                            data[13] *= 16;
                            data[13] |= v10;
                            var v11 = (byte)(data[25] >> 3);
                            data[25] *= 32;
                            data[25] |= v11;
                            var v12 = (byte)(data[15] >> 6);
                            data[15] *= 4;
                            data[15] |= v12;
                            var v13 = data[13];
                            data[13] = data[29];
                            data[29] = v13;
                            var v14 = data[23];
                            data[23] = data[23];
                            data[23] = v14;
                            var v15 = (byte)(data[25] >> 5);
                            data[25] *= 8;
                            data[25] |= v15;
                            var v16 = (byte)(data[18] >> 6);
                            data[18] *= 4;
                            data[18] |= v16;
                            var v17 = (byte)(data[7] >> 5);
                            data[7] *= 8;
                            data[7] |= v17;
                            data[10] ^= 0x75;
                            data[15] ^= 0xEA;
                        }
                        else
                        {
                            var v6 = data[5];
                            data[5] = data[9];
                            data[9] = v6;
                            var v7 = (byte)(data[13] >> 1);
                            data[13] <<= 7;
                            data[13] |= v7;
                            var v8 = data[10];
                            data[10] = data[0];
                            data[0] = v8;
                            var v9 = data[3];
                            data[3] = data[11];
                            data[11] = v9;
                        }
                    }
                    else
                    {
                        var v4 = (byte)(data[2] >> 1);
                        data[2] <<= 7;
                        data[2] |= v4;
                        data[4] ^= 0xEB;
                        var v5 = data[3];
                        data[3] = data[2];
                        data[2] = v5;
                        var v2 = (byte)((data[1] >> 1) & 1);
                        if (((data[1] >> 1) & 1) != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        if (v2 != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }
                    }
                }
                else
                {
                    data[3] ^= 0xDE;
                    data[3] ^= 0x9A;
                    var v3 = (byte)(data[1] >> 2);
                    data[1] <<= 6;
                    data[1] |= v3;
                }
            }
        }

        /// <inheritdoc/>
        public void Correct(IList<byte> data)
        {
            if (data.Count >= 4)
            {
                if (data.Count >= 8)
                {
                    if (data.Count >= 16)
                    {
                        if (data.Count >= 32)
                        {
                            data[15] ^= 0xEA;
                            data[10] ^= 0x75;
                            var v10 = (byte)(data[7] >> 3);
                            data[7] *= 32;
                            data[7] |= v10;
                            var v11 = (byte)(data[18] >> 2);
                            data[18] <<= 6;
                            data[18] |= v11;
                            var v12 = (byte)(data[25] >> 3);
                            data[25] *= 32;
                            data[25] |= v12;
                            var v13 = data[23];
                            data[23] = data[23];
                            data[23] = v13;
                            var v14 = data[13];
                            data[13] = data[29];
                            data[29] = v14;
                            var v15 = (byte)(data[15] >> 2);
                            data[15] <<= 6;
                            data[15] |= v15;
                            var v16 = (byte)(data[25] >> 5);
                            data[25] *= 8;
                            data[25] |= v16;
                            var v17 = (byte)(data[13] >> 4);
                            data[13] *= 16;
                            data[13] |= v17;
                            data[20] ^= 0x29;
                        }
                        else
                        {
                            var v6 = data[3];
                            data[3] = data[11];
                            data[11] = v6;
                            var v7 = data[10];
                            data[10] = data[0];
                            data[0] = v7;
                            var v8 = (byte)(data[13] >> 7);
                            data[13] *= 2;
                            data[13] |= v8;
                            var v9 = data[5];
                            data[5] = data[9];
                            data[9] = v9;
                        }
                    }
                    else
                    {
                        var v2 = (byte)((data[1] >> 1) & 1);
                        if (((data[1] >> 1) & 1) != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        if (v2 != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        var v4 = data[3];
                        data[3] = data[2];
                        data[2] = v4;
                        data[4] ^= 0xEB;
                        var v5 = (byte)(data[2] >> 7);
                        data[2] *= 2;
                        data[2] |= v5;
                    }
                }
                else
                {
                    var v3 = (byte)(data[1] >> 6);
                    data[1] *= 4;
                    data[1] |= v3;
                    data[3] ^= 0x9A;
                    data[3] ^= 0xDE;
                }
            }
        }
    }
}
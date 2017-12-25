// <copyright file="PacketTwisterOfGuildKick.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'GuildKick' type.
    /// </summary>
    internal class PacketTwisterOfGuildKick : IPacketTwister
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
                            data[16] ^= 0xD9;
                            var v24 = (byte)((data[28] >> 7) & 1);
                            if (((data[28] >> 1) & 1) != 0)
                            {
                                data[28] |= 0x80;
                            }
                            else
                            {
                                data[28] &= 0x7F;
                            }

                            if (v24 != 0)
                            {
                                data[28] |= 2;
                            }
                            else
                            {
                                data[28] &= 0xFD;
                            }

                            var v18 = data[2];
                            data[2] = data[4];
                            data[4] = v18;
                            var v23 = (byte)((data[5] >> 3) & 1);
                            if (((data[5] >> 6) & 1) != 0)
                            {
                                data[5] |= 8;
                            }
                            else
                            {
                                data[5] &= 0xF7;
                            }

                            if (v23 != 0)
                            {
                                data[5] |= 0x40;
                            }
                            else
                            {
                                data[5] &= 0xBF;
                            }

                            var v19 = (byte)(data[18] >> 6);
                            data[18] *= 4;
                            data[18] |= v19;
                            var v20 = data[16];
                            data[16] = data[0];
                            data[0] = v20;
                            data[10] ^= 0x2B;
                            var v22 = (byte)((data[24] >> 2) & 1);
                            if (((data[24] >> 2) & 1) != 0)
                            {
                                data[24] |= 4;
                            }
                            else
                            {
                                data[24] &= 0xFB;
                            }

                            if (v22 != 0)
                            {
                                data[24] |= 4;
                            }
                            else
                            {
                                data[24] &= 0xFB;
                            }

                            var v21 = (byte)(data[27] >> 1);
                            data[27] <<= 7;
                            data[27] |= v21;
                        }
                        else
                        {
                            var v12 = (byte)(data[9] >> 7);
                            data[9] *= 2;
                            data[9] |= v12;
                            var v13 = data[6];
                            data[6] = data[7];
                            data[7] = v13;
                            var v14 = data[9];
                            data[9] = data[14];
                            data[14] = v14;
                            var v15 = data[1];
                            data[1] = data[12];
                            data[12] = v15;
                            var v25 = (byte)((data[0] >> 7) & 1);
                            if (((data[0] >> 1) & 1) != 0)
                            {
                                data[0] |= 0x80;
                            }
                            else
                            {
                                data[0] &= 0x7F;
                            }

                            if (v25 != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            data[6] ^= 0x8D;
                            data[12] ^= 0x85;
                            data[11] ^= 0xCC;
                            var v16 = (byte)(data[9] >> 4);
                            data[9] *= 16;
                            data[9] |= v16;
                            var v17 = data[5];
                            data[5] = data[3];
                            data[3] = v17;
                        }
                    }
                    else
                    {
                        var v6 = data[6];
                        data[6] = data[6];
                        data[6] = v6;
                        var v7 = data[0];
                        data[0] = data[4];
                        data[4] = v7;
                        data[5] ^= 0xAC;
                        var v2 = (byte)((data[5] >> 2) & 1);
                        if (((data[5] >> 5) & 1) != 0)
                        {
                            data[5] |= 4;
                        }
                        else
                        {
                            data[5] &= 0xFB;
                        }

                        if (v2 != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        var v8 = data[7];
                        data[7] = data[0];
                        data[0] = v8;
                        data[4] ^= 0xE5;
                        var v9 = data[6];
                        data[6] = data[0];
                        data[0] = v9;
                        data[5] ^= 0xD9;
                        var v28 = (byte)((data[6] >> 4) & 1);
                        if (((data[6] >> 4) & 1) != 0)
                        {
                            data[6] |= 0x10;
                        }
                        else
                        {
                            data[6] &= 0xEF;
                        }

                        if (v28 != 0)
                        {
                            data[6] |= 0x10;
                        }
                        else
                        {
                            data[6] &= 0xEF;
                        }

                        var v27 = (byte)((data[1] >> 2) & 1);
                        if (((data[1] >> 4) & 1) != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        if (v27 != 0)
                        {
                            data[1] |= 0x10;
                        }
                        else
                        {
                            data[1] &= 0xEF;
                        }

                        var v26 = (byte)((data[4] >> 5) & 1);
                        if (((data[4] >> 1) & 1) != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        if (v26 != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        var v10 = (byte)(data[4] >> 6);
                        data[4] *= 4;
                        data[4] |= v10;
                        var v11 = data[7];
                        data[7] = data[2];
                        data[2] = v11;
                    }
                }
                else
                {
                    var v3 = (byte)(data[2] >> 1);
                    data[2] <<= 7;
                    data[2] |= v3;
                    data[2] ^= 0x71;
                    var v4 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v4;
                    var v5 = data[1];
                    data[1] = data[3];
                    data[3] = v5;
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
                            var v18 = (byte)(data[27] >> 7);
                            data[27] *= 2;
                            data[27] |= v18;
                            var v24 = (byte)((data[24] >> 2) & 1);
                            if (((data[24] >> 2) & 1) != 0)
                            {
                                data[24] |= 4;
                            }
                            else
                            {
                                data[24] &= 0xFB;
                            }

                            if (v24 != 0)
                            {
                                data[24] |= 4;
                            }
                            else
                            {
                                data[24] &= 0xFB;
                            }

                            data[10] ^= 0x2B;
                            var v19 = data[16];
                            data[16] = data[0];
                            data[0] = v19;
                            var v20 = (byte)(data[18] >> 2);
                            data[18] <<= 6;
                            data[18] |= v20;
                            var v23 = (byte)((data[5] >> 3) & 1);
                            if (((data[5] >> 6) & 1) != 0)
                            {
                                data[5] |= 8;
                            }
                            else
                            {
                                data[5] &= 0xF7;
                            }

                            if (v23 != 0)
                            {
                                data[5] |= 0x40;
                            }
                            else
                            {
                                data[5] &= 0xBF;
                            }

                            var v21 = data[2];
                            data[2] = data[4];
                            data[4] = v21;
                            var v22 = (byte)((data[28] >> 7) & 1);
                            if (((data[28] >> 1) & 1) != 0)
                            {
                                data[28] |= 0x80;
                            }
                            else
                            {
                                data[28] &= 0x7F;
                            }

                            if (v22 != 0)
                            {
                                data[28] |= 2;
                            }
                            else
                            {
                                data[28] &= 0xFD;
                            }

                            data[16] ^= 0xD9;
                        }
                        else
                        {
                            var v12 = data[5];
                            data[5] = data[3];
                            data[3] = v12;
                            var v13 = (byte)(data[9] >> 4);
                            data[9] *= 16;
                            data[9] |= v13;
                            data[11] ^= 0xCC;
                            data[12] ^= 0x85;
                            data[6] ^= 0x8D;
                            var v25 = (byte)((data[0] >> 7) & 1);
                            if (((data[0] >> 1) & 1) != 0)
                            {
                                data[0] |= 0x80;
                            }
                            else
                            {
                                data[0] &= 0x7F;
                            }

                            if (v25 != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            var v14 = data[1];
                            data[1] = data[12];
                            data[12] = v14;
                            var v15 = data[9];
                            data[9] = data[14];
                            data[14] = v15;
                            var v16 = data[6];
                            data[6] = data[7];
                            data[7] = v16;
                            var v17 = (byte)(data[9] >> 1);
                            data[9] <<= 7;
                            data[9] |= v17;
                        }
                    }
                    else
                    {
                        var v6 = data[7];
                        data[7] = data[2];
                        data[2] = v6;
                        var v7 = (byte)(data[4] >> 2);
                        data[4] <<= 6;
                        data[4] |= v7;
                        var v2 = (byte)((data[4] >> 5) & 1);
                        if (((data[4] >> 1) & 1) != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        if (v2 != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        var v28 = (byte)((data[1] >> 2) & 1);
                        if (((data[1] >> 4) & 1) != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        if (v28 != 0)
                        {
                            data[1] |= 0x10;
                        }
                        else
                        {
                            data[1] &= 0xEF;
                        }

                        var v27 = (byte)((data[6] >> 4) & 1);
                        if (((data[6] >> 4) & 1) != 0)
                        {
                            data[6] |= 0x10;
                        }
                        else
                        {
                            data[6] &= 0xEF;
                        }

                        if (v27 != 0)
                        {
                            data[6] |= 0x10;
                        }
                        else
                        {
                            data[6] &= 0xEF;
                        }

                        data[5] ^= 0xD9;
                        var v8 = data[6];
                        data[6] = data[0];
                        data[0] = v8;
                        data[4] ^= 0xE5;
                        var v9 = data[7];
                        data[7] = data[0];
                        data[0] = v9;
                        var v26 = (byte)((data[5] >> 2) & 1);
                        if (((data[5] >> 5) & 1) != 0)
                        {
                            data[5] |= 4;
                        }
                        else
                        {
                            data[5] &= 0xFB;
                        }

                        if (v26 != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        data[5] ^= 0xAC;
                        var v10 = data[0];
                        data[0] = data[4];
                        data[4] = v10;
                        var v11 = data[6];
                        data[6] = data[6];
                        data[6] = v11;
                    }
                }
                else
                {
                    var v3 = data[1];
                    data[1] = data[3];
                    data[3] = v3;
                    var v4 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v4;
                    data[2] ^= 0x71;
                    var v5 = (byte)(data[2] >> 7);
                    data[2] *= 2;
                    data[2] |= v5;
                }
            }
        }
    }
}
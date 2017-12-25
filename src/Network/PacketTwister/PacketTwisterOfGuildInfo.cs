// <copyright file="PacketTwisterOfGuildInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'GuildInfo' type.
    /// </summary>
    internal class PacketTwisterOfGuildInfo : IPacketTwister
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
                            var v10 = data[27];
                            data[27] = data[7];
                            data[7] = v10;
                            var v11 = (byte)(data[31] >> 6);
                            data[31] *= 4;
                            data[31] |= v11;
                            var v12 = (byte)(data[0] >> 4);
                            data[0] *= 16;
                            data[0] |= v12;
                            data[29] ^= 0x39;
                            data[13] ^= 0xCA;
                            var v13 = data[9];
                            data[9] = data[17];
                            data[17] = v13;
                            data[10] ^= 0xC2;
                            var v14 = data[6];
                            data[6] = data[11];
                            data[11] = v14;
                        }
                        else
                        {
                            var v17 = (byte)((data[2] >> 3) & 1);
                            if (((data[2] >> 6) & 1) != 0)
                            {
                                data[2] |= 8;
                            }
                            else
                            {
                                data[2] &= 0xF7;
                            }

                            if (v17 != 0)
                            {
                                data[2] |= 0x40;
                            }
                            else
                            {
                                data[2] &= 0xBF;
                            }

                            var v16 = (byte)((data[11] >> 2) & 1);
                            if (((data[11] >> 7) & 1) != 0)
                            {
                                data[11] |= 4;
                            }
                            else
                            {
                                data[11] &= 0xFB;
                            }

                            if (v16 != 0)
                            {
                                data[11] |= 0x80;
                            }
                            else
                            {
                                data[11] &= 0x7F;
                            }

                            data[12] ^= 0x2A;
                            var v6 = data[6];
                            data[6] = data[3];
                            data[3] = v6;
                            var v7 = (byte)(data[7] >> 5);
                            data[7] *= 8;
                            data[7] |= v7;
                            var v8 = data[9];
                            data[9] = data[10];
                            data[10] = v8;
                            var v15 = (byte)((data[10] >> 3) & 1);
                            if (((data[10] >> 4) & 1) != 0)
                            {
                                data[10] |= 8;
                            }
                            else
                            {
                                data[10] &= 0xF7;
                            }

                            if (v15 != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            var v9 = (byte)(data[9] >> 5);
                            data[9] *= 8;
                            data[9] |= v9;
                            data[11] ^= 0xA0;
                        }
                    }
                    else
                    {
                        data[5] ^= 0x35;
                        var v20 = (byte)((data[1] >> 1) & 1);
                        if (((data[1] >> 4) & 1) != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        if (v20 != 0)
                        {
                            data[1] |= 0x10;
                        }
                        else
                        {
                            data[1] &= 0xEF;
                        }

                        var v19 = (byte)((data[3] >> 2) & 1);
                        if (((data[3] >> 1) & 1) != 0)
                        {
                            data[3] |= 4;
                        }
                        else
                        {
                            data[3] &= 0xFB;
                        }

                        if (v19 != 0)
                        {
                            data[3] |= 2;
                        }
                        else
                        {
                            data[3] &= 0xFD;
                        }

                        var v3 = (byte)(data[2] >> 6);
                        data[2] *= 4;
                        data[2] |= v3;
                        data[7] ^= 0x95;
                        var v4 = (byte)(data[6] >> 1);
                        data[6] <<= 7;
                        data[6] |= v4;
                        var v5 = data[1];
                        data[1] = data[6];
                        data[6] = v5;
                        var v18 = (byte)((data[3] >> 4) & 1);
                        if (((data[3] >> 1) & 1) != 0)
                        {
                            data[3] |= 0x10;
                        }
                        else
                        {
                            data[3] &= 0xEF;
                        }

                        if (v18 != 0)
                        {
                            data[3] |= 2;
                        }
                        else
                        {
                            data[3] &= 0xFD;
                        }
                    }
                }
                else
                {
                    var v2 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 2) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    data[3] ^= 0x7F;
                    var v21 = (byte)((data[2] >> 5) & 1);
                    if (((data[2] >> 4) & 1) != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    if (v21 != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }
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
                            var v10 = data[6];
                            data[6] = data[11];
                            data[11] = v10;
                            data[10] ^= 0xC2;
                            var v11 = data[9];
                            data[9] = data[17];
                            data[17] = v11;
                            data[13] ^= 0xCA;
                            data[29] ^= 0x39;
                            var v12 = (byte)(data[0] >> 4);
                            data[0] *= 16;
                            data[0] |= v12;
                            var v13 = (byte)(data[31] >> 2);
                            data[31] <<= 6;
                            data[31] |= v13;
                            var v14 = data[27];
                            data[27] = data[7];
                            data[7] = v14;
                        }
                        else
                        {
                            data[11] ^= 0xA0;
                            var v6 = (byte)(data[9] >> 3);
                            data[9] *= 32;
                            data[9] |= v6;
                            var v17 = (byte)((data[10] >> 3) & 1);
                            if (((data[10] >> 4) & 1) != 0)
                            {
                                data[10] |= 8;
                            }
                            else
                            {
                                data[10] &= 0xF7;
                            }

                            if (v17 != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            var v7 = data[9];
                            data[9] = data[10];
                            data[10] = v7;
                            var v8 = (byte)(data[7] >> 3);
                            data[7] *= 32;
                            data[7] |= v8;
                            var v9 = data[6];
                            data[6] = data[3];
                            data[3] = v9;
                            data[12] ^= 0x2A;
                            var v16 = (byte)((data[11] >> 2) & 1);
                            if (((data[11] >> 7) & 1) != 0)
                            {
                                data[11] |= 4;
                            }
                            else
                            {
                                data[11] &= 0xFB;
                            }

                            if (v16 != 0)
                            {
                                data[11] |= 0x80;
                            }
                            else
                            {
                                data[11] &= 0x7F;
                            }

                            var v15 = (byte)((data[2] >> 3) & 1);
                            if (((data[2] >> 6) & 1) != 0)
                            {
                                data[2] |= 8;
                            }
                            else
                            {
                                data[2] &= 0xF7;
                            }

                            if (v15 != 0)
                            {
                                data[2] |= 0x40;
                            }
                            else
                            {
                                data[2] &= 0xBF;
                            }
                        }
                    }
                    else
                    {
                        var v20 = (byte)((data[3] >> 4) & 1);
                        if (((data[3] >> 1) & 1) != 0)
                        {
                            data[3] |= 0x10;
                        }
                        else
                        {
                            data[3] &= 0xEF;
                        }

                        if (v20 != 0)
                        {
                            data[3] |= 2;
                        }
                        else
                        {
                            data[3] &= 0xFD;
                        }

                        var v3 = data[1];
                        data[1] = data[6];
                        data[6] = v3;
                        var v4 = (byte)(data[6] >> 7);
                        data[6] *= 2;
                        data[6] |= v4;
                        data[7] ^= 0x95;
                        var v5 = (byte)(data[2] >> 2);
                        data[2] <<= 6;
                        data[2] |= v5;
                        var v19 = (byte)((data[3] >> 2) & 1);
                        if (((data[3] >> 1) & 1) != 0)
                        {
                            data[3] |= 4;
                        }
                        else
                        {
                            data[3] &= 0xFB;
                        }

                        if (v19 != 0)
                        {
                            data[3] |= 2;
                        }
                        else
                        {
                            data[3] &= 0xFD;
                        }

                        var v18 = (byte)((data[1] >> 1) & 1);
                        if (((data[1] >> 4) & 1) != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        if (v18 != 0)
                        {
                            data[1] |= 0x10;
                        }
                        else
                        {
                            data[1] &= 0xEF;
                        }

                        data[5] ^= 0x35;
                    }
                }
                else
                {
                    var v2 = (byte)((data[2] >> 5) & 1);
                    if (((data[2] >> 4) & 1) != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    data[3] ^= 0x7F;
                    var v21 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 2) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v21 != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }
                }
            }
        }
    }
}
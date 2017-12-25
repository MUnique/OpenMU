// <copyright file="PacketTwisterOfTradeCancel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'TradeCancel' type.
    /// </summary>
    internal class PacketTwisterOfTradeCancel : IPacketTwister
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
                            var v14 = (byte)(data[4] >> 5);
                            data[4] *= 8;
                            data[4] |= v14;
                            var v15 = data[31];
                            data[31] = data[9];
                            data[9] = v15;
                            var v16 = data[11];
                            data[11] = data[3];
                            data[3] = v16;
                            var v17 = (byte)((data[28] >> 2) & 1);
                            if (((data[28] >> 4) & 1) != 0)
                            {
                                data[28] |= 4;
                            }
                            else
                            {
                                data[28] &= 0xFB;
                            }

                            if (v17 != 0)
                            {
                                data[28] |= 0x10;
                            }
                            else
                            {
                                data[28] &= 0xEF;
                            }
                        }
                        else
                        {
                            var v22 = (byte)((data[2] >> 4) & 1);
                            if (((data[2] >> 1) & 1) != 0)
                            {
                                data[2] |= 0x10;
                            }
                            else
                            {
                                data[2] &= 0xEF;
                            }

                            if (v22 != 0)
                            {
                                data[2] |= 2;
                            }
                            else
                            {
                                data[2] &= 0xFD;
                            }

                            var v8 = (byte)(data[13] >> 1);
                            data[13] <<= 7;
                            data[13] |= v8;
                            var v21 = (byte)((data[14] >> 6) & 1);
                            if (((data[14] >> 3) & 1) != 0)
                            {
                                data[14] |= 0x40;
                            }
                            else
                            {
                                data[14] &= 0xBF;
                            }

                            if (v21 != 0)
                            {
                                data[14] |= 8;
                            }
                            else
                            {
                                data[14] &= 0xF7;
                            }

                            var v20 = (byte)((data[6] >> 2) & 1);
                            if (((data[6] >> 1) & 1) != 0)
                            {
                                data[6] |= 4;
                            }
                            else
                            {
                                data[6] &= 0xFB;
                            }

                            if (v20 != 0)
                            {
                                data[6] |= 2;
                            }
                            else
                            {
                                data[6] &= 0xFD;
                            }

                            var v9 = (byte)(data[1] >> 4);
                            data[1] *= 16;
                            data[1] |= v9;
                            var v19 = (byte)((data[15] >> 1) & 1);
                            if (((data[15] >> 4) & 1) != 0)
                            {
                                data[15] |= 2;
                            }
                            else
                            {
                                data[15] &= 0xFD;
                            }

                            if (v19 != 0)
                            {
                                data[15] |= 0x10;
                            }
                            else
                            {
                                data[15] &= 0xEF;
                            }

                            var v18 = (byte)((data[4] >> 2) & 1);
                            if (((data[4] >> 1) & 1) != 0)
                            {
                                data[4] |= 4;
                            }
                            else
                            {
                                data[4] &= 0xFB;
                            }

                            if (v18 != 0)
                            {
                                data[4] |= 2;
                            }
                            else
                            {
                                data[4] &= 0xFD;
                            }

                            var v10 = data[3];
                            data[3] = data[10];
                            data[10] = v10;
                            var v11 = data[4];
                            data[4] = data[7];
                            data[7] = v11;
                            var v12 = data[15];
                            data[15] = data[0];
                            data[0] = v12;
                            var v13 = data[3];
                            data[3] = data[5];
                            data[5] = v13;
                            data[11] ^= 0xD2;
                        }
                    }
                    else
                    {
                        var v6 = (byte)(data[4] >> 7);
                        data[4] *= 2;
                        data[4] |= v6;
                        var v26 = (byte)((data[3] >> 7) & 1);
                        if (((data[3] >> 2) & 1) != 0)
                        {
                            data[3] |= 0x80;
                        }
                        else
                        {
                            data[3] &= 0x7F;
                        }

                        if (v26 != 0)
                        {
                            data[3] |= 4;
                        }
                        else
                        {
                            data[3] &= 0xFB;
                        }

                        var v7 = (byte)(data[0] >> 7);
                        data[0] *= 2;
                        data[0] |= v7;
                        data[2] ^= 9;
                        var v25 = (byte)((data[5] >> 5) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        if (v25 != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        var v24 = (byte)((data[7] >> 6) & 1);
                        if (((data[7] >> 1) & 1) != 0)
                        {
                            data[7] |= 0x40;
                        }
                        else
                        {
                            data[7] &= 0xBF;
                        }

                        if (v24 != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        data[7] ^= 0x86;
                        data[4] ^= 0xB6;
                        data[7] ^= 0x32;
                        var v23 = (byte)((data[6] >> 6) & 1);
                        if (((data[6] >> 6) & 1) != 0)
                        {
                            data[6] |= 0x40;
                        }
                        else
                        {
                            data[6] &= 0xBF;
                        }

                        if (v23 != 0)
                        {
                            data[6] |= 0x40;
                        }
                        else
                        {
                            data[6] &= 0xBF;
                        }

                        data[0] ^= 0xD1;
                        data[2] ^= 0x33;
                    }
                }
                else
                {
                    var v3 = data[1];
                    data[1] = data[0];
                    data[0] = v3;
                    var v2 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 7) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    var v4 = (byte)(data[0] >> 5);
                    data[0] *= 8;
                    data[0] |= v4;
                    var v27 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 5) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v27 != 0)
                    {
                        data[3] |= 0x20;
                    }
                    else
                    {
                        data[3] &= 0xDF;
                    }

                    var v5 = data[0];
                    data[0] = data[2];
                    data[2] = v5;
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
                            var v17 = (byte)((data[28] >> 2) & 1);
                            if (((data[28] >> 4) & 1) != 0)
                            {
                                data[28] |= 4;
                            }
                            else
                            {
                                data[28] &= 0xFB;
                            }

                            if (v17 != 0)
                            {
                                data[28] |= 0x10;
                            }
                            else
                            {
                                data[28] &= 0xEF;
                            }

                            var v14 = data[11];
                            data[11] = data[3];
                            data[3] = v14;
                            var v15 = data[31];
                            data[31] = data[9];
                            data[9] = v15;
                            var v16 = (byte)(data[4] >> 3);
                            data[4] *= 32;
                            data[4] |= v16;
                        }
                        else
                        {
                            data[11] ^= 0xD2;
                            var v8 = data[3];
                            data[3] = data[5];
                            data[5] = v8;
                            var v9 = data[15];
                            data[15] = data[0];
                            data[0] = v9;
                            var v10 = data[4];
                            data[4] = data[7];
                            data[7] = v10;
                            var v11 = data[3];
                            data[3] = data[10];
                            data[10] = v11;
                            var v22 = (byte)((data[4] >> 2) & 1);
                            if (((data[4] >> 1) & 1) != 0)
                            {
                                data[4] |= 4;
                            }
                            else
                            {
                                data[4] &= 0xFB;
                            }

                            if (v22 != 0)
                            {
                                data[4] |= 2;
                            }
                            else
                            {
                                data[4] &= 0xFD;
                            }

                            var v21 = (byte)((data[15] >> 1) & 1);
                            if (((data[15] >> 4) & 1) != 0)
                            {
                                data[15] |= 2;
                            }
                            else
                            {
                                data[15] &= 0xFD;
                            }

                            if (v21 != 0)
                            {
                                data[15] |= 0x10;
                            }
                            else
                            {
                                data[15] &= 0xEF;
                            }

                            var v12 = (byte)(data[1] >> 4);
                            data[1] *= 16;
                            data[1] |= v12;
                            var v20 = (byte)((data[6] >> 2) & 1);
                            if (((data[6] >> 1) & 1) != 0)
                            {
                                data[6] |= 4;
                            }
                            else
                            {
                                data[6] &= 0xFB;
                            }

                            if (v20 != 0)
                            {
                                data[6] |= 2;
                            }
                            else
                            {
                                data[6] &= 0xFD;
                            }

                            var v19 = (byte)((data[14] >> 6) & 1);
                            if (((data[14] >> 3) & 1) != 0)
                            {
                                data[14] |= 0x40;
                            }
                            else
                            {
                                data[14] &= 0xBF;
                            }

                            if (v19 != 0)
                            {
                                data[14] |= 8;
                            }
                            else
                            {
                                data[14] &= 0xF7;
                            }

                            var v13 = (byte)(data[13] >> 7);
                            data[13] *= 2;
                            data[13] |= v13;
                            var v18 = (byte)((data[2] >> 4) & 1);
                            if (((data[2] >> 1) & 1) != 0)
                            {
                                data[2] |= 0x10;
                            }
                            else
                            {
                                data[2] &= 0xEF;
                            }

                            if (v18 != 0)
                            {
                                data[2] |= 2;
                            }
                            else
                            {
                                data[2] &= 0xFD;
                            }
                        }
                    }
                    else
                    {
                        data[2] ^= 0x33;
                        data[0] ^= 0xD1;
                        var v26 = (byte)((data[6] >> 6) & 1);
                        if (((data[6] >> 6) & 1) != 0)
                        {
                            data[6] |= 0x40;
                        }
                        else
                        {
                            data[6] &= 0xBF;
                        }

                        if (v26 != 0)
                        {
                            data[6] |= 0x40;
                        }
                        else
                        {
                            data[6] &= 0xBF;
                        }

                        data[7] ^= 0x32;
                        data[4] ^= 0xB6;
                        data[7] ^= 0x86;
                        var v25 = (byte)((data[7] >> 6) & 1);
                        if (((data[7] >> 1) & 1) != 0)
                        {
                            data[7] |= 0x40;
                        }
                        else
                        {
                            data[7] &= 0xBF;
                        }

                        if (v25 != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        var v24 = (byte)((data[5] >> 5) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        if (v24 != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        data[2] ^= 9;
                        var v6 = (byte)(data[0] >> 1);
                        data[0] <<= 7;
                        data[0] |= v6;
                        var v23 = (byte)((data[3] >> 7) & 1);
                        if (((data[3] >> 2) & 1) != 0)
                        {
                            data[3] |= 0x80;
                        }
                        else
                        {
                            data[3] &= 0x7F;
                        }

                        if (v23 != 0)
                        {
                            data[3] |= 4;
                        }
                        else
                        {
                            data[3] &= 0xFB;
                        }

                        var v7 = (byte)(data[4] >> 1);
                        data[4] <<= 7;
                        data[4] |= v7;
                    }
                }
                else
                {
                    var v3 = data[0];
                    data[0] = data[2];
                    data[2] = v3;
                    var v2 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 5) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 0x20;
                    }
                    else
                    {
                        data[3] &= 0xDF;
                    }

                    var v4 = (byte)(data[0] >> 3);
                    data[0] *= 32;
                    data[0] |= v4;
                    var v27 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 7) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v27 != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    var v5 = data[1];
                    data[1] = data[0];
                    data[0] = v5;
                }
            }
        }
    }
}
// <copyright file="PacketTwisterOfTradeButton.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'TradeButton' type.
    /// </summary>
    internal class PacketTwisterOfTradeButton : IPacketTwister
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
                            var v15 = (byte)(data[18] >> 5);
                            data[18] *= 8;
                            data[18] |= v15;
                            var v16 = (byte)(data[0] >> 6);
                            data[0] *= 4;
                            data[0] |= v16;
                            var v17 = data[2];
                            data[2] = data[25];
                            data[25] = v17;
                            var v18 = (byte)(data[19] >> 4);
                            data[19] *= 16;
                            data[19] |= v18;
                            var v19 = (byte)(data[19] >> 6);
                            data[19] *= 4;
                            data[19] |= v19;
                            var v20 = data[31];
                            data[31] = data[24];
                            data[24] = v20;
                            var v21 = (byte)(data[9] >> 6);
                            data[9] *= 4;
                            data[9] |= v21;
                            var v25 = (byte)((data[22] >> 1) & 1);
                            if (((data[22] >> 6) & 1) != 0)
                            {
                                data[22] |= 2;
                            }
                            else
                            {
                                data[22] &= 0xFD;
                            }

                            if (v25 != 0)
                            {
                                data[22] |= 0x40;
                            }
                            else
                            {
                                data[22] &= 0xBF;
                            }

                            var v22 = data[0];
                            data[0] = data[28];
                            data[28] = v22;
                            var v23 = data[19];
                            data[19] = data[25];
                            data[25] = v23;
                            data[23] ^= 0xA2;
                            var v24 = data[8];
                            data[8] = data[12];
                            data[12] = v24;
                        }
                        else
                        {
                            data[0] ^= 0x31;
                            var v11 = (byte)(data[12] >> 1);
                            data[12] <<= 7;
                            data[12] |= v11;
                            data[0] ^= 0xE8;
                            var v12 = data[1];
                            data[1] = data[15];
                            data[15] = v12;
                            var v30 = (byte)((data[8] >> 7) & 1);
                            if (((data[8] >> 2) & 1) != 0)
                            {
                                data[8] |= 0x80;
                            }
                            else
                            {
                                data[8] &= 0x7F;
                            }

                            if (v30 != 0)
                            {
                                data[8] |= 4;
                            }
                            else
                            {
                                data[8] &= 0xFB;
                            }

                            data[4] ^= 0x93;
                            var v29 = (byte)((data[10] >> 1) & 1);
                            if (((data[10] >> 6) & 1) != 0)
                            {
                                data[10] |= 2;
                            }
                            else
                            {
                                data[10] &= 0xFD;
                            }

                            if (v29 != 0)
                            {
                                data[10] |= 0x40;
                            }
                            else
                            {
                                data[10] &= 0xBF;
                            }

                            var v13 = (byte)(data[6] >> 6);
                            data[6] *= 4;
                            data[6] |= v13;
                            var v28 = (byte)((data[9] >> 6) & 1);
                            if (((data[9] >> 1) & 1) != 0)
                            {
                                data[9] |= 0x40;
                            }
                            else
                            {
                                data[9] &= 0xBF;
                            }

                            if (v28 != 0)
                            {
                                data[9] |= 2;
                            }
                            else
                            {
                                data[9] &= 0xFD;
                            }

                            var v14 = (byte)(data[5] >> 5);
                            data[5] *= 8;
                            data[5] |= v14;
                            var v27 = (byte)((data[10] >> 4) & 1);
                            if (((data[10] >> 4) & 1) != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            if (v27 != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            var v26 = (byte)((data[0] >> 1) & 1);
                            if (((data[0] >> 3) & 1) != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            if (v26 != 0)
                            {
                                data[0] |= 8;
                            }
                            else
                            {
                                data[0] &= 0xF7;
                            }
                        }
                    }
                    else
                    {
                        var v31 = (byte)((data[0] >> 6) & 1);
                        if (((data[0] >> 2) & 1) != 0)
                        {
                            data[0] |= 0x40;
                        }
                        else
                        {
                            data[0] &= 0xBF;
                        }

                        if (v31 != 0)
                        {
                            data[0] |= 4;
                        }
                        else
                        {
                            data[0] &= 0xFB;
                        }

                        data[3] ^= 0xFA;
                        var v9 = (byte)(data[4] >> 7);
                        data[4] *= 2;
                        data[4] |= v9;
                        var v10 = data[2];
                        data[2] = data[3];
                        data[3] = v10;
                    }
                }
                else
                {
                    var v3 = data[0];
                    data[0] = data[0];
                    data[0] = v3;
                    var v4 = data[1];
                    data[1] = data[3];
                    data[3] = v4;
                    var v2 = (byte)((data[1] >> 6) & 1);
                    if (((data[1] >> 4) & 1) != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    var v5 = data[2];
                    data[2] = data[1];
                    data[1] = v5;
                    var v6 = data[3];
                    data[3] = data[3];
                    data[3] = v6;
                    var v7 = data[3];
                    data[3] = data[3];
                    data[3] = v7;
                    data[3] ^= 0xF;
                    var v34 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v34 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    data[0] ^= 0xB1;
                    var v33 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 6) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v33 != 0)
                    {
                        data[2] |= 0x40;
                    }
                    else
                    {
                        data[2] &= 0xBF;
                    }

                    data[2] ^= 0x5C;
                    var v32 = (byte)((data[0] >> 1) & 1);
                    if (((data[0] >> 6) & 1) != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    if (v32 != 0)
                    {
                        data[0] |= 0x40;
                    }
                    else
                    {
                        data[0] &= 0xBF;
                    }

                    var v8 = data[3];
                    data[3] = data[2];
                    data[2] = v8;
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
                            var v15 = data[8];
                            data[8] = data[12];
                            data[12] = v15;
                            data[23] ^= 0xA2;
                            var v16 = data[19];
                            data[19] = data[25];
                            data[25] = v16;
                            var v17 = data[0];
                            data[0] = data[28];
                            data[28] = v17;
                            var v25 = (byte)((data[22] >> 1) & 1);
                            if (((data[22] >> 6) & 1) != 0)
                            {
                                data[22] |= 2;
                            }
                            else
                            {
                                data[22] &= 0xFD;
                            }

                            if (v25 != 0)
                            {
                                data[22] |= 0x40;
                            }
                            else
                            {
                                data[22] &= 0xBF;
                            }

                            var v18 = (byte)(data[9] >> 2);
                            data[9] <<= 6;
                            data[9] |= v18;
                            var v19 = data[31];
                            data[31] = data[24];
                            data[24] = v19;
                            var v20 = (byte)(data[19] >> 2);
                            data[19] <<= 6;
                            data[19] |= v20;
                            var v21 = (byte)(data[19] >> 4);
                            data[19] *= 16;
                            data[19] |= v21;
                            var v22 = data[2];
                            data[2] = data[25];
                            data[25] = v22;
                            var v23 = (byte)(data[0] >> 2);
                            data[0] <<= 6;
                            data[0] |= v23;
                            var v24 = (byte)(data[18] >> 3);
                            data[18] *= 32;
                            data[18] |= v24;
                        }
                        else
                        {
                            var v30 = (byte)((data[0] >> 1) & 1);
                            if (((data[0] >> 3) & 1) != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            if (v30 != 0)
                            {
                                data[0] |= 8;
                            }
                            else
                            {
                                data[0] &= 0xF7;
                            }

                            var v29 = (byte)((data[10] >> 4) & 1);
                            if (((data[10] >> 4) & 1) != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            if (v29 != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            var v11 = (byte)(data[5] >> 3);
                            data[5] *= 32;
                            data[5] |= v11;
                            var v28 = (byte)((data[9] >> 6) & 1);
                            if (((data[9] >> 1) & 1) != 0)
                            {
                                data[9] |= 0x40;
                            }
                            else
                            {
                                data[9] &= 0xBF;
                            }

                            if (v28 != 0)
                            {
                                data[9] |= 2;
                            }
                            else
                            {
                                data[9] &= 0xFD;
                            }

                            var v12 = (byte)(data[6] >> 2);
                            data[6] <<= 6;
                            data[6] |= v12;
                            var v27 = (byte)((data[10] >> 1) & 1);
                            if (((data[10] >> 6) & 1) != 0)
                            {
                                data[10] |= 2;
                            }
                            else
                            {
                                data[10] &= 0xFD;
                            }

                            if (v27 != 0)
                            {
                                data[10] |= 0x40;
                            }
                            else
                            {
                                data[10] &= 0xBF;
                            }

                            data[4] ^= 0x93;
                            var v26 = (byte)((data[8] >> 7) & 1);
                            if (((data[8] >> 2) & 1) != 0)
                            {
                                data[8] |= 0x80;
                            }
                            else
                            {
                                data[8] &= 0x7F;
                            }

                            if (v26 != 0)
                            {
                                data[8] |= 4;
                            }
                            else
                            {
                                data[8] &= 0xFB;
                            }

                            var v13 = data[1];
                            data[1] = data[15];
                            data[15] = v13;
                            data[0] ^= 0xE8;
                            var v14 = (byte)(data[12] >> 7);
                            data[12] *= 2;
                            data[12] |= v14;
                            data[0] ^= 0x31;
                        }
                    }
                    else
                    {
                        var v9 = data[2];
                        data[2] = data[3];
                        data[3] = v9;
                        var v10 = (byte)(data[4] >> 1);
                        data[4] <<= 7;
                        data[4] |= v10;
                        data[3] ^= 0xFA;
                        var v31 = (byte)((data[0] >> 6) & 1);
                        if (((data[0] >> 2) & 1) != 0)
                        {
                            data[0] |= 0x40;
                        }
                        else
                        {
                            data[0] &= 0xBF;
                        }

                        if (v31 != 0)
                        {
                            data[0] |= 4;
                        }
                        else
                        {
                            data[0] &= 0xFB;
                        }
                    }
                }
                else
                {
                    var v3 = data[3];
                    data[3] = data[2];
                    data[2] = v3;
                    var v2 = (byte)((data[0] >> 1) & 1);
                    if (((data[0] >> 6) & 1) != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 0x40;
                    }
                    else
                    {
                        data[0] &= 0xBF;
                    }

                    data[2] ^= 0x5C;
                    var v34 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 6) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v34 != 0)
                    {
                        data[2] |= 0x40;
                    }
                    else
                    {
                        data[2] &= 0xBF;
                    }

                    data[0] ^= 0xB1;
                    var v33 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v33 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    data[3] ^= 0xF;
                    var v4 = data[3];
                    data[3] = data[3];
                    data[3] = v4;
                    var v5 = data[3];
                    data[3] = data[3];
                    data[3] = v5;
                    var v6 = data[2];
                    data[2] = data[1];
                    data[1] = v6;
                    var v32 = (byte)((data[1] >> 6) & 1);
                    if (((data[1] >> 4) & 1) != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    if (v32 != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    var v7 = data[1];
                    data[1] = data[3];
                    data[3] = v7;
                    var v8 = data[0];
                    data[0] = data[0];
                    data[0] = v8;
                }
            }
        }
    }
}
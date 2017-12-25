// <copyright file="PacketTwisterOfTradeResponse.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'TradeResponse' type.
    /// </summary>
    internal class PacketTwisterOfTradeResponse : IPacketTwister
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
                            var v28 = (byte)((data[10] >> 4) & 1);
                            if (((data[10] >> 3) & 1) != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            if (v28 != 0)
                            {
                                data[10] |= 8;
                            }
                            else
                            {
                                data[10] &= 0xF7;
                            }

                            var v27 = (byte)((data[30] >> 4) & 1);
                            if (((data[30] >> 4) & 1) != 0)
                            {
                                data[30] |= 0x10;
                            }
                            else
                            {
                                data[30] &= 0xEF;
                            }

                            if (v27 != 0)
                            {
                                data[30] |= 0x10;
                            }
                            else
                            {
                                data[30] &= 0xEF;
                            }

                            var v26 = (byte)((data[22] >> 6) & 1);
                            if (((data[22] >> 4) & 1) != 0)
                            {
                                data[22] |= 0x40;
                            }
                            else
                            {
                                data[22] &= 0xBF;
                            }

                            if (v26 != 0)
                            {
                                data[22] |= 0x10;
                            }
                            else
                            {
                                data[22] &= 0xEF;
                            }

                            var v22 = data[15];
                            data[15] = data[4];
                            data[4] = v22;
                            data[1] ^= 0x43;
                            var v23 = (byte)(data[21] >> 5);
                            data[21] *= 8;
                            data[21] |= v23;
                            var v24 = data[14];
                            data[14] = data[0];
                            data[0] = v24;
                            data[20] ^= 0xF1;
                            var v25 = data[26];
                            data[26] = data[1];
                            data[1] = v25;
                        }
                        else
                        {
                            var v13 = data[6];
                            data[6] = data[10];
                            data[10] = v13;
                            var v14 = (byte)(data[10] >> 5);
                            data[10] *= 8;
                            data[10] |= v14;
                            var v15 = data[4];
                            data[4] = data[3];
                            data[3] = v15;
                            data[11] ^= 0x7D;
                            var v16 = data[5];
                            data[5] = data[7];
                            data[7] = v16;
                            data[13] ^= 0x5A;
                            var v17 = (byte)(data[10] >> 6);
                            data[10] *= 4;
                            data[10] |= v17;
                            var v18 = (byte)(data[1] >> 6);
                            data[1] *= 4;
                            data[1] |= v18;
                            var v29 = (byte)((data[0] >> 7) & 1);
                            if (((data[0] >> 5) & 1) != 0)
                            {
                                data[0] |= 0x80;
                            }
                            else
                            {
                                data[0] &= 0x7F;
                            }

                            if (v29 != 0)
                            {
                                data[0] |= 0x20;
                            }
                            else
                            {
                                data[0] &= 0xDF;
                            }

                            data[2] ^= 0x15;
                            var v19 = (byte)(data[1] >> 7);
                            data[1] *= 2;
                            data[1] |= v19;
                            var v20 = (byte)(data[4] >> 6);
                            data[4] *= 4;
                            data[4] |= v20;
                            var v21 = (byte)(data[3] >> 6);
                            data[3] *= 4;
                            data[3] |= v21;
                        }
                    }
                    else
                    {
                        data[3] ^= 0x60;
                        var v8 = data[4];
                        data[4] = data[1];
                        data[1] = v8;
                        var v9 = data[2];
                        data[2] = data[0];
                        data[0] = v9;
                        var v10 = data[0];
                        data[0] = data[3];
                        data[3] = v10;
                        var v11 = (byte)(data[6] >> 1);
                        data[6] <<= 7;
                        data[6] |= v11;
                        var v12 = (byte)(data[4] >> 1);
                        data[4] <<= 7;
                        data[4] |= v12;
                        var v31 = (byte)((data[0] >> 6) & 1);
                        if (((data[0] >> 7) & 1) != 0)
                        {
                            data[0] |= 0x40;
                        }
                        else
                        {
                            data[0] &= 0xBF;
                        }

                        if (v31 != 0)
                        {
                            data[0] |= 0x80;
                        }
                        else
                        {
                            data[0] &= 0x7F;
                        }

                        data[4] ^= 0xE5;
                        var v30 = (byte)((data[2] >> 2) & 1);
                        if (((data[2] >> 1) & 1) != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        if (v30 != 0)
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
                    data[0] ^= 0xCC;
                    var v2 = (byte)((data[1] >> 7) & 1);
                    if (((data[1] >> 3) & 1) != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    var v3 = data[1];
                    data[1] = data[2];
                    data[2] = v3;
                    var v4 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v4;
                    var v5 = data[1];
                    data[1] = data[1];
                    data[1] = v5;
                    var v6 = (byte)(data[1] >> 4);
                    data[1] *= 16;
                    data[1] |= v6;
                    var v7 = data[2];
                    data[2] = data[0];
                    data[0] = v7;
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
                            var v22 = data[26];
                            data[26] = data[1];
                            data[1] = v22;
                            data[20] ^= 0xF1;
                            var v23 = data[14];
                            data[14] = data[0];
                            data[0] = v23;
                            var v24 = (byte)(data[21] >> 3);
                            data[21] *= 32;
                            data[21] |= v24;
                            data[1] ^= 0x43;
                            var v25 = data[15];
                            data[15] = data[4];
                            data[4] = v25;
                            var v28 = (byte)((data[22] >> 6) & 1);
                            if (((data[22] >> 4) & 1) != 0)
                            {
                                data[22] |= 0x40;
                            }
                            else
                            {
                                data[22] &= 0xBF;
                            }

                            if (v28 != 0)
                            {
                                data[22] |= 0x10;
                            }
                            else
                            {
                                data[22] &= 0xEF;
                            }

                            var v27 = (byte)((data[30] >> 4) & 1);
                            if (((data[30] >> 4) & 1) != 0)
                            {
                                data[30] |= 0x10;
                            }
                            else
                            {
                                data[30] &= 0xEF;
                            }

                            if (v27 != 0)
                            {
                                data[30] |= 0x10;
                            }
                            else
                            {
                                data[30] &= 0xEF;
                            }

                            var v26 = (byte)((data[10] >> 4) & 1);
                            if (((data[10] >> 3) & 1) != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            if (v26 != 0)
                            {
                                data[10] |= 8;
                            }
                            else
                            {
                                data[10] &= 0xF7;
                            }
                        }
                        else
                        {
                            var v13 = (byte)(data[3] >> 2);
                            data[3] <<= 6;
                            data[3] |= v13;
                            var v14 = (byte)(data[4] >> 2);
                            data[4] <<= 6;
                            data[4] |= v14;
                            var v15 = (byte)(data[1] >> 1);
                            data[1] <<= 7;
                            data[1] |= v15;
                            data[2] ^= 0x15;
                            var v29 = (byte)((data[0] >> 7) & 1);
                            if (((data[0] >> 5) & 1) != 0)
                            {
                                data[0] |= 0x80;
                            }
                            else
                            {
                                data[0] &= 0x7F;
                            }

                            if (v29 != 0)
                            {
                                data[0] |= 0x20;
                            }
                            else
                            {
                                data[0] &= 0xDF;
                            }

                            var v16 = (byte)(data[1] >> 2);
                            data[1] <<= 6;
                            data[1] |= v16;
                            var v17 = (byte)(data[10] >> 2);
                            data[10] <<= 6;
                            data[10] |= v17;
                            data[13] ^= 0x5A;
                            var v18 = data[5];
                            data[5] = data[7];
                            data[7] = v18;
                            data[11] ^= 0x7D;
                            var v19 = data[4];
                            data[4] = data[3];
                            data[3] = v19;
                            var v20 = (byte)(data[10] >> 3);
                            data[10] *= 32;
                            data[10] |= v20;
                            var v21 = data[6];
                            data[6] = data[10];
                            data[10] = v21;
                        }
                    }
                    else
                    {
                        var v31 = (byte)((data[2] >> 2) & 1);
                        if (((data[2] >> 1) & 1) != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        if (v31 != 0)
                        {
                            data[2] |= 2;
                        }
                        else
                        {
                            data[2] &= 0xFD;
                        }

                        data[4] ^= 0xE5;
                        var v30 = (byte)((data[0] >> 6) & 1);
                        if (((data[0] >> 7) & 1) != 0)
                        {
                            data[0] |= 0x40;
                        }
                        else
                        {
                            data[0] &= 0xBF;
                        }

                        if (v30 != 0)
                        {
                            data[0] |= 0x80;
                        }
                        else
                        {
                            data[0] &= 0x7F;
                        }

                        var v8 = (byte)(data[4] >> 7);
                        data[4] *= 2;
                        data[4] |= v8;
                        var v9 = (byte)(data[6] >> 7);
                        data[6] *= 2;
                        data[6] |= v9;
                        var v10 = data[0];
                        data[0] = data[3];
                        data[3] = v10;
                        var v11 = data[2];
                        data[2] = data[0];
                        data[0] = v11;
                        var v12 = data[4];
                        data[4] = data[1];
                        data[1] = v12;
                        data[3] ^= 0x60;
                    }
                }
                else
                {
                    var v3 = data[2];
                    data[2] = data[0];
                    data[0] = v3;
                    var v4 = (byte)(data[1] >> 4);
                    data[1] *= 16;
                    data[1] |= v4;
                    var v5 = data[1];
                    data[1] = data[1];
                    data[1] = v5;
                    var v6 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v6;
                    var v7 = data[1];
                    data[1] = data[2];
                    data[2] = v7;
                    var v2 = (byte)((data[1] >> 7) & 1);
                    if (((data[1] >> 3) & 1) != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    data[0] ^= 0xCC;
                }
            }
        }
    }
}
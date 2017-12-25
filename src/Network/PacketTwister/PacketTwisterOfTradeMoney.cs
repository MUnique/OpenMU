// <copyright file="PacketTwisterOfTradeMoney.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'TradeMoney' type.
    /// </summary>
    internal class PacketTwisterOfTradeMoney : IPacketTwister
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
                            data[26] ^= 0xE4;
                            var v22 = (byte)((data[9] >> 2) & 1);
                            if (((data[9] >> 3) & 1) != 0)
                            {
                                data[9] |= 4;
                            }
                            else
                            {
                                data[9] &= 0xFB;
                            }

                            if (v22 != 0)
                            {
                                data[9] |= 8;
                            }
                            else
                            {
                                data[9] &= 0xF7;
                            }

                            var v21 = (byte)((data[12] >> 2) & 1);
                            if (((data[12] >> 3) & 1) != 0)
                            {
                                data[12] |= 4;
                            }
                            else
                            {
                                data[12] &= 0xFB;
                            }

                            if (v21 != 0)
                            {
                                data[12] |= 8;
                            }
                            else
                            {
                                data[12] &= 0xF7;
                            }

                            var v20 = (byte)((data[9] >> 5) & 1);
                            if (((data[9] >> 4) & 1) != 0)
                            {
                                data[9] |= 0x20;
                            }
                            else
                            {
                                data[9] &= 0xDF;
                            }

                            if (v20 != 0)
                            {
                                data[9] |= 0x10;
                            }
                            else
                            {
                                data[9] &= 0xEF;
                            }

                            var v13 = (byte)(data[30] >> 1);
                            data[30] <<= 7;
                            data[30] |= v13;
                            var v14 = (byte)(data[27] >> 3);
                            data[27] *= 32;
                            data[27] |= v14;
                            var v15 = data[30];
                            data[30] = data[29];
                            data[29] = v15;
                            var v16 = data[28];
                            data[28] = data[18];
                            data[18] = v16;
                            var v17 = (byte)(data[21] >> 3);
                            data[21] *= 32;
                            data[21] |= v17;
                            var v19 = (byte)((data[9] >> 6) & 1);
                            if (((data[9] >> 7) & 1) != 0)
                            {
                                data[9] |= 0x40;
                            }
                            else
                            {
                                data[9] &= 0xBF;
                            }

                            if (v19 != 0)
                            {
                                data[9] |= 0x80;
                            }
                            else
                            {
                                data[9] &= 0x7F;
                            }

                            var v18 = (byte)((data[11] >> 4) & 1);
                            if (((data[11] >> 5) & 1) != 0)
                            {
                                data[11] |= 0x10;
                            }
                            else
                            {
                                data[11] &= 0xEF;
                            }

                            if (v18 != 0)
                            {
                                data[11] |= 0x20;
                            }
                            else
                            {
                                data[11] &= 0xDF;
                            }
                        }
                        else
                        {
                            data[13] ^= 0xD5;
                            data[8] ^= 0x30;
                            var v12 = (byte)(data[12] >> 4);
                            data[12] *= 16;
                            data[12] |= v12;
                        }
                    }
                    else
                    {
                        data[1] ^= 0xB9;
                        data[6] ^= 0xEE;
                        var v23 = (byte)((data[0] >> 2) & 1);
                        if (((data[0] >> 2) & 1) != 0)
                        {
                            data[0] |= 4;
                        }
                        else
                        {
                            data[0] &= 0xFB;
                        }

                        if (v23 != 0)
                        {
                            data[0] |= 4;
                        }
                        else
                        {
                            data[0] &= 0xFB;
                        }

                        data[5] ^= 0xC4;
                        var v11 = data[6];
                        data[6] = data[5];
                        data[5] = v11;
                        data[7] ^= 0x60;
                    }
                }
                else
                {
                    var v3 = data[0];
                    data[0] = data[1];
                    data[1] = v3;
                    var v4 = data[2];
                    data[2] = data[0];
                    data[0] = v4;
                    var v5 = (byte)(data[3] >> 4);
                    data[3] *= 16;
                    data[3] |= v5;
                    var v6 = (byte)(data[1] >> 5);
                    data[1] *= 8;
                    data[1] |= v6;
                    var v2 = (byte)((data[3] >> 5) & 1);
                    if (((data[3] >> 7) & 1) != 0)
                    {
                        data[3] |= 0x20;
                    }
                    else
                    {
                        data[3] &= 0xDF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 0x80;
                    }
                    else
                    {
                        data[3] &= 0x7F;
                    }

                    var v7 = data[0];
                    data[0] = data[2];
                    data[2] = v7;
                    var v8 = data[0];
                    data[0] = data[2];
                    data[2] = v8;
                    data[2] ^= 3;
                    var v9 = data[1];
                    data[1] = data[0];
                    data[0] = v9;
                    var v24 = (byte)((data[1] >> 6) & 1);
                    if (((data[1] >> 6) & 1) != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    if (v24 != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    var v10 = (byte)(data[0] >> 4);
                    data[0] *= 16;
                    data[0] |= v10;
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
                            var v22 = (byte)((data[11] >> 4) & 1);
                            if (((data[11] >> 5) & 1) != 0)
                            {
                                data[11] |= 0x10;
                            }
                            else
                            {
                                data[11] &= 0xEF;
                            }

                            if (v22 != 0)
                            {
                                data[11] |= 0x20;
                            }
                            else
                            {
                                data[11] &= 0xDF;
                            }

                            var v21 = (byte)((data[9] >> 6) & 1);
                            if (((data[9] >> 7) & 1) != 0)
                            {
                                data[9] |= 0x40;
                            }
                            else
                            {
                                data[9] &= 0xBF;
                            }

                            if (v21 != 0)
                            {
                                data[9] |= 0x80;
                            }
                            else
                            {
                                data[9] &= 0x7F;
                            }

                            var v13 = (byte)(data[21] >> 5);
                            data[21] *= 8;
                            data[21] |= v13;
                            var v14 = data[28];
                            data[28] = data[18];
                            data[18] = v14;
                            var v15 = data[30];
                            data[30] = data[29];
                            data[29] = v15;
                            var v16 = (byte)(data[27] >> 5);
                            data[27] *= 8;
                            data[27] |= v16;
                            var v17 = (byte)(data[30] >> 7);
                            data[30] *= 2;
                            data[30] |= v17;
                            var v20 = (byte)((data[9] >> 5) & 1);
                            if (((data[9] >> 4) & 1) != 0)
                            {
                                data[9] |= 0x20;
                            }
                            else
                            {
                                data[9] &= 0xDF;
                            }

                            if (v20 != 0)
                            {
                                data[9] |= 0x10;
                            }
                            else
                            {
                                data[9] &= 0xEF;
                            }

                            var v19 = (byte)((data[12] >> 2) & 1);
                            if (((data[12] >> 3) & 1) != 0)
                            {
                                data[12] |= 4;
                            }
                            else
                            {
                                data[12] &= 0xFB;
                            }

                            if (v19 != 0)
                            {
                                data[12] |= 8;
                            }
                            else
                            {
                                data[12] &= 0xF7;
                            }

                            var v18 = (byte)((data[9] >> 2) & 1);
                            if (((data[9] >> 3) & 1) != 0)
                            {
                                data[9] |= 4;
                            }
                            else
                            {
                                data[9] &= 0xFB;
                            }

                            if (v18 != 0)
                            {
                                data[9] |= 8;
                            }
                            else
                            {
                                data[9] &= 0xF7;
                            }

                            data[26] ^= 0xE4;
                        }
                        else
                        {
                            var v12 = (byte)(data[12] >> 4);
                            data[12] *= 16;
                            data[12] |= v12;
                            data[8] ^= 0x30;
                            data[13] ^= 0xD5;
                        }
                    }
                    else
                    {
                        data[7] ^= 0x60;
                        var v11 = data[6];
                        data[6] = data[5];
                        data[5] = v11;
                        data[5] ^= 0xC4;
                        var v23 = (byte)((data[0] >> 2) & 1);
                        if (((data[0] >> 2) & 1) != 0)
                        {
                            data[0] |= 4;
                        }
                        else
                        {
                            data[0] &= 0xFB;
                        }

                        if (v23 != 0)
                        {
                            data[0] |= 4;
                        }
                        else
                        {
                            data[0] &= 0xFB;
                        }

                        data[6] ^= 0xEE;
                        data[1] ^= 0xB9;
                    }
                }
                else
                {
                    var v3 = (byte)(data[0] >> 4);
                    data[0] *= 16;
                    data[0] |= v3;
                    var v2 = (byte)((data[1] >> 6) & 1);
                    if (((data[1] >> 6) & 1) != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    var v4 = data[1];
                    data[1] = data[0];
                    data[0] = v4;
                    data[2] ^= 3;
                    var v5 = data[0];
                    data[0] = data[2];
                    data[2] = v5;
                    var v6 = data[0];
                    data[0] = data[2];
                    data[2] = v6;
                    var v24 = (byte)((data[3] >> 5) & 1);
                    if (((data[3] >> 7) & 1) != 0)
                    {
                        data[3] |= 0x20;
                    }
                    else
                    {
                        data[3] &= 0xDF;
                    }

                    if (v24 != 0)
                    {
                        data[3] |= 0x80;
                    }
                    else
                    {
                        data[3] &= 0x7F;
                    }

                    var v7 = (byte)(data[1] >> 3);
                    data[1] *= 32;
                    data[1] |= v7;
                    var v8 = (byte)(data[3] >> 4);
                    data[3] *= 16;
                    data[3] |= v8;
                    var v9 = data[2];
                    data[2] = data[0];
                    data[0] = v9;
                    var v10 = data[0];
                    data[0] = data[1];
                    data[1] = v10;
                }
            }
        }
    }
}
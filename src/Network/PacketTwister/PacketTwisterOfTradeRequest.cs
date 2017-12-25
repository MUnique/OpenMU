// <copyright file="PacketTwisterOfTradeRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'TradeRequest' type.
    /// </summary>
    internal class PacketTwisterOfTradeRequest : IPacketTwister
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
                            var v16 = (byte)((data[11] >> 1) & 1);
                            if (((data[11] >> 4) & 1) != 0)
                            {
                                data[11] |= 2;
                            }
                            else
                            {
                                data[11] &= 0xFD;
                            }

                            if (v16 != 0)
                            {
                                data[11] |= 0x10;
                            }
                            else
                            {
                                data[11] &= 0xEF;
                            }

                            data[13] ^= 0x35;
                            var v15 = (byte)((data[22] >> 6) & 1);
                            if (((data[22] >> 4) & 1) != 0)
                            {
                                data[22] |= 0x40;
                            }
                            else
                            {
                                data[22] &= 0xBF;
                            }

                            if (v15 != 0)
                            {
                                data[22] |= 0x10;
                            }
                            else
                            {
                                data[22] &= 0xEF;
                            }

                            var v12 = (byte)(data[31] >> 1);
                            data[31] <<= 7;
                            data[31] |= v12;
                            var v14 = (byte)((data[2] >> 4) & 1);
                            if (((data[2] >> 5) & 1) != 0)
                            {
                                data[2] |= 0x10;
                            }
                            else
                            {
                                data[2] &= 0xEF;
                            }

                            if (v14 != 0)
                            {
                                data[2] |= 0x20;
                            }
                            else
                            {
                                data[2] &= 0xDF;
                            }

                            var v13 = data[20];
                            data[20] = data[19];
                            data[19] = v13;
                        }
                        else
                        {
                            var v8 = (byte)(data[1] >> 6);
                            data[1] *= 4;
                            data[1] |= v8;
                            var v9 = (byte)(data[15] >> 6);
                            data[15] *= 4;
                            data[15] |= v9;
                            data[4] ^= 0xA5;
                            var v10 = (byte)(data[5] >> 3);
                            data[5] *= 32;
                            data[5] |= v10;
                            var v17 = (byte)((data[9] >> 1) & 1);
                            if (((data[9] >> 2) & 1) != 0)
                            {
                                data[9] |= 2;
                            }
                            else
                            {
                                data[9] &= 0xFD;
                            }

                            if (v17 != 0)
                            {
                                data[9] |= 4;
                            }
                            else
                            {
                                data[9] &= 0xFB;
                            }

                            var v11 = data[9];
                            data[9] = data[9];
                            data[9] = v11;
                        }
                    }
                    else
                    {
                        var v4 = data[3];
                        data[3] = data[1];
                        data[1] = v4;
                        data[7] ^= 0x4D;
                        var v5 = (byte)(data[6] >> 1);
                        data[6] <<= 7;
                        data[6] |= v5;
                        var v6 = data[4];
                        data[4] = data[1];
                        data[1] = v6;
                        data[3] ^= 0xCA;
                        var v7 = (byte)(data[5] >> 2);
                        data[5] <<= 6;
                        data[5] |= v7;
                        data[6] ^= 0x98;
                        var v18 = (byte)((data[7] >> 6) & 1);
                        if (((data[7] >> 6) & 1) != 0)
                        {
                            data[7] |= 0x40;
                        }
                        else
                        {
                            data[7] &= 0xBF;
                        }

                        if (v18 != 0)
                        {
                            data[7] |= 0x40;
                        }
                        else
                        {
                            data[7] &= 0xBF;
                        }
                    }
                }
                else
                {
                    var v3 = (byte)(data[2] >> 3);
                    data[2] *= 32;
                    data[2] |= v3;
                    data[3] ^= 0x48;
                    var v2 = (byte)((data[2] >> 1) & 1);
                    if (((data[2] >> 5) & 1) != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
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
                            var v12 = data[20];
                            data[20] = data[19];
                            data[19] = v12;
                            var v16 = (byte)((data[2] >> 4) & 1);
                            if (((data[2] >> 5) & 1) != 0)
                            {
                                data[2] |= 0x10;
                            }
                            else
                            {
                                data[2] &= 0xEF;
                            }

                            if (v16 != 0)
                            {
                                data[2] |= 0x20;
                            }
                            else
                            {
                                data[2] &= 0xDF;
                            }

                            var v13 = (byte)(data[31] >> 7);
                            data[31] *= 2;
                            data[31] |= v13;
                            var v15 = (byte)((data[22] >> 6) & 1);
                            if (((data[22] >> 4) & 1) != 0)
                            {
                                data[22] |= 0x40;
                            }
                            else
                            {
                                data[22] &= 0xBF;
                            }

                            if (v15 != 0)
                            {
                                data[22] |= 0x10;
                            }
                            else
                            {
                                data[22] &= 0xEF;
                            }

                            data[13] ^= 0x35;
                            var v14 = (byte)((data[11] >> 1) & 1);
                            if (((data[11] >> 4) & 1) != 0)
                            {
                                data[11] |= 2;
                            }
                            else
                            {
                                data[11] &= 0xFD;
                            }

                            if (v14 != 0)
                            {
                                data[11] |= 0x10;
                            }
                            else
                            {
                                data[11] &= 0xEF;
                            }
                        }
                        else
                        {
                            var v8 = data[9];
                            data[9] = data[9];
                            data[9] = v8;
                            var v17 = (byte)((data[9] >> 1) & 1);
                            if (((data[9] >> 2) & 1) != 0)
                            {
                                data[9] |= 2;
                            }
                            else
                            {
                                data[9] &= 0xFD;
                            }

                            if (v17 != 0)
                            {
                                data[9] |= 4;
                            }
                            else
                            {
                                data[9] &= 0xFB;
                            }

                            var v9 = (byte)(data[5] >> 5);
                            data[5] *= 8;
                            data[5] |= v9;
                            data[4] ^= 0xA5;
                            var v10 = (byte)(data[15] >> 2);
                            data[15] <<= 6;
                            data[15] |= v10;
                            var v11 = (byte)(data[1] >> 2);
                            data[1] <<= 6;
                            data[1] |= v11;
                        }
                    }
                    else
                    {
                        var v18 = (byte)((data[7] >> 6) & 1);
                        if (((data[7] >> 6) & 1) != 0)
                        {
                            data[7] |= 0x40;
                        }
                        else
                        {
                            data[7] &= 0xBF;
                        }

                        if (v18 != 0)
                        {
                            data[7] |= 0x40;
                        }
                        else
                        {
                            data[7] &= 0xBF;
                        }

                        data[6] ^= 0x98;
                        var v4 = (byte)(data[5] >> 6);
                        data[5] *= 4;
                        data[5] |= v4;
                        data[3] ^= 0xCA;
                        var v5 = data[4];
                        data[4] = data[1];
                        data[1] = v5;
                        var v6 = (byte)(data[6] >> 7);
                        data[6] *= 2;
                        data[6] |= v6;
                        data[7] ^= 0x4D;
                        var v7 = data[3];
                        data[3] = data[1];
                        data[1] = v7;
                    }
                }
                else
                {
                    var v2 = (byte)((data[2] >> 1) & 1);
                    if (((data[2] >> 5) & 1) != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    data[3] ^= 0x48;
                    var v3 = (byte)(data[2] >> 5);
                    data[2] *= 8;
                    data[2] |= v3;
                }
            }
        }
    }
}
// <copyright file="PacketTwisterOfCashShop.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'CashShop' type.
    /// </summary>
    internal class PacketTwisterOfCashShop : IPacketTwister
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
                            data[21] ^= 0x1A;
                            var v17 = (byte)((data[13] >> 1) & 1);
                            if (((data[13] >> 7) & 1) != 0)
                            {
                                data[13] |= 2;
                            }
                            else
                            {
                                data[13] &= 0xFD;
                            }

                            if (v17 != 0)
                            {
                                data[13] |= 0x80;
                            }
                            else
                            {
                                data[13] &= 0x7F;
                            }

                            var v11 = data[4];
                            data[4] = data[13];
                            data[13] = v11;
                            var v16 = (byte)((data[27] >> 1) & 1);
                            if (((data[27] >> 4) & 1) != 0)
                            {
                                data[27] |= 2;
                            }
                            else
                            {
                                data[27] &= 0xFD;
                            }

                            if (v16 != 0)
                            {
                                data[27] |= 0x10;
                            }
                            else
                            {
                                data[27] &= 0xEF;
                            }

                            var v12 = data[28];
                            data[28] = data[6];
                            data[6] = v12;
                            var v13 = (byte)(data[27] >> 1);
                            data[27] <<= 7;
                            data[27] |= v13;
                            data[13] ^= 0x77;
                            var v14 = (byte)(data[29] >> 2);
                            data[29] <<= 6;
                            data[29] |= v14;
                            var v15 = (byte)((data[26] >> 2) & 1);
                            if (((data[26] >> 4) & 1) != 0)
                            {
                                data[26] |= 4;
                            }
                            else
                            {
                                data[26] &= 0xFB;
                            }

                            if (v15 != 0)
                            {
                                data[26] |= 0x10;
                            }
                            else
                            {
                                data[26] &= 0xEF;
                            }
                        }
                        else
                        {
                            var v8 = (byte)(data[1] >> 6);
                            data[1] *= 4;
                            data[1] |= v8;
                            data[13] ^= 0x5D;
                            var v9 = data[0];
                            data[0] = data[10];
                            data[10] = v9;
                            var v10 = (byte)(data[6] >> 7);
                            data[6] *= 2;
                            data[6] |= v10;
                        }
                    }
                    else
                    {
                        var v5 = data[3];
                        data[3] = data[1];
                        data[1] = v5;
                        var v6 = (byte)(data[3] >> 3);
                        data[3] *= 32;
                        data[3] |= v6;
                        var v7 = data[6];
                        data[6] = data[7];
                        data[7] = v7;
                    }
                }
                else
                {
                    var v2 = (byte)((data[2] >> 4) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    var v3 = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= v3;
                    data[1] ^= 0x9F;
                    var v4 = (byte)(data[1] >> 1);
                    data[1] <<= 7;
                    data[1] |= v4;
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
                            var v17 = (byte)((data[26] >> 2) & 1);
                            if (((data[26] >> 4) & 1) != 0)
                            {
                                data[26] |= 4;
                            }
                            else
                            {
                                data[26] &= 0xFB;
                            }

                            if (v17 != 0)
                            {
                                data[26] |= 0x10;
                            }
                            else
                            {
                                data[26] &= 0xEF;
                            }

                            var v11 = (byte)(data[29] >> 6);
                            data[29] *= 4;
                            data[29] |= v11;
                            data[13] ^= 0x77;
                            var v12 = (byte)(data[27] >> 7);
                            data[27] *= 2;
                            data[27] |= v12;
                            var v13 = data[28];
                            data[28] = data[6];
                            data[6] = v13;
                            var v16 = (byte)((data[27] >> 1) & 1);
                            if (((data[27] >> 4) & 1) != 0)
                            {
                                data[27] |= 2;
                            }
                            else
                            {
                                data[27] &= 0xFD;
                            }

                            if (v16 != 0)
                            {
                                data[27] |= 0x10;
                            }
                            else
                            {
                                data[27] &= 0xEF;
                            }

                            var v14 = data[4];
                            data[4] = data[13];
                            data[13] = v14;
                            var v15 = (byte)((data[13] >> 1) & 1);
                            if (((data[13] >> 7) & 1) != 0)
                            {
                                data[13] |= 2;
                            }
                            else
                            {
                                data[13] &= 0xFD;
                            }

                            if (v15 != 0)
                            {
                                data[13] |= 0x80;
                            }
                            else
                            {
                                data[13] &= 0x7F;
                            }

                            data[21] ^= 0x1A;
                        }
                        else
                        {
                            var v8 = (byte)(data[6] >> 1);
                            data[6] <<= 7;
                            data[6] |= v8;
                            var v9 = data[0];
                            data[0] = data[10];
                            data[10] = v9;
                            data[13] ^= 0x5D;
                            var v10 = (byte)(data[1] >> 2);
                            data[1] <<= 6;
                            data[1] |= v10;
                        }
                    }
                    else
                    {
                        var v5 = data[6];
                        data[6] = data[7];
                        data[7] = v5;
                        var v6 = (byte)(data[3] >> 5);
                        data[3] *= 8;
                        data[3] |= v6;
                        var v7 = data[3];
                        data[3] = data[1];
                        data[1] = v7;
                    }
                }
                else
                {
                    var v3 = (byte)(data[1] >> 7);
                    data[1] *= 2;
                    data[1] |= v3;
                    data[1] ^= 0x9F;
                    var v4 = (byte)(data[2] >> 2);
                    data[2] <<= 6;
                    data[2] |= v4;
                    var v2 = (byte)((data[2] >> 4) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }
                }
            }
        }
    }
}
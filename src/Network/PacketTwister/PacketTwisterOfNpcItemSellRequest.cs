// <copyright file="PacketTwisterOfNpcItemSellRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'NpcItemSellRequest' type.
    /// </summary>
    internal class PacketTwisterOfNpcItemSellRequest : IPacketTwister
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
                            var v17 = data[10];
                            data[10] = data[4];
                            data[4] = v17;
                            var v18 = (byte)(data[16] >> 1);
                            data[16] <<= 7;
                            data[16] |= v18;
                            data[15] ^= 0x6B;
                            data[23] ^= 0x1C;
                            var v19 = data[14];
                            data[14] = data[7];
                            data[7] = v19;
                            var v20 = data[20];
                            data[20] = data[14];
                            data[14] = v20;
                            var v21 = (byte)((data[6] >> 2) & 1);
                            if (((data[6] >> 2) & 1) != 0)
                            {
                                data[6] |= 4;
                            }
                            else
                            {
                                data[6] &= 0xFB;
                            }

                            if (v21 != 0)
                            {
                                data[6] |= 4;
                            }
                            else
                            {
                                data[6] &= 0xFB;
                            }

                            data[12] ^= 0xF9;
                        }
                        else
                        {
                            var v12 = (byte)(data[15] >> 6);
                            data[15] *= 4;
                            data[15] |= v12;
                            data[3] ^= 0x90;
                            var v23 = (byte)((data[13] >> 5) & 1);
                            if (((data[13] >> 6) & 1) != 0)
                            {
                                data[13] |= 0x20;
                            }
                            else
                            {
                                data[13] &= 0xDF;
                            }

                            if (v23 != 0)
                            {
                                data[13] |= 0x40;
                            }
                            else
                            {
                                data[13] &= 0xBF;
                            }

                            var v13 = data[10];
                            data[10] = data[9];
                            data[9] = v13;
                            var v14 = data[12];
                            data[12] = data[12];
                            data[12] = v14;
                            var v15 = (byte)(data[9] >> 4);
                            data[9] *= 16;
                            data[9] |= v15;
                            data[0] ^= 0x58;
                            var v22 = (byte)((data[5] >> 2) & 1);
                            if (((data[5] >> 4) & 1) != 0)
                            {
                                data[5] |= 4;
                            }
                            else
                            {
                                data[5] &= 0xFB;
                            }

                            if (v22 != 0)
                            {
                                data[5] |= 0x10;
                            }
                            else
                            {
                                data[5] &= 0xEF;
                            }

                            var v16 = data[3];
                            data[3] = data[2];
                            data[2] = v16;
                        }
                    }
                    else
                    {
                        var v10 = (byte)(data[5] >> 7);
                        data[5] *= 2;
                        data[5] |= v10;
                        data[2] ^= 0x67;
                        var v11 = (byte)(data[4] >> 2);
                        data[4] <<= 6;
                        data[4] |= v11;
                        data[3] ^= 0xA;
                        data[1] ^= 0xCD;
                    }
                }
                else
                {
                    var v3 = data[3];
                    data[3] = data[1];
                    data[1] = v3;
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

                    var v4 = (byte)(data[3] >> 5);
                    data[3] *= 8;
                    data[3] |= v4;
                    var v5 = data[0];
                    data[0] = data[3];
                    data[3] = v5;
                    var v6 = data[0];
                    data[0] = data[2];
                    data[2] = v6;
                    var v7 = (byte)(data[3] >> 2);
                    data[3] <<= 6;
                    data[3] |= v7;
                    var v24 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 7) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v24 != 0)
                    {
                        data[2] |= 0x80;
                    }
                    else
                    {
                        data[2] &= 0x7F;
                    }

                    data[1] ^= 0x16;
                    var v8 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v8;
                    data[3] ^= 0x23;
                    data[2] ^= 0x6F;
                    var v9 = (byte)(data[0] >> 5);
                    data[0] *= 8;
                    data[0] |= v9;
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
                            data[12] ^= 0xF9;
                            var v21 = (byte)((data[6] >> 2) & 1);
                            if (((data[6] >> 2) & 1) != 0)
                            {
                                data[6] |= 4;
                            }
                            else
                            {
                                data[6] &= 0xFB;
                            }

                            if (v21 != 0)
                            {
                                data[6] |= 4;
                            }
                            else
                            {
                                data[6] &= 0xFB;
                            }

                            var v17 = data[20];
                            data[20] = data[14];
                            data[14] = v17;
                            var v18 = data[14];
                            data[14] = data[7];
                            data[7] = v18;
                            data[23] ^= 0x1C;
                            data[15] ^= 0x6B;
                            var v19 = (byte)(data[16] >> 7);
                            data[16] *= 2;
                            data[16] |= v19;
                            var v20 = data[10];
                            data[10] = data[4];
                            data[4] = v20;
                        }
                        else
                        {
                            var v12 = data[3];
                            data[3] = data[2];
                            data[2] = v12;
                            var v23 = (byte)((data[5] >> 2) & 1);
                            if (((data[5] >> 4) & 1) != 0)
                            {
                                data[5] |= 4;
                            }
                            else
                            {
                                data[5] &= 0xFB;
                            }

                            if (v23 != 0)
                            {
                                data[5] |= 0x10;
                            }
                            else
                            {
                                data[5] &= 0xEF;
                            }

                            data[0] ^= 0x58;
                            var v13 = (byte)(data[9] >> 4);
                            data[9] *= 16;
                            data[9] |= v13;
                            var v14 = data[12];
                            data[12] = data[12];
                            data[12] = v14;
                            var v15 = data[10];
                            data[10] = data[9];
                            data[9] = v15;
                            var v22 = (byte)((data[13] >> 5) & 1);
                            if (((data[13] >> 6) & 1) != 0)
                            {
                                data[13] |= 0x20;
                            }
                            else
                            {
                                data[13] &= 0xDF;
                            }

                            if (v22 != 0)
                            {
                                data[13] |= 0x40;
                            }
                            else
                            {
                                data[13] &= 0xBF;
                            }

                            data[3] ^= 0x90;
                            var v16 = (byte)(data[15] >> 2);
                            data[15] <<= 6;
                            data[15] |= v16;
                        }
                    }
                    else
                    {
                        data[1] ^= 0xCD;
                        data[3] ^= 0xA;
                        var v10 = (byte)(data[4] >> 6);
                        data[4] *= 4;
                        data[4] |= v10;
                        data[2] ^= 0x67;
                        var v11 = (byte)(data[5] >> 1);
                        data[5] <<= 7;
                        data[5] |= v11;
                    }
                }
                else
                {
                    var v3 = (byte)(data[0] >> 3);
                    data[0] *= 32;
                    data[0] |= v3;
                    data[2] ^= 0x6F;
                    data[3] ^= 0x23;
                    var v4 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v4;
                    data[1] ^= 0x16;
                    var v2 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 7) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 0x80;
                    }
                    else
                    {
                        data[2] &= 0x7F;
                    }

                    var v5 = (byte)(data[3] >> 6);
                    data[3] *= 4;
                    data[3] |= v5;
                    var v6 = data[0];
                    data[0] = data[2];
                    data[2] = v6;
                    var v7 = data[0];
                    data[0] = data[3];
                    data[3] = v7;
                    var v8 = (byte)(data[3] >> 3);
                    data[3] *= 32;
                    data[3] |= v8;
                    var v24 = (byte)((data[1] >> 1) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    if (v24 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    var v9 = data[3];
                    data[3] = data[1];
                    data[1] = v9;
                }
            }
        }
    }
}
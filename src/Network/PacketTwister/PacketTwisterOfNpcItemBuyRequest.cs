// <copyright file="PacketTwisterOfNpcItemBuyRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'NpcItemBuyRequest' type.
    /// </summary>
    internal class PacketTwisterOfNpcItemBuyRequest : IPacketTwister
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
                            data[14] ^= 0x60;
                            var v22 = (byte)((data[23] >> 3) & 1);
                            if (((data[23] >> 2) & 1) != 0)
                            {
                                data[23] |= 8;
                            }
                            else
                            {
                                data[23] &= 0xF7;
                            }

                            if (v22 != 0)
                            {
                                data[23] |= 4;
                            }
                            else
                            {
                                data[23] &= 0xFB;
                            }

                            var v21 = (byte)((data[20] >> 4) & 1);
                            if (((data[20] >> 3) & 1) != 0)
                            {
                                data[20] |= 0x10;
                            }
                            else
                            {
                                data[20] &= 0xEF;
                            }

                            if (v21 != 0)
                            {
                                data[20] |= 8;
                            }
                            else
                            {
                                data[20] &= 0xF7;
                            }

                            var v17 = (byte)(data[25] >> 1);
                            data[25] <<= 7;
                            data[25] |= v17;
                            data[17] ^= 0xC9;
                            data[0] ^= 0x98;
                            var v18 = data[15];
                            data[15] = data[24];
                            data[24] = v18;
                            var v19 = (byte)(data[18] >> 3);
                            data[18] *= 32;
                            data[18] |= v19;
                            var v20 = data[26];
                            data[26] = data[2];
                            data[2] = v20;
                        }
                        else
                        {
                            var v10 = data[15];
                            data[15] = data[8];
                            data[8] = v10;
                            var v11 = (byte)(data[15] >> 6);
                            data[15] *= 4;
                            data[15] |= v11;
                            data[1] ^= 0x3A;
                            var v24 = (byte)((data[3] >> 4) & 1);
                            if (((data[3] >> 2) & 1) != 0)
                            {
                                data[3] |= 0x10;
                            }
                            else
                            {
                                data[3] &= 0xEF;
                            }

                            if (v24 != 0)
                            {
                                data[3] |= 4;
                            }
                            else
                            {
                                data[3] &= 0xFB;
                            }

                            var v12 = (byte)(data[0] >> 2);
                            data[0] <<= 6;
                            data[0] |= v12;
                            var v13 = (byte)(data[6] >> 6);
                            data[6] *= 4;
                            data[6] |= v13;
                            var v14 = (byte)(data[0] >> 6);
                            data[0] *= 4;
                            data[0] |= v14;
                            var v15 = (byte)(data[15] >> 3);
                            data[15] *= 32;
                            data[15] |= v15;
                            var v16 = data[9];
                            data[9] = data[2];
                            data[2] = v16;
                            var v23 = (byte)((data[3] >> 7) & 1);
                            if (((data[3] >> 1) & 1) != 0)
                            {
                                data[3] |= 0x80;
                            }
                            else
                            {
                                data[3] &= 0x7F;
                            }

                            if (v23 != 0)
                            {
                                data[3] |= 2;
                            }
                            else
                            {
                                data[3] &= 0xFD;
                            }

                            data[2] ^= 0x95;
                        }
                    }
                    else
                    {
                        data[1] ^= 0x60;
                        data[5] ^= 0xB4;
                        data[1] ^= 0x76;
                        var v26 = (byte)((data[4] >> 5) & 1);
                        if (((data[4] >> 5) & 1) != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        if (v26 != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        data[5] ^= 0x9B;
                        var v6 = data[6];
                        data[6] = data[6];
                        data[6] = v6;
                        var v7 = data[1];
                        data[1] = data[6];
                        data[6] = v7;
                        var v25 = (byte)((data[1] >> 1) & 1);
                        if (((data[1] >> 6) & 1) != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        if (v25 != 0)
                        {
                            data[1] |= 0x40;
                        }
                        else
                        {
                            data[1] &= 0xBF;
                        }

                        var v8 = data[0];
                        data[0] = data[1];
                        data[1] = v8;
                        data[0] ^= 0xD8;
                        var v9 = data[2];
                        data[2] = data[6];
                        data[6] = v9;
                    }
                }
                else
                {
                    var v3 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v3;
                    data[2] ^= 0xA7;
                    var v4 = data[0];
                    data[0] = data[1];
                    data[1] = v4;
                    var v2 = (byte)((data[2] >> 3) & 1);
                    if (((data[2] >> 3) & 1) != 0)
                    {
                        data[2] |= 8;
                    }
                    else
                    {
                        data[2] &= 0xF7;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 8;
                    }
                    else
                    {
                        data[2] &= 0xF7;
                    }

                    var v5 = data[3];
                    data[3] = data[3];
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
                            var v17 = data[26];
                            data[26] = data[2];
                            data[2] = v17;
                            var v18 = (byte)(data[18] >> 5);
                            data[18] *= 8;
                            data[18] |= v18;
                            var v19 = data[15];
                            data[15] = data[24];
                            data[24] = v19;
                            data[0] ^= 0x98;
                            data[17] ^= 0xC9;
                            var v20 = (byte)(data[25] >> 7);
                            data[25] *= 2;
                            data[25] |= v20;
                            var v22 = (byte)((data[20] >> 4) & 1);
                            if (((data[20] >> 3) & 1) != 0)
                            {
                                data[20] |= 0x10;
                            }
                            else
                            {
                                data[20] &= 0xEF;
                            }

                            if (v22 != 0)
                            {
                                data[20] |= 8;
                            }
                            else
                            {
                                data[20] &= 0xF7;
                            }

                            var v21 = (byte)((data[23] >> 3) & 1);
                            if (((data[23] >> 2) & 1) != 0)
                            {
                                data[23] |= 8;
                            }
                            else
                            {
                                data[23] &= 0xF7;
                            }

                            if (v21 != 0)
                            {
                                data[23] |= 4;
                            }
                            else
                            {
                                data[23] &= 0xFB;
                            }

                            data[14] ^= 0x60;
                        }
                        else
                        {
                            data[2] ^= 0x95;
                            var v24 = (byte)((data[3] >> 7) & 1);
                            if (((data[3] >> 1) & 1) != 0)
                            {
                                data[3] |= 0x80;
                            }
                            else
                            {
                                data[3] &= 0x7F;
                            }

                            if (v24 != 0)
                            {
                                data[3] |= 2;
                            }
                            else
                            {
                                data[3] &= 0xFD;
                            }

                            var v10 = data[9];
                            data[9] = data[2];
                            data[2] = v10;
                            var v11 = (byte)(data[15] >> 5);
                            data[15] *= 8;
                            data[15] |= v11;
                            var v12 = (byte)(data[0] >> 2);
                            data[0] <<= 6;
                            data[0] |= v12;
                            var v13 = (byte)(data[6] >> 2);
                            data[6] <<= 6;
                            data[6] |= v13;
                            var v14 = (byte)(data[0] >> 6);
                            data[0] *= 4;
                            data[0] |= v14;
                            var v23 = (byte)((data[3] >> 4) & 1);
                            if (((data[3] >> 2) & 1) != 0)
                            {
                                data[3] |= 0x10;
                            }
                            else
                            {
                                data[3] &= 0xEF;
                            }

                            if (v23 != 0)
                            {
                                data[3] |= 4;
                            }
                            else
                            {
                                data[3] &= 0xFB;
                            }

                            data[1] ^= 0x3A;
                            var v15 = (byte)(data[15] >> 2);
                            data[15] <<= 6;
                            data[15] |= v15;
                            var v16 = data[15];
                            data[15] = data[8];
                            data[8] = v16;
                        }
                    }
                    else
                    {
                        var v6 = data[2];
                        data[2] = data[6];
                        data[6] = v6;
                        data[0] ^= 0xD8;
                        var v7 = data[0];
                        data[0] = data[1];
                        data[1] = v7;
                        var v26 = (byte)((data[1] >> 1) & 1);
                        if (((data[1] >> 6) & 1) != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        if (v26 != 0)
                        {
                            data[1] |= 0x40;
                        }
                        else
                        {
                            data[1] &= 0xBF;
                        }

                        var v8 = data[1];
                        data[1] = data[6];
                        data[6] = v8;
                        var v9 = data[6];
                        data[6] = data[6];
                        data[6] = v9;
                        data[5] ^= 0x9B;
                        var v25 = (byte)((data[4] >> 5) & 1);
                        if (((data[4] >> 5) & 1) != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        if (v25 != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        data[1] ^= 0x76;
                        data[5] ^= 0xB4;
                        data[1] ^= 0x60;
                    }
                }
                else
                {
                    var v3 = data[3];
                    data[3] = data[3];
                    data[3] = v3;
                    var v2 = (byte)((data[2] >> 3) & 1);
                    if (((data[2] >> 3) & 1) != 0)
                    {
                        data[2] |= 8;
                    }
                    else
                    {
                        data[2] &= 0xF7;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 8;
                    }
                    else
                    {
                        data[2] &= 0xF7;
                    }

                    var v4 = data[0];
                    data[0] = data[1];
                    data[1] = v4;
                    data[2] ^= 0xA7;
                    var v5 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v5;
                }
            }
        }
    }
}
// <copyright file="PacketTwisterOfCastleSiege.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'CastleSiege' type.
    /// </summary>
    internal class PacketTwisterOfCastleSiege : IPacketTwister
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
                            var v19 = (byte)((data[10] >> 4) & 1);
                            if (((data[10] >> 4) & 1) != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            if (v19 != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            var v18 = (byte)((data[16] >> 2) & 1);
                            if (((data[16] >> 7) & 1) != 0)
                            {
                                data[16] |= 4;
                            }
                            else
                            {
                                data[16] &= 0xFB;
                            }

                            if (v18 != 0)
                            {
                                data[16] |= 0x80;
                            }
                            else
                            {
                                data[16] &= 0x7F;
                            }

                            var v14 = (byte)(data[19] >> 3);
                            data[19] *= 32;
                            data[19] |= v14;
                            data[22] ^= 0xD4;
                            var v17 = (byte)((data[11] >> 1) & 1);
                            if (((data[11] >> 1) & 1) != 0)
                            {
                                data[11] |= 2;
                            }
                            else
                            {
                                data[11] &= 0xFD;
                            }

                            if (v17 != 0)
                            {
                                data[11] |= 2;
                            }
                            else
                            {
                                data[11] &= 0xFD;
                            }

                            var v16 = (byte)((data[7] >> 7) & 1);
                            if (((data[7] >> 3) & 1) != 0)
                            {
                                data[7] |= 0x80;
                            }
                            else
                            {
                                data[7] &= 0x7F;
                            }

                            if (v16 != 0)
                            {
                                data[7] |= 8;
                            }
                            else
                            {
                                data[7] &= 0xF7;
                            }

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
                        }
                        else
                        {
                            var v12 = (byte)(data[3] >> 3);
                            data[3] *= 32;
                            data[3] |= v12;
                            data[0] ^= 0x62;
                            var v24 = (byte)((data[0] >> 2) & 1);
                            if (((data[0] >> 4) & 1) != 0)
                            {
                                data[0] |= 4;
                            }
                            else
                            {
                                data[0] &= 0xFB;
                            }

                            if (v24 != 0)
                            {
                                data[0] |= 0x10;
                            }
                            else
                            {
                                data[0] &= 0xEF;
                            }

                            var v23 = (byte)((data[5] >> 5) & 1);
                            if (((data[5] >> 2) & 1) != 0)
                            {
                                data[5] |= 0x20;
                            }
                            else
                            {
                                data[5] &= 0xDF;
                            }

                            if (v23 != 0)
                            {
                                data[5] |= 4;
                            }
                            else
                            {
                                data[5] &= 0xFB;
                            }

                            var v22 = (byte)((data[9] >> 2) & 1);
                            if (((data[9] >> 1) & 1) != 0)
                            {
                                data[9] |= 4;
                            }
                            else
                            {
                                data[9] &= 0xFB;
                            }

                            if (v22 != 0)
                            {
                                data[9] |= 2;
                            }
                            else
                            {
                                data[9] &= 0xFD;
                            }

                            var v13 = data[7];
                            data[7] = data[5];
                            data[5] = v13;
                            var v21 = (byte)((data[4] >> 6) & 1);
                            if (((data[4] >> 2) & 1) != 0)
                            {
                                data[4] |= 0x40;
                            }
                            else
                            {
                                data[4] &= 0xBF;
                            }

                            if (v21 != 0)
                            {
                                data[4] |= 4;
                            }
                            else
                            {
                                data[4] &= 0xFB;
                            }

                            var v20 = (byte)((data[12] >> 6) & 1);
                            if (((data[12] >> 1) & 1) != 0)
                            {
                                data[12] |= 0x40;
                            }
                            else
                            {
                                data[12] &= 0xBF;
                            }

                            if (v20 != 0)
                            {
                                data[12] |= 2;
                            }
                            else
                            {
                                data[12] &= 0xFD;
                            }
                        }
                    }
                    else
                    {
                        data[4] ^= 0x3B;
                        var v27 = (byte)((data[5] >> 6) & 1);
                        if (((data[5] >> 5) & 1) != 0)
                        {
                            data[5] |= 0x40;
                        }
                        else
                        {
                            data[5] &= 0xBF;
                        }

                        if (v27 != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        data[4] ^= 0x1D;
                        var v5 = data[3];
                        data[3] = data[5];
                        data[5] = v5;
                        var v26 = (byte)((data[5] >> 5) & 1);
                        if (((data[5] >> 3) & 1) != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        if (v26 != 0)
                        {
                            data[5] |= 8;
                        }
                        else
                        {
                            data[5] &= 0xF7;
                        }

                        var v6 = (byte)(data[1] >> 1);
                        data[1] <<= 7;
                        data[1] |= v6;
                        var v7 = data[6];
                        data[6] = data[1];
                        data[1] = v7;
                        data[0] ^= 0x7C;
                        var v8 = (byte)(data[5] >> 5);
                        data[5] *= 8;
                        data[5] |= v8;
                        var v9 = (byte)(data[3] >> 6);
                        data[3] *= 4;
                        data[3] |= v9;
                        var v10 = data[1];
                        data[1] = data[6];
                        data[6] = v10;
                        var v11 = data[3];
                        data[3] = data[1];
                        data[1] = v11;
                        var v25 = (byte)((data[6] >> 2) & 1);
                        if (((data[6] >> 7) & 1) != 0)
                        {
                            data[6] |= 4;
                        }
                        else
                        {
                            data[6] &= 0xFB;
                        }

                        if (v25 != 0)
                        {
                            data[6] |= 0x80;
                        }
                        else
                        {
                            data[6] &= 0x7F;
                        }
                    }
                }
                else
                {
                    var v3 = (byte)(data[1] >> 7);
                    data[1] *= 2;
                    data[1] |= v3;
                    data[3] ^= 0xCA;
                    var v2 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 6) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    var v28 = (byte)((data[3] >> 5) & 1);
                    if (((data[3] >> 5) & 1) != 0)
                    {
                        data[3] |= 0x20;
                    }
                    else
                    {
                        data[3] &= 0xDF;
                    }

                    if (v28 != 0)
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
                    data[1] ^= 0x1A;
                    data[2] ^= 0x20;
                    data[3] ^= 0x3C;
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
                            var v19 = (byte)((data[22] >> 6) & 1);
                            if (((data[22] >> 4) & 1) != 0)
                            {
                                data[22] |= 0x40;
                            }
                            else
                            {
                                data[22] &= 0xBF;
                            }

                            if (v19 != 0)
                            {
                                data[22] |= 0x10;
                            }
                            else
                            {
                                data[22] &= 0xEF;
                            }

                            var v18 = (byte)((data[7] >> 7) & 1);
                            if (((data[7] >> 3) & 1) != 0)
                            {
                                data[7] |= 0x80;
                            }
                            else
                            {
                                data[7] &= 0x7F;
                            }

                            if (v18 != 0)
                            {
                                data[7] |= 8;
                            }
                            else
                            {
                                data[7] &= 0xF7;
                            }

                            var v17 = (byte)((data[11] >> 1) & 1);
                            if (((data[11] >> 1) & 1) != 0)
                            {
                                data[11] |= 2;
                            }
                            else
                            {
                                data[11] &= 0xFD;
                            }

                            if (v17 != 0)
                            {
                                data[11] |= 2;
                            }
                            else
                            {
                                data[11] &= 0xFD;
                            }

                            data[22] ^= 0xD4;
                            var v14 = (byte)(data[19] >> 5);
                            data[19] *= 8;
                            data[19] |= v14;
                            var v16 = (byte)((data[16] >> 2) & 1);
                            if (((data[16] >> 7) & 1) != 0)
                            {
                                data[16] |= 4;
                            }
                            else
                            {
                                data[16] &= 0xFB;
                            }

                            if (v16 != 0)
                            {
                                data[16] |= 0x80;
                            }
                            else
                            {
                                data[16] &= 0x7F;
                            }

                            var v15 = (byte)((data[10] >> 4) & 1);
                            if (((data[10] >> 4) & 1) != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            if (v15 != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }
                        }
                        else
                        {
                            var v24 = (byte)((data[12] >> 6) & 1);
                            if (((data[12] >> 1) & 1) != 0)
                            {
                                data[12] |= 0x40;
                            }
                            else
                            {
                                data[12] &= 0xBF;
                            }

                            if (v24 != 0)
                            {
                                data[12] |= 2;
                            }
                            else
                            {
                                data[12] &= 0xFD;
                            }

                            var v23 = (byte)((data[4] >> 6) & 1);
                            if (((data[4] >> 2) & 1) != 0)
                            {
                                data[4] |= 0x40;
                            }
                            else
                            {
                                data[4] &= 0xBF;
                            }

                            if (v23 != 0)
                            {
                                data[4] |= 4;
                            }
                            else
                            {
                                data[4] &= 0xFB;
                            }

                            var v12 = data[7];
                            data[7] = data[5];
                            data[5] = v12;
                            var v22 = (byte)((data[9] >> 2) & 1);
                            if (((data[9] >> 1) & 1) != 0)
                            {
                                data[9] |= 4;
                            }
                            else
                            {
                                data[9] &= 0xFB;
                            }

                            if (v22 != 0)
                            {
                                data[9] |= 2;
                            }
                            else
                            {
                                data[9] &= 0xFD;
                            }

                            var v21 = (byte)((data[5] >> 5) & 1);
                            if (((data[5] >> 2) & 1) != 0)
                            {
                                data[5] |= 0x20;
                            }
                            else
                            {
                                data[5] &= 0xDF;
                            }

                            if (v21 != 0)
                            {
                                data[5] |= 4;
                            }
                            else
                            {
                                data[5] &= 0xFB;
                            }

                            var v20 = (byte)((data[0] >> 2) & 1);
                            if (((data[0] >> 4) & 1) != 0)
                            {
                                data[0] |= 4;
                            }
                            else
                            {
                                data[0] &= 0xFB;
                            }

                            if (v20 != 0)
                            {
                                data[0] |= 0x10;
                            }
                            else
                            {
                                data[0] &= 0xEF;
                            }

                            data[0] ^= 0x62;
                            var v13 = (byte)(data[3] >> 5);
                            data[3] *= 8;
                            data[3] |= v13;
                        }
                    }
                    else
                    {
                        var v27 = (byte)((data[6] >> 2) & 1);
                        if (((data[6] >> 7) & 1) != 0)
                        {
                            data[6] |= 4;
                        }
                        else
                        {
                            data[6] &= 0xFB;
                        }

                        if (v27 != 0)
                        {
                            data[6] |= 0x80;
                        }
                        else
                        {
                            data[6] &= 0x7F;
                        }

                        var v5 = data[3];
                        data[3] = data[1];
                        data[1] = v5;
                        var v6 = data[1];
                        data[1] = data[6];
                        data[6] = v6;
                        var v7 = (byte)(data[3] >> 2);
                        data[3] <<= 6;
                        data[3] |= v7;
                        var v8 = (byte)(data[5] >> 3);
                        data[5] *= 32;
                        data[5] |= v8;
                        data[0] ^= 0x7C;
                        var v9 = data[6];
                        data[6] = data[1];
                        data[1] = v9;
                        var v10 = (byte)(data[1] >> 7);
                        data[1] *= 2;
                        data[1] |= v10;
                        var v26 = (byte)((data[5] >> 5) & 1);
                        if (((data[5] >> 3) & 1) != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        if (v26 != 0)
                        {
                            data[5] |= 8;
                        }
                        else
                        {
                            data[5] &= 0xF7;
                        }

                        var v11 = data[3];
                        data[3] = data[5];
                        data[5] = v11;
                        data[4] ^= 0x1D;
                        var v25 = (byte)((data[5] >> 6) & 1);
                        if (((data[5] >> 5) & 1) != 0)
                        {
                            data[5] |= 0x40;
                        }
                        else
                        {
                            data[5] &= 0xBF;
                        }

                        if (v25 != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        data[4] ^= 0x3B;
                    }
                }
                else
                {
                    data[3] ^= 0x3C;
                    data[2] ^= 0x20;
                    data[1] ^= 0x1A;
                    var v3 = (byte)(data[0] >> 5);
                    data[0] *= 8;
                    data[0] |= v3;
                    var v2 = (byte)((data[3] >> 5) & 1);
                    if (((data[3] >> 5) & 1) != 0)
                    {
                        data[3] |= 0x20;
                    }
                    else
                    {
                        data[3] &= 0xDF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 0x20;
                    }
                    else
                    {
                        data[3] &= 0xDF;
                    }

                    var v28 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 6) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v28 != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    data[3] ^= 0xCA;
                    var v4 = (byte)(data[1] >> 1);
                    data[1] <<= 7;
                    data[1] |= v4;
                }
            }
        }
    }
}
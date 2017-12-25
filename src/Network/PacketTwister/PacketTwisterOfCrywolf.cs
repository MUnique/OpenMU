// <copyright file="PacketTwisterOfCrywolf.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'Crywolf' type.
    /// </summary>
    internal class PacketTwisterOfCrywolf : IPacketTwister
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
                            data[24] ^= 0x16;
                            data[27] ^= 0x19;
                            var v16 = (byte)(data[11] >> 4);
                            data[11] *= 16;
                            data[11] |= v16;
                            data[24] ^= 0x4F;
                            var v17 = (byte)(data[0] >> 2);
                            data[0] <<= 6;
                            data[0] |= v17;
                            var v21 = (byte)((data[25] >> 4) & 1);
                            if (((data[25] >> 5) & 1) != 0)
                            {
                                data[25] |= 0x10;
                            }
                            else
                            {
                                data[25] &= 0xEF;
                            }

                            if (v21 != 0)
                            {
                                data[25] |= 0x20;
                            }
                            else
                            {
                                data[25] &= 0xDF;
                            }

                            var v20 = (byte)((data[30] >> 1) & 1);
                            if (((data[30] >> 4) & 1) != 0)
                            {
                                data[30] |= 2;
                            }
                            else
                            {
                                data[30] &= 0xFD;
                            }

                            if (v20 != 0)
                            {
                                data[30] |= 0x10;
                            }
                            else
                            {
                                data[30] &= 0xEF;
                            }

                            data[3] ^= 0x5C;
                            var v19 = (byte)((data[22] >> 2) & 1);
                            if (((data[22] >> 5) & 1) != 0)
                            {
                                data[22] |= 4;
                            }
                            else
                            {
                                data[22] &= 0xFB;
                            }

                            if (v19 != 0)
                            {
                                data[22] |= 0x20;
                            }
                            else
                            {
                                data[22] &= 0xDF;
                            }

                            var v18 = (byte)((data[23] >> 7) & 1);
                            if (((data[23] >> 6) & 1) != 0)
                            {
                                data[23] |= 0x80;
                            }
                            else
                            {
                                data[23] &= 0x7F;
                            }

                            if (v18 != 0)
                            {
                                data[23] |= 0x40;
                            }
                            else
                            {
                                data[23] &= 0xBF;
                            }

                            data[13] ^= 0x8C;
                        }
                        else
                        {
                            var v9 = data[7];
                            data[7] = data[13];
                            data[13] = v9;
                            var v10 = (byte)(data[14] >> 6);
                            data[14] *= 4;
                            data[14] |= v10;
                            var v11 = data[4];
                            data[4] = data[9];
                            data[9] = v11;
                            data[0] ^= 0x9B;
                            data[11] ^= 0xCA;
                            data[6] ^= 0x8F;
                            var v12 = (byte)(data[2] >> 7);
                            data[2] *= 2;
                            data[2] |= v12;
                            var v13 = data[11];
                            data[11] = data[6];
                            data[6] = v13;
                            var v14 = (byte)(data[13] >> 4);
                            data[13] *= 16;
                            data[13] |= v14;
                            var v15 = (byte)(data[11] >> 5);
                            data[11] *= 8;
                            data[11] |= v15;
                        }
                    }
                    else
                    {
                        var v6 = (byte)(data[7] >> 4);
                        data[7] *= 16;
                        data[7] |= v6;
                        data[1] ^= 0x9D;
                        var v26 = (byte)((data[0] >> 6) & 1);
                        if (((data[0] >> 1) & 1) != 0)
                        {
                            data[0] |= 0x40;
                        }
                        else
                        {
                            data[0] &= 0xBF;
                        }

                        if (v26 != 0)
                        {
                            data[0] |= 2;
                        }
                        else
                        {
                            data[0] &= 0xFD;
                        }

                        var v25 = (byte)((data[1] >> 7) & 1);
                        if (((data[1] >> 4) & 1) != 0)
                        {
                            data[1] |= 0x80;
                        }
                        else
                        {
                            data[1] &= 0x7F;
                        }

                        if (v25 != 0)
                        {
                            data[1] |= 0x10;
                        }
                        else
                        {
                            data[1] &= 0xEF;
                        }

                        var v24 = (byte)((data[1] >> 6) & 1);
                        if (((data[1] >> 2) & 1) != 0)
                        {
                            data[1] |= 0x40;
                        }
                        else
                        {
                            data[1] &= 0xBF;
                        }

                        if (v24 != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        var v23 = (byte)((data[5] >> 4) & 1);
                        if (((data[5] >> 4) & 1) != 0)
                        {
                            data[5] |= 0x10;
                        }
                        else
                        {
                            data[5] &= 0xEF;
                        }

                        if (v23 != 0)
                        {
                            data[5] |= 0x10;
                        }
                        else
                        {
                            data[5] &= 0xEF;
                        }

                        var v7 = (byte)(data[2] >> 4);
                        data[2] *= 16;
                        data[2] |= v7;
                        data[7] ^= 0xD;
                        var v8 = (byte)(data[2] >> 5);
                        data[2] *= 8;
                        data[2] |= v8;
                        var v22 = (byte)((data[5] >> 3) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 8;
                        }
                        else
                        {
                            data[5] &= 0xF7;
                        }

                        if (v22 != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }
                    }
                }
                else
                {
                    var v3 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v3;
                    data[0] ^= 0xC5;
                    data[3] ^= 0xFA;
                    var v2 = (byte)((data[2] >> 7) & 1);
                    if (((data[2] >> 5) & 1) != 0)
                    {
                        data[2] |= 0x80;
                    }
                    else
                    {
                        data[2] &= 0x7F;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    var v4 = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= v4;
                    var v29 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 4) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v29 != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    data[3] ^= 0xF7;
                    var v5 = (byte)(data[1] >> 3);
                    data[1] *= 32;
                    data[1] |= v5;
                    var v28 = (byte)((data[0] >> 3) & 1);
                    if (((data[0] >> 3) & 1) != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    if (v28 != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    var v27 = (byte)((data[3] >> 3) & 1);
                    if (((data[3] >> 3) & 1) != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    if (v27 != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
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
                            data[13] ^= 0x8C;
                            var v21 = (byte)((data[23] >> 7) & 1);
                            if (((data[23] >> 6) & 1) != 0)
                            {
                                data[23] |= 0x80;
                            }
                            else
                            {
                                data[23] &= 0x7F;
                            }

                            if (v21 != 0)
                            {
                                data[23] |= 0x40;
                            }
                            else
                            {
                                data[23] &= 0xBF;
                            }

                            var v20 = (byte)((data[22] >> 2) & 1);
                            if (((data[22] >> 5) & 1) != 0)
                            {
                                data[22] |= 4;
                            }
                            else
                            {
                                data[22] &= 0xFB;
                            }

                            if (v20 != 0)
                            {
                                data[22] |= 0x20;
                            }
                            else
                            {
                                data[22] &= 0xDF;
                            }

                            data[3] ^= 0x5C;
                            var v19 = (byte)((data[30] >> 1) & 1);
                            if (((data[30] >> 4) & 1) != 0)
                            {
                                data[30] |= 2;
                            }
                            else
                            {
                                data[30] &= 0xFD;
                            }

                            if (v19 != 0)
                            {
                                data[30] |= 0x10;
                            }
                            else
                            {
                                data[30] &= 0xEF;
                            }

                            var v18 = (byte)((data[25] >> 4) & 1);
                            if (((data[25] >> 5) & 1) != 0)
                            {
                                data[25] |= 0x10;
                            }
                            else
                            {
                                data[25] &= 0xEF;
                            }

                            if (v18 != 0)
                            {
                                data[25] |= 0x20;
                            }
                            else
                            {
                                data[25] &= 0xDF;
                            }

                            var v16 = (byte)(data[0] >> 6);
                            data[0] *= 4;
                            data[0] |= v16;
                            data[24] ^= 0x4F;
                            var v17 = (byte)(data[11] >> 4);
                            data[11] *= 16;
                            data[11] |= v17;
                            data[27] ^= 0x19;
                            data[24] ^= 0x16;
                        }
                        else
                        {
                            var v9 = (byte)(data[11] >> 3);
                            data[11] *= 32;
                            data[11] |= v9;
                            var v10 = (byte)(data[13] >> 4);
                            data[13] *= 16;
                            data[13] |= v10;
                            var v11 = data[11];
                            data[11] = data[6];
                            data[6] = v11;
                            var v12 = (byte)(data[2] >> 1);
                            data[2] <<= 7;
                            data[2] |= v12;
                            data[6] ^= 0x8F;
                            data[11] ^= 0xCA;
                            data[0] ^= 0x9B;
                            var v13 = data[4];
                            data[4] = data[9];
                            data[9] = v13;
                            var v14 = (byte)(data[14] >> 2);
                            data[14] <<= 6;
                            data[14] |= v14;
                            var v15 = data[7];
                            data[7] = data[13];
                            data[13] = v15;
                        }
                    }
                    else
                    {
                        var v26 = (byte)((data[5] >> 3) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 8;
                        }
                        else
                        {
                            data[5] &= 0xF7;
                        }

                        if (v26 != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        var v6 = (byte)(data[2] >> 3);
                        data[2] *= 32;
                        data[2] |= v6;
                        data[7] ^= 0xD;
                        var v7 = (byte)(data[2] >> 4);
                        data[2] *= 16;
                        data[2] |= v7;
                        var v25 = (byte)((data[5] >> 4) & 1);
                        if (((data[5] >> 4) & 1) != 0)
                        {
                            data[5] |= 0x10;
                        }
                        else
                        {
                            data[5] &= 0xEF;
                        }

                        if (v25 != 0)
                        {
                            data[5] |= 0x10;
                        }
                        else
                        {
                            data[5] &= 0xEF;
                        }

                        var v24 = (byte)((data[1] >> 6) & 1);
                        if (((data[1] >> 2) & 1) != 0)
                        {
                            data[1] |= 0x40;
                        }
                        else
                        {
                            data[1] &= 0xBF;
                        }

                        if (v24 != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        var v23 = (byte)((data[1] >> 7) & 1);
                        if (((data[1] >> 4) & 1) != 0)
                        {
                            data[1] |= 0x80;
                        }
                        else
                        {
                            data[1] &= 0x7F;
                        }

                        if (v23 != 0)
                        {
                            data[1] |= 0x10;
                        }
                        else
                        {
                            data[1] &= 0xEF;
                        }

                        var v22 = (byte)((data[0] >> 6) & 1);
                        if (((data[0] >> 1) & 1) != 0)
                        {
                            data[0] |= 0x40;
                        }
                        else
                        {
                            data[0] &= 0xBF;
                        }

                        if (v22 != 0)
                        {
                            data[0] |= 2;
                        }
                        else
                        {
                            data[0] &= 0xFD;
                        }

                        data[1] ^= 0x9D;
                        var v8 = (byte)(data[7] >> 4);
                        data[7] *= 16;
                        data[7] |= v8;
                    }
                }
                else
                {
                    var v2 = (byte)((data[3] >> 3) & 1);
                    if (((data[3] >> 3) & 1) != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    var v29 = (byte)((data[0] >> 3) & 1);
                    if (((data[0] >> 3) & 1) != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    if (v29 != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    var v3 = (byte)(data[1] >> 5);
                    data[1] *= 8;
                    data[1] |= v3;
                    data[3] ^= 0xF7;
                    var v28 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 4) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v28 != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    var v4 = (byte)(data[2] >> 2);
                    data[2] <<= 6;
                    data[2] |= v4;
                    var v27 = (byte)((data[2] >> 7) & 1);
                    if (((data[2] >> 5) & 1) != 0)
                    {
                        data[2] |= 0x80;
                    }
                    else
                    {
                        data[2] &= 0x7F;
                    }

                    if (v27 != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    data[3] ^= 0xFA;
                    data[0] ^= 0xC5;
                    var v5 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v5;
                }
            }
        }
    }
}
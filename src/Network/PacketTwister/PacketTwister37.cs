// <copyright file="PacketTwister37.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of an unknown type.
    /// </summary>
    internal class PacketTwister37 : IPacketTwister
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
                            var v19 = (byte)((data[31] >> 4) & 1);
                            if (((data[31] >> 4) & 1) != 0)
                            {
                                data[31] |= 0x10;
                            }
                            else
                            {
                                data[31] &= 0xEF;
                            }

                            if (v19 != 0)
                            {
                                data[31] |= 0x10;
                            }
                            else
                            {
                                data[31] &= 0xEF;
                            }

                            var v14 = data[29];
                            data[29] = data[7];
                            data[7] = v14;
                            data[17] ^= 0xE6;
                            var v15 = (byte)(data[9] >> 6);
                            data[9] *= 4;
                            data[9] |= v15;
                            var v18 = (byte)((data[24] >> 1) & 1);
                            if (((data[24] >> 3) & 1) != 0)
                            {
                                data[24] |= 2;
                            }
                            else
                            {
                                data[24] &= 0xFD;
                            }

                            if (v18 != 0)
                            {
                                data[24] |= 8;
                            }
                            else
                            {
                                data[24] &= 0xF7;
                            }

                            var v16 = (byte)(data[1] >> 4);
                            data[1] *= 16;
                            data[1] |= v16;
                            data[10] ^= 0x97;
                            var v17 = (byte)(data[16] >> 5);
                            data[16] *= 8;
                            data[16] |= v17;
                        }
                        else
                        {
                            var v9 = data[7];
                            data[7] = data[3];
                            data[3] = v9;
                            var v10 = data[10];
                            data[10] = data[8];
                            data[8] = v10;
                            var v21 = (byte)((data[9] >> 2) & 1);
                            if (((data[9] >> 6) & 1) != 0)
                            {
                                data[9] |= 4;
                            }
                            else
                            {
                                data[9] &= 0xFB;
                            }

                            if (v21 != 0)
                            {
                                data[9] |= 0x40;
                            }
                            else
                            {
                                data[9] &= 0xBF;
                            }

                            data[5] ^= 0x7E;
                            data[6] ^= 0xDF;
                            var v11 = data[10];
                            data[10] = data[8];
                            data[8] = v11;
                            var v12 = data[13];
                            data[13] = data[7];
                            data[7] = v12;
                            var v20 = (byte)((data[11] >> 6) & 1);
                            if (((data[11] >> 4) & 1) != 0)
                            {
                                data[11] |= 0x40;
                            }
                            else
                            {
                                data[11] &= 0xBF;
                            }

                            if (v20 != 0)
                            {
                                data[11] |= 0x10;
                            }
                            else
                            {
                                data[11] &= 0xEF;
                            }

                            var v13 = data[13];
                            data[13] = data[3];
                            data[3] = v13;
                        }
                    }
                    else
                    {
                        var v6 = data[1];
                        data[1] = data[1];
                        data[1] = v6;
                        var v23 = (byte)((data[5] >> 2) & 1);
                        if (((data[5] >> 3) & 1) != 0)
                        {
                            data[5] |= 4;
                        }
                        else
                        {
                            data[5] &= 0xFB;
                        }

                        if (v23 != 0)
                        {
                            data[5] |= 8;
                        }
                        else
                        {
                            data[5] &= 0xF7;
                        }

                        data[5] ^= 0x83;
                        var v7 = (byte)(data[7] >> 2);
                        data[7] <<= 6;
                        data[7] |= v7;
                        data[5] ^= 0x5D;
                        var v22 = (byte)((data[6] >> 2) & 1);
                        if (((data[6] >> 7) & 1) != 0)
                        {
                            data[6] |= 4;
                        }
                        else
                        {
                            data[6] &= 0xFB;
                        }

                        if (v22 != 0)
                        {
                            data[6] |= 0x80;
                        }
                        else
                        {
                            data[6] &= 0x7F;
                        }

                        var v8 = data[1];
                        data[1] = data[0];
                        data[0] = v8;
                        data[4] ^= 0x98;
                    }
                }
                else
                {
                    data[2] ^= 0x63;
                    var v2 = (byte)((data[3] >> 3) & 1);
                    if (((data[3] >> 6) & 1) != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }

                    var v3 = data[3];
                    data[3] = data[0];
                    data[0] = v3;
                    var v4 = data[1];
                    data[1] = data[3];
                    data[3] = v4;
                    var v5 = data[0];
                    data[0] = data[3];
                    data[3] = v5;
                    var v24 = (byte)((data[0] >> 6) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 0x40;
                    }
                    else
                    {
                        data[0] &= 0xBF;
                    }

                    if (v24 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
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
                            var v14 = (byte)(data[16] >> 3);
                            data[16] *= 32;
                            data[16] |= v14;
                            data[10] ^= 0x97;
                            var v15 = (byte)(data[1] >> 4);
                            data[1] *= 16;
                            data[1] |= v15;
                            var v19 = (byte)((data[24] >> 1) & 1);
                            if (((data[24] >> 3) & 1) != 0)
                            {
                                data[24] |= 2;
                            }
                            else
                            {
                                data[24] &= 0xFD;
                            }

                            if (v19 != 0)
                            {
                                data[24] |= 8;
                            }
                            else
                            {
                                data[24] &= 0xF7;
                            }

                            var v16 = (byte)(data[9] >> 2);
                            data[9] <<= 6;
                            data[9] |= v16;
                            data[17] ^= 0xE6;
                            var v17 = data[29];
                            data[29] = data[7];
                            data[7] = v17;
                            var v18 = (byte)((data[31] >> 4) & 1);
                            if (((data[31] >> 4) & 1) != 0)
                            {
                                data[31] |= 0x10;
                            }
                            else
                            {
                                data[31] &= 0xEF;
                            }

                            if (v18 != 0)
                            {
                                data[31] |= 0x10;
                            }
                            else
                            {
                                data[31] &= 0xEF;
                            }
                        }
                        else
                        {
                            var v9 = data[13];
                            data[13] = data[3];
                            data[3] = v9;
                            var v21 = (byte)((data[11] >> 6) & 1);
                            if (((data[11] >> 4) & 1) != 0)
                            {
                                data[11] |= 0x40;
                            }
                            else
                            {
                                data[11] &= 0xBF;
                            }

                            if (v21 != 0)
                            {
                                data[11] |= 0x10;
                            }
                            else
                            {
                                data[11] &= 0xEF;
                            }

                            var v10 = data[13];
                            data[13] = data[7];
                            data[7] = v10;
                            var v11 = data[10];
                            data[10] = data[8];
                            data[8] = v11;
                            data[6] ^= 0xDF;
                            data[5] ^= 0x7E;
                            var v20 = (byte)((data[9] >> 2) & 1);
                            if (((data[9] >> 6) & 1) != 0)
                            {
                                data[9] |= 4;
                            }
                            else
                            {
                                data[9] &= 0xFB;
                            }

                            if (v20 != 0)
                            {
                                data[9] |= 0x40;
                            }
                            else
                            {
                                data[9] &= 0xBF;
                            }

                            var v12 = data[10];
                            data[10] = data[8];
                            data[8] = v12;
                            var v13 = data[7];
                            data[7] = data[3];
                            data[3] = v13;
                        }
                    }
                    else
                    {
                        data[4] ^= 0x98;
                        var v6 = data[1];
                        data[1] = data[0];
                        data[0] = v6;
                        var v23 = (byte)((data[6] >> 2) & 1);
                        if (((data[6] >> 7) & 1) != 0)
                        {
                            data[6] |= 4;
                        }
                        else
                        {
                            data[6] &= 0xFB;
                        }

                        if (v23 != 0)
                        {
                            data[6] |= 0x80;
                        }
                        else
                        {
                            data[6] &= 0x7F;
                        }

                        data[5] ^= 0x5D;
                        var v7 = (byte)(data[7] >> 6);
                        data[7] *= 4;
                        data[7] |= v7;
                        data[5] ^= 0x83;
                        var v22 = (byte)((data[5] >> 2) & 1);
                        if (((data[5] >> 3) & 1) != 0)
                        {
                            data[5] |= 4;
                        }
                        else
                        {
                            data[5] &= 0xFB;
                        }

                        if (v22 != 0)
                        {
                            data[5] |= 8;
                        }
                        else
                        {
                            data[5] &= 0xF7;
                        }

                        var v8 = data[1];
                        data[1] = data[1];
                        data[1] = v8;
                    }
                }
                else
                {
                    var v2 = (byte)((data[0] >> 6) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 0x40;
                    }
                    else
                    {
                        data[0] &= 0xBF;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v3 = data[0];
                    data[0] = data[3];
                    data[3] = v3;
                    var v4 = data[1];
                    data[1] = data[3];
                    data[3] = v4;
                    var v5 = data[3];
                    data[3] = data[0];
                    data[0] = v5;
                    var v24 = (byte)((data[3] >> 3) & 1);
                    if (((data[3] >> 6) & 1) != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    if (v24 != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }

                    data[2] ^= 0x63;
                }
            }
        }
    }
}
// <copyright file="PacketTwisterOfItemDrop.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'ItemDrop' type.
    /// </summary>
    internal class PacketTwisterOfItemDrop : IPacketTwister
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
                            var v18 = (byte)(data[8] >> 7);
                            data[8] *= 2;
                            data[8] |= v18;
                            var v19 = data[1];
                            data[1] = data[20];
                            data[20] = v19;
                            var v24 = (byte)((data[28] >> 2) & 1);
                            if (((data[28] >> 1) & 1) != 0)
                            {
                                data[28] |= 4;
                            }
                            else
                            {
                                data[28] &= 0xFB;
                            }

                            if (v24 != 0)
                            {
                                data[28] |= 2;
                            }
                            else
                            {
                                data[28] &= 0xFD;
                            }

                            data[21] ^= 0x28;
                            var v20 = data[6];
                            data[6] = data[27];
                            data[27] = v20;
                            var v21 = (byte)(data[14] >> 7);
                            data[14] *= 2;
                            data[14] |= v21;
                            var v23 = (byte)((data[8] >> 4) & 1);
                            if (((data[8] >> 1) & 1) != 0)
                            {
                                data[8] |= 0x10;
                            }
                            else
                            {
                                data[8] &= 0xEF;
                            }

                            if (v23 != 0)
                            {
                                data[8] |= 2;
                            }
                            else
                            {
                                data[8] &= 0xFD;
                            }

                            data[4] ^= 0xBC;
                            data[22] ^= 0xB0;
                            var v22 = data[22];
                            data[22] = data[7];
                            data[7] = v22;
                        }
                        else
                        {
                            var v15 = data[10];
                            data[10] = data[6];
                            data[6] = v15;
                            var v29 = (byte)((data[0] >> 1) & 1);
                            if (((data[0] >> 2) & 1) != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            if (v29 != 0)
                            {
                                data[0] |= 4;
                            }
                            else
                            {
                                data[0] &= 0xFB;
                            }

                            var v28 = (byte)((data[12] >> 6) & 1);
                            if (((data[12] >> 6) & 1) != 0)
                            {
                                data[12] |= 0x40;
                            }
                            else
                            {
                                data[12] &= 0xBF;
                            }

                            if (v28 != 0)
                            {
                                data[12] |= 0x40;
                            }
                            else
                            {
                                data[12] &= 0xBF;
                            }

                            data[13] ^= 0x74;
                            var v16 = data[12];
                            data[12] = data[5];
                            data[5] = v16;
                            var v17 = (byte)(data[7] >> 7);
                            data[7] *= 2;
                            data[7] |= v17;
                            var v27 = (byte)((data[0] >> 4) & 1);
                            if (((data[0] >> 1) & 1) != 0)
                            {
                                data[0] |= 0x10;
                            }
                            else
                            {
                                data[0] &= 0xEF;
                            }

                            if (v27 != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            var v26 = (byte)((data[1] >> 1) & 1);
                            if (((data[1] >> 2) & 1) != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }

                            if (v26 != 0)
                            {
                                data[1] |= 4;
                            }
                            else
                            {
                                data[1] &= 0xFB;
                            }

                            var v25 = (byte)((data[5] >> 6) & 1);
                            if (((data[5] >> 7) & 1) != 0)
                            {
                                data[5] |= 0x40;
                            }
                            else
                            {
                                data[5] &= 0xBF;
                            }

                            if (v25 != 0)
                            {
                                data[5] |= 0x80;
                            }
                            else
                            {
                                data[5] &= 0x7F;
                            }
                        }
                    }
                    else
                    {
                        var v32 = (byte)((data[3] >> 2) & 1);
                        if (((data[3] >> 1) & 1) != 0)
                        {
                            data[3] |= 4;
                        }
                        else
                        {
                            data[3] &= 0xFB;
                        }

                        if (v32 != 0)
                        {
                            data[3] |= 2;
                        }
                        else
                        {
                            data[3] &= 0xFD;
                        }

                        data[7] ^= 0xC3;
                        var v31 = (byte)((data[6] >> 2) & 1);
                        if (((data[6] >> 4) & 1) != 0)
                        {
                            data[6] |= 4;
                        }
                        else
                        {
                            data[6] &= 0xFB;
                        }

                        if (v31 != 0)
                        {
                            data[6] |= 0x10;
                        }
                        else
                        {
                            data[6] &= 0xEF;
                        }

                        var v10 = data[7];
                        data[7] = data[0];
                        data[0] = v10;
                        data[5] ^= 0x43;
                        data[4] ^= 0x74;
                        var v11 = (byte)(data[1] >> 4);
                        data[1] *= 16;
                        data[1] |= v11;
                        var v12 = data[2];
                        data[2] = data[0];
                        data[0] = v12;
                        data[7] ^= 0xA4;
                        var v13 = data[7];
                        data[7] = data[7];
                        data[7] = v13;
                        var v14 = data[7];
                        data[7] = data[2];
                        data[2] = v14;
                        var v30 = (byte)((data[7] >> 4) & 1);
                        if (((data[7] >> 7) & 1) != 0)
                        {
                            data[7] |= 0x10;
                        }
                        else
                        {
                            data[7] &= 0xEF;
                        }

                        if (v30 != 0)
                        {
                            data[7] |= 0x80;
                        }
                        else
                        {
                            data[7] &= 0x7F;
                        }

                        data[4] ^= 0x9A;
                    }
                }
                else
                {
                    var v2 = (byte)((data[1] >> 4) & 1);
                    if (((data[1] >> 3) & 1) != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    var v3 = data[2];
                    data[2] = data[2];
                    data[2] = v3;
                    data[1] ^= 0x2F;
                    var v4 = (byte)(data[1] >> 7);
                    data[1] *= 2;
                    data[1] |= v4;
                    var v5 = data[1];
                    data[1] = data[2];
                    data[2] = v5;
                    var v6 = (byte)(data[2] >> 5);
                    data[2] *= 8;
                    data[2] |= v6;
                    data[3] ^= 0xC9;
                    var v7 = data[3];
                    data[3] = data[3];
                    data[3] = v7;
                    var v8 = (byte)(data[2] >> 4);
                    data[2] *= 16;
                    data[2] |= v8;
                    data[0] ^= 0x3E;
                    var v9 = (byte)(data[3] >> 6);
                    data[3] *= 4;
                    data[3] |= v9;
                    var v33 = (byte)((data[1] >> 3) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    if (v33 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
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
                            var v18 = data[22];
                            data[22] = data[7];
                            data[7] = v18;
                            data[22] ^= 0xB0;
                            data[4] ^= 0xBC;
                            var v24 = (byte)((data[8] >> 4) & 1);
                            if (((data[8] >> 1) & 1) != 0)
                            {
                                data[8] |= 0x10;
                            }
                            else
                            {
                                data[8] &= 0xEF;
                            }

                            if (v24 != 0)
                            {
                                data[8] |= 2;
                            }
                            else
                            {
                                data[8] &= 0xFD;
                            }

                            var v19 = (byte)(data[14] >> 1);
                            data[14] <<= 7;
                            data[14] |= v19;
                            var v20 = data[6];
                            data[6] = data[27];
                            data[27] = v20;
                            data[21] ^= 0x28;
                            var v23 = (byte)((data[28] >> 2) & 1);
                            if (((data[28] >> 1) & 1) != 0)
                            {
                                data[28] |= 4;
                            }
                            else
                            {
                                data[28] &= 0xFB;
                            }

                            if (v23 != 0)
                            {
                                data[28] |= 2;
                            }
                            else
                            {
                                data[28] &= 0xFD;
                            }

                            var v21 = data[1];
                            data[1] = data[20];
                            data[20] = v21;
                            var v22 = (byte)(data[8] >> 1);
                            data[8] <<= 7;
                            data[8] |= v22;
                        }
                        else
                        {
                            var v29 = (byte)((data[5] >> 6) & 1);
                            if (((data[5] >> 7) & 1) != 0)
                            {
                                data[5] |= 0x40;
                            }
                            else
                            {
                                data[5] &= 0xBF;
                            }

                            if (v29 != 0)
                            {
                                data[5] |= 0x80;
                            }
                            else
                            {
                                data[5] &= 0x7F;
                            }

                            var v28 = (byte)((data[1] >> 1) & 1);
                            if (((data[1] >> 2) & 1) != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }

                            if (v28 != 0)
                            {
                                data[1] |= 4;
                            }
                            else
                            {
                                data[1] &= 0xFB;
                            }

                            var v27 = (byte)((data[0] >> 4) & 1);
                            if (((data[0] >> 1) & 1) != 0)
                            {
                                data[0] |= 0x10;
                            }
                            else
                            {
                                data[0] &= 0xEF;
                            }

                            if (v27 != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            var v15 = (byte)(data[7] >> 1);
                            data[7] <<= 7;
                            data[7] |= v15;
                            var v16 = data[12];
                            data[12] = data[5];
                            data[5] = v16;
                            data[13] ^= 0x74;
                            var v26 = (byte)((data[12] >> 6) & 1);
                            if (((data[12] >> 6) & 1) != 0)
                            {
                                data[12] |= 0x40;
                            }
                            else
                            {
                                data[12] &= 0xBF;
                            }

                            if (v26 != 0)
                            {
                                data[12] |= 0x40;
                            }
                            else
                            {
                                data[12] &= 0xBF;
                            }

                            var v25 = (byte)((data[0] >> 1) & 1);
                            if (((data[0] >> 2) & 1) != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            if (v25 != 0)
                            {
                                data[0] |= 4;
                            }
                            else
                            {
                                data[0] &= 0xFB;
                            }

                            var v17 = data[10];
                            data[10] = data[6];
                            data[6] = v17;
                        }
                    }
                    else
                    {
                        data[4] ^= 0x9A;
                        var v32 = (byte)((data[7] >> 4) & 1);
                        if (((data[7] >> 7) & 1) != 0)
                        {
                            data[7] |= 0x10;
                        }
                        else
                        {
                            data[7] &= 0xEF;
                        }

                        if (v32 != 0)
                        {
                            data[7] |= 0x80;
                        }
                        else
                        {
                            data[7] &= 0x7F;
                        }

                        var v10 = data[7];
                        data[7] = data[2];
                        data[2] = v10;
                        var v11 = data[7];
                        data[7] = data[7];
                        data[7] = v11;
                        data[7] ^= 0xA4;
                        var v12 = data[2];
                        data[2] = data[0];
                        data[0] = v12;
                        var v13 = (byte)(data[1] >> 4);
                        data[1] *= 16;
                        data[1] |= v13;
                        data[4] ^= 0x74;
                        data[5] ^= 0x43;
                        var v14 = data[7];
                        data[7] = data[0];
                        data[0] = v14;
                        var v31 = (byte)((data[6] >> 2) & 1);
                        if (((data[6] >> 4) & 1) != 0)
                        {
                            data[6] |= 4;
                        }
                        else
                        {
                            data[6] &= 0xFB;
                        }

                        if (v31 != 0)
                        {
                            data[6] |= 0x10;
                        }
                        else
                        {
                            data[6] &= 0xEF;
                        }

                        data[7] ^= 0xC3;
                        var v30 = (byte)((data[3] >> 2) & 1);
                        if (((data[3] >> 1) & 1) != 0)
                        {
                            data[3] |= 4;
                        }
                        else
                        {
                            data[3] &= 0xFB;
                        }

                        if (v30 != 0)
                        {
                            data[3] |= 2;
                        }
                        else
                        {
                            data[3] &= 0xFD;
                        }
                    }
                }
                else
                {
                    var v2 = (byte)((data[1] >> 3) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    var v3 = (byte)(data[3] >> 2);
                    data[3] <<= 6;
                    data[3] |= v3;
                    data[0] ^= 0x3E;
                    var v4 = (byte)(data[2] >> 4);
                    data[2] *= 16;
                    data[2] |= v4;
                    var v5 = data[3];
                    data[3] = data[3];
                    data[3] = v5;
                    data[3] ^= 0xC9;
                    var v6 = (byte)(data[2] >> 3);
                    data[2] *= 32;
                    data[2] |= v6;
                    var v7 = data[1];
                    data[1] = data[2];
                    data[2] = v7;
                    var v8 = (byte)(data[1] >> 1);
                    data[1] <<= 7;
                    data[1] |= v8;
                    data[1] ^= 0x2F;
                    var v9 = data[2];
                    data[2] = data[2];
                    data[2] = v9;
                    var v33 = (byte)((data[1] >> 4) & 1);
                    if (((data[1] >> 3) & 1) != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    if (v33 != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }
                }
            }
        }
    }
}
// <copyright file="PacketTwisterOfItemMove.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'ItemMove' type.
    /// </summary>
    internal class PacketTwisterOfItemMove : IPacketTwister
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
                            var v20 = (byte)(data[21] >> 6);
                            data[21] *= 4;
                            data[21] |= v20;
                            var v27 = (byte)((data[5] >> 6) & 1);
                            if (((data[5] >> 1) & 1) != 0)
                            {
                                data[5] |= 0x40;
                            }
                            else
                            {
                                data[5] &= 0xBF;
                            }

                            if (v27 != 0)
                            {
                                data[5] |= 2;
                            }
                            else
                            {
                                data[5] &= 0xFD;
                            }

                            var v21 = data[12];
                            data[12] = data[12];
                            data[12] = v21;
                            var v22 = (byte)(data[3] >> 6);
                            data[3] *= 4;
                            data[3] |= v22;
                            data[13] ^= 0x7F;
                            var v26 = (byte)((data[21] >> 1) & 1);
                            if (((data[21] >> 7) & 1) != 0)
                            {
                                data[21] |= 2;
                            }
                            else
                            {
                                data[21] &= 0xFD;
                            }

                            if (v26 != 0)
                            {
                                data[21] |= 0x80;
                            }
                            else
                            {
                                data[21] &= 0x7F;
                            }

                            data[21] ^= 0xF5;
                            var v25 = (byte)((data[20] >> 1) & 1);
                            if (((data[20] >> 6) & 1) != 0)
                            {
                                data[20] |= 2;
                            }
                            else
                            {
                                data[20] &= 0xFD;
                            }

                            if (v25 != 0)
                            {
                                data[20] |= 0x40;
                            }
                            else
                            {
                                data[20] &= 0xBF;
                            }

                            var v23 = (byte)(data[21] >> 5);
                            data[21] *= 8;
                            data[21] |= v23;
                            var v24 = (byte)(data[13] >> 2);
                            data[13] <<= 6;
                            data[13] |= v24;
                        }
                        else
                        {
                            var v12 = (byte)(data[7] >> 6);
                            data[7] *= 4;
                            data[7] |= v12;
                            data[10] ^= 0xFE;
                            var v13 = data[15];
                            data[15] = data[5];
                            data[5] = v13;
                            var v14 = data[5];
                            data[5] = data[6];
                            data[6] = v14;
                            var v15 = (byte)(data[14] >> 7);
                            data[14] *= 2;
                            data[14] |= v15;
                            var v16 = (byte)(data[14] >> 6);
                            data[14] *= 4;
                            data[14] |= v16;
                            var v17 = (byte)(data[10] >> 7);
                            data[10] *= 2;
                            data[10] |= v17;
                            var v28 = (byte)((data[3] >> 2) & 1);
                            if (((data[3] >> 5) & 1) != 0)
                            {
                                data[3] |= 4;
                            }
                            else
                            {
                                data[3] &= 0xFB;
                            }

                            if (v28 != 0)
                            {
                                data[3] |= 0x20;
                            }
                            else
                            {
                                data[3] &= 0xDF;
                            }

                            var v18 = data[4];
                            data[4] = data[4];
                            data[4] = v18;
                            var v19 = (byte)(data[1] >> 6);
                            data[1] *= 4;
                            data[1] |= v19;
                        }
                    }
                    else
                    {
                        var v8 = (byte)(data[7] >> 1);
                        data[7] <<= 7;
                        data[7] |= v8;
                        data[5] ^= 0x7D;
                        data[7] ^= 0x7E;
                        var v9 = (byte)(data[1] >> 5);
                        data[1] *= 8;
                        data[1] |= v9;
                        var v10 = (byte)(data[5] >> 7);
                        data[5] *= 2;
                        data[5] |= v10;
                        var v32 = (byte)((data[1] >> 7) & 1);
                        if (((data[1] >> 5) & 1) != 0)
                        {
                            data[1] |= 0x80;
                        }
                        else
                        {
                            data[1] &= 0x7F;
                        }

                        if (v32 != 0)
                        {
                            data[1] |= 0x20;
                        }
                        else
                        {
                            data[1] &= 0xDF;
                        }

                        var v11 = (byte)(data[7] >> 2);
                        data[7] <<= 6;
                        data[7] |= v11;
                        data[0] ^= 0xDA;
                        var v31 = (byte)((data[0] >> 5) & 1);
                        if (((data[0] >> 3) & 1) != 0)
                        {
                            data[0] |= 0x20;
                        }
                        else
                        {
                            data[0] &= 0xDF;
                        }

                        if (v31 != 0)
                        {
                            data[0] |= 8;
                        }
                        else
                        {
                            data[0] &= 0xF7;
                        }

                        var v30 = (byte)((data[3] >> 4) & 1);
                        if (((data[3] >> 6) & 1) != 0)
                        {
                            data[3] |= 0x10;
                        }
                        else
                        {
                            data[3] &= 0xEF;
                        }

                        if (v30 != 0)
                        {
                            data[3] |= 0x40;
                        }
                        else
                        {
                            data[3] &= 0xBF;
                        }

                        var v29 = (byte)((data[6] >> 2) & 1);
                        if (((data[6] >> 1) & 1) != 0)
                        {
                            data[6] |= 4;
                        }
                        else
                        {
                            data[6] &= 0xFB;
                        }

                        if (v29 != 0)
                        {
                            data[6] |= 2;
                        }
                        else
                        {
                            data[6] &= 0xFD;
                        }
                    }
                }
                else
                {
                    data[3] ^= 0x4B;
                    var v2 = (byte)((data[1] >> 3) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    data[0] ^= 0x32;
                    var v36 = (byte)((data[2] >> 6) & 1);
                    if (((data[2] >> 4) & 1) != 0)
                    {
                        data[2] |= 0x40;
                    }
                    else
                    {
                        data[2] &= 0xBF;
                    }

                    if (v36 != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    var v35 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 6) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v35 != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }

                    var v34 = (byte)((data[1] >> 4) & 1);
                    if (((data[1] >> 5) & 1) != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    if (v34 != 0)
                    {
                        data[1] |= 0x20;
                    }
                    else
                    {
                        data[1] &= 0xDF;
                    }

                    var v3 = data[2];
                    data[2] = data[1];
                    data[1] = v3;
                    var v4 = data[2];
                    data[2] = data[0];
                    data[0] = v4;
                    var v33 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v33 != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    var v5 = (byte)(data[0] >> 1);
                    data[0] <<= 7;
                    data[0] |= v5;
                    var v6 = (byte)(data[0] >> 5);
                    data[0] *= 8;
                    data[0] |= v6;
                    data[1] ^= 0x69;
                    var v7 = data[3];
                    data[3] = data[1];
                    data[1] = v7;
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
                            var v20 = (byte)(data[13] >> 6);
                            data[13] *= 4;
                            data[13] |= v20;
                            var v21 = (byte)(data[21] >> 3);
                            data[21] *= 32;
                            data[21] |= v21;
                            var v27 = (byte)((data[20] >> 1) & 1);
                            if (((data[20] >> 6) & 1) != 0)
                            {
                                data[20] |= 2;
                            }
                            else
                            {
                                data[20] &= 0xFD;
                            }

                            if (v27 != 0)
                            {
                                data[20] |= 0x40;
                            }
                            else
                            {
                                data[20] &= 0xBF;
                            }

                            data[21] ^= 0xF5;
                            var v26 = (byte)((data[21] >> 1) & 1);
                            if (((data[21] >> 7) & 1) != 0)
                            {
                                data[21] |= 2;
                            }
                            else
                            {
                                data[21] &= 0xFD;
                            }

                            if (v26 != 0)
                            {
                                data[21] |= 0x80;
                            }
                            else
                            {
                                data[21] &= 0x7F;
                            }

                            data[13] ^= 0x7F;
                            var v22 = (byte)(data[3] >> 2);
                            data[3] <<= 6;
                            data[3] |= v22;
                            var v23 = data[12];
                            data[12] = data[12];
                            data[12] = v23;
                            var v25 = (byte)((data[5] >> 6) & 1);
                            if (((data[5] >> 1) & 1) != 0)
                            {
                                data[5] |= 0x40;
                            }
                            else
                            {
                                data[5] &= 0xBF;
                            }

                            if (v25 != 0)
                            {
                                data[5] |= 2;
                            }
                            else
                            {
                                data[5] &= 0xFD;
                            }

                            var v24 = (byte)(data[21] >> 2);
                            data[21] <<= 6;
                            data[21] |= v24;
                        }
                        else
                        {
                            var v12 = (byte)(data[1] >> 2);
                            data[1] <<= 6;
                            data[1] |= v12;
                            var v13 = data[4];
                            data[4] = data[4];
                            data[4] = v13;
                            var v28 = (byte)((data[3] >> 2) & 1);
                            if (((data[3] >> 5) & 1) != 0)
                            {
                                data[3] |= 4;
                            }
                            else
                            {
                                data[3] &= 0xFB;
                            }

                            if (v28 != 0)
                            {
                                data[3] |= 0x20;
                            }
                            else
                            {
                                data[3] &= 0xDF;
                            }

                            var v14 = (byte)(data[10] >> 1);
                            data[10] <<= 7;
                            data[10] |= v14;
                            var v15 = (byte)(data[14] >> 2);
                            data[14] <<= 6;
                            data[14] |= v15;
                            var v16 = (byte)(data[14] >> 1);
                            data[14] <<= 7;
                            data[14] |= v16;
                            var v17 = data[5];
                            data[5] = data[6];
                            data[6] = v17;
                            var v18 = data[15];
                            data[15] = data[5];
                            data[5] = v18;
                            data[10] ^= 0xFE;
                            var v19 = (byte)(data[7] >> 2);
                            data[7] <<= 6;
                            data[7] |= v19;
                        }
                    }
                    else
                    {
                        var v32 = (byte)((data[6] >> 2) & 1);
                        if (((data[6] >> 1) & 1) != 0)
                        {
                            data[6] |= 4;
                        }
                        else
                        {
                            data[6] &= 0xFB;
                        }

                        if (v32 != 0)
                        {
                            data[6] |= 2;
                        }
                        else
                        {
                            data[6] &= 0xFD;
                        }

                        var v31 = (byte)((data[3] >> 4) & 1);
                        if (((data[3] >> 6) & 1) != 0)
                        {
                            data[3] |= 0x10;
                        }
                        else
                        {
                            data[3] &= 0xEF;
                        }

                        if (v31 != 0)
                        {
                            data[3] |= 0x40;
                        }
                        else
                        {
                            data[3] &= 0xBF;
                        }

                        var v30 = (byte)((data[0] >> 5) & 1);
                        if (((data[0] >> 3) & 1) != 0)
                        {
                            data[0] |= 0x20;
                        }
                        else
                        {
                            data[0] &= 0xDF;
                        }

                        if (v30 != 0)
                        {
                            data[0] |= 8;
                        }
                        else
                        {
                            data[0] &= 0xF7;
                        }

                        data[0] ^= 0xDA;
                        var v8 = (byte)(data[7] >> 6);
                        data[7] *= 4;
                        data[7] |= v8;
                        var v29 = (byte)((data[1] >> 7) & 1);
                        if (((data[1] >> 5) & 1) != 0)
                        {
                            data[1] |= 0x80;
                        }
                        else
                        {
                            data[1] &= 0x7F;
                        }

                        if (v29 != 0)
                        {
                            data[1] |= 0x20;
                        }
                        else
                        {
                            data[1] &= 0xDF;
                        }

                        var v9 = (byte)(data[5] >> 1);
                        data[5] <<= 7;
                        data[5] |= v9;
                        var v10 = (byte)(data[1] >> 3);
                        data[1] *= 32;
                        data[1] |= v10;
                        data[7] ^= 0x7E;
                        data[5] ^= 0x7D;
                        var v11 = (byte)(data[7] >> 7);
                        data[7] *= 2;
                        data[7] |= v11;
                    }
                }
                else
                {
                    var v3 = data[3];
                    data[3] = data[1];
                    data[1] = v3;
                    data[1] ^= 0x69;
                    var v4 = (byte)(data[0] >> 3);
                    data[0] *= 32;
                    data[0] |= v4;
                    var v5 = (byte)(data[0] >> 7);
                    data[0] *= 2;
                    data[0] |= v5;
                    var v2 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    var v6 = data[2];
                    data[2] = data[0];
                    data[0] = v6;
                    var v7 = data[2];
                    data[2] = data[1];
                    data[1] = v7;
                    var v36 = (byte)((data[1] >> 4) & 1);
                    if (((data[1] >> 5) & 1) != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    if (v36 != 0)
                    {
                        data[1] |= 0x20;
                    }
                    else
                    {
                        data[1] &= 0xDF;
                    }

                    var v35 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 6) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v35 != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }

                    var v34 = (byte)((data[2] >> 6) & 1);
                    if (((data[2] >> 4) & 1) != 0)
                    {
                        data[2] |= 0x40;
                    }
                    else
                    {
                        data[2] &= 0xBF;
                    }

                    if (v34 != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    data[0] ^= 0x32;
                    var v33 = (byte)((data[1] >> 3) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    if (v33 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    data[3] ^= 0x4B;
                }
            }
        }
    }
}
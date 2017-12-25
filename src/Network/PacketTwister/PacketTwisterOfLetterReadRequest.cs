// <copyright file="PacketTwisterOfLetterReadRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'LetterReadRequest' type.
    /// </summary>
    internal class PacketTwisterOfLetterReadRequest : IPacketTwister
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
                            var v16 = data[14];
                            data[14] = data[1];
                            data[1] = v16;
                            var v17 = data[29];
                            data[29] = data[28];
                            data[28] = v17;
                            var v18 = data[16];
                            data[16] = data[11];
                            data[11] = v18;
                            var v24 = (byte)((data[28] >> 3) & 1);
                            if (((data[28] >> 6) & 1) != 0)
                            {
                                data[28] |= 8;
                            }
                            else
                            {
                                data[28] &= 0xF7;
                            }

                            if (v24 != 0)
                            {
                                data[28] |= 0x40;
                            }
                            else
                            {
                                data[28] &= 0xBF;
                            }

                            var v19 = data[8];
                            data[8] = data[12];
                            data[12] = v19;
                            var v20 = data[7];
                            data[7] = data[3];
                            data[3] = v20;
                            var v21 = (byte)(data[20] >> 2);
                            data[20] <<= 6;
                            data[20] |= v21;
                            var v22 = (byte)(data[12] >> 6);
                            data[12] *= 4;
                            data[12] |= v22;
                            var v23 = (byte)((data[15] >> 1) & 1);
                            if (((data[15] >> 3) & 1) != 0)
                            {
                                data[15] |= 2;
                            }
                            else
                            {
                                data[15] &= 0xFD;
                            }

                            if (v23 != 0)
                            {
                                data[15] |= 8;
                            }
                            else
                            {
                                data[15] &= 0xF7;
                            }
                        }
                        else
                        {
                            data[8] ^= 0x17;
                            var v28 = (byte)((data[12] >> 3) & 1);
                            if (((data[12] >> 2) & 1) != 0)
                            {
                                data[12] |= 8;
                            }
                            else
                            {
                                data[12] &= 0xF7;
                            }

                            if (v28 != 0)
                            {
                                data[12] |= 4;
                            }
                            else
                            {
                                data[12] &= 0xFB;
                            }

                            var v27 = (byte)((data[10] >> 1) & 1);
                            if (((data[10] >> 4) & 1) != 0)
                            {
                                data[10] |= 2;
                            }
                            else
                            {
                                data[10] &= 0xFD;
                            }

                            if (v27 != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            var v26 = (byte)((data[4] >> 7) & 1);
                            if (((data[4] >> 6) & 1) != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            if (v26 != 0)
                            {
                                data[4] |= 0x40;
                            }
                            else
                            {
                                data[4] &= 0xBF;
                            }

                            var v25 = (byte)((data[1] >> 5) & 1);
                            if (((data[1] >> 7) & 1) != 0)
                            {
                                data[1] |= 0x20;
                            }
                            else
                            {
                                data[1] &= 0xDF;
                            }

                            if (v25 != 0)
                            {
                                data[1] |= 0x80;
                            }
                            else
                            {
                                data[1] &= 0x7F;
                            }

                            var v11 = (byte)(data[0] >> 4);
                            data[0] *= 16;
                            data[0] |= v11;
                            var v12 = data[13];
                            data[13] = data[8];
                            data[8] = v12;
                            data[9] ^= 0xD4;
                            var v13 = (byte)(data[0] >> 7);
                            data[0] *= 2;
                            data[0] |= v13;
                            var v14 = (byte)(data[6] >> 6);
                            data[6] *= 4;
                            data[6] |= v14;
                            data[0] ^= 0xB0;
                            var v15 = data[12];
                            data[12] = data[12];
                            data[12] = v15;
                        }
                    }
                    else
                    {
                        data[2] ^= 0x5A;
                        var v7 = (byte)(data[4] >> 4);
                        data[4] *= 16;
                        data[4] |= v7;
                        var v31 = (byte)((data[3] >> 4) & 1);
                        if (((data[3] >> 1) & 1) != 0)
                        {
                            data[3] |= 0x10;
                        }
                        else
                        {
                            data[3] &= 0xEF;
                        }

                        if (v31 != 0)
                        {
                            data[3] |= 2;
                        }
                        else
                        {
                            data[3] &= 0xFD;
                        }

                        var v8 = data[1];
                        data[1] = data[0];
                        data[0] = v8;
                        var v30 = (byte)((data[0] >> 4) & 1);
                        if (((data[0] >> 4) & 1) != 0)
                        {
                            data[0] |= 0x10;
                        }
                        else
                        {
                            data[0] &= 0xEF;
                        }

                        if (v30 != 0)
                        {
                            data[0] |= 0x10;
                        }
                        else
                        {
                            data[0] &= 0xEF;
                        }

                        var v9 = data[4];
                        data[4] = data[7];
                        data[7] = v9;
                        var v29 = (byte)((data[2] >> 2) & 1);
                        if (((data[2] >> 3) & 1) != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        if (v29 != 0)
                        {
                            data[2] |= 8;
                        }
                        else
                        {
                            data[2] &= 0xF7;
                        }

                        var v10 = data[5];
                        data[5] = data[2];
                        data[2] = v10;
                    }
                }
                else
                {
                    var v3 = data[2];
                    data[2] = data[3];
                    data[3] = v3;
                    var v4 = (byte)(data[2] >> 4);
                    data[2] *= 16;
                    data[2] |= v4;
                    data[2] ^= 0xE4;
                    data[0] ^= 0x2F;
                    var v2 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 6) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }

                    var v5 = data[2];
                    data[2] = data[0];
                    data[0] = v5;
                    data[0] ^= 0x6D;
                    var v33 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v33 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    data[0] ^= 0x1B;
                    data[2] ^= 0xDF;
                    var v32 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v32 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    data[0] ^= 0xC9;
                    var v6 = (byte)(data[1] >> 6);
                    data[1] *= 4;
                    data[1] |= v6;
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
                            var v24 = (byte)((data[15] >> 1) & 1);
                            if (((data[15] >> 3) & 1) != 0)
                            {
                                data[15] |= 2;
                            }
                            else
                            {
                                data[15] &= 0xFD;
                            }

                            if (v24 != 0)
                            {
                                data[15] |= 8;
                            }
                            else
                            {
                                data[15] &= 0xF7;
                            }

                            var v16 = (byte)(data[12] >> 2);
                            data[12] <<= 6;
                            data[12] |= v16;
                            var v17 = (byte)(data[20] >> 6);
                            data[20] *= 4;
                            data[20] |= v17;
                            var v18 = data[7];
                            data[7] = data[3];
                            data[3] = v18;
                            var v19 = data[8];
                            data[8] = data[12];
                            data[12] = v19;
                            var v23 = (byte)((data[28] >> 3) & 1);
                            if (((data[28] >> 6) & 1) != 0)
                            {
                                data[28] |= 8;
                            }
                            else
                            {
                                data[28] &= 0xF7;
                            }

                            if (v23 != 0)
                            {
                                data[28] |= 0x40;
                            }
                            else
                            {
                                data[28] &= 0xBF;
                            }

                            var v20 = data[16];
                            data[16] = data[11];
                            data[11] = v20;
                            var v21 = data[29];
                            data[29] = data[28];
                            data[28] = v21;
                            var v22 = data[14];
                            data[14] = data[1];
                            data[1] = v22;
                        }
                        else
                        {
                            var v11 = data[12];
                            data[12] = data[12];
                            data[12] = v11;
                            data[0] ^= 0xB0;
                            var v12 = (byte)(data[6] >> 2);
                            data[6] <<= 6;
                            data[6] |= v12;
                            var v13 = (byte)(data[0] >> 1);
                            data[0] <<= 7;
                            data[0] |= v13;
                            data[9] ^= 0xD4;
                            var v14 = data[13];
                            data[13] = data[8];
                            data[8] = v14;
                            var v15 = (byte)(data[0] >> 4);
                            data[0] *= 16;
                            data[0] |= v15;
                            var v28 = (byte)((data[1] >> 5) & 1);
                            if (((data[1] >> 7) & 1) != 0)
                            {
                                data[1] |= 0x20;
                            }
                            else
                            {
                                data[1] &= 0xDF;
                            }

                            if (v28 != 0)
                            {
                                data[1] |= 0x80;
                            }
                            else
                            {
                                data[1] &= 0x7F;
                            }

                            var v27 = (byte)((data[4] >> 7) & 1);
                            if (((data[4] >> 6) & 1) != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            if (v27 != 0)
                            {
                                data[4] |= 0x40;
                            }
                            else
                            {
                                data[4] &= 0xBF;
                            }

                            var v26 = (byte)((data[10] >> 1) & 1);
                            if (((data[10] >> 4) & 1) != 0)
                            {
                                data[10] |= 2;
                            }
                            else
                            {
                                data[10] &= 0xFD;
                            }

                            if (v26 != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            var v25 = (byte)((data[12] >> 3) & 1);
                            if (((data[12] >> 2) & 1) != 0)
                            {
                                data[12] |= 8;
                            }
                            else
                            {
                                data[12] &= 0xF7;
                            }

                            if (v25 != 0)
                            {
                                data[12] |= 4;
                            }
                            else
                            {
                                data[12] &= 0xFB;
                            }

                            data[8] ^= 0x17;
                        }
                    }
                    else
                    {
                        var v7 = data[5];
                        data[5] = data[2];
                        data[2] = v7;
                        var v31 = (byte)((data[2] >> 2) & 1);
                        if (((data[2] >> 3) & 1) != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        if (v31 != 0)
                        {
                            data[2] |= 8;
                        }
                        else
                        {
                            data[2] &= 0xF7;
                        }

                        var v8 = data[4];
                        data[4] = data[7];
                        data[7] = v8;
                        var v30 = (byte)((data[0] >> 4) & 1);
                        if (((data[0] >> 4) & 1) != 0)
                        {
                            data[0] |= 0x10;
                        }
                        else
                        {
                            data[0] &= 0xEF;
                        }

                        if (v30 != 0)
                        {
                            data[0] |= 0x10;
                        }
                        else
                        {
                            data[0] &= 0xEF;
                        }

                        var v9 = data[1];
                        data[1] = data[0];
                        data[0] = v9;
                        var v29 = (byte)((data[3] >> 4) & 1);
                        if (((data[3] >> 1) & 1) != 0)
                        {
                            data[3] |= 0x10;
                        }
                        else
                        {
                            data[3] &= 0xEF;
                        }

                        if (v29 != 0)
                        {
                            data[3] |= 2;
                        }
                        else
                        {
                            data[3] &= 0xFD;
                        }

                        var v10 = (byte)(data[4] >> 4);
                        data[4] *= 16;
                        data[4] |= v10;
                        data[2] ^= 0x5A;
                    }
                }
                else
                {
                    var v3 = (byte)(data[1] >> 2);
                    data[1] <<= 6;
                    data[1] |= v3;
                    data[0] ^= 0xC9;
                    var v2 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    data[2] ^= 0xDF;
                    data[0] ^= 0x1B;
                    var v33 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v33 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    data[0] ^= 0x6D;
                    var v4 = data[2];
                    data[2] = data[0];
                    data[0] = v4;
                    var v32 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 6) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v32 != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }

                    data[0] ^= 0x2F;
                    data[2] ^= 0xE4;
                    var v5 = (byte)(data[2] >> 4);
                    data[2] *= 16;
                    data[2] |= v5;
                    var v6 = data[2];
                    data[2] = data[3];
                    data[3] = v6;
                }
            }
        }
    }
}
// <copyright file="PacketTwister79.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of an unknown type.
    /// </summary>
    internal class PacketTwister79 : IPacketTwister
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
                            var v25 = (byte)((data[11] >> 4) & 1);
                            if (((data[11] >> 1) & 1) != 0)
                            {
                                data[11] |= 0x10;
                            }
                            else
                            {
                                data[11] &= 0xEF;
                            }

                            if (v25 != 0)
                            {
                                data[11] |= 2;
                            }
                            else
                            {
                                data[11] &= 0xFD;
                            }

                            data[13] ^= 0xF3;
                            data[18] ^= 0xB5;
                            var v21 = data[9];
                            data[9] = data[27];
                            data[27] = v21;
                            var v22 = (byte)(data[30] >> 3);
                            data[30] *= 32;
                            data[30] |= v22;
                            var v23 = data[25];
                            data[25] = data[4];
                            data[4] = v23;
                            var v24 = (byte)(data[4] >> 7);
                            data[4] *= 2;
                            data[4] |= v24;
                        }
                        else
                        {
                            var v30 = (byte)((data[12] >> 2) & 1);
                            if (((data[12] >> 3) & 1) != 0)
                            {
                                data[12] |= 4;
                            }
                            else
                            {
                                data[12] &= 0xFB;
                            }

                            if (v30 != 0)
                            {
                                data[12] |= 8;
                            }
                            else
                            {
                                data[12] &= 0xF7;
                            }

                            var v29 = (byte)((data[9] >> 6) & 1);
                            if (((data[9] >> 5) & 1) != 0)
                            {
                                data[9] |= 0x40;
                            }
                            else
                            {
                                data[9] &= 0xBF;
                            }

                            if (v29 != 0)
                            {
                                data[9] |= 0x20;
                            }
                            else
                            {
                                data[9] &= 0xDF;
                            }

                            data[8] ^= 0x7F;
                            var v28 = (byte)((data[7] >> 7) & 1);
                            if (((data[7] >> 3) & 1) != 0)
                            {
                                data[7] |= 0x80;
                            }
                            else
                            {
                                data[7] &= 0x7F;
                            }

                            if (v28 != 0)
                            {
                                data[7] |= 8;
                            }
                            else
                            {
                                data[7] &= 0xF7;
                            }

                            var v27 = (byte)((data[15] >> 2) & 1);
                            if (((data[15] >> 3) & 1) != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            if (v27 != 0)
                            {
                                data[15] |= 8;
                            }
                            else
                            {
                                data[15] &= 0xF7;
                            }

                            var v17 = data[3];
                            data[3] = data[13];
                            data[13] = v17;
                            data[1] ^= 0x89;
                            var v18 = (byte)(data[4] >> 6);
                            data[4] *= 4;
                            data[4] |= v18;
                            var v19 = data[14];
                            data[14] = data[8];
                            data[8] = v19;
                            var v26 = (byte)((data[2] >> 1) & 1);
                            if (((data[2] >> 3) & 1) != 0)
                            {
                                data[2] |= 2;
                            }
                            else
                            {
                                data[2] &= 0xFD;
                            }

                            if (v26 != 0)
                            {
                                data[2] |= 8;
                            }
                            else
                            {
                                data[2] &= 0xF7;
                            }

                            data[12] ^= 0x76;
                            var v20 = (byte)(data[0] >> 6);
                            data[0] *= 4;
                            data[0] |= v20;
                        }
                    }
                    else
                    {
                        data[5] ^= 0x78;
                        var v13 = data[5];
                        data[5] = data[4];
                        data[4] = v13;
                        var v32 = (byte)((data[0] >> 4) & 1);
                        if (((data[0] >> 5) & 1) != 0)
                        {
                            data[0] |= 0x10;
                        }
                        else
                        {
                            data[0] &= 0xEF;
                        }

                        if (v32 != 0)
                        {
                            data[0] |= 0x20;
                        }
                        else
                        {
                            data[0] &= 0xDF;
                        }

                        data[0] ^= 0xB8;
                        var v31 = (byte)((data[4] >> 7) & 1);
                        if (((data[4] >> 1) & 1) != 0)
                        {
                            data[4] |= 0x80;
                        }
                        else
                        {
                            data[4] &= 0x7F;
                        }

                        if (v31 != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        var v14 = (byte)(data[7] >> 4);
                        data[7] *= 16;
                        data[7] |= v14;
                        var v15 = data[4];
                        data[4] = data[6];
                        data[6] = v15;
                        data[4] ^= 0x4F;
                        data[0] ^= 0xD;
                        var v16 = data[7];
                        data[7] = data[2];
                        data[2] = v16;
                    }
                }
                else
                {
                    var v3 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v3;
                    var v4 = (byte)(data[3] >> 3);
                    data[3] *= 32;
                    data[3] |= v4;
                    var v5 = data[3];
                    data[3] = data[3];
                    data[3] = v5;
                    data[1] ^= 0xA7;
                    var v6 = data[1];
                    data[1] = data[0];
                    data[0] = v6;
                    var v7 = data[3];
                    data[3] = data[0];
                    data[0] = v7;
                    var v8 = data[3];
                    data[3] = data[0];
                    data[0] = v8;
                    var v9 = (byte)(data[2] >> 1);
                    data[2] <<= 7;
                    data[2] |= v9;
                    var v10 = data[0];
                    data[0] = data[1];
                    data[1] = v10;
                    var v11 = (byte)(data[2] >> 4);
                    data[2] *= 16;
                    data[2] |= v11;
                    data[2] ^= 0xCF;
                    var v12 = data[0];
                    data[0] = data[2];
                    data[2] = v12;
                    var v2 = (byte)((data[3] >> 6) & 1);
                    if (((data[3] >> 6) & 1) != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
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
                            var v21 = (byte)(data[4] >> 1);
                            data[4] <<= 7;
                            data[4] |= v21;
                            var v22 = data[25];
                            data[25] = data[4];
                            data[4] = v22;
                            var v23 = (byte)(data[30] >> 5);
                            data[30] *= 8;
                            data[30] |= v23;
                            var v24 = data[9];
                            data[9] = data[27];
                            data[27] = v24;
                            data[18] ^= 0xB5;
                            data[13] ^= 0xF3;
                            var v25 = (byte)((data[11] >> 4) & 1);
                            if (((data[11] >> 1) & 1) != 0)
                            {
                                data[11] |= 0x10;
                            }
                            else
                            {
                                data[11] &= 0xEF;
                            }

                            if (v25 != 0)
                            {
                                data[11] |= 2;
                            }
                            else
                            {
                                data[11] &= 0xFD;
                            }
                        }
                        else
                        {
                            var v17 = (byte)(data[0] >> 2);
                            data[0] <<= 6;
                            data[0] |= v17;
                            data[12] ^= 0x76;
                            var v30 = (byte)((data[2] >> 1) & 1);
                            if (((data[2] >> 3) & 1) != 0)
                            {
                                data[2] |= 2;
                            }
                            else
                            {
                                data[2] &= 0xFD;
                            }

                            if (v30 != 0)
                            {
                                data[2] |= 8;
                            }
                            else
                            {
                                data[2] &= 0xF7;
                            }

                            var v18 = data[14];
                            data[14] = data[8];
                            data[8] = v18;
                            var v19 = (byte)(data[4] >> 2);
                            data[4] <<= 6;
                            data[4] |= v19;
                            data[1] ^= 0x89;
                            var v20 = data[3];
                            data[3] = data[13];
                            data[13] = v20;
                            var v29 = (byte)((data[15] >> 2) & 1);
                            if (((data[15] >> 3) & 1) != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            if (v29 != 0)
                            {
                                data[15] |= 8;
                            }
                            else
                            {
                                data[15] &= 0xF7;
                            }

                            var v28 = (byte)((data[7] >> 7) & 1);
                            if (((data[7] >> 3) & 1) != 0)
                            {
                                data[7] |= 0x80;
                            }
                            else
                            {
                                data[7] &= 0x7F;
                            }

                            if (v28 != 0)
                            {
                                data[7] |= 8;
                            }
                            else
                            {
                                data[7] &= 0xF7;
                            }

                            data[8] ^= 0x7F;
                            var v27 = (byte)((data[9] >> 6) & 1);
                            if (((data[9] >> 5) & 1) != 0)
                            {
                                data[9] |= 0x40;
                            }
                            else
                            {
                                data[9] &= 0xBF;
                            }

                            if (v27 != 0)
                            {
                                data[9] |= 0x20;
                            }
                            else
                            {
                                data[9] &= 0xDF;
                            }

                            var v26 = (byte)((data[12] >> 2) & 1);
                            if (((data[12] >> 3) & 1) != 0)
                            {
                                data[12] |= 4;
                            }
                            else
                            {
                                data[12] &= 0xFB;
                            }

                            if (v26 != 0)
                            {
                                data[12] |= 8;
                            }
                            else
                            {
                                data[12] &= 0xF7;
                            }
                        }
                    }
                    else
                    {
                        var v13 = data[7];
                        data[7] = data[2];
                        data[2] = v13;
                        data[0] ^= 0xD;
                        data[4] ^= 0x4F;
                        var v14 = data[4];
                        data[4] = data[6];
                        data[6] = v14;
                        var v15 = (byte)(data[7] >> 4);
                        data[7] *= 16;
                        data[7] |= v15;
                        var v32 = (byte)((data[4] >> 7) & 1);
                        if (((data[4] >> 1) & 1) != 0)
                        {
                            data[4] |= 0x80;
                        }
                        else
                        {
                            data[4] &= 0x7F;
                        }

                        if (v32 != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        data[0] ^= 0xB8;
                        var v31 = (byte)((data[0] >> 4) & 1);
                        if (((data[0] >> 5) & 1) != 0)
                        {
                            data[0] |= 0x10;
                        }
                        else
                        {
                            data[0] &= 0xEF;
                        }

                        if (v31 != 0)
                        {
                            data[0] |= 0x20;
                        }
                        else
                        {
                            data[0] &= 0xDF;
                        }

                        var v16 = data[5];
                        data[5] = data[4];
                        data[4] = v16;
                        data[5] ^= 0x78;
                    }
                }
                else
                {
                    var v2 = (byte)((data[3] >> 6) & 1);
                    if (((data[3] >> 6) & 1) != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }

                    var v3 = data[0];
                    data[0] = data[2];
                    data[2] = v3;
                    data[2] ^= 0xCF;
                    var v4 = (byte)(data[2] >> 4);
                    data[2] *= 16;
                    data[2] |= v4;
                    var v5 = data[0];
                    data[0] = data[1];
                    data[1] = v5;
                    var v6 = (byte)(data[2] >> 7);
                    data[2] *= 2;
                    data[2] |= v6;
                    var v7 = data[3];
                    data[3] = data[0];
                    data[0] = v7;
                    var v8 = data[3];
                    data[3] = data[0];
                    data[0] = v8;
                    var v9 = data[1];
                    data[1] = data[0];
                    data[0] = v9;
                    data[1] ^= 0xA7;
                    var v10 = data[3];
                    data[3] = data[3];
                    data[3] = v10;
                    var v11 = (byte)(data[3] >> 5);
                    data[3] *= 8;
                    data[3] |= v11;
                    var v12 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v12;
                }
            }
        }
    }
}
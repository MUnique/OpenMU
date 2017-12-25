// <copyright file="PacketTwisterOfAnimation.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'Animation' type.
    /// </summary>
    internal class PacketTwisterOfAnimation : IPacketTwister
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
                            data[29] ^= 0x7A;
                            var v23 = (byte)((data[18] >> 7) & 1);
                            if (((data[18] >> 3) & 1) != 0)
                            {
                                data[18] |= 0x80;
                            }
                            else
                            {
                                data[18] &= 0x7F;
                            }

                            if (v23 != 0)
                            {
                                data[18] |= 8;
                            }
                            else
                            {
                                data[18] &= 0xF7;
                            }

                            var v22 = (byte)((data[4] >> 7) & 1);
                            if (((data[4] >> 4) & 1) != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            if (v22 != 0)
                            {
                                data[4] |= 0x10;
                            }
                            else
                            {
                                data[4] &= 0xEF;
                            }

                            var v15 = data[27];
                            data[27] = data[2];
                            data[2] = v15;
                            var v16 = (byte)(data[9] >> 5);
                            data[9] *= 8;
                            data[9] |= v16;
                            var v21 = (byte)((data[19] >> 6) & 1);
                            if (((data[19] >> 3) & 1) != 0)
                            {
                                data[19] |= 0x40;
                            }
                            else
                            {
                                data[19] &= 0xBF;
                            }

                            if (v21 != 0)
                            {
                                data[19] |= 8;
                            }
                            else
                            {
                                data[19] &= 0xF7;
                            }

                            var v17 = (byte)(data[11] >> 1);
                            data[11] <<= 7;
                            data[11] |= v17;
                            var v20 = (byte)((data[27] >> 6) & 1);
                            if (((data[27] >> 5) & 1) != 0)
                            {
                                data[27] |= 0x40;
                            }
                            else
                            {
                                data[27] &= 0xBF;
                            }

                            if (v20 != 0)
                            {
                                data[27] |= 0x20;
                            }
                            else
                            {
                                data[27] &= 0xDF;
                            }

                            var v19 = (byte)((data[24] >> 2) & 1);
                            if (((data[24] >> 6) & 1) != 0)
                            {
                                data[24] |= 4;
                            }
                            else
                            {
                                data[24] &= 0xFB;
                            }

                            if (v19 != 0)
                            {
                                data[24] |= 0x40;
                            }
                            else
                            {
                                data[24] &= 0xBF;
                            }

                            var v18 = (byte)((data[28] >> 6) & 1);
                            if (((data[28] >> 5) & 1) != 0)
                            {
                                data[28] |= 0x40;
                            }
                            else
                            {
                                data[28] &= 0xBF;
                            }

                            if (v18 != 0)
                            {
                                data[28] |= 0x20;
                            }
                            else
                            {
                                data[28] &= 0xDF;
                            }
                        }
                        else
                        {
                            var v9 = (byte)(data[12] >> 5);
                            data[12] *= 8;
                            data[12] |= v9;
                            var v10 = data[8];
                            data[8] = data[1];
                            data[1] = v10;
                            var v11 = (byte)(data[1] >> 6);
                            data[1] *= 4;
                            data[1] |= v11;
                            var v25 = (byte)((data[14] >> 6) & 1);
                            if (((data[14] >> 1) & 1) != 0)
                            {
                                data[14] |= 0x40;
                            }
                            else
                            {
                                data[14] &= 0xBF;
                            }

                            if (v25 != 0)
                            {
                                data[14] |= 2;
                            }
                            else
                            {
                                data[14] &= 0xFD;
                            }

                            var v12 = (byte)(data[7] >> 4);
                            data[7] *= 16;
                            data[7] |= v12;
                            data[8] ^= 0xCE;
                            var v24 = (byte)((data[0] >> 3) & 1);
                            if (((data[0] >> 1) & 1) != 0)
                            {
                                data[0] |= 8;
                            }
                            else
                            {
                                data[0] &= 0xF7;
                            }

                            if (v24 != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            var v13 = (byte)(data[9] >> 6);
                            data[9] *= 4;
                            data[9] |= v13;
                            var v14 = (byte)(data[4] >> 3);
                            data[4] *= 32;
                            data[4] |= v14;
                        }
                    }
                    else
                    {
                        var v3 = (byte)(data[7] >> 5);
                        data[7] *= 8;
                        data[7] |= v3;
                        var v30 = (byte)((data[1] >> 7) & 1);
                        if (((data[1] >> 5) & 1) != 0)
                        {
                            data[1] |= 0x80;
                        }
                        else
                        {
                            data[1] &= 0x7F;
                        }

                        if (v30 != 0)
                        {
                            data[1] |= 0x20;
                        }
                        else
                        {
                            data[1] &= 0xDF;
                        }

                        var v29 = (byte)((data[2] >> 5) & 1);
                        if (((data[2] >> 6) & 1) != 0)
                        {
                            data[2] |= 0x20;
                        }
                        else
                        {
                            data[2] &= 0xDF;
                        }

                        if (v29 != 0)
                        {
                            data[2] |= 0x40;
                        }
                        else
                        {
                            data[2] &= 0xBF;
                        }

                        var v4 = (byte)(data[5] >> 4);
                        data[5] *= 16;
                        data[5] |= v4;
                        var v5 = (byte)(data[0] >> 1);
                        data[0] <<= 7;
                        data[0] |= v5;
                        var v28 = (byte)((data[2] >> 2) & 1);
                        if (((data[2] >> 5) & 1) != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        if (v28 != 0)
                        {
                            data[2] |= 0x20;
                        }
                        else
                        {
                            data[2] &= 0xDF;
                        }

                        var v27 = (byte)((data[6] >> 7) & 1);
                        if (((data[6] >> 7) & 1) != 0)
                        {
                            data[6] |= 0x80;
                        }
                        else
                        {
                            data[6] &= 0x7F;
                        }

                        if (v27 != 0)
                        {
                            data[6] |= 0x80;
                        }
                        else
                        {
                            data[6] &= 0x7F;
                        }

                        var v6 = (byte)(data[7] >> 5);
                        data[7] *= 8;
                        data[7] |= v6;
                        var v7 = data[0];
                        data[0] = data[2];
                        data[2] = v7;
                        var v8 = (byte)(data[6] >> 4);
                        data[6] *= 16;
                        data[6] |= v8;
                        var v26 = (byte)((data[4] >> 7) & 1);
                        if (((data[4] >> 7) & 1) != 0)
                        {
                            data[4] |= 0x80;
                        }
                        else
                        {
                            data[4] &= 0x7F;
                        }

                        if (v26 != 0)
                        {
                            data[4] |= 0x80;
                        }
                        else
                        {
                            data[4] &= 0x7F;
                        }
                    }
                }
                else
                {
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

                    data[1] = data[1];
                    data[1] ^= 0xB2;
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
                            var v23 = (byte)((data[28] >> 6) & 1);
                            if (((data[28] >> 5) & 1) != 0)
                            {
                                data[28] |= 0x40;
                            }
                            else
                            {
                                data[28] &= 0xBF;
                            }

                            if (v23 != 0)
                            {
                                data[28] |= 0x20;
                            }
                            else
                            {
                                data[28] &= 0xDF;
                            }

                            var v22 = (byte)((data[24] >> 2) & 1);
                            if (((data[24] >> 6) & 1) != 0)
                            {
                                data[24] |= 4;
                            }
                            else
                            {
                                data[24] &= 0xFB;
                            }

                            if (v22 != 0)
                            {
                                data[24] |= 0x40;
                            }
                            else
                            {
                                data[24] &= 0xBF;
                            }

                            var v21 = (byte)((data[27] >> 6) & 1);
                            if (((data[27] >> 5) & 1) != 0)
                            {
                                data[27] |= 0x40;
                            }
                            else
                            {
                                data[27] &= 0xBF;
                            }

                            if (v21 != 0)
                            {
                                data[27] |= 0x20;
                            }
                            else
                            {
                                data[27] &= 0xDF;
                            }

                            var v15 = (byte)(data[11] >> 7);
                            data[11] *= 2;
                            data[11] |= v15;
                            var v20 = (byte)((data[19] >> 6) & 1);
                            if (((data[19] >> 3) & 1) != 0)
                            {
                                data[19] |= 0x40;
                            }
                            else
                            {
                                data[19] &= 0xBF;
                            }

                            if (v20 != 0)
                            {
                                data[19] |= 8;
                            }
                            else
                            {
                                data[19] &= 0xF7;
                            }

                            var v16 = (byte)(data[9] >> 3);
                            data[9] *= 32;
                            data[9] |= v16;
                            var v17 = data[27];
                            data[27] = data[2];
                            data[2] = v17;
                            var v19 = (byte)((data[4] >> 7) & 1);
                            if (((data[4] >> 4) & 1) != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            if (v19 != 0)
                            {
                                data[4] |= 0x10;
                            }
                            else
                            {
                                data[4] &= 0xEF;
                            }

                            var v18 = (byte)((data[18] >> 7) & 1);
                            if (((data[18] >> 3) & 1) != 0)
                            {
                                data[18] |= 0x80;
                            }
                            else
                            {
                                data[18] &= 0x7F;
                            }

                            if (v18 != 0)
                            {
                                data[18] |= 8;
                            }
                            else
                            {
                                data[18] &= 0xF7;
                            }

                            data[29] ^= 0x7A;
                        }
                        else
                        {
                            var v9 = (byte)(data[4] >> 5);
                            data[4] *= 8;
                            data[4] |= v9;
                            var v10 = (byte)(data[9] >> 2);
                            data[9] <<= 6;
                            data[9] |= v10;
                            var v25 = (byte)((data[0] >> 3) & 1);
                            if (((data[0] >> 1) & 1) != 0)
                            {
                                data[0] |= 8;
                            }
                            else
                            {
                                data[0] &= 0xF7;
                            }

                            if (v25 != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            data[8] ^= 0xCE;
                            var v11 = (byte)(data[7] >> 4);
                            data[7] *= 16;
                            data[7] |= v11;
                            var v24 = (byte)((data[14] >> 6) & 1);
                            if (((data[14] >> 1) & 1) != 0)
                            {
                                data[14] |= 0x40;
                            }
                            else
                            {
                                data[14] &= 0xBF;
                            }

                            if (v24 != 0)
                            {
                                data[14] |= 2;
                            }
                            else
                            {
                                data[14] &= 0xFD;
                            }

                            var v12 = (byte)(data[1] >> 2);
                            data[1] <<= 6;
                            data[1] |= v12;
                            var v13 = data[8];
                            data[8] = data[1];
                            data[1] = v13;
                            var v14 = (byte)(data[12] >> 3);
                            data[12] *= 32;
                            data[12] |= v14;
                        }
                    }
                    else
                    {
                        var v30 = (byte)((data[4] >> 7) & 1);
                        if (((data[4] >> 7) & 1) != 0)
                        {
                            data[4] |= 0x80;
                        }
                        else
                        {
                            data[4] &= 0x7F;
                        }

                        if (v30 != 0)
                        {
                            data[4] |= 0x80;
                        }
                        else
                        {
                            data[4] &= 0x7F;
                        }

                        var v3 = (byte)(data[6] >> 4);
                        data[6] *= 16;
                        data[6] |= v3;
                        var v4 = data[0];
                        data[0] = data[2];
                        data[2] = v4;
                        var v5 = (byte)(data[7] >> 3);
                        data[7] *= 32;
                        data[7] |= v5;
                        var v29 = (byte)((data[6] >> 7) & 1);
                        if (((data[6] >> 7) & 1) != 0)
                        {
                            data[6] |= 0x80;
                        }
                        else
                        {
                            data[6] &= 0x7F;
                        }

                        if (v29 != 0)
                        {
                            data[6] |= 0x80;
                        }
                        else
                        {
                            data[6] &= 0x7F;
                        }

                        var v28 = (byte)((data[2] >> 2) & 1);
                        if (((data[2] >> 5) & 1) != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        if (v28 != 0)
                        {
                            data[2] |= 0x20;
                        }
                        else
                        {
                            data[2] &= 0xDF;
                        }

                        var v6 = (byte)(data[0] >> 7);
                        data[0] *= 2;
                        data[0] |= v6;
                        var v7 = (byte)(data[5] >> 4);
                        data[5] *= 16;
                        data[5] |= v7;
                        var v27 = (byte)((data[2] >> 5) & 1);
                        if (((data[2] >> 6) & 1) != 0)
                        {
                            data[2] |= 0x20;
                        }
                        else
                        {
                            data[2] &= 0xDF;
                        }

                        if (v27 != 0)
                        {
                            data[2] |= 0x40;
                        }
                        else
                        {
                            data[2] &= 0xBF;
                        }

                        var v26 = (byte)((data[1] >> 7) & 1);
                        if (((data[1] >> 5) & 1) != 0)
                        {
                            data[1] |= 0x80;
                        }
                        else
                        {
                            data[1] &= 0x7F;
                        }

                        if (v26 != 0)
                        {
                            data[1] |= 0x20;
                        }
                        else
                        {
                            data[1] &= 0xDF;
                        }

                        var v8 = (byte)(data[7] >> 3);
                        data[7] *= 32;
                        data[7] |= v8;
                    }
                }
                else
                {
                    data[1] ^= 0xB2;
                    data[1] = data[1];
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
                }
            }
        }
    }
}
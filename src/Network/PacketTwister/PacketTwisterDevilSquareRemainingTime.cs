// <copyright file="PacketTwisterDevilSquareRemainingTime.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'DevilSquareRemainingTime' type.
    /// </summary>
    internal class PacketTwisterDevilSquareRemainingTime : IPacketTwister
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
                            var v17 = data[11];
                            data[11] = data[16];
                            data[16] = v17;
                            var v18 = (byte)(data[29] >> 2);
                            data[29] <<= 6;
                            data[29] |= v18;
                            var v19 = (byte)(data[31] >> 1);
                            data[31] <<= 7;
                            data[31] |= v19;
                            var v20 = data[3];
                            data[3] = data[30];
                            data[30] = v20;
                            var v21 = (byte)(data[31] >> 5);
                            data[31] *= 8;
                            data[31] |= v21;
                            var v22 = data[23];
                            data[23] = data[27];
                            data[27] = v22;
                            var v23 = data[7];
                            data[7] = data[5];
                            data[5] = v23;
                            var v24 = data[1];
                            data[1] = data[19];
                            data[19] = v24;
                            var v25 = data[31];
                            data[31] = data[27];
                            data[27] = v25;
                            var v26 = (byte)(data[5] >> 2);
                            data[5] <<= 6;
                            data[5] |= v26;
                            data[29] ^= 0x2D;
                            var v27 = (byte)(data[28] >> 7);
                            data[28] *= 2;
                            data[28] |= v27;
                        }
                        else
                        {
                            var v14 = (byte)(data[3] >> 4);
                            data[3] *= 16;
                            data[3] |= v14;
                            data[13] ^= 0x1F;
                            var v15 = (byte)(data[12] >> 2);
                            data[12] <<= 6;
                            data[12] |= v15;
                            data[11] ^= 0x88;
                            data[0] ^= 0x90;
                            var v28 = (byte)((data[14] >> 2) & 1);
                            if (((data[14] >> 7) & 1) != 0)
                            {
                                data[14] |= 4;
                            }
                            else
                            {
                                data[14] &= 0xFB;
                            }

                            if (v28 != 0)
                            {
                                data[14] |= 0x80;
                            }
                            else
                            {
                                data[14] &= 0x7F;
                            }

                            var v16 = data[8];
                            data[8] = data[9];
                            data[9] = v16;
                            data[9] ^= 0x28;
                        }
                    }
                    else
                    {
                        var v9 = data[3];
                        data[3] = data[6];
                        data[6] = v9;
                        var v30 = (byte)((data[4] >> 4) & 1);
                        if (((data[4] >> 5) & 1) != 0)
                        {
                            data[4] |= 0x10;
                        }
                        else
                        {
                            data[4] &= 0xEF;
                        }

                        if (v30 != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        var v10 = data[1];
                        data[1] = data[2];
                        data[2] = v10;
                        data[2] ^= 0x5C;
                        var v29 = (byte)((data[4] >> 7) & 1);
                        if (((data[4] >> 5) & 1) != 0)
                        {
                            data[4] |= 0x80;
                        }
                        else
                        {
                            data[4] &= 0x7F;
                        }

                        if (v29 != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        var v11 = data[0];
                        data[0] = data[0];
                        data[0] = v11;
                        data[6] ^= 1;
                        data[7] ^= 0x23;
                        var v12 = data[3];
                        data[3] = data[5];
                        data[5] = v12;
                        var v13 = data[1];
                        data[1] = data[6];
                        data[6] = v13;
                    }
                }
                else
                {
                    var v3 = data[2];
                    data[2] = data[1];
                    data[1] = v3;
                    var v4 = data[1];
                    data[1] = data[3];
                    data[3] = v4;
                    var v5 = data[1];
                    data[1] = data[0];
                    data[0] = v5;
                    data[2] ^= 0x46;
                    var v2 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v6 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v6;
                    var v7 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v7;
                    var v32 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 6) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v32 != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    var v31 = (byte)((data[1] >> 4) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    if (v31 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    var v8 = (byte)(data[2] >> 1);
                    data[2] <<= 7;
                    data[2] |= v8;
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
                            var v17 = (byte)(data[28] >> 1);
                            data[28] <<= 7;
                            data[28] |= v17;
                            data[29] ^= 0x2D;
                            var v18 = (byte)(data[5] >> 6);
                            data[5] *= 4;
                            data[5] |= v18;
                            var v19 = data[31];
                            data[31] = data[27];
                            data[27] = v19;
                            var v20 = data[1];
                            data[1] = data[19];
                            data[19] = v20;
                            var v21 = data[7];
                            data[7] = data[5];
                            data[5] = v21;
                            var v22 = data[23];
                            data[23] = data[27];
                            data[27] = v22;
                            var v23 = (byte)(data[31] >> 3);
                            data[31] *= 32;
                            data[31] |= v23;
                            var v24 = data[3];
                            data[3] = data[30];
                            data[30] = v24;
                            var v25 = (byte)(data[31] >> 7);
                            data[31] *= 2;
                            data[31] |= v25;
                            var v26 = (byte)(data[29] >> 6);
                            data[29] *= 4;
                            data[29] |= v26;
                            var v27 = data[11];
                            data[11] = data[16];
                            data[16] = v27;
                        }
                        else
                        {
                            data[9] ^= 0x28;
                            var v14 = data[8];
                            data[8] = data[9];
                            data[9] = v14;
                            var v28 = (byte)((data[14] >> 2) & 1);
                            if (((data[14] >> 7) & 1) != 0)
                            {
                                data[14] |= 4;
                            }
                            else
                            {
                                data[14] &= 0xFB;
                            }

                            if (v28 != 0)
                            {
                                data[14] |= 0x80;
                            }
                            else
                            {
                                data[14] &= 0x7F;
                            }

                            data[0] ^= 0x90;
                            data[11] ^= 0x88;
                            var v15 = (byte)(data[12] >> 6);
                            data[12] *= 4;
                            data[12] |= v15;
                            data[13] ^= 0x1F;
                            var v16 = (byte)(data[3] >> 4);
                            data[3] *= 16;
                            data[3] |= v16;
                        }
                    }
                    else
                    {
                        var v9 = data[1];
                        data[1] = data[6];
                        data[6] = v9;
                        var v10 = data[3];
                        data[3] = data[5];
                        data[5] = v10;
                        data[7] ^= 0x23;
                        data[6] ^= 1;
                        var v11 = data[0];
                        data[0] = data[0];
                        data[0] = v11;
                        var v30 = (byte)((data[4] >> 7) & 1);
                        if (((data[4] >> 5) & 1) != 0)
                        {
                            data[4] |= 0x80;
                        }
                        else
                        {
                            data[4] &= 0x7F;
                        }

                        if (v30 != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        data[2] ^= 0x5C;
                        var v12 = data[1];
                        data[1] = data[2];
                        data[2] = v12;
                        var v29 = (byte)((data[4] >> 4) & 1);
                        if (((data[4] >> 5) & 1) != 0)
                        {
                            data[4] |= 0x10;
                        }
                        else
                        {
                            data[4] &= 0xEF;
                        }

                        if (v29 != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        var v13 = data[3];
                        data[3] = data[6];
                        data[6] = v13;
                    }
                }
                else
                {
                    var v3 = (byte)(data[2] >> 7);
                    data[2] *= 2;
                    data[2] |= v3;
                    var v2 = (byte)((data[1] >> 4) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    var v32 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 6) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v32 != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    var v4 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v4;
                    var v5 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v5;
                    var v31 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v31 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    data[2] ^= 0x46;
                    var v6 = data[1];
                    data[1] = data[0];
                    data[0] = v6;
                    var v7 = data[1];
                    data[1] = data[3];
                    data[3] = v7;
                    var v8 = data[2];
                    data[2] = data[1];
                    data[1] = v8;
                }
            }
        }
    }
}
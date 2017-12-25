// <copyright file="PacketTwisterOfEventChipRegistration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'EventChipRegistration' type.
    /// </summary>
    internal class PacketTwisterOfEventChipRegistration : IPacketTwister
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
                            var v23 = (byte)((data[15] >> 7) & 1);
                            if (((data[15] >> 1) & 1) != 0)
                            {
                                data[15] |= 0x80;
                            }
                            else
                            {
                                data[15] &= 0x7F;
                            }

                            if (v23 != 0)
                            {
                                data[15] |= 2;
                            }
                            else
                            {
                                data[15] &= 0xFD;
                            }

                            var v22 = (byte)((data[20] >> 7) & 1);
                            if (((data[20] >> 7) & 1) != 0)
                            {
                                data[20] |= 0x80;
                            }
                            else
                            {
                                data[20] &= 0x7F;
                            }

                            if (v22 != 0)
                            {
                                data[20] |= 0x80;
                            }
                            else
                            {
                                data[20] &= 0x7F;
                            }

                            data[3] ^= 0x7B;
                            var v14 = data[19];
                            data[19] = data[19];
                            data[19] = v14;
                            var v15 = (byte)(data[28] >> 5);
                            data[28] *= 8;
                            data[28] |= v15;
                            data[14] ^= 0x92;
                            var v21 = (byte)((data[25] >> 5) & 1);
                            if (((data[25] >> 2) & 1) != 0)
                            {
                                data[25] |= 0x20;
                            }
                            else
                            {
                                data[25] &= 0xDF;
                            }

                            if (v21 != 0)
                            {
                                data[25] |= 4;
                            }
                            else
                            {
                                data[25] &= 0xFB;
                            }

                            var v16 = (byte)(data[7] >> 6);
                            data[7] *= 4;
                            data[7] |= v16;
                            data[31] ^= 0xA3;
                            var v20 = (byte)((data[26] >> 6) & 1);
                            if (((data[26] >> 1) & 1) != 0)
                            {
                                data[26] |= 0x40;
                            }
                            else
                            {
                                data[26] &= 0xBF;
                            }

                            if (v20 != 0)
                            {
                                data[26] |= 2;
                            }
                            else
                            {
                                data[26] &= 0xFD;
                            }

                            var v17 = data[31];
                            data[31] = data[18];
                            data[18] = v17;
                            var v18 = data[4];
                            data[4] = data[17];
                            data[17] = v18;
                            var v19 = (byte)((data[26] >> 5) & 1);
                            if (((data[26] >> 5) & 1) != 0)
                            {
                                data[26] |= 0x20;
                            }
                            else
                            {
                                data[26] &= 0xDF;
                            }

                            if (v19 != 0)
                            {
                                data[26] |= 0x20;
                            }
                            else
                            {
                                data[26] &= 0xDF;
                            }
                        }
                        else
                        {
                            var v9 = (byte)(data[0] >> 7);
                            data[0] *= 2;
                            data[0] |= v9;
                            var v29 = (byte)((data[4] >> 3) & 1);
                            if (((data[4] >> 7) & 1) != 0)
                            {
                                data[4] |= 8;
                            }
                            else
                            {
                                data[4] &= 0xF7;
                            }

                            if (v29 != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            var v10 = data[3];
                            data[3] = data[12];
                            data[12] = v10;
                            var v28 = (byte)((data[2] >> 2) & 1);
                            if (((data[2] >> 7) & 1) != 0)
                            {
                                data[2] |= 4;
                            }
                            else
                            {
                                data[2] &= 0xFB;
                            }

                            if (v28 != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }

                            var v27 = (byte)((data[9] >> 1) & 1);
                            if (((data[9] >> 1) & 1) != 0)
                            {
                                data[9] |= 2;
                            }
                            else
                            {
                                data[9] &= 0xFD;
                            }

                            if (v27 != 0)
                            {
                                data[9] |= 2;
                            }
                            else
                            {
                                data[9] &= 0xFD;
                            }

                            var v11 = data[6];
                            data[6] = data[13];
                            data[13] = v11;
                            var v26 = (byte)((data[6] >> 4) & 1);
                            if (((data[6] >> 1) & 1) != 0)
                            {
                                data[6] |= 0x10;
                            }
                            else
                            {
                                data[6] &= 0xEF;
                            }

                            if (v26 != 0)
                            {
                                data[6] |= 2;
                            }
                            else
                            {
                                data[6] &= 0xFD;
                            }

                            data[6] ^= 0x6A;
                            var v12 = (byte)(data[8] >> 7);
                            data[8] *= 2;
                            data[8] |= v12;
                            var v25 = (byte)((data[5] >> 1) & 1);
                            if (((data[5] >> 7) & 1) != 0)
                            {
                                data[5] |= 2;
                            }
                            else
                            {
                                data[5] &= 0xFD;
                            }

                            if (v25 != 0)
                            {
                                data[5] |= 0x80;
                            }
                            else
                            {
                                data[5] &= 0x7F;
                            }

                            var v24 = (byte)((data[8] >> 2) & 1);
                            if (((data[8] >> 4) & 1) != 0)
                            {
                                data[8] |= 4;
                            }
                            else
                            {
                                data[8] &= 0xFB;
                            }

                            if (v24 != 0)
                            {
                                data[8] |= 0x10;
                            }
                            else
                            {
                                data[8] &= 0xEF;
                            }

                            var v13 = data[15];
                            data[15] = data[1];
                            data[1] = v13;
                            data[12] ^= 0x51;
                        }
                    }
                    else
                    {
                        data[3] ^= 0x6B;
                        var v30 = (byte)((data[1] >> 3) & 1);
                        if (((data[1] >> 6) & 1) != 0)
                        {
                            data[1] |= 8;
                        }
                        else
                        {
                            data[1] &= 0xF7;
                        }

                        if (v30 != 0)
                        {
                            data[1] |= 0x40;
                        }
                        else
                        {
                            data[1] &= 0xBF;
                        }

                        var v7 = data[5];
                        data[5] = data[3];
                        data[3] = v7;
                        data[2] ^= 0xD6;
                        data[4] ^= 0x97;
                        var v8 = (byte)(data[5] >> 6);
                        data[5] *= 4;
                        data[5] |= v8;
                    }
                }
                else
                {
                    var v2 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 3) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    data[2] ^= 0x24;
                    var v3 = data[0];
                    data[0] = data[3];
                    data[3] = v3;
                    var v4 = data[1];
                    data[1] = data[3];
                    data[3] = v4;
                    var v5 = data[1];
                    data[1] = data[0];
                    data[0] = v5;
                    var v33 = (byte)((data[3] >> 3) & 1);
                    if (((data[3] >> 2) & 1) != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    if (v33 != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    var v32 = (byte)((data[0] >> 4) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 0x10;
                    }
                    else
                    {
                        data[0] &= 0xEF;
                    }

                    if (v32 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v6 = (byte)(data[3] >> 3);
                    data[3] *= 32;
                    data[3] |= v6;
                    data[1] ^= 0x94;
                    data[1] ^= 0x45;
                    var v31 = (byte)((data[3] >> 5) & 1);
                    if (((data[3] >> 7) & 1) != 0)
                    {
                        data[3] |= 0x20;
                    }
                    else
                    {
                        data[3] &= 0xDF;
                    }

                    if (v31 != 0)
                    {
                        data[3] |= 0x80;
                    }
                    else
                    {
                        data[3] &= 0x7F;
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
                            var v23 = (byte)((data[26] >> 5) & 1);
                            if (((data[26] >> 5) & 1) != 0)
                            {
                                data[26] |= 0x20;
                            }
                            else
                            {
                                data[26] &= 0xDF;
                            }

                            if (v23 != 0)
                            {
                                data[26] |= 0x20;
                            }
                            else
                            {
                                data[26] &= 0xDF;
                            }

                            var v14 = data[4];
                            data[4] = data[17];
                            data[17] = v14;
                            var v15 = data[31];
                            data[31] = data[18];
                            data[18] = v15;
                            var v22 = (byte)((data[26] >> 6) & 1);
                            if (((data[26] >> 1) & 1) != 0)
                            {
                                data[26] |= 0x40;
                            }
                            else
                            {
                                data[26] &= 0xBF;
                            }

                            if (v22 != 0)
                            {
                                data[26] |= 2;
                            }
                            else
                            {
                                data[26] &= 0xFD;
                            }

                            data[31] ^= 0xA3;
                            var v16 = (byte)(data[7] >> 2);
                            data[7] <<= 6;
                            data[7] |= v16;
                            var v21 = (byte)((data[25] >> 5) & 1);
                            if (((data[25] >> 2) & 1) != 0)
                            {
                                data[25] |= 0x20;
                            }
                            else
                            {
                                data[25] &= 0xDF;
                            }

                            if (v21 != 0)
                            {
                                data[25] |= 4;
                            }
                            else
                            {
                                data[25] &= 0xFB;
                            }

                            data[14] ^= 0x92;
                            var v17 = (byte)(data[28] >> 3);
                            data[28] *= 32;
                            data[28] |= v17;
                            var v18 = data[19];
                            data[19] = data[19];
                            data[19] = v18;
                            data[3] ^= 0x7B;
                            var v20 = (byte)((data[20] >> 7) & 1);
                            if (((data[20] >> 7) & 1) != 0)
                            {
                                data[20] |= 0x80;
                            }
                            else
                            {
                                data[20] &= 0x7F;
                            }

                            if (v20 != 0)
                            {
                                data[20] |= 0x80;
                            }
                            else
                            {
                                data[20] &= 0x7F;
                            }

                            var v19 = (byte)((data[15] >> 7) & 1);
                            if (((data[15] >> 1) & 1) != 0)
                            {
                                data[15] |= 0x80;
                            }
                            else
                            {
                                data[15] &= 0x7F;
                            }

                            if (v19 != 0)
                            {
                                data[15] |= 2;
                            }
                            else
                            {
                                data[15] &= 0xFD;
                            }
                        }
                        else
                        {
                            data[12] ^= 0x51;
                            var v9 = data[15];
                            data[15] = data[1];
                            data[1] = v9;
                            var v29 = (byte)((data[8] >> 2) & 1);
                            if (((data[8] >> 4) & 1) != 0)
                            {
                                data[8] |= 4;
                            }
                            else
                            {
                                data[8] &= 0xFB;
                            }

                            if (v29 != 0)
                            {
                                data[8] |= 0x10;
                            }
                            else
                            {
                                data[8] &= 0xEF;
                            }

                            var v28 = (byte)((data[5] >> 1) & 1);
                            if (((data[5] >> 7) & 1) != 0)
                            {
                                data[5] |= 2;
                            }
                            else
                            {
                                data[5] &= 0xFD;
                            }

                            if (v28 != 0)
                            {
                                data[5] |= 0x80;
                            }
                            else
                            {
                                data[5] &= 0x7F;
                            }

                            var v10 = (byte)(data[8] >> 1);
                            data[8] <<= 7;
                            data[8] |= v10;
                            data[6] ^= 0x6A;
                            var v27 = (byte)((data[6] >> 4) & 1);
                            if (((data[6] >> 1) & 1) != 0)
                            {
                                data[6] |= 0x10;
                            }
                            else
                            {
                                data[6] &= 0xEF;
                            }

                            if (v27 != 0)
                            {
                                data[6] |= 2;
                            }
                            else
                            {
                                data[6] &= 0xFD;
                            }

                            var v11 = data[6];
                            data[6] = data[13];
                            data[13] = v11;
                            var v26 = (byte)((data[9] >> 1) & 1);
                            if (((data[9] >> 1) & 1) != 0)
                            {
                                data[9] |= 2;
                            }
                            else
                            {
                                data[9] &= 0xFD;
                            }

                            if (v26 != 0)
                            {
                                data[9] |= 2;
                            }
                            else
                            {
                                data[9] &= 0xFD;
                            }

                            var v25 = (byte)((data[2] >> 2) & 1);
                            if (((data[2] >> 7) & 1) != 0)
                            {
                                data[2] |= 4;
                            }
                            else
                            {
                                data[2] &= 0xFB;
                            }

                            if (v25 != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }

                            var v12 = data[3];
                            data[3] = data[12];
                            data[12] = v12;
                            var v24 = (byte)((data[4] >> 3) & 1);
                            if (((data[4] >> 7) & 1) != 0)
                            {
                                data[4] |= 8;
                            }
                            else
                            {
                                data[4] &= 0xF7;
                            }

                            if (v24 != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            var v13 = (byte)(data[0] >> 1);
                            data[0] <<= 7;
                            data[0] |= v13;
                        }
                    }
                    else
                    {
                        var v7 = (byte)(data[5] >> 2);
                        data[5] <<= 6;
                        data[5] |= v7;
                        data[4] ^= 0x97;
                        data[2] ^= 0xD6;
                        var v8 = data[5];
                        data[5] = data[3];
                        data[3] = v8;
                        var v30 = (byte)((data[1] >> 3) & 1);
                        if (((data[1] >> 6) & 1) != 0)
                        {
                            data[1] |= 8;
                        }
                        else
                        {
                            data[1] &= 0xF7;
                        }

                        if (v30 != 0)
                        {
                            data[1] |= 0x40;
                        }
                        else
                        {
                            data[1] &= 0xBF;
                        }

                        data[3] ^= 0x6B;
                    }
                }
                else
                {
                    var v2 = (byte)((data[3] >> 5) & 1);
                    if (((data[3] >> 7) & 1) != 0)
                    {
                        data[3] |= 0x20;
                    }
                    else
                    {
                        data[3] &= 0xDF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 0x80;
                    }
                    else
                    {
                        data[3] &= 0x7F;
                    }

                    data[1] ^= 0x45;
                    data[1] ^= 0x94;
                    var v3 = (byte)(data[3] >> 5);
                    data[3] *= 8;
                    data[3] |= v3;
                    var v33 = (byte)((data[0] >> 4) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 0x10;
                    }
                    else
                    {
                        data[0] &= 0xEF;
                    }

                    if (v33 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v32 = (byte)((data[3] >> 3) & 1);
                    if (((data[3] >> 2) & 1) != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    if (v32 != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    var v4 = data[1];
                    data[1] = data[0];
                    data[0] = v4;
                    var v5 = data[1];
                    data[1] = data[3];
                    data[3] = v5;
                    var v6 = data[0];
                    data[0] = data[3];
                    data[3] = v6;
                    data[2] ^= 0x24;
                    var v31 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 3) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v31 != 0)
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
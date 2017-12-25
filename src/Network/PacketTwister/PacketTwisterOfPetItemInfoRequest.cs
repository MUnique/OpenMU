// <copyright file="PacketTwisterOfPetItemInfoRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'PetItemInfoRequest' type.
    /// </summary>
    internal class PacketTwisterOfPetItemInfoRequest : IPacketTwister
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
                            var v25 = (byte)((data[12] >> 4) & 1);
                            if (((data[12] >> 2) & 1) != 0)
                            {
                                data[12] |= 0x10;
                            }
                            else
                            {
                                data[12] &= 0xEF;
                            }

                            if (v25 != 0)
                            {
                                data[12] |= 4;
                            }
                            else
                            {
                                data[12] &= 0xFB;
                            }

                            var v16 = data[2];
                            data[2] = data[9];
                            data[9] = v16;
                            var v24 = (byte)((data[10] >> 2) & 1);
                            if (((data[10] >> 6) & 1) != 0)
                            {
                                data[10] |= 4;
                            }
                            else
                            {
                                data[10] &= 0xFB;
                            }

                            if (v24 != 0)
                            {
                                data[10] |= 0x40;
                            }
                            else
                            {
                                data[10] &= 0xBF;
                            }

                            var v17 = data[8];
                            data[8] = data[23];
                            data[23] = v17;
                            var v18 = (byte)(data[29] >> 6);
                            data[29] *= 4;
                            data[29] |= v18;
                            var v19 = data[25];
                            data[25] = data[8];
                            data[8] = v19;
                            var v20 = (byte)(data[1] >> 1);
                            data[1] <<= 7;
                            data[1] |= v20;
                            var v23 = (byte)((data[15] >> 2) & 1);
                            if (((data[15] >> 2) & 1) != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            if (v23 != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            data[6] ^= 0x90;
                            var v21 = data[16];
                            data[16] = data[17];
                            data[17] = v21;
                            var v22 = (byte)(data[27] >> 3);
                            data[27] *= 32;
                            data[27] |= v22;
                            data[13] ^= 0x81;
                        }
                        else
                        {
                            var v31 = (byte)((data[7] >> 3) & 1);
                            if (((data[7] >> 6) & 1) != 0)
                            {
                                data[7] |= 8;
                            }
                            else
                            {
                                data[7] &= 0xF7;
                            }

                            if (v31 != 0)
                            {
                                data[7] |= 0x40;
                            }
                            else
                            {
                                data[7] &= 0xBF;
                            }

                            var v13 = data[0];
                            data[0] = data[14];
                            data[14] = v13;
                            var v14 = (byte)(data[1] >> 2);
                            data[1] <<= 6;
                            data[1] |= v14;
                            var v30 = (byte)((data[6] >> 3) & 1);
                            if (((data[6] >> 3) & 1) != 0)
                            {
                                data[6] |= 8;
                            }
                            else
                            {
                                data[6] &= 0xF7;
                            }

                            if (v30 != 0)
                            {
                                data[6] |= 8;
                            }
                            else
                            {
                                data[6] &= 0xF7;
                            }

                            var v29 = (byte)((data[5] >> 4) & 1);
                            if (((data[5] >> 5) & 1) != 0)
                            {
                                data[5] |= 0x10;
                            }
                            else
                            {
                                data[5] &= 0xEF;
                            }

                            if (v29 != 0)
                            {
                                data[5] |= 0x20;
                            }
                            else
                            {
                                data[5] &= 0xDF;
                            }

                            var v28 = (byte)((data[12] >> 6) & 1);
                            if (((data[12] >> 3) & 1) != 0)
                            {
                                data[12] |= 0x40;
                            }
                            else
                            {
                                data[12] &= 0xBF;
                            }

                            if (v28 != 0)
                            {
                                data[12] |= 8;
                            }
                            else
                            {
                                data[12] &= 0xF7;
                            }

                            data[5] ^= 0xA9;
                            var v27 = (byte)((data[4] >> 3) & 1);
                            if (((data[4] >> 6) & 1) != 0)
                            {
                                data[4] |= 8;
                            }
                            else
                            {
                                data[4] &= 0xF7;
                            }

                            if (v27 != 0)
                            {
                                data[4] |= 0x40;
                            }
                            else
                            {
                                data[4] &= 0xBF;
                            }

                            data[15] ^= 0x38;
                            data[0] ^= 0xE8;
                            var v26 = (byte)((data[5] >> 1) & 1);
                            if (((data[5] >> 1) & 1) != 0)
                            {
                                data[5] |= 2;
                            }
                            else
                            {
                                data[5] &= 0xFD;
                            }

                            if (v26 != 0)
                            {
                                data[5] |= 2;
                            }
                            else
                            {
                                data[5] &= 0xFD;
                            }

                            data[7] ^= 0x50;
                            var v15 = (byte)(data[14] >> 4);
                            data[14] *= 16;
                            data[14] |= v15;
                        }
                    }
                    else
                    {
                        data[2] ^= 0x98;
                        var v35 = (byte)((data[5] >> 4) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 0x10;
                        }
                        else
                        {
                            data[5] &= 0xEF;
                        }

                        if (v35 != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        var v9 = data[2];
                        data[2] = data[5];
                        data[5] = v9;
                        var v34 = (byte)((data[7] >> 7) & 1);
                        if (((data[7] >> 5) & 1) != 0)
                        {
                            data[7] |= 0x80;
                        }
                        else
                        {
                            data[7] &= 0x7F;
                        }

                        if (v34 != 0)
                        {
                            data[7] |= 0x20;
                        }
                        else
                        {
                            data[7] &= 0xDF;
                        }

                        var v10 = (byte)(data[3] >> 5);
                        data[3] *= 8;
                        data[3] |= v10;
                        var v33 = (byte)((data[3] >> 5) & 1);
                        if (((data[3] >> 7) & 1) != 0)
                        {
                            data[3] |= 0x20;
                        }
                        else
                        {
                            data[3] &= 0xDF;
                        }

                        if (v33 != 0)
                        {
                            data[3] |= 0x80;
                        }
                        else
                        {
                            data[3] &= 0x7F;
                        }

                        data[4] ^= 0x41;
                        data[3] ^= 0x44;
                        var v11 = (byte)(data[2] >> 6);
                        data[2] *= 4;
                        data[2] |= v11;
                        var v12 = (byte)(data[2] >> 4);
                        data[2] *= 16;
                        data[2] |= v12;
                        var v32 = (byte)((data[0] >> 6) & 1);
                        if (((data[0] >> 1) & 1) != 0)
                        {
                            data[0] |= 0x40;
                        }
                        else
                        {
                            data[0] &= 0xBF;
                        }

                        if (v32 != 0)
                        {
                            data[0] |= 2;
                        }
                        else
                        {
                            data[0] &= 0xFD;
                        }
                    }
                }
                else
                {
                    data[1] ^= 0xFC;
                    var v3 = (byte)(data[2] >> 3);
                    data[2] *= 32;
                    data[2] |= v3;
                    var v4 = (byte)(data[1] >> 2);
                    data[1] <<= 6;
                    data[1] |= v4;
                    data[0] ^= 0xFD;
                    var v2 = (byte)((data[3] >> 3) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    var v5 = data[3];
                    data[3] = data[2];
                    data[2] = v5;
                    var v6 = (byte)(data[2] >> 3);
                    data[2] *= 32;
                    data[2] |= v6;
                    var v7 = data[1];
                    data[1] = data[2];
                    data[2] = v7;
                    var v36 = (byte)((data[0] >> 4) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 0x10;
                    }
                    else
                    {
                        data[0] &= 0xEF;
                    }

                    if (v36 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v8 = data[1];
                    data[1] = data[2];
                    data[2] = v8;
                    data[3] ^= 0x66;
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
                            data[13] ^= 0x81;
                            var v16 = (byte)(data[27] >> 5);
                            data[27] *= 8;
                            data[27] |= v16;
                            var v17 = data[16];
                            data[16] = data[17];
                            data[17] = v17;
                            data[6] ^= 0x90;
                            var v25 = (byte)((data[15] >> 2) & 1);
                            if (((data[15] >> 2) & 1) != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            if (v25 != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            var v18 = (byte)(data[1] >> 7);
                            data[1] *= 2;
                            data[1] |= v18;
                            var v19 = data[25];
                            data[25] = data[8];
                            data[8] = v19;
                            var v20 = (byte)(data[29] >> 2);
                            data[29] <<= 6;
                            data[29] |= v20;
                            var v21 = data[8];
                            data[8] = data[23];
                            data[23] = v21;
                            var v24 = (byte)((data[10] >> 2) & 1);
                            if (((data[10] >> 6) & 1) != 0)
                            {
                                data[10] |= 4;
                            }
                            else
                            {
                                data[10] &= 0xFB;
                            }

                            if (v24 != 0)
                            {
                                data[10] |= 0x40;
                            }
                            else
                            {
                                data[10] &= 0xBF;
                            }

                            var v22 = data[2];
                            data[2] = data[9];
                            data[9] = v22;
                            var v23 = (byte)((data[12] >> 4) & 1);
                            if (((data[12] >> 2) & 1) != 0)
                            {
                                data[12] |= 0x10;
                            }
                            else
                            {
                                data[12] &= 0xEF;
                            }

                            if (v23 != 0)
                            {
                                data[12] |= 4;
                            }
                            else
                            {
                                data[12] &= 0xFB;
                            }
                        }
                        else
                        {
                            var v13 = (byte)(data[14] >> 4);
                            data[14] *= 16;
                            data[14] |= v13;
                            data[7] ^= 0x50;
                            var v31 = (byte)((data[5] >> 1) & 1);
                            if (((data[5] >> 1) & 1) != 0)
                            {
                                data[5] |= 2;
                            }
                            else
                            {
                                data[5] &= 0xFD;
                            }

                            if (v31 != 0)
                            {
                                data[5] |= 2;
                            }
                            else
                            {
                                data[5] &= 0xFD;
                            }

                            data[0] ^= 0xE8;
                            data[15] ^= 0x38;
                            var v30 = (byte)((data[4] >> 3) & 1);
                            if (((data[4] >> 6) & 1) != 0)
                            {
                                data[4] |= 8;
                            }
                            else
                            {
                                data[4] &= 0xF7;
                            }

                            if (v30 != 0)
                            {
                                data[4] |= 0x40;
                            }
                            else
                            {
                                data[4] &= 0xBF;
                            }

                            data[5] ^= 0xA9;
                            var v29 = (byte)((data[12] >> 6) & 1);
                            if (((data[12] >> 3) & 1) != 0)
                            {
                                data[12] |= 0x40;
                            }
                            else
                            {
                                data[12] &= 0xBF;
                            }

                            if (v29 != 0)
                            {
                                data[12] |= 8;
                            }
                            else
                            {
                                data[12] &= 0xF7;
                            }

                            var v28 = (byte)((data[5] >> 4) & 1);
                            if (((data[5] >> 5) & 1) != 0)
                            {
                                data[5] |= 0x10;
                            }
                            else
                            {
                                data[5] &= 0xEF;
                            }

                            if (v28 != 0)
                            {
                                data[5] |= 0x20;
                            }
                            else
                            {
                                data[5] &= 0xDF;
                            }

                            var v27 = (byte)((data[6] >> 3) & 1);
                            if (((data[6] >> 3) & 1) != 0)
                            {
                                data[6] |= 8;
                            }
                            else
                            {
                                data[6] &= 0xF7;
                            }

                            if (v27 != 0)
                            {
                                data[6] |= 8;
                            }
                            else
                            {
                                data[6] &= 0xF7;
                            }

                            var v14 = (byte)(data[1] >> 6);
                            data[1] *= 4;
                            data[1] |= v14;
                            var v15 = data[0];
                            data[0] = data[14];
                            data[14] = v15;
                            var v26 = (byte)((data[7] >> 3) & 1);
                            if (((data[7] >> 6) & 1) != 0)
                            {
                                data[7] |= 8;
                            }
                            else
                            {
                                data[7] &= 0xF7;
                            }

                            if (v26 != 0)
                            {
                                data[7] |= 0x40;
                            }
                            else
                            {
                                data[7] &= 0xBF;
                            }
                        }
                    }
                    else
                    {
                        var v35 = (byte)((data[0] >> 6) & 1);
                        if (((data[0] >> 1) & 1) != 0)
                        {
                            data[0] |= 0x40;
                        }
                        else
                        {
                            data[0] &= 0xBF;
                        }

                        if (v35 != 0)
                        {
                            data[0] |= 2;
                        }
                        else
                        {
                            data[0] &= 0xFD;
                        }

                        var v9 = (byte)(data[2] >> 4);
                        data[2] *= 16;
                        data[2] |= v9;
                        var v10 = (byte)(data[2] >> 2);
                        data[2] <<= 6;
                        data[2] |= v10;
                        data[3] ^= 0x44;
                        data[4] ^= 0x41;
                        var v34 = (byte)((data[3] >> 5) & 1);
                        if (((data[3] >> 7) & 1) != 0)
                        {
                            data[3] |= 0x20;
                        }
                        else
                        {
                            data[3] &= 0xDF;
                        }

                        if (v34 != 0)
                        {
                            data[3] |= 0x80;
                        }
                        else
                        {
                            data[3] &= 0x7F;
                        }

                        var v11 = (byte)(data[3] >> 3);
                        data[3] *= 32;
                        data[3] |= v11;
                        var v33 = (byte)((data[7] >> 7) & 1);
                        if (((data[7] >> 5) & 1) != 0)
                        {
                            data[7] |= 0x80;
                        }
                        else
                        {
                            data[7] &= 0x7F;
                        }

                        if (v33 != 0)
                        {
                            data[7] |= 0x20;
                        }
                        else
                        {
                            data[7] &= 0xDF;
                        }

                        var v12 = data[2];
                        data[2] = data[5];
                        data[5] = v12;
                        var v32 = (byte)((data[5] >> 4) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 0x10;
                        }
                        else
                        {
                            data[5] &= 0xEF;
                        }

                        if (v32 != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        data[2] ^= 0x98;
                    }
                }
                else
                {
                    data[1] ^= 0xB2;
                    data[3] ^= 0x66;
                    var v3 = data[1];
                    data[1] = data[2];
                    data[2] = v3;
                    var v2 = (byte)((data[0] >> 4) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 0x10;
                    }
                    else
                    {
                        data[0] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v4 = data[1];
                    data[1] = data[2];
                    data[2] = v4;
                    var v5 = (byte)(data[2] >> 5);
                    data[2] *= 8;
                    data[2] |= v5;
                    var v6 = data[3];
                    data[3] = data[2];
                    data[2] = v6;
                    var v36 = (byte)((data[3] >> 3) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    if (v36 != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    data[0] ^= 0xFD;
                    var v7 = (byte)(data[1] >> 6);
                    data[1] *= 4;
                    data[1] |= v7;
                    var v8 = (byte)(data[2] >> 5);
                    data[2] *= 8;
                    data[2] |= v8;
                    data[1] ^= 0xFC;
                }
            }
        }
    }
}
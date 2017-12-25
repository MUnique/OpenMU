// <copyright file="PacketTwisterOfItemRepairRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'ItemRepairRequest' type.
    /// </summary>
    internal class PacketTwisterOfItemRepairRequest : IPacketTwister
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
                            data[21] ^= 0xAF;
                            var v16 = data[20];
                            data[20] = data[2];
                            data[2] = v16;
                            var v24 = (byte)((data[27] >> 7) & 1);
                            if (((data[27] >> 2) & 1) != 0)
                            {
                                data[27] |= 0x80;
                            }
                            else
                            {
                                data[27] &= 0x7F;
                            }

                            if (v24 != 0)
                            {
                                data[27] |= 4;
                            }
                            else
                            {
                                data[27] &= 0xFB;
                            }

                            var v17 = (byte)(data[22] >> 5);
                            data[22] *= 8;
                            data[22] |= v17;
                            data[9] ^= 0x12;
                            var v23 = (byte)((data[22] >> 6) & 1);
                            if (((data[22] >> 3) & 1) != 0)
                            {
                                data[22] |= 0x40;
                            }
                            else
                            {
                                data[22] &= 0xBF;
                            }

                            if (v23 != 0)
                            {
                                data[22] |= 8;
                            }
                            else
                            {
                                data[22] &= 0xF7;
                            }

                            var v18 = (byte)(data[20] >> 4);
                            data[20] *= 16;
                            data[20] |= v18;
                            var v22 = (byte)((data[10] >> 4) & 1);
                            if (((data[10] >> 5) & 1) != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            if (v22 != 0)
                            {
                                data[10] |= 0x20;
                            }
                            else
                            {
                                data[10] &= 0xDF;
                            }

                            var v19 = data[13];
                            data[13] = data[25];
                            data[25] = v19;
                            var v21 = (byte)((data[26] >> 3) & 1);
                            if (((data[26] >> 1) & 1) != 0)
                            {
                                data[26] |= 8;
                            }
                            else
                            {
                                data[26] &= 0xF7;
                            }

                            if (v21 != 0)
                            {
                                data[26] |= 2;
                            }
                            else
                            {
                                data[26] &= 0xFD;
                            }

                            data[26] ^= 0x92;
                            data[28] ^= 0xBA;
                            var v20 = (byte)(data[16] >> 2);
                            data[16] <<= 6;
                            data[16] |= v20;
                        }
                        else
                        {
                            var v30 = (byte)((data[10] >> 1) & 1);
                            if (((data[10] >> 3) & 1) != 0)
                            {
                                data[10] |= 2;
                            }
                            else
                            {
                                data[10] &= 0xFD;
                            }

                            if (v30 != 0)
                            {
                                data[10] |= 8;
                            }
                            else
                            {
                                data[10] &= 0xF7;
                            }

                            var v12 = (byte)(data[9] >> 1);
                            data[9] <<= 7;
                            data[9] |= v12;
                            data[3] ^= 0x57;
                            data[14] ^= 1;
                            var v29 = (byte)((data[15] >> 6) & 1);
                            if (((data[15] >> 3) & 1) != 0)
                            {
                                data[15] |= 0x40;
                            }
                            else
                            {
                                data[15] &= 0xBF;
                            }

                            if (v29 != 0)
                            {
                                data[15] |= 8;
                            }
                            else
                            {
                                data[15] &= 0xF7;
                            }

                            data[13] ^= 0x84;
                            var v28 = (byte)((data[1] >> 2) & 1);
                            if (((data[1] >> 1) & 1) != 0)
                            {
                                data[1] |= 4;
                            }
                            else
                            {
                                data[1] &= 0xFB;
                            }

                            if (v28 != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }

                            var v13 = (byte)(data[3] >> 3);
                            data[3] *= 32;
                            data[3] |= v13;
                            var v14 = (byte)(data[2] >> 5);
                            data[2] *= 8;
                            data[2] |= v14;
                            var v27 = (byte)((data[7] >> 3) & 1);
                            if (((data[7] >> 3) & 1) != 0)
                            {
                                data[7] |= 8;
                            }
                            else
                            {
                                data[7] &= 0xF7;
                            }

                            if (v27 != 0)
                            {
                                data[7] |= 8;
                            }
                            else
                            {
                                data[7] &= 0xF7;
                            }

                            var v26 = (byte)((data[6] >> 4) & 1);
                            if (((data[6] >> 7) & 1) != 0)
                            {
                                data[6] |= 0x10;
                            }
                            else
                            {
                                data[6] &= 0xEF;
                            }

                            if (v26 != 0)
                            {
                                data[6] |= 0x80;
                            }
                            else
                            {
                                data[6] &= 0x7F;
                            }

                            var v25 = (byte)((data[3] >> 6) & 1);
                            if (((data[3] >> 6) & 1) != 0)
                            {
                                data[3] |= 0x40;
                            }
                            else
                            {
                                data[3] &= 0xBF;
                            }

                            if (v25 != 0)
                            {
                                data[3] |= 0x40;
                            }
                            else
                            {
                                data[3] &= 0xBF;
                            }

                            var v15 = data[2];
                            data[2] = data[4];
                            data[4] = v15;
                        }
                    }
                    else
                    {
                        var v9 = data[7];
                        data[7] = data[5];
                        data[5] = v9;
                        var v32 = (byte)((data[4] >> 2) & 1);
                        if (((data[4] >> 6) & 1) != 0)
                        {
                            data[4] |= 4;
                        }
                        else
                        {
                            data[4] &= 0xFB;
                        }

                        if (v32 != 0)
                        {
                            data[4] |= 0x40;
                        }
                        else
                        {
                            data[4] &= 0xBF;
                        }

                        var v10 = (byte)(data[6] >> 4);
                        data[6] *= 16;
                        data[6] |= v10;
                        var v31 = (byte)((data[4] >> 7) & 1);
                        if (((data[4] >> 4) & 1) != 0)
                        {
                            data[4] |= 0x80;
                        }
                        else
                        {
                            data[4] &= 0x7F;
                        }

                        if (v31 != 0)
                        {
                            data[4] |= 0x10;
                        }
                        else
                        {
                            data[4] &= 0xEF;
                        }

                        data[0] ^= 0xD0;
                        var v11 = data[6];
                        data[6] = data[3];
                        data[3] = v11;
                    }
                }
                else
                {
                    var v3 = data[1];
                    data[1] = data[0];
                    data[0] = v3;
                    data[1] ^= 0xCE;
                    var v4 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v4;
                    var v5 = (byte)(data[3] >> 5);
                    data[3] *= 8;
                    data[3] |= v5;
                    data[0] ^= 0x68;
                    var v6 = (byte)(data[2] >> 1);
                    data[2] <<= 7;
                    data[2] |= v6;
                    var v7 = (byte)(data[3] >> 6);
                    data[3] *= 4;
                    data[3] |= v7;
                    var v2 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 2) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    var v8 = data[0];
                    data[0] = data[1];
                    data[1] = v8;
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
                            var v16 = (byte)(data[16] >> 6);
                            data[16] *= 4;
                            data[16] |= v16;
                            data[28] ^= 0xBA;
                            data[26] ^= 0x92;
                            var v24 = (byte)((data[26] >> 3) & 1);
                            if (((data[26] >> 1) & 1) != 0)
                            {
                                data[26] |= 8;
                            }
                            else
                            {
                                data[26] &= 0xF7;
                            }

                            if (v24 != 0)
                            {
                                data[26] |= 2;
                            }
                            else
                            {
                                data[26] &= 0xFD;
                            }

                            var v17 = data[13];
                            data[13] = data[25];
                            data[25] = v17;
                            var v23 = (byte)((data[10] >> 4) & 1);
                            if (((data[10] >> 5) & 1) != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            if (v23 != 0)
                            {
                                data[10] |= 0x20;
                            }
                            else
                            {
                                data[10] &= 0xDF;
                            }

                            var v18 = (byte)(data[20] >> 4);
                            data[20] *= 16;
                            data[20] |= v18;
                            var v22 = (byte)((data[22] >> 6) & 1);
                            if (((data[22] >> 3) & 1) != 0)
                            {
                                data[22] |= 0x40;
                            }
                            else
                            {
                                data[22] &= 0xBF;
                            }

                            if (v22 != 0)
                            {
                                data[22] |= 8;
                            }
                            else
                            {
                                data[22] &= 0xF7;
                            }

                            data[9] ^= 0x12;
                            var v19 = (byte)(data[22] >> 3);
                            data[22] *= 32;
                            data[22] |= v19;
                            var v21 = (byte)((data[27] >> 7) & 1);
                            if (((data[27] >> 2) & 1) != 0)
                            {
                                data[27] |= 0x80;
                            }
                            else
                            {
                                data[27] &= 0x7F;
                            }

                            if (v21 != 0)
                            {
                                data[27] |= 4;
                            }
                            else
                            {
                                data[27] &= 0xFB;
                            }

                            var v20 = data[20];
                            data[20] = data[2];
                            data[2] = v20;
                            data[21] ^= 0xAF;
                        }
                        else
                        {
                            var v12 = data[2];
                            data[2] = data[4];
                            data[4] = v12;
                            var v30 = (byte)((data[3] >> 6) & 1);
                            if (((data[3] >> 6) & 1) != 0)
                            {
                                data[3] |= 0x40;
                            }
                            else
                            {
                                data[3] &= 0xBF;
                            }

                            if (v30 != 0)
                            {
                                data[3] |= 0x40;
                            }
                            else
                            {
                                data[3] &= 0xBF;
                            }

                            var v29 = (byte)((data[6] >> 4) & 1);
                            if (((data[6] >> 7) & 1) != 0)
                            {
                                data[6] |= 0x10;
                            }
                            else
                            {
                                data[6] &= 0xEF;
                            }

                            if (v29 != 0)
                            {
                                data[6] |= 0x80;
                            }
                            else
                            {
                                data[6] &= 0x7F;
                            }

                            var v28 = (byte)((data[7] >> 3) & 1);
                            if (((data[7] >> 3) & 1) != 0)
                            {
                                data[7] |= 8;
                            }
                            else
                            {
                                data[7] &= 0xF7;
                            }

                            if (v28 != 0)
                            {
                                data[7] |= 8;
                            }
                            else
                            {
                                data[7] &= 0xF7;
                            }

                            var v13 = (byte)(data[2] >> 3);
                            data[2] *= 32;
                            data[2] |= v13;
                            var v14 = (byte)(data[3] >> 5);
                            data[3] *= 8;
                            data[3] |= v14;
                            var v27 = (byte)((data[1] >> 2) & 1);
                            if (((data[1] >> 1) & 1) != 0)
                            {
                                data[1] |= 4;
                            }
                            else
                            {
                                data[1] &= 0xFB;
                            }

                            if (v27 != 0)
                            {
                                data[1] |= 2;
                            }
                            else
                            {
                                data[1] &= 0xFD;
                            }

                            data[13] ^= 0x84;
                            var v26 = (byte)((data[15] >> 6) & 1);
                            if (((data[15] >> 3) & 1) != 0)
                            {
                                data[15] |= 0x40;
                            }
                            else
                            {
                                data[15] &= 0xBF;
                            }

                            if (v26 != 0)
                            {
                                data[15] |= 8;
                            }
                            else
                            {
                                data[15] &= 0xF7;
                            }

                            data[14] ^= 1;
                            data[3] ^= 0x57;
                            var v15 = (byte)(data[9] >> 7);
                            data[9] *= 2;
                            data[9] |= v15;
                            var v25 = (byte)((data[10] >> 1) & 1);
                            if (((data[10] >> 3) & 1) != 0)
                            {
                                data[10] |= 2;
                            }
                            else
                            {
                                data[10] &= 0xFD;
                            }

                            if (v25 != 0)
                            {
                                data[10] |= 8;
                            }
                            else
                            {
                                data[10] &= 0xF7;
                            }
                        }
                    }
                    else
                    {
                        var v9 = data[6];
                        data[6] = data[3];
                        data[3] = v9;
                        data[0] ^= 0xD0;
                        var v32 = (byte)((data[4] >> 7) & 1);
                        if (((data[4] >> 4) & 1) != 0)
                        {
                            data[4] |= 0x80;
                        }
                        else
                        {
                            data[4] &= 0x7F;
                        }

                        if (v32 != 0)
                        {
                            data[4] |= 0x10;
                        }
                        else
                        {
                            data[4] &= 0xEF;
                        }

                        var v10 = (byte)(data[6] >> 4);
                        data[6] *= 16;
                        data[6] |= v10;
                        var v31 = (byte)((data[4] >> 2) & 1);
                        if (((data[4] >> 6) & 1) != 0)
                        {
                            data[4] |= 4;
                        }
                        else
                        {
                            data[4] &= 0xFB;
                        }

                        if (v31 != 0)
                        {
                            data[4] |= 0x40;
                        }
                        else
                        {
                            data[4] &= 0xBF;
                        }

                        var v11 = data[7];
                        data[7] = data[5];
                        data[5] = v11;
                    }
                }
                else
                {
                    var v3 = data[0];
                    data[0] = data[1];
                    data[1] = v3;
                    var v2 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 2) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    var v4 = (byte)(data[3] >> 2);
                    data[3] <<= 6;
                    data[3] |= v4;
                    var v5 = (byte)(data[2] >> 7);
                    data[2] *= 2;
                    data[2] |= v5;
                    data[0] ^= 0x68;
                    var v6 = (byte)(data[3] >> 3);
                    data[3] *= 32;
                    data[3] |= v6;
                    var v7 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v7;
                    data[1] ^= 0xCE;
                    var v8 = data[1];
                    data[1] = data[0];
                    data[0] = v8;
                }
            }
        }
    }
}
// <copyright file="PacketTwisterOfVaultClose.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'VaultClose' type.
    /// </summary>
    internal class PacketTwisterOfVaultClose : IPacketTwister
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
                            var v12 = data[3];
                            data[3] = data[15];
                            data[15] = v12;
                            var v21 = (byte)((data[20] >> 2) & 1);
                            if (((data[20] >> 1) & 1) != 0)
                            {
                                data[20] |= 4;
                            }
                            else
                            {
                                data[20] &= 0xFB;
                            }

                            if (v21 != 0)
                            {
                                data[20] |= 2;
                            }
                            else
                            {
                                data[20] &= 0xFD;
                            }

                            var v13 = (byte)(data[17] >> 6);
                            data[17] *= 4;
                            data[17] |= v13;
                            data[5] ^= 0x1E;
                            var v14 = (byte)(data[30] >> 4);
                            data[30] *= 16;
                            data[30] |= v14;
                            var v15 = data[25];
                            data[25] = data[31];
                            data[31] = v15;
                            data[21] ^= 0xAA;
                            var v20 = (byte)((data[16] >> 1) & 1);
                            if (((data[16] >> 3) & 1) != 0)
                            {
                                data[16] |= 2;
                            }
                            else
                            {
                                data[16] &= 0xFD;
                            }

                            if (v20 != 0)
                            {
                                data[16] |= 8;
                            }
                            else
                            {
                                data[16] &= 0xF7;
                            }

                            data[30] ^= 0x5B;
                            var v16 = data[5];
                            data[5] = data[7];
                            data[7] = v16;
                            var v19 = (byte)((data[0] >> 4) & 1);
                            if (((data[0] >> 1) & 1) != 0)
                            {
                                data[0] |= 0x10;
                            }
                            else
                            {
                                data[0] &= 0xEF;
                            }

                            if (v19 != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            var v18 = (byte)((data[9] >> 4) & 1);
                            if (((data[9] >> 5) & 1) != 0)
                            {
                                data[9] |= 0x10;
                            }
                            else
                            {
                                data[9] &= 0xEF;
                            }

                            if (v18 != 0)
                            {
                                data[9] |= 0x20;
                            }
                            else
                            {
                                data[9] &= 0xDF;
                            }

                            var v17 = (byte)(data[7] >> 4);
                            data[7] *= 16;
                            data[7] |= v17;
                        }
                        else
                        {
                            var v24 = (byte)((data[11] >> 4) & 1);
                            if (((data[11] >> 7) & 1) != 0)
                            {
                                data[11] |= 0x10;
                            }
                            else
                            {
                                data[11] &= 0xEF;
                            }

                            if (v24 != 0)
                            {
                                data[11] |= 0x80;
                            }
                            else
                            {
                                data[11] &= 0x7F;
                            }

                            var v9 = data[10];
                            data[10] = data[0];
                            data[0] = v9;
                            data[15] ^= 0xD2;
                            var v10 = data[15];
                            data[15] = data[9];
                            data[9] = v10;
                            data[6] ^= 0xD8;
                            var v23 = (byte)((data[4] >> 3) & 1);
                            if (((data[4] >> 3) & 1) != 0)
                            {
                                data[4] |= 8;
                            }
                            else
                            {
                                data[4] &= 0xF7;
                            }

                            if (v23 != 0)
                            {
                                data[4] |= 8;
                            }
                            else
                            {
                                data[4] &= 0xF7;
                            }

                            var v11 = data[1];
                            data[1] = data[3];
                            data[3] = v11;
                            data[13] ^= 0x6D;
                            data[11] ^= 0xD4;
                            var v22 = (byte)((data[2] >> 6) & 1);
                            if (((data[2] >> 5) & 1) != 0)
                            {
                                data[2] |= 0x40;
                            }
                            else
                            {
                                data[2] &= 0xBF;
                            }

                            if (v22 != 0)
                            {
                                data[2] |= 0x20;
                            }
                            else
                            {
                                data[2] &= 0xDF;
                            }

                            data[11] ^= 0xD2;
                        }
                    }
                    else
                    {
                        data[6] ^= 0xEA;
                        data[5] ^= 0x42;
                        var v5 = (byte)(data[1] >> 5);
                        data[1] *= 8;
                        data[1] |= v5;
                        var v6 = (byte)(data[1] >> 7);
                        data[1] *= 2;
                        data[1] |= v6;
                        var v29 = (byte)((data[4] >> 1) & 1);
                        if (((data[4] >> 5) & 1) != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        if (v29 != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        data[1] ^= 0xDA;
                        var v28 = (byte)((data[4] >> 5) & 1);
                        if (((data[4] >> 4) & 1) != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        if (v28 != 0)
                        {
                            data[4] |= 0x10;
                        }
                        else
                        {
                            data[4] &= 0xEF;
                        }

                        var v7 = (byte)(data[7] >> 7);
                        data[7] *= 2;
                        data[7] |= v7;
                        var v8 = data[4];
                        data[4] = data[0];
                        data[0] = v8;
                        var v27 = (byte)((data[6] >> 2) & 1);
                        if (((data[6] >> 1) & 1) != 0)
                        {
                            data[6] |= 4;
                        }
                        else
                        {
                            data[6] &= 0xFB;
                        }

                        if (v27 != 0)
                        {
                            data[6] |= 2;
                        }
                        else
                        {
                            data[6] &= 0xFD;
                        }

                        var v26 = (byte)((data[0] >> 2) & 1);
                        if (((data[0] >> 1) & 1) != 0)
                        {
                            data[0] |= 4;
                        }
                        else
                        {
                            data[0] &= 0xFB;
                        }

                        if (v26 != 0)
                        {
                            data[0] |= 2;
                        }
                        else
                        {
                            data[0] &= 0xFD;
                        }

                        var v25 = (byte)((data[4] >> 2) & 1);
                        if (((data[4] >> 7) & 1) != 0)
                        {
                            data[4] |= 4;
                        }
                        else
                        {
                            data[4] &= 0xFB;
                        }

                        if (v25 != 0)
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
                    data[2] ^= 0x79;
                    var v3 = data[2];
                    data[2] = data[1];
                    data[1] = v3;
                    var v4 = data[2];
                    data[2] = data[1];
                    data[1] = v4;
                    var v2 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
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
                            var v12 = (byte)(data[7] >> 4);
                            data[7] *= 16;
                            data[7] |= v12;
                            var v21 = (byte)((data[9] >> 4) & 1);
                            if (((data[9] >> 5) & 1) != 0)
                            {
                                data[9] |= 0x10;
                            }
                            else
                            {
                                data[9] &= 0xEF;
                            }

                            if (v21 != 0)
                            {
                                data[9] |= 0x20;
                            }
                            else
                            {
                                data[9] &= 0xDF;
                            }

                            var v20 = (byte)((data[0] >> 4) & 1);
                            if (((data[0] >> 1) & 1) != 0)
                            {
                                data[0] |= 0x10;
                            }
                            else
                            {
                                data[0] &= 0xEF;
                            }

                            if (v20 != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            var v13 = data[5];
                            data[5] = data[7];
                            data[7] = v13;
                            data[30] ^= 0x5B;
                            var v19 = (byte)((data[16] >> 1) & 1);
                            if (((data[16] >> 3) & 1) != 0)
                            {
                                data[16] |= 2;
                            }
                            else
                            {
                                data[16] &= 0xFD;
                            }

                            if (v19 != 0)
                            {
                                data[16] |= 8;
                            }
                            else
                            {
                                data[16] &= 0xF7;
                            }

                            data[21] ^= 0xAA;
                            var v14 = data[25];
                            data[25] = data[31];
                            data[31] = v14;
                            var v15 = (byte)(data[30] >> 4);
                            data[30] *= 16;
                            data[30] |= v15;
                            data[5] ^= 0x1E;
                            var v16 = (byte)(data[17] >> 2);
                            data[17] <<= 6;
                            data[17] |= v16;
                            var v18 = (byte)((data[20] >> 2) & 1);
                            if (((data[20] >> 1) & 1) != 0)
                            {
                                data[20] |= 4;
                            }
                            else
                            {
                                data[20] &= 0xFB;
                            }

                            if (v18 != 0)
                            {
                                data[20] |= 2;
                            }
                            else
                            {
                                data[20] &= 0xFD;
                            }

                            var v17 = data[3];
                            data[3] = data[15];
                            data[15] = v17;
                        }
                        else
                        {
                            data[11] ^= 0xD2;
                            var v24 = (byte)((data[2] >> 6) & 1);
                            if (((data[2] >> 5) & 1) != 0)
                            {
                                data[2] |= 0x40;
                            }
                            else
                            {
                                data[2] &= 0xBF;
                            }

                            if (v24 != 0)
                            {
                                data[2] |= 0x20;
                            }
                            else
                            {
                                data[2] &= 0xDF;
                            }

                            data[11] ^= 0xD4;
                            data[13] ^= 0x6D;
                            var v9 = data[1];
                            data[1] = data[3];
                            data[3] = v9;
                            var v23 = (byte)((data[4] >> 3) & 1);
                            if (((data[4] >> 3) & 1) != 0)
                            {
                                data[4] |= 8;
                            }
                            else
                            {
                                data[4] &= 0xF7;
                            }

                            if (v23 != 0)
                            {
                                data[4] |= 8;
                            }
                            else
                            {
                                data[4] &= 0xF7;
                            }

                            data[6] ^= 0xD8;
                            var v10 = data[15];
                            data[15] = data[9];
                            data[9] = v10;
                            data[15] ^= 0xD2;
                            var v11 = data[10];
                            data[10] = data[0];
                            data[0] = v11;
                            var v22 = (byte)((data[11] >> 4) & 1);
                            if (((data[11] >> 7) & 1) != 0)
                            {
                                data[11] |= 0x10;
                            }
                            else
                            {
                                data[11] &= 0xEF;
                            }

                            if (v22 != 0)
                            {
                                data[11] |= 0x80;
                            }
                            else
                            {
                                data[11] &= 0x7F;
                            }
                        }
                    }
                    else
                    {
                        var v29 = (byte)((data[4] >> 2) & 1);
                        if (((data[4] >> 7) & 1) != 0)
                        {
                            data[4] |= 4;
                        }
                        else
                        {
                            data[4] &= 0xFB;
                        }

                        if (v29 != 0)
                        {
                            data[4] |= 0x80;
                        }
                        else
                        {
                            data[4] &= 0x7F;
                        }

                        var v28 = (byte)((data[0] >> 2) & 1);
                        if (((data[0] >> 1) & 1) != 0)
                        {
                            data[0] |= 4;
                        }
                        else
                        {
                            data[0] &= 0xFB;
                        }

                        if (v28 != 0)
                        {
                            data[0] |= 2;
                        }
                        else
                        {
                            data[0] &= 0xFD;
                        }

                        var v27 = (byte)((data[6] >> 2) & 1);
                        if (((data[6] >> 1) & 1) != 0)
                        {
                            data[6] |= 4;
                        }
                        else
                        {
                            data[6] &= 0xFB;
                        }

                        if (v27 != 0)
                        {
                            data[6] |= 2;
                        }
                        else
                        {
                            data[6] &= 0xFD;
                        }

                        var v5 = data[4];
                        data[4] = data[0];
                        data[0] = v5;
                        var v6 = (byte)(data[7] >> 1);
                        data[7] <<= 7;
                        data[7] |= v6;
                        var v26 = (byte)((data[4] >> 5) & 1);
                        if (((data[4] >> 4) & 1) != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        if (v26 != 0)
                        {
                            data[4] |= 0x10;
                        }
                        else
                        {
                            data[4] &= 0xEF;
                        }

                        data[1] ^= 0xDA;
                        var v25 = (byte)((data[4] >> 1) & 1);
                        if (((data[4] >> 5) & 1) != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        if (v25 != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        var v7 = (byte)(data[1] >> 1);
                        data[1] <<= 7;
                        data[1] |= v7;
                        var v8 = (byte)(data[1] >> 3);
                        data[1] *= 32;
                        data[1] |= v8;
                        data[5] ^= 0x42;
                        data[6] ^= 0xEA;
                    }
                }
                else
                {
                    var v2 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    var v3 = data[2];
                    data[2] = data[1];
                    data[1] = v3;
                    var v4 = data[2];
                    data[2] = data[1];
                    data[1] = v4;
                    data[2] ^= 0x79;
                }
            }
        }
    }
}
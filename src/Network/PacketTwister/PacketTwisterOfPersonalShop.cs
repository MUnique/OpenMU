// <copyright file="PacketTwisterOfPersonalShop.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'PersonalShop' type.
    /// </summary>
    internal class PacketTwisterOfPersonalShop : IPacketTwister
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
                            var v14 = (byte)(data[31] >> 6);
                            data[31] *= 4;
                            data[31] |= v14;
                            var v25 = (byte)((data[0] >> 2) & 1);
                            if (((data[0] >> 3) & 1) != 0)
                            {
                                data[0] |= 4;
                            }
                            else
                            {
                                data[0] &= 0xFB;
                            }

                            if (v25 != 0)
                            {
                                data[0] |= 8;
                            }
                            else
                            {
                                data[0] &= 0xF7;
                            }

                            var v15 = (byte)(data[28] >> 1);
                            data[28] <<= 7;
                            data[28] |= v15;
                            var v16 = data[28];
                            data[28] = data[13];
                            data[13] = v16;
                            var v24 = (byte)((data[19] >> 5) & 1);
                            if (((data[19] >> 4) & 1) != 0)
                            {
                                data[19] |= 0x20;
                            }
                            else
                            {
                                data[19] &= 0xDF;
                            }

                            if (v24 != 0)
                            {
                                data[19] |= 0x10;
                            }
                            else
                            {
                                data[19] &= 0xEF;
                            }

                            var v17 = data[8];
                            data[8] = data[31];
                            data[31] = v17;
                            var v18 = (byte)(data[9] >> 6);
                            data[9] *= 4;
                            data[9] |= v18;
                            var v19 = data[10];
                            data[10] = data[19];
                            data[19] = v19;
                            var v23 = (byte)((data[13] >> 4) & 1);
                            if (((data[13] >> 3) & 1) != 0)
                            {
                                data[13] |= 0x10;
                            }
                            else
                            {
                                data[13] &= 0xEF;
                            }

                            if (v23 != 0)
                            {
                                data[13] |= 8;
                            }
                            else
                            {
                                data[13] &= 0xF7;
                            }

                            var v20 = (byte)(data[3] >> 3);
                            data[3] *= 32;
                            data[3] |= v20;
                            var v22 = (byte)((data[9] >> 3) & 1);
                            if (((data[9] >> 4) & 1) != 0)
                            {
                                data[9] |= 8;
                            }
                            else
                            {
                                data[9] &= 0xF7;
                            }

                            if (v22 != 0)
                            {
                                data[9] |= 0x10;
                            }
                            else
                            {
                                data[9] &= 0xEF;
                            }

                            var v21 = data[22];
                            data[22] = data[9];
                            data[9] = v21;
                        }
                        else
                        {
                            var v10 = data[15];
                            data[15] = data[9];
                            data[9] = v10;
                            var v11 = (byte)(data[5] >> 7);
                            data[5] *= 2;
                            data[5] |= v11;
                            data[8] ^= 0xAE;
                            var v12 = data[12];
                            data[12] = data[6];
                            data[6] = v12;
                            var v28 = (byte)((data[8] >> 5) & 1);
                            if (((data[8] >> 4) & 1) != 0)
                            {
                                data[8] |= 0x20;
                            }
                            else
                            {
                                data[8] &= 0xDF;
                            }

                            if (v28 != 0)
                            {
                                data[8] |= 0x10;
                            }
                            else
                            {
                                data[8] &= 0xEF;
                            }

                            var v27 = (byte)((data[3] >> 2) & 1);
                            if (((data[3] >> 4) & 1) != 0)
                            {
                                data[3] |= 4;
                            }
                            else
                            {
                                data[3] &= 0xFB;
                            }

                            if (v27 != 0)
                            {
                                data[3] |= 0x10;
                            }
                            else
                            {
                                data[3] &= 0xEF;
                            }

                            data[15] ^= 0x57;
                            var v13 = data[11];
                            data[11] = data[0];
                            data[0] = v13;
                            var v26 = (byte)((data[3] >> 2) & 1);
                            if (((data[3] >> 5) & 1) != 0)
                            {
                                data[3] |= 4;
                            }
                            else
                            {
                                data[3] &= 0xFB;
                            }

                            if (v26 != 0)
                            {
                                data[3] |= 0x20;
                            }
                            else
                            {
                                data[3] &= 0xDF;
                            }

                            data[13] ^= 0xB2;
                        }
                    }
                    else
                    {
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

                        var v6 = data[4];
                        data[4] = data[6];
                        data[6] = v6;
                        var v7 = (byte)(data[1] >> 6);
                        data[1] *= 4;
                        data[1] |= v7;
                        var v8 = data[7];
                        data[7] = data[6];
                        data[6] = v8;
                        var v9 = data[4];
                        data[4] = data[0];
                        data[0] = v9;
                    }
                }
                else
                {
                    var v3 = data[2];
                    data[2] = data[2];
                    data[2] = v3;
                    var v4 = data[3];
                    data[3] = data[3];
                    data[3] = v4;
                    var v5 = (byte)(data[1] >> 2);
                    data[1] <<= 6;
                    data[1] |= v5;
                    data[1] ^= 0x5A;
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
                            var v14 = data[22];
                            data[22] = data[9];
                            data[9] = v14;
                            var v25 = (byte)((data[9] >> 3) & 1);
                            if (((data[9] >> 4) & 1) != 0)
                            {
                                data[9] |= 8;
                            }
                            else
                            {
                                data[9] &= 0xF7;
                            }

                            if (v25 != 0)
                            {
                                data[9] |= 0x10;
                            }
                            else
                            {
                                data[9] &= 0xEF;
                            }

                            var v15 = (byte)(data[3] >> 5);
                            data[3] *= 8;
                            data[3] |= v15;
                            var v24 = (byte)((data[13] >> 4) & 1);
                            if (((data[13] >> 3) & 1) != 0)
                            {
                                data[13] |= 0x10;
                            }
                            else
                            {
                                data[13] &= 0xEF;
                            }

                            if (v24 != 0)
                            {
                                data[13] |= 8;
                            }
                            else
                            {
                                data[13] &= 0xF7;
                            }

                            var v16 = data[10];
                            data[10] = data[19];
                            data[19] = v16;
                            var v17 = (byte)(data[9] >> 2);
                            data[9] <<= 6;
                            data[9] |= v17;
                            var v18 = data[8];
                            data[8] = data[31];
                            data[31] = v18;
                            var v23 = (byte)((data[19] >> 5) & 1);
                            if (((data[19] >> 4) & 1) != 0)
                            {
                                data[19] |= 0x20;
                            }
                            else
                            {
                                data[19] &= 0xDF;
                            }

                            if (v23 != 0)
                            {
                                data[19] |= 0x10;
                            }
                            else
                            {
                                data[19] &= 0xEF;
                            }

                            var v19 = data[28];
                            data[28] = data[13];
                            data[13] = v19;
                            var v20 = (byte)(data[28] >> 7);
                            data[28] *= 2;
                            data[28] |= v20;
                            var v22 = (byte)((data[0] >> 2) & 1);
                            if (((data[0] >> 3) & 1) != 0)
                            {
                                data[0] |= 4;
                            }
                            else
                            {
                                data[0] &= 0xFB;
                            }

                            if (v22 != 0)
                            {
                                data[0] |= 8;
                            }
                            else
                            {
                                data[0] &= 0xF7;
                            }

                            var v21 = (byte)(data[31] >> 2);
                            data[31] <<= 6;
                            data[31] |= v21;
                        }
                        else
                        {
                            data[13] ^= 0xB2;
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

                            var v10 = data[11];
                            data[11] = data[0];
                            data[0] = v10;
                            data[15] ^= 0x57;
                            var v27 = (byte)((data[3] >> 2) & 1);
                            if (((data[3] >> 4) & 1) != 0)
                            {
                                data[3] |= 4;
                            }
                            else
                            {
                                data[3] &= 0xFB;
                            }

                            if (v27 != 0)
                            {
                                data[3] |= 0x10;
                            }
                            else
                            {
                                data[3] &= 0xEF;
                            }

                            var v26 = (byte)((data[8] >> 5) & 1);
                            if (((data[8] >> 4) & 1) != 0)
                            {
                                data[8] |= 0x20;
                            }
                            else
                            {
                                data[8] &= 0xDF;
                            }

                            if (v26 != 0)
                            {
                                data[8] |= 0x10;
                            }
                            else
                            {
                                data[8] &= 0xEF;
                            }

                            var v11 = data[12];
                            data[12] = data[6];
                            data[6] = v11;
                            data[8] ^= 0xAE;
                            var v12 = (byte)(data[5] >> 1);
                            data[5] <<= 7;
                            data[5] |= v12;
                            var v13 = data[15];
                            data[15] = data[9];
                            data[9] = v13;
                        }
                    }
                    else
                    {
                        var v6 = data[4];
                        data[4] = data[0];
                        data[0] = v6;
                        var v7 = data[7];
                        data[7] = data[6];
                        data[6] = v7;
                        var v8 = (byte)(data[1] >> 2);
                        data[1] <<= 6;
                        data[1] |= v8;
                        var v9 = data[4];
                        data[4] = data[6];
                        data[6] = v9;
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
                    }
                }
                else
                {
                    data[1] ^= 0x5A;
                    var v3 = (byte)(data[1] >> 6);
                    data[1] *= 4;
                    data[1] |= v3;
                    var v4 = data[3];
                    data[3] = data[3];
                    data[3] = v4;
                    var v5 = data[2];
                    data[2] = data[2];
                    data[2] = v5;
                }
            }
        }
    }
}
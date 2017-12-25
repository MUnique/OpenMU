// <copyright file="PacketTwisterOfItemConsume.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'ItemConsume' type.
    /// </summary>
    internal class PacketTwisterOfItemConsume : IPacketTwister
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
                            var v20 = (byte)(data[19] >> 2);
                            data[19] <<= 6;
                            data[19] |= v20;
                            data[22] ^= 0xAA;
                            var v21 = (byte)(data[11] >> 3);
                            data[11] *= 32;
                            data[11] |= v21;
                            var v22 = data[2];
                            data[2] = data[9];
                            data[9] = v22;
                            var v23 = (byte)(data[10] >> 6);
                            data[10] *= 4;
                            data[10] |= v23;
                            var v24 = (byte)(data[19] >> 5);
                            data[19] *= 8;
                            data[19] |= v24;
                            data[5] ^= 0x5D;
                            data[1] ^= 0x6F;
                            var v25 = data[19];
                            data[19] = data[30];
                            data[30] = v25;
                            var v28 = (byte)((data[18] >> 2) & 1);
                            if (((data[18] >> 2) & 1) != 0)
                            {
                                data[18] |= 4;
                            }
                            else
                            {
                                data[18] &= 0xFB;
                            }

                            if (v28 != 0)
                            {
                                data[18] |= 4;
                            }
                            else
                            {
                                data[18] &= 0xFB;
                            }

                            var v26 = data[24];
                            data[24] = data[22];
                            data[22] = v26;
                            var v27 = (byte)(data[6] >> 6);
                            data[6] *= 4;
                            data[6] |= v27;
                        }
                        else
                        {
                            var v13 = data[2];
                            data[2] = data[8];
                            data[8] = v13;
                            var v14 = (byte)(data[10] >> 4);
                            data[10] *= 16;
                            data[10] |= v14;
                            var v30 = (byte)((data[15] >> 2) & 1);
                            if (((data[15] >> 5) & 1) != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            if (v30 != 0)
                            {
                                data[15] |= 0x20;
                            }
                            else
                            {
                                data[15] &= 0xDF;
                            }

                            data[12] ^= 0xF7;
                            var v29 = (byte)((data[14] >> 7) & 1);
                            if (((data[14] >> 2) & 1) != 0)
                            {
                                data[14] |= 0x80;
                            }
                            else
                            {
                                data[14] &= 0x7F;
                            }

                            if (v29 != 0)
                            {
                                data[14] |= 4;
                            }
                            else
                            {
                                data[14] &= 0xFB;
                            }

                            var v15 = (byte)(data[13] >> 6);
                            data[13] *= 4;
                            data[13] |= v15;
                            var v16 = data[4];
                            data[4] = data[14];
                            data[14] = v16;
                            data[10] ^= 0xD7;
                            var v17 = data[7];
                            data[7] = data[13];
                            data[13] = v17;
                            var v18 = (byte)(data[1] >> 1);
                            data[1] <<= 7;
                            data[1] |= v18;
                            var v19 = data[8];
                            data[8] = data[9];
                            data[9] = v19;
                        }
                    }
                    else
                    {
                        var v9 = data[1];
                        data[1] = data[4];
                        data[4] = v9;
                        var v10 = data[3];
                        data[3] = data[0];
                        data[0] = v10;
                        var v11 = data[2];
                        data[2] = data[1];
                        data[1] = v11;
                        var v12 = (byte)(data[3] >> 4);
                        data[3] *= 16;
                        data[3] |= v12;
                    }
                }
                else
                {
                    var v3 = (byte)(data[2] >> 7);
                    data[2] *= 2;
                    data[2] |= v3;
                    var v4 = data[1];
                    data[1] = data[0];
                    data[0] = v4;
                    var v5 = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= v5;
                    var v2 = (byte)((data[3] >> 3) & 1);
                    if (((data[3] >> 4) & 1) != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    var v32 = (byte)((data[1] >> 7) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    if (v32 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

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

                    var v6 = data[2];
                    data[2] = data[0];
                    data[0] = v6;
                    data[1] ^= 0xE;
                    var v7 = (byte)(data[0] >> 1);
                    data[0] <<= 7;
                    data[0] |= v7;
                    var v8 = data[1];
                    data[1] = data[2];
                    data[2] = v8;
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
                            var v20 = (byte)(data[6] >> 2);
                            data[6] <<= 6;
                            data[6] |= v20;
                            var v21 = data[24];
                            data[24] = data[22];
                            data[22] = v21;
                            var v28 = (byte)((data[18] >> 2) & 1);
                            if (((data[18] >> 2) & 1) != 0)
                            {
                                data[18] |= 4;
                            }
                            else
                            {
                                data[18] &= 0xFB;
                            }

                            if (v28 != 0)
                            {
                                data[18] |= 4;
                            }
                            else
                            {
                                data[18] &= 0xFB;
                            }

                            var v22 = data[19];
                            data[19] = data[30];
                            data[30] = v22;
                            data[1] ^= 0x6F;
                            data[5] ^= 0x5D;
                            var v23 = (byte)(data[19] >> 3);
                            data[19] *= 32;
                            data[19] |= v23;
                            var v24 = (byte)(data[10] >> 2);
                            data[10] <<= 6;
                            data[10] |= v24;
                            var v25 = data[2];
                            data[2] = data[9];
                            data[9] = v25;
                            var v26 = (byte)(data[11] >> 5);
                            data[11] *= 8;
                            data[11] |= v26;
                            data[22] ^= 0xAA;
                            var v27 = (byte)(data[19] >> 6);
                            data[19] *= 4;
                            data[19] |= v27;
                        }
                        else
                        {
                            var v13 = data[8];
                            data[8] = data[9];
                            data[9] = v13;
                            var v14 = (byte)(data[1] >> 7);
                            data[1] *= 2;
                            data[1] |= v14;
                            var v15 = data[7];
                            data[7] = data[13];
                            data[13] = v15;
                            data[10] ^= 0xD7;
                            var v16 = data[4];
                            data[4] = data[14];
                            data[14] = v16;
                            var v17 = (byte)(data[13] >> 2);
                            data[13] <<= 6;
                            data[13] |= v17;
                            var v30 = (byte)((data[14] >> 7) & 1);
                            if (((data[14] >> 2) & 1) != 0)
                            {
                                data[14] |= 0x80;
                            }
                            else
                            {
                                data[14] &= 0x7F;
                            }

                            if (v30 != 0)
                            {
                                data[14] |= 4;
                            }
                            else
                            {
                                data[14] &= 0xFB;
                            }

                            data[12] ^= 0xF7;
                            var v29 = (byte)((data[15] >> 2) & 1);
                            if (((data[15] >> 5) & 1) != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            if (v29 != 0)
                            {
                                data[15] |= 0x20;
                            }
                            else
                            {
                                data[15] &= 0xDF;
                            }

                            var v18 = (byte)(data[10] >> 4);
                            data[10] *= 16;
                            data[10] |= v18;
                            var v19 = data[2];
                            data[2] = data[8];
                            data[8] = v19;
                        }
                    }
                    else
                    {
                        var v9 = (byte)(data[3] >> 4);
                        data[3] *= 16;
                        data[3] |= v9;
                        var v10 = data[2];
                        data[2] = data[1];
                        data[1] = v10;
                        var v11 = data[3];
                        data[3] = data[0];
                        data[0] = v11;
                        var v12 = data[1];
                        data[1] = data[4];
                        data[4] = v12;
                    }
                }
                else
                {
                    var v3 = data[1];
                    data[1] = data[2];
                    data[2] = v3;
                    var v4 = (byte)(data[0] >> 7);
                    data[0] *= 2;
                    data[0] |= v4;
                    data[1] ^= 0xE;
                    var v5 = data[2];
                    data[2] = data[0];
                    data[0] = v5;
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

                    var v32 = (byte)((data[1] >> 7) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    if (v32 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    var v31 = (byte)((data[3] >> 3) & 1);
                    if (((data[3] >> 4) & 1) != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    if (v31 != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    var v6 = (byte)(data[2] >> 2);
                    data[2] <<= 6;
                    data[2] |= v6;
                    var v7 = data[1];
                    data[1] = data[0];
                    data[0] = v7;
                    var v8 = (byte)(data[2] >> 1);
                    data[2] <<= 7;
                    data[2] |= v8;
                }
            }
        }
    }
}
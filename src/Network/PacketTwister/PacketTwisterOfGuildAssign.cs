// <copyright file="PacketTwisterOfGuildAssign.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'GuildAssign' type.
    /// </summary>
    internal class PacketTwisterOfGuildAssign : IPacketTwister
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
                            var v19 = data[27];
                            data[27] = data[12];
                            data[12] = v19;
                            data[5] ^= 0x75;
                            data[9] ^= 0xEB;
                            var v20 = (byte)(data[26] >> 4);
                            data[26] *= 16;
                            data[26] |= v20;
                            var v21 = (byte)(data[17] >> 3);
                            data[17] *= 32;
                            data[17] |= v21;
                            var v22 = (byte)(data[27] >> 5);
                            data[27] *= 8;
                            data[27] |= v22;
                            data[3] ^= 0xE;
                            var v23 = (byte)(data[15] >> 3);
                            data[15] *= 32;
                            data[15] |= v23;
                            var v24 = (byte)(data[3] >> 3);
                            data[3] *= 32;
                            data[3] |= v24;
                            var v25 = data[30];
                            data[30] = data[7];
                            data[7] = v25;
                            var v26 = (byte)((data[5] >> 5) & 1);
                            if (((data[5] >> 2) & 1) != 0)
                            {
                                data[5] |= 0x20;
                            }
                            else
                            {
                                data[5] &= 0xDF;
                            }

                            if (v26 != 0)
                            {
                                data[5] |= 4;
                            }
                            else
                            {
                                data[5] &= 0xFB;
                            }
                        }
                        else
                        {
                            data[12] ^= 0x31;
                            var v14 = data[2];
                            data[2] = data[10];
                            data[10] = v14;
                            var v15 = (byte)(data[15] >> 7);
                            data[15] *= 2;
                            data[15] |= v15;
                            var v16 = data[15];
                            data[15] = data[5];
                            data[5] = v16;
                            var v28 = (byte)((data[12] >> 2) & 1);
                            if (((data[12] >> 3) & 1) != 0)
                            {
                                data[12] |= 4;
                            }
                            else
                            {
                                data[12] &= 0xFB;
                            }

                            if (v28 != 0)
                            {
                                data[12] |= 8;
                            }
                            else
                            {
                                data[12] &= 0xF7;
                            }

                            var v17 = (byte)(data[2] >> 5);
                            data[2] *= 8;
                            data[2] |= v17;
                            var v18 = data[3];
                            data[3] = data[0];
                            data[0] = v18;
                            var v27 = (byte)((data[14] >> 4) & 1);
                            if (((data[14] >> 7) & 1) != 0)
                            {
                                data[14] |= 0x10;
                            }
                            else
                            {
                                data[14] &= 0xEF;
                            }

                            if (v27 != 0)
                            {
                                data[14] |= 0x80;
                            }
                            else
                            {
                                data[14] &= 0x7F;
                            }
                        }
                    }
                    else
                    {
                        var v30 = (byte)((data[4] >> 6) & 1);
                        if (((data[4] >> 7) & 1) != 0)
                        {
                            data[4] |= 0x40;
                        }
                        else
                        {
                            data[4] &= 0xBF;
                        }

                        if (v30 != 0)
                        {
                            data[4] |= 0x80;
                        }
                        else
                        {
                            data[4] &= 0x7F;
                        }

                        var v10 = data[2];
                        data[2] = data[3];
                        data[3] = v10;
                        var v11 = data[0];
                        data[0] = data[1];
                        data[1] = v11;
                        var v12 = (byte)(data[7] >> 7);
                        data[7] *= 2;
                        data[7] |= v12;
                        data[7] ^= 0x42;
                        data[1] ^= 0xEF;
                        data[6] ^= 0x54;
                        var v29 = (byte)((data[5] >> 4) & 1);
                        if (((data[5] >> 4) & 1) != 0)
                        {
                            data[5] |= 0x10;
                        }
                        else
                        {
                            data[5] &= 0xEF;
                        }

                        if (v29 != 0)
                        {
                            data[5] |= 0x10;
                        }
                        else
                        {
                            data[5] &= 0xEF;
                        }

                        var v13 = data[1];
                        data[1] = data[3];
                        data[3] = v13;
                    }
                }
                else
                {
                    var v3 = (byte)(data[1] >> 7);
                    data[1] *= 2;
                    data[1] |= v3;
                    var v4 = data[2];
                    data[2] = data[2];
                    data[2] = v4;
                    data[3] ^= 8;
                    var v5 = data[1];
                    data[1] = data[0];
                    data[0] = v5;
                    var v6 = data[3];
                    data[3] = data[2];
                    data[2] = v6;
                    var v7 = data[2];
                    data[2] = data[3];
                    data[3] = v7;
                    var v2 = (byte)((data[2] >> 1) & 1);
                    if (((data[2] >> 2) & 1) != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    var v8 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v8;
                    var v9 = data[1];
                    data[1] = data[3];
                    data[3] = v9;
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
                            var v26 = (byte)((data[5] >> 5) & 1);
                            if (((data[5] >> 2) & 1) != 0)
                            {
                                data[5] |= 0x20;
                            }
                            else
                            {
                                data[5] &= 0xDF;
                            }

                            if (v26 != 0)
                            {
                                data[5] |= 4;
                            }
                            else
                            {
                                data[5] &= 0xFB;
                            }

                            var v19 = data[30];
                            data[30] = data[7];
                            data[7] = v19;
                            var v20 = (byte)(data[3] >> 5);
                            data[3] *= 8;
                            data[3] |= v20;
                            var v21 = (byte)(data[15] >> 5);
                            data[15] *= 8;
                            data[15] |= v21;
                            data[3] ^= 0xE;
                            var v22 = (byte)(data[27] >> 3);
                            data[27] *= 32;
                            data[27] |= v22;
                            var v23 = (byte)(data[17] >> 5);
                            data[17] *= 8;
                            data[17] |= v23;
                            var v24 = (byte)(data[26] >> 4);
                            data[26] *= 16;
                            data[26] |= v24;
                            data[9] ^= 0xEB;
                            data[5] ^= 0x75;
                            var v25 = data[27];
                            data[27] = data[12];
                            data[12] = v25;
                        }
                        else
                        {
                            var v28 = (byte)((data[14] >> 4) & 1);
                            if (((data[14] >> 7) & 1) != 0)
                            {
                                data[14] |= 0x10;
                            }
                            else
                            {
                                data[14] &= 0xEF;
                            }

                            if (v28 != 0)
                            {
                                data[14] |= 0x80;
                            }
                            else
                            {
                                data[14] &= 0x7F;
                            }

                            var v14 = data[3];
                            data[3] = data[0];
                            data[0] = v14;
                            var v15 = (byte)(data[2] >> 3);
                            data[2] *= 32;
                            data[2] |= v15;
                            var v27 = (byte)((data[12] >> 2) & 1);
                            if (((data[12] >> 3) & 1) != 0)
                            {
                                data[12] |= 4;
                            }
                            else
                            {
                                data[12] &= 0xFB;
                            }

                            if (v27 != 0)
                            {
                                data[12] |= 8;
                            }
                            else
                            {
                                data[12] &= 0xF7;
                            }

                            var v16 = data[15];
                            data[15] = data[5];
                            data[5] = v16;
                            var v17 = (byte)(data[15] >> 1);
                            data[15] <<= 7;
                            data[15] |= v17;
                            var v18 = data[2];
                            data[2] = data[10];
                            data[10] = v18;
                            data[12] ^= 0x31;
                        }
                    }
                    else
                    {
                        var v10 = data[1];
                        data[1] = data[3];
                        data[3] = v10;
                        var v30 = (byte)((data[5] >> 4) & 1);
                        if (((data[5] >> 4) & 1) != 0)
                        {
                            data[5] |= 0x10;
                        }
                        else
                        {
                            data[5] &= 0xEF;
                        }

                        if (v30 != 0)
                        {
                            data[5] |= 0x10;
                        }
                        else
                        {
                            data[5] &= 0xEF;
                        }

                        data[6] ^= 0x54;
                        data[1] ^= 0xEF;
                        data[7] ^= 0x42;
                        var v11 = (byte)(data[7] >> 1);
                        data[7] <<= 7;
                        data[7] |= v11;
                        var v12 = data[0];
                        data[0] = data[1];
                        data[1] = v12;
                        var v13 = data[2];
                        data[2] = data[3];
                        data[3] = v13;
                        var v29 = (byte)((data[4] >> 6) & 1);
                        if (((data[4] >> 7) & 1) != 0)
                        {
                            data[4] |= 0x40;
                        }
                        else
                        {
                            data[4] &= 0xBF;
                        }

                        if (v29 != 0)
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
                    var v3 = data[1];
                    data[1] = data[3];
                    data[3] = v3;
                    var v4 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v4;
                    var v2 = (byte)((data[2] >> 1) & 1);
                    if (((data[2] >> 2) & 1) != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    var v5 = data[2];
                    data[2] = data[3];
                    data[3] = v5;
                    var v6 = data[3];
                    data[3] = data[2];
                    data[2] = v6;
                    var v7 = data[1];
                    data[1] = data[0];
                    data[0] = v7;
                    data[3] ^= 8;
                    var v8 = data[2];
                    data[2] = data[2];
                    data[2] = v8;
                    var v9 = (byte)(data[1] >> 1);
                    data[1] <<= 7;
                    data[1] |= v9;
                }
            }
        }
    }
}
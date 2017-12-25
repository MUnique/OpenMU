// <copyright file="PacketTwisterOfChaosMachineClose.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'ChaosMachineClose' type.
    /// </summary>
    internal class PacketTwisterOfChaosMachineClose : IPacketTwister
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
                            var v13 = data[13];
                            data[13] = data[7];
                            data[7] = v13;
                            var v14 = data[13];
                            data[13] = data[23];
                            data[23] = v14;
                            var v15 = (byte)(data[15] >> 4);
                            data[15] *= 16;
                            data[15] |= v15;
                            var v16 = data[26];
                            data[26] = data[6];
                            data[6] = v16;
                            var v17 = data[27];
                            data[27] = data[8];
                            data[8] = v17;
                            data[19] ^= 0xF0;
                            data[28] ^= 0x57;
                            var v18 = (byte)(data[16] >> 3);
                            data[16] *= 32;
                            data[16] |= v18;
                            data[3] ^= 0x26;
                        }
                        else
                        {
                            var v22 = (byte)((data[12] >> 5) & 1);
                            if (((data[12] >> 1) & 1) != 0)
                            {
                                data[12] |= 0x20;
                            }
                            else
                            {
                                data[12] &= 0xDF;
                            }

                            if (v22 != 0)
                            {
                                data[12] |= 2;
                            }
                            else
                            {
                                data[12] &= 0xFD;
                            }

                            var v21 = (byte)((data[6] >> 1) & 1);
                            if (((data[6] >> 5) & 1) != 0)
                            {
                                data[6] |= 2;
                            }
                            else
                            {
                                data[6] &= 0xFD;
                            }

                            if (v21 != 0)
                            {
                                data[6] |= 0x20;
                            }
                            else
                            {
                                data[6] &= 0xDF;
                            }

                            data[9] ^= 0x6D;
                            var v20 = (byte)((data[0] >> 2) & 1);
                            if (((data[0] >> 6) & 1) != 0)
                            {
                                data[0] |= 4;
                            }
                            else
                            {
                                data[0] &= 0xFB;
                            }

                            if (v20 != 0)
                            {
                                data[0] |= 0x40;
                            }
                            else
                            {
                                data[0] &= 0xBF;
                            }

                            var v19 = (byte)((data[4] >> 3) & 1);
                            if (((data[4] >> 6) & 1) != 0)
                            {
                                data[4] |= 8;
                            }
                            else
                            {
                                data[4] &= 0xF7;
                            }

                            if (v19 != 0)
                            {
                                data[4] |= 0x40;
                            }
                            else
                            {
                                data[4] &= 0xBF;
                            }

                            var v10 = (byte)(data[15] >> 5);
                            data[15] *= 8;
                            data[15] |= v10;
                            var v11 = data[7];
                            data[7] = data[15];
                            data[15] = v11;
                            var v12 = (byte)(data[2] >> 5);
                            data[2] *= 8;
                            data[2] |= v12;
                        }
                    }
                    else
                    {
                        var v6 = (byte)(data[6] >> 7);
                        data[6] *= 2;
                        data[6] |= v6;
                        data[3] ^= 0xED;
                        var v7 = data[4];
                        data[4] = data[6];
                        data[6] = v7;
                        var v8 = data[5];
                        data[5] = data[6];
                        data[6] = v8;
                        var v9 = (byte)(data[2] >> 1);
                        data[2] <<= 7;
                        data[2] |= v9;
                        var v23 = (byte)((data[7] >> 1) & 1);
                        if (((data[7] >> 1) & 1) != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        if (v23 != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        data[1] ^= 0xF3;
                    }
                }
                else
                {
                    var v3 = (byte)(data[0] >> 3);
                    data[0] *= 32;
                    data[0] |= v3;
                    var v2 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 3) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    var v25 = (byte)((data[1] >> 3) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    if (v25 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    var v24 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v24 != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    var v4 = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= v4;
                    var v5 = data[2];
                    data[2] = data[3];
                    data[3] = v5;
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
                            data[3] ^= 0x26;
                            var v13 = (byte)(data[16] >> 5);
                            data[16] *= 8;
                            data[16] |= v13;
                            data[28] ^= 0x57;
                            data[19] ^= 0xF0;
                            var v14 = data[27];
                            data[27] = data[8];
                            data[8] = v14;
                            var v15 = data[26];
                            data[26] = data[6];
                            data[6] = v15;
                            var v16 = (byte)(data[15] >> 4);
                            data[15] *= 16;
                            data[15] |= v16;
                            var v17 = data[13];
                            data[13] = data[23];
                            data[23] = v17;
                            var v18 = data[13];
                            data[13] = data[7];
                            data[7] = v18;
                        }
                        else
                        {
                            var v10 = (byte)(data[2] >> 3);
                            data[2] *= 32;
                            data[2] |= v10;
                            var v11 = data[7];
                            data[7] = data[15];
                            data[15] = v11;
                            var v12 = (byte)(data[15] >> 3);
                            data[15] *= 32;
                            data[15] |= v12;
                            var v22 = (byte)((data[4] >> 3) & 1);
                            if (((data[4] >> 6) & 1) != 0)
                            {
                                data[4] |= 8;
                            }
                            else
                            {
                                data[4] &= 0xF7;
                            }

                            if (v22 != 0)
                            {
                                data[4] |= 0x40;
                            }
                            else
                            {
                                data[4] &= 0xBF;
                            }

                            var v21 = (byte)((data[0] >> 2) & 1);
                            if (((data[0] >> 6) & 1) != 0)
                            {
                                data[0] |= 4;
                            }
                            else
                            {
                                data[0] &= 0xFB;
                            }

                            if (v21 != 0)
                            {
                                data[0] |= 0x40;
                            }
                            else
                            {
                                data[0] &= 0xBF;
                            }

                            data[9] ^= 0x6D;
                            var v20 = (byte)((data[6] >> 1) & 1);
                            if (((data[6] >> 5) & 1) != 0)
                            {
                                data[6] |= 2;
                            }
                            else
                            {
                                data[6] &= 0xFD;
                            }

                            if (v20 != 0)
                            {
                                data[6] |= 0x20;
                            }
                            else
                            {
                                data[6] &= 0xDF;
                            }

                            var v19 = (byte)((data[12] >> 5) & 1);
                            if (((data[12] >> 1) & 1) != 0)
                            {
                                data[12] |= 0x20;
                            }
                            else
                            {
                                data[12] &= 0xDF;
                            }

                            if (v19 != 0)
                            {
                                data[12] |= 2;
                            }
                            else
                            {
                                data[12] &= 0xFD;
                            }
                        }
                    }
                    else
                    {
                        data[1] ^= 0xF3;
                        var v23 = (byte)((data[7] >> 1) & 1);
                        if (((data[7] >> 1) & 1) != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        if (v23 != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        var v6 = (byte)(data[2] >> 7);
                        data[2] *= 2;
                        data[2] |= v6;
                        var v7 = data[5];
                        data[5] = data[6];
                        data[6] = v7;
                        var v8 = data[4];
                        data[4] = data[6];
                        data[6] = v8;
                        data[3] ^= 0xED;
                        var v9 = (byte)(data[6] >> 1);
                        data[6] <<= 7;
                        data[6] |= v9;
                    }
                }
                else
                {
                    var v3 = data[2];
                    data[2] = data[3];
                    data[3] = v3;
                    var v4 = (byte)(data[2] >> 2);
                    data[2] <<= 6;
                    data[2] |= v4;
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

                    var v25 = (byte)((data[1] >> 3) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    if (v25 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    var v24 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 3) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v24 != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    var v5 = (byte)(data[0] >> 5);
                    data[0] *= 8;
                    data[0] |= v5;
                }
            }
        }
    }
}
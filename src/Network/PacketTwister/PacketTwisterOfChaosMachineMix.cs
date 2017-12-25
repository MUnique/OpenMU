// <copyright file="PacketTwisterOfChaosMachineMix.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'ChaosMachineMix' type.
    /// </summary>
    internal class PacketTwisterOfChaosMachineMix : IPacketTwister
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
                            var v10 = data[12];
                            data[12] = data[10];
                            data[10] = v10;
                            var v15 = (byte)((data[3] >> 7) & 1);
                            if (((data[3] >> 6) & 1) != 0)
                            {
                                data[3] |= 0x80;
                            }
                            else
                            {
                                data[3] &= 0x7F;
                            }

                            if (v15 != 0)
                            {
                                data[3] |= 0x40;
                            }
                            else
                            {
                                data[3] &= 0xBF;
                            }

                            var v11 = data[27];
                            data[27] = data[27];
                            data[27] = v11;
                            data[31] ^= 0xB4;
                            var v12 = data[3];
                            data[3] = data[2];
                            data[2] = v12;
                            var v13 = data[19];
                            data[19] = data[22];
                            data[22] = v13;
                            data[12] ^= 0x8F;
                            var v14 = data[9];
                            data[9] = data[7];
                            data[7] = v14;
                            data[26] ^= 0xA2;
                        }
                        else
                        {
                            data[10] ^= 0xD1;
                            var v8 = (byte)(data[7] >> 3);
                            data[7] *= 32;
                            data[7] |= v8;
                            var v9 = data[0];
                            data[0] = data[6];
                            data[6] = v9;
                        }
                    }
                    else
                    {
                        var v4 = data[2];
                        data[2] = data[7];
                        data[7] = v4;
                        var v5 = data[1];
                        data[1] = data[2];
                        data[2] = v5;
                        var v6 = data[7];
                        data[7] = data[4];
                        data[4] = v6;
                        var v7 = data[3];
                        data[3] = data[5];
                        data[5] = v7;
                        data[2] ^= 0xCB;
                    }
                }
                else
                {
                    var v2 = (byte)((data[2] >> 4) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    var v16 = (byte)((data[2] >> 5) & 1);
                    if (((data[2] >> 4) & 1) != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    if (v16 != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    data[3] ^= 0x14;
                    var v3 = (byte)(data[0] >> 5);
                    data[0] *= 8;
                    data[0] |= v3;
                    data[1] ^= 0xD2;
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
                            data[26] ^= 0xA2;
                            var v10 = data[9];
                            data[9] = data[7];
                            data[7] = v10;
                            data[12] ^= 0x8F;
                            var v11 = data[19];
                            data[19] = data[22];
                            data[22] = v11;
                            var v12 = data[3];
                            data[3] = data[2];
                            data[2] = v12;
                            data[31] ^= 0xB4;
                            var v13 = data[27];
                            data[27] = data[27];
                            data[27] = v13;
                            var v15 = (byte)((data[3] >> 7) & 1);
                            if (((data[3] >> 6) & 1) != 0)
                            {
                                data[3] |= 0x80;
                            }
                            else
                            {
                                data[3] &= 0x7F;
                            }

                            if (v15 != 0)
                            {
                                data[3] |= 0x40;
                            }
                            else
                            {
                                data[3] &= 0xBF;
                            }

                            var v14 = data[12];
                            data[12] = data[10];
                            data[10] = v14;
                        }
                        else
                        {
                            var v8 = data[0];
                            data[0] = data[6];
                            data[6] = v8;
                            var v9 = (byte)(data[7] >> 5);
                            data[7] *= 8;
                            data[7] |= v9;
                            data[10] ^= 0xD1;
                        }
                    }
                    else
                    {
                        data[2] ^= 0xCB;
                        var v4 = data[3];
                        data[3] = data[5];
                        data[5] = v4;
                        var v5 = data[7];
                        data[7] = data[4];
                        data[4] = v5;
                        var v6 = data[1];
                        data[1] = data[2];
                        data[2] = v6;
                        var v7 = data[2];
                        data[2] = data[7];
                        data[7] = v7;
                    }
                }
                else
                {
                    data[1] ^= 0xD2;
                    var v3 = (byte)(data[0] >> 3);
                    data[0] *= 32;
                    data[0] |= v3;
                    data[3] ^= 0x14;
                    var v2 = (byte)((data[2] >> 5) & 1);
                    if (((data[2] >> 4) & 1) != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    var v16 = (byte)((data[2] >> 4) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    if (v16 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }
                }
            }
        }
    }
}
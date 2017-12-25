// <copyright file="PacketTwisterOfClientChecksum.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'ClientChecksum' type.
    /// </summary>
    internal class PacketTwisterOfClientChecksum : IPacketTwister
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
                            var v15 = (byte)(data[5] >> 6);
                            data[5] *= 4;
                            data[5] |= v15;
                            var v19 = (byte)((data[23] >> 1) & 1);
                            if (((data[23] >> 4) & 1) != 0)
                            {
                                data[23] |= 2;
                            }
                            else
                            {
                                data[23] &= 0xFD;
                            }

                            if (v19 != 0)
                            {
                                data[23] |= 0x10;
                            }
                            else
                            {
                                data[23] &= 0xEF;
                            }

                            var v16 = (byte)(data[29] >> 6);
                            data[29] *= 4;
                            data[29] |= v16;
                            var v17 = data[24];
                            data[24] = data[2];
                            data[2] = v17;
                            var v18 = (byte)(data[28] >> 1);
                            data[28] <<= 7;
                            data[28] |= v18;
                        }
                        else
                        {
                            data[1] ^= 0x53;
                            var v10 = (byte)(data[11] >> 2);
                            data[11] <<= 6;
                            data[11] |= v10;
                            data[8] ^= 2;
                            data[7] ^= 0x21;
                            var v11 = data[15];
                            data[15] = data[0];
                            data[0] = v11;
                            var v12 = (byte)(data[1] >> 6);
                            data[1] *= 4;
                            data[1] |= v12;
                            var v13 = (byte)(data[12] >> 7);
                            data[12] *= 2;
                            data[12] |= v13;
                            var v14 = (byte)(data[3] >> 6);
                            data[3] *= 4;
                            data[3] |= v14;
                        }
                    }
                    else
                    {
                        var v7 = data[1];
                        data[1] = data[2];
                        data[2] = v7;
                        var v8 = data[6];
                        data[6] = data[3];
                        data[3] = v8;
                        var v9 = data[4];
                        data[4] = data[6];
                        data[6] = v9;
                    }
                }
                else
                {
                    var v3 = data[0];
                    data[0] = data[3];
                    data[3] = v3;
                    data[0] ^= 0xD7;
                    var v2 = (byte)((data[1] >> 5) & 1);
                    if (((data[1] >> 6) & 1) != 0)
                    {
                        data[1] |= 0x20;
                    }
                    else
                    {
                        data[1] &= 0xDF;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    var v22 = (byte)((data[1] >> 7) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    if (v22 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    var v4 = (byte)(data[1] >> 6);
                    data[1] *= 4;
                    data[1] |= v4;
                    var v21 = (byte)((data[1] >> 7) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    if (v21 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    data[1] ^= 0x2D;
                    var v5 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v5;
                    data[1] ^= 0x3F;
                    var v6 = data[3];
                    data[3] = data[0];
                    data[0] = v6;
                    var v20 = (byte)((data[2] >> 5) & 1);
                    if (((data[2] >> 5) & 1) != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    if (v20 != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
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
                            var v15 = (byte)(data[28] >> 7);
                            data[28] *= 2;
                            data[28] |= v15;
                            var v16 = data[24];
                            data[24] = data[2];
                            data[2] = v16;
                            var v17 = (byte)(data[29] >> 2);
                            data[29] <<= 6;
                            data[29] |= v17;
                            var v19 = (byte)((data[23] >> 1) & 1);
                            if (((data[23] >> 4) & 1) != 0)
                            {
                                data[23] |= 2;
                            }
                            else
                            {
                                data[23] &= 0xFD;
                            }

                            if (v19 != 0)
                            {
                                data[23] |= 0x10;
                            }
                            else
                            {
                                data[23] &= 0xEF;
                            }

                            var v18 = (byte)(data[5] >> 2);
                            data[5] <<= 6;
                            data[5] |= v18;
                        }
                        else
                        {
                            var v10 = (byte)(data[3] >> 2);
                            data[3] <<= 6;
                            data[3] |= v10;
                            var v11 = (byte)(data[12] >> 1);
                            data[12] <<= 7;
                            data[12] |= v11;
                            var v12 = (byte)(data[1] >> 2);
                            data[1] <<= 6;
                            data[1] |= v12;
                            var v13 = data[15];
                            data[15] = data[0];
                            data[0] = v13;
                            data[7] ^= 0x21;
                            data[8] ^= 2;
                            var v14 = (byte)(data[11] >> 6);
                            data[11] *= 4;
                            data[11] |= v14;
                            data[1] ^= 0x53;
                        }
                    }
                    else
                    {
                        var v7 = data[4];
                        data[4] = data[6];
                        data[6] = v7;
                        var v8 = data[6];
                        data[6] = data[3];
                        data[3] = v8;
                        var v9 = data[1];
                        data[1] = data[2];
                        data[2] = v9;
                    }
                }
                else
                {
                    var v2 = (byte)((data[2] >> 5) & 1);
                    if (((data[2] >> 5) & 1) != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    var v3 = data[3];
                    data[3] = data[0];
                    data[0] = v3;
                    data[1] ^= 0x3F;
                    var v4 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v4;
                    data[1] ^= 0x2D;
                    var v22 = (byte)((data[1] >> 7) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    if (v22 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    var v5 = (byte)(data[1] >> 2);
                    data[1] <<= 6;
                    data[1] |= v5;
                    var v21 = (byte)((data[1] >> 7) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    if (v21 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    var v20 = (byte)((data[1] >> 5) & 1);
                    if (((data[1] >> 6) & 1) != 0)
                    {
                        data[1] |= 0x20;
                    }
                    else
                    {
                        data[1] &= 0xDF;
                    }

                    if (v20 != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    data[0] ^= 0xD7;
                    var v6 = data[0];
                    data[0] = data[3];
                    data[3] = v6;
                }
            }
        }
    }
}
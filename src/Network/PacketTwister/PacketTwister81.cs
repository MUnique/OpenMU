// <copyright file="PacketTwister81.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of an unknown type.
    /// </summary>
    internal class PacketTwister81 : IPacketTwister
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
                            var v16 = (byte)((data[14] >> 6) & 1);
                            if (((data[14] >> 2) & 1) != 0)
                            {
                                data[14] |= 0x40;
                            }
                            else
                            {
                                data[14] &= 0xBF;
                            }

                            if (v16 != 0)
                            {
                                data[14] |= 4;
                            }
                            else
                            {
                                data[14] &= 0xFB;
                            }

                            var v13 = (byte)(data[9] >> 1);
                            data[9] <<= 7;
                            data[9] |= v13;
                            data[4] ^= 0xCE;
                            var v14 = (byte)(data[22] >> 2);
                            data[22] <<= 6;
                            data[22] |= v14;
                            var v15 = (byte)((data[6] >> 3) & 1);
                            if (((data[6] >> 7) & 1) != 0)
                            {
                                data[6] |= 8;
                            }
                            else
                            {
                                data[6] &= 0xF7;
                            }

                            if (v15 != 0)
                            {
                                data[6] |= 0x80;
                            }
                            else
                            {
                                data[6] &= 0x7F;
                            }

                            data[28] ^= 0x6D;
                        }
                        else
                        {
                            var v19 = (byte)((data[13] >> 5) & 1);
                            if (((data[13] >> 7) & 1) != 0)
                            {
                                data[13] |= 0x20;
                            }
                            else
                            {
                                data[13] &= 0xDF;
                            }

                            if (v19 != 0)
                            {
                                data[13] |= 0x80;
                            }
                            else
                            {
                                data[13] &= 0x7F;
                            }

                            var v12 = (byte)(data[14] >> 3);
                            data[14] *= 32;
                            data[14] |= v12;
                            var v18 = (byte)((data[3] >> 5) & 1);
                            if (((data[3] >> 5) & 1) != 0)
                            {
                                data[3] |= 0x20;
                            }
                            else
                            {
                                data[3] &= 0xDF;
                            }

                            if (v18 != 0)
                            {
                                data[3] |= 0x20;
                            }
                            else
                            {
                                data[3] &= 0xDF;
                            }

                            var v17 = (byte)((data[14] >> 2) & 1);
                            if (((data[14] >> 7) & 1) != 0)
                            {
                                data[14] |= 4;
                            }
                            else
                            {
                                data[14] &= 0xFB;
                            }

                            if (v17 != 0)
                            {
                                data[14] |= 0x80;
                            }
                            else
                            {
                                data[14] &= 0x7F;
                            }

                            data[7] ^= 0x79;
                            data[4] ^= 0x3A;
                        }
                    }
                    else
                    {
                        var v21 = (byte)((data[2] >> 4) & 1);
                        if (((data[2] >> 4) & 1) != 0)
                        {
                            data[2] |= 0x10;
                        }
                        else
                        {
                            data[2] &= 0xEF;
                        }

                        if (v21 != 0)
                        {
                            data[2] |= 0x10;
                        }
                        else
                        {
                            data[2] &= 0xEF;
                        }

                        var v9 = (byte)(data[6] >> 4);
                        data[6] *= 16;
                        data[6] |= v9;
                        var v20 = (byte)((data[7] >> 3) & 1);
                        if (((data[7] >> 7) & 1) != 0)
                        {
                            data[7] |= 8;
                        }
                        else
                        {
                            data[7] &= 0xF7;
                        }

                        if (v20 != 0)
                        {
                            data[7] |= 0x80;
                        }
                        else
                        {
                            data[7] &= 0x7F;
                        }

                        var v10 = data[5];
                        data[5] = data[2];
                        data[2] = v10;
                        var v11 = data[0];
                        data[0] = data[4];
                        data[4] = v11;
                    }
                }
                else
                {
                    var v3 = data[3];
                    data[3] = data[1];
                    data[1] = v3;
                    var v4 = data[2];
                    data[2] = data[0];
                    data[0] = v4;
                    var v5 = (byte)(data[0] >> 5);
                    data[0] *= 8;
                    data[0] |= v5;
                    var v6 = (byte)(data[1] >> 6);
                    data[1] *= 4;
                    data[1] |= v6;
                    data[0] ^= 0x7C;
                    var v7 = data[0];
                    data[0] = data[1];
                    data[1] = v7;
                    var v2 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 6) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    var v23 = (byte)((data[0] >> 3) & 1);
                    if (((data[0] >> 5) & 1) != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    if (v23 != 0)
                    {
                        data[0] |= 0x20;
                    }
                    else
                    {
                        data[0] &= 0xDF;
                    }

                    var v8 = (byte)(data[3] >> 3);
                    data[3] *= 32;
                    data[3] |= v8;
                    var v22 = (byte)((data[1] >> 1) & 1);
                    if (((data[1] >> 4) & 1) != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    if (v22 != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
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
                            data[28] ^= 0x6D;
                            var v16 = (byte)((data[6] >> 3) & 1);
                            if (((data[6] >> 7) & 1) != 0)
                            {
                                data[6] |= 8;
                            }
                            else
                            {
                                data[6] &= 0xF7;
                            }

                            if (v16 != 0)
                            {
                                data[6] |= 0x80;
                            }
                            else
                            {
                                data[6] &= 0x7F;
                            }

                            var v13 = (byte)(data[22] >> 6);
                            data[22] *= 4;
                            data[22] |= v13;
                            data[4] ^= 0xCE;
                            var v14 = (byte)(data[9] >> 7);
                            data[9] *= 2;
                            data[9] |= v14;
                            var v15 = (byte)((data[14] >> 6) & 1);
                            if (((data[14] >> 2) & 1) != 0)
                            {
                                data[14] |= 0x40;
                            }
                            else
                            {
                                data[14] &= 0xBF;
                            }

                            if (v15 != 0)
                            {
                                data[14] |= 4;
                            }
                            else
                            {
                                data[14] &= 0xFB;
                            }
                        }
                        else
                        {
                            data[4] ^= 0x3A;
                            data[7] ^= 0x79;
                            var v19 = (byte)((data[14] >> 2) & 1);
                            if (((data[14] >> 7) & 1) != 0)
                            {
                                data[14] |= 4;
                            }
                            else
                            {
                                data[14] &= 0xFB;
                            }

                            if (v19 != 0)
                            {
                                data[14] |= 0x80;
                            }
                            else
                            {
                                data[14] &= 0x7F;
                            }

                            var v18 = (byte)((data[3] >> 5) & 1);
                            if (((data[3] >> 5) & 1) != 0)
                            {
                                data[3] |= 0x20;
                            }
                            else
                            {
                                data[3] &= 0xDF;
                            }

                            if (v18 != 0)
                            {
                                data[3] |= 0x20;
                            }
                            else
                            {
                                data[3] &= 0xDF;
                            }

                            var v12 = (byte)(data[14] >> 5);
                            data[14] *= 8;
                            data[14] |= v12;
                            var v17 = (byte)((data[13] >> 5) & 1);
                            if (((data[13] >> 7) & 1) != 0)
                            {
                                data[13] |= 0x20;
                            }
                            else
                            {
                                data[13] &= 0xDF;
                            }

                            if (v17 != 0)
                            {
                                data[13] |= 0x80;
                            }
                            else
                            {
                                data[13] &= 0x7F;
                            }
                        }
                    }
                    else
                    {
                        var v9 = data[0];
                        data[0] = data[4];
                        data[4] = v9;
                        var v10 = data[5];
                        data[5] = data[2];
                        data[2] = v10;
                        var v21 = (byte)((data[7] >> 3) & 1);
                        if (((data[7] >> 7) & 1) != 0)
                        {
                            data[7] |= 8;
                        }
                        else
                        {
                            data[7] &= 0xF7;
                        }

                        if (v21 != 0)
                        {
                            data[7] |= 0x80;
                        }
                        else
                        {
                            data[7] &= 0x7F;
                        }

                        var v11 = (byte)(data[6] >> 4);
                        data[6] *= 16;
                        data[6] |= v11;
                        var v20 = (byte)((data[2] >> 4) & 1);
                        if (((data[2] >> 4) & 1) != 0)
                        {
                            data[2] |= 0x10;
                        }
                        else
                        {
                            data[2] &= 0xEF;
                        }

                        if (v20 != 0)
                        {
                            data[2] |= 0x10;
                        }
                        else
                        {
                            data[2] &= 0xEF;
                        }
                    }
                }
                else
                {
                    var v2 = (byte)((data[1] >> 1) & 1);
                    if (((data[1] >> 4) & 1) != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    var v3 = (byte)(data[3] >> 5);
                    data[3] *= 8;
                    data[3] |= v3;
                    var v23 = (byte)((data[0] >> 3) & 1);
                    if (((data[0] >> 5) & 1) != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    if (v23 != 0)
                    {
                        data[0] |= 0x20;
                    }
                    else
                    {
                        data[0] &= 0xDF;
                    }

                    var v22 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 6) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v22 != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    var v4 = data[0];
                    data[0] = data[1];
                    data[1] = v4;
                    data[0] ^= 0x7C;
                    var v5 = (byte)(data[1] >> 2);
                    data[1] <<= 6;
                    data[1] |= v5;
                    var v6 = (byte)(data[0] >> 3);
                    data[0] *= 32;
                    data[0] |= v6;
                    var v7 = data[2];
                    data[2] = data[0];
                    data[0] = v7;
                    var v8 = data[3];
                    data[3] = data[1];
                    data[1] = v8;
                }
            }
        }
    }
}
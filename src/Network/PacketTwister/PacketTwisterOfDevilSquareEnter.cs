// <copyright file="PacketTwisterOfDevilSquareEnter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'DevilSquareEnter' type.
    /// </summary>
    internal class PacketTwisterOfDevilSquareEnter : IPacketTwister
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
                            var v21 = (byte)((data[21] >> 5) & 1);
                            if (((data[21] >> 1) & 1) != 0)
                            {
                                data[21] |= 0x20;
                            }
                            else
                            {
                                data[21] &= 0xDF;
                            }

                            if (v21 != 0)
                            {
                                data[21] |= 2;
                            }
                            else
                            {
                                data[21] &= 0xFD;
                            }

                            var v17 = (byte)(data[12] >> 5);
                            data[12] *= 8;
                            data[12] |= v17;
                            data[0] ^= 0x8A;
                            var v18 = (byte)(data[17] >> 3);
                            data[17] *= 32;
                            data[17] |= v18;
                            var v19 = data[7];
                            data[7] = data[14];
                            data[14] = v19;
                            var v20 = (byte)((data[10] >> 4) & 1);
                            if (((data[10] >> 4) & 1) != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            if (v20 != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }
                        }
                        else
                        {
                            var v14 = data[0];
                            data[0] = data[3];
                            data[3] = v14;
                            data[0] ^= 0xC7;
                            var v15 = (byte)(data[7] >> 3);
                            data[7] *= 32;
                            data[7] |= v15;
                            var v16 = data[5];
                            data[5] = data[10];
                            data[10] = v16;
                            data[13] ^= 0x34;
                        }
                    }
                    else
                    {
                        var v23 = (byte)((data[7] >> 6) & 1);
                        if (((data[7] >> 4) & 1) != 0)
                        {
                            data[7] |= 0x40;
                        }
                        else
                        {
                            data[7] &= 0xBF;
                        }

                        if (v23 != 0)
                        {
                            data[7] |= 0x10;
                        }
                        else
                        {
                            data[7] &= 0xEF;
                        }

                        var v22 = (byte)((data[6] >> 4) & 1);
                        if (((data[6] >> 6) & 1) != 0)
                        {
                            data[6] |= 0x10;
                        }
                        else
                        {
                            data[6] &= 0xEF;
                        }

                        if (v22 != 0)
                        {
                            data[6] |= 0x40;
                        }
                        else
                        {
                            data[6] &= 0xBF;
                        }

                        var v10 = data[2];
                        data[2] = data[7];
                        data[7] = v10;
                        data[1] ^= 0x6A;
                        var v11 = (byte)(data[1] >> 2);
                        data[1] <<= 6;
                        data[1] |= v11;
                        var v12 = data[5];
                        data[5] = data[7];
                        data[7] = v12;
                        var v13 = (byte)(data[6] >> 1);
                        data[6] <<= 7;
                        data[6] |= v13;
                    }
                }
                else
                {
                    var v2 = (byte)((data[1] >> 7) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    var v3 = data[3];
                    data[3] = data[0];
                    data[0] = v3;
                    var v4 = (byte)(data[1] >> 6);
                    data[1] *= 4;
                    data[1] |= v4;
                    data[2] ^= 0x40;
                    var v5 = data[1];
                    data[1] = data[1];
                    data[1] = v5;
                    var v25 = (byte)((data[1] >> 1) & 1);
                    if (((data[1] >> 6) & 1) != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    if (v25 != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    var v6 = (byte)(data[0] >> 7);
                    data[0] *= 2;
                    data[0] |= v6;
                    data[3] ^= 0x90;
                    var v7 = data[3];
                    data[3] = data[0];
                    data[0] = v7;
                    var v8 = (byte)(data[3] >> 6);
                    data[3] *= 4;
                    data[3] |= v8;
                    var v24 = (byte)((data[1] >> 1) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    if (v24 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    data[0] ^= 0xE4;
                    var v9 = data[3];
                    data[3] = data[2];
                    data[2] = v9;
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
                            var v21 = (byte)((data[10] >> 4) & 1);
                            if (((data[10] >> 4) & 1) != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            if (v21 != 0)
                            {
                                data[10] |= 0x10;
                            }
                            else
                            {
                                data[10] &= 0xEF;
                            }

                            var v17 = data[7];
                            data[7] = data[14];
                            data[14] = v17;
                            var v18 = (byte)(data[17] >> 5);
                            data[17] *= 8;
                            data[17] |= v18;
                            data[0] ^= 0x8A;
                            var v19 = (byte)(data[12] >> 3);
                            data[12] *= 32;
                            data[12] |= v19;
                            var v20 = (byte)((data[21] >> 5) & 1);
                            if (((data[21] >> 1) & 1) != 0)
                            {
                                data[21] |= 0x20;
                            }
                            else
                            {
                                data[21] &= 0xDF;
                            }

                            if (v20 != 0)
                            {
                                data[21] |= 2;
                            }
                            else
                            {
                                data[21] &= 0xFD;
                            }
                        }
                        else
                        {
                            data[13] ^= 0x34;
                            var v14 = data[5];
                            data[5] = data[10];
                            data[10] = v14;
                            var v15 = (byte)(data[7] >> 5);
                            data[7] *= 8;
                            data[7] |= v15;
                            data[0] ^= 0xC7;
                            var v16 = data[0];
                            data[0] = data[3];
                            data[3] = v16;
                        }
                    }
                    else
                    {
                        var v10 = (byte)(data[6] >> 7);
                        data[6] *= 2;
                        data[6] |= v10;
                        var v11 = data[5];
                        data[5] = data[7];
                        data[7] = v11;
                        var v12 = (byte)(data[1] >> 6);
                        data[1] *= 4;
                        data[1] |= v12;
                        data[1] ^= 0x6A;
                        var v13 = data[2];
                        data[2] = data[7];
                        data[7] = v13;
                        var v23 = (byte)((data[6] >> 4) & 1);
                        if (((data[6] >> 6) & 1) != 0)
                        {
                            data[6] |= 0x10;
                        }
                        else
                        {
                            data[6] &= 0xEF;
                        }

                        if (v23 != 0)
                        {
                            data[6] |= 0x40;
                        }
                        else
                        {
                            data[6] &= 0xBF;
                        }

                        var v22 = (byte)((data[7] >> 6) & 1);
                        if (((data[7] >> 4) & 1) != 0)
                        {
                            data[7] |= 0x40;
                        }
                        else
                        {
                            data[7] &= 0xBF;
                        }

                        if (v22 != 0)
                        {
                            data[7] |= 0x10;
                        }
                        else
                        {
                            data[7] &= 0xEF;
                        }
                    }
                }
                else
                {
                    var v3 = data[3];
                    data[3] = data[2];
                    data[2] = v3;
                    data[0] ^= 0xE4;
                    var v2 = (byte)((data[1] >> 1) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    var v4 = (byte)(data[3] >> 2);
                    data[3] <<= 6;
                    data[3] |= v4;
                    var v5 = data[3];
                    data[3] = data[0];
                    data[0] = v5;
                    data[3] ^= 0x90;
                    var v6 = (byte)(data[0] >> 1);
                    data[0] <<= 7;
                    data[0] |= v6;
                    var v25 = (byte)((data[1] >> 1) & 1);
                    if (((data[1] >> 6) & 1) != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    if (v25 != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    var v7 = data[1];
                    data[1] = data[1];
                    data[1] = v7;
                    data[2] ^= 0x40;
                    var v8 = (byte)(data[1] >> 2);
                    data[1] <<= 6;
                    data[1] |= v8;
                    var v9 = data[3];
                    data[3] = data[0];
                    data[0] = v9;
                    var v24 = (byte)((data[1] >> 7) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    if (v24 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }
                }
            }
        }
    }
}
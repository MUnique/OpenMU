// <copyright file="PacketTwisterOfLetterListRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'LetterListRequest' type.
    /// </summary>
    internal class PacketTwisterOfLetterListRequest : IPacketTwister
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
                            var v18 = (byte)((data[14] >> 7) & 1);
                            if (((data[14] >> 6) & 1) != 0)
                            {
                                data[14] |= 0x80;
                            }
                            else
                            {
                                data[14] &= 0x7F;
                            }

                            if (v18 != 0)
                            {
                                data[14] |= 0x40;
                            }
                            else
                            {
                                data[14] &= 0xBF;
                            }

                            data[18] ^= 0x4D;
                            var v17 = data[26];
                            data[26] = data[6];
                            data[6] = v17;
                        }
                        else
                        {
                            var v8 = (byte)(data[10] >> 2);
                            data[10] <<= 6;
                            data[10] |= v8;
                            data[12] ^= 0xF;
                            var v9 = data[14];
                            data[14] = data[1];
                            data[1] = v9;
                            var v10 = data[7];
                            data[7] = data[8];
                            data[8] = v10;
                            data[3] ^= 0xB8;
                            var v11 = data[12];
                            data[12] = data[3];
                            data[3] = v11;
                            var v12 = data[14];
                            data[14] = data[12];
                            data[12] = v12;
                            var v13 = (byte)(data[14] >> 5);
                            data[14] *= 8;
                            data[14] |= v13;
                            var v19 = (byte)((data[2] >> 5) & 1);
                            if (((data[2] >> 1) & 1) != 0)
                            {
                                data[2] |= 0x20;
                            }
                            else
                            {
                                data[2] &= 0xDF;
                            }

                            if (v19 != 0)
                            {
                                data[2] |= 2;
                            }
                            else
                            {
                                data[2] &= 0xFD;
                            }

                            data[1] ^= 0xD5;
                            var v14 = data[9];
                            data[9] = data[14];
                            data[14] = v14;
                            var v15 = data[12];
                            data[12] = data[6];
                            data[6] = v15;
                            var v16 = data[2];
                            data[2] = data[4];
                            data[4] = v16;
                        }
                    }
                    else
                    {
                        data[2] ^= 0xC5;
                        var v5 = data[5];
                        data[5] = data[3];
                        data[3] = v5;
                        var v22 = (byte)((data[4] >> 6) & 1);
                        if (((data[4] >> 5) & 1) != 0)
                        {
                            data[4] |= 0x40;
                        }
                        else
                        {
                            data[4] &= 0xBF;
                        }

                        if (v22 != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        var v21 = (byte)((data[1] >> 1) & 1);
                        if (((data[1] >> 7) & 1) != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        if (v21 != 0)
                        {
                            data[1] |= 0x80;
                        }
                        else
                        {
                            data[1] &= 0x7F;
                        }

                        var v6 = (byte)(data[5] >> 5);
                        data[5] *= 8;
                        data[5] |= v6;
                        var v7 = data[7];
                        data[7] = data[5];
                        data[5] = v7;
                        var v20 = (byte)((data[6] >> 4) & 1);
                        if (((data[6] >> 7) & 1) != 0)
                        {
                            data[6] |= 0x10;
                        }
                        else
                        {
                            data[6] &= 0xEF;
                        }

                        if (v20 != 0)
                        {
                            data[6] |= 0x80;
                        }
                        else
                        {
                            data[6] &= 0x7F;
                        }
                    }
                }
                else
                {
                    var v2 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 3) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    var v3 = (byte)(data[2] >> 2);
                    data[2] <<= 6;
                    data[2] |= v3;
                    var v4 = data[1];
                    data[1] = data[2];
                    data[2] = v4;
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
                            var v17 = data[26];
                            data[26] = data[6];
                            data[6] = v17;
                            data[18] ^= 0x4D;
                            var v18 = (byte)((data[14] >> 7) & 1);
                            if (((data[14] >> 6) & 1) != 0)
                            {
                                data[14] |= 0x80;
                            }
                            else
                            {
                                data[14] &= 0x7F;
                            }

                            if (v18 != 0)
                            {
                                data[14] |= 0x40;
                            }
                            else
                            {
                                data[14] &= 0xBF;
                            }
                        }
                        else
                        {
                            var v8 = data[2];
                            data[2] = data[4];
                            data[4] = v8;
                            var v9 = data[12];
                            data[12] = data[6];
                            data[6] = v9;
                            var v10 = data[9];
                            data[9] = data[14];
                            data[14] = v10;
                            data[1] ^= 0xD5;
                            var v19 = (byte)((data[2] >> 5) & 1);
                            if (((data[2] >> 1) & 1) != 0)
                            {
                                data[2] |= 0x20;
                            }
                            else
                            {
                                data[2] &= 0xDF;
                            }

                            if (v19 != 0)
                            {
                                data[2] |= 2;
                            }
                            else
                            {
                                data[2] &= 0xFD;
                            }

                            var v11 = (byte)(data[14] >> 3);
                            data[14] *= 32;
                            data[14] |= v11;
                            var v12 = data[14];
                            data[14] = data[12];
                            data[12] = v12;
                            var v13 = data[12];
                            data[12] = data[3];
                            data[3] = v13;
                            data[3] ^= 0xB8;
                            var v14 = data[7];
                            data[7] = data[8];
                            data[8] = v14;
                            var v15 = data[14];
                            data[14] = data[1];
                            data[1] = v15;
                            data[12] ^= 0xF;
                            var v16 = (byte)(data[10] >> 6);
                            data[10] *= 4;
                            data[10] |= v16;
                        }
                    }
                    else
                    {
                        var v22 = (byte)((data[6] >> 4) & 1);
                        if (((data[6] >> 7) & 1) != 0)
                        {
                            data[6] |= 0x10;
                        }
                        else
                        {
                            data[6] &= 0xEF;
                        }

                        if (v22 != 0)
                        {
                            data[6] |= 0x80;
                        }
                        else
                        {
                            data[6] &= 0x7F;
                        }

                        var v5 = data[7];
                        data[7] = data[5];
                        data[5] = v5;
                        var v6 = (byte)(data[5] >> 3);
                        data[5] *= 32;
                        data[5] |= v6;
                        var v21 = (byte)((data[1] >> 1) & 1);
                        if (((data[1] >> 7) & 1) != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        if (v21 != 0)
                        {
                            data[1] |= 0x80;
                        }
                        else
                        {
                            data[1] &= 0x7F;
                        }

                        var v20 = (byte)((data[4] >> 6) & 1);
                        if (((data[4] >> 5) & 1) != 0)
                        {
                            data[4] |= 0x40;
                        }
                        else
                        {
                            data[4] &= 0xBF;
                        }

                        if (v20 != 0)
                        {
                            data[4] |= 0x20;
                        }
                        else
                        {
                            data[4] &= 0xDF;
                        }

                        var v7 = data[5];
                        data[5] = data[3];
                        data[3] = v7;
                        data[2] ^= 0xC5;
                    }
                }
                else
                {
                    var v3 = data[1];
                    data[1] = data[2];
                    data[2] = v3;
                    var v4 = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= v4;
                    var v2 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 3) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }
                }
            }
        }
    }
}
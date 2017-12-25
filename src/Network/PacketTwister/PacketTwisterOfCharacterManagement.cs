// <copyright file="PacketTwisterOfCharacterManagement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'CharacterManagement' type.
    /// </summary>
    internal class PacketTwisterOfCharacterManagement : IPacketTwister
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
                            data[29] ^= 0xC0;
                            var v14 = (byte)(data[12] >> 4);
                            data[12] *= 16;
                            data[12] |= v14;
                            data[14] ^= 0xAF;
                        }
                        else
                        {
                            var v16 = (byte)((data[1] >> 2) & 1);
                            if (((data[1] >> 6) & 1) != 0)
                            {
                                data[1] |= 4;
                            }
                            else
                            {
                                data[1] &= 0xFB;
                            }

                            if (v16 != 0)
                            {
                                data[1] |= 0x40;
                            }
                            else
                            {
                                data[1] &= 0xBF;
                            }

                            var v10 = data[2];
                            data[2] = data[8];
                            data[8] = v10;
                            data[5] ^= 0x99;
                            data[1] ^= 0x94;
                            var v11 = data[10];
                            data[10] = data[7];
                            data[7] = v11;
                            var v12 = (byte)(data[7] >> 4);
                            data[7] *= 16;
                            data[7] |= v12;
                            var v15 = (byte)((data[2] >> 2) & 1);
                            if (((data[2] >> 6) & 1) != 0)
                            {
                                data[2] |= 4;
                            }
                            else
                            {
                                data[2] &= 0xFB;
                            }

                            if (v15 != 0)
                            {
                                data[2] |= 0x40;
                            }
                            else
                            {
                                data[2] &= 0xBF;
                            }

                            var v13 = (byte)(data[13] >> 6);
                            data[13] *= 4;
                            data[13] |= v13;
                        }
                    }
                    else
                    {
                        var v21 = (byte)((data[5] >> 2) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 4;
                        }
                        else
                        {
                            data[5] &= 0xFB;
                        }

                        if (v21 != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        var v6 = data[6];
                        data[6] = data[7];
                        data[7] = v6;
                        var v7 = data[0];
                        data[0] = data[5];
                        data[5] = v7;
                        data[3] ^= 0x6E;
                        var v8 = data[7];
                        data[7] = data[7];
                        data[7] = v8;
                        data[7] ^= 0x2A;
                        var v20 = (byte)((data[3] >> 7) & 1);
                        if (((data[3] >> 6) & 1) != 0)
                        {
                            data[3] |= 0x80;
                        }
                        else
                        {
                            data[3] &= 0x7F;
                        }

                        if (v20 != 0)
                        {
                            data[3] |= 0x40;
                        }
                        else
                        {
                            data[3] &= 0xBF;
                        }

                        data[7] ^= 0xE6;
                        var v19 = (byte)((data[7] >> 1) & 1);
                        if (((data[7] >> 2) & 1) != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        if (v19 != 0)
                        {
                            data[7] |= 4;
                        }
                        else
                        {
                            data[7] &= 0xFB;
                        }

                        var v18 = (byte)((data[2] >> 4) & 1);
                        if (((data[2] >> 5) & 1) != 0)
                        {
                            data[2] |= 0x10;
                        }
                        else
                        {
                            data[2] &= 0xEF;
                        }

                        if (v18 != 0)
                        {
                            data[2] |= 0x20;
                        }
                        else
                        {
                            data[2] &= 0xDF;
                        }

                        data[5] ^= 0xC2;
                        var v17 = (byte)((data[2] >> 3) & 1);
                        if (((data[2] >> 6) & 1) != 0)
                        {
                            data[2] |= 8;
                        }
                        else
                        {
                            data[2] &= 0xF7;
                        }

                        if (v17 != 0)
                        {
                            data[2] |= 0x40;
                        }
                        else
                        {
                            data[2] &= 0xBF;
                        }

                        var v9 = data[5];
                        data[5] = data[6];
                        data[6] = v9;
                    }
                }
                else
                {
                    var v3 = data[1];
                    data[1] = data[0];
                    data[0] = v3;
                    var v4 = data[0];
                    data[0] = data[1];
                    data[1] = v4;
                    var v5 = (byte)(data[1] >> 7);
                    data[1] *= 2;
                    data[1] |= v5;
                    var v2 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
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
                            data[14] ^= 0xAF;
                            var v14 = (byte)(data[12] >> 4);
                            data[12] *= 16;
                            data[12] |= v14;
                            data[29] ^= 0xC0;
                        }
                        else
                        {
                            var v10 = (byte)(data[13] >> 2);
                            data[13] <<= 6;
                            data[13] |= v10;
                            var v16 = (byte)((data[2] >> 2) & 1);
                            if (((data[2] >> 6) & 1) != 0)
                            {
                                data[2] |= 4;
                            }
                            else
                            {
                                data[2] &= 0xFB;
                            }

                            if (v16 != 0)
                            {
                                data[2] |= 0x40;
                            }
                            else
                            {
                                data[2] &= 0xBF;
                            }

                            var v11 = (byte)(data[7] >> 4);
                            data[7] *= 16;
                            data[7] |= v11;
                            var v12 = data[10];
                            data[10] = data[7];
                            data[7] = v12;
                            data[1] ^= 0x94;
                            data[5] ^= 0x99;
                            var v13 = data[2];
                            data[2] = data[8];
                            data[8] = v13;
                            var v15 = (byte)((data[1] >> 2) & 1);
                            if (((data[1] >> 6) & 1) != 0)
                            {
                                data[1] |= 4;
                            }
                            else
                            {
                                data[1] &= 0xFB;
                            }

                            if (v15 != 0)
                            {
                                data[1] |= 0x40;
                            }
                            else
                            {
                                data[1] &= 0xBF;
                            }
                        }
                    }
                    else
                    {
                        var v6 = data[5];
                        data[5] = data[6];
                        data[6] = v6;
                        var v21 = (byte)((data[2] >> 3) & 1);
                        if (((data[2] >> 6) & 1) != 0)
                        {
                            data[2] |= 8;
                        }
                        else
                        {
                            data[2] &= 0xF7;
                        }

                        if (v21 != 0)
                        {
                            data[2] |= 0x40;
                        }
                        else
                        {
                            data[2] &= 0xBF;
                        }

                        data[5] ^= 0xC2;
                        var v20 = (byte)((data[2] >> 4) & 1);
                        if (((data[2] >> 5) & 1) != 0)
                        {
                            data[2] |= 0x10;
                        }
                        else
                        {
                            data[2] &= 0xEF;
                        }

                        if (v20 != 0)
                        {
                            data[2] |= 0x20;
                        }
                        else
                        {
                            data[2] &= 0xDF;
                        }

                        var v19 = (byte)((data[7] >> 1) & 1);
                        if (((data[7] >> 2) & 1) != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        if (v19 != 0)
                        {
                            data[7] |= 4;
                        }
                        else
                        {
                            data[7] &= 0xFB;
                        }

                        data[7] ^= 0xE6;
                        var v18 = (byte)((data[3] >> 7) & 1);
                        if (((data[3] >> 6) & 1) != 0)
                        {
                            data[3] |= 0x80;
                        }
                        else
                        {
                            data[3] &= 0x7F;
                        }

                        if (v18 != 0)
                        {
                            data[3] |= 0x40;
                        }
                        else
                        {
                            data[3] &= 0xBF;
                        }

                        data[7] ^= 0x2A;
                        var v7 = data[7];
                        data[7] = data[7];
                        data[7] = v7;
                        data[3] ^= 0x6E;
                        var v8 = data[0];
                        data[0] = data[5];
                        data[5] = v8;
                        var v9 = data[6];
                        data[6] = data[7];
                        data[7] = v9;
                        var v17 = (byte)((data[5] >> 2) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 4;
                        }
                        else
                        {
                            data[5] &= 0xFB;
                        }

                        if (v17 != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }
                    }
                }
                else
                {
                    var v2 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    var v3 = (byte)(data[1] >> 1);
                    data[1] <<= 7;
                    data[1] |= v3;
                    var v4 = data[0];
                    data[0] = data[1];
                    data[1] = v4;
                    var v5 = data[1];
                    data[1] = data[0];
                    data[0] = v5;
                }
            }
        }
    }
}
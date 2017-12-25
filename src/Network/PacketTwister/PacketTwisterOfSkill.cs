// <copyright file="PacketTwisterOfSkill.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'Skill' type.
    /// </summary>
    internal class PacketTwisterOfSkill : IPacketTwister
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
                            var v15 = data[16];
                            data[16] = data[17];
                            data[17] = v15;
                            var v17 = (byte)((data[10] >> 5) & 1);
                            if (((data[10] >> 7) & 1) != 0)
                            {
                                data[10] |= 0x20;
                            }
                            else
                            {
                                data[10] &= 0xDF;
                            }

                            if (v17 != 0)
                            {
                                data[10] |= 0x80;
                            }
                            else
                            {
                                data[10] &= 0x7F;
                            }

                            var v16 = (byte)((data[5] >> 3) & 1);
                            if (((data[5] >> 1) & 1) != 0)
                            {
                                data[5] |= 8;
                            }
                            else
                            {
                                data[5] &= 0xF7;
                            }

                            if (v16 != 0)
                            {
                                data[5] |= 2;
                            }
                            else
                            {
                                data[5] &= 0xFD;
                            }
                        }
                        else
                        {
                            data[8] ^= 0xB5;
                            var v12 = (byte)(data[8] >> 6);
                            data[8] *= 4;
                            data[8] |= v12;
                            var v18 = (byte)((data[11] >> 2) & 1);
                            if (((data[11] >> 4) & 1) != 0)
                            {
                                data[11] |= 4;
                            }
                            else
                            {
                                data[11] &= 0xFB;
                            }

                            if (v18 != 0)
                            {
                                data[11] |= 0x10;
                            }
                            else
                            {
                                data[11] &= 0xEF;
                            }

                            var v13 = (byte)(data[6] >> 6);
                            data[6] *= 4;
                            data[6] |= v13;
                            var v14 = (byte)(data[8] >> 6);
                            data[8] *= 4;
                            data[8] |= v14;
                        }
                    }
                    else
                    {
                        var v7 = data[0];
                        data[0] = data[5];
                        data[5] = v7;
                        var v8 = (byte)(data[5] >> 2);
                        data[5] <<= 6;
                        data[5] |= v8;
                        var v20 = (byte)((data[4] >> 4) & 1);
                        if (((data[4] >> 2) & 1) != 0)
                        {
                            data[4] |= 0x10;
                        }
                        else
                        {
                            data[4] &= 0xEF;
                        }

                        if (v20 != 0)
                        {
                            data[4] |= 4;
                        }
                        else
                        {
                            data[4] &= 0xFB;
                        }

                        var v9 = data[1];
                        data[1] = data[2];
                        data[2] = v9;
                        var v10 = (byte)(data[2] >> 5);
                        data[2] *= 8;
                        data[2] |= v10;
                        var v19 = (byte)((data[2] >> 1) & 1);
                        if (((data[2] >> 6) & 1) != 0)
                        {
                            data[2] |= 2;
                        }
                        else
                        {
                            data[2] &= 0xFD;
                        }

                        if (v19 != 0)
                        {
                            data[2] |= 0x40;
                        }
                        else
                        {
                            data[2] &= 0xBF;
                        }

                        var v11 = (byte)(data[2] >> 6);
                        data[2] *= 4;
                        data[2] |= v11;
                        data[1] ^= 0x11;
                    }
                }
                else
                {
                    var v2 = (byte)((data[3] >> 1) & 1);
                    if (((data[3] >> 6) & 1) != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }

                    var v3 = data[1];
                    data[1] = data[0];
                    data[0] = v3;
                    var v24 = (byte)((data[0] >> 7) & 1);
                    if (((data[0] >> 2) & 1) != 0)
                    {
                        data[0] |= 0x80;
                    }
                    else
                    {
                        data[0] &= 0x7F;
                    }

                    if (v24 != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    var v23 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v23 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v22 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 3) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v22 != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    var v4 = data[0];
                    data[0] = data[0];
                    data[0] = v4;
                    var v5 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v5;
                    data[3] ^= 0x10;
                    var v6 = (byte)(data[2] >> 3);
                    data[2] *= 32;
                    data[2] |= v6;
                    var v21 = (byte)((data[2] >> 6) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 0x40;
                    }
                    else
                    {
                        data[2] &= 0xBF;
                    }

                    if (v21 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    data[0] ^= 0xAB;
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
                            var v17 = (byte)((data[5] >> 3) & 1);
                            if (((data[5] >> 1) & 1) != 0)
                            {
                                data[5] |= 8;
                            }
                            else
                            {
                                data[5] &= 0xF7;
                            }

                            if (v17 != 0)
                            {
                                data[5] |= 2;
                            }
                            else
                            {
                                data[5] &= 0xFD;
                            }

                            var v16 = (byte)((data[10] >> 5) & 1);
                            if (((data[10] >> 7) & 1) != 0)
                            {
                                data[10] |= 0x20;
                            }
                            else
                            {
                                data[10] &= 0xDF;
                            }

                            if (v16 != 0)
                            {
                                data[10] |= 0x80;
                            }
                            else
                            {
                                data[10] &= 0x7F;
                            }

                            var v15 = data[16];
                            data[16] = data[17];
                            data[17] = v15;
                        }
                        else
                        {
                            var v12 = (byte)(data[8] >> 2);
                            data[8] <<= 6;
                            data[8] |= v12;
                            var v13 = (byte)(data[6] >> 2);
                            data[6] <<= 6;
                            data[6] |= v13;
                            var v18 = (byte)((data[11] >> 2) & 1);
                            if (((data[11] >> 4) & 1) != 0)
                            {
                                data[11] |= 4;
                            }
                            else
                            {
                                data[11] &= 0xFB;
                            }

                            if (v18 != 0)
                            {
                                data[11] |= 0x10;
                            }
                            else
                            {
                                data[11] &= 0xEF;
                            }

                            var v14 = (byte)(data[8] >> 2);
                            data[8] <<= 6;
                            data[8] |= v14;
                            data[8] ^= 0xB5;
                        }
                    }
                    else
                    {
                        data[1] ^= 0x11;
                        var v7 = (byte)(data[2] >> 2);
                        data[2] <<= 6;
                        data[2] |= v7;
                        var v20 = (byte)((data[2] >> 1) & 1);
                        if (((data[2] >> 6) & 1) != 0)
                        {
                            data[2] |= 2;
                        }
                        else
                        {
                            data[2] &= 0xFD;
                        }

                        if (v20 != 0)
                        {
                            data[2] |= 0x40;
                        }
                        else
                        {
                            data[2] &= 0xBF;
                        }

                        var v8 = (byte)(data[2] >> 3);
                        data[2] *= 32;
                        data[2] |= v8;
                        var v9 = data[1];
                        data[1] = data[2];
                        data[2] = v9;
                        var v19 = (byte)((data[4] >> 4) & 1);
                        if (((data[4] >> 2) & 1) != 0)
                        {
                            data[4] |= 0x10;
                        }
                        else
                        {
                            data[4] &= 0xEF;
                        }

                        if (v19 != 0)
                        {
                            data[4] |= 4;
                        }
                        else
                        {
                            data[4] &= 0xFB;
                        }

                        var v10 = (byte)(data[5] >> 6);
                        data[5] *= 4;
                        data[5] |= v10;
                        var v11 = data[0];
                        data[0] = data[5];
                        data[5] = v11;
                    }
                }
                else
                {
                    data[0] ^= 0xAB;
                    var v2 = (byte)((data[2] >> 6) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 0x40;
                    }
                    else
                    {
                        data[2] &= 0xBF;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    var v3 = (byte)(data[2] >> 5);
                    data[2] *= 8;
                    data[2] |= v3;
                    data[3] ^= 0x10;
                    var v4 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v4;
                    var v5 = data[0];
                    data[0] = data[0];
                    data[0] = v5;
                    var v24 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 3) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v24 != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    var v23 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v23 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v22 = (byte)((data[0] >> 7) & 1);
                    if (((data[0] >> 2) & 1) != 0)
                    {
                        data[0] |= 0x80;
                    }
                    else
                    {
                        data[0] &= 0x7F;
                    }

                    if (v22 != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    var v6 = data[1];
                    data[1] = data[0];
                    data[0] = v6;
                    var v21 = (byte)((data[3] >> 1) & 1);
                    if (((data[3] >> 6) & 1) != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    if (v21 != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }
                }
            }
        }
    }
}
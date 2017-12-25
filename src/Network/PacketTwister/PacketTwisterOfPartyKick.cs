// <copyright file="PacketTwisterOfPartyKick.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'PartyKick' type.
    /// </summary>
    internal class PacketTwisterOfPartyKick : IPacketTwister
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
                            var v17 = (byte)(data[24] >> 5);
                            data[24] *= 8;
                            data[24] |= v17;
                            var v18 = data[23];
                            data[23] = data[14];
                            data[14] = v18;
                            var v19 = data[16];
                            data[16] = data[3];
                            data[3] = v19;
                            data[7] ^= 0x47;
                            var v20 = (byte)((data[28] >> 3) & 1);
                            if (((data[28] >> 1) & 1) != 0)
                            {
                                data[28] |= 8;
                            }
                            else
                            {
                                data[28] &= 0xF7;
                            }

                            if (v20 != 0)
                            {
                                data[28] |= 2;
                            }
                            else
                            {
                                data[28] &= 0xFD;
                            }
                        }
                        else
                        {
                            data[14] ^= 0xA0;
                            var v13 = data[3];
                            data[3] = data[10];
                            data[10] = v13;
                            data[15] ^= 0x2C;
                            var v14 = data[3];
                            data[3] = data[3];
                            data[3] = v14;
                            var v15 = (byte)(data[10] >> 3);
                            data[10] *= 32;
                            data[10] |= v15;
                            var v16 = data[3];
                            data[3] = data[3];
                            data[3] = v16;
                            var v22 = (byte)((data[4] >> 6) & 1);
                            if (((data[4] >> 6) & 1) != 0)
                            {
                                data[4] |= 0x40;
                            }
                            else
                            {
                                data[4] &= 0xBF;
                            }

                            if (v22 != 0)
                            {
                                data[4] |= 0x40;
                            }
                            else
                            {
                                data[4] &= 0xBF;
                            }

                            var v21 = (byte)((data[3] >> 2) & 1);
                            if (((data[3] >> 1) & 1) != 0)
                            {
                                data[3] |= 4;
                            }
                            else
                            {
                                data[3] &= 0xFB;
                            }

                            if (v21 != 0)
                            {
                                data[3] |= 2;
                            }
                            else
                            {
                                data[3] &= 0xFD;
                            }
                        }
                    }
                    else
                    {
                        data[3] ^= 0xD9;
                        var v24 = (byte)((data[7] >> 7) & 1);
                        if (((data[7] >> 1) & 1) != 0)
                        {
                            data[7] |= 0x80;
                        }
                        else
                        {
                            data[7] &= 0x7F;
                        }

                        if (v24 != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        data[7] ^= 0x7B;
                        var v11 = (byte)(data[6] >> 7);
                        data[6] *= 2;
                        data[6] |= v11;
                        var v12 = data[6];
                        data[6] = data[1];
                        data[1] = v12;
                        var v23 = (byte)((data[5] >> 7) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 0x80;
                        }
                        else
                        {
                            data[5] &= 0x7F;
                        }

                        if (v23 != 0)
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
                    var v3 = data[1];
                    data[1] = data[0];
                    data[0] = v3;
                    var v4 = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= v4;
                    var v5 = data[3];
                    data[3] = data[2];
                    data[2] = v5;
                    var v6 = data[2];
                    data[2] = data[0];
                    data[0] = v6;
                    var v7 = (byte)(data[3] >> 7);
                    data[3] *= 2;
                    data[3] |= v7;
                    var v2 = (byte)((data[2] >> 5) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    var v27 = (byte)((data[3] >> 5) & 1);
                    if (((data[3] >> 3) & 1) != 0)
                    {
                        data[3] |= 0x20;
                    }
                    else
                    {
                        data[3] &= 0xDF;
                    }

                    if (v27 != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    var v8 = data[3];
                    data[3] = data[0];
                    data[0] = v8;
                    var v9 = data[2];
                    data[2] = data[2];
                    data[2] = v9;
                    var v10 = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= v10;
                    var v26 = (byte)((data[3] >> 5) & 1);
                    if (((data[3] >> 3) & 1) != 0)
                    {
                        data[3] |= 0x20;
                    }
                    else
                    {
                        data[3] &= 0xDF;
                    }

                    if (v26 != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    var v25 = (byte)((data[0] >> 4) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 0x10;
                    }
                    else
                    {
                        data[0] &= 0xEF;
                    }

                    if (v25 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
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
                            var v20 = (byte)((data[28] >> 3) & 1);
                            if (((data[28] >> 1) & 1) != 0)
                            {
                                data[28] |= 8;
                            }
                            else
                            {
                                data[28] &= 0xF7;
                            }

                            if (v20 != 0)
                            {
                                data[28] |= 2;
                            }
                            else
                            {
                                data[28] &= 0xFD;
                            }

                            data[7] ^= 0x47;
                            var v17 = data[16];
                            data[16] = data[3];
                            data[3] = v17;
                            var v18 = data[23];
                            data[23] = data[14];
                            data[14] = v18;
                            var v19 = (byte)(data[24] >> 3);
                            data[24] *= 32;
                            data[24] |= v19;
                        }
                        else
                        {
                            var v22 = (byte)((data[3] >> 2) & 1);
                            if (((data[3] >> 1) & 1) != 0)
                            {
                                data[3] |= 4;
                            }
                            else
                            {
                                data[3] &= 0xFB;
                            }

                            if (v22 != 0)
                            {
                                data[3] |= 2;
                            }
                            else
                            {
                                data[3] &= 0xFD;
                            }

                            var v21 = (byte)((data[4] >> 6) & 1);
                            if (((data[4] >> 6) & 1) != 0)
                            {
                                data[4] |= 0x40;
                            }
                            else
                            {
                                data[4] &= 0xBF;
                            }

                            if (v21 != 0)
                            {
                                data[4] |= 0x40;
                            }
                            else
                            {
                                data[4] &= 0xBF;
                            }

                            var v13 = data[3];
                            data[3] = data[3];
                            data[3] = v13;
                            var v14 = (byte)(data[10] >> 5);
                            data[10] *= 8;
                            data[10] |= v14;
                            var v15 = data[3];
                            data[3] = data[3];
                            data[3] = v15;
                            data[15] ^= 0x2C;
                            var v16 = data[3];
                            data[3] = data[10];
                            data[10] = v16;
                            data[14] ^= 0xA0;
                        }
                    }
                    else
                    {
                        var v24 = (byte)((data[5] >> 7) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 0x80;
                        }
                        else
                        {
                            data[5] &= 0x7F;
                        }

                        if (v24 != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        var v11 = data[6];
                        data[6] = data[1];
                        data[1] = v11;
                        var v12 = (byte)(data[6] >> 1);
                        data[6] <<= 7;
                        data[6] |= v12;
                        data[7] ^= 0x7B;
                        var v23 = (byte)((data[7] >> 7) & 1);
                        if (((data[7] >> 1) & 1) != 0)
                        {
                            data[7] |= 0x80;
                        }
                        else
                        {
                            data[7] &= 0x7F;
                        }

                        if (v23 != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        data[3] ^= 0xD9;
                    }
                }
                else
                {
                    var v2 = (byte)((data[0] >> 4) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 0x10;
                    }
                    else
                    {
                        data[0] &= 0xEF;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v27 = (byte)((data[3] >> 5) & 1);
                    if (((data[3] >> 3) & 1) != 0)
                    {
                        data[3] |= 0x20;
                    }
                    else
                    {
                        data[3] &= 0xDF;
                    }

                    if (v27 != 0)
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
                    var v4 = data[2];
                    data[2] = data[2];
                    data[2] = v4;
                    var v5 = data[3];
                    data[3] = data[0];
                    data[0] = v5;
                    var v26 = (byte)((data[3] >> 5) & 1);
                    if (((data[3] >> 3) & 1) != 0)
                    {
                        data[3] |= 0x20;
                    }
                    else
                    {
                        data[3] &= 0xDF;
                    }

                    if (v26 != 0)
                    {
                        data[3] |= 8;
                    }
                    else
                    {
                        data[3] &= 0xF7;
                    }

                    var v25 = (byte)((data[2] >> 5) & 1);
                    if (((data[2] >> 1) & 1) != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    if (v25 != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    var v6 = (byte)(data[3] >> 1);
                    data[3] <<= 7;
                    data[3] |= v6;
                    var v7 = data[2];
                    data[2] = data[0];
                    data[0] = v7;
                    var v8 = data[3];
                    data[3] = data[2];
                    data[2] = v8;
                    var v9 = (byte)(data[2] >> 2);
                    data[2] <<= 6;
                    data[2] |= v9;
                    var v10 = data[1];
                    data[1] = data[0];
                    data[0] = v10;
                }
            }
        }
    }
}
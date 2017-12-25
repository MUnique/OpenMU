// <copyright file="PacketTwisterOfAllianceJoinRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'AllianceJoinRequest' type.
    /// </summary>
    internal class PacketTwisterOfAllianceJoinRequest : IPacketTwister
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
                            data[8] ^= 0x15;
                            var v20 = data[1];
                            data[1] = data[17];
                            data[17] = v20;
                            data[2] ^= 0x27;
                            var v21 = data[3];
                            data[3] = data[20];
                            data[20] = v21;
                            var v26 = (byte)((data[30] >> 3) & 1);
                            if (((data[30] >> 7) & 1) != 0)
                            {
                                data[30] |= 8;
                            }
                            else
                            {
                                data[30] &= 0xF7;
                            }

                            if (v26 != 0)
                            {
                                data[30] |= 0x80;
                            }
                            else
                            {
                                data[30] &= 0x7F;
                            }

                            var v22 = (byte)(data[2] >> 2);
                            data[2] <<= 6;
                            data[2] |= v22;
                            var v25 = (byte)((data[2] >> 2) & 1);
                            if (((data[2] >> 7) & 1) != 0)
                            {
                                data[2] |= 4;
                            }
                            else
                            {
                                data[2] &= 0xFB;
                            }

                            if (v25 != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }

                            var v23 = data[16];
                            data[16] = data[16];
                            data[16] = v23;
                            data[31] ^= 0x97;
                            var v24 = (byte)(data[6] >> 6);
                            data[6] *= 4;
                            data[6] |= v24;
                        }
                        else
                        {
                            var v11 = data[14];
                            data[14] = data[2];
                            data[2] = v11;
                            var v12 = (byte)(data[5] >> 6);
                            data[5] *= 4;
                            data[5] |= v12;
                            var v13 = (byte)(data[7] >> 2);
                            data[7] <<= 6;
                            data[7] |= v13;
                            data[10] ^= 0x94;
                            var v14 = (byte)(data[2] >> 3);
                            data[2] *= 32;
                            data[2] |= v14;
                            var v15 = data[0];
                            data[0] = data[5];
                            data[5] = v15;
                            var v16 = data[1];
                            data[1] = data[15];
                            data[15] = v16;
                            var v17 = data[15];
                            data[15] = data[15];
                            data[15] = v17;
                            var v18 = data[13];
                            data[13] = data[1];
                            data[1] = v18;
                            var v19 = data[5];
                            data[5] = data[12];
                            data[12] = v19;
                        }
                    }
                    else
                    {
                        data[4] ^= 0xAF;
                        data[2] ^= 0x77;
                        data[0] ^= 0xA7;
                        var v29 = (byte)((data[3] >> 3) & 1);
                        if (((data[3] >> 6) & 1) != 0)
                        {
                            data[3] |= 8;
                        }
                        else
                        {
                            data[3] &= 0xF7;
                        }

                        if (v29 != 0)
                        {
                            data[3] |= 0x40;
                        }
                        else
                        {
                            data[3] &= 0xBF;
                        }

                        var v28 = (byte)((data[1] >> 2) & 1);
                        if (((data[1] >> 2) & 1) != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        if (v28 != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        var v10 = (byte)(data[7] >> 6);
                        data[7] *= 4;
                        data[7] |= v10;
                        var v27 = (byte)((data[2] >> 2) & 1);
                        if (((data[2] >> 5) & 1) != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        if (v27 != 0)
                        {
                            data[2] |= 0x20;
                        }
                        else
                        {
                            data[2] &= 0xDF;
                        }

                        data[7] ^= 0xE1;
                        data[2] ^= 5;
                        data[2] ^= 0x73;
                    }
                }
                else
                {
                    var v2 = (byte)((data[3] >> 1) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    var v3 = (byte)(data[3] >> 5);
                    data[3] *= 8;
                    data[3] |= v3;
                    data[2] ^= 0xED;
                    data[3] ^= 0x52;
                    var v4 = (byte)(data[0] >> 7);
                    data[0] *= 2;
                    data[0] |= v4;
                    var v5 = (byte)(data[2] >> 2);
                    data[2] <<= 6;
                    data[2] |= v5;
                    var v6 = data[2];
                    data[2] = data[0];
                    data[0] = v6;
                    var v7 = data[3];
                    data[3] = data[3];
                    data[3] = v7;
                    var v8 = (byte)(data[1] >> 6);
                    data[1] *= 4;
                    data[1] |= v8;
                    data[1] ^= 0x67;
                    var v30 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 3) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v30 != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    var v9 = (byte)(data[1] >> 7);
                    data[1] *= 2;
                    data[1] |= v9;
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
                            var v20 = (byte)(data[6] >> 2);
                            data[6] <<= 6;
                            data[6] |= v20;
                            data[31] ^= 0x97;
                            var v21 = data[16];
                            data[16] = data[16];
                            data[16] = v21;
                            var v26 = (byte)((data[2] >> 2) & 1);
                            if (((data[2] >> 7) & 1) != 0)
                            {
                                data[2] |= 4;
                            }
                            else
                            {
                                data[2] &= 0xFB;
                            }

                            if (v26 != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }

                            var v22 = (byte)(data[2] >> 6);
                            data[2] *= 4;
                            data[2] |= v22;
                            var v25 = (byte)((data[30] >> 3) & 1);
                            if (((data[30] >> 7) & 1) != 0)
                            {
                                data[30] |= 8;
                            }
                            else
                            {
                                data[30] &= 0xF7;
                            }

                            if (v25 != 0)
                            {
                                data[30] |= 0x80;
                            }
                            else
                            {
                                data[30] &= 0x7F;
                            }

                            var v23 = data[3];
                            data[3] = data[20];
                            data[20] = v23;
                            data[2] ^= 0x27;
                            var v24 = data[1];
                            data[1] = data[17];
                            data[17] = v24;
                            data[8] ^= 0x15;
                        }
                        else
                        {
                            var v11 = data[5];
                            data[5] = data[12];
                            data[12] = v11;
                            var v12 = data[13];
                            data[13] = data[1];
                            data[1] = v12;
                            var v13 = data[15];
                            data[15] = data[15];
                            data[15] = v13;
                            var v14 = data[1];
                            data[1] = data[15];
                            data[15] = v14;
                            var v15 = data[0];
                            data[0] = data[5];
                            data[5] = v15;
                            var v16 = (byte)(data[2] >> 5);
                            data[2] *= 8;
                            data[2] |= v16;
                            data[10] ^= 0x94;
                            var v17 = (byte)(data[7] >> 6);
                            data[7] *= 4;
                            data[7] |= v17;
                            var v18 = (byte)(data[5] >> 2);
                            data[5] <<= 6;
                            data[5] |= v18;
                            var v19 = data[14];
                            data[14] = data[2];
                            data[2] = v19;
                        }
                    }
                    else
                    {
                        data[2] ^= 0x73;
                        data[2] ^= 5;
                        data[7] ^= 0xE1;
                        var v29 = (byte)((data[2] >> 2) & 1);
                        if (((data[2] >> 5) & 1) != 0)
                        {
                            data[2] |= 4;
                        }
                        else
                        {
                            data[2] &= 0xFB;
                        }

                        if (v29 != 0)
                        {
                            data[2] |= 0x20;
                        }
                        else
                        {
                            data[2] &= 0xDF;
                        }

                        var v10 = (byte)(data[7] >> 2);
                        data[7] <<= 6;
                        data[7] |= v10;
                        var v28 = (byte)((data[1] >> 2) & 1);
                        if (((data[1] >> 2) & 1) != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        if (v28 != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        var v27 = (byte)((data[3] >> 3) & 1);
                        if (((data[3] >> 6) & 1) != 0)
                        {
                            data[3] |= 8;
                        }
                        else
                        {
                            data[3] &= 0xF7;
                        }

                        if (v27 != 0)
                        {
                            data[3] |= 0x40;
                        }
                        else
                        {
                            data[3] &= 0xBF;
                        }

                        data[0] ^= 0xA7;
                        data[2] ^= 0x77;
                        data[4] ^= 0xAF;
                    }
                }
                else
                {
                    var v3 = (byte)(data[1] >> 1);
                    data[1] <<= 7;
                    data[1] |= v3;
                    var v2 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 3) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    data[1] ^= 0x67;
                    var v4 = (byte)(data[1] >> 2);
                    data[1] <<= 6;
                    data[1] |= v4;
                    var v5 = data[3];
                    data[3] = data[3];
                    data[3] = v5;
                    var v6 = data[2];
                    data[2] = data[0];
                    data[0] = v6;
                    var v7 = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= v7;
                    var v8 = (byte)(data[0] >> 1);
                    data[0] <<= 7;
                    data[0] |= v8;
                    data[3] ^= 0x52;
                    data[2] ^= 0xED;
                    var v9 = (byte)(data[3] >> 3);
                    data[3] *= 32;
                    data[3] |= v9;
                    var v30 = (byte)((data[3] >> 1) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    if (v30 != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }
                }
            }
        }
    }
}
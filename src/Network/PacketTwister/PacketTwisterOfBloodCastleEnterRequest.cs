// <copyright file="PacketTwisterOfBloodCastleEnterRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'BloodCastleEnterRequest' type.
    /// </summary>
    internal class PacketTwisterOfBloodCastleEnterRequest : IPacketTwister
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
                            var v21 = (byte)((data[11] >> 7) & 1);
                            if (((data[11] >> 3) & 1) != 0)
                            {
                                data[11] |= 0x80;
                            }
                            else
                            {
                                data[11] &= 0x7F;
                            }

                            if (v21 != 0)
                            {
                                data[11] |= 8;
                            }
                            else
                            {
                                data[11] &= 0xF7;
                            }

                            var v14 = data[1];
                            data[1] = data[13];
                            data[13] = v14;
                            var v15 = data[3];
                            data[3] = data[5];
                            data[5] = v15;
                            var v16 = data[20];
                            data[20] = data[7];
                            data[7] = v16;
                            var v17 = data[15];
                            data[15] = data[4];
                            data[4] = v17;
                            data[4] ^= 0x6B;
                            var v20 = (byte)((data[0] >> 1) & 1);
                            if (((data[0] >> 3) & 1) != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            if (v20 != 0)
                            {
                                data[0] |= 8;
                            }
                            else
                            {
                                data[0] &= 0xF7;
                            }

                            var v18 = data[1];
                            data[1] = data[28];
                            data[28] = v18;
                            var v19 = data[24];
                            data[24] = data[9];
                            data[9] = v19;
                        }
                        else
                        {
                            var v7 = data[1];
                            data[1] = data[2];
                            data[2] = v7;
                            var v8 = (byte)(data[12] >> 2);
                            data[12] <<= 6;
                            data[12] |= v8;
                            var v9 = data[12];
                            data[12] = data[9];
                            data[9] = v9;
                            var v10 = data[3];
                            data[3] = data[7];
                            data[7] = v10;
                            var v11 = (byte)(data[9] >> 3);
                            data[9] *= 32;
                            data[9] |= v11;
                            var v12 = (byte)(data[12] >> 1);
                            data[12] <<= 7;
                            data[12] |= v12;
                            var v13 = (byte)(data[1] >> 2);
                            data[1] <<= 6;
                            data[1] |= v13;
                        }
                    }
                    else
                    {
                        data[0] ^= 0x4D;
                        var v5 = (byte)(data[4] >> 5);
                        data[4] *= 8;
                        data[4] |= v5;
                        var v24 = (byte)((data[7] >> 2) & 1);
                        if (((data[7] >> 3) & 1) != 0)
                        {
                            data[7] |= 4;
                        }
                        else
                        {
                            data[7] &= 0xFB;
                        }

                        if (v24 != 0)
                        {
                            data[7] |= 8;
                        }
                        else
                        {
                            data[7] &= 0xF7;
                        }

                        var v23 = (byte)((data[4] >> 1) & 1);
                        if (((data[4] >> 1) & 1) != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        if (v23 != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        var v6 = data[5];
                        data[5] = data[3];
                        data[3] = v6;
                        var v22 = (byte)((data[3] >> 1) & 1);
                        if (((data[3] >> 1) & 1) != 0)
                        {
                            data[3] |= 2;
                        }
                        else
                        {
                            data[3] &= 0xFD;
                        }

                        if (v22 != 0)
                        {
                            data[3] |= 2;
                        }
                        else
                        {
                            data[3] &= 0xFD;
                        }

                        data[7] ^= 0xC2;
                        data[7] ^= 0xCB;
                    }
                }
                else
                {
                    var v3 = data[0];
                    data[0] = data[1];
                    data[1] = v3;
                    data[2] ^= 0x46;
                    var v2 = (byte)((data[3] >> 7) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 0x80;
                    }
                    else
                    {
                        data[3] &= 0x7F;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    var v4 = data[1];
                    data[1] = data[1];
                    data[1] = v4;
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
                            var v14 = data[24];
                            data[24] = data[9];
                            data[9] = v14;
                            var v15 = data[1];
                            data[1] = data[28];
                            data[28] = v15;
                            var v21 = (byte)((data[0] >> 1) & 1);
                            if (((data[0] >> 3) & 1) != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            if (v21 != 0)
                            {
                                data[0] |= 8;
                            }
                            else
                            {
                                data[0] &= 0xF7;
                            }

                            data[4] ^= 0x6B;
                            var v16 = data[15];
                            data[15] = data[4];
                            data[4] = v16;
                            var v17 = data[20];
                            data[20] = data[7];
                            data[7] = v17;
                            var v18 = data[3];
                            data[3] = data[5];
                            data[5] = v18;
                            var v19 = data[1];
                            data[1] = data[13];
                            data[13] = v19;
                            var v20 = (byte)((data[11] >> 7) & 1);
                            if (((data[11] >> 3) & 1) != 0)
                            {
                                data[11] |= 0x80;
                            }
                            else
                            {
                                data[11] &= 0x7F;
                            }

                            if (v20 != 0)
                            {
                                data[11] |= 8;
                            }
                            else
                            {
                                data[11] &= 0xF7;
                            }
                        }
                        else
                        {
                            var v7 = (byte)(data[1] >> 6);
                            data[1] *= 4;
                            data[1] |= v7;
                            var v8 = (byte)(data[12] >> 7);
                            data[12] *= 2;
                            data[12] |= v8;
                            var v9 = (byte)(data[9] >> 5);
                            data[9] *= 8;
                            data[9] |= v9;
                            var v10 = data[3];
                            data[3] = data[7];
                            data[7] = v10;
                            var v11 = data[12];
                            data[12] = data[9];
                            data[9] = v11;
                            var v12 = (byte)(data[12] >> 6);
                            data[12] *= 4;
                            data[12] |= v12;
                            var v13 = data[1];
                            data[1] = data[2];
                            data[2] = v13;
                        }
                    }
                    else
                    {
                        data[7] ^= 0xCB;
                        data[7] ^= 0xC2;
                        var v24 = (byte)((data[3] >> 1) & 1);
                        if (((data[3] >> 1) & 1) != 0)
                        {
                            data[3] |= 2;
                        }
                        else
                        {
                            data[3] &= 0xFD;
                        }

                        if (v24 != 0)
                        {
                            data[3] |= 2;
                        }
                        else
                        {
                            data[3] &= 0xFD;
                        }

                        var v5 = data[5];
                        data[5] = data[3];
                        data[3] = v5;
                        var v23 = (byte)((data[4] >> 1) & 1);
                        if (((data[4] >> 1) & 1) != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        if (v23 != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        var v22 = (byte)((data[7] >> 2) & 1);
                        if (((data[7] >> 3) & 1) != 0)
                        {
                            data[7] |= 4;
                        }
                        else
                        {
                            data[7] &= 0xFB;
                        }

                        if (v22 != 0)
                        {
                            data[7] |= 8;
                        }
                        else
                        {
                            data[7] &= 0xF7;
                        }

                        var v6 = (byte)(data[4] >> 3);
                        data[4] *= 32;
                        data[4] |= v6;
                        data[0] ^= 0x4D;
                    }
                }
                else
                {
                    var v3 = data[1];
                    data[1] = data[1];
                    data[1] = v3;
                    var v2 = (byte)((data[3] >> 7) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 0x80;
                    }
                    else
                    {
                        data[3] &= 0x7F;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    data[2] ^= 0x46;
                    var v4 = data[0];
                    data[0] = data[1];
                    data[1] = v4;
                }
            }
        }
    }
}
// <copyright file="PacketTwisterOfBloodCastleInfoRequest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'BloodCastleInfoRequest' type.
    /// </summary>
    internal class PacketTwisterOfBloodCastleInfoRequest : IPacketTwister
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
                            var v22 = data[7];
                            data[7] = data[4];
                            data[4] = v22;
                            var v24 = (byte)((data[7] >> 3) & 1);
                            if (((data[7] >> 1) & 1) != 0)
                            {
                                data[7] |= 8;
                            }
                            else
                            {
                                data[7] &= 0xF7;
                            }

                            if (v24 != 0)
                            {
                                data[7] |= 2;
                            }
                            else
                            {
                                data[7] &= 0xFD;
                            }

                            data[12] ^= 0x56;
                            var v23 = (byte)((data[13] >> 5) & 1);
                            if (((data[13] >> 2) & 1) != 0)
                            {
                                data[13] |= 0x20;
                            }
                            else
                            {
                                data[13] &= 0xDF;
                            }

                            if (v23 != 0)
                            {
                                data[13] |= 4;
                            }
                            else
                            {
                                data[13] &= 0xFB;
                            }
                        }
                        else
                        {
                            var v16 = (byte)(data[5] >> 3);
                            data[5] *= 32;
                            data[5] |= v16;
                            var v17 = (byte)(data[11] >> 6);
                            data[11] *= 4;
                            data[11] |= v17;
                            data[3] ^= 0xF4;
                            var v27 = (byte)((data[1] >> 4) & 1);
                            if (((data[1] >> 5) & 1) != 0)
                            {
                                data[1] |= 0x10;
                            }
                            else
                            {
                                data[1] &= 0xEF;
                            }

                            if (v27 != 0)
                            {
                                data[1] |= 0x20;
                            }
                            else
                            {
                                data[1] &= 0xDF;
                            }

                            var v26 = (byte)((data[9] >> 1) & 1);
                            if (((data[9] >> 4) & 1) != 0)
                            {
                                data[9] |= 2;
                            }
                            else
                            {
                                data[9] &= 0xFD;
                            }

                            if (v26 != 0)
                            {
                                data[9] |= 0x10;
                            }
                            else
                            {
                                data[9] &= 0xEF;
                            }

                            var v18 = data[3];
                            data[3] = data[7];
                            data[7] = v18;
                            var v19 = data[3];
                            data[3] = data[0];
                            data[0] = v19;
                            var v20 = (byte)(data[14] >> 7);
                            data[14] *= 2;
                            data[14] |= v20;
                            var v25 = (byte)((data[2] >> 3) & 1);
                            if (((data[2] >> 7) & 1) != 0)
                            {
                                data[2] |= 8;
                            }
                            else
                            {
                                data[2] &= 0xF7;
                            }

                            if (v25 != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }

                            var v21 = (byte)(data[1] >> 3);
                            data[1] *= 32;
                            data[1] |= v21;
                        }
                    }
                    else
                    {
                        var v13 = (byte)(data[2] >> 2);
                        data[2] <<= 6;
                        data[2] |= v13;
                        data[2] ^= 0xFE;
                        var v14 = (byte)(data[5] >> 6);
                        data[5] *= 4;
                        data[5] |= v14;
                        var v15 = (byte)(data[3] >> 6);
                        data[3] *= 4;
                        data[3] |= v15;
                    }
                }
                else
                {
                    data[3] ^= 0x96;
                    var v3 = (byte)(data[0] >> 5);
                    data[0] *= 8;
                    data[0] |= v3;
                    var v4 = data[1];
                    data[1] = data[2];
                    data[2] = v4;
                    var v5 = (byte)(data[1] >> 4);
                    data[1] *= 16;
                    data[1] |= v5;
                    var v6 = data[0];
                    data[0] = data[2];
                    data[2] = v6;
                    var v7 = (byte)(data[3] >> 7);
                    data[3] *= 2;
                    data[3] |= v7;
                    var v2 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 5) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    var v8 = data[1];
                    data[1] = data[3];
                    data[3] = v8;
                    var v9 = data[2];
                    data[2] = data[1];
                    data[1] = v9;
                    var v10 = data[2];
                    data[2] = data[2];
                    data[2] = v10;
                    var v28 = (byte)((data[1] >> 6) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    if (v28 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    var v11 = data[3];
                    data[3] = data[0];
                    data[0] = v11;
                    var v12 = (byte)(data[3] >> 5);
                    data[3] *= 8;
                    data[3] |= v12;
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
                            var v24 = (byte)((data[13] >> 5) & 1);
                            if (((data[13] >> 2) & 1) != 0)
                            {
                                data[13] |= 0x20;
                            }
                            else
                            {
                                data[13] &= 0xDF;
                            }

                            if (v24 != 0)
                            {
                                data[13] |= 4;
                            }
                            else
                            {
                                data[13] &= 0xFB;
                            }

                            data[12] ^= 0x56;
                            var v23 = (byte)((data[7] >> 3) & 1);
                            if (((data[7] >> 1) & 1) != 0)
                            {
                                data[7] |= 8;
                            }
                            else
                            {
                                data[7] &= 0xF7;
                            }

                            if (v23 != 0)
                            {
                                data[7] |= 2;
                            }
                            else
                            {
                                data[7] &= 0xFD;
                            }

                            var v22 = data[7];
                            data[7] = data[4];
                            data[4] = v22;
                        }
                        else
                        {
                            var v16 = (byte)(data[1] >> 5);
                            data[1] *= 8;
                            data[1] |= v16;
                            var v27 = (byte)((data[2] >> 3) & 1);
                            if (((data[2] >> 7) & 1) != 0)
                            {
                                data[2] |= 8;
                            }
                            else
                            {
                                data[2] &= 0xF7;
                            }

                            if (v27 != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }

                            var v17 = (byte)(data[14] >> 1);
                            data[14] <<= 7;
                            data[14] |= v17;
                            var v18 = data[3];
                            data[3] = data[0];
                            data[0] = v18;
                            var v19 = data[3];
                            data[3] = data[7];
                            data[7] = v19;
                            var v26 = (byte)((data[9] >> 1) & 1);
                            if (((data[9] >> 4) & 1) != 0)
                            {
                                data[9] |= 2;
                            }
                            else
                            {
                                data[9] &= 0xFD;
                            }

                            if (v26 != 0)
                            {
                                data[9] |= 0x10;
                            }
                            else
                            {
                                data[9] &= 0xEF;
                            }

                            var v25 = (byte)((data[1] >> 4) & 1);
                            if (((data[1] >> 5) & 1) != 0)
                            {
                                data[1] |= 0x10;
                            }
                            else
                            {
                                data[1] &= 0xEF;
                            }

                            if (v25 != 0)
                            {
                                data[1] |= 0x20;
                            }
                            else
                            {
                                data[1] &= 0xDF;
                            }

                            data[3] ^= 0xF4;
                            var v20 = (byte)(data[11] >> 2);
                            data[11] <<= 6;
                            data[11] |= v20;
                            var v21 = (byte)(data[5] >> 5);
                            data[5] *= 8;
                            data[5] |= v21;
                        }
                    }
                    else
                    {
                        var v13 = (byte)(data[3] >> 2);
                        data[3] <<= 6;
                        data[3] |= v13;
                        var v14 = (byte)(data[5] >> 2);
                        data[5] <<= 6;
                        data[5] |= v14;
                        data[2] ^= 0xFE;
                        var v15 = (byte)(data[2] >> 6);
                        data[2] *= 4;
                        data[2] |= v15;
                    }
                }
                else
                {
                    var v3 = (byte)(data[3] >> 3);
                    data[3] *= 32;
                    data[3] |= v3;
                    var v4 = data[3];
                    data[3] = data[0];
                    data[0] = v4;
                    var v2 = (byte)((data[1] >> 6) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 0x40;
                    }
                    else
                    {
                        data[1] &= 0xBF;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    var v5 = data[2];
                    data[2] = data[2];
                    data[2] = v5;
                    var v6 = data[2];
                    data[2] = data[1];
                    data[1] = v6;
                    var v7 = data[1];
                    data[1] = data[3];
                    data[3] = v7;
                    var v28 = (byte)((data[2] >> 2) & 1);
                    if (((data[2] >> 5) & 1) != 0)
                    {
                        data[2] |= 4;
                    }
                    else
                    {
                        data[2] &= 0xFB;
                    }

                    if (v28 != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    var v8 = (byte)(data[3] >> 1);
                    data[3] <<= 7;
                    data[3] |= v8;
                    var v9 = data[0];
                    data[0] = data[2];
                    data[2] = v9;
                    var v10 = (byte)(data[1] >> 4);
                    data[1] *= 16;
                    data[1] |= v10;
                    var v11 = data[1];
                    data[1] = data[2];
                    data[2] = v11;
                    var v12 = (byte)(data[0] >> 3);
                    data[0] *= 32;
                    data[0] |= v12;
                    data[3] ^= 0x96;
                }
            }
        }
    }
}
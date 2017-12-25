// <copyright file="PacketTwister59.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of an unknown type.
    /// </summary>
    internal class PacketTwister59 : IPacketTwister
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
                            var v15 = (byte)(data[20] >> 1);
                            data[20] <<= 7;
                            data[20] |= v15;
                            var v16 = (byte)(data[14] >> 6);
                            data[14] *= 4;
                            data[14] |= v16;
                            data[7] ^= 0x87;
                            var v17 = (byte)(data[17] >> 1);
                            data[17] <<= 7;
                            data[17] |= v17;
                            var v18 = (byte)(data[17] >> 6);
                            data[17] *= 4;
                            data[17] |= v18;
                        }
                        else
                        {
                            var v20 = (byte)((data[4] >> 7) & 1);
                            if (((data[4] >> 1) & 1) != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            if (v20 != 0)
                            {
                                data[4] |= 2;
                            }
                            else
                            {
                                data[4] &= 0xFD;
                            }

                            data[7] ^= 0xB6;
                            var v12 = data[12];
                            data[12] = data[6];
                            data[6] = v12;
                            var v13 = (byte)(data[1] >> 6);
                            data[1] *= 4;
                            data[1] |= v13;
                            var v14 = data[9];
                            data[9] = data[7];
                            data[7] = v14;
                            var v19 = (byte)((data[9] >> 7) & 1);
                            if (((data[9] >> 1) & 1) != 0)
                            {
                                data[9] |= 0x80;
                            }
                            else
                            {
                                data[9] &= 0x7F;
                            }

                            if (v19 != 0)
                            {
                                data[9] |= 2;
                            }
                            else
                            {
                                data[9] &= 0xFD;
                            }

                            data[4] ^= 0x5B;
                        }
                    }
                    else
                    {
                        data[1] ^= 0x9D;
                        data[7] ^= 0xC2;
                        var v6 = (byte)(data[3] >> 1);
                        data[3] <<= 7;
                        data[3] |= v6;
                        var v23 = (byte)((data[6] >> 2) & 1);
                        if (((data[6] >> 5) & 1) != 0)
                        {
                            data[6] |= 4;
                        }
                        else
                        {
                            data[6] &= 0xFB;
                        }

                        if (v23 != 0)
                        {
                            data[6] |= 0x20;
                        }
                        else
                        {
                            data[6] &= 0xDF;
                        }

                        var v22 = (byte)((data[5] >> 5) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        if (v22 != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        var v7 = data[0];
                        data[0] = data[2];
                        data[2] = v7;
                        var v8 = data[7];
                        data[7] = data[2];
                        data[2] = v8;
                        var v9 = (byte)(data[0] >> 2);
                        data[0] <<= 6;
                        data[0] |= v9;
                        var v10 = data[5];
                        data[5] = data[6];
                        data[6] = v10;
                        var v11 = data[5];
                        data[5] = data[6];
                        data[6] = v11;
                        var v21 = (byte)((data[4] >> 1) & 1);
                        if (((data[4] >> 4) & 1) != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        if (v21 != 0)
                        {
                            data[4] |= 0x10;
                        }
                        else
                        {
                            data[4] &= 0xEF;
                        }
                    }
                }
                else
                {
                    data[3] ^= 0x4F;
                    var v3 = data[2];
                    data[2] = data[0];
                    data[0] = v3;
                    var v2 = (byte)((data[0] >> 1) & 1);
                    if (((data[0] >> 3) & 1) != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    data[3] ^= 0x2E;
                    var v4 = data[3];
                    data[3] = data[2];
                    data[2] = v4;
                    var v24 = (byte)((data[0] >> 3) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    if (v24 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v5 = data[3];
                    data[3] = data[0];
                    data[0] = v5;
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
                            var v15 = (byte)(data[17] >> 2);
                            data[17] <<= 6;
                            data[17] |= v15;
                            var v16 = (byte)(data[17] >> 7);
                            data[17] *= 2;
                            data[17] |= v16;
                            data[7] ^= 0x87;
                            var v17 = (byte)(data[14] >> 2);
                            data[14] <<= 6;
                            data[14] |= v17;
                            var v18 = (byte)(data[20] >> 7);
                            data[20] *= 2;
                            data[20] |= v18;
                        }
                        else
                        {
                            data[4] ^= 0x5B;
                            var v20 = (byte)((data[9] >> 7) & 1);
                            if (((data[9] >> 1) & 1) != 0)
                            {
                                data[9] |= 0x80;
                            }
                            else
                            {
                                data[9] &= 0x7F;
                            }

                            if (v20 != 0)
                            {
                                data[9] |= 2;
                            }
                            else
                            {
                                data[9] &= 0xFD;
                            }

                            var v12 = data[9];
                            data[9] = data[7];
                            data[7] = v12;
                            var v13 = (byte)(data[1] >> 2);
                            data[1] <<= 6;
                            data[1] |= v13;
                            var v14 = data[12];
                            data[12] = data[6];
                            data[6] = v14;
                            data[7] ^= 0xB6;
                            var v19 = (byte)((data[4] >> 7) & 1);
                            if (((data[4] >> 1) & 1) != 0)
                            {
                                data[4] |= 0x80;
                            }
                            else
                            {
                                data[4] &= 0x7F;
                            }

                            if (v19 != 0)
                            {
                                data[4] |= 2;
                            }
                            else
                            {
                                data[4] &= 0xFD;
                            }
                        }
                    }
                    else
                    {
                        var v23 = (byte)((data[4] >> 1) & 1);
                        if (((data[4] >> 4) & 1) != 0)
                        {
                            data[4] |= 2;
                        }
                        else
                        {
                            data[4] &= 0xFD;
                        }

                        if (v23 != 0)
                        {
                            data[4] |= 0x10;
                        }
                        else
                        {
                            data[4] &= 0xEF;
                        }

                        var v6 = data[5];
                        data[5] = data[6];
                        data[6] = v6;
                        var v7 = data[5];
                        data[5] = data[6];
                        data[6] = v7;
                        var v8 = (byte)(data[0] >> 6);
                        data[0] *= 4;
                        data[0] |= v8;
                        var v9 = data[7];
                        data[7] = data[2];
                        data[2] = v9;
                        var v10 = data[0];
                        data[0] = data[2];
                        data[2] = v10;
                        var v22 = (byte)((data[5] >> 5) & 1);
                        if (((data[5] >> 1) & 1) != 0)
                        {
                            data[5] |= 0x20;
                        }
                        else
                        {
                            data[5] &= 0xDF;
                        }

                        if (v22 != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        var v21 = (byte)((data[6] >> 2) & 1);
                        if (((data[6] >> 5) & 1) != 0)
                        {
                            data[6] |= 4;
                        }
                        else
                        {
                            data[6] &= 0xFB;
                        }

                        if (v21 != 0)
                        {
                            data[6] |= 0x20;
                        }
                        else
                        {
                            data[6] &= 0xDF;
                        }

                        var v11 = (byte)(data[3] >> 7);
                        data[3] *= 2;
                        data[3] |= v11;
                        data[7] ^= 0xC2;
                        data[1] ^= 0x9D;
                    }
                }
                else
                {
                    var v3 = data[3];
                    data[3] = data[0];
                    data[0] = v3;
                    var v2 = (byte)((data[0] >> 3) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v4 = data[3];
                    data[3] = data[2];
                    data[2] = v4;
                    data[3] ^= 0x2E;
                    var v24 = (byte)((data[0] >> 1) & 1);
                    if (((data[0] >> 3) & 1) != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    if (v24 != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    var v5 = data[2];
                    data[2] = data[0];
                    data[0] = v5;
                    data[3] ^= 0x4F;
                }
            }
        }
    }
}
// <copyright file="PacketTwister58.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of an unknown type.
    /// </summary>
    internal class PacketTwister58 : IPacketTwister
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
                            var v17 = data[13];
                            data[13] = data[31];
                            data[31] = v17;
                            data[31] ^= 0xCD;
                            var v18 = data[25];
                            data[25] = data[27];
                            data[27] = v18;
                        }
                        else
                        {
                            data[12] ^= 0x9A;
                            data[7] ^= 0xC1;
                            data[8] ^= 0xF9;
                            data[0] ^= 0x9D;
                            var v15 = data[4];
                            data[4] = data[3];
                            data[3] = v15;
                            data[6] ^= 0xC3;
                            var v16 = data[11];
                            data[11] = data[11];
                            data[11] = v16;
                        }
                    }
                    else
                    {
                        var v21 = (byte)((data[7] >> 1) & 1);
                        if (((data[7] >> 7) & 1) != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        if (v21 != 0)
                        {
                            data[7] |= 0x80;
                        }
                        else
                        {
                            data[7] &= 0x7F;
                        }

                        var v20 = (byte)((data[3] >> 7) & 1);
                        if (((data[3] >> 1) & 1) != 0)
                        {
                            data[3] |= 0x80;
                        }
                        else
                        {
                            data[3] &= 0x7F;
                        }

                        if (v20 != 0)
                        {
                            data[3] |= 2;
                        }
                        else
                        {
                            data[3] &= 0xFD;
                        }

                        data[3] ^= 0xA1;
                        data[0] ^= 0x5A;
                        var v11 = (byte)(data[6] >> 5);
                        data[6] *= 8;
                        data[6] |= v11;
                        var v19 = (byte)((data[3] >> 5) & 1);
                        if (((data[3] >> 5) & 1) != 0)
                        {
                            data[3] |= 0x20;
                        }
                        else
                        {
                            data[3] &= 0xDF;
                        }

                        if (v19 != 0)
                        {
                            data[3] |= 0x20;
                        }
                        else
                        {
                            data[3] &= 0xDF;
                        }

                        var v12 = (byte)(data[7] >> 1);
                        data[7] <<= 7;
                        data[7] |= v12;
                        var v13 = (byte)(data[5] >> 7);
                        data[5] *= 2;
                        data[5] |= v13;
                        data[7] ^= 0x53;
                        var v14 = data[3];
                        data[3] = data[4];
                        data[4] = v14;
                    }
                }
                else
                {
                    data[3] ^= 0x87;
                    var v3 = data[3];
                    data[3] = data[0];
                    data[0] = v3;
                    var v4 = (byte)(data[1] >> 4);
                    data[1] *= 16;
                    data[1] |= v4;
                    var v5 = data[1];
                    data[1] = data[0];
                    data[0] = v5;
                    data[2] ^= 0x44;
                    var v6 = (byte)(data[3] >> 6);
                    data[3] *= 4;
                    data[3] |= v6;
                    var v7 = data[2];
                    data[2] = data[0];
                    data[0] = v7;
                    var v8 = data[2];
                    data[2] = data[0];
                    data[0] = v8;
                    var v9 = data[3];
                    data[3] = data[2];
                    data[2] = v9;
                    var v10 = data[0];
                    data[0] = data[3];
                    data[3] = v10;
                    data[2] ^= 0x50;
                    var v2 = (byte)((data[1] >> 1) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    var v22 = (byte)((data[1] >> 3) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    if (v22 != 0)
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
                            var v17 = data[25];
                            data[25] = data[27];
                            data[27] = v17;
                            data[31] ^= 0xCD;
                            var v18 = data[13];
                            data[13] = data[31];
                            data[31] = v18;
                        }
                        else
                        {
                            var v15 = data[11];
                            data[11] = data[11];
                            data[11] = v15;
                            data[6] ^= 0xC3;
                            var v16 = data[4];
                            data[4] = data[3];
                            data[3] = v16;
                            data[0] ^= 0x9D;
                            data[8] ^= 0xF9;
                            data[7] ^= 0xC1;
                            data[12] ^= 0x9A;
                        }
                    }
                    else
                    {
                        var v11 = data[3];
                        data[3] = data[4];
                        data[4] = v11;
                        data[7] ^= 0x53;
                        var v12 = (byte)(data[5] >> 1);
                        data[5] <<= 7;
                        data[5] |= v12;
                        var v13 = (byte)(data[7] >> 7);
                        data[7] *= 2;
                        data[7] |= v13;
                        var v21 = (byte)((data[3] >> 5) & 1);
                        if (((data[3] >> 5) & 1) != 0)
                        {
                            data[3] |= 0x20;
                        }
                        else
                        {
                            data[3] &= 0xDF;
                        }

                        if (v21 != 0)
                        {
                            data[3] |= 0x20;
                        }
                        else
                        {
                            data[3] &= 0xDF;
                        }

                        var v14 = (byte)(data[6] >> 3);
                        data[6] *= 32;
                        data[6] |= v14;
                        data[0] ^= 0x5A;
                        data[3] ^= 0xA1;
                        var v20 = (byte)((data[3] >> 7) & 1);
                        if (((data[3] >> 1) & 1) != 0)
                        {
                            data[3] |= 0x80;
                        }
                        else
                        {
                            data[3] &= 0x7F;
                        }

                        if (v20 != 0)
                        {
                            data[3] |= 2;
                        }
                        else
                        {
                            data[3] &= 0xFD;
                        }

                        var v19 = (byte)((data[7] >> 1) & 1);
                        if (((data[7] >> 7) & 1) != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        if (v19 != 0)
                        {
                            data[7] |= 0x80;
                        }
                        else
                        {
                            data[7] &= 0x7F;
                        }
                    }
                }
                else
                {
                    var v2 = (byte)((data[1] >> 3) & 1);
                    if (((data[1] >> 2) & 1) != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    var v22 = (byte)((data[1] >> 1) & 1);
                    if (((data[1] >> 1) & 1) != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    if (v22 != 0)
                    {
                        data[1] |= 2;
                    }
                    else
                    {
                        data[1] &= 0xFD;
                    }

                    data[2] ^= 0x50;
                    var v3 = data[0];
                    data[0] = data[3];
                    data[3] = v3;
                    var v4 = data[3];
                    data[3] = data[2];
                    data[2] = v4;
                    var v5 = data[2];
                    data[2] = data[0];
                    data[0] = v5;
                    var v6 = data[2];
                    data[2] = data[0];
                    data[0] = v6;
                    var v7 = (byte)(data[3] >> 2);
                    data[3] <<= 6;
                    data[3] |= v7;
                    data[2] ^= 0x44;
                    var v8 = data[1];
                    data[1] = data[0];
                    data[0] = v8;
                    var v9 = (byte)(data[1] >> 4);
                    data[1] *= 16;
                    data[1] |= v9;
                    var v10 = data[3];
                    data[3] = data[0];
                    data[0] = v10;
                    data[3] ^= 0x87;
                }
            }
        }
    }
}
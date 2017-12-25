// <copyright file="PacketTwisterOfPetItemCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'PetItemCommand' type.
    /// </summary>
    internal class PacketTwisterOfPetItemCommand : IPacketTwister
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
                            data[3] ^= 0x9D;
                            data[14] ^= 0x36;
                            var v25 = (byte)((data[15] >> 3) & 1);
                            if (((data[15] >> 1) & 1) != 0)
                            {
                                data[15] |= 8;
                            }
                            else
                            {
                                data[15] &= 0xF7;
                            }

                            if (v25 != 0)
                            {
                                data[15] |= 2;
                            }
                            else
                            {
                                data[15] &= 0xFD;
                            }

                            var v24 = (byte)((data[25] >> 5) & 1);
                            if (((data[25] >> 1) & 1) != 0)
                            {
                                data[25] |= 0x20;
                            }
                            else
                            {
                                data[25] &= 0xDF;
                            }

                            if (v24 != 0)
                            {
                                data[25] |= 2;
                            }
                            else
                            {
                                data[25] &= 0xFD;
                            }
                        }
                        else
                        {
                            var v19 = (byte)(data[12] >> 6);
                            data[12] *= 4;
                            data[12] |= v19;
                            var v29 = (byte)((data[8] >> 5) & 1);
                            if (((data[8] >> 6) & 1) != 0)
                            {
                                data[8] |= 0x20;
                            }
                            else
                            {
                                data[8] &= 0xDF;
                            }

                            if (v29 != 0)
                            {
                                data[8] |= 0x40;
                            }
                            else
                            {
                                data[8] &= 0xBF;
                            }

                            var v20 = (byte)(data[2] >> 7);
                            data[2] *= 2;
                            data[2] |= v20;
                            var v21 = (byte)(data[14] >> 6);
                            data[14] *= 4;
                            data[14] |= v21;
                            var v28 = (byte)((data[2] >> 7) & 1);
                            if (((data[2] >> 6) & 1) != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }

                            if (v28 != 0)
                            {
                                data[2] |= 0x40;
                            }
                            else
                            {
                                data[2] &= 0xBF;
                            }

                            var v22 = data[4];
                            data[4] = data[1];
                            data[1] = v22;
                            data[15] ^= 0xE4;
                            var v27 = (byte)((data[0] >> 1) & 1);
                            if (((data[0] >> 5) & 1) != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            if (v27 != 0)
                            {
                                data[0] |= 0x20;
                            }
                            else
                            {
                                data[0] &= 0xDF;
                            }

                            var v23 = (byte)(data[12] >> 5);
                            data[12] *= 8;
                            data[12] |= v23;
                            var v26 = (byte)((data[11] >> 1) & 1);
                            if (((data[11] >> 2) & 1) != 0)
                            {
                                data[11] |= 2;
                            }
                            else
                            {
                                data[11] &= 0xFD;
                            }

                            if (v26 != 0)
                            {
                                data[11] |= 4;
                            }
                            else
                            {
                                data[11] &= 0xFB;
                            }
                        }
                    }
                    else
                    {
                        var v11 = data[3];
                        data[3] = data[3];
                        data[3] = v11;
                        var v12 = data[2];
                        data[2] = data[0];
                        data[0] = v12;
                        var v31 = (byte)((data[0] >> 6) & 1);
                        if (((data[0] >> 4) & 1) != 0)
                        {
                            data[0] |= 0x40;
                        }
                        else
                        {
                            data[0] &= 0xBF;
                        }

                        if (v31 != 0)
                        {
                            data[0] |= 0x10;
                        }
                        else
                        {
                            data[0] &= 0xEF;
                        }

                        var v13 = data[4];
                        data[4] = data[5];
                        data[5] = v13;
                        var v14 = (byte)(data[1] >> 6);
                        data[1] *= 4;
                        data[1] |= v14;
                        data[0] ^= 0x7D;
                        data[6] ^= 0xAF;
                        var v15 = data[3];
                        data[3] = data[2];
                        data[2] = v15;
                        var v16 = (byte)(data[2] >> 7);
                        data[2] *= 2;
                        data[2] |= v16;
                        var v17 = (byte)(data[1] >> 4);
                        data[1] *= 16;
                        data[1] |= v17;
                        var v18 = (byte)(data[1] >> 6);
                        data[1] *= 4;
                        data[1] |= v18;
                        var v30 = (byte)((data[5] >> 1) & 1);
                        if (((data[5] >> 2) & 1) != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        if (v30 != 0)
                        {
                            data[5] |= 4;
                        }
                        else
                        {
                            data[5] &= 0xFB;
                        }
                    }
                }
                else
                {
                    var v3 = (byte)(data[3] >> 4);
                    data[3] *= 16;
                    data[3] |= v3;
                    var v4 = data[3];
                    data[3] = data[2];
                    data[2] = v4;
                    var v2 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v5 = (byte)(data[1] >> 7);
                    data[1] *= 2;
                    data[1] |= v5;
                    var v6 = data[0];
                    data[0] = data[3];
                    data[3] = v6;
                    var v7 = (byte)(data[0] >> 6);
                    data[0] *= 4;
                    data[0] |= v7;
                    var v8 = (byte)(data[1] >> 7);
                    data[1] *= 2;
                    data[1] |= v8;
                    var v9 = data[2];
                    data[2] = data[2];
                    data[2] = v9;
                    data[2] ^= 0xD2;
                    var v10 = (byte)(data[1] >> 3);
                    data[1] *= 32;
                    data[1] |= v10;
                    var v32 = (byte)((data[3] >> 4) & 1);
                    if (((data[3] >> 3) & 1) != 0)
                    {
                        data[3] |= 0x10;
                    }
                    else
                    {
                        data[3] &= 0xEF;
                    }

                    if (v32 != 0)
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
                            var v25 = (byte)((data[25] >> 5) & 1);
                            if (((data[25] >> 1) & 1) != 0)
                            {
                                data[25] |= 0x20;
                            }
                            else
                            {
                                data[25] &= 0xDF;
                            }

                            if (v25 != 0)
                            {
                                data[25] |= 2;
                            }
                            else
                            {
                                data[25] &= 0xFD;
                            }

                            var v24 = (byte)((data[15] >> 3) & 1);
                            if (((data[15] >> 1) & 1) != 0)
                            {
                                data[15] |= 8;
                            }
                            else
                            {
                                data[15] &= 0xF7;
                            }

                            if (v24 != 0)
                            {
                                data[15] |= 2;
                            }
                            else
                            {
                                data[15] &= 0xFD;
                            }

                            data[14] ^= 0x36;
                            data[3] ^= 0x9D;
                        }
                        else
                        {
                            var v29 = (byte)((data[11] >> 1) & 1);
                            if (((data[11] >> 2) & 1) != 0)
                            {
                                data[11] |= 2;
                            }
                            else
                            {
                                data[11] &= 0xFD;
                            }

                            if (v29 != 0)
                            {
                                data[11] |= 4;
                            }
                            else
                            {
                                data[11] &= 0xFB;
                            }

                            var v19 = (byte)(data[12] >> 3);
                            data[12] *= 32;
                            data[12] |= v19;
                            var v28 = (byte)((data[0] >> 1) & 1);
                            if (((data[0] >> 5) & 1) != 0)
                            {
                                data[0] |= 2;
                            }
                            else
                            {
                                data[0] &= 0xFD;
                            }

                            if (v28 != 0)
                            {
                                data[0] |= 0x20;
                            }
                            else
                            {
                                data[0] &= 0xDF;
                            }

                            data[15] ^= 0xE4;
                            var v20 = data[4];
                            data[4] = data[1];
                            data[1] = v20;
                            var v27 = (byte)((data[2] >> 7) & 1);
                            if (((data[2] >> 6) & 1) != 0)
                            {
                                data[2] |= 0x80;
                            }
                            else
                            {
                                data[2] &= 0x7F;
                            }

                            if (v27 != 0)
                            {
                                data[2] |= 0x40;
                            }
                            else
                            {
                                data[2] &= 0xBF;
                            }

                            var v21 = (byte)(data[14] >> 2);
                            data[14] <<= 6;
                            data[14] |= v21;
                            var v22 = (byte)(data[2] >> 1);
                            data[2] <<= 7;
                            data[2] |= v22;
                            var v26 = (byte)((data[8] >> 5) & 1);
                            if (((data[8] >> 6) & 1) != 0)
                            {
                                data[8] |= 0x20;
                            }
                            else
                            {
                                data[8] &= 0xDF;
                            }

                            if (v26 != 0)
                            {
                                data[8] |= 0x40;
                            }
                            else
                            {
                                data[8] &= 0xBF;
                            }

                            var v23 = (byte)(data[12] >> 2);
                            data[12] <<= 6;
                            data[12] |= v23;
                        }
                    }
                    else
                    {
                        var v31 = (byte)((data[5] >> 1) & 1);
                        if (((data[5] >> 2) & 1) != 0)
                        {
                            data[5] |= 2;
                        }
                        else
                        {
                            data[5] &= 0xFD;
                        }

                        if (v31 != 0)
                        {
                            data[5] |= 4;
                        }
                        else
                        {
                            data[5] &= 0xFB;
                        }

                        var v11 = (byte)(data[1] >> 2);
                        data[1] <<= 6;
                        data[1] |= v11;
                        var v12 = (byte)(data[1] >> 4);
                        data[1] *= 16;
                        data[1] |= v12;
                        var v13 = (byte)(data[2] >> 1);
                        data[2] <<= 7;
                        data[2] |= v13;
                        var v14 = data[3];
                        data[3] = data[2];
                        data[2] = v14;
                        data[6] ^= 0xAF;
                        data[0] ^= 0x7D;
                        var v15 = (byte)(data[1] >> 2);
                        data[1] <<= 6;
                        data[1] |= v15;
                        var v16 = data[4];
                        data[4] = data[5];
                        data[5] = v16;
                        var v30 = (byte)((data[0] >> 6) & 1);
                        if (((data[0] >> 4) & 1) != 0)
                        {
                            data[0] |= 0x40;
                        }
                        else
                        {
                            data[0] &= 0xBF;
                        }

                        if (v30 != 0)
                        {
                            data[0] |= 0x10;
                        }
                        else
                        {
                            data[0] &= 0xEF;
                        }

                        var v17 = data[2];
                        data[2] = data[0];
                        data[0] = v17;
                        var v18 = data[3];
                        data[3] = data[3];
                        data[3] = v18;
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

                    var v3 = (byte)(data[1] >> 5);
                    data[1] *= 8;
                    data[1] |= v3;
                    data[2] ^= 0xD2;
                    var v4 = data[2];
                    data[2] = data[2];
                    data[2] = v4;
                    var v5 = (byte)(data[1] >> 1);
                    data[1] <<= 7;
                    data[1] |= v5;
                    var v6 = (byte)(data[0] >> 2);
                    data[0] <<= 6;
                    data[0] |= v6;
                    var v7 = data[0];
                    data[0] = data[3];
                    data[3] = v7;
                    var v8 = (byte)(data[1] >> 1);
                    data[1] <<= 7;
                    data[1] |= v8;
                    var v32 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 1) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v32 != 0)
                    {
                        data[0] |= 2;
                    }
                    else
                    {
                        data[0] &= 0xFD;
                    }

                    var v9 = data[3];
                    data[3] = data[2];
                    data[2] = v9;
                    var v10 = (byte)(data[3] >> 4);
                    data[3] *= 16;
                    data[3] |= v10;
                }
            }
        }
    }
}
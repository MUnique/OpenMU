// <copyright file="PacketTwisterOfVaultPassword.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'VaultPassword' type.
    /// </summary>
    internal class PacketTwisterOfVaultPassword : IPacketTwister
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
                            data[20] ^= 0x92;
                            data[16] ^= 0x82;
                            data[2] ^= 0xF0;
                            var v17 = (byte)(data[29] >> 3);
                            data[29] *= 32;
                            data[29] |= v17;
                            var v18 = data[29];
                            data[29] = data[13];
                            data[13] = v18;
                            var v19 = (byte)(data[29] >> 3);
                            data[29] *= 32;
                            data[29] |= v19;
                        }
                        else
                        {
                            var v11 = data[6];
                            data[6] = data[14];
                            data[14] = v11;
                            var v12 = (byte)(data[10] >> 2);
                            data[10] <<= 6;
                            data[10] |= v12;
                            var v22 = (byte)((data[5] >> 7) & 1);
                            if (((data[5] >> 1) & 1) != 0)
                            {
                                data[5] |= 0x80;
                            }
                            else
                            {
                                data[5] &= 0x7F;
                            }

                            if (v22 != 0)
                            {
                                data[5] |= 2;
                            }
                            else
                            {
                                data[5] &= 0xFD;
                            }

                            var v21 = (byte)((data[14] >> 4) & 1);
                            if (((data[14] >> 7) & 1) != 0)
                            {
                                data[14] |= 0x10;
                            }
                            else
                            {
                                data[14] &= 0xEF;
                            }

                            if (v21 != 0)
                            {
                                data[14] |= 0x80;
                            }
                            else
                            {
                                data[14] &= 0x7F;
                            }

                            var v20 = (byte)((data[6] >> 1) & 1);
                            if (((data[6] >> 1) & 1) != 0)
                            {
                                data[6] |= 2;
                            }
                            else
                            {
                                data[6] &= 0xFD;
                            }

                            if (v20 != 0)
                            {
                                data[6] |= 2;
                            }
                            else
                            {
                                data[6] &= 0xFD;
                            }

                            data[14] ^= 0xA9;
                            var v13 = (byte)(data[8] >> 2);
                            data[8] <<= 6;
                            data[8] |= v13;
                            data[15] ^= 0xA2;
                            var v14 = data[4];
                            data[4] = data[2];
                            data[2] = v14;
                            data[11] ^= 0x61;
                            var v15 = data[8];
                            data[8] = data[2];
                            data[2] = v15;
                            var v16 = data[8];
                            data[8] = data[11];
                            data[11] = v16;
                            data[15] ^= 0x3C;
                        }
                    }
                    else
                    {
                        var v7 = data[1];
                        data[1] = data[6];
                        data[6] = v7;
                        var v8 = data[3];
                        data[3] = data[6];
                        data[6] = v8;
                        var v24 = (byte)((data[7] >> 2) & 1);
                        if (((data[7] >> 7) & 1) != 0)
                        {
                            data[7] |= 4;
                        }
                        else
                        {
                            data[7] &= 0xFB;
                        }

                        if (v24 != 0)
                        {
                            data[7] |= 0x80;
                        }
                        else
                        {
                            data[7] &= 0x7F;
                        }

                        data[7] ^= 0x73;
                        var v9 = (byte)(data[0] >> 7);
                        data[0] *= 2;
                        data[0] |= v9;
                        var v10 = data[0];
                        data[0] = data[5];
                        data[5] = v10;
                        var v23 = (byte)((data[0] >> 4) & 1);
                        if (((data[0] >> 3) & 1) != 0)
                        {
                            data[0] |= 0x10;
                        }
                        else
                        {
                            data[0] &= 0xEF;
                        }

                        if (v23 != 0)
                        {
                            data[0] |= 8;
                        }
                        else
                        {
                            data[0] &= 0xF7;
                        }
                    }
                }
                else
                {
                    data[2] ^= 0x54;
                    var v3 = data[1];
                    data[1] = data[2];
                    data[2] = v3;
                    var v4 = (byte)(data[1] >> 6);
                    data[1] *= 4;
                    data[1] |= v4;
                    var v2 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 2) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    var v26 = (byte)((data[2] >> 1) & 1);
                    if (((data[2] >> 5) & 1) != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    if (v26 != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    var v25 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 4) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v25 != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    var v5 = (byte)(data[2] >> 5);
                    data[2] *= 8;
                    data[2] |= v5;
                    var v6 = data[2];
                    data[2] = data[3];
                    data[3] = v6;
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
                            var v17 = (byte)(data[29] >> 5);
                            data[29] *= 8;
                            data[29] |= v17;
                            var v18 = data[29];
                            data[29] = data[13];
                            data[13] = v18;
                            var v19 = (byte)(data[29] >> 5);
                            data[29] *= 8;
                            data[29] |= v19;
                            data[2] ^= 0xF0;
                            data[16] ^= 0x82;
                            data[20] ^= 0x92;
                        }
                        else
                        {
                            data[15] ^= 0x3C;
                            var v11 = data[8];
                            data[8] = data[11];
                            data[11] = v11;
                            var v12 = data[8];
                            data[8] = data[2];
                            data[2] = v12;
                            data[11] ^= 0x61;
                            var v13 = data[4];
                            data[4] = data[2];
                            data[2] = v13;
                            data[15] ^= 0xA2;
                            var v14 = (byte)(data[8] >> 6);
                            data[8] *= 4;
                            data[8] |= v14;
                            data[14] ^= 0xA9;
                            var v22 = (byte)((data[6] >> 1) & 1);
                            if (((data[6] >> 1) & 1) != 0)
                            {
                                data[6] |= 2;
                            }
                            else
                            {
                                data[6] &= 0xFD;
                            }

                            if (v22 != 0)
                            {
                                data[6] |= 2;
                            }
                            else
                            {
                                data[6] &= 0xFD;
                            }

                            var v21 = (byte)((data[14] >> 4) & 1);
                            if (((data[14] >> 7) & 1) != 0)
                            {
                                data[14] |= 0x10;
                            }
                            else
                            {
                                data[14] &= 0xEF;
                            }

                            if (v21 != 0)
                            {
                                data[14] |= 0x80;
                            }
                            else
                            {
                                data[14] &= 0x7F;
                            }

                            var v20 = (byte)((data[5] >> 7) & 1);
                            if (((data[5] >> 1) & 1) != 0)
                            {
                                data[5] |= 0x80;
                            }
                            else
                            {
                                data[5] &= 0x7F;
                            }

                            if (v20 != 0)
                            {
                                data[5] |= 2;
                            }
                            else
                            {
                                data[5] &= 0xFD;
                            }

                            var v15 = (byte)(data[10] >> 6);
                            data[10] *= 4;
                            data[10] |= v15;
                            var v16 = data[6];
                            data[6] = data[14];
                            data[14] = v16;
                        }
                    }
                    else
                    {
                        var v24 = (byte)((data[0] >> 4) & 1);
                        if (((data[0] >> 3) & 1) != 0)
                        {
                            data[0] |= 0x10;
                        }
                        else
                        {
                            data[0] &= 0xEF;
                        }

                        if (v24 != 0)
                        {
                            data[0] |= 8;
                        }
                        else
                        {
                            data[0] &= 0xF7;
                        }

                        var v7 = data[0];
                        data[0] = data[5];
                        data[5] = v7;
                        var v8 = (byte)(data[0] >> 1);
                        data[0] <<= 7;
                        data[0] |= v8;
                        data[7] ^= 0x73;
                        var v23 = (byte)((data[7] >> 2) & 1);
                        if (((data[7] >> 7) & 1) != 0)
                        {
                            data[7] |= 4;
                        }
                        else
                        {
                            data[7] &= 0xFB;
                        }

                        if (v23 != 0)
                        {
                            data[7] |= 0x80;
                        }
                        else
                        {
                            data[7] &= 0x7F;
                        }

                        var v9 = data[3];
                        data[3] = data[6];
                        data[6] = v9;
                        var v10 = data[1];
                        data[1] = data[6];
                        data[6] = v10;
                    }
                }
                else
                {
                    var v3 = data[2];
                    data[2] = data[3];
                    data[3] = v3;
                    var v4 = (byte)(data[2] >> 3);
                    data[2] *= 32;
                    data[2] |= v4;
                    var v2 = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 4) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    var v26 = (byte)((data[2] >> 1) & 1);
                    if (((data[2] >> 5) & 1) != 0)
                    {
                        data[2] |= 2;
                    }
                    else
                    {
                        data[2] &= 0xFD;
                    }

                    if (v26 != 0)
                    {
                        data[2] |= 0x20;
                    }
                    else
                    {
                        data[2] &= 0xDF;
                    }

                    var v25 = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 2) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (v25 != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    var v5 = (byte)(data[1] >> 2);
                    data[1] <<= 6;
                    data[1] |= v5;
                    var v6 = data[1];
                    data[1] = data[2];
                    data[2] = v6;
                    data[2] ^= 0x54;
                }
            }
        }
    }
}
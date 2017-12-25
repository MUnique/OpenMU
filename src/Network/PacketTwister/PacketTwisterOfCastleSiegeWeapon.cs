// <copyright file="PacketTwisterOfCastleSiegeWeapon.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'CastleSiegeWeapon' type.
    /// </summary>
    internal class PacketTwisterOfCastleSiegeWeapon : IPacketTwister
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
                            var v13 = data[13];
                            data[13] = data[11];
                            data[11] = v13;
                            var v14 = data[18];
                            data[18] = data[11];
                            data[11] = v14;
                            var v15 = (byte)(data[16] >> 5);
                            data[16] *= 8;
                            data[16] |= v15;
                            data[28] ^= 0x11;
                            var v16 = (byte)(data[6] >> 4);
                            data[6] *= 16;
                            data[6] |= v16;
                            data[5] ^= 0xE7;
                            data[4] ^= 0x55;
                            var v17 = (byte)(data[15] >> 6);
                            data[15] *= 4;
                            data[15] |= v17;
                        }
                        else
                        {
                            var v18 = (byte)((data[0] >> 3) & 1);
                            if (((data[0] >> 3) & 1) != 0)
                            {
                                data[0] |= 8;
                            }
                            else
                            {
                                data[0] &= 0xF7;
                            }

                            if (v18 != 0)
                            {
                                data[0] |= 8;
                            }
                            else
                            {
                                data[0] &= 0xF7;
                            }

                            var v8 = (byte)(data[5] >> 1);
                            data[5] <<= 7;
                            data[5] |= v8;
                            data[1] ^= 0x4B;
                            data[2] ^= 0x81;
                            var v9 = data[4];
                            data[4] = data[7];
                            data[7] = v9;
                            var v10 = (byte)(data[4] >> 7);
                            data[4] *= 2;
                            data[4] |= v10;
                            var v11 = (byte)(data[7] >> 6);
                            data[7] *= 4;
                            data[7] |= v11;
                            var v12 = (byte)(data[13] >> 3);
                            data[13] *= 32;
                            data[13] |= v12;
                            data[11] ^= 0xAC;
                        }
                    }
                    else
                    {
                        var v6 = (byte)(data[4] >> 5);
                        data[4] *= 8;
                        data[4] |= v6;
                        var v7 = data[0];
                        data[0] = data[6];
                        data[6] = v7;
                        data[0] ^= 0xEA;
                        data[6] ^= 2;
                    }
                }
                else
                {
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

                    data[1] ^= 0xEB;
                    var v3 = data[3];
                    data[3] = data[1];
                    data[1] = v3;
                    var v4 = (byte)(data[3] >> 1);
                    data[3] <<= 7;
                    data[3] |= v4;
                    var v19 = (byte)((data[3] >> 6) & 1);
                    if (((data[3] >> 6) & 1) != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }

                    if (v19 != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }

                    data[3] ^= 0xF6;
                    var v5 = (byte)(data[0] >> 4);
                    data[0] *= 16;
                    data[0] |= v5;
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
                            var v13 = (byte)(data[15] >> 2);
                            data[15] <<= 6;
                            data[15] |= v13;
                            data[4] ^= 0x55;
                            data[5] ^= 0xE7;
                            var v14 = (byte)(data[6] >> 4);
                            data[6] *= 16;
                            data[6] |= v14;
                            data[28] ^= 0x11;
                            var v15 = (byte)(data[16] >> 3);
                            data[16] *= 32;
                            data[16] |= v15;
                            var v16 = data[18];
                            data[18] = data[11];
                            data[11] = v16;
                            var v17 = data[13];
                            data[13] = data[11];
                            data[11] = v17;
                        }
                        else
                        {
                            data[11] ^= 0xAC;
                            var v8 = (byte)(data[13] >> 5);
                            data[13] *= 8;
                            data[13] |= v8;
                            var v9 = (byte)(data[7] >> 2);
                            data[7] <<= 6;
                            data[7] |= v9;
                            var v10 = (byte)(data[4] >> 1);
                            data[4] <<= 7;
                            data[4] |= v10;
                            var v11 = data[4];
                            data[4] = data[7];
                            data[7] = v11;
                            data[2] ^= 0x81;
                            data[1] ^= 0x4B;
                            var v12 = (byte)(data[5] >> 7);
                            data[5] *= 2;
                            data[5] |= v12;
                            var v18 = (byte)((data[0] >> 3) & 1);
                            if (((data[0] >> 3) & 1) != 0)
                            {
                                data[0] |= 8;
                            }
                            else
                            {
                                data[0] &= 0xF7;
                            }

                            if (v18 != 0)
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
                        data[6] ^= 2;
                        data[0] ^= 0xEA;
                        var v6 = data[0];
                        data[0] = data[6];
                        data[6] = v6;
                        var v7 = (byte)(data[4] >> 3);
                        data[4] *= 32;
                        data[4] |= v7;
                    }
                }
                else
                {
                    var v3 = (byte)(data[0] >> 4);
                    data[0] *= 16;
                    data[0] |= v3;
                    data[3] ^= 0xF6;
                    var v2 = (byte)((data[3] >> 6) & 1);
                    if (((data[3] >> 6) & 1) != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }

                    if (v2 != 0)
                    {
                        data[3] |= 0x40;
                    }
                    else
                    {
                        data[3] &= 0xBF;
                    }

                    var v4 = (byte)(data[3] >> 7);
                    data[3] *= 2;
                    data[3] |= v4;
                    var v5 = data[3];
                    data[3] = data[1];
                    data[1] = v5;
                    data[1] ^= 0xEB;
                    var v19 = (byte)((data[3] >> 7) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 0x80;
                    }
                    else
                    {
                        data[3] &= 0x7F;
                    }

                    if (v19 != 0)
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
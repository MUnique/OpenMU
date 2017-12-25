// <copyright file="PacketTwisterOfVaultMoney.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'VaultMoney' type.
    /// </summary>
    internal class PacketTwisterOfVaultMoney : IPacketTwister
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
                            var v16 = (byte)((data[6] >> 7) & 1);
                            if (((data[6] >> 4) & 1) != 0)
                            {
                                data[6] |= 0x80;
                            }
                            else
                            {
                                data[6] &= 0x7F;
                            }

                            if (v16 != 0)
                            {
                                data[6] |= 0x10;
                            }
                            else
                            {
                                data[6] &= 0xEF;
                            }

                            var v12 = (byte)(data[26] >> 2);
                            data[26] <<= 6;
                            data[26] |= v12;
                            var v13 = (byte)(data[1] >> 3);
                            data[1] *= 32;
                            data[1] |= v13;
                            var v14 = (byte)(data[9] >> 3);
                            data[9] *= 32;
                            data[9] |= v14;
                            data[13] ^= 0x26;
                            data[10] ^= 0xC8;
                            data[10] ^= 0xD7;
                            var v15 = (byte)(data[27] >> 6);
                            data[27] *= 4;
                            data[27] |= v15;
                        }
                        else
                        {
                            var v7 = (byte)(data[5] >> 3);
                            data[5] *= 32;
                            data[5] |= v7;
                            data[14] ^= 0x24;
                            var v8 = (byte)(data[2] >> 2);
                            data[2] <<= 6;
                            data[2] |= v8;
                            data[1] ^= 0x55;
                            var v9 = (byte)(data[7] >> 6);
                            data[7] *= 4;
                            data[7] |= v9;
                            var v10 = (byte)(data[13] >> 6);
                            data[13] *= 4;
                            data[13] |= v10;
                            var v11 = data[3];
                            data[3] = data[1];
                            data[1] = v11;
                        }
                    }
                    else
                    {
                        var v18 = (byte)((data[0] >> 1) & 1);
                        if (((data[0] >> 6) & 1) != 0)
                        {
                            data[0] |= 2;
                        }
                        else
                        {
                            data[0] &= 0xFD;
                        }

                        if (v18 != 0)
                        {
                            data[0] |= 0x40;
                        }
                        else
                        {
                            data[0] &= 0xBF;
                        }

                        data[2] ^= 0xE4;
                        var v17 = (byte)((data[2] >> 5) & 1);
                        if (((data[2] >> 6) & 1) != 0)
                        {
                            data[2] |= 0x20;
                        }
                        else
                        {
                            data[2] &= 0xDF;
                        }

                        if (v17 != 0)
                        {
                            data[2] |= 0x40;
                        }
                        else
                        {
                            data[2] &= 0xBF;
                        }

                        var v6 = (byte)(data[4] >> 6);
                        data[4] *= 4;
                        data[4] |= v6;
                    }
                }
                else
                {
                    data[0] ^= 0x3C;
                    var v3 = (byte)(data[1] >> 4);
                    data[1] *= 16;
                    data[1] |= v3;
                    data[0] ^= 0xEE;
                    var v4 = (byte)(data[3] >> 3);
                    data[3] *= 32;
                    data[3] |= v4;
                    var v5 = data[1];
                    data[1] = data[2];
                    data[2] = v5;
                    var v2 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 7) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 0x80;
                    }
                    else
                    {
                        data[0] &= 0x7F;
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
                            var v12 = (byte)(data[27] >> 2);
                            data[27] <<= 6;
                            data[27] |= v12;
                            data[10] ^= 0xD7;
                            data[10] ^= 0xC8;
                            data[13] ^= 0x26;
                            var v13 = (byte)(data[9] >> 5);
                            data[9] *= 8;
                            data[9] |= v13;
                            var v14 = (byte)(data[1] >> 5);
                            data[1] *= 8;
                            data[1] |= v14;
                            var v15 = (byte)(data[26] >> 6);
                            data[26] *= 4;
                            data[26] |= v15;
                            var v16 = (byte)((data[6] >> 7) & 1);
                            if (((data[6] >> 4) & 1) != 0)
                            {
                                data[6] |= 0x80;
                            }
                            else
                            {
                                data[6] &= 0x7F;
                            }

                            if (v16 != 0)
                            {
                                data[6] |= 0x10;
                            }
                            else
                            {
                                data[6] &= 0xEF;
                            }
                        }
                        else
                        {
                            var v7 = data[3];
                            data[3] = data[1];
                            data[1] = v7;
                            var v8 = (byte)(data[13] >> 2);
                            data[13] <<= 6;
                            data[13] |= v8;
                            var v9 = (byte)(data[7] >> 2);
                            data[7] <<= 6;
                            data[7] |= v9;
                            data[1] ^= 0x55;
                            var v10 = (byte)(data[2] >> 6);
                            data[2] *= 4;
                            data[2] |= v10;
                            data[14] ^= 0x24;
                            var v11 = (byte)(data[5] >> 5);
                            data[5] *= 8;
                            data[5] |= v11;
                        }
                    }
                    else
                    {
                        var v6 = (byte)(data[4] >> 2);
                        data[4] <<= 6;
                        data[4] |= v6;
                        var v18 = (byte)((data[2] >> 5) & 1);
                        if (((data[2] >> 6) & 1) != 0)
                        {
                            data[2] |= 0x20;
                        }
                        else
                        {
                            data[2] &= 0xDF;
                        }

                        if (v18 != 0)
                        {
                            data[2] |= 0x40;
                        }
                        else
                        {
                            data[2] &= 0xBF;
                        }

                        data[2] ^= 0xE4;
                        var v17 = (byte)((data[0] >> 1) & 1);
                        if (((data[0] >> 6) & 1) != 0)
                        {
                            data[0] |= 2;
                        }
                        else
                        {
                            data[0] &= 0xFD;
                        }

                        if (v17 != 0)
                        {
                            data[0] |= 0x40;
                        }
                        else
                        {
                            data[0] &= 0xBF;
                        }
                    }
                }
                else
                {
                    var v2 = (byte)((data[0] >> 2) & 1);
                    if (((data[0] >> 7) & 1) != 0)
                    {
                        data[0] |= 4;
                    }
                    else
                    {
                        data[0] &= 0xFB;
                    }

                    if (v2 != 0)
                    {
                        data[0] |= 0x80;
                    }
                    else
                    {
                        data[0] &= 0x7F;
                    }

                    var v3 = data[1];
                    data[1] = data[2];
                    data[2] = v3;
                    var v4 = (byte)(data[3] >> 5);
                    data[3] *= 8;
                    data[3] |= v4;
                    data[0] ^= 0xEE;
                    var v5 = (byte)(data[1] >> 4);
                    data[1] *= 16;
                    data[1] |= v5;
                    data[0] ^= 0x3C;
                }
            }
        }
    }
}
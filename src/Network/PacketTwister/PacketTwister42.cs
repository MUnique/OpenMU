// <copyright file="PacketTwister42.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of an unknown type.
    /// </summary>
    internal class PacketTwister42 : IPacketTwister
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
                            var v8 = (byte)(data[8] >> 7);
                            data[8] *= 2;
                            data[8] |= v8;
                            var v9 = (byte)(data[10] >> 2);
                            data[10] <<= 6;
                            data[10] |= v9;
                            data[28] ^= 0xAE;
                        }
                        else
                        {
                            var v2 = (byte)((data[14] >> 2) & 1);
                            if (((data[14] >> 1) & 1) != 0)
                            {
                                data[14] |= 4;
                            }
                            else
                            {
                                data[14] &= 0xFB;
                            }

                            if (v2 != 0)
                            {
                                data[14] |= 2;
                            }
                            else
                            {
                                data[14] &= 0xFD;
                            }

                            data[4] ^= 0x40;
                            var v10 = (byte)((data[3] >> 2) & 1);
                            if (((data[3] >> 3) & 1) != 0)
                            {
                                data[3] |= 4;
                            }
                            else
                            {
                                data[3] &= 0xFB;
                            }

                            if (v10 != 0)
                            {
                                data[3] |= 8;
                            }
                            else
                            {
                                data[3] &= 0xF7;
                            }

                            data[0] ^= 0xC7;
                            var v7 = (byte)(data[4] >> 4);
                            data[4] *= 16;
                            data[4] |= v7;
                        }
                    }
                    else
                    {
                        var v4 = data[4];
                        data[4] = data[1];
                        data[1] = v4;
                        data[1] ^= 0x64;
                        var v5 = data[4];
                        data[4] = data[1];
                        data[1] = v5;
                        var v6 = (byte)(data[1] >> 5);
                        data[1] *= 8;
                        data[1] |= v6;
                    }
                }
                else
                {
                    data[1] ^= 0x3C;
                    data[2] ^= 0x96;
                    var v3 = data[1];
                    data[1] = data[2];
                    data[2] = v3;
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
                            data[28] ^= 0xAE;
                            var v8 = (byte)(data[10] >> 6);
                            data[10] *= 4;
                            data[10] |= v8;
                            var v9 = (byte)(data[8] >> 1);
                            data[8] <<= 7;
                            data[8] |= v9;
                        }
                        else
                        {
                            var v7 = (byte)(data[4] >> 4);
                            data[4] *= 16;
                            data[4] |= v7;
                            data[0] ^= 0xC7;
                            var v2 = (byte)((data[3] >> 2) & 1);
                            if (((data[3] >> 3) & 1) != 0)
                            {
                                data[3] |= 4;
                            }
                            else
                            {
                                data[3] &= 0xFB;
                            }

                            if (v2 != 0)
                            {
                                data[3] |= 8;
                            }
                            else
                            {
                                data[3] &= 0xF7;
                            }

                            data[4] ^= 0x40;
                            var v10 = (byte)((data[14] >> 2) & 1);
                            if (((data[14] >> 1) & 1) != 0)
                            {
                                data[14] |= 4;
                            }
                            else
                            {
                                data[14] &= 0xFB;
                            }

                            if (v10 != 0)
                            {
                                data[14] |= 2;
                            }
                            else
                            {
                                data[14] &= 0xFD;
                            }
                        }
                    }
                    else
                    {
                        var v4 = (byte)(data[1] >> 3);
                        data[1] *= 32;
                        data[1] |= v4;
                        var v5 = data[4];
                        data[4] = data[1];
                        data[1] = v5;
                        data[1] ^= 0x64;
                        var v6 = data[4];
                        data[4] = data[1];
                        data[1] = v6;
                    }
                }
                else
                {
                    var v3 = data[1];
                    data[1] = data[2];
                    data[2] = v3;
                    data[2] ^= 0x96;
                    data[1] ^= 0x3C;
                }
            }
        }
    }
}
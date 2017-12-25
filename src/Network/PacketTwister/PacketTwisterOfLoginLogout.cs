// <copyright file="PacketTwisterOfLoginLogout.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'LoginLogout' type.
    /// </summary>
    internal class PacketTwisterOfLoginLogout : IPacketTwister
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
                            var temp = data[26];
                            data[26] = data[1];
                            data[1] = temp;
                            temp = data[11];
                            data[11] = data[11];
                            data[11] = temp;
                            temp = data[22];
                            data[22] = data[6];
                            data[6] = temp;
                            temp = data[9];
                            data[9] = data[24];
                            data[24] = temp;
                        }
                        else
                        {
                            var temp = (byte)((data[8] >> 2) & 1);
                            if (((data[8] >> 7) & 1) != 0)
                            {
                                data[8] |= 4;
                            }
                            else
                            {
                                data[8] &= 0xFB;
                            }

                            if (temp != 0)
                            {
                                data[8] |= 0x80;
                            }
                            else
                            {
                                data[8] &= 0x7F;
                            }

                            temp = (byte)(data[2] >> 6);
                            data[2] *= 4;
                            data[2] |= temp;
                            temp = (byte)(data[10] >> 4);
                            data[10] *= 16;
                            data[10] |= temp;
                            temp = (byte)(data[13] >> 6);
                            data[13] *= 4;
                            data[13] |= temp;
                            data[8] ^= 0x37;
                            temp = (byte)((data[8] >> 6) & 1);
                            if (((data[8] >> 4) & 1) != 0)
                            {
                                data[8] |= 0x40;
                            }
                            else
                            {
                                data[8] &= 0xBF;
                            }

                            if (temp != 0)
                            {
                                data[8] |= 0x10;
                            }
                            else
                            {
                                data[8] &= 0xEF;
                            }

                            temp = (byte)(data[7] >> 5);
                            data[7] *= 8;
                            data[7] |= temp;
                            data[4] ^= 0x79;
                            temp = data[15];
                            data[15] = data[3];
                            data[3] = temp;
                            data[14] ^= 0x77;
                            temp = (byte)((data[15] >> 2) & 1);
                            if (((data[15] >> 4) & 1) != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            if (temp != 0)
                            {
                                data[15] |= 0x10;
                            }
                            else
                            {
                                data[15] &= 0xEF;
                            }
                        }
                    }
                    else
                    {
                        var temp = (byte)(data[0] >> 6);
                        data[0] *= 4;
                        data[0] |= temp;
                        temp = data[7];
                        data[7] = data[0];
                        data[0] = temp;
                        temp = data[4];
                        data[4] = data[5];
                        data[5] = temp;
                        temp = (byte)(data[3] >> 6);
                        data[3] *= 4;
                        data[3] |= temp;
                        data[5] ^= 0xD2;
                        temp = (byte)(data[6] >> 2);
                        data[6] <<= 6;
                        data[6] |= temp;
                        data[2] ^= 0xC4;
                        data[5] ^= 0xC4;
                        temp = (byte)(data[7] >> 6);
                        data[7] *= 4;
                        data[7] |= temp;
                        temp = (byte)(data[6] >> 1);
                        data[6] <<= 7;
                        data[6] |= temp;
                        data[0] ^= 0xDA;
                        temp = data[5];
                        data[5] = data[1];
                        data[1] = temp;
                        data[2] ^= 0x75;
                    }
                }
                else
                {
                    data[1] ^= 0x51;
                    var temp = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (temp != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    temp = (byte)(data[0] >> 1);
                    data[0] <<= 7;
                    data[0] |= temp;
                    data[1] ^= 0xC1;
                    temp = (byte)((data[2] >> 4) & 1);
                    if (((data[2] >> 4) & 1) != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    if (temp != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
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
                            var temp = data[9];
                            data[9] = data[24];
                            data[24] = temp;
                            temp = data[22];
                            data[22] = data[6];
                            data[6] = temp;
                            temp = data[11];
                            data[11] = data[11];
                            data[11] = temp;
                            temp = data[26];
                            data[26] = data[1];
                            data[1] = temp;
                        }
                        else
                        {
                            var temp = (byte)((data[15] >> 2) & 1);
                            if (((data[15] >> 4) & 1) != 0)
                            {
                                data[15] |= 4;
                            }
                            else
                            {
                                data[15] &= 0xFB;
                            }

                            if (temp != 0)
                            {
                                data[15] |= 0x10;
                            }
                            else
                            {
                                data[15] &= 0xEF;
                            }

                            data[14] ^= 0x77;
                            temp = data[15];
                            data[15] = data[3];
                            data[3] = temp;
                            data[4] ^= 0x79;
                            temp = (byte)(data[7] >> 3);
                            data[7] *= 32;
                            data[7] |= temp;
                            temp = (byte)((data[8] >> 6) & 1);
                            if (((data[8] >> 4) & 1) != 0)
                            {
                                data[8] |= 0x40;
                            }
                            else
                            {
                                data[8] &= 0xBF;
                            }

                            if (temp != 0)
                            {
                                data[8] |= 0x10;
                            }
                            else
                            {
                                data[8] &= 0xEF;
                            }

                            data[8] ^= 0x37;
                            temp = (byte)(data[13] >> 2);
                            data[13] <<= 6;
                            data[13] |= temp;
                            temp = (byte)(data[10] >> 4);
                            data[10] *= 16;
                            data[10] |= temp;
                            temp = (byte)(data[2] >> 2);
                            data[2] <<= 6;
                            data[2] |= temp;
                            temp = (byte)((data[8] >> 2) & 1);
                            if (((data[8] >> 7) & 1) != 0)
                            {
                                data[8] |= 4;
                            }
                            else
                            {
                                data[8] &= 0xFB;
                            }

                            if (temp != 0)
                            {
                                data[8] |= 0x80;
                            }
                            else
                            {
                                data[8] &= 0x7F;
                            }
                        }
                    }
                    else
                    {
                        data[2] ^= 0x75;
                        var temp = data[5];
                        data[5] = data[1];
                        data[1] = temp;
                        data[0] ^= 0xDA;
                        temp = (byte)(data[6] >> 7);
                        data[6] *= 2;
                        data[6] |= temp;
                        temp = (byte)(data[7] >> 2);
                        data[7] <<= 6;
                        data[7] |= temp;
                        data[5] ^= 0xC4;
                        data[2] ^= 0xC4;
                        temp = (byte)(data[6] >> 6);
                        data[6] *= 4;
                        data[6] |= temp;
                        data[5] ^= 0xD2;
                        temp = (byte)(data[3] >> 2);
                        data[3] <<= 6;
                        data[3] |= temp;
                        temp = data[4];
                        data[4] = data[5];
                        data[5] = temp;
                        temp = data[7];
                        data[7] = data[0];
                        data[0] = temp;
                        temp = (byte)(data[0] >> 2);
                        data[0] <<= 6;
                        data[0] |= temp;
                    }
                }
                else
                {
                    var temp = (byte)((data[2] >> 4) & 1);
                    if (((data[2] >> 4) & 1) != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    if (temp != 0)
                    {
                        data[2] |= 0x10;
                    }
                    else
                    {
                        data[2] &= 0xEF;
                    }

                    data[1] ^= 0xC1;
                    temp = (byte)(data[0] >> 7);
                    data[0] *= 2;
                    data[0] |= temp;
                    temp = (byte)((data[3] >> 2) & 1);
                    if (((data[3] >> 1) & 1) != 0)
                    {
                        data[3] |= 4;
                    }
                    else
                    {
                        data[3] &= 0xFB;
                    }

                    if (temp != 0)
                    {
                        data[3] |= 2;
                    }
                    else
                    {
                        data[3] &= 0xFD;
                    }

                    data[1] ^= 0x51;
                }
            }
        }
    }
}
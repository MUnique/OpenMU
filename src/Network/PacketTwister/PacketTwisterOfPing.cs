// <copyright file="PacketTwisterOfPing.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// PacketTwister implementation for packets of 'Ping' type.
    /// </summary>
    internal class PacketTwisterOfPing : IPacketTwister
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
                            var temp = data[15];
                            data[15] = data[8];
                            data[8] = temp;
                            temp = (byte)(data[28] >> 1);
                            data[28] <<= 7;
                            data[28] |= temp;
                            data[7] ^= 0xCB;
                            temp = (byte)(data[17] >> 5);
                            data[17] *= 8;
                            data[17] |= temp;
                            temp = data[31];
                            data[31] = data[4];
                            data[4] = temp;
                            data[7] ^= 0x7C;
                            data[13] ^= 0x53;
                            temp = (byte)((data[30] >> 3) & 1);
                            if (((data[30] >> 2) & 1) != 0)
                            {
                                data[30] |= 8;
                            }
                            else
                            {
                                data[30] &= 0xF7;
                            }

                            if (temp != 0)
                            {
                                data[30] |= 4;
                            }
                            else
                            {
                                data[30] &= 0xFB;
                            }

                            temp = (byte)(data[2] >> 3);
                            data[2] *= 32;
                            data[2] |= temp;
                            temp = (byte)((data[11] >> 3) & 1);
                            if (((data[11] >> 1) & 1) != 0)
                            {
                                data[11] |= 8;
                            }
                            else
                            {
                                data[11] &= 0xF7;
                            }

                            if (temp != 0)
                            {
                                data[11] |= 2;
                            }
                            else
                            {
                                data[11] &= 0xFD;
                            }

                            data[28] ^= 0x75;
                        }
                        else
                        {
                            var temp = (byte)((data[5] >> 7) & 1);
                            if (((data[5] >> 7) & 1) != 0)
                            {
                                data[5] |= 0x80;
                            }
                            else
                            {
                                data[5] &= 0x7F;
                            }

                            if (temp != 0)
                            {
                                data[5] |= 0x80;
                            }
                            else
                            {
                                data[5] &= 0x7F;
                            }

                            temp = data[10];
                            data[10] = data[7];
                            data[7] = temp;
                            data[0] ^= 0x6C;
                            temp = data[9];
                            data[9] = data[5];
                            data[5] = temp;
                            temp = (byte)(data[3] >> 4);
                            data[3] *= 16;
                            data[3] |= temp;
                            temp = data[8];
                            data[8] = data[9];
                            data[9] = temp;
                            data[2] ^= 0x26;
                            temp = (byte)((data[9] >> 5) & 1);
                            if (((data[9] >> 1) & 1) != 0)
                            {
                                data[9] |= 0x20;
                            }
                            else
                            {
                                data[9] &= 0xDF;
                            }

                            if (temp != 0)
                            {
                                data[9] |= 2;
                            }
                            else
                            {
                                data[9] &= 0xFD;
                            }
                        }
                    }
                    else
                    {
                        var temp = (byte)((data[7] >> 5) & 1);
                        if (((data[7] >> 1) & 1) != 0)
                        {
                            data[7] |= 0x20;
                        }
                        else
                        {
                            data[7] &= 0xDF;
                        }

                        if (temp != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }

                        temp = (byte)(data[7] >> 6);
                        data[7] *= 4;
                        data[7] |= temp;
                        temp = (byte)(data[0] >> 6);
                        data[0] *= 4;
                        data[0] |= temp;
                        data[1] ^= 0x1A;
                        temp = (byte)((data[2] >> 6) & 1);
                        if (((data[2] >> 3) & 1) != 0)
                        {
                            data[2] |= 0x40;
                        }
                        else
                        {
                            data[2] &= 0xBF;
                        }

                        if (temp != 0)
                        {
                            data[2] |= 8;
                        }
                        else
                        {
                            data[2] &= 0xF7;
                        }

                        temp = data[2];
                        data[2] = data[5];
                        data[5] = temp;
                        temp = (byte)(data[3] >> 2);
                        data[3] <<= 6;
                        data[3] |= temp;
                        temp = data[6];
                        data[6] = data[3];
                        data[3] = temp;
                        temp = (byte)((data[1] >> 2) & 1);
                        if (((data[1] >> 4) & 1) != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        if (temp != 0)
                        {
                            data[1] |= 0x10;
                        }
                        else
                        {
                            data[1] &= 0xEF;
                        }

                        data[1] ^= 0x91;
                        temp = (byte)((data[1] >> 4) & 1);
                        if (((data[1] >> 1) & 1) != 0)
                        {
                            data[1] |= 0x10;
                        }
                        else
                        {
                            data[1] &= 0xEF;
                        }

                        if (temp != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }
                    }
                }
                else
                {
                    var temp = (byte)((data[2] >> 7) & 1);
                    if (((data[2] >> 6) & 1) != 0)
                    {
                        data[2] |= 0x80;
                    }
                    else
                    {
                        data[2] &= 0x7F;
                    }

                    if (temp != 0)
                    {
                        data[2] |= 0x40;
                    }
                    else
                    {
                        data[2] &= 0xBF;
                    }

                    temp = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 7) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (temp != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    data[0] ^= 0xB1;
                    temp = data[0];
                    data[0] = data[3];
                    data[3] = temp;
                    temp = (byte)((data[0] >> 3) & 1);
                    if (((data[0] >> 7) & 1) != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    if (temp != 0)
                    {
                        data[0] |= 0x80;
                    }
                    else
                    {
                        data[0] &= 0x7F;
                    }

                    temp = (byte)(data[2] >> 2);
                    data[2] <<= 6;
                    data[2] |= temp;
                    data[0] ^= 0x9C;
                    temp = (byte)(data[1] >> 3);
                    data[1] *= 32;
                    data[1] |= temp;
                    temp = (byte)((data[1] >> 3) & 1);
                    if (((data[1] >> 4) & 1) != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    if (temp != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
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
                            data[28] ^= 0x75;
                            var temp = (byte)((data[11] >> 3) & 1);
                            if (((data[11] >> 1) & 1) != 0)
                            {
                                data[11] |= 8;
                            }
                            else
                            {
                                data[11] &= 0xF7;
                            }

                            if (temp != 0)
                            {
                                data[11] |= 2;
                            }
                            else
                            {
                                data[11] &= 0xFD;
                            }

                            temp = (byte)(data[2] >> 5);
                            data[2] *= 8;
                            data[2] |= temp;
                            temp = (byte)((data[30] >> 3) & 1);
                            if (((data[30] >> 2) & 1) != 0)
                            {
                                data[30] |= 8;
                            }
                            else
                            {
                                data[30] &= 0xF7;
                            }

                            if (temp != 0)
                            {
                                data[30] |= 4;
                            }
                            else
                            {
                                data[30] &= 0xFB;
                            }

                            data[13] ^= 0x53;
                            data[7] ^= 0x7C;
                            temp = data[31];
                            data[31] = data[4];
                            data[4] = temp;
                            temp = (byte)(data[17] >> 3);
                            data[17] *= 32;
                            data[17] |= temp;
                            data[7] ^= 0xCB;
                            temp = (byte)(data[28] >> 7);
                            data[28] *= 2;
                            data[28] |= temp;
                            temp = data[15];
                            data[15] = data[8];
                            data[8] = temp;
                        }
                        else
                        {
                            var temp = (byte)((data[9] >> 5) & 1);
                            if (((data[9] >> 1) & 1) != 0)
                            {
                                data[9] |= 0x20;
                            }
                            else
                            {
                                data[9] &= 0xDF;
                            }

                            if (temp != 0)
                            {
                                data[9] |= 2;
                            }
                            else
                            {
                                data[9] &= 0xFD;
                            }

                            data[2] ^= 0x26;
                            temp = data[8];
                            data[8] = data[9];
                            data[9] = temp;
                            temp = (byte)(data[3] >> 4);
                            data[3] *= 16;
                            data[3] |= temp;
                            temp = data[9];
                            data[9] = data[5];
                            data[5] = temp;
                            data[0] ^= 0x6C;
                            temp = data[10];
                            data[10] = data[7];
                            data[7] = temp;
                            temp = (byte)((data[5] >> 7) & 1);
                            if (((data[5] >> 7) & 1) != 0)
                            {
                                data[5] |= 0x80;
                            }
                            else
                            {
                                data[5] &= 0x7F;
                            }

                            if (temp != 0)
                            {
                                data[5] |= 0x80;
                            }
                            else
                            {
                                data[5] &= 0x7F;
                            }
                        }
                    }
                    else
                    {
                        var temp = (byte)((data[1] >> 4) & 1);
                        if (((data[1] >> 1) & 1) != 0)
                        {
                            data[1] |= 0x10;
                        }
                        else
                        {
                            data[1] &= 0xEF;
                        }

                        if (temp != 0)
                        {
                            data[1] |= 2;
                        }
                        else
                        {
                            data[1] &= 0xFD;
                        }

                        data[1] ^= 0x91;
                        temp = (byte)((data[1] >> 2) & 1);
                        if (((data[1] >> 4) & 1) != 0)
                        {
                            data[1] |= 4;
                        }
                        else
                        {
                            data[1] &= 0xFB;
                        }

                        if (temp != 0)
                        {
                            data[1] |= 0x10;
                        }
                        else
                        {
                            data[1] &= 0xEF;
                        }

                        temp = data[6];
                        data[6] = data[3];
                        data[3] = temp;
                        temp = (byte)(data[3] >> 6);
                        data[3] *= 4;
                        data[3] |= temp;
                        temp = data[2];
                        data[2] = data[5];
                        data[5] = temp;
                        temp = (byte)((data[2] >> 6) & 1);
                        if (((data[2] >> 3) & 1) != 0)
                        {
                            data[2] |= 0x40;
                        }
                        else
                        {
                            data[2] &= 0xBF;
                        }

                        if (temp != 0)
                        {
                            data[2] |= 8;
                        }
                        else
                        {
                            data[2] &= 0xF7;
                        }

                        data[1] ^= 0x1A;
                        temp = (byte)(data[0] >> 2);
                        data[0] <<= 6;
                        data[0] |= temp;
                        temp = (byte)(data[7] >> 2);
                        data[7] <<= 6;
                        data[7] |= temp;
                        temp = (byte)((data[7] >> 5) & 1);
                        if (((data[7] >> 1) & 1) != 0)
                        {
                            data[7] |= 0x20;
                        }
                        else
                        {
                            data[7] &= 0xDF;
                        }

                        if (temp != 0)
                        {
                            data[7] |= 2;
                        }
                        else
                        {
                            data[7] &= 0xFD;
                        }
                    }
                }
                else
                {
                    var temp = (byte)((data[1] >> 3) & 1);
                    if (((data[1] >> 4) & 1) != 0)
                    {
                        data[1] |= 8;
                    }
                    else
                    {
                        data[1] &= 0xF7;
                    }

                    if (temp != 0)
                    {
                        data[1] |= 0x10;
                    }
                    else
                    {
                        data[1] &= 0xEF;
                    }

                    temp = (byte)(data[1] >> 5);
                    data[1] *= 8;
                    data[1] |= temp;
                    data[0] ^= 0x9C;
                    temp = (byte)(data[2] >> 6);
                    data[2] *= 4;
                    data[2] |= temp;
                    temp = (byte)((data[0] >> 3) & 1);
                    if (((data[0] >> 7) & 1) != 0)
                    {
                        data[0] |= 8;
                    }
                    else
                    {
                        data[0] &= 0xF7;
                    }

                    if (temp != 0)
                    {
                        data[0] |= 0x80;
                    }
                    else
                    {
                        data[0] &= 0x7F;
                    }

                    temp = data[0];
                    data[0] = data[3];
                    data[3] = temp;
                    data[0] ^= 0xB1;
                    temp = (byte)((data[1] >> 2) & 1);
                    if (((data[1] >> 7) & 1) != 0)
                    {
                        data[1] |= 4;
                    }
                    else
                    {
                        data[1] &= 0xFB;
                    }

                    if (temp != 0)
                    {
                        data[1] |= 0x80;
                    }
                    else
                    {
                        data[1] &= 0x7F;
                    }

                    temp = (byte)((data[2] >> 7) & 1);
                    if (((data[2] >> 6) & 1) != 0)
                    {
                        data[2] |= 0x80;
                    }
                    else
                    {
                        data[2] &= 0x7F;
                    }

                    if (temp != 0)
                    {
                        data[2] |= 0x40;
                    }
                    else
                    {
                        data[2] &= 0xBF;
                    }
                }
            }
        }
    }
}
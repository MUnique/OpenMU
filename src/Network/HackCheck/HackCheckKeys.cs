// <copyright file="HackCheckKeys.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.HackCheck;

using System.Text;

/// <summary>
/// Keys used by the HackCheck stream encryption.
/// </summary>
public readonly struct HackCheckKeys
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HackCheckKeys"/> struct.
    /// </summary>
    /// <param name="key1">The first key byte.</param>
    /// <param name="key2">The second key byte.</param>
    public HackCheckKeys(byte key1, byte key2)
    {
        this.Key1 = key1;
        this.Key2 = key2;
    }

    /// <summary>
    /// Gets the first key byte.
    /// </summary>
    public byte Key1 { get; }

    /// <summary>
    /// Gets the second key byte.
    /// </summary>
    public byte Key2 { get; }

    /// <summary>
    /// Creates HackCheck keys based on the client main configuration values.
    /// </summary>
    /// <param name="customerName">The customer name.</param>
    /// <param name="clientSerial">The client serial.</param>
    /// <returns>The created keys.</returns>
    public static HackCheckKeys Create(string customerName, string clientSerial)
    {
        var customer = new byte[32];
        var serial = new byte[17];

        var customerBytes = Encoding.ASCII.GetBytes(customerName ?? string.Empty);
        var serialBytes = Encoding.ASCII.GetBytes(clientSerial ?? string.Empty);

        var customerLength = Math.Min(customerBytes.Length, customer.Length);
        var serialLength = Math.Min(serialBytes.Length, serial.Length);

        customerBytes.AsSpan(0, customerLength).CopyTo(customer);
        serialBytes.AsSpan(0, serialLength).CopyTo(serial);

        ushort encDecKey = 0;
        for (var i = 0; i < customer.Length; i++)
        {
            var customerByte = customer[i];
            var serialByte = serial[i % serial.Length];
            encDecKey = unchecked((ushort)(encDecKey + (byte)(customerByte ^ serialByte)));
            encDecKey = unchecked((ushort)(encDecKey ^ (byte)(customerByte - serialByte)));
        }

        var key1 = unchecked((byte)(0xB0 + (encDecKey & 0xFF)));
        var key2 = unchecked((byte)(0xF8 + ((encDecKey >> 8) & 0xFF)));

        return new HackCheckKeys(key1, key2);
    }
}

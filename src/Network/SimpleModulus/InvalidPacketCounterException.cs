﻿// <copyright file="InvalidPacketCounterException.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.SimpleModulus;

/// <summary>
/// This exception occurs when the received packet counter (a value between 0 and 255) doesn't match the expectation of the <see cref="PipelinedSimpleModulusDecryptor"/>.
/// </summary>
/// <seealso cref="System.Exception" />
public class InvalidPacketCounterException : Exception
{
    private readonly byte _actual;
    private readonly byte _expected;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPacketCounterException"/> class.
    /// </summary>
    /// <param name="actual">The actual.</param>
    /// <param name="expected">The expected.</param>
    public InvalidPacketCounterException(byte actual, byte expected)
    {
        this._actual = actual;
        this._expected = expected;
    }

    /// <inheritdoc />
    public override string Message => $"Invalid packet counter, actual {this._actual}, expected {this._expected}";
}
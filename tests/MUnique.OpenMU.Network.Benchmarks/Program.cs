// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#pragma warning disable SA1200

global using BenchmarkDotNet.Attributes;
global using BenchmarkDotNet.Jobs;
global using BenchmarkDotNet.Running;

#pragma warning restore SA1200

namespace MUnique.OpenMU.Network.Benchmarks;

/// <summary>
/// The class of the entry point of the benchmark.
/// </summary>
public static class Program
{
    /// <summary>
    /// The entry point of the benchmark.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<EncryptionBenchmarks>();
        BenchmarkRunner.Run<PacketSending>();
    }
}
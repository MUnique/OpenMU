// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Measurements;

using System.Diagnostics;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.EntityFramework.Json;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Entry point of the measurement tool.
/// </summary>
/// <remarks>
/// Each measurement runs in its own process (see <see cref="RunAllAsync"/>),
/// so that the first ("cold") numbers include JIT and first-time initialization,
/// like they would at server start.
/// </remarks>
public static class Program
{
    private const string Usage = """
        Usage: MUnique.OpenMU.Measurements <mode> [options]

        Modes:
          init                 Drops and re-creates the database with the season 6 data and test accounts.
          plugins              Measures plugin discovery and registration (including proxy compilation).
          proxies              Measures only the runtime compilation of the plugin proxies.
          efmodel              Measures the runtime model building of the EF Core contexts.
          config               Measures loading the game configuration.
          config-cold          Measures the steps of loading the game configuration, each step cold.
          account              Measures loading accounts.
          snapshot <folder>    Writes canonical JSON snapshots of the configuration and the test accounts.
          all                  Runs all measurement modes, each in a new process.

        Options:
          --iterations <n>     Number of warm iterations for config/account (default: 5).
          --accounts <a,b,..>  Login names for account/snapshot (default: test0,test400,testgm).

        The database connection is taken from ConnectionSettings.xml of the EntityFramework project.
        """;

    /// <summary>
    /// The entry point.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>The exit code.</returns>
    public static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine(Usage);
            return 1;
        }

        // Same as the server does at startup.
        JsonConverterRegistry.RegisterConverter(new LocalizedStringJsonConverter());
        JsonConverterRegistry.RegisterConverter(new BinaryAsHexJsonConverter());

        var options = Options.Parse(args.Skip(1).ToArray());
        switch (args[0])
        {
            case "init":
                await DatabaseInitializer.RunAsync().ConfigureAwait(false);
                break;
            case "plugins":
                PlugInMeasurements.MeasureDiscoveryAndRegistration();
                break;
            case "proxies":
                PlugInMeasurements.MeasureProxyCompilation();
                break;
            case "efmodel":
                ModelMeasurements.Measure();
                break;
            case "config":
                await LoadingMeasurements.MeasureConfigurationAsync(options.Iterations).ConfigureAwait(false);
                break;
            case "config-cold":
                await LoadingMeasurements.MeasureConfigurationColdAsync().ConfigureAwait(false);
                break;
            case "account":
                await LoadingMeasurements.MeasureAccountsAsync(options.Iterations, options.Accounts).ConfigureAwait(false);
                break;
            case "snapshot":
                await SnapshotWriter.WriteAsync(options.Folder ?? "snapshots", options.Accounts).ConfigureAwait(false);
                break;
            case "all":
                return await RunAllAsync(args.Skip(1).ToArray()).ConfigureAwait(false);
            default:
                Console.WriteLine(Usage);
                return 1;
        }

        return 0;
    }

    private static async ValueTask<int> RunAllAsync(string[] passThroughArgs)
    {
        Console.WriteLine($"Machine: {Environment.ProcessorCount} logical cores, {Environment.OSVersion}, .NET {Environment.Version}");
        Console.WriteLine();

        foreach (var mode in new[] { "plugins", "proxies", "efmodel", "config", "config-cold", "account" })
        {
            var startInfo = new ProcessStartInfo(Environment.ProcessPath!)
            {
                UseShellExecute = false,
            };
            startInfo.ArgumentList.Add(mode);
            foreach (var arg in passThroughArgs)
            {
                startInfo.ArgumentList.Add(arg);
            }

            using var process = Process.Start(startInfo)!;
            await process.WaitForExitAsync().ConfigureAwait(false);
            if (process.ExitCode != 0)
            {
                Console.WriteLine($"Mode '{mode}' failed with exit code {process.ExitCode}.");
                return process.ExitCode;
            }

            Console.WriteLine();
        }

        return 0;
    }

    private sealed record Options(int Iterations, string[] Accounts, string? Folder)
    {
        public static Options Parse(string[] args)
        {
            var iterations = 5;
            var accounts = new[] { "test0", "test400", "testgm" };
            string? folder = null;
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--iterations":
                        iterations = int.Parse(args[++i]);
                        break;
                    case "--accounts":
                        accounts = args[++i].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        break;
                    default:
                        folder = args[i];
                        break;
                }
            }

            return new Options(iterations, accounts, folder);
        }
    }
}

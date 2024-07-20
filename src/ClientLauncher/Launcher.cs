// <copyright file="Launcher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ClientLauncher;

using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Microsoft.Win32;

/// <summary>
/// The default launcher, which writes
/// * host and port into the registry (official way until season 6 at the global server)
/// * adds parameters /u and /p (works in some other versions of the game client)
/// before starting the main.exe.
/// </summary>
public class Launcher : ILauncher
{
    /// <summary>
    /// Gets or sets the host ip.
    /// </summary>
    public string? HostAddress { get; set; }

    /// <summary>
    /// Gets or sets the host port.
    /// </summary>
    public int HostPort { get; set; }

    /// <summary>
    /// Gets or sets the main executable path.
    /// </summary>
    public string? MainExePath { get; set; }

    /// <summary>
    /// Launches Mu with the set configuration.
    /// </summary>
    public void LaunchClient()
    {
        if (string.IsNullOrWhiteSpace(this.HostAddress))
        {
            MessageBox.Show("Host address is not set.", "Error");
            return;
        }

        if (string.IsNullOrWhiteSpace(this.MainExePath))
        {
            MessageBox.Show("The path to the main.exe is not set.", "Error");
            return;
        }

        if (this.ResolveHost() is not { } ipAddress)
        {
            MessageBox.Show($"Address '{this.HostAddress} could not be resolved to an IPv4 address.", "Error");
            return;
        }

        if (OperatingSystem.IsWindows())
        {
            using var localMachineKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            using var key = localMachineKey.CreateSubKey(@"SOFTWARE\WebZen\Mu\Connection");
            key.SetValue("Key", Environment.TickCount, RegistryValueKind.DWord);
            key.SetValue("ParameterA", this.HostEncode(ipAddress), RegistryValueKind.String);
            key.SetValue("ParameterB", this.PortEncode(ipAddress), RegistryValueKind.DWord);
        }
        else
        {
            if (MessageBox.Show(
                    "IP and Port couldn't be set, because the operating system is not windows. Try to launch the game client anyway?",
                    string.Empty,
                    MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
        }

        var info = new DirectoryInfo(this.MainExePath!);
        var startInfo = new ProcessStartInfo(this.MainExePath, ["connect", $"/u{ipAddress}", $"/p{this.HostPort}"])
        {
            WorkingDirectory = info.Parent!.FullName,
            UseShellExecute = true,
            Verb = "open",
        };
        if (OperatingSystem.IsWindows())
        {
            startInfo.LoadUserProfile = true;
        }

        Process.Start(startInfo);
    }

    /// <summary>
    /// Encodes the Port, so that the main.exe can read it.
    /// </summary>
    /// <returns>Encoded port value which will be put into ParameterB.</returns>
    private int PortEncode(string ipAddress)
    {
        var port = this.HostPort;
        switch (ipAddress.Length % 4)
        {
            case 0:
                port += 12 - (((port / 4) % 4) * 8);
                return port;

            case 1:
                port += 7 - ((port % 8) * 2);
                return port;

            case 2:
                port += 3 - ((port % 4) * 2);
                return port;

            case 3:
                port += (0x13 - ((port % 4) * 2)) - (((port / 0x10) % 2) * 0x20);
                return port;
            default:
                // we'll hopefully never run into this one
                return port;
        }
    }

    private string? ResolveHost()
    {
        if (IPAddress.TryParse(this.HostAddress, out _))
        {
            return this.HostAddress;
        }

        var entry = Dns.GetHostEntry(this.HostAddress!);
        if (entry.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork) is { } ipAddress)
        {
            var address = ipAddress.ToString();

            // The game client blocks connections to 127.0.0.1 (probably to prevent cheating).
            return address == "127.0.0.1" ? "127.127.127.127" : address;
        }

        return null;
    }

    /// <summary>
    /// Encodes the IP Address of the server, so that the main.exe can read it.
    /// </summary>
    /// <returns>Encoded string value of the ip address which to put into ParameterA.</returns>
    private string HostEncode(string ipAddress)
    {
        var result = new StringBuilder();
        var counter = 0;
        foreach (var ch in ipAddress)
        {
            var encodedCharacter = '\0';
            counter++;
            switch (counter)
            {
                case 1:
                    encodedCharacter = (char)(ch + '\f');
                    break;

                case 2:
                    encodedCharacter = (char)(ch + '\a');
                    break;

                case 3:
                    encodedCharacter = (char)(ch + '\x0003');
                    break;

                case 4:
                    encodedCharacter = (char)(ch + '\x0013');
                    counter = 0;
                    break;
                default:
                    // we should not run into this case, since it's always 1 to 4.
                    break;
            }

            result.Append(encodedCharacter);
        }

        return result.ToString();
    }
}
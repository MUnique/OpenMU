// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.LoginProbe;

using System;
using System.Buffers;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.Packets.ConnectServer;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Network.Xor;
using Pipelines.Sockets.Unofficial;

/// <summary>
/// A wire-level login probe: it does what a real client does - ask the connect server for a game
/// server, log in there, and hold the session - so a scenario can prove what the server does while
/// an account is genuinely connected (e.g. that a scripted actor refuses to animate it).
/// </summary>
/// <remarks>
/// Meant to be driven from test scripts. It speaks the protocol through the server's own network
/// stack (<c>MUnique.OpenMU.Network</c>), so it stays correct when the encryption or the packet
/// layout changes.
/// </remarks>
public static class Program
{
    /// <summary>
    /// The environment variable which carries the password, so it neither shows up in the process
    /// list nor lands in a shell history. Unset, the password is the account name, which is what
    /// the shipped test accounts use.
    /// </summary>
    public static readonly string PasswordVariableName = "LOGINPROBE_PASSWORD";

    /// <summary>The client version a Season 6 Episode 3 client reports ("20404" in ASCII).</summary>
    private static readonly byte[] ClientVersion = [0x32, 0x30, 0x34, 0x30, 0x34];

    /// <summary>The client serial of the open-source client.</summary>
    private static readonly byte[] ClientSerial = Encoding.ASCII.GetBytes("k1Pk2jcET48mxL3b");

    /// <summary>
    /// Runs the probe.
    /// </summary>
    /// <param name="args">The command line arguments; see <see cref="Usage"/>.</param>
    /// <returns>0 when the login succeeded, 1 when it failed, 2 on an argument error.</returns>
    public static async Task<int> Main(string[] args)
    {
        var options = ProbeOptions.Parse(args);
        if (options is null)
        {
            Usage();
            return 2;
        }

        try
        {
            var (gameServerHost, gameServerPort) = await ResolveGameServerAsync(options).ConfigureAwait(false);
            Write(new { step = "server_selected", host = gameServerHost, port = gameServerPort, server = options.ServerId });
            return await LoginAndHoldAsync(options, gameServerHost, gameServerPort).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Write(new { ok = false, step = "failed", error = ex.Message });
            return 1;
        }
    }

    private static void Usage()
    {
        Console.Error.WriteLine("usage: MUnique.OpenMU.Network.LoginProbe --account <name> [--host 127.0.0.1]");
        Console.Error.WriteLine("       [--connect-port 44406] [--server 0] [--hold <seconds>]");
        Console.Error.WriteLine();
        Console.Error.WriteLine($"The password is read from the environment variable {PasswordVariableName};");
        Console.Error.WriteLine("when it is unset, the account name is used as the password.");
        Console.Error.WriteLine("Logs into the game server like a real client and holds the session.");
        Console.Error.WriteLine("Writes one JSON object per step to stdout; exit 0 on a successful login.");
    }

    private static void Write(object payload)
    {
        Console.WriteLine(JsonSerializer.Serialize(payload));
        Console.Out.Flush();
    }

    /// <summary>
    /// Asks the connect server which game server to use, exactly like a client does.
    /// </summary>
    private static async Task<(string Host, ushort Port)> ResolveGameServerAsync(ProbeOptions options)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(options.Host, options.ConnectPort).ConfigureAwait(false);
        using var stream = client.GetStream();
        stream.ReadTimeout = 10000;

        // The connect server greets with a hello packet; it is not needed, but it has to be read.
        await ReadPacketAsync(stream).ConfigureAwait(false);

        var serverListRequest = new byte[ServerListRequest.Length];
        _ = new ServerListRequest(serverListRequest);
        await stream.WriteAsync(serverListRequest).ConfigureAwait(false);
        await ReadPacketAsync(stream).ConfigureAwait(false);

        var infoRequest = new byte[ConnectionInfoRequest.Length];
        _ = new ConnectionInfoRequest(infoRequest) { ServerId = (ushort)options.ServerId };
        await stream.WriteAsync(infoRequest).ConfigureAwait(false);

        var response = await ReadPacketAsync(stream).ConfigureAwait(false);
        var info = new ConnectionInfo(response);
        return (info.IpAddress, info.Port);
    }

    private static async Task<byte[]> ReadPacketAsync(NetworkStream stream)
    {
        var header = new byte[3];
        await stream.ReadExactlyAsync(header.AsMemory(0, 1)).ConfigureAwait(false);
        int length;
        int headerSize;
        if (header[0] is 0xC1 or 0xC3)
        {
            await stream.ReadExactlyAsync(header.AsMemory(1, 1)).ConfigureAwait(false);
            length = header[1];
            headerSize = 2;
        }
        else
        {
            await stream.ReadExactlyAsync(header.AsMemory(1, 2)).ConfigureAwait(false);
            length = (header[1] << 8) | header[2];
            headerSize = 3;
        }

        var packet = new byte[length];
        header.AsSpan(0, headerSize).CopyTo(packet);
        await stream.ReadExactlyAsync(packet.AsMemory(headerSize, length - headerSize)).ConfigureAwait(false);
        return packet;
    }

    /// <summary>
    /// Logs in on the game server and keeps the connection open for the requested time.
    /// </summary>
    private static async Task<int> LoginAndHoldAsync(ProbeOptions options, string gameServerHost, ushort gameServerPort)
    {
        var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        await socket.ConnectAsync(IPAddress.Parse(gameServerHost), gameServerPort).ConfigureAwait(false);
        var socketConnection = SocketConnection.Create(socket);

        var encryptionFactory = new OpenSourceClientNetworkEncryptionFactoryPlugIn();
        using var connection = new Connection(
            socketConnection,
            encryptionFactory.CreateDecryptor(socketConnection.Input, DataDirection.ServerToClient),
            encryptionFactory.CreateEncryptor(socketConnection.Output, DataDirection.ClientToServer),
            new NullLogger<Connection>());

        var loginResult = new TaskCompletionSource<LoginResponse.LoginResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.PacketReceived += packet =>
        {
            HandlePacket(packet, loginResult);
            return ValueTask.CompletedTask;
        };
        connection.Disconnected += () =>
        {
            loginResult.TrySetException(new IOException("The game server closed the connection."));
            return ValueTask.CompletedTask;
        };

        _ = connection.BeginReceiveAsync();

        // The client waits for the server's greeting before it sends its credentials.
        await Task.Delay(500).ConfigureAwait(false);

        await connection.SendLoginLongPasswordAsync(
            Encrypt(options.Account),
            Encrypt(options.Password),
            (uint)Environment.TickCount,
            ClientVersion,
            ClientSerial).ConfigureAwait(false);

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var result = await loginResult.Task.WaitAsync(timeout.Token).ConfigureAwait(false);
        var ok = result == LoginResponse.LoginResult.Okay;
        Write(new { ok, step = "login", account = options.Account, result = result.ToString() });
        if (!ok)
        {
            await connection.DisconnectAsync().ConfigureAwait(false);
            return 1;
        }

        if (options.HoldSeconds > 0)
        {
            Write(new { ok = true, step = "holding", seconds = options.HoldSeconds });
            await Task.Delay(TimeSpan.FromSeconds(options.HoldSeconds)).ConfigureAwait(false);
        }

        await connection.DisconnectAsync().ConfigureAwait(false);
        Write(new { ok = true, step = "disconnected", account = options.Account });
        return 0;
    }

    private static void HandlePacket(ReadOnlySequence<byte> packet, TaskCompletionSource<LoginResponse.LoginResult> loginResult)
    {
        var data = packet.ToArray();
        if (data.Length >= LoginResponse.Length
            && data[0] == LoginResponse.HeaderType
            && data[2] == LoginResponse.Code
            && data[3] == LoginResponse.SubCode)
        {
            loginResult.TrySetResult(new LoginResponse(data).Success);
        }
    }

    /// <summary>
    /// Encrypts a credential the way the client does, with the well-known three byte key.
    /// </summary>
    private static Memory<byte> Encrypt(string value)
    {
        var buffer = new byte[10];
        Encoding.ASCII.GetBytes(value).AsSpan(0, Math.Min(value.Length, buffer.Length)).CopyTo(buffer);
        new Xor3Encryptor(0).Encrypt(buffer);
        return buffer;
    }

    private sealed class ProbeOptions
    {
        public string Host { get; private set; } = "127.0.0.1";

        public int ConnectPort { get; private set; } = 44406;

        public int ServerId { get; private set; }

        public int HoldSeconds { get; private set; } = 30;

        public string Account { get; private set; } = string.Empty;

        public string Password { get; private set; } = string.Empty;

        public static ProbeOptions? Parse(string[] args)
        {
            var options = new ProbeOptions();
            var index = 0;
            while (index < args.Length)
            {
                if (index + 1 >= args.Length)
                {
                    return null;
                }

                var value = args[index + 1];
                switch (args[index])
                {
                    case "--host":
                        options.Host = value;
                        break;
                    case "--connect-port":
                        options.ConnectPort = int.Parse(value, CultureInfo.InvariantCulture);
                        break;
                    case "--server":
                        options.ServerId = int.Parse(value, CultureInfo.InvariantCulture);
                        break;
                    case "--hold":
                        options.HoldSeconds = int.Parse(value, CultureInfo.InvariantCulture);
                        break;
                    case "--account":
                        options.Account = value;
                        break;
                    default:
                        return null;
                }

                index += 2;
            }

            if (string.IsNullOrEmpty(options.Account))
            {
                return null;
            }

            options.Password = Environment.GetEnvironmentVariable(PasswordVariableName) is { Length: > 0 } password
                ? password
                : options.Account;

            return options;
        }
    }
}

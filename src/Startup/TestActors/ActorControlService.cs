// <copyright file="ActorControlService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup.TestActors;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic.TestActors;

/// <summary>
/// The local control endpoint: a plain TCP listener which speaks newline-delimited JSON.
/// </summary>
/// <remarks>
/// Development tooling, without any authentication: it is only started when
/// <see cref="EnvironmentVariableName"/> names a port, the local stack publishes that port on the
/// host's loopback address only, and the release deployment never sets the variable.
/// </remarks>
public sealed class ActorControlService : BackgroundService
{
    /// <summary>
    /// The environment variable which enables the endpoint and names its port.
    /// </summary>
    public static readonly string EnvironmentVariableName = "OPENMU_ACTOR_PORT";

    /// <summary>
    /// UTF-8 without a byte order mark: the first line of a connection must be plain JSON, or a
    /// strict reader (stdlib Python, <c>jq</c>) chokes on the BOM.
    /// </summary>
    private static readonly Encoding LineEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    /// <summary>
    /// How long the SIGTERM handler waits for the actors to log out before it lets the process die.
    /// Docker's default grace period is ten seconds.
    /// </summary>
    private static readonly TimeSpan SignalStopTimeout = TimeSpan.FromSeconds(8);

    private readonly int _port;
    private readonly IActorRegistry _registry;
    private readonly ActorProtocolHandler _handler;
    private readonly ILogger<ActorControlService> _logger;
    private PosixSignalRegistration? _sigTermRegistration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActorControlService"/> class.
    /// </summary>
    /// <param name="options">The endpoint options, i.e. the port.</param>
    /// <param name="registry">The actor registry, so shutdown can stop every actor.</param>
    /// <param name="handler">The protocol handler.</param>
    /// <param name="logger">The logger.</param>
    public ActorControlService(ActorEndpointOptions options, IActorRegistry registry, ActorProtocolHandler handler, ILogger<ActorControlService> logger)
    {
        this._port = options.Port;
        this._registry = registry;
        this._handler = handler;
        this._logger = logger;
    }

    /// <summary>
    /// Gets the port configured in the environment, or <c>null</c> when the endpoint is off (the default).
    /// </summary>
    public static int? ConfiguredPort
    {
        get
        {
            var value = Environment.GetEnvironmentVariable(EnvironmentVariableName);
            return int.TryParse(value, out var port) && port is > 0 and <= 65535 ? port : null;
        }
    }

    /// <summary>
    /// Stops every actor through the normal logout path, before the game servers of this host stop.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        var stopped = await this._registry.StopAllAsync().ConfigureAwait(false);
        if (stopped > 0)
        {
            this._logger.LogInformation("Stopped {Count} actor(s) before shutdown.", stopped);
        }

        await base.StopAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this._sigTermRegistration?.Dispose();
        base.Dispose();
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // `docker stop` sends SIGTERM, but this host's main loop only stops the services from an
        // AppDomain.ProcessExit handler, which the runtime cuts short - so the hosted services'
        // StopAsync usually never runs in a container, and an actor's unsaved progress (anything
        // since the last periodic save) would be lost on every `dev down`. Logging the actors out
        // straight from the signal handler makes the shutdown promise hold regardless.
        this._sigTermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, _ => this.StopActorsOnSignal());

        var listener = new TcpListener(IPAddress.Any, this._port);
        try
        {
            listener.Start();
            this._logger.LogInformation("Actor control endpoint listening on 0.0.0.0:{Port}.", this._port);

            while (!stoppingToken.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync(stoppingToken).ConfigureAwait(false);
                _ = this.HandleClientAsync(client, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // The host is stopping.
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "The actor control endpoint stopped unexpectedly.");
        }
        finally
        {
            listener.Stop();
        }
    }

    private void StopActorsOnSignal()
    {
        try
        {
            this._logger.LogInformation("SIGTERM received: logging the actors out before the game servers stop.");

            // Blocking is the point: the handler runs on the signal thread and the process is about
            // to die, so the logout has to finish here rather than on some continuation which will
            // never be scheduled. Bounded, so a stuck actor cannot hold the shutdown open.
#pragma warning disable VSTHRD002 // Synchronously waiting on tasks or awaiters may cause deadlocks
            var stopTask = Task.Run(() => this._registry.StopAllAsync().AsTask());
            if (!stopTask.Wait(SignalStopTimeout))
            {
                this._logger.LogWarning("The actors did not stop within {Timeout}; their last progress may be lost.", SignalStopTimeout);
                return;
            }

            this._logger.LogInformation("Stopped {Count} actor(s) on SIGTERM.", stopTask.Result);
#pragma warning restore VSTHRD002
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Failed to stop the actors on SIGTERM.");
        }
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken stoppingToken)
    {
        using var connectionSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        var cancellationToken = connectionSource.Token;
        try
        {
            using (client)
            {
                await using var stream = client.GetStream();
                using var reader = new StreamReader(stream, LineEncoding, leaveOpen: true);
                await using var writer = new StreamWriter(stream, LineEncoding, leaveOpen: true) { AutoFlush = true, NewLine = "\n" };

                async ValueTask WriteLineAsync(string line)
                {
                    await writer.WriteLineAsync(line.AsMemory(), cancellationToken).ConfigureAwait(false);
                }

                while (!cancellationToken.IsCancellationRequested
                       && await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) is { } line)
                {
                    // A malformed line is answered with an error; the connection stays open and the
                    // other connections and actors are unaffected.
                    await this._handler.HandleLineAsync(line, WriteLineAsync, cancellationToken).ConfigureAwait(false);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // The connection or the host went away.
        }
        catch (IOException)
        {
            // The client hung up, e.g. an interrupted 'events --follow'.
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "An actor control connection failed.");
        }
    }
}

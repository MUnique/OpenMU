// <copyright file="ActorControlService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup.TestActors;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.TestActors;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The local control endpoint: a plain TCP listener which speaks newline-delimited JSON.
/// </summary>
/// <remarks>
/// Development tooling, without any authentication: it is only started when
/// <see cref="PortVariableName"/> names a port, and it binds the loopback address unless
/// <see cref="AddressVariableName"/> explicitly names another one (see
/// <see cref="ActorEndpointOptions.TryParse"/>). So setting the port on a host which is run
/// directly opens the endpoint for local processes only; a container has to ask for
/// <c>0.0.0.0</c> and rely on its port publishing.
/// </remarks>
public sealed class ActorControlService : BackgroundService
{
    /// <summary>
    /// The environment variable which enables the endpoint by naming its port.
    /// </summary>
    public static readonly string PortVariableName = "OPENMU_ACTOR_PORT";

    /// <summary>
    /// The environment variable which names the address to bind; unset, the loopback address.
    /// </summary>
    public static readonly string AddressVariableName = "OPENMU_ACTOR_ADDRESS";

    /// <summary>
    /// UTF-8 without a byte order mark: the first line of a connection must be plain JSON, or a
    /// strict reader (stdlib Python, <c>jq</c>) chokes on the BOM.
    /// </summary>
    private static readonly Encoding LineEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    private readonly IPEndPoint _endPoint;
    private readonly IActorRegistry _registry;
    private readonly ActorProtocolHandler _handler;
    private readonly PlugInManager _plugInManager;
    private readonly ILogger<ActorControlService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActorControlService"/> class.
    /// </summary>
    /// <param name="options">The endpoint options, i.e. the address and port.</param>
    /// <param name="registry">The actor registry, so shutdown can stop every actor.</param>
    /// <param name="handler">The protocol handler.</param>
    /// <param name="plugInManager">The plugin manager of the game servers, to register the hit recorder.</param>
    /// <param name="logger">The logger.</param>
    public ActorControlService(ActorEndpointOptions options, IActorRegistry registry, ActorProtocolHandler handler, PlugInManager plugInManager, ILogger<ActorControlService> logger)
    {
        this._endPoint = options.EndPoint;
        this._registry = registry;
        this._handler = handler;
        this._plugInManager = plugInManager;
        this._logger = logger;
    }

    /// <summary>
    /// Gets the endpoint configured in the environment, or <c>null</c> when the endpoint is off (the default).
    /// </summary>
    public static ActorEndpointOptions? ConfiguredOptions
        => ActorEndpointOptions.TryParse(
            Environment.GetEnvironmentVariable(PortVariableName),
            Environment.GetEnvironmentVariable(AddressVariableName));

    /// <summary>
    /// Registers the hit recorder, which is not a discoverable plugin on purpose, and starts listening.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        this._plugInManager.RegisterPlugIn<IAttackableGotHitPlugIn, ActorHitRecorderPlugIn>();
        return base.StartAsync(cancellationToken);
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
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var listener = new TcpListener(this._endPoint);
        try
        {
            listener.Start();
            this._logger.LogInformation("Actor control endpoint listening on {EndPoint}.", listener.LocalEndpoint);

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

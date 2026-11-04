// <copyright file="GameServerAdministrationHttpTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Threading;
using global::Dapr.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.GameServer.Host;
using MUnique.OpenMU.Interfaces;

/// <summary>Exercises the internal administration controller over real HTTP, before action execution.</summary>
[TestFixture]
public sealed class GameServerAdministrationHttpTests
{
    private const string TestToken = "test-only-sidecar-token";

    /// <summary>All privileged routes reject callers which bypass the sidecar.</summary>
    /// <param name="operation">The privileged route.</param>
    [TestCase("TeleportAsync")]
    [TestCase("GiveItemAsync")]
    [TestCase("SetLevelAsync")]
    [TestCase("StartEventAsync")]
    [TestCase("ExecuteAdminOperationAsync")]
    public async Task RejectsUnauthenticatedActionsAsync(string operation)
    {
        var server = new Mock<IGameServer>();
        var live = server.As<IGameServerAdministration>();
        await using var app = await CreateHostAsync(server.Object, TestToken).ConfigureAwait(false);
        using var http = new HttpClient { BaseAddress = new Uri(app.Urls.Single()) };
        foreach (var prefix in new[] { string.Empty, "/gameServer/4" })
        {
            using var body = JsonContent.Create(new GameServerAdministrationRequest { ServerId = 4 });
            using var response = await http.PostAsync($"{prefix}/administration/{operation}", body).ConfigureAwait(false);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        Assert.That(live.Invocations, Is.Empty);
        using var health = await http.GetAsync("/health/live").ConfigureAwait(false);
        Assert.That(health.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    /// <summary>Only one matching token is accepted; missing configuration also fails closed.</summary>
    /// <param name="configured">Whether the host has a token.</param>
    /// <param name="supplied">The supplied header token.</param>
    /// <param name="duplicate">Whether to send a duplicate credential.</param>
    [TestCase(true, null, false)]
    [TestCase(true, "incorrect", false)]
    [TestCase(true, TestToken, true)]
    [TestCase(false, TestToken, false)]
    [TestCase(true, TestToken, false)]
    public async Task ValidatesSidecarTokenAsync(bool configured, string? supplied, bool duplicate)
    {
        var server = new Mock<IGameServer>();
        server.SetupGet(s => s.Id).Returns(4);
        var live = server.As<IGameServerAdministration>();
        live.Setup(s => s.SetLevelAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(GameServerAdminResult.Success);
        await using var app = await CreateHostAsync(server.Object, configured ? TestToken : null).ConfigureAwait(false);
        using var http = new HttpClient { BaseAddress = new Uri(app.Urls.Single()) };
        if (supplied is not null)
        {
            http.DefaultRequestHeaders.TryAddWithoutValidation("dapr-api-token", duplicate ? [supplied, supplied] : [supplied]);
        }

        using var body = JsonContent.Create(new GameServerAdministrationRequest { ServerId = 4, CharacterName = "Hero", Level = 100 });
        using var response = await http.PostAsync("/administration/SetLevelAsync", body).ConfigureAwait(false);
        var accepted = configured && supplied == TestToken && !duplicate;
        Assert.That(response.StatusCode, Is.EqualTo(accepted ? HttpStatusCode.OK : HttpStatusCode.Unauthorized));
        live.Verify(s => s.SetLevelAsync("Hero", It.IsAny<Guid>(), 100, false, It.IsAny<CancellationToken>()), accepted ? Times.Once() : Times.Never());
    }

    /// <summary>Runs the real Dapr process and verifies authenticated service invocation to the live controller.</summary>
    [Test]
    [Explicit("Set OPENMU_TEST_DAPRD_PATH to a local daprd executable and select this test explicitly.")]
    public async Task DaprSidecarInvokesAuthenticatedLiveChannelAsync()
    {
        var executable = Environment.GetEnvironmentVariable("OPENMU_TEST_DAPRD_PATH") ?? throw new InvalidOperationException("Set OPENMU_TEST_DAPRD_PATH to daprd.");
        Assert.That(executable, Is.Not.Null.And.Not.Empty);
        var server = new Mock<IGameServer>();
        server.SetupGet(s => s.Id).Returns(4);
        var live = server.As<IGameServerAdministration>();
        var hero = new LivePlayerInfo(Guid.NewGuid(), "tester", "Hero", 0, 1, 2, 10, 0, null, false);
        live.Setup(s => s.GetPlayerSnapshotAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { hero });
        live.Setup(s => s.GiveItemAsync("Hero", hero.CharacterId, It.IsAny<ItemSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(GameServerAdminResult.Success);
        await using var app = await CreateHostAsync(server.Object, TestToken).ConfigureAwait(false);
        var ports = new HashSet<int>();
        while (ports.Count < 3)
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            ports.Add(((IPEndPoint)listener.LocalEndpoint).Port);
        }

        var available = ports.ToArray();
        var resources = Directory.CreateTempSubdirectory("openmu-dapr-tests-");
        var start = new ProcessStartInfo(executable) { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        foreach (var argument in new[] { "--app-id", "game-server-4", "--app-port", new Uri(app.Urls.Single()).Port.ToString(System.Globalization.CultureInfo.InvariantCulture),
            "--app-channel-address", "127.0.0.1", "--dapr-http-port", available[0].ToString(System.Globalization.CultureInfo.InvariantCulture),
            "--dapr-grpc-port", available[1].ToString(System.Globalization.CultureInfo.InvariantCulture), "--dapr-internal-grpc-port", available[2].ToString(System.Globalization.CultureInfo.InvariantCulture),
            "--dapr-listen-addresses", "127.0.0.1", "--resources-path", resources.FullName, "--log-level", "warn" })
        {
            start.ArgumentList.Add(argument);
        }

        start.Environment["APP_API_TOKEN"] = TestToken;
        using var process = Process.Start(start) ?? throw new InvalidOperationException("Could not start daprd.");
        var output = process.StandardOutput.ReadToEndAsync();
        var errors = process.StandardError.ReadToEndAsync();
        try
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var endpoint = $"http://127.0.0.1:{available[0]}";
            using var http = new HttpClient { BaseAddress = new Uri(endpoint) };
            while (true)
            {
                if (process.HasExited)
                {
                    Assert.Fail("Dapr exited before becoming ready: " + await errors.ConfigureAwait(false) + await output.ConfigureAwait(false));
                }

                try
                {
                    using var health = await http.GetAsync("/v1.0/healthz", timeout.Token).ConfigureAwait(false);
                    if (health.IsSuccessStatusCode)
                    {
                        break;
                    }
                }
                catch (HttpRequestException)
                {
                    // The process has not bound its API listener yet.
                }

                await Task.Delay(100, timeout.Token).ConfigureAwait(false);
            }

            using var dapr = new DaprClientBuilder().UseHttpEndpoint(endpoint).Build();
            var client = new GameServerClient(dapr, new ServerStateData { Id = 4, Type = ServerType.GameServer });
            var snapshot = await client.GetPlayerSnapshotAsync(timeout.Token).ConfigureAwait(false);
            Assert.That(snapshot.Single().Character, Is.EqualTo("Hero"));
            Assert.That((await client.GiveItemAsync("Hero", hero.CharacterId, new(0, 1), timeout.Token).ConfigureAwait(false)).Succeeded, Is.True);
            live.Verify(s => s.GiveItemAsync("Hero", hero.CharacterId, It.Is<ItemSpecification>(i => i.Group == 0 && i.Number == 1), It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }

            await process.WaitForExitAsync().ConfigureAwait(false);
            await output.ConfigureAwait(false);
            await errors.ConfigureAwait(false);
            resources.Delete(recursive: true);
        }
    }

    private static async ValueTask<WebApplication> CreateHostAsync(IGameServer server, string? token)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?> { ["APP_API_TOKEN"] = token });
        builder.Services.AddSingleton(server);
        builder.Services.AddSingleton<AdministrationDeliveries>();
        builder.Services.AddSingleton<DaprAppTokenAuthorizationFilter>();
        builder.Services.AddControllers().AddApplicationPart(typeof(GameServerAdministrationController).Assembly);
        var app = builder.Build();
        app.UsePathBase("/gameServer/4");
        app.MapControllers();
        app.MapGet("/health/live", () => "ok");
        await app.StartAsync().ConfigureAwait(false);
        return app;
    }
}

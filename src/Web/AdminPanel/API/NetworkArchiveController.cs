// <copyright file="NetworkArchiveController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.API;

using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Network.Analyzer.Archive;

/// <summary>
/// Controller which offers the archived sessions of the observed accounts as a download.
/// </summary>
/// <remarks>
/// The archived files are not served statically: they contain the traffic of a player in plain
/// text, including its login packet. Like every other controller of the admin panel, this one
/// requires an authenticated user.
/// </remarks>
[Route("api/network-archive/")]
public class NetworkArchiveController : Controller
{
    private readonly IServiceProvider _serviceProvider;

    private readonly ILogger<NetworkArchiveController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkArchiveController"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider, used to resolve the archive
    /// optionally - it's only registered when the network observation is configured.</param>
    /// <param name="logger">The logger.</param>
    public NetworkArchiveController(IServiceProvider serviceProvider, ILogger<NetworkArchiveController> logger)
    {
        this._serviceProvider = serviceProvider;
        this._logger = logger;
    }

    /// <summary>
    /// Downloads the specified archived session as one capture file, which can be opened by
    /// the analyzer tool.
    /// </summary>
    /// <param name="sessionId">The identifier of the session.</param>
    /// <returns>The async task.</returns>
    [HttpGet("{**sessionId}")]
    public async Task DownloadAsync(string sessionId)
    {
        if (this._serviceProvider.GetService(typeof(IPacketArchive)) is not IPacketArchive archive
            || await archive.GetSessionAsync(sessionId).ConfigureAwait(false) is not { } session)
        {
            this.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        // Downloading the traffic of a player is as intrusive as observing it, so it leaves a
        // trace as well.
        this._logger.LogInformation("The archived session {SessionId} has been downloaded.", session.Id);

        this.Response.ContentType = "text/csv";
        this.Response.Headers.ContentDisposition = $"attachment; filename=\"{GetFileName(session)}\"";

        // The parts of the session are concatenated to one file: they only differ by the point
        // in time at which the previous one got too big, and the timestamps of the packets are
        // relative to the start of the session anyway.
        await using var writer = new StreamWriter(this.Response.Body);
        await writer.WriteLineAsync(session.Metadata.StartTimestamp.ToString("O", CultureInfo.InvariantCulture)).ConfigureAwait(false);
        foreach (var part in session.Metadata.Parts)
        {
            await WritePartAsync(session, part, writer).ConfigureAwait(false);
        }
    }

    private static async Task WritePartAsync(ArchivedSessionInfo session, string part, StreamWriter writer)
    {
        var path = Path.Combine(session.DirectoryPath, Path.GetFileName(part));
        if (!System.IO.File.Exists(path))
        {
            return;
        }

        // The session may still be running, so the file is shared with its writer.
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        using var reader = new StreamReader(stream);

        // The first line of each part is the start timestamp, which is already written.
        _ = await reader.ReadLineAsync().ConfigureAwait(false);
        while (await reader.ReadLineAsync().ConfigureAwait(false) is { } line)
        {
            await writer.WriteLineAsync(line).ConfigureAwait(false);
        }
    }

    private static string GetFileName(ArchivedSessionInfo session)
    {
        var accountName = new string(session.Metadata.AccountName
            .Where(character => char.IsLetterOrDigit(character) || character is '-' or '_')
            .ToArray());
        if (string.IsNullOrEmpty(accountName))
        {
            accountName = "session";
        }

        return string.Create(
            CultureInfo.InvariantCulture,
            $"{accountName}_{session.Metadata.StartTimestamp:yyyy-MM-dd_HH-mm-ss}.mucap");
    }
}

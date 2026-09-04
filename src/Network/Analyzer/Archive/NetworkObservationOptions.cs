// <copyright file="NetworkObservationOptions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer.Archive;

using System.IO;

/// <summary>
/// The options of the network observation, which archives the traffic of the accounts whose
/// observation is active.
/// </summary>
/// <remarks>
/// They are configured in the system configuration of the database. This class is the plain
/// counterpart of it, so that the archive doesn't need to know the data model.
/// </remarks>
public sealed class NetworkObservationOptions
{
    /// <summary>
    /// The default path of the archive, relative to the directory of the application.
    /// </summary>
    public const string DefaultArchivePath = "captures";

    /// <summary>
    /// The default maximum size of one file of a session, in megabytes.
    /// </summary>
    public const int DefaultMaximumSessionSizeMb = 50;

    /// <summary>
    /// The default maximum size of the whole archive, in megabytes.
    /// </summary>
    public const int DefaultMaximumTotalSizeMb = 1000;

    /// <summary>
    /// The default number of days after which an archived session is removed.
    /// </summary>
    public const int DefaultRetentionDays = 30;

    /// <summary>
    /// Gets or sets the path of the archive. A relative path is resolved against the directory
    /// of the application.
    /// </summary>
    /// <remarks>
    /// It must not point into a folder which is served by the web server - an archived session
    /// contains the login packet of the player in plain text.
    /// </remarks>
    public string ArchivePath { get; set; } = DefaultArchivePath;

    /// <summary>
    /// Gets or sets the maximum size of one file of a session, in megabytes. A session which
    /// grows bigger is continued in another file.
    /// </summary>
    public int MaximumSessionSizeMb { get; set; } = DefaultMaximumSessionSizeMb;

    /// <summary>
    /// Gets or sets the maximum size of the whole archive, in megabytes. When it's exceeded,
    /// the oldest sessions are removed. A value of 0 or less means that the size is unlimited.
    /// </summary>
    public int MaximumTotalSizeMb { get; set; } = DefaultMaximumTotalSizeMb;

    /// <summary>
    /// Gets or sets the number of days after which an archived session is removed. A value of
    /// 0 or less means that the sessions are kept forever.
    /// </summary>
    public int RetentionDays { get; set; } = DefaultRetentionDays;

    /// <summary>
    /// Gets the maximum size of one file of a session, in bytes.
    /// </summary>
    /// <returns>The maximum size of one file of a session, in bytes.</returns>
    public long GetMaximumSessionSizeInBytes()
    {
        var sizeInMegabytes = this.MaximumSessionSizeMb > 0 ? this.MaximumSessionSizeMb : DefaultMaximumSessionSizeMb;
        return (long)sizeInMegabytes * 1024 * 1024;
    }

    /// <summary>
    /// Gets the full path of the archive.
    /// </summary>
    /// <returns>The full path of the archive.</returns>
    public string GetFullArchivePath()
    {
        var path = string.IsNullOrWhiteSpace(this.ArchivePath) ? DefaultArchivePath : this.ArchivePath;
        return Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(AppContext.BaseDirectory, path));
    }
}

// <copyright file="AdminLogAccess.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.IO;

/// <summary>Host-owned paths and availability for local log inspection.</summary>
public sealed class AdminLogAccess
{
    /// <summary>Gets or sets a value indicating whether this host owns game log files.</summary>
    public bool LocalFilesAvailable { get; set; }

    /// <summary>Gets or sets the trusted log directory; callers cannot choose a path.</summary>
    public string DirectoryPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "logs");
}

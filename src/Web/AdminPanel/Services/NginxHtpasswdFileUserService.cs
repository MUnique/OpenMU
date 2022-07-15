// <copyright file="NginxHtpasswdFileUserService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.IO;
using Blazored.Modal.Services;

/// <summary>
/// Implementation of <see cref="IUserService"/> which modifies the .htpasswd file of the nginx basic authentication.
/// </summary>
public class NginxHtpasswdFileUserService : UserServiceBase
{
    private const string HtpasswdFilePath = "/etc/nginx/.htpasswd";

    /// <summary>
    /// Initializes a new instance of the <see cref="NginxHtpasswdFileUserService"/> class.
    /// </summary>
    /// <param name="modalService">The modal service.</param>
    public NginxHtpasswdFileUserService(IModalService modalService)
        : base(modalService)
    {
    }

    /// <inheritdoc />
    public override bool IsAvailable => File.Exists(HtpasswdFilePath);

    /// <inheritdoc />
    public override ICollection<string> Users
    {
        get
        {
            var lines = File.ReadAllLines(HtpasswdFilePath);
            var relevant = lines.Where(l => !l.StartsWith('#'))
                .Where(l => l.Contains(':'));

            var users = relevant.Select(line => line.Split(':')[0]).ToList();
            return users;
        }
    }

    /// <summary>
    /// Deletes a user with the specified name.
    /// </summary>
    /// <param name="user">The username.</param>
    public override async Task DeleteUserAsync(string user)
    {
        var userPrefix = $"{user}:";
        var lines = File.ReadAllLines(HtpasswdFilePath)!;

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line.StartsWith(userPrefix, StringComparison.InvariantCulture))
            {
                lines[i] = $"# Deleted at {DateTime.UtcNow}: " + lines[i];
                break;
            }
        }

        var accountsRemaining = lines.Count(l => !l.StartsWith('#') && l.Contains(':'));
        if (accountsRemaining > 0)
        {
            await File.WriteAllLinesAsync(HtpasswdFilePath, lines).ConfigureAwait(false);
        }
        else
        {
            // show error?
        }
    }

    /// <inheritdoc />
    protected override Task CreateUserAsync(string user, string password)
    {
        return File.AppendAllLinesAsync(HtpasswdFilePath, new[] { this.GenerateEntry(user, password) });
    }

    /// <inheritdoc />
    protected override async Task ChangePasswordAsync(string user, string newPassword)
    {
        var userPrefix = $"{user}:";
        var lines = File.ReadAllLines(HtpasswdFilePath)!;
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line.StartsWith(userPrefix, StringComparison.InvariantCulture))
            {
                lines[i] = this.GenerateEntry(user, newPassword);
                await File.WriteAllLinesAsync(HtpasswdFilePath, lines).ConfigureAwait(false);
                break;
            }
        }
    }

    private string GenerateEntry(string user, string password)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        return $"{user}:{hash}";
    }
}
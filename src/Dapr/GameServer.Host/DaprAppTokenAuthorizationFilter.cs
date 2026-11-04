// <copyright file="DaprAppTokenAuthorizationFilter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.Host;

using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

/// <summary>Authenticates the sidecar before any live administration action or model binding.</summary>
public sealed class DaprAppTokenAuthorizationFilter : IAuthorizationFilter
{
    private readonly byte[]? _tokenHash;

    /// <summary>Initializes a new instance of the <see cref="DaprAppTokenAuthorizationFilter"/> class.</summary>
    /// <param name="configuration">The host configuration, including APP_API_TOKEN.</param>
    public DaprAppTokenAuthorizationFilter(IConfiguration configuration)
    {
        var token = configuration["APP_API_TOKEN"];
        this._tokenHash = string.IsNullOrWhiteSpace(token) ? null : SHA256.HashData(Encoding.UTF8.GetBytes(token));
    }

    /// <inheritdoc />
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var supplied = context.HttpContext.Request.Headers["dapr-api-token"];
        if (this._tokenHash is null || supplied.Count != 1 || string.IsNullOrEmpty(supplied[0])
            || !CryptographicOperations.FixedTimeEquals(this._tokenHash, SHA256.HashData(Encoding.UTF8.GetBytes(supplied[0]!))))
        {
            // Missing configuration also fails closed. Never return or log either credential.
            context.Result = new UnauthorizedResult();
        }
    }
}

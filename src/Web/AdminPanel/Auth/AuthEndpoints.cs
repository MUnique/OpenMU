// <copyright file="AuthEndpoints.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// The http endpoints which set and remove the authentication cookie.
/// </summary>
/// <remarks>
/// A cookie can only be set on a http response, which an interactive blazor component doesn't have.
/// These endpoints are therefore called by the browser in the background, with a single use ticket
/// the circuit issued after it validated the credentials.
/// </remarks>
public static class AuthEndpoints
{
    /// <summary>
    /// Maps the endpoints which set and remove the authentication cookie.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapAdminPanelAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
                AdminAuthenticationDefaults.SignInEndpointPath,
                async (SignInRequest request, HttpContext httpContext, SignInTicketService ticketService) =>
                {
                    if (!ticketService.TryRedeem(request.Ticket, out var ticket) || ticket is null)
                    {
                        return Results.Unauthorized();
                    }

                    var identity = new ClaimsIdentity(
                        ticket.Claims,
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        ClaimTypes.Name,
                        ClaimTypes.Role);
                    var properties = new AuthenticationProperties
                    {
                        IsPersistent = ticket.IsPersistent,
                    };

                    await httpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(identity),
                            properties)
                        .ConfigureAwait(false);
                    return Results.NoContent();
                })
            .AllowAnonymous()
            .DisableAntiforgery();

        endpoints.MapPost(
                AdminAuthenticationDefaults.SignOutEndpointPath,
                async (HttpContext httpContext) =>
                {
                    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
                    return Results.NoContent();
                })
            .AllowAnonymous()
            .DisableAntiforgery();

        return endpoints;
    }

    /// <summary>
    /// The request body of the sign in endpoint.
    /// </summary>
    /// <param name="Ticket">The single use ticket which was issued by the circuit.</param>
    public record SignInRequest(string Ticket);
}

// <copyright file="AdminOperationExceptionFilter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Returns only explicitly safe operational errors through the public HTTP API.</summary>
public sealed class AdminOperationExceptionFilter : IExceptionFilter
{
    /// <inheritdoc />
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is AdminOperationException error)
        {
            context.Result = new ObjectResult(new { error = error.Message })
            {
                StatusCode = error.Kind switch
                {
                    AdminOperationErrorKind.Forbidden => StatusCodes.Status403Forbidden,
                    AdminOperationErrorKind.InvalidInput => StatusCodes.Status400BadRequest,
                    AdminOperationErrorKind.Unavailable => StatusCodes.Status503ServiceUnavailable,
                    _ => StatusCodes.Status409Conflict,
                },
            };
            context.ExceptionHandled = true;
        }
    }
}

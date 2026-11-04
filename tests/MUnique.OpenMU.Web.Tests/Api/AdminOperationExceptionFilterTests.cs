// <copyright file="AdminOperationExceptionFilterTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Api;

using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using MUnique.OpenMU.Web.AdminPanel.API;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Verifies the public HTTP status and error body contract.</summary>
[TestFixture]
public sealed class AdminOperationExceptionFilterTests
{
    /// <summary>Conflict and uncertain outcomes remain distinguishable even though both use HTTP 409.</summary>
    [TestCase(AdminOperationErrorKind.Conflict, 409)]
    [TestCase(AdminOperationErrorKind.Uncertain, 409)]
    [TestCase(AdminOperationErrorKind.Forbidden, 403)]
    [TestCase(AdminOperationErrorKind.InvalidInput, 400)]
    [TestCase(AdminOperationErrorKind.Unavailable, 503)]
    public void ReturnsKindWithoutChangingMessageOrStatus(AdminOperationErrorKind kind, int status)
    {
        var context = new ExceptionContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), [])
        {
            Exception = new AdminOperationException("Safe operational message.", kind),
        };
        new AdminOperationExceptionFilter().OnException(context);
        Assert.That(context.ExceptionHandled, Is.True);
        var result = (ObjectResult)context.Result!;
        Assert.That(result.StatusCode, Is.EqualTo(status));
        var body = JsonSerializer.SerializeToElement(result.Value);
        Assert.That(body.GetProperty("error").GetString(), Is.EqualTo("Safe operational message."));
        Assert.That(body.GetProperty("kind").GetString(), Is.EqualTo(kind.ToString()));
    }
}

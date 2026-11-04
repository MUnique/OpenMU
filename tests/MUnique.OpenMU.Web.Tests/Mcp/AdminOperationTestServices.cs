// <copyright file="AdminOperationTestServices.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Constructs a real audit executor with an explicit test principal.</summary>
internal static class AdminOperationTestServices
{
    /// <summary>Creates an executor with a trusted test caller and in-memory audit storage.</summary>
    internal static AdminOperationExecutor CreateExecutor(string frontend = "panel", InMemoryAdminAuditRepository? audit = null, Func<AdminActor>? actor = null, bool demo = false, ILogger<AdminOperationExecutor>? logger = null)
    {
        var context = new Mock<IAdminOperationContext>();
        context.Setup(c => c.GetActorAsync(It.IsAny<CancellationToken>())).Returns(() => ValueTask.FromResult(actor?.Invoke()
            ?? new AdminActor("test-admin-id", "test-admin", frontend, [AdminRoles.Administrator])));
        return new AdminOperationExecutor(context.Object, audit ?? new InMemoryAdminAuditRepository(), new AdminWriteSettings { ReadOnly = false }, logger ?? NullLogger<AdminOperationExecutor>.Instance, new AdminAuditSettings { IsDemo = demo });
    }
}

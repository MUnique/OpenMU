// <copyright file="AdminToolExecution.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Mcp;

using System.Text.Json.Nodes;
using ModelContextProtocol;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Returns safe operational messages while leaving unexpected exception details to the SDK.</summary>
internal static class AdminToolExecution
{
    /// <summary>Executes the shared helper with the supplied values.</summary>
    /// <param name="operation">The operation delegate.</param>
    /// <returns>The safe result.</returns>
    internal static async ValueTask<JsonObject> ExecuteAsync(Func<ValueTask<JsonObject>> operation)
    {
        try
        {
            return await operation().ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is ArgumentException or AdminOperationException)
        {
            throw new McpException(ex.Message, ex);
        }
    }
}

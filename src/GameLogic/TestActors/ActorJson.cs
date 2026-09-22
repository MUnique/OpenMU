// <copyright file="ActorJson.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

using System.Text.Json;

/// <summary>
/// Writes the single-line JSON objects of the control protocol.
/// </summary>
public static class ActorJson
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = false,
    };

    /// <summary>
    /// Writes one response line.
    /// </summary>
    /// <param name="id">The request id to echo; may be <c>null</c>.</param>
    /// <param name="result">The result of the command.</param>
    /// <returns>The JSON line, without the newline.</returns>
    public static string WriteResponse(string? id, ActorCommandResult result)
    {
        var response = new Dictionary<string, object?> { ["ok"] = result.Ok };
        if (id is not null)
        {
            response["id"] = id;
        }

        if (!result.Ok)
        {
            response["code"] = result.Code;
            response["error"] = result.Error;
        }

        foreach (var field in result.Fields)
        {
            response[field.Name] = field.Value;
        }

        return JsonSerializer.Serialize(response, Options);
    }

    /// <summary>
    /// Writes one event line of a followed stream.
    /// </summary>
    /// <param name="actorEvent">The event.</param>
    /// <returns>The JSON line, without the newline.</returns>
    public static string WriteEvent(ActorEvent actorEvent)
        => JsonSerializer.Serialize(new Dictionary<string, object?> { ["event"] = ToDictionary(actorEvent) }, Options);

    /// <summary>
    /// Flattens an event into the object the protocol writes: sequence number, timestamp, type and
    /// then its own fields.
    /// </summary>
    /// <param name="actorEvent">The event.</param>
    /// <returns>The dictionary.</returns>
    public static Dictionary<string, object?> ToDictionary(ActorEvent actorEvent)
    {
        var result = new Dictionary<string, object?>
        {
            ["seq"] = actorEvent.Seq,
            ["utc"] = actorEvent.Utc.ToString("O"),
            ["type"] = actorEvent.Type,
        };

        foreach (var field in actorEvent.Fields)
        {
            result[field.Name] = field.Value;
        }

        return result;
    }
}

// <copyright file="DaprClientExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using System.Threading;
using global::Dapr.Client;

/// <summary>
/// Extensions for the <see cref="DaprClient"/>.
/// </summary>
public static class DaprClientExtensions
{
    /// <summary>
    /// The name of the pub/sub component.
    /// </summary>
    public const string PubSubName = "pubsub";

    /// <summary>
    /// The time after which a published command expires, when it wasn't delivered until then.
    /// </summary>
    /// <remarks>
    /// Without it, a command for a process which is currently down would be delivered when it's
    /// up again, e.g. a restart command would restart it right after it got restarted.
    /// </remarks>
    private static readonly Dictionary<string, string> CommandMetadata = new() { { "ttlInSeconds", "30" } };

    /// <summary>
    /// Publishes a command, which expires when it can't be delivered in time.
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    /// <param name="daprClient">The dapr client.</param>
    /// <param name="topicName">Name of the topic.</param>
    /// <param name="data">The data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static Task PublishCommandAsync<TData>(this DaprClient daprClient, string topicName, TData data, CancellationToken cancellationToken = default)
    {
        return daprClient.PublishEventAsync(PubSubName, topicName, data, CommandMetadata, cancellationToken);
    }
}

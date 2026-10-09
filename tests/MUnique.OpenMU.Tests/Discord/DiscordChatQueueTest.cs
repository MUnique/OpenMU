// <copyright file="DiscordChatQueueTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Discord.ChatBridge;

/// <summary>
/// Tests for the <see cref="DiscordChatQueue"/>.
/// </summary>
[TestFixture]
public class DiscordChatQueueTest
{
    /// <summary>
    /// Tests that messages which come in while one is sent are combined, but not beyond the maximum length of Discord.
    /// </summary>
    [Test]
    public async Task MessagesAreCombinedUpToMaximumLengthAsync()
    {
        var messenger = new BlockingMessenger();
        await using (var queue = new DiscordChatQueue(1, messenger, NullLogger.Instance, 100))
        {
            queue.Enqueue("first");
            await messenger.FirstSendStarted.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            queue.Enqueue("second");
            queue.Enqueue("third");
            queue.Enqueue(new string('x', DiscordChatQueue.MaximumMessageLength - 5));
            messenger.Release.SetResult();
            await messenger.WaitForTextsAsync(3).ConfigureAwait(false);
        }

        Assert.That(messenger.Texts, Has.Count.EqualTo(3));
        Assert.That(messenger.Texts[0], Is.EqualTo("first"));
        Assert.That(messenger.Texts[1], Is.EqualTo("second\nthird"));
        Assert.That(messenger.Texts[2], Has.Length.EqualTo(DiscordChatQueue.MaximumMessageLength - 5));
    }

    private sealed class BlockingMessenger : IDiscordChatMessenger
    {
        public TaskCompletionSource FirstSendStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public List<string> Texts { get; } = new();

        public async Task SendTextAsync(ulong channelId, string text, CancellationToken cancellationToken)
        {
            lock (this.Texts)
            {
                this.Texts.Add(text);
            }

            this.FirstSendStarted.TrySetResult();
            await this.Release.Task.ConfigureAwait(false);
        }

        public async Task WaitForTextsAsync(int count)
        {
            var deadline = DateTime.UtcNow.AddSeconds(5);
            while (this.Texts.Count < count && DateTime.UtcNow < deadline)
            {
                await Task.Delay(10).ConfigureAwait(false);
            }
        }
    }
}

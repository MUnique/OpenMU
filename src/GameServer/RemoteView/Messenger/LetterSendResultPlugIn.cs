// <copyright file="LetterSendResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Messenger
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Messenger;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="ILetterSendResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("LetterSendResultPlugIn", "The default implementation of the ILetterSendResultPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("c67cad23-20ba-4cd7-ba4e-b672beed427c")]
    public class LetterSendResultPlugIn : ILetterSendResultPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="LetterSendResultPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public LetterSendResultPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void LetterSendResult(LetterSendSuccess success, uint letterId)
        {
            using var writer = this.player.Connection.StartSafeWrite(LetterSendResponse.HeaderType, LetterSendResponse.Length);
            _ = new LetterSendResponse(writer.Span)
            {
                LetterId = letterId,
                Result = Convert(success),
            };

            writer.Commit();
        }

        private static LetterSendResponse.LetterSendRequestResult Convert(LetterSendSuccess success)
        {
            return success switch
            {
                LetterSendSuccess.TryAgain => LetterSendResponse.LetterSendRequestResult.TryAgain,
                LetterSendSuccess.Success => LetterSendResponse.LetterSendRequestResult.Success,
                LetterSendSuccess.MailboxFull => LetterSendResponse.LetterSendRequestResult.MailboxFull,
                LetterSendSuccess.ReceiverNotExists => LetterSendResponse.LetterSendRequestResult.ReceiverNotExists,
                LetterSendSuccess.CantSendToYourself => LetterSendResponse.LetterSendRequestResult.CantSendToYourself,
                LetterSendSuccess.NotEnoughMoney => LetterSendResponse.LetterSendRequestResult.NotEnoughMoney,
                _ => throw new ArgumentException($"Unhandled case {success}."),
            };
        }
    }
}
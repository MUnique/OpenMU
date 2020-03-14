// <copyright file="ModalExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Blazored.Modal;
    using Blazored.Modal.Services;
    using MUnique.OpenMU.AdminPanel.Components;
    using MUnique.OpenMU.AdminPanel.Components.Form;

    /// <summary>
    /// Extension methods for the <see cref="IModalService"/>.
    /// </summary>
    public static class ModalExtensions
    {
        /// <summary>
        /// Shows a loading indicator in a modal dialog.
        /// </summary>
        /// <param name="modalService">The modal service.</param>
        /// <returns>The disposable which closes the modal indicator.</returns>
        public static IDisposable ShowLoadingIndicator(this IModalService modalService)
        {
            var cts = new CancellationTokenSource();
            var modalOptions = new ModalOptions
            {
                DisableBackgroundCancel = true,
                HideCloseButton = true,
                HideHeader = true,
            };
            var modalParameters = new ModalParameters();
            modalParameters.Add(nameof(ModalLoadingIndicator.CancellationToken), cts.Token);
            modalService.Show<ModalLoadingIndicator>(string.Empty, modalParameters, modalOptions);

            return new DisposeWrapper(() =>
            {
                cts.Cancel();
                cts.Dispose();
            });
        }

        /// <summary>
        /// Shows a message in a modal dialog.
        /// </summary>
        /// <param name="modalService">The modal service.</param>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        public static Task ShowMessageAsync(this IModalService modalService, string title, string message)
        {
            var messageParams = new ModalParameters();
            messageParams.Add(nameof(ModalMessage.Text), message);
            var modal = modalService.Show<ModalMessage>(title, messageParams);
            return modal.Result;
        }
    }
}
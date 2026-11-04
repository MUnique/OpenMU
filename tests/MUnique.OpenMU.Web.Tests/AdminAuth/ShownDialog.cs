// <copyright file="ShownDialog.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using MUnique.OpenMU.Web.Shared.Components.Modal;

/// <summary>
/// A dialog which a dialog service of a test has shown.
/// </summary>
/// <param name="ComponentType">The type of the component of the dialog.</param>
/// <param name="Title">The title of the dialog.</param>
/// <param name="Parameters">The parameters of the dialog.</param>
/// <param name="Options">The options of the dialog.</param>
internal sealed record ShownDialog(Type ComponentType, string Title, ModalParameters? Parameters, ModalOptions? Options);

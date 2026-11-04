// <copyright file="AuthenticatorSetup.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// The data which is needed to set an authenticator app up.
/// </summary>
/// <param name="SharedKey">The shared key, formatted in groups of four characters for manual entry.</param>
/// <param name="AuthenticatorUri">The otpauth uri which is encoded in the QR code.</param>
/// <param name="QrCodeSvg">The QR code as inline SVG.</param>
public record AuthenticatorSetup(string SharedKey, string AuthenticatorUri, string QrCodeSvg);

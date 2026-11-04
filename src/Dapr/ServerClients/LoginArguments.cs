// <copyright file="LoginArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for an account login.
/// </summary>
public record LoginArguments(string AccountName, byte ServerId);
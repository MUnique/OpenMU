// <copyright file="AdminResourceNotFoundException.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>A requested admin resource does not exist; other argument failures are not missing resources.</summary>
/// <param name="message">The safe missing-resource message.</param>
/// <param name="paramName">The parameter identifying the missing resource.</param>
public sealed class AdminResourceNotFoundException(string message, string paramName) : ArgumentException(message, paramName);

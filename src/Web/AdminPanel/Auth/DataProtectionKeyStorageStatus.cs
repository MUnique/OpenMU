// <copyright file="DataProtectionKeyStorageStatus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// The result of setting the storage of the data protection key ring up.
/// </summary>
/// <param name="Path">The path at which the keys should be stored.</param>
/// <param name="Error">The error which prevented the usage of that path; <c>null</c>, if it works.</param>
/// <remarks>
/// The key ring protects the authentication cookies and the stored authenticator keys. When it
/// can't be persisted, the admin panel still works - but everybody is signed out after a restart
/// and the stored authenticator keys become unreadable. That's worth a warning, but it's not worth
/// taking the whole panel down for, which is what an exception at this point would do.
/// </remarks>
public record DataProtectionKeyStorageStatus(string Path, Exception? Error);

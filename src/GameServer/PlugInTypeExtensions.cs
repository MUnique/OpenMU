﻿// <copyright file="PlugInTypeExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Extensions for plugin types.
    /// </summary>
    public static class PlugInTypeExtensions
    {
        /// <summary>
        /// Determines whether the given plug in is suitable for the specified client version.
        /// </summary>
        /// <param name="clientVersion">The client version.</param>
        /// <param name="plugInType">Type of the plug in.</param>
        /// <returns>
        ///   <c>true</c> if the given plug in is suitable for the specified client version; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPlugInSuitable(this ClientVersion clientVersion, Type plugInType)
        {
            var clientAttribute = plugInType.GetCustomAttribute(typeof(ClientAttribute)) as ClientAttribute;
            return ReferenceEquals(clientAttribute, null) // if the plugin doesn't specify a version, it always suits us
                   || clientVersion.CompareTo(clientAttribute.Client) >= 0;
        }
    }
}
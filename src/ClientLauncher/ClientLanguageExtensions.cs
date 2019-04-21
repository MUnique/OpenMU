// <copyright file="ClientLanguageExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ClientLauncher
{
    using System;

    /// <summary>
    /// Extensions for <see cref="ClientLanguage"/>.
    /// </summary>
    public static class ClientLanguageExtensions
    {
        /// <summary>
        /// The language keys used in the registry.
        /// </summary>
        private static readonly string[] LanguageKeys = { "Eng", "Por", "Spn" };

        /// <summary>
        /// Gets the string which should be set in the registry for the given <see cref="ClientLanguage"/>.
        /// </summary>
        /// <param name="clientLanguage">The client language.</param>
        /// <returns>The string which should be set in the registry for the given <see cref="ClientLanguage"/>.</returns>
        public static string GetString(this ClientLanguage clientLanguage)
        {
            var index = (int)clientLanguage;
            return index < LanguageKeys.Length ? LanguageKeys[index] : null;
        }

        /// <summary>
        /// Gets the <see cref="ClientLanguage"/> based on the given key, which is set in the registry.
        /// </summary>
        /// <param name="languageKey">The language key.</param>
        /// <returns>The <see cref="ClientLanguage"/> based on the given key, which is set in the registry.</returns>
        public static ClientLanguage GetLanguage(this string languageKey)
        {
            return (ClientLanguage)Array.IndexOf(LanguageKeys, languageKey);
        }
    }
}
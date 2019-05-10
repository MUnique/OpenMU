// <copyright file="ClientLanguage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PlugIns
{
    /// <summary>
    /// The language of the client.
    /// Webzen released clients to different countries, with slightly different network protocol, mostly different header values.
    /// </summary>
    public enum ClientLanguage
    {
        /// <summary>
        /// Invariant client language. A plugin marked with this value is compatible with any client.
        /// </summary>
        Invariant,

        /// <summary>
        /// The english client language. A plugin marked with this value is compatible only with the english (GMO) client.
        /// </summary>
        English,

        /// <summary>
        /// The japanese client language. A plugin marked with this value is compatible only with the japanese client.
        /// </summary>
        Japanese,

        /// <summary>
        /// The vietnamese client language. A plugin marked with this value is compatible only with the vietnamese client.
        /// </summary>
        Vietnamese,

        /// <summary>
        /// The filipino client language. A plugin marked with this value is compatible only with the filipino client.
        /// </summary>
        Filipino,

        /// <summary>
        /// The chinese client language. A plugin marked with this value is compatible only with the chinese client.
        /// </summary>
        Chinese,

        /// <summary>
        /// The korean client language. A plugin marked with this value is compatible only with the korean client.
        /// </summary>
        Korean,

        /// <summary>
        /// The thai client language. A plugin marked with this value is compatible only with the thai client.
        /// </summary>
        Thai,
    }
}
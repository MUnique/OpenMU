// <copyright file="ClientResolution.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ClientLauncher
{
    /// <summary>
    /// The available mu online client screen resolutions.
    /// </summary>
    public enum ClientResolution
    {
        /// <summary>
        /// The undefined resolution. Will fall back to 800x600 pixels.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The resolution of 800x600 pixels.
        /// </summary>
        Resolution800X600 = 1,

        /// <summary>
        /// The resolution of 1024x768 pixels.
        /// </summary>
        Resolution1024X768 = 2,

        /// <summary>
        /// The resolution of 1280x1024 pixels.
        /// </summary>
        Resolution1280X1024 = 3,
    }
}
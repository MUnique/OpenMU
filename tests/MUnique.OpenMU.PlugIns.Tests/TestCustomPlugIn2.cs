// <copyright file="TestCustomPlugIn2.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns.Tests
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// A second test implementation of <see cref="ITestCustomPlugIn"/>.
    /// </summary>
    [PlugIn(nameof(TestCustomPlugIn2), "")]
    [Guid("9431C449-1F0C-47C1-BE5D-F9E356090DAB")]
    public class TestCustomPlugIn2 : ITestCustomPlugIn
    {
    }
}
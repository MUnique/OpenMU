// <copyright file="TestCustomPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns.Tests
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// A test implementation of <see cref="ITestCustomPlugIn"/>.
    /// </summary>
    [PlugIn(nameof(TestCustomPlugIn), "")]
    [Guid("77CF382A-2F87-4642-889A-85BF6D76E218")]
    public class TestCustomPlugIn : ITestCustomPlugIn
    {
    }
}
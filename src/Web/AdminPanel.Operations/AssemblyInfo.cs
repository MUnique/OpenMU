// <copyright file="AssemblyInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Runtime.CompilerServices;

// Authorization attributes need compile-time policy names. Keep these constants internal
// and share them only with the admin front ends and their tests.
[assembly: InternalsVisibleTo("MUnique.OpenMU.Web.AdminPanel")]
[assembly: InternalsVisibleTo("MUnique.OpenMU.Web.AdminPanel.Mcp")]
[assembly: InternalsVisibleTo("MUnique.OpenMU.Web.Tests")]

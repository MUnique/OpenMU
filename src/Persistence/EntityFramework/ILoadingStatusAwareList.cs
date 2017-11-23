// <copyright file="ILoadingStatusAwareList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System.Collections;

    /// <summary>
    /// Interface for a loading status aware list.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Persistence.EntityFramework.ILoadingStatusAware" />
    /// <seealso cref="System.Collections.IList" />
    internal interface ILoadingStatusAwareList : ILoadingStatusAware, IList
    {
    }
}
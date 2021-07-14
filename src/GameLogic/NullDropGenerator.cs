// <copyright file="NullDropGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// A drop generator which generates nothing.
    /// </summary>
    public class NullDropGenerator : IDropGenerator
    {
        private static IDropGenerator? instance;

        /// <summary>
        /// Prevents a default instance of the <see cref="NullDropGenerator"/> class from being created.
        /// </summary>
        private NullDropGenerator()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static IDropGenerator Instance => instance ??= new NullDropGenerator();

        /// <inheritdoc />
        public IEnumerable<Item> GenerateItemDrops(MonsterDefinition monster, int gainedExperience, Player player, out uint? droppedMoney)
        {
            droppedMoney = null;
            return Enumerable.Empty<Item>();
        }
    }
}
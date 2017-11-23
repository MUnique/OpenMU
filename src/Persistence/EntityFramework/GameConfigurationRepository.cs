// <copyright file="GameConfigurationRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System.Collections;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The game configuration repository. It just fills the experience table, because the entity framework can't map arrays.
    /// And instead of loading dependent data by calling other repositories, this repository uses the built-in functions of
    /// the entity framework. That's a bit faster, and we don't need all of these repositories.
    /// </summary>
    internal class GameConfigurationRepository : GenericRepository<GameConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameConfigurationRepository"/> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        public GameConfigurationRepository(IRepositoryManager repositoryManager)
            : base(repositoryManager)
        {
        }

        /// <summary>
        /// Loads the dependent data of the object from the corresponding repositories.
        /// Instead of loading dependent data by calling other repositories, like the base does,
        /// this repository uses the built-in functions of the entity framework.
        /// That's a bit faster and we don't need all of these repositories.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="currentContext">The current context with which the object got loaded. It is neccessary to retrieve the foreign key ids.</param>
        protected override void LoadDependentData(object obj, DbContext currentContext)
        {
            var entityEntry = currentContext.Entry(obj);
            foreach (var navigation in entityEntry.Navigations)
            {
                if (navigation.IsLoaded)
                {
                    continue;
                }

                navigation.Load();
                if (navigation.CurrentValue != null)
                {
                    if (navigation.CurrentValue is IEnumerable enumerable)
                    {
                        this.LoadDependentData(enumerable, currentContext);
                    }
                    else
                    {
                        this.LoadDependentData(navigation.CurrentValue, currentContext);
                    }
                }
            }

            foreach (var collection in entityEntry.Collections)
            {
                if (collection.IsLoaded)
                {
                    continue;
                }

                collection.Load();
                if (collection.CurrentValue != null)
                {
                    this.LoadDependentData(collection.CurrentValue, currentContext);
                }
            }

            this.SetExperienceTables(obj as GameConfiguration);
        }

        private void SetExperienceTables(GameConfiguration gameConfiguration)
        {
            if (gameConfiguration != null)
            {
                gameConfiguration.ExperienceTable =
                    Enumerable.Range(0, gameConfiguration.MaximumLevel + 1)
                        .Select(level => this.CalculateNeededExperience(level))
                        .ToArray();
                gameConfiguration.MasterExperienceTable =
                    Enumerable.Range(0, 201).Select(level => this.CalcNeededMasterExp(level)).ToArray();
            }
        }

        private long CalcNeededMasterExp(long lvl)
        {
            // f(x) = 505 * x^3 + 35278500 * x + 228045 * x^2
            return (505 * lvl * lvl * lvl) + (35278500 * lvl) + (228045 * lvl * lvl);
        }

        private long CalculateNeededExperience(long level)
        {
            if (level == 0)
            {
                return 0;
            }

            if (level < 256)
            {
                return 10 * (level + 8) * (level - 1) * (level - 1);
            }

            return (10 * (level + 8) * (level - 1) * (level - 1)) + (1000 * (level - 247) * (level - 256) * (level - 256));
        }
    }
}

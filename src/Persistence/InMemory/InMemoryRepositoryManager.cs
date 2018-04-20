// <copyright file="InMemoryRepositoryManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory
{
    using System;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Persistence.EntityFramework;

    /// <summary>
    /// A repository manager which uses in-memory repositories, e.g. for testing or demo purposes.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Persistence.BaseRepositoryManager" />
    public class InMemoryRepositoryManager : BaseRepositoryManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryManager"/> class.
        /// </summary>
        public InMemoryRepositoryManager()
        {
            this.RegisterRepository(new MemoryRepository<GameConfiguration>());
            var accountRepository = new AccountRepository();
            this.RegisterRepository(accountRepository);
            this.RegisterRepository(new FriendViewItemRepository(accountRepository));
            this.RegisterRepository(new LetterBodyRepository());
        }

        /// <inheritdoc />
        public override IRepository<T> GetRepository<T>()
        {
            return this.InternalGetRepository(typeof(T)) as IRepository<T>
                   ?? this.CreateAndRegisterMemoryRepository<T>();
        }

        /// <inheritdoc/>
        public override T CreateNew<T>(params object[] args)
        {
            var newObject = TypeHelper.CreateNew<T>(args);
            if (newObject is IIdentifiable identifiable)
            {
                if (identifiable.Id == Guid.Empty)
                {
                    identifiable.Id = Guid.NewGuid();
                }

                var repository = this.GetRepository<T>() as MemoryRepository<T>;
                repository?.Add(identifiable.Id, newObject);
            }

            return newObject;
        }

        private IRepository<T> CreateAndRegisterMemoryRepository<T>()
        {
            var repository = new MemoryRepository<T>();
            this.RegisterRepository(repository);
            return repository;
        }
    }
}

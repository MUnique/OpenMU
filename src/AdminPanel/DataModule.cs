// <copyright file="DataModule.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Persistence;
    using Nancy;
    using Nancy.ModelBinding;
    using Newtonsoft.Json;

    /// <summary>
    /// A module which provides data.
    /// </summary>
    public class DataModule : NancyModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DataModule));
        private readonly IRepositoryManager repositoryManager;
        private readonly IList<string> registeredRepositoryTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataModule"/> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        public DataModule(IRepositoryManager repositoryManager)
            : base("/data")
        {
            this.repositoryManager = repositoryManager;
            this.registeredRepositoryTypes = new List<string>();
            this.Get["/registered"] = _ => this.Response.AsJson(this.registeredRepositoryTypes);

            this.RegisterRepository(repositoryManager.GetRepository<GameConfiguration>());

            // TODO: add authentification
        }

        private void RegisterRepository<T>(IRepository<T> repository)
            where T : class
        {
            var className = typeof(T).Name;
            this.Get[className] = _ =>
                {
                    var allData = repository.GetAll();
                    var list = JsonConvert.SerializeObject(
                        allData,
                        Formatting.None,
                        new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                    return list;
                };
            this.Get[className + "/{id:guid}"] = _ => this.Response.AsJson(repository.GetById((Guid)_.id));
            this.Get[className + "/enums"] = _ => this.GetEnums<T>();
            this.Post[className] = _ => this.Save(repository);
            this.Post[className + "/all"] = _ => this.SaveAll(repository);
            this.Delete[className] = _ => repository.Delete(this.Bind<T>());
            this.Delete[className + "/{id:guid}"] = _ => repository.Delete((Guid)_.id);

            this.registeredRepositoryTypes.Add(className);
        }

        private object GetEnums<T>()
        {
            var enumProperties = typeof(T).GetTypeInfo().GetProperties().Where(p => p.PropertyType.GetTypeInfo().IsEnum);
            var result = enumProperties.Select(e => new { PropertyName = e.Name, TypeName = e.PropertyType.Name, Values = e.PropertyType.GetTypeInfo().GetEnumNames().Select((name, i) => new { Name = name, Value = e.PropertyType.GetTypeInfo().GetEnumValues().GetValue(i) }) });
            return result;
        }

        private dynamic SaveAll<T>(IRepository<T> repository)
            where T : class
        {
            try
            {
                var foo = this.Bind<IEnumerable<T>>();
                using (var context = this.repositoryManager.UseTemporaryContext())
                {
                    foreach (T item in foo)
                    {
                        this.UpdateOrCreateObject(repository, item);
                    }

                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return false;
            }
        }

        private dynamic Save<T>(IRepository<T> repository)
            where T : class
        {
            byte[] body = new byte[this.Request.Body.Length];
            this.Request.Body.Position = 0;
            this.Request.Body.Read(body, 0, body.Length);
            var bodyString = System.Text.Encoding.UTF8.GetString(body);
            Log.Debug(bodyString);

            try
            {
                using (var context = this.repositoryManager.UseTemporaryContext())
                {
                    var item = this.Bind<T>();
                    this.UpdateOrCreateObject(repository, item);
                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return false;
            }
        }

        private void UpdateOrCreateObject<T>(IRepository<T> repository, T item)
            where T : class
        {
            var itemId = item.GetId();
            T persistentItem = itemId != Guid.Empty ? repository.GetById(itemId) : this.repositoryManager.CreateNew<T>();

            FieldInfo[] myObjectFields = item.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo fi in myObjectFields)
            {
                fi.SetValue(persistentItem, fi.GetValue(item));
            }
        }
}
}
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
    using MUnique.OpenMU.Persistence.Json;
    using Nancy;
    using Nancy.ModelBinding;
    using Newtonsoft.Json;

    /// <summary>
    /// A module which provides data.
    /// </summary>
    public class DataModule : NancyModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DataModule));
        private readonly IPersistenceContextProvider persistenceContextProvider;
        private readonly IList<string> registeredRepositoryTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataModule"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public DataModule(IPersistenceContextProvider persistenceContextProvider)
            : base("/data")
        {
            this.persistenceContextProvider = persistenceContextProvider;
            this.registeredRepositoryTypes = new List<string>();
            this.Get("/registered", _ => this.Response.AsJson(this.registeredRepositoryTypes));
            this.Get("/gameconfiguration.json", _ =>
            {
                using (var context = this.persistenceContextProvider.CreateNewContext())
                {
                    var firstConfiguration = context.Get<GameConfiguration>().First();
                    if (firstConfiguration is IConvertibleTo<Persistence.BasicModel.GameConfiguration> convertibleTo)
                    {
                        return convertibleTo.Convert().ToJson();
                    }

                    return null;
                }
            });
            this.Get("/{id:guid}/gameconfiguration.json", parameters =>
            {
                using (var context = this.persistenceContextProvider.CreateNewContext())
                {
                    var configuration = context.GetById<GameConfiguration>((Guid)parameters.id);
                    if (configuration is IConvertibleTo<Persistence.BasicModel.GameConfiguration> convertibleTo)
                    {
                        return convertibleTo.Convert().ToJson();
                    }

                    return null;
                }
            });

            this.RegisterType<GameMapDefinition>();
            //// TODO: add authentification
        }

        private void RegisterType<T>()
            where T : class
        {
            var className = typeof(T).Name;
            this.Get(className, _ =>
            {
                var context = this.persistenceContextProvider.CreateNewContext();
                var allData = context.Get<T>();
                var list = JsonConvert.SerializeObject(
                    allData,
                    Formatting.None,
                    new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                return list;
            });
            this.Get(className + "/{id:guid}", _ => FormatterExtensions.AsJson<T>(this.Response, this.GetById<T>((Guid)_.id)));
            this.Get(className + "/enums", _ => this.GetEnums<T>());
            this.Post(className, _ => this.Save<T>());
            this.Post(className + "/all", _ => this.SaveAll<T>());
            this.Delete(className, _ => this.DeleteItem(this.Bind<T>()));
            this.Delete(className + "/{id:guid}", _ => this.DeleteItemById<T>((Guid)_.id));

            this.registeredRepositoryTypes.Add(className);
        }

        private object GetEnums<T>()
        {
            var enumProperties = typeof(T).GetTypeInfo().GetProperties().Where(p => p.PropertyType.GetTypeInfo().IsEnum);
            var result = enumProperties.Select(e => new { PropertyName = e.Name, TypeName = e.PropertyType.Name, Values = e.PropertyType.GetTypeInfo().GetEnumNames().Select((name, i) => new { Name = name, Value = e.PropertyType.GetTypeInfo().GetEnumValues().GetValue(i) }) });
            return result;
        }

        private dynamic SaveAll<T>()
            where T : class
        {
            try
            {
                var foo = this.Bind<IEnumerable<T>>();
                using (var context = this.persistenceContextProvider.CreateNewContext())
                {
                    foreach (T item in foo)
                    {
                        this.UpdateOrCreateObject(context, item);
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

        private dynamic Save<T>()
            where T : class
        {
            byte[] body = new byte[this.Request.Body.Length];
            this.Request.Body.Position = 0;
            this.Request.Body.Read(body, 0, body.Length);
            var bodyString = System.Text.Encoding.UTF8.GetString(body);
            Log.Debug(bodyString);

            try
            {
                using (var context = this.persistenceContextProvider.CreateNewContext())
                {
                    var item = this.Bind<T>();
                    this.UpdateOrCreateObject(context, item);
                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return false;
            }
        }

        private dynamic GetById<T>(Guid id)
            where T : class
        {
            try
            {
                using (var context = this.persistenceContextProvider.CreateNewContext())
                {
                    return context.GetById<T>(id);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return false;
            }
        }

        private dynamic DeleteItemById<T>(Guid id)
            where T : class
        {
            try
            {
                using (var context = this.persistenceContextProvider.CreateNewContext())
                {
                    context.Delete(context.GetById<T>(id));
                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return false;
            }
        }

        private dynamic DeleteItem<T>(T item)
            where T : class
        {
            try
            {
               using (var context = this.persistenceContextProvider.CreateNewContext())
               {
                   context.Delete(item);
                   return context.SaveChanges();
               }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return false;
            }
        }

        private void UpdateOrCreateObject<T>(IContext context, T item)
            where T : class
        {
            var itemId = item.GetId();
            T persistentItem = itemId != Guid.Empty ? context.GetById<T>(itemId) : context.CreateNew<T>();

            FieldInfo[] myObjectFields = item.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo fi in myObjectFields)
            {
                fi.SetValue(persistentItem, fi.GetValue(item));
            }
        }
}
}
// <copyright file="DefaultJavaScriptConverter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using Nancy.Json;

    /// <summary>
    /// A javascript converter which can convert generic dictionaries,
    /// with considering the "Id"-Properties which are used as keys.
    /// </summary>
    public abstract class DefaultJavaScriptConverter : JavaScriptConverter
    {
        /// <inheritdoc/>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            var result = Activator.CreateInstance(type);
            foreach (var kvp in dictionary)
            {
                var propertyInfo = type.GetProperty(kvp.Key);
                if (propertyInfo != null)
                {
                    var convertMethod = serializer.GetType().GetMethod("ConvertToType"); // JavaScriptSerializer.ConvertToType
                    if (convertMethod != null)
                    {
                        var value = convertMethod.MakeGenericMethod(propertyInfo.PropertyType).Invoke(serializer, new[] { kvp.Value });
                        propertyInfo.SetMethod.Invoke(result, new[] { value });
                    }
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var type = obj.GetType().GetTypeInfo();
            var result = new Dictionary<string, object>();
            if (obj is IEnumerable && type.GetTypeInfo().IsGenericType && type.GetGenericArguments().Length == 2)
            {
                var dictionary = (dynamic)obj;
                foreach (var key in dictionary.Keys)
                {
                    var value = this.Serialize(dictionary[key], serializer);
                    var id = key.GetId();
                    result.Add(id != Guid.Empty ? id.ToString() : serializer.Serialize(key), value);
                }

                return result;
            }

            var properties = type.GetProperties();

            foreach (var prop in properties)
            {
                var val = prop.GetMethod.Invoke(obj, new object[0]);
                result.Add(prop.Name, val);
            }

            return result;
        }
    }
}

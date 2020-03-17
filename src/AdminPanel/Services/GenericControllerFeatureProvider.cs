// <copyright file="GenericControllerFeatureProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// A feature to create generic <see cref="JsonDownloadController{T,TSerializable}"/> for supported types.
    /// </summary>
    public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        /// <summary>
        /// Gets the pair of types which are supported. For each of them, a <see cref="JsonDownloadController{T,TSerializable}"/> is created.
        /// </summary>
        public static (Type, Type)[] SupportedTypes { get; } =
        {
            (typeof(GameConfiguration), typeof(Persistence.BasicModel.GameConfiguration)),
            (typeof(Account), typeof(Persistence.BasicModel.Account)),
        };

        /// <inheritdoc />
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var typePair in SupportedTypes)
            {
                var controllerType = typeof(JsonDownloadController<,>).MakeGenericType(typePair.Item1, typePair.Item2).GetTypeInfo();
                feature.Controllers.Add(controllerType);
            }
        }
    }
}
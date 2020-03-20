// <copyright file="GenericControllerNameAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services
{
    using System;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;

    /// <summary>
    /// Attribute which marks a generic controller and sets its name which affects the route.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class GenericControllerNameAttribute : Attribute, IControllerModelConvention
    {
        /// <inheritdoc />
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.GetGenericTypeDefinition() == typeof(JsonDownloadController<,>))
            {
                var entityType = controller.ControllerType.GenericTypeArguments[0];
                controller.ControllerName = entityType.Name;
            }
        }
    }
}
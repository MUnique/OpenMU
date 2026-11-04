// <copyright file="AccountDownloadAuthorizationConvention.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Requires the <see cref="AdminRole.Operator"/> role for the json download of accounts.
/// </summary>
/// <remarks>
/// The <see cref="JsonDownloadController{T,TSerializable}"/> itself only requires a login, which is enough
/// for the game configuration. An account contains its password hash, its security code and its vault
/// password, though, and only an operator may edit accounts.
/// </remarks>
internal sealed class AccountDownloadAuthorizationConvention : IControllerModelConvention
{
    /// <inheritdoc />
    public void Apply(ControllerModel controller)
    {
        var type = controller.ControllerType;
        if (!type.IsGenericType
            || type.GetGenericTypeDefinition() != typeof(JsonDownloadController<,>)
            || type.GenericTypeArguments[0] != typeof(Account))
        {
            return;
        }

        foreach (var selector in controller.Actions.SelectMany(action => action.Selectors))
        {
            selector.EndpointMetadata.Add(new AuthorizeAttribute(AdminPolicies.Operator));
        }
    }
}

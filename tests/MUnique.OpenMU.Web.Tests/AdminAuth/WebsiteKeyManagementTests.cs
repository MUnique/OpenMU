// <copyright file="WebsiteKeyManagementTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using Microsoft.AspNetCore.Components;
using Moq;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// Tests that the functions of a website key are selected with check boxes in the admin panel.
/// </summary>
[TestFixture]
public sealed class WebsiteKeyManagementTests
{
    /// <summary>
    /// Creating a website key asks for its functions and stores the selected ones as its allowlist.
    /// </summary>
    [Test]
    public async Task CreatedWebsiteKeyGetsSelectedFunctionsAsync()
    {
        var users = new InMemoryAdminUserRepository();
        await users.AddAsync(AdminPanelSetupModeTests.CreateUser("admin")).ConfigureAwait(false);
        var keys = new InMemoryApiKeyRepository();
        var modals = new ScriptedModalService(
            item => SetProperty(item, "Name", "website", "Role", ApiKeyRole.WebManager),
            item => SetProperty(item, "Status", true, "Register", true));
        var service = new ApiKeyManagementService(keys, modals, Mock.Of<IToastService>(), AdminPanelSetupModeTests.CreateSetupMode(users, keys));

        var createdKey = await service.CreateNewInModalDialogAsync().ConfigureAwait(false);

        Assert.That(createdKey, Is.Not.Null);
        var stored = (await keys.GetAllAsync().ConfigureAwait(false)).Single();
        Assert.That(stored.Roles, Is.EqualTo(AdminRoles.WebManager));
        Assert.That(stored.AllowedToolGroups, Is.EqualTo($"{WebsitePermissions.Status},{WebsitePermissions.Register}"));
    }

    /// <summary>
    /// Changing a website key shows its current functions and stores the new selection.
    /// </summary>
    [Test]
    public async Task ChangedWebsiteKeyKeepsUnchangedFunctionsAsync()
    {
        var keys = new InMemoryApiKeyRepository();
        var key = new ApiKey { Id = Guid.NewGuid(), Name = "website", Roles = AdminRoles.WebManager, AllowedToolGroups = WebsitePermissions.Register };
        await keys.AddAsync(key).ConfigureAwait(false);
        var modals = new ScriptedModalService(item =>
        {
            Assert.That(item.GetType().GetProperty("Register")!.GetValue(item), Is.EqualTo(true));
            SetProperty(item, "Status", true);
        });
        var service = new ApiKeyManagementService(keys, modals, Mock.Of<IToastService>(), AdminPanelSetupModeTests.CreateSetupMode(new InMemoryAdminUserRepository(), keys));

        Assert.That(await service.EditToolGroupsAsync(key).ConfigureAwait(false), Is.True);
        Assert.That(key.AllowedToolGroups, Is.EqualTo($"{WebsitePermissions.Status},{WebsitePermissions.Register}"));
    }

    private static void SetProperty(object item, params object[] namesAndValues)
    {
        for (var i = 0; i < namesAndValues.Length; i += 2)
        {
            item.GetType().GetProperty((string)namesAndValues[i])!.SetValue(item, namesAndValues[i + 1]);
        }
    }

    /// <summary>
    /// A <see cref="IModalService"/> which fills each dialog like a user would, and confirms it.
    /// </summary>
    private sealed class ScriptedModalService(params Action<object>[] fillers) : IModalService
    {
        private int _shown;

        /// <summary>Gets the shown dialogs, with the arguments they were shown with.</summary>
        public List<(Type ComponentType, string Title, ModalParameters? Parameters, ModalOptions? Options)> Shown { get; } = [];

        /// <inheritdoc />
        public IModalReference Show<TComponent>(string title, ModalParameters? parameters = null, ModalOptions? options = null)
            where TComponent : class, IComponent
            => this.Show(typeof(TComponent), title, parameters, options);

        /// <inheritdoc />
        public IModalReference Show(Type componentType, string title, ModalParameters? parameters = null, ModalOptions? options = null)
        {
            this.Shown.Add((componentType, title, parameters, options));
            var item = parameters!.Single(p => p.Key == "Item").Value!;
            fillers[this._shown++](item);
            var reference = new Mock<IModalReference>();
            reference.Setup(r => r.Result).Returns(Task.FromResult(ModalResult.Ok(true)));
            return reference.Object;
        }
    }
}

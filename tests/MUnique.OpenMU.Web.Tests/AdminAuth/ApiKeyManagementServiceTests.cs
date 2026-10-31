// <copyright file="ApiKeyManagementServiceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using Microsoft.AspNetCore.Components;
using Moq;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

/// <summary>
/// Tests when the <see cref="ApiKeyManagementService"/> creates an API key.
/// </summary>
[TestFixture]
public sealed class ApiKeyManagementServiceTests
{
    /// <summary>
    /// Tests that no key is created while the admin panel has no user: in that setup mode, anybody
    /// who reaches the admin panel could create one, and it would keep working after the first
    /// user exists.
    /// </summary>
    [Test]
    public async Task NoKeyWithoutAUserAsync()
    {
        var keys = new InMemoryApiKeyRepository();
        var modals = new TestModalService();
        var toasts = new Mock<IToastService>();
        var service = new ApiKeyManagementService(keys, modals, toasts.Object, AdminPanelSetupModeTests.CreateSetupMode(new InMemoryAdminUserRepository(), keys));

        var createdKey = await service.CreateNewInModalDialogAsync().ConfigureAwait(false);

        Assert.That(createdKey, Is.Null);
        Assert.That(modals.ShownDialogs, Is.Empty);
        Assert.That(await keys.GetAllAsync().ConfigureAwait(false), Is.Empty);
        toasts.Verify(toast => toast.ShowError(It.IsAny<string>(), It.IsAny<string?>()), Times.Once);
    }

    /// <summary>
    /// Tests that a key is created once the admin panel has a user.
    /// </summary>
    [Test]
    public async Task KeyWithAUserAsync()
    {
        var users = new InMemoryAdminUserRepository();
        await users.AddAsync(AdminPanelSetupModeTests.CreateUser("admin")).ConfigureAwait(false);
        var keys = new InMemoryApiKeyRepository();
        var modals = new TestModalService();
        var service = new ApiKeyManagementService(keys, modals, Mock.Of<IToastService>(), AdminPanelSetupModeTests.CreateSetupMode(users, keys));

        var createdKey = await service.CreateNewInModalDialogAsync().ConfigureAwait(false);

        Assert.That(createdKey, Is.Not.Null);
        Assert.That(modals.ShownDialogs, Has.Count.EqualTo(1));
        Assert.That(await keys.GetAllAsync().ConfigureAwait(false), Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Tests that no key is created when the last user is gone while the dialog of the key is
    /// open: the check is repeated right before the key is stored, not only when the dialog opens.
    /// </summary>
    [Test]
    public async Task NoKeyWhenTheLastUserIsGoneWhileTheDialogIsOpenAsync()
    {
        var users = new InMemoryAdminUserRepository();
        var user = AdminPanelSetupModeTests.CreateUser("admin");
        await users.AddAsync(user).ConfigureAwait(false);
        var keys = new InMemoryApiKeyRepository();
        var dialog = new PendingDialog();
        var toasts = new Mock<IToastService>();
        var service = new ApiKeyManagementService(keys, dialog, toasts.Object, AdminPanelSetupModeTests.CreateSetupMode(users, keys));

        var creation = service.CreateNewInModalDialogAsync();
        Assert.That(dialog.Shown, Has.Count.EqualTo(1));
        Assert.That(creation.IsCompleted, Is.False);
        await users.DeleteAsync(user).ConfigureAwait(false);
        dialog.Confirm();
        var createdKey = await creation.ConfigureAwait(false);

        Assert.That(createdKey, Is.Null);
        Assert.That(await keys.GetAllAsync().ConfigureAwait(false), Is.Empty);
        toasts.Verify(toast => toast.ShowError(It.IsAny<string>(), It.IsAny<string?>()), Times.Once);
    }

    /// <summary>
    /// A dialog which stays open until the test confirms it.
    /// </summary>
    private sealed class PendingDialog : IModalService
    {
        private readonly TaskCompletionSource<ModalResult> _result = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public IList<ShownDialog> Shown { get; } = new List<ShownDialog>();

        public void Confirm() => this._result.SetResult(ModalResult.Ok(true));

        public IModalReference Show<TComponent>(string title, ModalParameters? parameters = null, ModalOptions? options = null)
            where TComponent : class, IComponent
        {
            return this.Show(typeof(TComponent), title, parameters, options);
        }

        public IModalReference Show(Type componentType, string title, ModalParameters? parameters = null, ModalOptions? options = null)
        {
            this.Shown.Add(new ShownDialog(componentType, title, parameters, options));
            return new Reference(this._result.Task);
        }

        private sealed class Reference : IModalReference
        {
            public Reference(Task<ModalResult> result)
            {
                this.Result = result;
            }

            public Task<ModalResult> Result { get; }
        }
    }
}

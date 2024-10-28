// <copyright file="ConfigurationChangeMediator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Collections.Concurrent;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A mediator which notifies about configuration changes for registered instances.
/// </summary>
public class ConfigurationChangeMediator : IConfigurationChangeMediator, IConfigurationChangeMediatorListener
{
    private readonly ConcurrentDictionary<Guid, IChangeRegistration> _registrations = new();
    private readonly ConcurrentDictionary<Type, ICreateRegistration> _createRegistrations = new();

    /// <summary>
    /// A non-generic interface for the change-registration.
    /// </summary>
    private interface IChangeRegistration : IDisposable
    {
        /// <summary>
        /// Raises the <see cref="ChangeRegistration{TConfig}"/> event.
        /// </summary>
        ValueTask RaiseOnChangeAsync();

        /// <summary>
        /// Raises the <see cref="ChangeRegistration{TConfig}"/> event.
        /// </summary>
        ValueTask RaiseOnDeleteAsync();
    }

    /// <summary>
    /// An non-generic interface for the <see cref="CreateRegistration{TConfig}"/>.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    private interface ICreateRegistration : IDisposable
    {
        /// <summary>
        /// Raises the <see cref="ChangeRegistration{TConfig}"/> event.
        /// </summary>
        /// <param name="created">The created configuration.</param>
        ValueTask RaiseOnCreateAsync(object created);
    }

    /// <inheritdoc />
    public IDisposable RegisterObject<TConfig, T>(TConfig config, T obj, Func<Action, TConfig, T, ValueTask>? onChange = null, Func<TConfig, T, ValueTask>? onDelete = null)
        where T : class
        where TConfig : class
    {
        var registration = (ChangeRegistration<TConfig>)this._registrations.AddOrUpdate(
            config.GetId(),
            _ => new ChangeRegistration<TConfig>(config),
            (_, value) => value);

        if (onChange is not null)
        {
            registration.OnChange += InvokeOnChangeAsync;
        }

        if (onDelete is not null)
        {
            registration.OnDelete += InvokeOnDeleteAsync;
        }

        var disposable = new Nito.Disposables.Disposable(() =>
        {
            if (onChange is not null)
            {
                registration.OnChange -= InvokeOnChangeAsync;
            }

            if (onDelete is not null)
            {
                registration.OnDelete -= InvokeOnDeleteAsync;
            }
        });

        return disposable;

        async ValueTask InvokeOnChangeAsync(TConfig changedConfig)
        {
            await onChange(
                () =>
                {
                    registration.OnChange -= InvokeOnChangeAsync;
                    if (onDelete is not null)
                    {
                        registration.OnDelete -= InvokeOnDeleteAsync;
                    }
                },
                changedConfig,
                obj).ConfigureAwait(false);
        }

        async ValueTask InvokeOnDeleteAsync(TConfig changedConfig)
        {
            await onDelete(changedConfig, obj).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public IDisposable RegisterForNew<TConfig, T>(T obj, Func<TConfig, T, ValueTask> onNewConfig)
    {
        var registration = (CreateRegistration<TConfig>)this._createRegistrations.AddOrUpdate(
            typeof(TConfig),
            _ => new CreateRegistration<TConfig>(),
            (_, value) => value);
        registration.OnCreate += InvokeOnCreateAsync;

        return registration;
        async ValueTask InvokeOnCreateAsync(TConfig config)
        {
            await onNewConfig(config, obj).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask HandleConfigurationChangedAsync(Type type, Guid id, object configuration)
    {
        if (this._registrations.TryGetValue(id, out var registration))
        {
            await registration.RaiseOnChangeAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask HandleConfigurationAddedAsync(Type type, Guid id, object configuration)
    {
        if (this._createRegistrations.TryGetValue(type, out var createRegistration)
            || this._createRegistrations.TryGetValue(type.BaseType!, out createRegistration))
        {
            await createRegistration.RaiseOnCreateAsync(configuration).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask HandleConfigurationRemovedAsync(Type type, Guid id)
    {
        if (this._registrations.Remove(id, out var registration))
        {
            await registration.RaiseOnDeleteAsync().ConfigureAwait(false);
            registration.Dispose();
        }
    }

    /// <summary>
    /// A registration for a change of a configuration.
    /// </summary>
    /// <typeparam name="TConfig">The type of the configuration.</typeparam>
    private class ChangeRegistration<TConfig> : Disposable, IChangeRegistration
        where TConfig : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeRegistration{TConfig}"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public ChangeRegistration(TConfig config)
        {
            this.Configuration = config;
        }

        /// <summary>
        /// Occurs when a config has been changed.
        /// </summary>
        public event AsyncEventHandler<TConfig>? OnChange;

        /// <summary>
        /// Occurs when a config has been deleted.
        /// </summary>
        public event AsyncEventHandler<TConfig>? OnDelete;

        /// <summary>
        /// Gets the configuration in which the registration is interested in.
        /// </summary>
        private TConfig Configuration { get; }

        /// <inheritdoc />
        public async ValueTask RaiseOnChangeAsync()
        {
            await this.OnChange.SafeInvokeAsync(this.Configuration).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async ValueTask RaiseOnDeleteAsync()
        {
            await this.OnDelete.SafeInvokeAsync(this.Configuration).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.OnDelete = null;
            this.OnChange = null;
        }
    }

    /// <summary>
    /// A registration for a creation of a configuration.
    /// </summary>
    /// <typeparam name="TConfig">The type of the configuration.</typeparam>
    private class CreateRegistration<TConfig> : Disposable, ICreateRegistration
    {
        /// <summary>
        /// Occurs when a new config of <typeparamref name="TConfig"/> is created.
        /// </summary>
        public event AsyncEventHandler<TConfig>? OnCreate;

        /// <inheritdoc />
        public async ValueTask RaiseOnCreateAsync(object created)
        {
            if (created is not TConfig typed)
            {
                throw new InvalidOperationException("created object is of wrong type");
            }

            await this.OnCreate.SafeInvokeAsync(typed).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.OnCreate = null;
        }
    }
}
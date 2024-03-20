// <copyright file="PlugInContainerBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

using Nito.AsyncEx;

/// <summary>
/// Base class for the implementation of plugin point proxies.
/// </summary>
/// <remarks>
/// It is used by the <see cref="PlugInProxyTypeGenerator"/> to implement proxies with Roslyn.
/// </remarks>
/// <typeparam name="TPlugIn">The type of the plug in.</typeparam>
/// <seealso cref="IPlugInContainer{TPlugIn}" />
public class PlugInContainerBase<TPlugIn> : IPlugInContainer<TPlugIn>
    where TPlugIn : class
{
    private readonly IList<TPlugIn> _knownPlugIns = new List<TPlugIn>();

    /// <summary>
    /// Initializes a new instance of the <see cref="PlugInContainerBase{TPlugIn}" /> class.
    /// </summary>
    /// <param name="manager">The plugin manager which manages this instance.</param>
    protected PlugInContainerBase(PlugInManager manager)
    {
        this.Manager = manager ?? throw new ArgumentNullException(nameof(manager));
        this.Manager.PlugInActivated += this.OnPlugInActivated;
        this.Manager.PlugInDeactivated += this.OnPlugInDeactivated;
        this.Manager.PlugInConfigurationChanged += this.OnPlugInConfigurationChanged;
    }

    /// <inheritdoc />
    IEnumerable<TPlugIn> IPlugInContainer<TPlugIn>.ActivePlugIns => this.ActivePlugIns;

    /// <summary>
    /// Gets the plugin manager which manages this instance.
    /// </summary>
    protected PlugInManager Manager { get; }

    /// <summary>
    /// Gets the reader writer lock which is used to add and remove plugins.
    /// </summary>
    protected AsyncReaderWriterLock Lock { get; } = new ();

    /// <summary>
    /// Gets the currently active plug ins.
    /// </summary>
    /// <value>
    /// The currently active plug ins.
    /// </value>
    protected IList<TPlugIn> ActivePlugIns { get; } = new List<TPlugIn>();

    /// <summary>
    /// Gets the known plug ins.
    /// </summary>
    protected IEnumerable<TPlugIn> KnownPlugIns
    {
        get
        {
            using var l = this.Lock.ReaderLock();
            return this._knownPlugIns.ToList();
        }
    }

    /// <inheritdoc />
    /// <exception cref="T:System.Threading.SynchronizationLockException">The current thread has not entered the lock in write mode.</exception>
    /// <exception cref="T:System.Threading.LockRecursionException">The <see cref="P:System.Threading.ReaderWriterLockSlim.RecursionPolicy"></see> property is <see cref="F:System.Threading.LockRecursionPolicy.NoRecursion"></see> and the current thread has already entered the lock in any mode.   -or-   The current thread has entered read mode, so trying to enter the lock in write mode would create the possibility of a deadlock.   -or-   The recursion number would exceed the capacity of the counter. The limit is so large that applications should never encounter it.</exception>
    public void AddPlugIn(TPlugIn plugIn, bool isActive)
    {
        using var l = this.Lock.WriterLock();
        this._knownPlugIns.Add(plugIn);
        if (isActive)
        {
            this.ActivatePlugIn(plugIn);
        }
    }

    /// <summary>
    /// Removes the plug in from the <see cref="KnownPlugIns"/>.
    /// </summary>
    /// <param name="plugIn">The plug in.</param>
    protected void RemovePlugIn(TPlugIn plugIn)
    {
        using var l = this.Lock.WriterLock();
        this._knownPlugIns.Remove(plugIn);
    }

    /// <summary>
    /// Activates the plug in.
    /// </summary>
    /// <param name="plugIn">The plug in.</param>
    protected virtual void ActivatePlugIn(TPlugIn plugIn)
    {
        this.ActivePlugIns.Add(plugIn);
    }

    /// <summary>
    /// Deactivates the plug in.
    /// </summary>
    /// <param name="plugIn">The plug in.</param>
    protected virtual void DeactivatePlugIn(TPlugIn plugIn)
    {
        this.ActivePlugIns.Remove(plugIn);
    }

    /// <summary>
    /// Is called before the plug in of the specified type gets activated.
    /// </summary>
    /// <param name="plugInType">Type of the plug in.</param>
    protected virtual void BeforeActivatePlugInType(Type plugInType)
    {
        // can be overwritten to do additional stuff before activating the type.
    }

    /// <summary>
    /// Is called after the specified plug in has been deactivated.
    /// </summary>
    /// <param name="deactivatedPlugIn">The deactivated plug in.</param>
    protected virtual void AfterDeactivatePlugInType(TPlugIn deactivatedPlugIn)
    {
        // can be overwritten to do additional stuff after deactivating the plugin.
    }

    /// <summary>
    /// Finds the known plugin.
    /// </summary>
    /// <param name="plugInType">Type of the plug in.</param>
    /// <returns>The known plugin, if found. Otherwise, null.</returns>
    protected TPlugIn? FindKnownPlugin(Type plugInType)
    {
        using var l = this.Lock.ReaderLock();
        return this._knownPlugIns.FirstOrDefault(p => p.GetType() == plugInType);
    }

    private TPlugIn? FindActivePlugin(Type plugInType)
    {
        using var l = this.Lock.ReaderLock();
        return this.ActivePlugIns.FirstOrDefault(plugInType.IsInstanceOfType);
    }

    private bool IsEventRelevant(PlugInEventArgs e) => typeof(TPlugIn).IsAssignableFrom(e.PlugInType);

    private void OnPlugInDeactivated(object? sender, PlugInEventArgs e)
    {
        if (!this.IsEventRelevant(e))
        {
            return;
        }

        var plugIn = this.FindActivePlugin(e.PlugInType);
        if (plugIn is null)
        {
            return;
        }

        using (this.Lock.WriterLock())
        {
            this.DeactivatePlugIn(plugIn);
        }

        this.AfterDeactivatePlugInType(plugIn);
    }

    private void OnPlugInActivated(object? sender, PlugInEventArgs e)
    {
        if (!this.IsEventRelevant(e))
        {
            return;
        }

        this.BeforeActivatePlugInType(e.PlugInType);

        var plugIn = this.FindActivePlugin(e.PlugInType);
        if (plugIn != null)
        {
            return;
        }

        plugIn = this.FindKnownPlugin(e.PlugInType);
        if (plugIn is null)
        {
            return;
        }

        using (this.Lock.WriterLock())
        {
            this.ActivatePlugIn(plugIn);
        }
    }

    private void OnPlugInConfigurationChanged(object? sender, PlugInConfigurationChangedEventArgs e)
    {
        if (!this.IsEventRelevant(e))
        {
            return;
        }

        var plugIn = this.FindActivePlugin(e.PlugInType) ?? this.FindKnownPlugin(e.PlugInType);
        if (plugIn is null)
        {
            return;
        }

        if (e.PlugInType.GetCustomConfigurationSupportInterfaceType()
            is { } configSupportInterface)
        {
            var configType = configSupportInterface.GenericTypeArguments[0];
            var typedCustomConfiguration = e.Configuration.GetConfiguration(configType, this.Manager.CustomConfigReferenceHandler);
            configSupportInterface
                .GetProperty(nameof(ISupportCustomConfiguration<object>.Configuration))
                ?.SetMethod
                ?.Invoke(plugIn, new[] { typedCustomConfiguration });
        }
    }
}
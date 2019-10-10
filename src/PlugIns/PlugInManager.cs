// <copyright file="PlugInManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using log4net;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// The manager for plugins.
    /// </summary>
    public class PlugInManager
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IDictionary<Type, object> plugInPoints = new Dictionary<Type, object>();
        private readonly IDictionary<Guid, Type> knownPlugIns = new ConcurrentDictionary<Guid, Type>();
        private readonly ConcurrentDictionary<Type, ISet<Type>> knownPlugInsPerInterfaceType = new ConcurrentDictionary<Type, ISet<Type>>();
        private readonly ConcurrentDictionary<Guid, Type> activePlugIns = new ConcurrentDictionary<Guid, Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PlugInManager"/> class.
        /// </summary>
        public PlugInManager()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlugInManager" /> class.
        /// </summary>
        /// <param name="configurations">The configurations.</param>
        public PlugInManager(ICollection<PlugInConfiguration> configurations)
        {
            this.DiscoverAndRegisterPlugIns();
            var loadedAssemblies = new HashSet<string>();
            foreach (var configuration in configurations)
            {
                this.ReadConfiguration(configuration, loadedAssemblies);
            }
        }

        /// <summary>
        /// Occurs when a plug in got deactivated.
        /// </summary>
        public event EventHandler<PlugInEventArgs> PlugInDeactivated;

        /// <summary>
        /// Occurs when a plug in got activated.
        /// </summary>
        public event EventHandler<PlugInEventArgs> PlugInActivated;

        /// <summary>
        /// Gets the known plug in types.
        /// </summary>
        /// <value>
        /// The known plug in types.
        /// </value>
        public IEnumerable<Type> KnownPlugInTypes => this.knownPlugIns.Values;

        /// <summary>
        /// Discovers and registers all plug ins of all loaded assemblies.
        /// </summary>
        public void DiscoverAndRegisterPlugIns()
        {
            var plugIns = this.DiscoverNewPlugIns();
            this.RegisterPlugIns(plugIns);
        }

        /// <summary>
        /// Gets the known plug ins of the given interface type.
        /// </summary>
        /// <typeparam name="T">The plugin interface type. Type parameter of a <see cref="CustomPlugInContainerBase{TPlugIn}"/>.</typeparam>
        /// <returns>The known plugins of the given interface type.</returns>
        public IEnumerable<Type> GetKnownPlugInsOf<T>()
        {
            if (this.knownPlugInsPerInterfaceType.TryGetValue(typeof(T), out var result))
            {
                return result;
            }

            return Enumerable.Empty<Type>();
        }

        /// <summary>
        /// Discovers and registers plug ins of the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public void DiscoverAndRegisterPlugIns(Assembly assembly)
        {
            var plugIns = this.DiscoverNewPlugIns(this.DiscoverPlugIns(assembly));
            this.RegisterPlugIns(plugIns);
        }

        /// <summary>
        /// Discovers the and register plug ins of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the plugins which should be discovered.</typeparam>
        public void DiscoverAndRegisterPlugInsOf<T>()
        {
            var plugIns = this.DiscoverAllPlugIns().Where(type => typeof(T).IsAssignableFrom(type));
            this.RegisterPlugIns(plugIns);
        }

        /// <summary>
        /// Deactivates the plug in of type <typeparamref name="TPlugIn"/>.
        /// </summary>
        /// <typeparam name="TPlugIn">The type of the plug in.</typeparam>
        public void DeactivatePlugIn<TPlugIn>() => this.DeactivatePlugIn(typeof(TPlugIn));

        /// <summary>
        /// Deactivates the plug in of the specified type.
        /// </summary>
        /// <param name="plugIn">The plug in.</param>
        public void DeactivatePlugIn(Type plugIn) => this.DeactivatePlugIn(plugIn.GUID);

        /// <summary>
        /// Deactivates the plug in with the specified type id.
        /// </summary>
        /// <param name="plugInId">The plug in identifier.</param>
        public void DeactivatePlugIn(Guid plugInId)
        {
            if (this.knownPlugIns.TryGetValue(plugInId, out var plugInType))
            {
                this.activePlugIns.TryRemove(plugInId, out _);
                this.PlugInDeactivated?.Invoke(this, new PlugInEventArgs(plugInType));
            }
        }

        /// <summary>
        /// Activates the plug in of type <typeparamref name="TPlugIn"/>.
        /// </summary>
        /// <typeparam name="TPlugIn">The type of the plug in.</typeparam>
        public void ActivatePlugIn<TPlugIn>() => this.ActivatePlugIn(typeof(TPlugIn));

        /// <summary>
        /// Activates the plug in of the specified type.
        /// </summary>
        /// <param name="plugIn">The plug in.</param>
        public void ActivatePlugIn(Type plugIn) => this.ActivatePlugIn(plugIn.GUID);

        /// <summary>
        /// Activates the plug in with the specified type id.
        /// </summary>
        /// <param name="plugInId">The plug in identifier.</param>
        public void ActivatePlugIn(Guid plugInId)
        {
            if (this.knownPlugIns.TryGetValue(plugInId, out var plugInType))
            {
                this.activePlugIns.TryAdd(plugInId, plugInType);
                this.PlugInActivated?.Invoke(this, new PlugInEventArgs(plugInType));
            }
        }

        /// <summary>
        /// Gets the plug in point which implements <typeparamref name="TPlugIn"/>.
        /// </summary>
        /// <typeparam name="TPlugIn">The type of the plug in.</typeparam>
        /// <returns>The plug in point which implements <typeparamref name="TPlugIn"/>, if available; Otherwise, <c>null</c>.</returns>
        public TPlugIn GetPlugInPoint<TPlugIn>()
            where TPlugIn : class
        {
            if (this.plugInPoints.TryGetValue(typeof(TPlugIn), out var obj) && obj is TPlugIn plugIn)
            {
                return plugIn;
            }

            return null;
        }

        /// <summary>
        /// Gets the strategy plug in.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TStrategy">The type of the strategy.</typeparam>
        /// <returns>The strategy plug in.</returns>
        public IStrategyPlugInProvider<TKey, TStrategy> GetStrategyProvider<TKey, TStrategy>()
            where TStrategy : class, IStrategyPlugIn<TKey>
        {
            if (this.plugInPoints.TryGetValue(typeof(TStrategy), out var obj) && obj is IStrategyPlugInProvider<TKey, TStrategy> plugIn)
            {
                return plugIn;
            }

            return null;
        }

        /// <summary>
        /// Gets the strategy plug in.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TStrategy">The type of the strategy.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The strategy plug in of the specified key, if available; Otherwise, <c>null</c>.</returns>
        public TStrategy GetStrategy<TKey, TStrategy>(TKey key)
            where TStrategy : class, IStrategyPlugIn<TKey>
        {
            return this.GetStrategyProvider<TKey, TStrategy>()[key];
        }

        /// <summary>
        /// Gets the strategy plug in.
        /// </summary>
        /// <typeparam name="TStrategy">The type of the strategy.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The strategy plug in of the specified key, if available; Otherwise, <c>null</c>.</returns>
        public TStrategy GetStrategy<TStrategy>(string key)
            where TStrategy : class, IStrategyPlugIn<string>
        {
            return this.GetStrategy<string, TStrategy>(key);
        }

        /// <summary>
        /// Determines whether the specified plug in type is configured as active.
        /// </summary>
        /// <param name="plugInType">Type of the plug in.</param>
        /// <returns>
        ///   <c>true</c> if the specified plug in type is configured as active; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPlugInActive(Type plugInType)
        {
            return this.activePlugIns.ContainsKey(plugInType.GUID);
        }

        /// <summary>
        /// Registers the plug in class for the specified plug in interface.
        /// </summary>
        /// <typeparam name="TPlugInInterface">The type of the plug in interface.</typeparam>
        /// <typeparam name="TPlugInClass">The type of the plug in class.</typeparam>
        public void RegisterPlugIn<TPlugInInterface, TPlugInClass>()
            where TPlugInInterface : class
            where TPlugInClass : TPlugInInterface
        {
            this.RegisterPlugInType(typeof(TPlugInClass));
            if (typeof(TPlugInInterface).GetCustomAttribute(typeof(PlugInPointAttribute)) != null)
            {
                this.RegisterPlugInAtPlugInPoint<TPlugInInterface>(Activator.CreateInstance<TPlugInClass>());
            }
            else if (this.GetCustomPlugInPointType(typeof(TPlugInInterface)) is Type customPlugInPointType)
            {
                if (!this.knownPlugInsPerInterfaceType.TryGetValue(customPlugInPointType, out var plugInList))
                {
                    plugInList = new HashSet<Type>();
                    this.knownPlugInsPerInterfaceType.TryAdd(customPlugInPointType, plugInList);
                }

                plugInList.Add(typeof(TPlugInClass));
                this.PlugInActivated?.Invoke(this, new PlugInEventArgs(typeof(TPlugInClass)));
            }
            else
            {
                Log.Warn($"Plugin {typeof(TPlugInClass)} wasn't registered, because it's not an implementation of an interface which is marked with {nameof(PlugInPointAttribute)} or {nameof(CustomPlugInContainerAttribute)}.");
            }
        }

        /// <summary>
        /// Registers the plug in instance for the specified plugin point interface.
        /// </summary>
        /// <typeparam name="TPlugInInterface">The type of the plug in interface.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <exception cref="ArgumentException">Plugin Type {instance.GetType()} - instance.</exception>
        public void RegisterPlugInAtPlugInPoint<TPlugInInterface>(TPlugInInterface instance)
            where TPlugInInterface : class
        {
            if (instance.GetType().GetCustomAttribute<GuidAttribute>() == null)
            {
                throw new ArgumentException($"Plugin Type {instance.GetType()} is missing a {typeof(GuidAttribute)}. It's required to identify the plugin in the configuration. Otherwise, the plugin would get a different Guid every time it gets compiled.", nameof(instance));
            }

            if (!this.plugInPoints.TryGetValue(typeof(TPlugInInterface), out var point))
            {
                var proxy = this.CreateProxy<TPlugInInterface>();
                proxy.AddPlugIn(instance, true);
                this.plugInPoints.Add(typeof(TPlugInInterface), proxy);
            }
            else if (point is IPlugInContainer<TPlugInInterface> proxy)
            {
                proxy.AddPlugIn(instance, true);
            }
            else
            {
                throw new InvalidPlugInProxyException(point.GetType(), typeof(IPlugInContainer<TPlugInInterface>));
            }

            this.RegisterPlugInType(instance.GetType());
        }

        private Type GetCustomPlugInPointType(Type interfaceType)
        {
            if (interfaceType.GetCustomAttribute<CustomPlugInContainerAttribute>() != null)
            {
                return interfaceType;
            }

            return interfaceType.GetInterfaces().FirstOrDefault(i => i.GetCustomAttribute<CustomPlugInContainerAttribute>() != null);
        }

        private void RegisterPlugInType(Type plugInType)
        {
            var plugInTypeId = plugInType.GUID;
            if (!this.knownPlugIns.ContainsKey(plugInTypeId))
            {
                this.knownPlugIns.Add(plugInTypeId, plugInType);
                this.activePlugIns.TryAdd(plugInTypeId, plugInType); // registered plugins are by default active
                Log.DebugFormat("Added known plugin {0}, {1}", plugInTypeId, plugInType);
            }
        }

        private IPlugInContainer<TPlugInInterface> CreateProxy<TPlugInInterface>()
            where TPlugInInterface : class
        {
            IPlugInContainer<TPlugInInterface> proxy;
            var strategyPlugInInterface = typeof(TPlugInInterface).GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IStrategyPlugIn<>));
            if (strategyPlugInInterface != null)
            {
                var keyType = strategyPlugInInterface.GetGenericArguments()[0];
                var providerType = typeof(StrategyPlugInProvider<,>).MakeGenericType(keyType, typeof(TPlugInInterface));
                proxy = Activator.CreateInstance(providerType, this) as IPlugInContainer<TPlugInInterface>;
            }
            else
            {
                var proxyGenerator = new PlugInProxyTypeGenerator();
                proxy = proxyGenerator.GenerateProxy<TPlugInInterface>(this);
            }

            return proxy;
        }

        private void ReadConfiguration(PlugInConfiguration configuration, HashSet<string> loadedAssemblies)
        {
            if (!this.knownPlugIns.ContainsKey(configuration.TypeId))
            {
                if (!string.IsNullOrEmpty(configuration.ExternalAssemblyName)
                    && !loadedAssemblies.Contains(configuration.ExternalAssemblyName.ToLower()))
                {
                    loadedAssemblies.Add(configuration.ExternalAssemblyName.ToLower());

                    try
                    {
                        var assembly = Assembly.LoadFile("plugins\\" + configuration.ExternalAssemblyName);
                        this.DiscoverAndRegisterPlugIns(assembly);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Error while loading external plugin assembly {configuration.ExternalAssemblyName} for plugin {configuration.TypeId}.", e);
                        return;
                    }
                }
                else if (!string.IsNullOrEmpty(configuration.CustomPlugInSource))
                {
                    Assembly customPlugInAssembly = this.CompileCustomPlugInAssembly(configuration);
                    this.DiscoverAndRegisterPlugIns(customPlugInAssembly);
                }
                else
                {
                    // nothing we can do
                }
            }

            if (this.knownPlugIns.TryGetValue(configuration.TypeId, out Type plugInType))
            {
                if (!configuration.IsActive)
                {
                    this.DeactivatePlugIn(plugInType);
                }

                // When the IsActive property changed, we activate/deactivate accordingly.
                // Currently, property changes are only fired for IsActive, so we don't need to check it.
                configuration.PropertyChanged += (sender, args) => this.OnConfigurationChanged(configuration, plugInType);
            }
            else
            {
                Log.Warn($"Unknown plugin type for id {configuration.TypeId}");
            }
        }

        private void OnConfigurationChanged(PlugInConfiguration configuration, Type plugInType)
        {
            if (configuration.IsActive)
            {
                this.ActivatePlugIn(plugInType);
            }
            else
            {
                this.DeactivatePlugIn(plugInType);
            }
        }

        private Assembly CompileCustomPlugInAssembly(PlugInConfiguration configuration)
        {
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(configuration.CustomPlugInSource);
            return syntaxTree.CompileAndLoad($"CustomPlugIn_{configuration.TypeId}");
        }

        private IEnumerable<Type> DiscoverNewPlugIns()
        {
            return this.DiscoverNewPlugIns(this.DiscoverAllPlugIns());
        }

        private IEnumerable<Type> DiscoverNewPlugIns(IEnumerable<Type> allPlugIns)
        {
            var newPlugIns = allPlugIns.Where(plugIn => !this.knownPlugIns.ContainsKey(plugIn.GUID)).ToList();
            return newPlugIns;
        }

        private IEnumerable<Type> DiscoverAllPlugIns()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.FullName.StartsWith("System"))
                .Where(assembly => !assembly.FullName.StartsWith("Microsoft"))
                .SelectMany(assembly => assembly.DefinedTypes.Where(type => type.GetCustomAttribute<PlugInAttribute>() != null));
        }

        private IEnumerable<Type> DiscoverPlugIns(Assembly assembly)
        {
            return assembly.DefinedTypes.Where(type => type.GetCustomAttribute<PlugInAttribute>() != null);
        }

        private void RegisterPlugIns(IEnumerable<Type> plugIns)
        {
            foreach (var plugIn in plugIns)
            {
                try
                {
                    // A plugin usually should be small, but it should be possible that one plugin can implement more than one plugin interface.
                    // In this case, we need to register it at every plugin point
                    var plugInInterfaces = plugIn.GetInterfaces().Where(t => t.IsInterface && (t.GetCustomAttribute<PlugInPointAttribute>() != null || t.GetCustomAttribute<CustomPlugInContainerAttribute>() != null));
                    foreach (var plugInInterface in plugInInterfaces)
                    {
                        var genericMethod = this.GetType().GetMethods().FirstOrDefault(mi => mi.IsGenericMethod && mi.Name == nameof(this.RegisterPlugIn))?.MakeGenericMethod(plugInInterface, plugIn);
                        genericMethod?.Invoke(this, Array.Empty<object>());
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Couldn't register plugin type {plugIn}", e);
                }
            }
        }

        /// <summary>
        /// Exception which occurs when the created proxy doesn't implement the expected interface <see cref="IPlugInContainer{TPlugIn}"/>.
        /// </summary>
        /// <seealso cref="System.Exception" />
        public class InvalidPlugInProxyException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InvalidPlugInProxyException"/> class.
            /// </summary>
            /// <param name="type">The actual class type of the proxy.</param>
            /// <param name="expectedType">The expected interface type of the proxy.</param>
            public InvalidPlugInProxyException(Type type, Type expectedType)
            {
                this.Type = type;
                this.ExpectedType = expectedType;
            }

            /// <summary>
            /// Gets the actual class type of the proxy.
            /// </summary>
            /// <value>
            /// The type.
            /// </value>
            public Type Type { get; }

            /// <summary>
            /// Gets the expected interface type of the proxy.
            /// </summary>
            public Type ExpectedType { get; }

            /// <inheritdoc />
            public override string ToString()
            {
                return $"Unexpected plugin proxy type {this.Type}. Expected one of {this.ExpectedType}";
            }
        }
    }
}

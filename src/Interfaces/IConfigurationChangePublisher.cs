namespace MUnique.OpenMU.Interfaces;

public interface IConfigurationChangePublisher
{
    void ConfigurationChanged(Type type, Guid id, object configuration);

    void ConfigurationAdded(Type type, Guid id, object configuration);

    void ConfigurationRemoved(Type type, Guid id);

    void ConfigurationChanged<T>(Guid id, T configuration) where T : class => this.ConfigurationChanged(typeof(T), id, configuration);

    void ConfigurationAdded<T>(Guid id, T configuration) where T : class => this.ConfigurationAdded(typeof(T), id, configuration);

    void ConfigurationRemoved<T>(Guid id) => this.ConfigurationRemoved(typeof(T), id);

    static IConfigurationChangePublisher None { get; } = new NoneConfigurationChangePublisher();

    private class NoneConfigurationChangePublisher : IConfigurationChangePublisher
    {
        public void ConfigurationChanged(Type type, Guid id, object configuration)
        {
            // do nothing.
        }

        public void ConfigurationAdded(Type type, Guid id, object configuration)
        {
            // do nothing
        }

        public void ConfigurationRemoved(Type type, Guid id)
        {
            // do nothing
        }
    }
}

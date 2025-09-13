namespace MUnique.OpenMU.GameLogic;

using System.Collections.Concurrent;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Caches monster base attributes by monster number, loaded once from persistence (similar to AdminPanel data source).
/// Stores stat-id/value pairs to allow safe cloning into any target type.
/// </summary>
internal static class MonsterDefinitionAttributeCache
{
    private static readonly ConcurrentDictionary<short, IReadOnlyList<(Guid StatId, float Value)>> Cache = new();
    private static volatile bool _loaded;

    private static void AddAttributeCorrectly(IGameContext context, ICollection<MonsterAttribute>? target, Guid statId, float value)
    {
        if (target is null)
        {
            return;
        }

        var def = context.Configuration.Attributes.FirstOrDefault(a => a.Id == statId);
        if (def is null)
        {
            return;
        }

        var collectionType = target.GetType();
        if (collectionType.IsGenericType && collectionType.GetGenericTypeDefinition() == typeof(MUnique.OpenMU.Persistence.CollectionAdapter<,>))
        {
            var efItemType = collectionType.GetGenericArguments()[1];
            if (Activator.CreateInstance(efItemType) is MonsterAttribute efAttr)
            {
                efAttr.AttributeDefinition = def;
                efAttr.Value = value;
                target.Add(efAttr);
                return;
            }
        }

        target.Add(new MonsterAttribute { AttributeDefinition = def, Value = value });
    }

    public static void EnsureLoaded(IGameContext gameContext)
    {
        if (_loaded)
        {
            return;
        }

        lock (Cache)
        {
            if (_loaded)
            {
                return;
            }

            try
            {
                // Load using the AdminPanel-like data source (includes children according to GameConfigurationHelper)
                var ds = new GameConfigurationDataSource(gameContext.LoggerFactory.CreateLogger<GameConfigurationDataSource>(), gameContext.PersistenceContextProvider);
                var ownerObj = ds.GetOwnerAsync(default).AsTask().GetAwaiter().GetResult();
                _ = ds.GetAll<MonsterAttribute>(); // touch to ensure attributes get materialized
                if (ownerObj is GameConfiguration owner)
                {
                    foreach (var def in owner.Monsters)
                    {
                        if (def?.Attributes is { Count: > 0 })
                        {
                            var pairs = def.Attributes
                                .Where(a => a.AttributeDefinition is { })
                                .Select(a => (a.AttributeDefinition!.Id, a.Value))
                                .ToList();
                            if (pairs.Count > 0)
                            {
                                Cache[def.Number] = pairs;
                            }
                        }
                    }
                }
            }
            catch
            {
                // ignore; cache may remain empty and runtime code can handle it.
            }
            finally
            {
                _loaded = true;
            }
        }
    }

    public static bool TryFillAttributes(IGameContext gameContext, short monsterNumber, MonsterDefinition target)
    {
        EnsureLoaded(gameContext);
        if (Cache.TryGetValue(monsterNumber, out var list) && list.Count > 0)
        {
            foreach (var (statId, value) in list)
            {
                AddAttributeCorrectly(gameContext, target.Attributes, statId, value);
            }
            return true;
        }

        return false;
    }
}

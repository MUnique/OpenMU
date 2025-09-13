namespace MUnique.OpenMU.GameLogic;

using System.Collections.Concurrent;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Caches monster base attributes by monster number, loaded once from persistence (similar to AdminPanel data source).
/// </summary>
internal static class MonsterDefinitionAttributeCache
{
    private static readonly ConcurrentDictionary<short, IList<MonsterAttribute>> Cache = new();
    private static volatile bool _loaded;

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
                // Force-load child collections like the Admin Panel does (builds internal dictionary by enumerating children)
                _ = ds.GetAll<MonsterAttribute>();
                if (ownerObj is GameConfiguration owner)
                {
                    foreach (var def in owner.Monsters)
                    {
                        if (def?.Attributes is { Count: > 0 })
                        {
                            Cache[def.Number] = def.Attributes
                                .Select(a => new MonsterAttribute { AttributeDefinition = a.AttributeDefinition, Value = a.Value })
                                .ToList();
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
            foreach (var a in list)
            {
                target.Attributes?.Add(new MonsterAttribute { AttributeDefinition = a.AttributeDefinition, Value = a.Value });
            }
            return true;
        }

        return false;
    }
}

namespace MUnique.OpenMU.GameLogic;

using System.Collections.Concurrent;
using MUnique.OpenMU.DataModel.Configuration;

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
                // Load using the GameConfiguration aggregate root (same approach as Admin Panel IDataSource)
                using var ctx = gameContext.PersistenceContextProvider.CreateNewTypedContext(typeof(GameConfiguration), useCache: false);
                var owner = ctx.GetAsync<GameConfiguration>().AsTask().GetAwaiter().GetResult().FirstOrDefault();
                if (owner is not null)
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

// <copyright file="ModelMeasurements.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Measurements;

using Microsoft.EntityFrameworkCore;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.DataModel.Statistics;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Measures the runtime model building of the EF Core contexts.
/// </summary>
internal static class ModelMeasurements
{
    /// <summary>
    /// The edit types which are used for typed contexts outside of the admin panel.
    /// </summary>
    private static readonly Type[] GameTypedContextTypes =
    [
        typeof(PlugInConfiguration),
        typeof(SystemConfiguration),
        typeof(GameServerDefinition),
        typeof(ConnectServerDefinition),
        typeof(CastleSiegeData),
        typeof(CastleSiegeGuildRegistration),
        typeof(CastleSiegePendingReward),
        typeof(GensMember),
        typeof(GensAbuse),
        typeof(MiniGameRankingEntry),
    ];

    /// <summary>
    /// Measures the model building.
    /// </summary>
    public static void Measure()
    {
        Measurements.Measure.Section("EF Core model building");

        Measurements.Measure.Row("EntityDataContext (cold, first model)", BuildModel(() => new EntityDataContext()), "includes EF Core startup and JIT");
        Measurements.Measure.Row("ConfigurationContext", BuildModel(() => new ConfigurationContext()));
        Measurements.Measure.Row("AccountContext", BuildModel(() => new AccountContext()));
        Measurements.Measure.Row("TradeContext", BuildModel(() => new TradeContext()));
        Measurements.Measure.Row("GuildContext", BuildModel(() => new GuildContext()));
        Measurements.Measure.Row("FriendContext", BuildModel(() => new FriendContext()));

        var gameTypedContexts = Measurements.Measure.Run(() =>
        {
            foreach (var editType in GameTypedContextTypes)
            {
                using var context = new TypedContext(editType);
                _ = context.Model;
            }
        });
        Measurements.Measure.Row($"TypedContext for the {GameTypedContextTypes.Length} edit types used by the servers", gameTypedContexts, "one model per edit type");

        using var completeContext = new EntityDataContext();
        var allEditTypes = completeContext.Model.GetEntityTypes()
            .Select(t => t.ClrType.BaseType is { } baseType && baseType != typeof(object) && baseType.Namespace?.StartsWith("MUnique.OpenMU.DataModel", StringComparison.Ordinal) is true ? baseType : t.ClrType)
            .Except(GameTypedContextTypes)
            .Distinct()
            .ToList();
        var allTypedContexts = Measurements.Measure.Run(() =>
        {
            foreach (var editType in allEditTypes)
            {
                using var context = new TypedContext(editType);
                _ = context.Model;
            }
        });
        Measurements.Measure.Row($"TypedContext for the other {allEditTypes.Count} entity types", allTypedContexts, "worst case, e.g. admin panel");
    }

    private static Measurements.Measure.Result BuildModel(Func<DbContext> contextFactory)
    {
        return Measurements.Measure.Run(() =>
        {
            using var context = contextFactory();
            _ = context.Model;
        });
    }
}

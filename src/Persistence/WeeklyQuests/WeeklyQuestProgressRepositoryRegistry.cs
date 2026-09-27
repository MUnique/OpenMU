// <copyright file="WeeklyQuestProgressRepositoryRegistry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.WeeklyQuests;

/// <summary>
/// Holds the <see cref="IWeeklyQuestProgressRepository"/> of the process.
/// </summary>
/// <remarks>
/// The weekly quest plugin needs a parameterless constructor, because the default configuration
/// of plugins is created with <see cref="Activator.CreateInstance(Type)"/>. That's why it can't get
/// the repository by constructor injection. The host sets it here after it has been built.
/// </remarks>
public static class WeeklyQuestProgressRepositoryRegistry
{
    /// <summary>
    /// Gets or sets the repository. If it's <c>null</c>, the progress is kept in memory.
    /// </summary>
    public static IWeeklyQuestProgressRepository? Current { get; set; }
}

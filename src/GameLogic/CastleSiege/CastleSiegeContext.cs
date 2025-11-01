// <copyright file="CastleSiegeContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

/// <summary>
/// Context for castle siege functionality.
/// </summary>
public class CastleSiegeContext
{
    /// <summary>
    /// Gets the registered alliance guilds for castle siege.
    /// </summary>
    public IList<CastleSiegeRegistration> RegisteredAlliances { get; } = new List<CastleSiegeRegistration>();

    /// <summary>
    /// Gets or sets the current castle owner alliance.
    /// </summary>
    public CastleSiegeRegistration? OwnerAlliance { get; set; }

    /// <summary>
    /// Gets or sets the castle siege state.
    /// </summary>
    public CastleSiegeState State { get; set; }

    /// <summary>
    /// Registers an alliance for castle siege.
    /// </summary>
    /// <param name="allianceMasterGuildId">The alliance master guild identifier.</param>
    /// <param name="guildMarksSubmitted">The number of guild marks submitted.</param>
    /// <returns>True if registration was successful; False otherwise.</returns>
    public bool RegisterAlliance(uint allianceMasterGuildId, int guildMarksSubmitted = 0)
    {
        // Check if already registered
        if (this.RegisteredAlliances.Any(r => r.AllianceMasterGuildId == allianceMasterGuildId))
        {
            return false;
        }

        var registration = new CastleSiegeRegistration
        {
            AllianceMasterGuildId = allianceMasterGuildId,
            GuildMarksSubmitted = guildMarksSubmitted,
            RegistrationDate = DateTime.UtcNow,
        };

        this.RegisteredAlliances.Add(registration);
        return true;
    }

    /// <summary>
    /// Unregisters an alliance from castle siege.
    /// </summary>
    /// <param name="allianceMasterGuildId">The alliance master guild identifier.</param>
    /// <returns>True if unregistration was successful; False otherwise.</returns>
    public bool UnregisterAlliance(uint allianceMasterGuildId)
    {
        var registration = this.RegisteredAlliances.FirstOrDefault(r => r.AllianceMasterGuildId == allianceMasterGuildId);
        if (registration is null)
        {
            return false;
        }

        this.RegisteredAlliances.Remove(registration);
        return true;
    }

    /// <summary>
    /// Adds guild marks to an alliance's registration.
    /// </summary>
    /// <param name="allianceMasterGuildId">The alliance master guild identifier.</param>
    /// <param name="marksToAdd">The number of marks to add.</param>
    /// <returns>The total number of marks after addition, or -1 if not registered.</returns>
    public int AddGuildMarks(uint allianceMasterGuildId, int marksToAdd)
    {
        var registration = this.RegisteredAlliances.FirstOrDefault(r => r.AllianceMasterGuildId == allianceMasterGuildId);
        if (registration is null)
        {
            return -1;
        }

        registration.GuildMarksSubmitted += marksToAdd;
        return registration.GuildMarksSubmitted;
    }

    /// <summary>
    /// Gets the registration for a specific alliance.
    /// </summary>
    /// <param name="allianceMasterGuildId">The alliance master guild identifier.</param>
    /// <returns>The registration, or null if not found.</returns>
    public CastleSiegeRegistration? GetRegistration(uint allianceMasterGuildId)
    {
        return this.RegisteredAlliances.FirstOrDefault(r => r.AllianceMasterGuildId == allianceMasterGuildId);
    }

    /// <summary>
    /// Sets the castle owner to the specified alliance.
    /// </summary>
    /// <param name="allianceMasterGuildId">The alliance master guild identifier.</param>
    /// <returns>True if the owner was set successfully; False if the alliance is not registered.</returns>
    public bool SetOwner(uint allianceMasterGuildId)
    {
        var registration = this.GetRegistration(allianceMasterGuildId);
        if (registration is null)
        {
            return false;
        }

        this.OwnerAlliance = registration;
        return true;
    }

    /// <summary>
    /// Clears the castle owner.
    /// </summary>
    public void ClearOwner()
    {
        this.OwnerAlliance = null;
    }
}

/// <summary>
/// Represents a castle siege registration for an alliance.
/// </summary>
public class CastleSiegeRegistration
{
    /// <summary>
    /// Gets or sets the alliance master guild identifier.
    /// </summary>
    public required uint AllianceMasterGuildId { get; init; }

    /// <summary>
    /// Gets or sets the number of guild marks submitted.
    /// </summary>
    public int GuildMarksSubmitted { get; set; }

    /// <summary>
    /// Gets or sets the registration date.
    /// </summary>
    public DateTime RegistrationDate { get; init; }
}

/// <summary>
/// Represents the state of the castle siege.
/// </summary>
public enum CastleSiegeState
{
    /// <summary>
    /// Castle siege is not active.
    /// </summary>
    Inactive,

    /// <summary>
    /// Registration period is open.
    /// </summary>
    RegistrationOpen,

    /// <summary>
    /// Siege is in progress.
    /// </summary>
    InProgress,

    /// <summary>
    /// Siege has ended.
    /// </summary>
    Ended,
}

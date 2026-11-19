// <copyright file="WebsiteLoginThrottle.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

/// <summary>
/// Limits the failed logins of the website per account, so that the password of an account can't be guessed
/// through the website.
/// </summary>
/// <remarks>
/// After <see cref="MaximumFailures"/> failed logins within <see cref="Window"/>, the account is locked out for
/// <see cref="Window"/>, counted from the failure which started the lockout. During the lockout, it can't log in at
/// the website, not even with the right password: otherwise, the answer would still tell an attacker when a guess was
/// right. The game itself is not affected.
/// Each password check is reserved with <see cref="TryBeginAttempt"/> before it starts, and the checks which are
/// still running count against the limit, so parallel requests can't check more passwords than the limit allows.
/// The server only sees the website, not its visitors, so the website still has to limit the logins per visitor.
/// The state is kept in the memory of the process: it's lost on a restart, which only shortens a lockout, and
/// several instances of the admin panel count the failed logins separately.
/// </remarks>
public sealed class WebsiteLoginThrottle
{
    /// <summary>
    /// The number of failed logins within the <see cref="Window"/> after which an account is locked out.
    /// </summary>
    internal const int MaximumFailures = 5;

    /// <summary>
    /// The number of tracked accounts above which the expired entries are removed.
    /// </summary>
    private const int CleanupThreshold = 1000;

    private readonly Dictionary<string, Entry> _entries = new(StringComparer.OrdinalIgnoreCase);

    private readonly System.Threading.Lock _lock = new();

    /// <summary>
    /// Gets the time span in which the failed logins are counted, and for which an account is locked out.
    /// </summary>
    internal static TimeSpan Window { get; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Reserves a password check of the account, unless it's locked out or the running checks would exceed the limit.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    /// <param name="utcNow">The current time, in UTC.</param>
    /// <returns>
    /// <c>true</c>, if the password may be checked; then the check has to be finished with <see cref="CompleteAttempt"/>
    /// or <see cref="CancelAttempt"/>. <c>false</c>, if the account can't log in at the website for now.
    /// </returns>
    public bool TryBeginAttempt(string loginName, DateTime utcNow)
    {
        lock (this._lock)
        {
            this.RemoveExpiredEntries(utcNow);
            var entry = this.GetCurrentEntry(loginName, utcNow);
            if (entry.LockedOutUntilUtc is not null || entry.Failures + entry.RunningAttempts >= MaximumFailures)
            {
                return false;
            }

            entry.RunningAttempts++;
            this._entries[loginName] = entry;
            return true;
        }
    }

    /// <summary>
    /// Finishes a reserved password check of the account.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    /// <param name="succeeded"><c>true</c>, if the password was right; then the failed logins are forgotten.</param>
    /// <param name="utcNow">The current time, in UTC.</param>
    /// <returns><c>true</c>, if this failed login started the lockout of the account.</returns>
    public bool CompleteAttempt(string loginName, bool succeeded, DateTime utcNow)
    {
        lock (this._lock)
        {
            var entry = this.GetCurrentEntry(loginName, utcNow);
            entry.RunningAttempts = Math.Max(0, entry.RunningAttempts - 1);
            if (succeeded)
            {
                entry = entry with { Failures = 0, FirstFailureUtc = default, LockedOutUntilUtc = null };
            }
            else
            {
                if (entry.Failures == 0)
                {
                    entry.FirstFailureUtc = utcNow;
                }

                entry.Failures++;
            }

            var startsLockout = entry.LockedOutUntilUtc is null && entry.Failures >= MaximumFailures;
            if (startsLockout)
            {
                entry.LockedOutUntilUtc = utcNow + Window;
            }

            this.Store(loginName, entry);
            return startsLockout;
        }
    }

    /// <summary>
    /// Releases a reserved password check of the account, which didn't check the password, e.g. because of an error.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    public void CancelAttempt(string loginName)
    {
        lock (this._lock)
        {
            if (this._entries.TryGetValue(loginName, out var entry))
            {
                entry.RunningAttempts = Math.Max(0, entry.RunningAttempts - 1);
                this.Store(loginName, entry);
            }
        }
    }

    /// <summary>
    /// Gets the time after which the account may try to log in at the website again.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    /// <param name="utcNow">The current time, in UTC.</param>
    /// <returns>
    /// The remaining time of the lockout; <see cref="Window"/>, if the account isn't locked out yet, but the running
    /// password checks may still lock it out; <see cref="TimeSpan.Zero"/>, if it may try again right now.
    /// </returns>
    public TimeSpan GetRetryAfter(string loginName, DateTime utcNow)
    {
        lock (this._lock)
        {
            var entry = this.GetCurrentEntry(loginName, utcNow);
            if (entry.LockedOutUntilUtc is { } lockedOutUntil)
            {
                return lockedOutUntil - utcNow;
            }

            return entry.Failures + entry.RunningAttempts >= MaximumFailures ? Window : TimeSpan.Zero;
        }
    }

    /// <summary>
    /// Gets the entry of the account, without the failures of an expired window or lockout.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    /// <param name="utcNow">The current time, in UTC.</param>
    /// <returns>The current entry; an empty one, if the account isn't tracked.</returns>
    private Entry GetCurrentEntry(string loginName, DateTime utcNow)
    {
        if (!this._entries.TryGetValue(loginName, out var entry))
        {
            return default;
        }

        if (entry.LockedOutUntilUtc is { } lockedOutUntil
            ? utcNow >= lockedOutUntil
            : entry.Failures > 0 && utcNow - entry.FirstFailureUtc >= Window)
        {
            entry = entry with { Failures = 0, FirstFailureUtc = default, LockedOutUntilUtc = null };
        }

        return entry;
    }

    /// <summary>
    /// Stores the entry of the account, or removes it, if it doesn't hold anything anymore.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    /// <param name="entry">The entry.</param>
    private void Store(string loginName, Entry entry)
    {
        if (entry is { Failures: 0, RunningAttempts: 0, LockedOutUntilUtc: null })
        {
            this._entries.Remove(loginName);
        }
        else
        {
            this._entries[loginName] = entry;
        }
    }

    /// <summary>
    /// Removes the expired entries, when many accounts are tracked.
    /// </summary>
    /// <param name="utcNow">The current time, in UTC.</param>
    private void RemoveExpiredEntries(DateTime utcNow)
    {
        if (this._entries.Count <= CleanupThreshold)
        {
            return;
        }

        foreach (var loginName in this._entries.Keys.ToList())
        {
            this.Store(loginName, this.GetCurrentEntry(loginName, utcNow));
        }
    }

    /// <summary>
    /// The state of the logins of an account.
    /// </summary>
    /// <param name="Failures">The number of failed logins since <paramref name="FirstFailureUtc"/>.</param>
    /// <param name="FirstFailureUtc">The time of the first counted failed login.</param>
    /// <param name="RunningAttempts">The number of reserved password checks which haven't finished yet.</param>
    /// <param name="LockedOutUntilUtc">The end of the lockout; <c>null</c>, if the account isn't locked out.</param>
    private record struct Entry(int Failures, DateTime FirstFailureUtc, int RunningAttempts, DateTime? LockedOutUntilUtc);
}

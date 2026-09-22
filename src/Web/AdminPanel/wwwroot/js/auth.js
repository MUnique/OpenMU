// Exchanges a single use sign in ticket for the authentication cookie, without navigating away.
export async function signIn(ticket) {
    const response = await fetch('/auth/complete', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ticket: ticket }),
        credentials: 'same-origin',
        cache: 'no-store'
    });
    return response.ok;
}

// Removes the authentication cookie, without navigating away.
export async function signOut() {
    const response = await fetch('/auth/logout', {
        method: 'POST',
        credentials: 'same-origin',
        cache: 'no-store'
    });
    return response.ok;
}

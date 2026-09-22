---
title: Signing in
sidebar_label: Signing in
sidebar_position: 13
description: How the admin panel login works, and how to set a second factor up.
---

# Signing in

The admin panel authenticates its users itself. Earlier versions relied on the
basic authentication of the reverse proxy against an `.htpasswd` file; that file
is gone, and so are the proxy configurations which used it.

What you get instead:

* a real login page with a logout,
* an optional second factor with an authenticator app,
* roles, so not every user can change everything,
* the same protection no matter how you run the server — from source, in docker,
  or in the distributed deployment.

## The login

Enter your login name and password. If your account has a second factor, the
form asks for the code right away, on the same page — nothing reloads, and you
don't lose what you had open.

Check *Keep me signed in* to stay signed in after closing the browser. Without
it, the session ends when the browser does.

A session is only created once **everything** checked out, so an active session
always means the second factor was used when your account has one.

After five failed attempts the account is locked for five minutes. Wrong
authenticator codes count towards that too.

## The first user

On a fresh installation there is no database yet — and the admin panel is the
tool which creates it. Until the first user exists, the panel stays reachable
**without a login** and shows a warning banner.

:::danger[Close that window]
Anybody who reaches the panel during that time can set your server up and read
all account data afterwards. Either finish the installation and create your
first user immediately, or configure a bootstrap user before the first start —
see below.
:::

### Bootstrap user

A bootstrap user is configured outside of the database, so it works from the
very first second — before any installation, and also when you locked yourself
out later. Set these environment variables on the container or process which
hosts the admin panel:

```bash
OPENMU_ADMIN_USER=admin
OPENMU_ADMIN_PASSWORD=<a long password>
# optional, if the bootstrap user should require a second factor:
OPENMU_ADMIN_TOTP_SECRET=<base32 secret>
```

The docker compose files already pass these through, so you can put them in a
`.env` file next to the compose file.

The same can be configured under `AdminPanel:Auth:BootstrapUser` in the
`appsettings.json`.

:::note[It is a way in, not an account to work with]
Changes to the bootstrap user — a password change, a newly set up authenticator,
its lockout counter — are only kept in memory and are gone after a restart,
because there is nowhere to store them. Use it to create a real user on the
[Users page](users.md), then work with that one.
:::

## Two-factor authentication

Every user can protect its own account with a time based one time password
(TOTP) under *Account security*. It works with the **Microsoft Authenticator**
app and with any other authenticator app — Google Authenticator, Aegis,
Bitwarden, 1Password, and so on.

### Setting it up

1. Open *Account security* from the header, next to your user name.
2. Click **Set up authenticator app**.
3. Scan the QR code with your app. If you can't scan it, type the key which is
   shown below the code into the app by hand.
4. Enter the six digit code your app shows and click **Verify**.

The second factor is only switched on after that last step succeeded, so a
mis-scan can't lock you out of your own panel.

### Recovery codes

Right after the setup you get ten recovery codes. **They are shown exactly
once.** Store them somewhere safe, outside of the server — a password manager,
or on paper.

Each code signs you in once when you don't have your authenticator app, through
*Use a recovery code instead* on the login page. When you run low, generate a
new set under *Account security*; the old ones stop working then.

Only hashes of the codes are stored, so a database dump does not hand out usable
second factors.

### Lost the authenticator and the recovery codes

An administrator can reset the second factor of any user on the
[Users page](users.md). If nobody can sign in anymore, use a
[bootstrap user](#bootstrap-user).

### Requiring it from everybody

Set `AdminPanel:Auth:RequireTwoFactor` to `true` to require a second factor from
every user. Users who don't have one yet are then asked to set it up before they
can use the panel.

## Roles

Each user has one role. They build up on each other:

| Role | May do |
|---|---|
| **Viewer** | See the servers, accounts and the configuration |
| **Operator** | Everything above, plus operating the servers and editing accounts |
| **Administrator** | Everything above, plus the setup, plugins, configuration updates, log files and the user management |

Give each administrator their own user, so you can remove one without changing
everybody else's password.

## API keys for external applications

The server has a small public API under `/api` — the server status, the number
of online players, whether an account is online, and a global message. A game
launcher, a status page or a website needs it, and none of them can go through
a login form with a second factor.

They authenticate with an API key instead. Send it in the `X-Api-Key` header:

```http
GET /api/status HTTP/1.1
X-Api-Key: <the key>
```

`Authorization: Bearer <the key>` works as well, for clients which only speak
that.

### Creating a key

Open **API keys** in the navigation — it's next to *Users* and needs the
administrator role. Click **Create API key**, give the application a name and
pick its role.

The generated key is then shown once, with a button which copies it to the
clipboard.

:::warning[The key is shown exactly once]
Only a hash of the key is stored, the same way recovery codes are. Copy it
straight into the application which needs it before you close the message. If it
gets lost, delete the key and create a new one.
:::

The list shows each key by its name and its first characters, so you can tell
them apart without knowing them.

**Disable** stops a key from working without deleting it, which is the quickest
reaction when you suspect a key has leaked and you don't want to touch the
application's configuration yet. **Delete** removes it for good.

### What a key may do

A key has the same [roles](#roles) as a user, and defaults to **Viewer**:

| Endpoint | Needs |
|---|---|
| `GET /api/status` | Viewer |
| `GET /api/is-online/{account}` | Viewer |
| `GET /api/send/{server}?msg=` | Operator |

So a status page gets a Viewer key and can only read, while an application which
announces something in the game needs an Operator key. The role is chosen when
the key is created and can't be changed afterwards — create a new key with the
role you need and delete the old one.

A signed in admin panel user can use the API as well, with the same roles — this
is handy while trying things out in the browser.

:::warning[The key is a password]
It is sent in plain text with every request, so use HTTPS, and keep it out of
client side code — a key in a launcher which ships to players is a key your
players have. Requests without a valid key get `401`, and requests whose key
lacks the role get `403`.
:::

Like the panel itself, the API is open as long as [no user exists at all](#the-first-user).

## Keeping the sessions alive across restarts

The sessions and the stored authenticator secrets are protected with a key ring
which has to survive a restart. The docker compose files mount the
`adminpanel-keys` volume at `/app/data-protection-keys` for that.

:::warning[Don't lose that volume]
If the key ring is lost, everybody is signed out **and** every stored
authenticator secret becomes unreadable, so every user has to set its second
factor up again. Recovery codes still work, and so does a bootstrap user.
:::

The location can be changed with `AdminPanel:Auth:DataProtectionKeyPath`.

## Still worth doing

* **Set up HTTPS.** The session cookie travels over whatever the request used —
  without TLS it can be read on the way
  ([all-in-one](../deployment/all-in-one.md#option-b--with-https),
  [Traefik](../deployment/all-in-one-traefik.md#option-b--with-https)).
* Don't expose the admin panel port to the whole internet if you can reach it
  through a VPN or an SSH tunnel instead.
* Remember that admin panel access means full access to your players' account
  data.

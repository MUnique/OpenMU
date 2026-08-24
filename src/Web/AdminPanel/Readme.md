# Admin Panel

The admin panel is meant to offer functions for administrative tasks.

It's implemented with ASP.NET Core Blazor Server and it's accessible via <http://localhost/> or <http://localhost/admin/>
The current features are:

## Authentication

The admin panel authenticates its users itself. Previously this was done by the
basic authentication of the reverse proxy (nginx or Traefik) against an
`.htpasswd` file, which meant there was no logout, no roles, and no protection
at all when the panel was started without a proxy in front of it.

### How the login works

The whole login - including the second factor - happens inside the running
Blazor circuit, so no page reload is needed:

1. The login component checks the credentials.
2. If the user has a second factor, the form switches to the code input in
   place.
3. Only when everything checked out, the circuit issues a single use ticket
   which the browser posts to `/auth/complete` in the background. That request
   is a normal http request, so it can set the authentication cookie.
4. The new state is pushed into the circuit, and every `AuthorizeView`
   re-renders in place.

A cookie is only ever issued after the second factor was verified, so an
authenticated session always means that the second factor was used when the
user has one.

### Where the users are stored

The users live in an own `admin` schema of the same PostgreSQL database, in the
`AdminUser` table, with an own migration history. They are deliberately not
stored in the `Account` table of the game:

* The game password is typed into the game client and travels over the game
  protocol, while an admin panel user can restart servers, edit the whole game
  configuration and read the logs.
* Every game server connects with the `account` database role and could
  otherwise read and overwrite the admin password hashes. The `admin` schema is
  not granted to any of the game server roles.
* The admin panel is the tool which creates the game database in the first
  place, so its users can't depend on it.

Passwords are hashed with BCrypt, and the TOTP secrets are encrypted with ASP.NET
Core data protection before they are stored.

### The first user

On a fresh installation there is no database and therefore no user yet. Until
the first user exists, the panel runs in an initial setup mode: it stays
reachable without a login and shows a warning. To avoid that window, configure a
bootstrap user which works without any database:

```
OPENMU_ADMIN_USER=admin
OPENMU_ADMIN_PASSWORD=<a long password>
OPENMU_ADMIN_TOTP_SECRET=<optional base32 secret>
```

The same can be configured under `AdminPanel:Auth:BootstrapUser` in the
`appsettings.json`. The bootstrap user is meant to create the first real user
and as a way back in when that's not possible anymore; changes to it are only
kept in memory.

### Two-factor authentication

Every user can set a time based one time password (TOTP) up under
_Account security_. It works with the Microsoft Authenticator app as well as
with any other authenticator app, since it uses the standard parameters
(SHA-1, 6 digits, 30 seconds) - the Microsoft Authenticator app ignores
deviating values in the otpauth uri, so they must not be changed.

The second factor is only activated after the user entered a code which the app
produced, so a mistake while scanning can't lock anybody out. Ten recovery codes
are handed out once and stored as hashes. An administrator can reset the second
factor of a user in the _Users_ page.

Set `AdminPanel:Auth:RequireTwoFactor` to `true` to require a second factor from
every user - users without one are then asked to set it up before they can use
the panel.

### Roles

There are three roles which build up on each other: `Viewer`, `Operator` and
`Administrator`. The setup, plugin, update, log file and user pages require the
`Administrator` role.

### Deployment notes

* The data protection key ring protects the cookies and the stored TOTP secrets.
  It has to be persisted, otherwise a restart invalidates all sessions and makes
  the stored secrets unreadable. The docker compose files mount the
  `adminpanel-keys` volume at `/app/data-protection-keys` for that. The path can
  be changed with `AdminPanel:Auth:DataProtectionKeyPath`.
* Terminating TLS at the reverse proxy is still recommended - the cookie is sent
  over whatever the request used.

## Server list

* Start / Shutdown
* Player count monitoring
* Links to show live maps (see below)

Ideas for the future:

* Expand-Buttons to show the players which are playing on a server
* Button to disconnect a player

## Edit Pages

To be able edit most of the data without writing some SQL, there are a generic
edit pages which is generated automatically by reflection.
Some fields can't be edited or created yet, because not all have a corresponding
Component yet.
Also keep in mind, these pages are a very technical and a generic view of the data,
so you need to know what you're doing.

More user-friendly configuration and account/character editors are planned for
the future.

## Account list

It shows the list of accounts, ordered by the login name. Functions:

* Creating new accounts

* Banning/deactivating accounts

* Clicking on Edit sends you to the generic edit page for the account.
  For example, creating Characters involves some initialization logic which
  is not done yet on the web interface.

## Game Configuration

It's possible to edit every bit of the game configuration by the generic edit page.

## Log view

It's possible to view a real-time log of the server. Because a server can generate
a lot of log messages, there are some filter-features to see only messages of a
specific player, server, and/or logger.

## Live map

It's a graphical representation of a specific map to monitor some kind of actions
on it:

* player / npc movements
* player attacks

It's implemented in WebGL (by three.js) and makes use of Blazors javascript interop
to update the visible entites.

Ideas for the future:

* Zooming in to monitor players more closely

* Display of all kind of skill animations

* Display of active magic effects (buffs etc.)

* Display of health status

* Functions to detect and show suspicious players

* Functions to directly ban suspicious players

* Overview with several maps on the same page

* View of public chats

* Game-Master features, such as:
  * Dropping of items
  * Starting automated events
  * Sending chat messages
  * Sending global messages (the golden ones)

## Other feature ideas

* Based on the Live Map, we could create a graphical editor for monster spawn
  areas, gates, etc.

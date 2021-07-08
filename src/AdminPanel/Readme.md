# Admin Panel

The admin panel is meant to offer functions for administrative tasks.

It's implemented with ASP.NET Core Blazor Server and it's accessible via <http://localhost:1234/>
The current features are:

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

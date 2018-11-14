# Admin Panel

The admin panel is meant to offer functions for administrative tasks.

The user interface is implemented with [React](https://facebook.github.io/react/), the backend with ASP.NET Core and [SignalR](http://signalr.net/).
Please don't wonder if the frontend code and my javascript "build system" is not perfect here - I'm not doing javascript development in my daily job ;-)

It's accessible via http://localhost:1234/admin
The current features are:

## Server list
  * Start / Shutdown
  * Player count monitoring
  * Links to show (embedded) live maps

Ideas for the future:
  * Expand-Buttons to show the players which are playing on a server
  * Button to disconnect a player

## Account list
It shows the list of accounts, ordered by the login name. Functions:
  * Creating new accounts
  * Banning/deactivating accounts

Feature ideas for the future:
  * Changing password of an account
  * Account editor which allows to even edit characters and their items
    
## Log view
It's possible to view a real-time log of the server. Because a server can generate a lot of log messages,
there are some filter-features to see only messages of a specific player, server, and/or logger.

## System view
A very basic real-time view of the cpu utilization, caused by the OpenMU process.

## Live map
It's a graphical representation of a specific map to monitor some kind of actions on it: 
  * player / npc movements
  * player attacks

It's implemented in WebGL (by three.js) and communicates with the server via SignalR.

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

  * Configuration editor
  * Based on the Live Map, we could create a graphical editor for monster spawn areas, gates, etc.
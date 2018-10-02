# OpenMU Project #

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build Status](https://travis-ci.org/MUnique/OpenMU.svg?branch=master)](https://travis-ci.org/MUnique/OpenMU)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/eee4aebcd9fd46888013530bd8f96a17)](https://www.codacy.com/project/sven-n/OpenMU/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=MUnique/OpenMU&amp;utm_campaign=Badge_Grade_Dashboard)
[![Gitter chat](https://badges.gitter.im/OpenMU-Project/gitter.svg)](https://gitter.im/OpenMU-Project/Lobby)

Welcome to the OpenMU project. 

This project aims to create an easy to use, extendable and customizable server for a MMORPG called "MU Online"
in the version of Season 6 Episode 3 using the ENG (english) protocol. 
However, parts of the software can also be suitable for the development of other games, even for other kind of games.

The code is a complete rewrite from scratch - it's not based on any pre-existing projects, and it's also explicitly
not based on the well-known decompiled server source of "Deathway" or one of its countless derivates.

There also exists a [blog](https://munique.net) which may contain some valuable information about this development.

## Current project state ##
This project is currently under development without any release.

## Licensing ##
This project is released under the MIT license (see LICENSE file).

## Used technologies ##
The project is mainly written in C# and targets .NET Standard/Core 2, except for some little tools which still require the full .NET Framework 4.6.1.

The servers admin panel is an embedded webserver, which is using the [React](https://reactjs.org) framework on top
of the [Nancy](http://nancyfx.org) framework.

At the moment the persistence layer uses the [Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore)
and [PostgreSQL](https://www.postgresql.org) as database.

## Contributions ##
Contributions are welcome if they meet the following criteria:

* Language is english.
* Code should be StyleCop compliant - this project uses the [StyleCop.Analyzers](https://www.nuget.org/packages/StyleCop.Analyzers/) for VS2017 so you should see issues directly as warnings.
* Coding style (naming, etc.) and quality should fit to the current state.
* No code copied/converted from the well-known decompiled source of the original server.

If you want to contribute, please create a new issue for the feature or bug (if the issue doesn't exist yet) so we
can see who is working on something and can discuss possible solutions.

Apart of that, contributions from non-developers are welcome as well. You can test the server, submit issues or
suggestions, packet descriptions or documentations about the concepts and mechanics of the game itself. Please use markdown files/syntax for this purpose.

If you have questions about that, don't hesitate to ask by submitting an issue.

## How to contribute code ##
If you want to contribute code, please do the following steps:

1. fork this project from the original MUnique OpenMU Project.
2. create a feature branch from the master branch
3. commit your changes to your feature branch
4. submit a pull request to the original master branch
5. lean back, wait for the code review and merge :)

## How to use ##
Have a look at the [quick start guide](QuickStart.md).

## Gameplay differences to the original server ##
This project doesn't have the goal to copy the original MU Online server behavior to 100 %. This is not entirely
possible, because the original server is written in another programming language and has a complete different architecture.
With some points we make our live easier in this project, with other points we try to improve the gameplay.

### Calculations ###
The calculations of attribute values (like character damage decrement etc.) is done with 32 bit float numbers and without rounding off, like the original server does at some places.

### Money drops ###
The original server drops money (zen) on the ground when a player kills a monster.
The serialization of it is a bit strange, because it's sent as 'item drop' and misuses item fields to represent the money value.
We make our live here easier, because we don't drop money. Instead, we directly add it to the inventory of the
player(s).
Benefits:
  * More performance, as we don't create a ton of new "Item" objects which are actually no items. Money drops really often on the original server.
  * Cleaner code as we don't misuse "Item" objects and don't need to invent something like a "MoneyItem".
  * This also slightly improves gameplay - picking up all the money on the ground is a bit annoying.

### Countdown when changing character or sub-server ###
The original server uses a five second countdown when a player wants to change his character or the sub-server.
Maybe this was done for some performance reasons, as the original server would then save the character/account data.
We think that's really annoying and see no real value in that, so we don't use a countdown.


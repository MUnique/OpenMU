# OpenMU Project #

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE) [![Build Status](https://travis-ci.org/MUnique/OpenMU.svg?branch=master)](https://travis-ci.org/MUnique/OpenMU)

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
The project is mainly written in C# with the help of Visual Studio 2017 and targets .NET 4.6. In the long term the goal
is to be able to run it on .NET Core.

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

Apart of that, contribtions from non-developers are welcome as well. You can test the server, submit issues or
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
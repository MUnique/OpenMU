---
title: Contributing
sidebar_position: 4
description: How to contribute code, documentation and other things to OpenMU.
---

# Contributing

Contributions are welcome if they meet the following criteria:

* Language is english.
* Code should be StyleCop compliant — this project uses the
  [StyleCop.Analyzers](https://www.nuget.org/packages/StyleCop.Analyzers/) for
  VS2022, so you should see issues directly as warnings.
* Coding style (naming, etc.) and quality should fit the current state.
* No code copied or converted from the well-known decompiled source of the
  original server.

If you want to contribute, please create a new issue for the feature or bug (if
the issue doesn't exist yet), so we can see who is working on something and can
discuss possible solutions. If it's a small thing, you can also just send a pull
request without adding an issue.

## How to contribute code

1. Fork this project from the original
   [MUnique OpenMU project](https://github.com/MUnique/OpenMU).
2. Create a feature branch from the master branch.
3. Commit your changes to your feature branch.
4. Please test your changes — **don't send AI generated code without testing it
   yourself**.
5. Submit a pull request to the original master branch.
6. Wait for the code review and merge. 🙂

## Contributions from non-developers

Contributions from non-developers are welcome as well. You can

* test the server and submit issues or suggestions,
* contribute packet descriptions,
* write documentation about the concepts and mechanics of the game itself.

Please use markdown files/syntax for this purpose.

## Contributing to this documentation

The site you are reading lives in the
[`docs-website/`](https://github.com/MUnique/OpenMU/tree/master/docs-website)
folder of the repository. Every page has an *Edit this page* link at the bottom
which takes you straight to the right file.

To run the site locally:

```bash
cd docs-website
npm install
npm start
```

Please keep the existing style: line width around 80 characters, one sentence per
idea, and a link instead of a copy when the information already exists somewhere
else.

## Questions

If you have questions, don't hesitate to ask in our
[Discord channel](https://discord.gg/2u5Agkd) or by submitting an issue.

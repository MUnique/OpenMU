# Contributions

Contributions are welcome if they meet the following criteria:

* Language is english.

* Code should be StyleCop compliant - this project uses the [StyleCop.Analyzers](https://www.nuget.org/packages/StyleCop.Analyzers/)
  for VS2022 so you should see issues directly as warnings.

* Coding style (naming, etc.) and quality should fit to the current state.

* No code copied/converted from the well-known decompiled source of the
  original server.

If you want to contribute, please create a new issue for the feature or bug (if
the issue doesn't exist yet) so we can see who is working on something and can
discuss possible solutions. If it's a small thing, you can also just send a
pull request without adding an issue.

Apart of that, contributions from non-developers are welcome as well. You can
test the server, submit issues or suggestions, packet descriptions or
documentations about the concepts and mechanics of the game itself. Please use
markdown files/syntax for this purpose.

If you have questions about that, don't hesitate to ask in our [discord channel](https://discord.gg/2u5Agkd)
or by submitting an issue.

## How to contribute code

1. fork this project from the original MUnique OpenMU Project.
2. create a feature branch from the master branch
3. commit your changes to your feature branch
4. please test your changes, don't send AI generated code without testing it yourself
5. submit a pull request to the original master branch
6. wait for the code review and merge :)

## Where to start

* [Run from source](docs-website/docs/getting-started/from-source.md) - building
  and debugging the server
* [Architecture](docs-website/docs/development/architecture.md) - how the server
  is structured internally
* [Solution structure](docs-website/docs/development/solution-structure.md) -
  what lives in which project
* [The plugin system](src/PlugIns/Readme.md) - how most features are
  implemented, and how to write your own plugin

## Contributing documentation

The documentation website is built from the [docs-website](docs-website) folder.
Every page has an *Edit this page* link at the bottom which leads to its source
file. To run it locally:

```bash
cd docs-website
npm install
npm start
```

The deeper, code-bound technical documentation stays next to the code, in
[docs](docs) and in the `Readme.md` files of the projects under [src](src).

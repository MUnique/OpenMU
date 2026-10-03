# Agent Instructions

This file is the entry point for AI coding assistants (Claude Code, Copilot,
Gemini, Codex, …) working in this repository. It also applies to human
contributors.

## Coding rules — read first

**The coding rules in [`docs/CODING_RULES.md`](docs/CODING_RULES.md) apply to all
code changes in this repository, by humans and by AI agents alike.**

Before writing or modifying code:

1. Read [`docs/CODING_RULES.md`](docs/CODING_RULES.md).
2. Apply the rules to any new or changed code.
3. When reviewing a change, treat `docs/CODING_RULES.md` as the authoritative
   style and quality baseline.

The rules are not exhaustive — they capture the conventions that matter most for
this codebase, and especially the ones which are easy to get wrong because they
involve generated code, persisted data or client compatibility. Follow the style
of the surrounding code where the rules don't speak.

## Contributing

[`CONTRIBUTING.md`](CONTRIBUTING.md) is the front door: what we accept, how to
open a pull request, and where to start. Two points from it are worth repeating
here because they are easy to miss:

* **No code copied or converted from the decompiled source of the original
  server.** See rule 9 of the coding rules for what you *may* do with those
  sources.
* **Test your change yourself.** Don't submit AI generated code you haven't run.
  See rule 13 for what that means in this repository.

## Orientation

| Topic | Where |
|---|---|
| Build and debug from source | [Run from source](docs-website/docs/getting-started/from-source.md) |
| How the server is structured | [Architecture](docs-website/docs/development/architecture.md) |
| What lives in which project | [Solution structure](docs-website/docs/development/solution-structure.md) |
| The plugin system | [`src/PlugIns/Readme.md`](src/PlugIns/Readme.md) |
| The packet protocol | [Packet documentation](docs-website/docs/reference/packets.md), [`docs/Packets`](docs/Packets) |
| Attribute / damage calculation | [`src/AttributeSystem/Readme.md`](src/AttributeSystem/Readme.md) |
| Where documentation belongs | [`docs/Readme.md`](docs/Readme.md) |
| Database migrations | [Migrations cheatsheet](src/Persistence/EntityFramework/Migrations/Cheatsheet.md) |

Most projects under [`src`](src) have their own `Readme.md` next to the code.

## Branch and pull request conventions

* The target branch for pull requests is `master`.
* Keep changes focused — one concern per pull request (see rule 1 in the coding
  rules). Smaller diffs review faster.
* Use conventional-commit subjects, as most recent commits do:
  `feat(raklion): …`, `fix(items): …`, `docs(…)`, `test(…)`, `refactor(…)`,
  `style(…)`. Lowercase, imperative, scope = the area you touched.
* For anything beyond a small fix, open an issue first so others can see who is
  working on what and the approach can be discussed.
* Reference the related issue in the pull request description when applicable.

## Out of scope for AI changes

Don't perform large retroactive cleanups of existing code to fit the rules unless
the user explicitly asks for it. Apply the rules going forward; pre-existing code
can be refactored opportunistically when you're already touching it.

Some parts of the codebase knowingly violate the rules and say so in a comment.
Leave them alone unless fixing them is the task.

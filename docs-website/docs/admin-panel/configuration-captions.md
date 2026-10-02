---
title: Configuration captions
sidebar_position: 4
description: Keep the translated names of the game configuration in sync with the translations which ship with OpenMU.
---

# Configuration captions

**Navigation:** *Captions* — route `/config-captions`

The names of built-in configuration objects, e.g. maps, monsters and character
classes, can contain translations for several languages. New OpenMU versions can
ship additional languages or corrected translations. This page compares the
captions of your configuration with these built-in sources and lets you decide
which differences to apply. No configuration update is required for that.

The page guides you through two steps:

1. If none of your captions is linked to its source yet (see
   [Link built-in captions](#link-built-in-captions)), the page only offers to link them.
2. Afterwards it lists the differences, which you can review and apply. If there
   are no differences, it shows *All available localizations are in place*.

## Kinds of changes

| Kind | Meaning | Selected by default |
| --- | --- | --- |
| New | A translation is available, but missing in your configuration (or it's just a copy of the English text). | Yes |
| Updated | The built-in text was changed and your text is still the one which was originally taken over. | Yes |
| Removed | The built-in translation doesn't exist anymore and your text is still the one which was originally taken over. | Yes |
| Customized | Your text differs from the built-in one, because it was changed (e.g. in the admin panel) or its origin is unknown. | No |

You can filter the list by language and kind, select or deselect single changes
and apply the selection. Customized texts are only overwritten if you select
them explicitly. Like configuration updates, the changes require a restart of
the server process to take effect.

## Link built-in captions

To know where a caption comes from, OpenMU stores a source reference with it
(for example `MapNames/Lorencia`). Configurations which were created with an
older version don't have these references yet, so they don't show any changes.

*Link built-in captions* executes the data initialization of your configuration
in memory and adds the references to the matching captions of your
configuration. It doesn't change any text. Captions whose English text was
customized are skipped. This is only required once; afterwards the page shows
the available differences. *Link captions again* (at the bottom of the page) links
captions which aren't linked yet, e.g. after configuration updates added new objects.

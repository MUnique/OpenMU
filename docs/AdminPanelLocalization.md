# Admin panel localization

Select **简体中文** in the admin panel language selector to use the Simplified
Chinese (`zh-CN`) resources. The selected culture is preserved by the existing
culture cookie. English remains the neutral resource language and fallback.

## Coverage

The translations cover administration pages, shared form controls, model labels
and descriptions, enum captions, configuration summaries, plugin metadata,
plugin configuration fields, and configuration update descriptions.

Regional culture names are preserved during culture selection and request
localization. Culture discovery also checks deployed satellite assemblies, so
`zh-CN` is available on platforms where ICU enumerates a different Chinese
culture name.

Plugin extension point names and authorization roles are translated for display.
Their stored identifiers are unchanged. Protocol identifiers, command words,
external plugin text without a translation, and diagnostic logs retain their
original text.

Names stored in game configuration data are separate from UI resource files.
Adding these resources does not translate existing database records or alter
configuration initialization, gameplay values, or database schemas.

## Maintaining translations

Add neutral text to the corresponding `Properties/*.resx` file and add the
translation under the same key in `*.zh-CN.resx`. Keep formatting placeholders
unchanged. Model captions use the naming conventions in `ModelResourceProvider`;
plugin `DisplayAttribute` metadata must reference public resource properties.

Keep contribution descriptions, source comments, and documentation in English.
Do not translate command syntax, packet identifiers, or authorization values.

Run the admin panel tests with:

```sh
dotnet test tests/MUnique.OpenMU.Web.Tests/MUnique.OpenMU.Web.Tests.csproj -c Release -p:ci=true
```

Localization tests cover culture selection, enum round trips, model captions and
summaries, plugin resource metadata, resource completeness and placeholders, and
paging with the shared grid state.

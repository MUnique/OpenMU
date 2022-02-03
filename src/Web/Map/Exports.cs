namespace MUnique.OpenMU.Web.Map;

using System.Collections.Immutable;

public static class Exports
{
    public static string Prefix { get; } = $"_content/{typeof(Exports).Namespace}";
    public static ImmutableList<string> Scripts = new []
    {
        $"{Prefix}/js/system-production.js",
        $"{Prefix}/js/map.js",
        $"{Prefix}/js/app.js",
        $"{Prefix}/js/Stats.js",
    }.ToImmutableList();

    public static ImmutableList<(string Key, string Path)> ScriptMappings = new (string Key, string Path)[]
    {
        ("three", $"{Prefix}/js/three.min.js"),
        ("tween", $"{Prefix}/js/tween.min.js"),
    }.ToImmutableList();

    public static ImmutableList<string> Stylesheets = new[]
    {
        $"{Prefix}/css/site.css",
    }.ToImmutableList();
}
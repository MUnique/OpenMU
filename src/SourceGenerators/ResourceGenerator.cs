// <copyright file="ResourceGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.SourceGenerators;

using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// A <see cref="IIncrementalGenerator"/> which creates resource strings for all classes and properties of
/// the data model.
/// </summary>
// [Generator]
public class ResourceGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var typeDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax or EnumDeclarationSyntax,
                transform: static (ctx, _) => (BaseTypeDeclarationSyntax)ctx.Node)
            .Collect();

        var projectDirProvider = context.AnalyzerConfigOptionsProvider
            .Select((options, _) =>
            {
                options.GlobalOptions.TryGetValue("build_property.projectdir", out var projectDir);
                return projectDir;
            });

        var compilationAndTypes = context.CompilationProvider
            .Combine(typeDeclarations)
            .Combine(projectDirProvider);

        context.RegisterSourceOutput(compilationAndTypes, (_, source) =>
        {
            var ((compilation, types), projectDir) = source;
            Execute(compilation, types, projectDir);
        });
    }

    private static void Execute(Compilation compilation, ImmutableArray<BaseTypeDeclarationSyntax> types, string? projectDir)
    {
        if (types.IsDefaultOrEmpty || string.IsNullOrEmpty(projectDir))
        {
            return;
        }

        var declaredTypes = new List<(BaseTypeDeclarationSyntax, INamedTypeSymbol)>();

        foreach (var typeDeclaration in types)
        {
            var semanticModel = compilation.GetSemanticModel(typeDeclaration.SyntaxTree);
            var declaredSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration);
            if (declaredSymbol is INamedTypeSymbol namedTypeSymbol)
            {
                declaredTypes.Add((typeDeclaration, namedTypeSymbol));
            }
        }

        var sb = StartResourceFile();

        foreach (var (declarationSyntax, namedTypeSymbol) in declaredTypes.OrderBy(tuple => tuple.Item1.Identifier.Text))
        {
            AppendResourceStrings(sb, declarationSyntax, namedTypeSymbol);
        }

        sb.AppendLine("</root>");

        var targetPath = Path.Combine(projectDir!, "Properties", "ModelResources.resx");
#pragma warning disable RS1035
        File.WriteAllText(targetPath, sb.ToString(), Encoding.UTF8);
#pragma warning restore RS1035
    }

    private static StringBuilder StartResourceFile()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"""
                       <?xml version="1.0" encoding="utf-8"?>
                       <root>
                         <resheader name="resmimetype">
                           <value>text/microsoft-resx</value>
                         </resheader>
                         <resheader name="version">
                           <value>2.0</value>
                         </resheader>
                         <resheader name="reader">
                           <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
                         </resheader>
                         <resheader name="writer">
                           <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
                         </resheader>

                       """);
        return sb;
    }

    private static void AppendResourceStrings(StringBuilder sb, BaseTypeDeclarationSyntax annotatedClass, INamedTypeSymbol declaredClassSymbol)
    {
        switch (annotatedClass)
        {
            case ClassDeclarationSyntax classDecl:
                AppendResourceStrings(sb, classDecl, declaredClassSymbol);
                break;
            case EnumDeclarationSyntax enumDecl:
                AppendResourceStrings(sb, enumDecl);
                break;
            default:
                // do nothing
                break;
        }
    }

    private static void AppendResourceStrings(StringBuilder sb, ClassDeclarationSyntax annotatedClass, INamedTypeSymbol declaredClassSymbol)
    {
        var className = annotatedClass.Identifier.Text;

        sb.AppendLine($"""
                        <data name="{className}_TypeCaption" xml:space="preserve">
                          <value>{CaptionHelper.GetTypeCaption(annotatedClass)}</value>
                        </data>
                        <data name="{className}_TypeCaptionPlural" xml:space="preserve">
                          <value>{CaptionHelper.GetPluralizedTypeCaption(annotatedClass)}</value>
                        </data>
                        <data name="{className}_TypeDescription" xml:space="preserve">
                          <value></value>
                        </data>
                      """);

        GenerateProperties(sb, declaredClassSymbol, className);
    }

    private static void AppendResourceStrings(StringBuilder sb, EnumDeclarationSyntax annotatedEnum)
    {
        var enumName = annotatedEnum.Identifier.Text;
        sb.AppendLine($"""
                         <data name="{enumName}_TypeCaption" xml:space="preserve">
                           <value>{CaptionHelper.SeparateWords(enumName)}</value>
                         </data>
                         <data name="{enumName}_TypeDescription" xml:space="preserve">
                           <value></value>
                         </data>
                       """);

        foreach (var member in annotatedEnum.Members)
        {
            sb.AppendLine($"""
                             <data name="{enumName}_{member.Identifier.Text}_Caption" xml:space="preserve">
                               <value>{CaptionHelper.SeparateWords(member.Identifier.Text)}</value>
                             </data>
                             <data name="{enumName}_{member.Identifier.Text}_Description" xml:space="preserve">
                               <value></value>
                             </data>
                           """);
        }
    }

    private static void GenerateProperties(StringBuilder sb, INamedTypeSymbol declaredClassSymbol, string className)
    {
        var properties = declaredClassSymbol
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(ps => ps.DeclaredAccessibility == Accessibility.Public);
        foreach (var property in properties)
        {
            sb.AppendLine($"""
                             <data name="{className}_{property.Name}_Caption" xml:space="preserve">
                               <value>{CaptionHelper.SeparateWords(property.Name)}</value>
                             </data>
                             <data name="{className}_{property.Name}_Description" xml:space="preserve">
                               <value></value>
                             </data>
                           """);
        }
    }
}
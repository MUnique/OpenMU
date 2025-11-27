// <copyright file="ResourceGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.IO;

namespace MUnique.OpenMU.SourceGenerators;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

/// <summary>
/// A <see cref="ISourceGenerator"/> which creates resource strings for all classes and properties of
/// the data model. It looks for the <c>CloneableAttribute</c> to identify which classes should get the generated code.
/// </summary>
[Generator]
public class ResourceGenerator : ISourceGenerator
{
    //private const string CloneableAttributeFullName = "MUnique.OpenMU.Annotations.CloneableAttribute";

    //private const string CloneableAttributeName = "CloneableAttribute";


    /// <inheritdoc />
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    /// <inheritdoc />
    public void Execute(GeneratorExecutionContext context)
    {
        if (!Debugger.IsAttached)
        {
            // Uncomment the following line to be able to debug it during the build:
            //// Debugger.Launch();
        }

        var sb = this.StartResourceFile();
        foreach (SyntaxTree tree in context.Compilation.SyntaxTrees)
        {
            var semanticModel = context.Compilation.GetSemanticModel(tree);
            foreach (var declaredClass in tree
                         .GetRoot()
                         .DescendantNodes()
                         .OfType<ClassDeclarationSyntax>())
            {
                var declaredClassSymbol = semanticModel.GetDeclaredSymbol(declaredClass);
                if (declaredClassSymbol is null)
                {
                    continue;
                }

                this.AppendResourceStrings(sb, declaredClass, declaredClassSymbol);
            }

            foreach (var declaredClass in tree
                         .GetRoot()
                         .DescendantNodes()
                         .OfType<EnumDeclarationSyntax>())
            {
                var declaredClassSymbol = semanticModel.GetDeclaredSymbol(declaredClass);
                if (declaredClassSymbol is null)
                {
                    continue;
                }

                this.AppendResourceStrings(sb, declaredClass, declaredClassSymbol);
            }
        }

        sb.AppendLine("</root>");

        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.projectdir", out var projectDir))
        {
            var targetPath = Path.Combine(projectDir, "Properties", "ModelResources.resx");
#pragma warning disable RS1035
            File.WriteAllText(targetPath, sb.ToString(), Encoding.UTF8);
#pragma warning restore RS1035
        }
    }

    private StringBuilder StartResourceFile()
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

    private void AppendResourceStrings(StringBuilder sb, ClassDeclarationSyntax annotatedClass, INamedTypeSymbol declaredClassSymbol)
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

        this.GenerateProperties(sb, declaredClassSymbol, className);
    }

    private void AppendResourceStrings(StringBuilder sb, EnumDeclarationSyntax annotatedEnum, INamedTypeSymbol declaredClassSymbol)
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

    private void GenerateProperties(StringBuilder sb, INamedTypeSymbol declaredClassSymbol, string className)
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

public static class CaptionHelper
{
    private static readonly Regex WordSeparatorRegex = new("([a-z])([A-Z])", RegexOptions.Compiled);

    /// <summary>
    /// Separates the words by a space. Words are detected by upper case letters.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The separated words.</returns>
    public static string SeparateWords(string input)
    {
        return WordSeparatorRegex.Replace(input, "$1 $2")
            .Replace(" Definitions", "s")
            .Replace(" Definition", "s");
    }

    /// <summary>
    /// Gets a nice caption for types.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>A nice caption for types.</returns>
    public static string GetTypeCaption(ClassDeclarationSyntax type)
    {
        return SeparateWords(type.Identifier.Text);
    }

    /// <summary>
    /// Gets a pluralized caption for a type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>A nice caption for types.</returns>
    public static string GetPluralizedTypeCaption(ClassDeclarationSyntax type)
    {
        var result = GetTypeCaption(type);
        result = result
            .Replace(" Definitions", "s")
            .Replace(" Definition", "s");
        if (!result.EndsWith("s"))
        {
            result += "s";
        }

        return result;
    }
}

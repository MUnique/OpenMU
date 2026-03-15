// <copyright file="CloneableGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.SourceGenerators;

using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// A <see cref="IIncrementalGenerator"/> which implements <see cref="ICloneable{}"/>
/// and for convenience also <see cref="IAssignable"/> and <see cref="IAssignable{}"/>.
/// </summary>
[Generator]
public class CloneableGenerator : IIncrementalGenerator
{
    private const string CloneableAttributeFullName = "MUnique.OpenMU.Annotations.CloneableAttribute";

    private const string CloneableAttributeName = "CloneableAttribute";

    private const string IgnoreWhenCloningAttributeName = "IgnoreWhenCloningAttribute";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: static (ctx, _) => GetClassWithCloneableAttribute(ctx))
            .Where(static m => m is not null);

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, (spc, source) => Execute(source.Left, source.Right!, spc));
    }

    private static ClassDeclarationSyntax? GetClassWithCloneableAttribute(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeList in classDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var symbolInfo = context.SemanticModel.GetSymbolInfo(attribute);
                if (symbolInfo.Symbol is IMethodSymbol attributeSymbol)
                {
                    var attributeContainingType = attributeSymbol.ContainingType;
                    var fullName = attributeContainingType.ToDisplayString();

                    if (fullName == CloneableAttributeFullName)
                    {
                        return classDeclaration;
                    }
                }
            }
        }

        return null;
    }

    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax?> classes, SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty)
        {
            return;
        }

        foreach (var classDeclaration in classes.Distinct())
        {
            if (classDeclaration is null)
            {
                continue;
            }

            var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            var declaredClassSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);
            if (declaredClassSymbol is null)
            {
                continue;
            }

            var generatedClass = GeneratePartialClass(classDeclaration, declaredClassSymbol);
            context.AddSource($"{classDeclaration.Identifier}_Cloneable", SourceText.From(generatedClass.ToString(), Encoding.UTF8));
        }
    }

    private static StringBuilder GeneratePartialClass(ClassDeclarationSyntax annotatedClass, INamedTypeSymbol declaredClassSymbol)
    {
        var sb = new StringBuilder();
        var className = annotatedClass.Identifier.Text;
        var ns = declaredClassSymbol.ContainingNamespace?.ToString() ?? string.Empty;
        var isInheritedClonable = declaredClassSymbol.BaseType?.GetAttributes().Any(a => a.AttributeClass?.Name == CloneableAttributeName) ?? false;

        sb.AppendLine($"""
                        namespace {ns};

                        using System;
                        using System.Collections.Generic;
                        using MUnique.OpenMU.DataModel;
                        using MUnique.OpenMU.DataModel.Configuration;

                        public partial class {className} : IAssignable, IAssignable<{className}>, ICloneable<{className}>
                        """);
        sb.AppendLine("{");
        sb.AppendLine($"""
                          /// <inheritdoc />
                          public virtual {className} Clone(GameConfiguration gameConfiguration)
                      """);
        sb.AppendLine("    {");
        sb.AppendLine($"""
                               var clone = new {className}();
                               clone.AssignValuesOf(this, gameConfiguration);
                               return clone;
                       """);
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($$"""
                           /// <inheritdoc />
                           public {{(isInheritedClonable ? "override" : "virtual")}} void AssignValuesOf(object other, GameConfiguration gameConfiguration)
                           {
                               if (other is {{className}} typedOther)
                               {
                                   AssignValuesOf(typedOther, gameConfiguration);
                               }
                           }

                           /// <inheritdoc />
                           public virtual void AssignValuesOf({{className}} other, GameConfiguration gameConfiguration)
                      """);
        sb.AppendLine("    {");
        if (isInheritedClonable)
        {
            sb.AppendLine("        base.AssignValuesOf(other, gameConfiguration);");
        }

        GenerateAssignments(sb, declaredClassSymbol);
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb;
    }

    private static void GenerateAssignments(StringBuilder sb, INamedTypeSymbol declaredClassSymbol)
    {
        var properties = declaredClassSymbol
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.SetMethod is not null)
            .Where(p => !p.GetAttributes().Any(a => object.Equals(a.AttributeClass?.Name, IgnoreWhenCloningAttributeName)));
        foreach (var property in properties)
        {
            if (property.SetMethod?.DeclaredAccessibility == Accessibility.Protected && property.IsVirtual)
            {
                // collection
                var genericElementType = (property.Type as INamedTypeSymbol)?.TypeArguments.FirstOrDefault();
                if (genericElementType?.IsValueType ?? false)
                {
                    sb.AppendLine($"        this.{property.Name}.AssignCollection(other.{property.Name});");
                }
                else
                {
                    sb.AppendLine($"        this.{property.Name}.AssignCollection(other.{property.Name}, gameConfiguration);");
                }
            }
            else if (property.IsVirtual)
            {
                sb.AppendLine($"        this.{property.Name} = gameConfiguration.GetObjectOfConfig(other.{property.Name});");
            }
            else
            {
                // for normal value properties, just assign the value
                sb.AppendLine($"        this.{property.Name} = other.{property.Name};");
            }
        }
    }
}
// <copyright file="CloneableGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.SourceGenerators;

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// A <see cref="ISourceGenerator"/> which implements <see cref="ICloneable{}"/>
/// and for convenience also <see cref="IAssignable"/> and <see cref="IAssignable{}"/>.
/// </summary>
[Generator]
public class CloneableGenerator : ISourceGenerator
{
    private const string CloneableAttributeFullName = "MUnique.OpenMU.Annotations.CloneableAttribute";

    private const string CloneableAttributeName = "CloneableAttribute";

    private const string IgnoreWhenCloningAttributeName = "IgnoreWhenCloningAttribute";

    /// <inheritdoc />
    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this one
    }

    /// <inheritdoc />
    public void Execute(GeneratorExecutionContext context)
    {
        var attributeSymbol = context.Compilation.GetTypeByMetadataName(CloneableAttributeFullName);

        var classWithAttributes = context.Compilation.SyntaxTrees.Where(st => st.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
            .Any(p => p.DescendantNodes().OfType<AttributeSyntax>().Any()));

        if (!Debugger.IsAttached)
        {
            // Uncomment the following line to be able to debug it during the build:
            //// Debugger.Launch();
        }

        foreach (SyntaxTree tree in classWithAttributes)
        {
            var semanticModel = context.Compilation.GetSemanticModel(tree);

            foreach (var declaredClass in tree
                         .GetRoot()
                         .DescendantNodes()
                         .OfType<ClassDeclarationSyntax>()
                         .Where(cd => cd.DescendantNodes().OfType<AttributeSyntax>().Any()))
            {
                var nodes = declaredClass
                    .DescendantNodes()
                    .OfType<AttributeSyntax>()
                    .FirstOrDefault(a => a.DescendantTokens().Any(dt => dt.IsKind(SyntaxKind.IdentifierToken) && semanticModel.GetTypeInfo(dt.Parent!).Type?.Name == attributeSymbol!.Name))
                    ?.DescendantTokens()
                    ?.Where(dt => dt.IsKind(SyntaxKind.IdentifierToken))
                    ?.ToList();

                if (nodes == null || !nodes.Any())
                {
                    continue;
                }

                var declaredClassSymbol = semanticModel.GetDeclaredSymbol(declaredClass);
                if (declaredClassSymbol is null)
                {
                    continue;
                }

                var generatedClass = this.GeneratePartialClass(declaredClass, declaredClassSymbol);

                context.AddSource($"{declaredClass.Identifier}_Cloneable", SourceText.From(generatedClass.ToString(), Encoding.UTF8));
            }
        }
    }

    private StringBuilder GeneratePartialClass(ClassDeclarationSyntax annotatedClass, INamedTypeSymbol declaredClassSymbol)
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

        this.GenerateAssignments(sb, declaredClassSymbol);
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb;
    }

    private void GenerateAssignments(StringBuilder sb, INamedTypeSymbol declaredClassSymbol)
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
// <copyright file="PlugInProxyGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns.Generators;

using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// Generates the proxies of the plugin points, which are interfaces marked with the <c>PlugInPointAttribute</c>.
/// A proxy implements the interface by calling all active plugins of the plugin point.
/// </summary>
/// <remarks>
/// Without a generated proxy, the <c>PlugInManager</c> generates it at runtime with Roslyn, which costs
/// several seconds at startup. The generated code is the same as the one of the <c>PlugInProxyTypeGenerator</c>.
/// Each proxy registers itself at the <c>PlugInProxyRegistry</c> with a module initializer.
/// Interfaces which are not supported by the <c>PlugInProxyTypeGenerator</c> are skipped, so that the behavior
/// stays the same for them.
/// </remarks>
[Generator]
public class PlugInProxyGenerator : IIncrementalGenerator
{
    private const string PlugInPointAttributeFullName = "MUnique.OpenMU.PlugIns.PlugInPointAttribute";

    private const string StrategyPlugInInterfaceName = "MUnique.OpenMU.PlugIns.IStrategyPlugIn<TKey>";

    private const string CancelEventArgsFullName = "System.ComponentModel.CancelEventArgs";

    private static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
        .AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    private enum ReturnKind
    {
        Void,
        ValueTask,
        Task,
    }

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var proxies = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                PlugInPointAttributeFullName,
                predicate: static (node, _) => node is InterfaceDeclarationSyntax,
                transform: static (ctx, _) => CreateProxyModel((INamedTypeSymbol)ctx.TargetSymbol))
            .Where(static model => model is not null);

        context.RegisterSourceOutput(proxies, static (spc, model) => spc.AddSource(model!.HintName, SourceText.From(GenerateSource(model), Encoding.UTF8)));
    }

    private static ProxyModel? CreateProxyModel(INamedTypeSymbol interfaceSymbol)
    {
        if (interfaceSymbol.IsGenericType
            || !IsAccessibleFromAssembly(interfaceSymbol)
            || interfaceSymbol.AllInterfaces.Any(i => i.OriginalDefinition.ToDisplayString() == StrategyPlugInInterfaceName))
        {
            // Strategy plugin points don't need a proxy, they're handled by the StrategyPlugInProvider.
            return null;
        }

        if (interfaceSymbol.AllInterfaces.Any(i => i.GetMembers().Any(m => m is not INamedTypeSymbol)))
        {
            // The PlugInProxyTypeGenerator just implements the members which are declared by the interface itself.
            return null;
        }

        var methods = ImmutableArray.CreateBuilder<MethodModel>();
        foreach (var member in interfaceSymbol.GetMembers())
        {
            if (member is INamedTypeSymbol)
            {
                // Nested types are no members which need to be implemented.
                continue;
            }

            if (member is not IMethodSymbol { MethodKind: MethodKind.Ordinary, IsStatic: false, IsAbstract: true, IsGenericMethod: false } method
                || method.Parameters.Any(p => p.RefKind != RefKind.None || p.IsParams))
            {
                return null;
            }

            var returnKind = GetReturnKind(method.ReturnType);
            if (returnKind is null)
            {
                return null;
            }

            var parameters = method.Parameters
                .Select(p => new ParameterModel(p.Type.ToDisplayString(TypeFormat), EscapeIdentifier(p.Name), IsCancelEventArgs(p.Type)))
                .ToImmutableArray();
            methods.Add(new MethodModel(method.Name, returnKind.Value, new EquatableArray<ParameterModel>(parameters)));
        }

        var containingTypes = string.Concat(GetContainingTypes(interfaceSymbol).Select(t => t.Name + "_"));
        var proxyName = containingTypes + (interfaceSymbol.Name.StartsWith("I") ? interfaceSymbol.Name.Substring(1) : interfaceSymbol.Name) + "Proxy";
        var ns = interfaceSymbol.ContainingNamespace.IsGlobalNamespace ? null : interfaceSymbol.ContainingNamespace.ToDisplayString();
        var hintName = interfaceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", string.Empty).Replace('<', '_').Replace('>', '_') + ".Proxy.g.cs";

        return new ProxyModel(
            hintName,
            ns,
            proxyName,
            interfaceSymbol.ToDisplayString(TypeFormat),
            new EquatableArray<MethodModel>(methods.ToImmutable()));
    }

    private static IEnumerable<INamedTypeSymbol> GetContainingTypes(INamedTypeSymbol symbol)
    {
        var containingTypes = new List<INamedTypeSymbol>();
        for (var containingType = symbol.ContainingType; containingType is not null; containingType = containingType.ContainingType)
        {
            containingTypes.Insert(0, containingType);
        }

        return containingTypes;
    }

    private static bool IsAccessibleFromAssembly(INamedTypeSymbol symbol)
    {
        for (ISymbol? current = symbol; current is INamedTypeSymbol type; current = type.ContainingType)
        {
            if (type.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal))
            {
                return false;
            }
        }

        return true;
    }

    private static ReturnKind? GetReturnKind(ITypeSymbol returnType)
    {
        if (returnType.SpecialType == SpecialType.System_Void)
        {
            return ReturnKind.Void;
        }

        return returnType.ToDisplayString() switch
        {
            "System.Threading.Tasks.ValueTask" => ReturnKind.ValueTask,
            "System.Threading.Tasks.Task" => ReturnKind.Task,
            _ => null,
        };
    }

    private static bool IsCancelEventArgs(ITypeSymbol type)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (current.ToDisplayString() == CancelEventArgsFullName)
            {
                return true;
            }
        }

        return false;
    }

    private static string EscapeIdentifier(string name)
    {
        return SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None ? "@" + name : name;
    }

    private static string GenerateSource(ProxyModel model)
    {
        var builder = new StringBuilder();
        builder.AppendLine("// <auto-generated />");
        builder.AppendLine("#nullable enable");
        builder.AppendLine();
        if (model.Namespace is not null)
        {
            builder.Append("namespace ").Append(model.Namespace).AppendLine(";");
            builder.AppendLine();
        }

        builder.AppendLine("/// <summary>");
        builder.Append("/// The generated proxy of the plugin point <see cref=\"").Append(model.InterfaceName.Replace('<', '{').Replace('>', '}')).AppendLine("\"/>.");
        builder.AppendLine("/// </summary>");
        builder.AppendLine("[global::System.CodeDom.Compiler.GeneratedCode(\"MUnique.OpenMU.PlugIns.Generators.PlugInProxyGenerator\", \"1.0\")]");
        builder.Append("internal sealed class ").Append(model.ProxyName)
            .Append(" : global::MUnique.OpenMU.PlugIns.PlugInContainerBase<").Append(model.InterfaceName).Append(">, ")
            .AppendLine(model.InterfaceName);
        builder.AppendLine("{");
        builder.Append("    public ").Append(model.ProxyName).AppendLine("(global::MUnique.OpenMU.PlugIns.PlugInManager manager)");
        builder.AppendLine("        : base(manager)");
        builder.AppendLine("    {");
        builder.AppendLine("    }");

        foreach (var method in model.Methods)
        {
            AppendMethod(builder, model.InterfaceName, method);
        }

        builder.AppendLine();
        builder.AppendLine("    [global::System.Runtime.CompilerServices.ModuleInitializer]");
        builder.AppendLine("    internal static void Register()");
        builder.AppendLine("    {");
        builder.Append("        global::MUnique.OpenMU.PlugIns.PlugInProxyRegistry.Register<").Append(model.InterfaceName).Append(">(manager => new ")
            .Append(model.ProxyName).AppendLine("(manager));");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        return builder.ToString();
    }

    private static void AppendMethod(StringBuilder builder, string interfaceName, MethodModel method)
    {
        var isAsync = method.ReturnKind != ReturnKind.Void;
        var returnType = method.ReturnKind switch
        {
            ReturnKind.ValueTask => "global::System.Threading.Tasks.ValueTask",
            ReturnKind.Task => "global::System.Threading.Tasks.Task",
            _ => "void",
        };

        builder.AppendLine();
        builder.Append("    public ").Append(isAsync ? "async " : string.Empty).Append(returnType).Append(' ').Append(method.Name).Append('(')
            .Append(string.Join(", ", method.Parameters.Select(p => p.Type + " " + p.Name)))
            .AppendLine(")");
        builder.AppendLine("    {");
        builder.AppendLine(isAsync
            ? "        using var l = await this.Lock.ReaderLockAsync();"
            : "        using var l = this.Lock.ReaderLock();");
        builder.Append("        foreach (").Append(interfaceName).AppendLine(" plugIn in this.ActivePlugIns)");
        builder.AppendLine("        {");

        var call = (isAsync ? "await " : string.Empty) + "plugIn." + method.Name + "(" + string.Join(", ", method.Parameters.Select(p => p.Name)) + ");";

        // Like the PlugInProxyTypeGenerator, just the first parameter of type CancelEventArgs is checked.
        var cancelEventArgs = method.Parameters.FirstOrDefault(p => p.IsCancelEventArgs);
        if (cancelEventArgs is not null)
        {
            builder.Append("            if (!").Append(cancelEventArgs.Name).AppendLine(".Cancel)");
            builder.AppendLine("            {");
            builder.Append("                ").AppendLine(call);
            builder.AppendLine("            }");
        }
        else
        {
            builder.Append("            ").AppendLine(call);
        }

        builder.AppendLine("        }");
        builder.AppendLine("    }");
    }

    private sealed record ProxyModel(string HintName, string? Namespace, string ProxyName, string InterfaceName, EquatableArray<MethodModel> Methods);

    private sealed record MethodModel(string Name, ReturnKind ReturnKind, EquatableArray<ParameterModel> Parameters);

    private sealed record ParameterModel(string Type, string Name, bool IsCancelEventArgs);
}

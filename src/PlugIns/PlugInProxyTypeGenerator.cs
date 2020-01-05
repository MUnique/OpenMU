// <copyright file="PlugInProxyTypeGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

    /// <summary>
    /// Generates the implementations of <see cref="IPlugInContainer{TPlugIn}"/> for specific plugin interface types.
    /// </summary>
    internal class PlugInProxyTypeGenerator
    {
        /// <summary>
        /// Generates the proxy for the given <typeparamref name="TPlugIn"/> interface.
        /// </summary>
        /// <typeparam name="TPlugIn">The type of the plug in.</typeparam>
        /// <param name="manager">The manager.</param>
        /// <returns>The generated proxy implementation.</returns>
        /// <exception cref="ArgumentException">
        /// Generic type argument {typeof(TPlugIn)}
        /// or
        /// Generic type argument {typeof(TPlugIn)} is not marked with {typeof(PlugInPointAttribute)}.
        /// </exception>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found.</exception>
        public IPlugInContainer<TPlugIn> GenerateProxy<TPlugIn>(PlugInManager manager)
        {
            var type = typeof(TPlugIn);
            if (!type.IsInterface)
            {
                throw new ArgumentException($"Generic type argument {typeof(TPlugIn)} is not an interface.");
            }

            var attribute = type.GetCustomAttribute<PlugInPointAttribute>();
            if (attribute == null)
            {
                throw new ArgumentException($"Generic type argument {typeof(TPlugIn)} is not marked with {typeof(PlugInPointAttribute)}.");
            }

            var syntaxFactory = CompilationUnit();
            var namespaceSyntax = NamespaceDeclaration(ParseName(this.GetType().Namespace + ".Proxies"))
                .AddUsings(
                    UsingDirective(ParseName("System")),
                    UsingDirective(ParseName("System.Collections.Generic")),
                    UsingDirective(ParseName(typeof(TPlugIn).Namespace)));
            var referencedNamespaces = this.GetReferencedNamespaces(type).Select(ns => UsingDirective(ParseName(ns)));
            namespaceSyntax = namespaceSyntax.AddUsings(referencedNamespaces.ToArray());
            var typeSyntax = this.ImplementProxyType<TPlugIn>(type);

            namespaceSyntax = namespaceSyntax.AddMembers(typeSyntax);
            syntaxFactory = syntaxFactory.AddMembers(namespaceSyntax).NormalizeWhitespace();
            var proxyAssembly = syntaxFactory.SyntaxTree.CompileAndLoad(typeSyntax.Identifier.Text);
            var proxyType = proxyAssembly.GetType(namespaceSyntax.Name + "." + typeSyntax.Identifier.Text);
            return Activator.CreateInstance(proxyType, manager) as IPlugInContainer<TPlugIn>;
        }

        private IEnumerable<string> GetReferencedNamespaces(Type type)
        {
            return type.GetMethods().SelectMany(method => method.GetParameters().Select(p => p.ParameterType.Namespace)).Distinct();
        }

        private string GetTypeName(Type type)
        {
            if (type.DeclaringType != null)
            {
                return this.GetTypeName(type.DeclaringType) + "." + type.Name;
            }

            return type.Name;
        }

        private ClassDeclarationSyntax ImplementProxyType<TPlugIn>(Type type)
        {
            var typeName = this.GetTypeName(type);
            var proxyTypeName = typeof(TPlugIn).Name.Substring(1) + "Proxy";
            var typeSyntax = ClassDeclaration(proxyTypeName)
                            .AddModifiers(Token(SyntaxKind.PublicKeyword))
                            .AddBaseListTypes(
                                SimpleBaseType(ParseTypeName($"{nameof(PlugInContainerBase<object>).Split('`').First()}<{typeName}>")),
                                SimpleBaseType(ParseTypeName(typeName)));
            typeSyntax = typeSyntax.AddMembers(this.ImplementConstructor(proxyTypeName));
            foreach (var method in type.GetMethods().Where(m => m.ReturnType == typeof(void)))
            {
                MethodDeclarationSyntax methodDeclaration = this.ImplementMethod(type, method);
                typeSyntax = typeSyntax.AddMembers(methodDeclaration);
            }

            return typeSyntax;
        }

        private ConstructorDeclarationSyntax ImplementConstructor(string proxyTypeName)
        {
            return ConstructorDeclaration(proxyTypeName)
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(ParameterList(SeparatedList(
                            new[]
                            {
                                Parameter(
                                    List<AttributeListSyntax>(),
                                    TokenList(),
                                    ParseTypeName(nameof(PlugInManager)),
                                    ParseToken("manager"),
                                    null),
                            })))
                .WithInitializer(
                    ConstructorInitializer(SyntaxKind.BaseConstructorInitializer)
                        .AddArgumentListArguments(Argument(IdentifierName("manager"))))
                .WithBody(Block()); // empty body
        }

        private MethodDeclarationSyntax ImplementMethod(Type type, MethodInfo method)
        {
            const string forEachVariableName = "plugIn";
            var methodDeclaration = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), method.Name)
                                .AddModifiers(Token(SyntaxKind.PublicKeyword));
            var methodCallStatement = forEachVariableName + "." + method.Name + "(";
            bool first = true;
            ParameterInfo cancelEventArgs = null;
            foreach (var parameter in method.GetParameters())
            {
                methodDeclaration = methodDeclaration.AddParameterListParameters(Parameter(
                    List<AttributeListSyntax>(),
                    TokenList(),
                    ParseTypeName(this.GetTypeName(parameter.ParameterType)),
                    ParseToken(parameter.Name),
                    null));
                if (!first)
                {
                    methodCallStatement += ", ";
                }

                methodCallStatement += parameter.Name;
                cancelEventArgs = cancelEventArgs ?? (parameter.ParameterType == typeof(CancelEventArgs) || parameter.ParameterType.IsSubclassOf(typeof(CancelEventArgs)) ? parameter : null);

                first = false;
            }

            methodCallStatement += ");";
            var methodCall = ParseStatement(methodCallStatement);
            BlockSyntax forEachBody = Block(methodCall);
            if (cancelEventArgs != null)
            {
                forEachBody = Block(IfStatement(ParseExpression("!" + cancelEventArgs.Name + ".Cancel"), forEachBody));
            }

            BlockSyntax forEachBlock = Block(
                ForEachStatement(
                    ParseTypeName(this.GetTypeName(type)),
                    forEachVariableName,
                    ParseExpression("this.ActivePlugIns"),
                    forEachBody));

            BlockSyntax methodBody = Block(
                ParseStatement("this.LockSlim.EnterReadLock();"),
                TryStatement(
                    Token(SyntaxKind.TryKeyword),
                    forEachBlock,
                    List<CatchClauseSyntax>(), // no catch clause
                    FinallyClause(
                        Block(ParseStatement("this.LockSlim.ExitReadLock();")))));

            methodDeclaration = methodDeclaration.WithBody(methodBody);
            return methodDeclaration;
        }
    }
}

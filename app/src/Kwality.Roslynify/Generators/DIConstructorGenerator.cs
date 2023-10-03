// =====================================================================================================================
// = LICENSE:       Copyright (c) 2023 Kevin De Coninck
// =
// =                Permission is hereby granted, free of charge, to any person
// =                obtaining a copy of this software and associated documentation
// =                files (the "Software"), to deal in the Software without
// =                restriction, including without limitation the rights to use,
// =                copy, modify, merge, publish, distribute, sublicense, and/or sell
// =                copies of the Software, and to permit persons to whom the
// =                Software is furnished to do so, subject to the following
// =                conditions:
// =
// =                The above copyright notice and this permission notice shall be
// =                included in all copies or substantial portions of the Software.
// =
// =                THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// =                EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// =                OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// =                NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// =                HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// =                WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// =                FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// =                OTHER DEALINGS IN THE SOFTWARE.
// =====================================================================================================================
namespace Kwality.Roslynify.Generators;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Kwality.Roslynify.Common.Constraints.Roslyn.Node;
using Kwality.Roslynify.Common.Constraints.Roslyn.Syntax.Attribute;
using Kwality.Roslynify.Common.Constraints.Roslyn.Syntax.Type;
using Kwality.Roslynify.Common.Extensions.Roslyn.Context;
using Kwality.Roslynify.Common.Extensions.Roslyn.Symbol;
using Kwality.Roslynify.Common.Extensions.Roslyn.Syntax;
using Kwality.Roslynify.Common.Models.Output;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator(LanguageNames.CSharp)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class DIConstructorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Add the "marker" attribute.
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource("DIConstructor.Attribute.g.cs", SourceText.From(
            """
            [System.AttributeUsage(System.AttributeTargets.Class)]
            public sealed class DIConstructorAttribute : System.Attribute
            {
                // NOTE: Intentionally left blank.
                //       Used as a "marker" attribute.
            }
            """, Encoding.UTF8)));

        // Stage 1:
        // - Predicate: Locate all the "Marker" attributes that are applied to a "class".
        // - Transformation: Get the types (class) that the "Marker" attribute is placed on.
        var types = context.SyntaxProvider.CreateSyntaxProvider(
                (node, _) => SelectClassesWithTheMarkerAttribute(node), (ctx, _) => ExtractClass(ctx))
            .Where(t => t is not null).Collect();

        // Final: Generate the source code for all the types that's found.
        context.RegisterSourceOutput(types, GenerateSymbolSource);
    }

    private static bool SelectClassesWithTheMarkerAttribute(SyntaxNode node)
    {
        return new IsAttributeConstraint().IsTrueFor(node) &&
               new HasNameConstraint("DIConstructorAttribute").IsTrueFor((AttributeSyntax)node) &&
               new IsAppliedToClassConstraint().IsTrueFor((AttributeSyntax)node);
    }

    private static ITypeSymbol? ExtractClass(GeneratorSyntaxContext context)
    {
        return IsNodeValid(context.Node.Parent?.Parent)
            ? ((AttributeSyntax)context.Node).GetTargetClass(context)
            : null;
    }

    private static bool IsNodeValid(SyntaxNode? syntaxNode)
    {
        return syntaxNode switch
        {
            ClassDeclarationSyntax syntax => ValidateTypeSyntax(syntax),
            InterfaceDeclarationSyntax syntax => ValidateTypeSyntax(syntax),
            StructDeclarationSyntax syntax => ValidateTypeSyntax(syntax),
            _ => true
        };

        bool ValidateTypeSyntax(TypeDeclarationSyntax syntax)
        {
            return new IsPartialConstraint().IsTrueFor(syntax) && IsNodeValid(syntax.Parent);
        }
    }

    private static void GenerateSymbolSource(SourceProductionContext context, ImmutableArray<ITypeSymbol?> types)
    {
        if (types.IsDefaultOrEmpty) return;

        foreach (var symbol in (IEnumerable<ITypeSymbol>)types.Where(x => x != null))
        {
            // Stop generating if it was requested.
            context.CancellationToken.ThrowIfCancellationRequested();

            // Add the source for the current symbol.
            AddSymbolSource(context, symbol);
        }
    }

    private static Type GetNestedTypeName(ITypeSymbol symbol, int level = 0)
    {
        if (symbol.ContainingType == null) return new Type(symbol);

        var parentName = GetNestedTypeName(symbol.ContainingType, level + 1);
        var currentName = new Type(symbol, level + 1);
        parentName.Next = currentName;

        return parentName;
    }

    private static void AddSymbolSource(SourceProductionContext context, ITypeSymbol symbol)
    {
        var (fileName, @namespace) = symbol.GetNamespace() is { } symbolNamespace
            ? ($"{symbolNamespace}.{symbol.Name}.g.cs", symbolNamespace)
            : ($"{symbol.Name}.g.cs", null);

        context.AddFile(new File(fileName, @namespace, GetNestedTypeName(symbol)));
    }
}

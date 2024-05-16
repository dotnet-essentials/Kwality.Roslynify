// =====================================================================================================================
// == LICENSE:       Copyright (c) 2024 Kevin De Coninck
// ==
// ==                Permission is hereby granted, free of charge, to any person
// ==                obtaining a copy of this software and associated documentation
// ==                files (the "Software"), to deal in the Software without
// ==                restriction, including without limitation the rights to use,
// ==                copy, modify, merge, publish, distribute, sublicense, and/or sell
// ==                copies of the Software, and to permit persons to whom the
// ==                Software is furnished to do so, subject to the following
// ==                conditions:
// ==
// ==                The above copyright notice and this permission notice shall be
// ==                included in all copies or substantial portions of the Software.
// ==
// ==                THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// ==                EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// ==                OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// ==                NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// ==                HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// ==                WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// ==                FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// ==                OTHER DEALINGS IN THE SOFTWARE.
// =====================================================================================================================
namespace Kwality.Roslynify.Generators;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Kwality.Roslynify.Common.Code.Definitions;
using Kwality.Roslynify.Common.Code.Definitions.Enumerations;
using Kwality.Roslynify.Common.Extensions.Attributes;
using Kwality.Roslynify.Common.Extensions.Symbols;
using Kwality.Roslynify.Common.Extensions.Syntax;
using Kwality.Roslynify.Generators.Base;
using Kwality.Roslynify.Models;
using Kwality.Roslynify.Models.Output;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator(LanguageNames.CSharp)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class DIConstructorGenerator : MarkerBasedIncrementalGenerator
{
    private const string markerAttributeName = "DIConstructorAttribute";

    private readonly AttributeDefinition markerAttribute
        = new(markerAttributeName, AttributeTargets.Class, TypeAccessibility.Public);

    protected override string MarkerAttributeFileName => "DIConstructor.Attribute.g.cs";
    protected override SourceText MarkerAttribute => SourceText.From(this.markerAttribute.ToString(), Encoding.UTF8);

    protected override void Setup(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<ISymbol?>> types = context
                                                                   .SyntaxProvider
                                                                   .CreateSyntaxProvider(
                                                                       static (node, _) => FindCandidates(node),
                                                                       static (ctx, _) => ExtractClasses(ctx))
                                                                   .Where(static typeSymbol => typeSymbol is not null)
                                                                   .Collect();

        context.RegisterSourceOutput(types, GenerateSymbolSource);
    }

    private static bool FindCandidates(SyntaxNode node)
    {
        return node is AttributeSyntax { Parent.Parent: not null } attributeSyntax &&
               attributeSyntax.Parent.Parent.HasRecursivePartialParent() &&
               attributeSyntax.HasName(markerAttributeName) && attributeSyntax.IsAppliedTo(TypeKind.Class);
    }

    private static ISymbol? ExtractClasses(GeneratorSyntaxContext context)
    {
        bool isValid = context.Node.Parent?.Parent switch
        {
            ClassDeclarationSyntax syntax => syntax.Modifiers.Any(static x => x.IsKind(SyntaxKind.PartialKeyword)),
            _ => false,
        };

        if (isValid)
        {
            return ((AttributeSyntax)context.Node).Parent?.Parent is ClassDeclarationSyntax @class
                ? context.SemanticModel.GetDeclaredSymbol(@class)
                : null;
        }

        return null;
    }

    private static void GenerateSymbolSource(SourceProductionContext context, ImmutableArray<ISymbol?> types)
    {
        if (types.IsDefaultOrEmpty)
        {
            return;
        }

        foreach (ITypeSymbol symbol in types.Where(static x => x != null)
                                            .OfType<ITypeSymbol>())
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            AddSymbolSource(context, symbol);
        }
    }

    private static void AddSymbolSource(SourceProductionContext context, ITypeSymbol typeSymbol)
    {
        (string? fileName, string? @namespace) = typeSymbol.GetNamespace() is { } symbolNamespace
            ? ($"{symbolNamespace}.{typeSymbol.Name}.g.cs", symbolNamespace)
            : ($"{typeSymbol.Name}.g.cs", null);

        var file = new File(fileName, @namespace, new TypeSymbolStack(typeSymbol));
        context.AddSource(file.Name, SourceText.From(file.ToString(), Encoding.UTF8));
    }
}

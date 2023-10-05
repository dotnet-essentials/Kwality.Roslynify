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
namespace Kwality.Roslynify.Analyzers;

using System.Collections.Immutable;

using Kwality.Roslynify.Common.Constraints.Roslyn.Syntax.Attribute.Data;
using Kwality.Roslynify.Common.Options;
using Kwality.Roslynify.Properties;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ProducesResponseTypeMustHaveXmlCommentAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.KW003, DiagnosticDescriptors.KW004);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);

        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(RegisterCompilationStart);
    }

    private static void RegisterCompilationStart(CompilationStartAnalysisContext startContext)
    {
        var optionsProvider = startContext.Options.AnalyzerConfigOptionsProvider;

        startContext.RegisterSymbolAction(actionContext => InspectMethod(actionContext, optionsProvider),
            SymbolKind.Method);
    }

    private static void InspectMethod(SymbolAnalysisContext context, AnalyzerConfigOptionsProvider options)
    {
        var method = (IMethodSymbol)context.Symbol;

        var attributes = method.GetAttributes().Where(attribute =>
            new HasFullyQualifiedNameConstraint("Microsoft.AspNetCore.Mvc.ProducesResponseType").IsTrueFor(attribute));

        foreach (var attribute in attributes)
            switch (attribute.ConstructorArguments.Length)
            {
                case 1:
                    ReportDiagnosticOnAttributeLevel(context, attribute);

                    break;
                case 2:
                    AnalyzeStatusCode(context, options, attribute, method,
                        Convert.ToInt32(attribute.ConstructorArguments[1].Value));

                    break;
            }
    }

    private static void AnalyzeStatusCode(SymbolAnalysisContext context, AnalyzerConfigOptionsProvider options,
        AttributeData attribute, ISymbol methodSymbol, int statusCode)
    {
        if (methodSymbol.Locations[0].SourceTree is not { } sourceTree) return;

        var optionsReader = new OptionsReader(options, sourceTree);

        if (optionsReader.ReadInts("roslynify_kw004_allowed_status_codes") is not { } allowedStatusCodes) return;
        if (allowedStatusCodes.Contains(statusCode)) return;
        if (attribute.ApplicationSyntaxReference is not { } attributeSyntaxReference) return;

        var attributeSyntax = attributeSyntaxReference.GetSyntax();

        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.KW004, attributeSyntax.GetLocation(),
            methodSymbol.Name, statusCode);

        context.ReportDiagnostic(diagnostic);
    }

    private static void ReportDiagnosticOnAttributeLevel(SymbolAnalysisContext context, AttributeData responseType)
    {
        if (responseType.ApplicationSyntaxReference is not { } attributeSyntaxReference) return;

        var location = attributeSyntaxReference.GetSyntax().GetLocation();
        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.KW003, location));
    }
}

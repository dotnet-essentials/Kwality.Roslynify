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
using System.Diagnostics.CodeAnalysis;

using Kwality.Roslynify.Common.Constraints.Roslyn.Syntax.Type;
using Kwality.Roslynify.Common.Extensions.Roslyn.Syntax;
using Kwality.Roslynify.Properties;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class DIConstructorAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.KW001, DiagnosticDescriptors.KW002);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);

        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(InspectType, SyntaxKind.ClassDeclaration);
    }

    private static void InspectType(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclaration) return;

        var isPartial = new IsPartialConstraint().IsTrueFor(classDeclaration);

        switch (classDeclaration.HasAttribute("DIConstructorAttribute"))
        {
            case true when !isPartial:
                ReportDiagnostic(context, classDeclaration, DiagnosticDescriptors.KW001);

                break;
            case true when isPartial && !classDeclaration.GetConstructorArguments(context).Any():
                ReportDiagnostic(context, classDeclaration, DiagnosticDescriptors.KW002);

                break;
        }
    }

    private static void ReportDiagnostic(SyntaxNodeAnalysisContext context,
        BaseTypeDeclarationSyntax typeDeclarationSyntax, DiagnosticDescriptor diagnosticDescriptor)
    {
        var diagnostic = Diagnostic.Create(diagnosticDescriptor, typeDeclarationSyntax.GetLocation(),
            typeDeclarationSyntax.Identifier.ValueText);

        context.ReportDiagnostic(diagnostic);
    }
}

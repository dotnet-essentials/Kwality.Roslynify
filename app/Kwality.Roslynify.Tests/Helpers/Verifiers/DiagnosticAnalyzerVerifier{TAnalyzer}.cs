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
namespace Kwality.Roslynify.Tests.Helpers.Verifiers;

using System.Collections.Immutable;
using System.Globalization;

using Kwality.Roslynify.Tests.Helpers.Models;
using Kwality.Roslynify.Tests.Helpers.Verifiers.Base;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using Xunit;

internal sealed class DiagnosticAnalyzerVerifier<TAnalyzer> : RoslynComponentVerifier
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public DiagnosticResult[]? ExpectedDiagnostics { get; set; } = [];

    public async Task VerifyAsync()
    {
        // Arrange.
        Compilation compilation = this.CreateCompilation();

        // Act.
        CompilationWithAnalyzers withAnalyzers = compilation.WithAnalyzers([new TAnalyzer()]);

        ImmutableArray<Diagnostic> diagnostics = await withAnalyzers.GetAnalyzerDiagnosticsAsync()
                                                                    .ConfigureAwait(false);

        // Assert.
        this.AssertExpectedDiagnostics(diagnostics.ToArray());
        this.AssertNoUnexpectedDiagnostic(diagnostics.ToArray());
    }

    private void AssertExpectedDiagnostics(Diagnostic[] diagnostics)
    {
        if (this.ExpectedDiagnostics == null)
        {
            return;
        }

        foreach (DiagnosticResult diagnostic in this.ExpectedDiagnostics.Where(_ => !diagnostics.Any(this.IsExpected)))
        {
            Assert.Fail($"Diagnostic \"{diagnostic.Id}\" with message \"{diagnostic.Message}\" was not reported.");
        }

        foreach (DiagnosticResult diagnostic in this.ExpectedDiagnostics.Where(_ => !diagnostics.Any(this.IsExpected)))
        {
            Assert.Fail($"Diagnostic \"{diagnostic.Id}\" with message \"{diagnostic.Message}\" was not reported.");
        }
    }

    private void AssertNoUnexpectedDiagnostic(IReadOnlyList<Diagnostic> diagnostics)
    {
        if (diagnostics.Count <= 0)
        {
            return;
        }

        Diagnostic diagnostic = diagnostics[0];
        string diagnosticId = diagnostic.Id;

        int diagnosticLocation = diagnostic.Location.GetLineSpan()
                                           .StartLinePosition.Line + 1;

        if (!this.IsExpected(diagnostic))
        {
            Assert.Fail($"Unexpected diagnostic reported: \"{diagnosticId}\" at line {diagnosticLocation}");
        }
    }

    private bool IsExpected(Diagnostic diagnostic)
    {
        return (this.ExpectedDiagnostics ?? []).Any(x =>
            diagnostic.Id == x.Id && diagnostic.GetMessage(CultureInfo.InvariantCulture) == x.Message &&
            diagnostic.Severity == x.Severity);
    }
}

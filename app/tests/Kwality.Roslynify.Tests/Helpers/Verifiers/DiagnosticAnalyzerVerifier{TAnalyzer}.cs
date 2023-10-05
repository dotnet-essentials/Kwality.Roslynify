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
namespace Kwality.Roslynify.Tests.Helpers.Verifiers;

using System.Collections.Immutable;

using Kwality.Roslynify.Tests.Helpers.Verifiers.Base;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using Xunit;

public record DiagnosticResult(string Id, string Message, DiagnosticSeverity Severity);

internal sealed class DiagnosticAnalyzerVerifier<TAnalyzer> : RoslynComponentVerifier
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public DiagnosticResult[]? ExpectedDiagnostics { get; init; }

    public async Task VerifyAsync()
    {
        // Arrange.
        var compilation = this.CreateCompilation();
        var analyzer = new TAnalyzer() as DiagnosticAnalyzer;

        // ACT.
        var withAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create(analyzer));
        var diagnostics = await withAnalyzers.GetAnalyzerDiagnosticsAsync().ConfigureAwait(false);

        // ASSERT.
        this.AssertExpectedDiagnostics(diagnostics.ToArray());
        this.AssertNoUnexpectedDiagnostic(diagnostics.ToArray());
    }

    private void AssertExpectedDiagnostics(Diagnostic[] diagnostics)
    {
        if (this.ExpectedDiagnostics == null) return;

        foreach (var (diagnosticId, diagnosticMessage, _) in this.ExpectedDiagnostics)
            if (!diagnostics.Any(this.IsExpected))
                Assert.Fail($"Diagnostic \"{diagnosticId}\" with message \"{diagnosticMessage}\" was not reported.");
    }

    private void AssertNoUnexpectedDiagnostic(IReadOnlyList<Diagnostic> diagnostics)
    {
        if (diagnostics.Count <= 0) return;

        var diagnostic = diagnostics[0];
        var diagnosticId = diagnostic.Id;
        var diagnosticLocation = diagnostic.Location.GetLineSpan().StartLinePosition.Line + 1;

        if (!this.IsExpected(diagnostic))
            Assert.Fail($"Unexpected diagnostic reported: \"{diagnosticId}\" at line {diagnosticLocation}");
    }

    private bool IsExpected(Diagnostic diagnostic)
    {
        return (this.ExpectedDiagnostics ?? Array.Empty<DiagnosticResult>()).Any(x =>
            diagnostic.Id == x.Id && diagnostic.GetMessage() == x.Message && diagnostic.Severity == x.Severity);
    }
}

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
namespace Kwality.Roslynify.Tests.Analyzers;

using Kwality.Roslynify.Analyzers;
using Kwality.Roslynify.Tests.Helpers.Verifiers;

using Microsoft.CodeAnalysis;

using Xunit;

public sealed class ProducesResponseTypeMustHaveXmlCommentAnalyzerTests
{
    [Fact(DisplayName = "Using a custom `ProducesResponseType` attribute does NOT report a diagnostic.")]
    public async Task UsingACustomProducesResponseTypeAttributeDoesNotReportADiagnostic()
    {
        // Arrange, act & assert.
        await new DiagnosticAnalyzerVerifier<ProducesResponseTypeMustHaveXmlCommentAnalyzer>
        {
            InputSources = new[]
            {
                """
                namespace Lib;

                [System.AttributeUsage(System.AttributeTargets.Class)]
                public sealed class ProducesResponseTypeAttribute : System.Attribute
                {
                }
                """,
                """
                namespace Lib;

                public sealed class Handler
                {
                    [ProducesResponseType]
                    public static void HandleRequest()
                    {
                    }
                }
                """
            }
        }.VerifyAsync();
    }

    [Fact(DisplayName = "Using the `ProducesResponseType` (short) attribute without a type reports a diagnostic.")]
    public async Task UsingTheProducesResponseTypeShortAttributeWithoutTypeReportsADiagnostic()
    {
        // Arrange, act & assert.
        await new DiagnosticAnalyzerVerifier<ProducesResponseTypeMustHaveXmlCommentAnalyzer>
        {
            InputSources = new[]
            {
                """
                namespace Lib;

                using Microsoft.AspNetCore.Http;
                using Microsoft.AspNetCore.Mvc;

                public sealed class Handler
                {
                    [ProducesResponseType(200)]
                    public static void HandleRequest()
                    {
                    }
                }
                """
            },
            ExpectedDiagnostics = new[]
            {
                new DiagnosticResult("KW003", "The attribute `ProducesResponseType` must have a valid type",
                    DiagnosticSeverity.Warning)
            }
        }.VerifyAsync();
    }

    [Fact(DisplayName = "Using the `ProducesResponseType` (long) attribute without a type reports a diagnostic.")]
    public async Task UsingTheProducesResponseTypeLongAttributeWithoutTypeReportsADiagnostic()
    {
        // Arrange, act & assert.
        await new DiagnosticAnalyzerVerifier<ProducesResponseTypeMustHaveXmlCommentAnalyzer>
        {
            InputSources = new[]
            {
                """
                namespace Lib;

                using Microsoft.AspNetCore.Http;
                using Microsoft.AspNetCore.Mvc;

                public sealed class Handler
                {
                    [ProducesResponseTypeAttribute(200)]
                    public static void HandleRequest()
                    {
                    }
                }
                """
            },
            ExpectedDiagnostics = new[]
            {
                new DiagnosticResult("KW003", "The attribute `ProducesResponseType` must have a valid type",
                    DiagnosticSeverity.Warning)
            }
        }.VerifyAsync();
    }
}

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

using System.Diagnostics.CodeAnalysis;

using Kwality.Roslynify.Analyzers;
using Kwality.Roslynify.Tests.Helpers.Verifiers;

using Microsoft.CodeAnalysis;

using Xunit;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class DIConstructorAnalyzerTests
{
    [Fact(DisplayName = "Using the `short` attribute on a NON `partial` class does report a diagnostic.")]
    public async Task UsingTheShortAttributeOnANonPartialClassReportsADiagnostic()
    {
        // Arrange, act & assert.
        await new DiagnosticAnalyzerVerifier<DIConstructorAnalyzer>
        {
            InputSources = new[]
            {
                """
                [DIConstructor]
                public class UserManager { }
                """
            },
            ExpectedDiagnostics = new[]
            {
                new DiagnosticResult("KW001", "The class `UserManager` must be defined as `partial`",
                    DiagnosticSeverity.Error)
            }
        }.VerifyAsync();
    }

    [Fact(DisplayName = "Using the `long` attribute on a NON `partial` class does report a diagnostic.")]
    public async Task UsingTheLongAttributeOnANonPartialClassReportsADiagnostic()
    {
        // Arrange, act & assert.
        await new DiagnosticAnalyzerVerifier<DIConstructorAnalyzer>
        {
            InputSources = new[]
            {
                """
                [DIConstructorAttribute]
                public class UserManager { }
                """
            },
            ExpectedDiagnostics = new[]
            {
                new DiagnosticResult("KW001", "The class `UserManager` must be defined as `partial`",
                    DiagnosticSeverity.Error)
            }
        }.VerifyAsync();
    }

    [Fact(DisplayName = "Using a class without any fields does report a diagnostic.")]
    public async Task UsingAClassWithoutFieldsReportsADiagnostic()
    {
        // Arrange, act & assert.
        await new DiagnosticAnalyzerVerifier<DIConstructorAnalyzer>
        {
            InputSources = new[]
            {
                """
                [DIConstructorAttribute]
                public partial class UserManager { }
                """
            },
            ExpectedDiagnostics = new[]
            {
                new DiagnosticResult("KW002",
                    "The class `UserManager` must at least have one `readonly` field (NOT `static`, NOT `initialized` fields)",
                    DiagnosticSeverity.Warning)
            }
        }.VerifyAsync();
    }

    [Fact(DisplayName = "Using a class with fields does NOT report a diagnostic.")]
    public async Task UsingAClassWithFieldsReportsNoDiagnostic()
    {
        // Arrange, act & assert.
        await new DiagnosticAnalyzerVerifier<DIConstructorAnalyzer>
        {
            InputSources = new[]
            {
                """
                public interface IUserStore { }
                """,
                """
                [DIConstructorAttribute]
                public partial class UserManager
                {
                    private readonly IUserStore _userStore;
                }
                """
            }
        }.VerifyAsync();
    }

    [Fact(DisplayName = "Using a class that calls a `default` base constructor does report a diagnostic.")]
    public async Task UsingAClassThatCallsADefaultBaseClassReportsNoDiagnostic()
    {
        // Arrange, act & assert.
        await new DiagnosticAnalyzerVerifier<DIConstructorAnalyzer>
        {
            InputSources = new[]
            {
                """
                public interface IUserStore { }
                """,
                """
                public abstract class ManagerBase
                {
                    protected ManagerBase() { }
                }
                """,
                """
                [DIConstructorAttribute]
                public partial class UserManager : ManagerBase { }
                """
            },
            ExpectedDiagnostics = new[]
            {
                new DiagnosticResult("KW002",
                    "The class `UserManager` must at least have one `readonly` field (NOT `static`, NOT `initialized` fields)",
                    DiagnosticSeverity.Warning)
            }
        }.VerifyAsync();
    }

    [Fact(DisplayName = "Using a class that calls a base constructor does NOT report a diagnostic.")]
    public async Task UsingAClassThatCallsABaseConstructorReportsNoDiagnostic()
    {
        // Arrange, act & assert.
        await new DiagnosticAnalyzerVerifier<DIConstructorAnalyzer>
        {
            InputSources = new[]
            {
                """
                public interface IUserStore { }
                """,
                """
                public abstract class ManagerBase
                {
                    protected ManagerBase(IUserStore userStore) { }
                }
                """,
                """
                [DIConstructorAttribute]
                public partial class UserManager : ManagerBase { }
                """
            }
        }.VerifyAsync();
    }
}

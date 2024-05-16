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
namespace Kwality.Roslynify.Tests.Analyzers;

using System.Diagnostics.CodeAnalysis;

using Kwality.Roslynify.Analyzers;
using Kwality.Roslynify.Tests.Facts;
using Kwality.Roslynify.Tests.Helpers.Models;
using Kwality.Roslynify.Tests.Helpers.Verifiers;

using Microsoft.CodeAnalysis;

using Xunit;

[Trait("Analyzer", "Dependency Injection Constructor")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "MemberCanBeFileLocal")]
public sealed class DIConstructorAnalyzerTests
{
    private const string kw001Description
        = "KW001 (The `DIConstructor` attribute can only be applied to `partial` classes).";

    private const string kw002Description
        = "KW002 (The `DIConstructor` attribute can only be applied to members if all the parent members are defined as `partial`).";

    private const string kw003Description
        = "KW003 (The `DIConstructor` attribute only make sense on types which contains `readonly` (NOT `static`, NOT `initialized`) fields).";

    private readonly DiagnosticResult KW001Diagnostic = new("KW001",
        "The class `UserManager` must be defined as `partial`", DiagnosticSeverity.Error);

    private readonly DiagnosticResult KW002Diagnostic = new("KW002",
        "All of `UserManager's` parent members must be defined as `partial`", DiagnosticSeverity.Error);

    private readonly DiagnosticResult KW003Diagnostic = new("KW003",
        "The class `UserManager` must at least have one `readonly` field (NOT `static`, NOT `initialized`) fields",
        DiagnosticSeverity.Warning);

    private readonly DiagnosticAnalyzerVerifier<DIConstructorAnalyzer> verifier = new();

    [Trait("Diagnostic", kw001Description)]
    [DiagnosticFact("Decorate a NON \"partial\" class with \"DIConstructor\".")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal Task Decorate_non_partial_class_with_short_attribute()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            [DIConstructor]
            public class UserManager { }
            """,
        ];

        this.verifier.ExpectedDiagnostics = [this.KW001Diagnostic];

        // Act & assert.
        return this.verifier.VerifyAsync();
    }

    [Trait("Diagnostic", kw001Description)]
    [DiagnosticFact("Decorate a NON \"partial\" class with \"DIConstructorAttribute\".")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal Task Decorate_non_partial_class_with_long_attribute()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            [DIConstructorAttribute]
            public class UserManager { }
            """,
        ];

        this.verifier.ExpectedDiagnostics = [this.KW001Diagnostic];

        // Act & assert.
        return this.verifier.VerifyAsync();
    }

    [Trait("Diagnostic", kw002Description)]
    [DiagnosticFact("Decorate a \"partial\" class inside a NON \"partial\" class.")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal Task Decorate_partial_class_inside_a_non_partial_class()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            public class Parent
            {
                [DIConstructorAttribute]
                public partial class UserManager { }
            }
            """,
        ];

        this.verifier.ExpectedDiagnostics = [this.KW002Diagnostic];

        // Act & assert.
        return this.verifier.VerifyAsync();
    }

    [Trait("Diagnostic", kw002Description)]
    [DiagnosticFact("Decorate a \"partial\" class inside a NON \"partial\" record.")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal Task Decorate_partial_class_inside_a_non_partial_record()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            public record Parent
            {
                [DIConstructorAttribute]
                public partial class UserManager { }
            }
            """,
        ];

        this.verifier.ExpectedDiagnostics = [this.KW002Diagnostic];

        // Act & assert.
        return this.verifier.VerifyAsync();
    }

    [Trait("Diagnostic", kw002Description)]
    [DiagnosticFact("Decorate a \"partial\" class inside a NON \"partial\" interface.")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal Task Decorate_partial_class_inside_a_non_partial_interface()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            public interface Parent
            {
                [DIConstructorAttribute]
                public partial class UserManager { }
            }
            """,
        ];

        this.verifier.ExpectedDiagnostics = [this.KW002Diagnostic];

        // Act & assert.
        return this.verifier.VerifyAsync();
    }

    [Trait("Diagnostic", kw002Description)]
    [DiagnosticFact("Decorate a \"partial\" class inside a NON \"partial\" struct.")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal Task Decorate_partial_class_inside_a_non_partial_struct()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            public struct Parent
            {
                [DIConstructorAttribute]
                public partial class UserManager { }
            }
            """,
        ];

        this.verifier.ExpectedDiagnostics = [this.KW002Diagnostic];

        // Act & assert.
        return this.verifier.VerifyAsync();
    }

    [Trait("Diagnostic", kw002Description)]
    [DiagnosticFact("Decorate a \"partial\" class inside a NON \"partial\" top level parent.")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal Task Decorate_partial_class_inside_a_non_partial_top_level_parent()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            public class Top
            {
                public partial class Sub
                {
                    [DIConstructorAttribute]
                    public partial class UserManager { }
                }
            }
            """,
        ];

        this.verifier.ExpectedDiagnostics = [this.KW002Diagnostic];

        // Act & assert.
        return this.verifier.VerifyAsync();
    }

    [Trait("Diagnostic", kw003Description)]
    [DiagnosticFact("Decorate a class without fields.")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal Task Decorate_class_without_fields()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            [DIConstructorAttribute]
            public partial class UserManager { }
            """,
        ];

        this.verifier.ExpectedDiagnostics = [this.KW003Diagnostic];

        // Act & assert.
        return this.verifier.VerifyAsync();
    }

    [Trait("Diagnostic", kw003Description)]
    [DiagnosticFact("Diagnostic: Decorate a class with only `static` fields.")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal Task Decorate_class_with_only_static_fields()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            [DIConstructorAttribute]
            public partial class UserManager
            {
                private static readonly IUserManager;
            }
            """,
        ];

        this.verifier.ExpectedDiagnostics = [this.KW003Diagnostic];

        // Act & assert.
        return this.verifier.VerifyAsync();
    }

    [Trait("Diagnostic", kw003Description)]
    [DiagnosticFact("Decorate a class with only `initialized` fields.")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal Task Decorate_class_with_only_initialized_fields()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            [DIConstructorAttribute]
            public partial class UserManager
            {
                private static readonly int _maxAge = 10;
            }
            """,
        ];

        this.verifier.ExpectedDiagnostics = [this.KW003Diagnostic];

        // Act & assert.
        return this.verifier.VerifyAsync();
    }

    [NoDiagnosticFact("Decorate a class with fields.")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal Task Decorate_class_with_fields()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            [DIConstructorAttribute]
            public partial class UserManager
            {
                private readonly IUserStore _userStore;
            }
            """,
        ];

        // Act & assert.
        return this.verifier.VerifyAsync();
    }
}

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
namespace Kwality.Roslynify.Tests.Generators;

using System.Diagnostics.CodeAnalysis;

using Kwality.Roslynify.Generators;
using Kwality.Roslynify.Tests.Facts;
using Kwality.Roslynify.Tests.Helpers.Verifiers;

using Xunit;

[Trait("Source Generator (Incremental)", "Dependency Injection Constructor")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "MemberCanBeFileLocal")]
public sealed class DIConstructorGeneratorTests
{
    private const string markerAttribute = """
                                           [global::System.AttributeUsage(global::System.AttributeTargets.Class)]
                                           public sealed class DIConstructorAttribute : System.Attribute
                                           {
                                               // NOTE: Intentionally left blank.
                                               //       Used as a "marker" attribute.
                                           }
                                           """;

    private readonly SourceGeneratorVerifier<DIConstructorGenerator> verifier = new();

    [SourceGeneratorFact("Marker attribute", "No C# source file(s).")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal void No_input()
    {
        // Arrange.
        this.verifier.ExpectedGeneratedSources = [markerAttribute];

        // Act & assert.
        this.verifier.Verify();
    }

    [SourceGeneratorFact("Marker attribute", "Decorate a NON \"partial\" class with \"DIConstructor\".")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal void Decorate_non_partial_class_with_short_attribute()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            [DIConstructor]
            public class UserManager { }
            """,
        ];

        this.verifier.ExpectedGeneratedSources = [markerAttribute];

        // Act & assert.
        this.verifier.Verify();
    }

    [SourceGeneratorFact("Empty class", "Decorate a \"partial\" class with \"DIConstructor\".")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal void Decorate_partial_class_with_short_attribute()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            [DIConstructor]
            public partial class UserManager { }
            """,
        ];

        this.verifier.ExpectedGeneratedSources =
        [
            markerAttribute,
            """
            partial class UserManager
            {
            }

            """,
        ];

        // Act & assert.
        this.verifier.Verify();
    }

    [SourceGeneratorFact("Marker attribute", "Decorate a NON \"partial\" class with \"DIConstructorAttribute\".")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal void Decorate_non_partial_class_with_long_attribute()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            [DIConstructorAttribute]
            public class UserManager { }
            """,
        ];

        this.verifier.ExpectedGeneratedSources = [markerAttribute];

        // Act & assert.
        this.verifier.Verify();
    }

    [SourceGeneratorFact("Empty class", "Decorate a \"partial\" class with \"DIConstructorAttribute\".")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal void Decorate_partial_class_with_long_attribute()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            [DIConstructorAttribute]
            public partial class UserManager { }
            """,
        ];

        this.verifier.ExpectedGeneratedSources =
        [
            markerAttribute,
            """
            partial class UserManager
            {
            }

            """,
        ];

        // Act & assert.
        this.verifier.Verify();
    }

    [SourceGeneratorFact("Marker attribute", "Decorate a \"partial\" class inside a NON \"partial\" class.")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal void Decorate_partial_class_inside_a_non_partial_class()
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

        this.verifier.ExpectedGeneratedSources = [markerAttribute];

        // Act & assert.
        this.verifier.Verify();
    }

    [SourceGeneratorFact("Marker attribute", "Decorate a \"partial\" class inside a NON \"partial\" record.")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal void Decorate_partial_class_inside_a_non_partial_record()
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

        this.verifier.ExpectedGeneratedSources = [markerAttribute];

        // Act & assert.
        this.verifier.Verify();
    }

    [SourceGeneratorFact("Decorate a \"partial\" class inside a NON \"partial\" interface.")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal void Decorate_partial_class_inside_a_non_partial_interface()
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

        this.verifier.ExpectedGeneratedSources = [markerAttribute];

        // Act & assert.
        this.verifier.Verify();
    }

    [SourceGeneratorFact("Marker attribute, Constructor", "Decorate a class.")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal void Decorate_class()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            [DIConstructorAttribute]
            public partial class UserManager : Base.ManagerBase
            {
                public interface IUserStore { }
            
                private readonly IUserStore _userStore;
                private readonly Lib.IDateTimeProvider dateTimeProvider;
                private readonly int maxAge = 10;
                private readonly static bool logStatistics = true;
            }
            """,
            """
            namespace Base;

            public abstract class ManagerBase(ManagerBase.IConnectionProvider connectionProvider)
            {
                public interface IConnectionProvider;
            }
            """,
            """
            namespace Lib;

            public interface IDateTimeProvider { }
            """,
        ];

        this.verifier.ExpectedGeneratedSources =
        [
            markerAttribute,
            """
            partial class UserManager
            {
                public UserManager(global::UserManager.IUserStore @userStore, global::Lib.IDateTimeProvider @dateTimeProvider, global::Base.ManagerBase.IConnectionProvider @connectionProvider)
                    : base(@connectionProvider)
                {
                    this._userStore = @userStore;
                    this.dateTimeProvider = @dateTimeProvider;
                }
            }

            """,
        ];

        // Act & assert.
        this.verifier.Verify();
    }

    [SourceGeneratorFact("Marker attribute, Empty class", "Decorate a class inside a namespace.")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal void Decorate_class_in_namespace()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            namespace Lib;

            [DIConstructorAttribute]
            public partial class UserManager { }
            """,
        ];

        this.verifier.ExpectedGeneratedSources =
        [
            markerAttribute,
            """
            namespace Lib;

            partial class UserManager
            {
            }

            """,
        ];

        // Act & assert.
        this.verifier.Verify();
    }

    [SourceGeneratorFact("Marker attribute, Empty class", "Decorate a nested class.")]
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    internal void Decorate_nested_class()
    {
        // Arrange.
        this.verifier.InputSources =
        [
            """
            partial class Top
            {
                partial class Sub
                {
                    [DIConstructorAttribute]
                    partial class UserManager { }
                }
            }
            """,
        ];

        this.verifier.ExpectedGeneratedSources =
        [
            markerAttribute,
            """
            partial class Top
            {
                partial class Sub
                {
                    partial class UserManager
                    {
                    }
                }
            }

            """,
        ];

        // Act & assert.
        this.verifier.Verify();
    }
}

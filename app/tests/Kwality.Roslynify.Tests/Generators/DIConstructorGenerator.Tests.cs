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
namespace Kwality.Roslynify.Tests.Generators;

using System.Diagnostics.CodeAnalysis;

using Kwality.Roslynify.Generators;
using Kwality.Roslynify.Tests.Helpers;

using Xunit;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class DIConstructorGeneratorTests
{
    private const string markerAttribute = """
                                           [System.AttributeUsage(System.AttributeTargets.Class)]
                                           public sealed class DIConstructorAttribute : System.Attribute
                                           {
                                               // NOTE: Intentionally left blank.
                                               //       Used as a "marker" attribute.
                                           }
                                           """;

    [Fact(DisplayName = "The `marker` attribute is added.")]
    public void The_marker_attribute_is_added()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSources = new [] { string.Empty }, GeneratedSources = new[] { markerAttribute }
        }.Verify();
    }

    [Fact(DisplayName = "A NON `partial` class (with the `marker` attribute) does NOT generate any code.")]
    public void A_non_partial_class_with_the_marker_attribute_does_not_generate_code()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSources = new[]
            {
                """
                namespace Lib;

                [DIConstructor]
                public class UserManager
                {
                    public interface IUserStore { }
                    
                    private readonly IUserStore _userStore;
                }
                """
            },
            GeneratedSources = new[] { markerAttribute }
        }.Verify();
    }

    [Fact(DisplayName = "A `partial` class (without the `marker` attribute) does NOT generate any code.")]
    public void A_partial_class_without_the_marker_attribute_does_not_generate_code()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSources = new[]
            {
                """
                namespace Lib;

                public partial class UserManager
                {
                    public interface IUserStore { }
                    
                    private readonly IUserStore _userStore;
                }
                """
            },
            GeneratedSources = new[] { markerAttribute }
        }.Verify();
    }

    [Fact(DisplayName = "A class inside a NON `partial` class does NOT generate any code.")]
    public void A_class_inside_a_non_partial_class_does_not_generate_code()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSources = new[]
            {
                """
                namespace Lib;

                public class Parent
                {
                    [DIConstructor]
                    public partial class UserManager
                    {
                        public interface IUserStore { }
                        
                        private readonly IUserStore _userStore;
                    }
                }
                """
            },
            GeneratedSources = new[] { markerAttribute }
        }.Verify();
    }

    [Fact(DisplayName = "A class without any fields does generate an empty class.")]
    public void A_class_without_any_fields_does_generate_an_empty_class()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSources = new[]
            {
                """
                namespace Lib;

                [DIConstructor]
                public partial class UserManager
                {
                }
                """
            },
            GeneratedSources = new[]
            {
                """
                namespace Lib;

                partial class UserManager
                {
                }

                """,
                markerAttribute
            }
        }.Verify();
    }

    [Fact(DisplayName = "A class is with the `short` attribute name does generate a class.")]
    public void A_class_with_the_short_attribute_name_does_generate_a_class()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSources = new[]
            {
                """
                namespace Lib;

                [DIConstructor]
                public partial class UserManager
                {
                }
                """
            },
            GeneratedSources = new[]
            {
                """
                namespace Lib;

                partial class UserManager
                {
                }

                """,
                markerAttribute
            }
        }.Verify();
    }

    [Fact(DisplayName = "A class is with the `long` attribute name does generate a class.")]
    public void A_class_with_the_long_attribute_name_does_generate_a_class()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSources = new[]
            {
                """
                namespace Lib;

                [DIConstructorAttribute]
                public partial class UserManager
                {
                }
                """
            },
            GeneratedSources = new[]
            {
                """
                namespace Lib;

                partial class UserManager
                {
                }

                """,
                markerAttribute
            }
        }.Verify();
    }

    [Fact(DisplayName = "A class in the `global` namespace does generate a class.")]
    public void A_class_in_the_global_namespace_does_generate_a_class()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSources = new[]
            {
                """
                [DIConstructorAttribute]
                public partial class UserManager
                {
                }
                """
            },
            GeneratedSources = new[]
            {
                """
                partial class UserManager
                {
                }

                """,
                markerAttribute
            }
        }.Verify();
    }

    [Fact(DisplayName = "A class inside a class does generate a class.")]
    public void A_class_inside_a_class_does_generate_a_class()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSources = new[]
            {
                """
                namespace Lib;
                 
                public partial class Parent
                {
                    [DIConstructor]
                    public partial class UserManager
                    {
                    }
                }
                """
            },
            GeneratedSources = new[]
            {
                """
                namespace Lib;

                partial class Parent
                {
                    partial class UserManager
                    {
                    }
                }

                """,
                markerAttribute
            }
        }.Verify();
    }

    [Fact(DisplayName = "A class inside an interface does generate a class.")]
    public void A_class_inside_an_interface_does_generate_a_class()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSources = new[]
            {
                """
                namespace Lib;
                 
                public partial interface Parent
                {
                    [DIConstructor]
                    public partial class UserManager
                    {
                    }
                }
                """
            },
            GeneratedSources = new[]
            {
                """
                namespace Lib;

                partial interface Parent
                {
                    partial class UserManager
                    {
                    }
                }

                """,
                markerAttribute
            }
        }.Verify();
    }

    [Fact(DisplayName = "A class inside a struct does generate a class.")]
    public void A_class_inside_a_struct_does_generate_a_class()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSources = new[]
            {
                """
                namespace Lib;
                 
                public partial struct Parent
                {
                    [DIConstructor]
                    public partial class UserManager
                    {
                    }
                }
                """
            },
            GeneratedSources = new[]
            {
                """
                namespace Lib;

                partial struct Parent
                {
                    partial class UserManager
                    {
                    }
                }

                """,
                markerAttribute
            }
        }.Verify();
    }

    [Fact(DisplayName = "Fields (`readonly`) are injected in the constructor.")]
    public void Readonly_fields_are_injected_in_the_constructor()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSources = new[]
            {
                """
                namespace Lib;
                 
                [DIConstructor]
                public partial class UserManager
                {
                    public interface IUserStore { }
                
                    private readonly IUserStore _userStore;
                }
                """
            },
            GeneratedSources = new[]
            {
                """
                namespace Lib;

                partial class UserManager
                {
                    public UserManager(global::Lib.UserManager.IUserStore @userStore)
                    {
                        this._userStore = @userStore;
                    }
                }

                """,
                markerAttribute
            }
        }.Verify();
    }

    [Fact(DisplayName = "Fields (`static`, `readonly`) are NOT injected in the constructor.")]
    public void Static_readonly_fields_are_not_injected_in_the_constructor()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSources = new[]
            {
                """
                namespace Lib;
                 
                [DIConstructor]
                public partial class UserManager
                {
                    public interface IUserStore { }
                
                    private readonly IUserStore _userStore;
                    private static readonly int S = 10;
                }
                """
            },
            GeneratedSources = new[]
            {
                """
                namespace Lib;

                partial class UserManager
                {
                    public UserManager(global::Lib.UserManager.IUserStore @userStore)
                    {
                        this._userStore = @userStore;
                    }
                }

                """,
                markerAttribute
            }
        }.Verify();
    }
    
    [Fact(DisplayName = "Fields (`readonly`, initialized) are NOT injected in the constructor.")]
    public void Initialized_readonly_fields_are_not_injected_in_the_constructor()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSources = new[]
            {
                """
                namespace Lib;
                 
                [DIConstructor]
                public partial class UserManager
                {
                    public interface IUserStore { }
                
                    private readonly IUserStore _userStore;
                    private readonly int S = 10;
                }
                """
            },
            GeneratedSources = new[]
            {
                """
                namespace Lib;

                partial class UserManager
                {
                    public UserManager(global::Lib.UserManager.IUserStore @userStore)
                    {
                        this._userStore = @userStore;
                    }
                }

                """,
                markerAttribute
            }
        }.Verify();
    }
    
    [Fact(DisplayName = "Fields defined in all `partial` types are injected in the constructor.")]
    public void Fields_defined_all_partial_types_are_injected()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSources = new[]
            {
                """
                namespace Lib;
                 
                [DIConstructor]
                public partial class UserManager
                {
                    public interface IUserStore { }
                
                    private readonly IUserStore _userStore;
                }
                """,
                """
                namespace Lib;
                 
                [DIConstructor]
                public partial class UserManager
                {
                    public interface ITimeProvider { }
                
                    private readonly ITimeProvider _timeProvider;
                }
                """,
            },
            GeneratedSources = new[]
            {
                """
                namespace Lib;

                partial class UserManager
                {
                    public UserManager(global::Lib.UserManager.IUserStore @userStore, global::Lib.UserManager.ITimeProvider @timeProvider)
                    {
                        this._userStore = @userStore;
                        this._timeProvider = @timeProvider;
                    }
                }

                """,
                markerAttribute
            }
        }.Verify();
    }
}

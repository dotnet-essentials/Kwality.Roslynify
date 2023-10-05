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
using Kwality.Roslynify.Tests.Helpers.Verifiers;

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

    [Fact(DisplayName = "The `marker` attribute is added when using `NO` input.")]
    public void NoInputOnlyAddsTheMarkerAttribute()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSources = new[] { string.Empty }, GeneratedSources = new[] { markerAttribute }
        }.Verify();
    }

    [Fact(DisplayName = "No class is generated when using a NON `partial` class with the `marker` attribute.")]
    public void ANonPartialClassWithTheMarkerAttributeGeneratesNoClass()
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

    [Fact(DisplayName = "No class is generated when using a `partial` class without the `marker` attribute.")]
    public void APartialClassWithoutTheMarkerAttributeGeneratesNoClass()
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

    [Fact(DisplayName =
        "No class is generated when using a `partial` class with the `marker` attribute inside a NON `partial` class.")]
    public void APartialClassWithTheMarkerAttributeInsideANonPartialClassGeneratesNoClass()
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

    [Fact(DisplayName = "Using the `short` version of the `marker` attribute generates a class.")]
    public void UsingTheShortVersionOfTheMarkerAttributeGeneratesAClass()
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

    [Fact(DisplayName = "Using the `long` version of the `marker` attribute generates a class.")]
    public void UsingTheLongVersionOfTheMarkerAttributeGeneratesAClass()
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

    [Fact(DisplayName = "Using a class in the `global` namespace generates a class.")]
    public void UsingAClassInTheGlobalNamespaceGeneratesAClass()
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

    [Fact(DisplayName = "Using a class inside a class generates a class.")]
    public void UsingAClassInsideAClassGeneratesAClass()
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

    [Fact(DisplayName = "Using a class inside an interface generates a class.")]
    public void UsingAClassInsideAnInterfaceGeneratesAClass()
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

    [Fact(DisplayName = "Using a class inside a struct generates a class.")]
    public void UsingAClassInsideAStructGeneratesAClass()
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
    public void ReadonlyFieldsAreInjectedInTheConstructor()
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

    [Fact(DisplayName = "Fields (`readonly`, `static`) are NOT injected in the constructor.")]
    public void ReadonlyStaticFieldsAreNotInjectedInTheConstructor()
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
    public void ReadonlyInitializedFieldsAreNotInjectedInTheConstructor()
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
    public void FieldsDefinedInAllPartialTypesAreInjectedInTheConstructor()
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
                 
                public partial class UserManager
                {
                    public interface ITimeProvider { }
                
                    private readonly ITimeProvider _timeProvider;
                }
                """
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

    [Fact(DisplayName = "The base constructor is called in the generated constructor.")]
    public void TheBaseConstructorIsCalledInTheGeneratedConstructor()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSources = new[]
            {
                """
                namespace Lib;

                public interface IUserStore { }
                public interface ITimeProvider { }
                """,
                """
                namespace Lib;
                 
                [DIConstructor]
                public partial class UserManager : ManagerBase
                {
                    private readonly IUserStore _userStore;
                }
                """,
                """
                namespace Lib;
                 
                public abstract class ManagerBase
                {
                    private readonly ITimeProvider _timeProvider;
                    
                    protected ManagerBase(ITimeProvider timeProvider)
                    {
                        this._timeProvider = timeProvider;
                    }
                }
                """
            },
            GeneratedSources = new[]
            {
                """
                namespace Lib;

                partial class UserManager
                {
                    public UserManager(global::Lib.IUserStore @userStore, global::Lib.ITimeProvider @timeProvider)
                        : base(@timeProvider)
                    {
                        this._userStore = @userStore;
                    }
                }

                """,
                markerAttribute
            }
        }.Verify();
    }
}

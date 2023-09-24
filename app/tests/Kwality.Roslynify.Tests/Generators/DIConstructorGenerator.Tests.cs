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

    [Fact(DisplayName = "The \"marker\" attribute is added.")]
    public void The_marker_attribute_is_added()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSource = string.Empty, GeneratedSources = new[] { markerAttribute }
        }.Verify();
    }

    [Fact(DisplayName = "No code is added for types that aren't marked as \"partial\".")]
    public void No_code_is_added_for_types_that_are_not_marked_as_partial()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSource = """
                          namespace Lib;

                          [DIConstructor]
                          public class UserManager
                          {
                              public interface IUserStore { }
                              
                              private readonly IUserStore _userStore;
                          }
                          """,
            GeneratedSources = new[] { markerAttribute }
        }.Verify();
    }

    [Fact(DisplayName = "No code is added for `partial` types that aren't marked with the `marker` attribute.")]
    public void No_code_is_added_for_partial_types_that_are_not_marked_with_the_marker_attribute()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSource = """
                          namespace Lib;

                          public partial class UserManager
                          {
                              public interface IUserStore { }
                              
                              private readonly IUserStore _userStore;
                          }
                          """,
            GeneratedSources = new[] { markerAttribute }
        }.Verify();
    }

    [Fact(DisplayName = "No code is added for `partial` types that are nested in NON `partial` types.")]
    public void No_code_is_added_for_partial_types_that_are_nested_in_non_partial_types()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSource = """
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
                          """,
            GeneratedSources = new[] { markerAttribute }
        }.Verify();
    }

    [Fact(DisplayName = "A `partial` class is added for a class that doesn't contain fields.")]
    public void A_partial_class_is_added_for_a_class_that_does_not_contain_fields()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSource = """
                          namespace Lib;

                          [DIConstructor]
                          public partial class UserManager
                          {
                          }
                          """,
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

    [Fact(DisplayName = "A constructor is added when using the \"short\" attribute name.")]
    public void A_constructor_is_added_when_using_the_short_attribute_name()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSource = """
                          namespace Lib;

                          [DIConstructor]
                          public partial class UserManager
                          {
                              public interface IUserStore { }
                              public interface IFileSystem { }
                              
                              private readonly IUserStore _userStore;
                              private readonly IFileSystem _fileSystem;
                          }
                          """,
            GeneratedSources = new[]
            {
                """
                namespace Lib;

                partial class UserManager
                {
                    public UserManager(global::Lib.UserManager.IUserStore @userStore, global::Lib.UserManager.IFileSystem @fileSystem)
                    {
                        this._userStore = @userStore;
                        this._fileSystem = @fileSystem;
                    }
                }

                """,
                markerAttribute
            }
        }.Verify();
    }

    [Fact(DisplayName = "A constructor is added when using the \"long\" attribute name.")]
    public void A_constructor_is_added_when_using_the_long_attribute_name()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSource = """
                          namespace Lib;

                          [DIConstructorAttribute]
                          public partial class UserManager
                          {
                              public interface IUserStore { }
                              public interface IFileSystem { }
                              
                              private readonly IUserStore _userStore;
                              private readonly IFileSystem _fileSystem;
                          }
                          """,
            GeneratedSources = new[]
            {
                """
                namespace Lib;

                partial class UserManager
                {
                    public UserManager(global::Lib.UserManager.IUserStore @userStore, global::Lib.UserManager.IFileSystem @fileSystem)
                    {
                        this._userStore = @userStore;
                        this._fileSystem = @fileSystem;
                    }
                }

                """,
                markerAttribute
            }
        }.Verify();
    }

    [Fact(DisplayName = "A constructor is added when the type is NOT contained inside a namespace.")]
    public void A_constructor_is_added_when_the_type_is_not_contained_inside_a_namespace()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSource = """
                          [DIConstructorAttribute]
                          public partial class UserManager
                          {
                              public interface IUserStore { }
                              public interface IFileSystem { }
                              
                              private readonly IUserStore _userStore;
                              private readonly IFileSystem _fileSystem;
                          }
                          """,
            GeneratedSources = new[]
            {
                """
                partial class UserManager
                {
                    public UserManager(global::UserManager.IUserStore @userStore, global::UserManager.IFileSystem @fileSystem)
                    {
                        this._userStore = @userStore;
                        this._fileSystem = @fileSystem;
                    }
                }

                """,
                markerAttribute
            }
        }.Verify();
    }

    [Fact(DisplayName = "A constructor is added for a class \"nested\" in another class.")]
    public void A_constructor_is_added_for_a_class_nested_in_another_class()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSource = """
                          namespace Lib;
                           
                          public partial class Parent
                          {
                              [DIConstructor]
                              public partial class UserManager
                              {
                                  public interface IUserStore { }
                                  public interface IFileSystem { }
                              
                                  private readonly IUserStore _userStore;
                                  private readonly IFileSystem _fileSystem;
                              }
                          }
                          """,
            GeneratedSources = new[]
            {
                """
                namespace Lib;

                partial class Parent
                {
                    partial class UserManager
                    {
                        public UserManager(global::Lib.Parent.UserManager.IUserStore @userStore, global::Lib.Parent.UserManager.IFileSystem @fileSystem)
                        {
                            this._userStore = @userStore;
                            this._fileSystem = @fileSystem;
                        }
                    }
                }

                """,
                markerAttribute
            }
        }.Verify();
    }

    [Fact(DisplayName = "A constructor is added for a class \"nested\" in an interfaces.")]
    public void A_constructor_is_added_for_a_class_nested_in_an_interface()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSource = """
                          namespace Lib;
                           
                          public partial interface Parent
                          {
                              [DIConstructor]
                              public partial class UserManager
                              {
                                  public interface IUserStore { }
                                  public interface IFileSystem { }
                              
                                  private readonly IUserStore _userStore;
                                  private readonly IFileSystem _fileSystem;
                              }
                          }
                          """,
            GeneratedSources = new[]
            {
                """
                namespace Lib;

                partial interface Parent
                {
                    partial class UserManager
                    {
                        public UserManager(global::Lib.Parent.UserManager.IUserStore @userStore, global::Lib.Parent.UserManager.IFileSystem @fileSystem)
                        {
                            this._userStore = @userStore;
                            this._fileSystem = @fileSystem;
                        }
                    }
                }

                """,
                markerAttribute
            }
        }.Verify();
    }

    [Fact(DisplayName = "A constructor is added for a class \"nested\" in a struct.")]
    public void A_constructor_is_added_for_a_class_nested_in_a_struct()
    {
        // Arrange, act & assert.
        new SourceGeneratorVerifier<DIConstructorGenerator>
        {
            InputSource = """
                          namespace Lib;
                           
                          public partial struct Parent
                          {
                              [DIConstructor]
                              public partial class UserManager
                              {
                                  public interface IUserStore { }
                                  public interface IFileSystem { }
                              
                                  private readonly IUserStore _userStore;
                                  private readonly IFileSystem _fileSystem;
                              }
                          }
                          """,
            GeneratedSources = new[]
            {
                """
                namespace Lib;

                partial struct Parent
                {
                    partial class UserManager
                    {
                        public UserManager(global::Lib.Parent.UserManager.IUserStore @userStore, global::Lib.Parent.UserManager.IFileSystem @fileSystem)
                        {
                            this._userStore = @userStore;
                            this._fileSystem = @fileSystem;
                        }
                    }
                }

                """,
                markerAttribute
            }
        }.Verify();
    }
}

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
namespace Kwality.Roslynify.Common.Extensions.Symbols;

using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class ITypeSymbolExtensions
{
    public static string? GetNamespace(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.ContainingNamespace.IsGlobalNamespace ? null : typeSymbol.ContainingNamespace.ToString();
    }

    public static string AsPartialTypeString(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.TypeKind switch
        {
            TypeKind.Class => $"partial class {typeSymbol.Name}",
            TypeKind.Interface => $"partial struct {typeSymbol.Name}",
            TypeKind.Struct => $"partial struct {typeSymbol.Name}",
            TypeKind.Unknown or TypeKind.Array or TypeKind.Delegate or TypeKind.Dynamic or TypeKind.Enum
                or TypeKind.Error or TypeKind.Module or TypeKind.Pointer or TypeKind.TypeParameter
                or TypeKind.Submission
                or TypeKind.FunctionPointer => throw new NotSupportedException(
                    $"Can't format `{typeSymbol.TypeKind}` as a string."),
            _ => throw new NotSupportedException($"Can't format `{typeSymbol.TypeKind}` as a string."),
        };
    }

    public static IEnumerable<IFieldSymbol> GetFields(this ITypeSymbol typeSymbol, Func<IFieldSymbol, bool> filter)
    {
        return typeSymbol.GetMembers()
                         .OfType<IFieldSymbol>()
                         .Where(filter);
    }
}

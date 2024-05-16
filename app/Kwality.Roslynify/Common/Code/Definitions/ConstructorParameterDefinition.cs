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
namespace Kwality.Roslynify.Common.Code.Definitions;

using Kwality.Roslynify.Common.Helpers.Normalizers;

using Microsoft.CodeAnalysis;

internal sealed class ConstructorParameterDefinition
{
    public ConstructorParameterDefinition(IFieldSymbol symbol)
    {
        this.FullyQualifiedTypeName = symbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        this.Name = FieldNameNormalizer.Normalize(symbol.Name);
    }

    public ConstructorParameterDefinition(IParameterSymbol symbol)
    {
        this.FullyQualifiedTypeName = symbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        this.Name = FieldNameNormalizer.Normalize(symbol.Name);
    }

    private string FullyQualifiedTypeName { get; }
    public string Name { get; }

    public override string ToString()
    {
        return $"{this.FullyQualifiedTypeName} {this.Name}";
    }
}

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
namespace Kwality.Roslynify.Models;

using Kwality.Roslynify.Common.Code.Definitions;
using Kwality.Roslynify.Common.Code.Definitions.Enumerations;
using Kwality.Roslynify.Common.Extensions.Symbols;
using Kwality.Roslynify.Common.Helpers.Builders;

using Microsoft.CodeAnalysis;

internal sealed class TypeSymbolStack : Stack<ITypeSymbol>
{
    private readonly IndentedStringBuilder stringBuilder = new();

    public TypeSymbolStack(ITypeSymbol typeSymbol)
    {
        this.Push(typeSymbol);

        while (this.GetContainingType() != null)
            this.Push(this.GetContainingType() ?? throw new NotSupportedException("Parent NOT found."));
    }

    private ITypeSymbol? GetContainingType()
    {
        return this.First()
                   .ContainingType;
    }

    public override string ToString()
    {
        while (this.Count > 0)
        {
            ITypeSymbol? typeSymbol = this.Pop();
            this.stringBuilder.AppendLine(typeSymbol.AsPartialTypeString());
            this.stringBuilder.AppendLine("{");

            if (typeSymbol.TypeKind == TypeKind.Class)
            {
                this.WriteClass(typeSymbol);
            }

            this.stringBuilder.Indent();
        }

        while (this.stringBuilder.IsIndented())
        {
            this.stringBuilder.Outdent();
            this.stringBuilder.AppendLine("}");
        }

        return this.stringBuilder.ToString();
    }

    private void WriteClass(ITypeSymbol symbol)
    {
        var constructorDefinition = new ConstructorDefinition(symbol, TypeAccessibility.Public);

        if (!constructorDefinition.HasParameters())
        {
            return;
        }

        constructorDefinition.WriteTo(this.stringBuilder);
    }
}

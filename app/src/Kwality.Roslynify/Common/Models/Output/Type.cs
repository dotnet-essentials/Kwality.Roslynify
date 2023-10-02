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
namespace Kwality.Roslynify.Common.Models.Output;

using System.Text;

using Kwality.Roslynify.Common.Extensions.Roslyn.Multiple;
using Kwality.Roslynify.Common.Extensions.Roslyn.Symbol;
using Kwality.Roslynify.Common.Filters.Roslyn.Symbol;
using Kwality.Roslynify.Common.Filters.Roslyn.Symbol.Field;
using Kwality.Roslynify.Common.Filters.Roslyn.Symbol.Method;

using Microsoft.CodeAnalysis;

public sealed class Type
{
    private readonly int level;
    private readonly ITypeSymbol symbol;

    public Type(ITypeSymbol symbol, int level = 0)
    {
        this.symbol = symbol;
        this.level = level;
    }

    public Type? Next { get; set; }

    public override string ToString()
    {
        var prefix = new string(' ', this.level * 4);
        var sBuilder = new StringBuilder();

        switch (this)
        {
            case { symbol.TypeKind: TypeKind.Class }:
                this.WriteClass(sBuilder, prefix);

                break;
            case { symbol.TypeKind: TypeKind.Interface }:
                this.WriteInterface(sBuilder, prefix);

                break;
            case { symbol.TypeKind: TypeKind.Struct }:
                this.WriteStruct(sBuilder, prefix);

                break;
        }

        if (this.Next != null) sBuilder.AppendLine(this.Next.ToString());
        sBuilder.Append($"{prefix}}}");

        return sBuilder.ToString();
    }

    private void WriteClass(StringBuilder sBuilder, string prefix)
    {
        var baseCtorParameters = Enumerable.Empty<Argument>();

        var constructors = this.symbol.BaseType?.Constructors
            .ApplyFilters(new IsNotStaticFilter(), new HasParametersFilter()).ToList();

        if (constructors != null)
        {
            var constructor = constructors.Any() && constructors.Count > 1 ? null : constructors.FirstOrDefault();
       
            if (constructor != null)
            {
                baseCtorParameters = constructor.Parameters.Select(p => new Argument(p));
            }
        }
            
        var constructorArgs = this.symbol.GetFields()
            .ApplyFilters(new IsReadonlyFilter(), new IsNotStaticFilter(), new IsNotInitializedFilter())
            .Select(x => new Argument(x)).ToList();
        
        sBuilder.AppendLine($"{prefix}partial class {this.symbol.Name}");
        sBuilder.AppendLine($"{prefix}{{");

        if (constructorArgs is not { Count: > 0 }) return;

        if (baseCtorParameters.Any())
        {
            var args = constructorArgs.Concat(baseCtorParameters);
            sBuilder.AppendLine($"{prefix}    public {this.symbol.Name}({string.Join(", ", args)})");
            sBuilder.AppendLine($"{prefix}        : base({string.Join(", ", baseCtorParameters.Select(x => x.Name))})");
        }
        else
        {
            sBuilder.AppendLine($"{prefix}    public {this.symbol.Name}({string.Join(", ", constructorArgs)})");
        }
        
        sBuilder.AppendLine($"{prefix}    {{");
        foreach (var arg in constructorArgs) sBuilder.AppendLine($"{prefix}        this.{arg.FieldName} = {arg.Name};");
        sBuilder.AppendLine($"{prefix}    }}");
    }

    private void WriteInterface(StringBuilder sBuilder, string prefix)
    {
        sBuilder.AppendLine($"{prefix}partial interface {this.symbol.Name}");
        sBuilder.AppendLine($"{prefix}{{");
    }

    private void WriteStruct(StringBuilder sBuilder, string prefix)
    {
        sBuilder.AppendLine($"{prefix}partial struct {this.symbol.Name}");
        sBuilder.AppendLine($"{prefix}{{");
    }
}

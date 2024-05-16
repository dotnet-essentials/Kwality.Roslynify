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

using Kwality.Roslynify.Common.Code.Definitions.Enumerations;
using Kwality.Roslynify.Common.Code.Extensions;
using Kwality.Roslynify.Common.Extensions.Symbols;
using Kwality.Roslynify.Common.Helpers.Builders;
using Kwality.Roslynify.Common.Helpers.Normalizers;

using Microsoft.CodeAnalysis;

internal sealed class ConstructorDefinition
{
    private readonly TypeAccessibility accessibility;
    private readonly IEnumerable<ConstructorParameterDefinition> baseParameters;
    private readonly IEnumerable<FieldAssignmentDefinition> fieldAssignments;
    private readonly string name;
    private readonly IEnumerable<ConstructorParameterDefinition> parameters;

    public ConstructorDefinition(ITypeSymbol symbol, TypeAccessibility accessibility)
    {
        this.name = symbol.Name;
        this.accessibility = accessibility;

        List<IMethodSymbol>? parameterizedBaseConstructors = symbol.BaseType?.Constructors
                                                                   .Where(static x => !x.IsStatic && x.Parameters.Any())
                                                                   .OrderBy(static x => x.Parameters.Length)
                                                                   .ToList();

        IMethodSymbol? parameterizedBaseConstructor = parameterizedBaseConstructors?.FirstOrDefault();

        this.baseParameters = parameterizedBaseConstructor
                              ?.Parameters.Select(static p => new ConstructorParameterDefinition(p))
                              .ToList() ?? [];

        IFieldSymbol[] fields = symbol.GetFields(static symbol =>
                                          symbol.IsReadOnly && !symbol.IsStatic && !symbol.IsInitialized())
                                      .ToArray();

        this.parameters = fields.Select(static x => new ConstructorParameterDefinition(x));

        this.fieldAssignments = fields.Select(static x =>
            new FieldAssignmentDefinition(x.Name, FieldNameNormalizer.Normalize(x.Name)));
    }

    public bool HasParameters()
    {
        return this.parameters.Any();
    }

    public void WriteTo(IndentedStringBuilder stringBuilder)
    {
        if (this.baseParameters.Any())
        {
            IEnumerable<ConstructorParameterDefinition>
                combinedParameters = this.parameters.Concat(this.baseParameters);

            stringBuilder.AppendLine(
                $"    {this.accessibility.ToDisplayString()} {this.name}({string.Join(", ", combinedParameters)})");

            stringBuilder.AppendLine(
                $"        : base({string.Join(", ", this.baseParameters.Select(static x => x.Name))})");
        }
        else
        {
            stringBuilder.AppendLine(
                $"    {this.accessibility.ToDisplayString()} {this.name}({string.Join(", ", this.parameters)})");
        }

        stringBuilder.AppendLine("    {");

        foreach (FieldAssignmentDefinition? arg in this.fieldAssignments)
        {
            arg.WriteTo(stringBuilder);
        }

        stringBuilder.AppendLine("    }");
    }
}

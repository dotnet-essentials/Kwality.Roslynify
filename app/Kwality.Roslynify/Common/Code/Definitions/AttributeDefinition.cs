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
using Kwality.Roslynify.Common.Extensions.Attributes;
using Kwality.Roslynify.Common.Helpers.Normalizers;

internal sealed class AttributeDefinition(string name, AttributeTargets targets, TypeAccessibility accessibility)
{
    public override string ToString()
    {
        string attributeName = AttributeNameNormalizer.Normalize(name);

        return $$"""
                 [global::System.AttributeUsage({{targets.ToUsageString()}})]
                 {{accessibility.ToDisplayString()}} sealed class {{attributeName}}Attribute : System.Attribute
                 {
                     // NOTE: Intentionally left blank.
                     //       Used as a "marker" attribute.
                 }
                 """;
    }
}

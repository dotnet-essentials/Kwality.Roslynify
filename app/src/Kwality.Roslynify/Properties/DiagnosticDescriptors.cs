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
namespace Kwality.Roslynify.Properties;

using Microsoft.CodeAnalysis;

internal static class DiagnosticDescriptors
{
    private const string Usage = "Usage";
    private const string Documentation = "Documentation";

    // ReSharper disable once InconsistentNaming
    public static readonly DiagnosticDescriptor KW001 = new(nameof(KW001),
        "The `DIConstructor` attribute can only be applied to `partial` classes",
        "The class `{0}` must be defined as `partial`", Usage, DiagnosticSeverity.Error, true,
        "The `DIConstructor` does only work when it's applied to a `partial` type.");

    // ReSharper disable once InconsistentNaming
    public static readonly DiagnosticDescriptor KW002 = new(nameof(KW002),
        "The `DIConstructor` attribute only make sense on types which contains `readonly` (NOT `static`, NOT `initialized` fields)",
        "The class `{0}` must at least have one `readonly` field (NOT `static`, NOT `initialized` fields)", Usage,
        DiagnosticSeverity.Warning, true,
        "The `DIConstructor` should only be placed on a classes that contains `readonly` (NOT `static`, NOT `initialized` fields).");

    // ReSharper disable once InconsistentNaming
    public static readonly DiagnosticDescriptor KW003 = new(nameof(KW003),
        "The `ProducesResponseType` must be be used with 2 arguments",
        "The attribute `ProducesResponseType` must have a valid type", Documentation, DiagnosticSeverity.Warning, true,
        "The `ProducesResponseType` must contain the model that it will return.");

    // ReSharper disable once InconsistentNaming
    public static readonly DiagnosticDescriptor KW004 = new(nameof(KW004),
        "The `ProducesResponseType` indicates that a status code is returned that's disallowed",
        "The method `{0}` returns the status code `{1}` which isn't allowed", Usage, DiagnosticSeverity.Error, true,
        "The `ProducesResponseType` must indicate an allowed status code.");
}

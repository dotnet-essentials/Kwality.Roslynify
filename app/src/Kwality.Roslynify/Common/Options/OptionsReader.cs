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
namespace Kwality.Roslynify.Common.Options;

using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

public sealed class OptionsReader
{
    private readonly AnalyzerConfigOptionsProvider provider;
    private readonly SyntaxTree syntaxTree;

    public OptionsReader(AnalyzerConfigOptionsProvider provider, SyntaxTree syntaxTree)
    {
        this.provider = provider;
        this.syntaxTree = syntaxTree;
    }

    public ImmutableHashSet<int> ReadInts(string key)
    {
        if (this.GetValue(key) is { } value)
            return value.Split(',').Select(x => x.Trim()).Select(int.Parse).ToImmutableHashSet();

        return new HashSet<int>().ToImmutableHashSet();
    }

    private string? GetValue(string key)
    {
        var options = this.provider.GetOptions(this.syntaxTree);

        return options.TryGetValue(key, out var value) ? value : string.Empty;
    }
}

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
namespace Kwality.Roslynify.Common.Helpers.Builders;

using System.Text;

internal sealed class IndentedStringBuilder
{
    private const char indentChar = ' ';
    private const int indentSize = 4;
    private readonly StringBuilder sBuilder = new();
    private int indentLevel;

    public void Indent()
    {
        this.indentLevel += 1;
    }

    public void Outdent()
    {
        if (this.IsIndented())
        {
            this.indentLevel -= 1;
        }
    }

    public bool IsIndented()
    {
        return this.indentLevel > 0;
    }

    public void AppendLine(string value)
    {
        var identPrefix = new string(indentChar, this.indentLevel * indentSize);
        this.sBuilder.AppendLine($"{identPrefix}{value}");
    }

    public override string ToString()
    {
        return this.sBuilder.ToString();
    }
}

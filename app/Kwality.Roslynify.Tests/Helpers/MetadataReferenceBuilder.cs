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
namespace Kwality.Roslynify.Tests.Helpers;

using System.Reflection;

using Microsoft.CodeAnalysis;

internal static class MetadataReferenceBuilder
{
    public static IEnumerable<MetadataReference> BuildRecursive(params Type[] typesInAssemblies)
    {
        return BuildRecursive(typesInAssemblies.SelectMany(GetAssembliesInType).ToArray());

        static IEnumerable<Assembly> GetAssembliesInType(Type type)
        {
            yield return type.Assembly;

            if (type.IsGenericType)
                foreach (var argument in type.GetGenericArguments())
                    yield return argument.Assembly;
        }
    }

    private static IEnumerable<MetadataReference> BuildRecursive(params Assembly[] assemblies)
    {
        if (assemblies is null) throw new ArgumentNullException(nameof(assemblies));

        foreach (var assembly in GetReferencedAssembliesRecursive(assemblies))
            yield return MetadataReference.CreateFromFile(assembly.Location);
    }

    private static HashSet<Assembly> GetReferencedAssembliesRecursive(IEnumerable<Assembly> assemblies,
        HashSet<Assembly>? recursiveAssemblies = null)
    {
        recursiveAssemblies ??= new HashSet<Assembly>();
        foreach (var assembly in assemblies) GetReferencedAssembliesRecursive(assembly, recursiveAssemblies);

        return recursiveAssemblies;
    }

    private static HashSet<Assembly> GetReferencedAssembliesRecursive(Assembly? assembly,
        HashSet<Assembly>? recursiveAssemblies = null)
    {
        recursiveAssemblies ??= new HashSet<Assembly>();

        if (assembly == null || !recursiveAssemblies.Add(assembly)) return recursiveAssemblies;

        foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
            if (TryGetOrLoad(referencedAssemblyName, out var referencedAssembly))
                _ = GetReferencedAssembliesRecursive(referencedAssembly, recursiveAssemblies);

        return recursiveAssemblies;

        bool TryGetOrLoad(AssemblyName name, out Assembly? result)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            result = assemblies.SingleOrDefault(x => IsMatch(x.GetName()));

            if (result != null) return true;

            try
            {
                result = Assembly.Load(name);

                return true;
            }
            catch
            {
                return false;
            }

            bool IsMatch(AssemblyName candidate)
            {
                return AssemblyName.ReferenceMatchesDefinition(candidate, name) && candidate.Version == name.Version;
            }
        }
    }
}

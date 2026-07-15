using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace FmgLib.MauiMarkup.Generator.Test;

/// <summary>
/// Guards the shipped extension surface against caller-side CS0121 ambiguities.
///
/// Two generic extension methods are ambiguous for a caller when they share the same name and
/// parameter shape while their <c>where T :</c> constraint targets sit on the same inheritance
/// chain (e.g. <c>FontSize</c> on both <c>Entry</c> and its base <c>InputView</c>): both become
/// applicable for the derived receiver and the compiler cannot choose. .NET MAUI regularly moves
/// bindable properties down to base classes between versions (Entry/Editor/SearchBar fonts moved
/// to InputView, Style/StyleClass moved to StyleableElement in MAUI 10), so this scan must stay
/// green after every MAUI version bump.
/// </summary>
[TestFixture]
public class ApiSurfaceTests
{
    [Test]
    public void ExtensionSurface_HasNoAmbiguousGenericMethodPairs()
    {
        var libraryPath = LocateLibrary();
        if (libraryPath is null)
        {
            Assert.Inconclusive("FmgLib.MauiMarkup.dll not found — build src/FmgLib.MauiMarkup first.");
            return;
        }

        using var context = CreateLoadContext(libraryPath);
        var assembly = context.LoadFromAssemblyPath(libraryPath);

        var entries = new List<(string Cls, string Name, string Shape, Type Target)>();

        foreach (var type in assembly.GetExportedTypes())
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                if (!method.IsGenericMethodDefinition || method.GetGenericArguments().Length != 1)
                {
                    continue;
                }

                var constraints = method.GetGenericArguments()[0]
                    .GetGenericParameterConstraints()
                    .Where(c => c.IsClass)
                    .ToArray();
                if (constraints.Length != 1)
                {
                    continue;
                }

                var shape = string.Join(",", method.GetParameters().Select(p => Normalize(p.ParameterType)));
                entries.Add((type.FullName!, method.Name, shape, constraints[0]));
            }
        }

        var report = new StringBuilder();
        var conflictCount = 0;

        foreach (var group in entries.GroupBy(e => (e.Name, e.Shape)))
        {
            var list = group.ToList();
            for (var i = 0; i < list.Count; i++)
            {
                for (var j = i + 1; j < list.Count; j++)
                {
                    var a = list[i];
                    var b = list[j];
                    if (a.Target == b.Target)
                    {
                        continue;
                    }

                    if (a.Target.IsAssignableFrom(b.Target) || b.Target.IsAssignableFrom(a.Target))
                    {
                        conflictCount++;
                        report.AppendLine($"{group.Key.Name}({group.Key.Shape})");
                        report.AppendLine($"    {a.Cls} [T : {a.Target.Name}]  <->  {b.Cls} [T : {b.Target.Name}]");
                    }
                }
            }
        }

        Assert.That(conflictCount, Is.Zero,
            $"Ambiguous extension method pairs detected (callers would hit CS0121):{Environment.NewLine}{report}");
    }

    private static string? LocateLibrary()
    {
        // Prefer the freshly built assembly; fall back to the checked-in metadata copy.
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !Directory.Exists(Path.Combine(dir.FullName, "src")))
        {
            dir = dir.Parent;
        }

        if (dir is not null)
        {
            var candidates = new[] { "Debug", "Release" }
                .Select(c => Path.Combine(dir.FullName, "src", "FmgLib.MauiMarkup", "bin", c, "net10.0", "FmgLib.MauiMarkup.dll"))
                .Where(File.Exists)
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .ToList();
            if (candidates.Count > 0)
            {
                return candidates[0];
            }
        }

        var fallback = Path.Combine(AppContext.BaseDirectory, "DLLs", "FmgLib.MauiMarkup.dll");
        return File.Exists(fallback) ? fallback : null;
    }

    private static MetadataLoadContext CreateLoadContext(string libraryPath)
    {
        var dlls = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        void AddDir(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            foreach (var file in Directory.GetFiles(path, "*.dll"))
            {
                dlls.TryAdd(Path.GetFileNameWithoutExtension(file), file);
            }
        }

        AddDir(RuntimeEnvironment.GetRuntimeDirectory());

        // Resolve MAUI (and friends) from the NuGet cache, preferring the highest version with a modern TFM.
        var nuget = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");
        if (Directory.Exists(nuget))
        {
            foreach (var package in Directory.GetDirectories(nuget))
            {
                static Version ParseVersion(string path)
                {
                    var name = Path.GetFileName(path);
                    var dash = name.IndexOf('-');
                    return Version.TryParse(dash > 0 ? name[..dash] : name, out var v) ? v : new Version(0, 0);
                }

                var versions = Directory.GetDirectories(package)
                    .OrderByDescending(ParseVersion);
                foreach (var version in versions)
                {
                    var added = false;
                    foreach (var tfm in new[] { "net10.0", "net9.0", "net8.0", "netstandard2.1", "netstandard2.0" })
                    {
                        var lib = Path.Combine(version, "lib", tfm);
                        if (Directory.Exists(lib))
                        {
                            AddDir(lib);
                            added = true;
                            break;
                        }
                    }

                    if (added)
                    {
                        break;
                    }
                }
            }
        }

        dlls[Path.GetFileNameWithoutExtension(libraryPath)] = libraryPath;
        return new MetadataLoadContext(new PathAssemblyResolver(dlls.Values));
    }

    private static string Normalize(Type type)
    {
        if (type.IsGenericParameter)
        {
            return "$" + type.GenericParameterPosition;
        }

        if (type.IsArray)
        {
            return Normalize(type.GetElementType()!) + "[]";
        }

        if (type.IsByRef)
        {
            return Normalize(type.GetElementType()!) + "&";
        }

        if (type.IsGenericType)
        {
            var name = type.GetGenericTypeDefinition().FullName ?? type.Name;
            var tick = name.IndexOf('`');
            if (tick > 0)
            {
                name = name[..tick];
            }

            return name + "<" + string.Join(",", type.GetGenericArguments().Select(Normalize)) + ">";
        }

        return type.FullName ?? type.Name;
    }
}

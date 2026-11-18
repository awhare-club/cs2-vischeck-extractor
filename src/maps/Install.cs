using Microsoft.Win32;

namespace Cs2VischeckExtractor.Maps;

static class Install
{
    public static string? Find(string? given)
    {
        if (!string.IsNullOrWhiteSpace(given))
            return Directory.Exists(given) ? given : null;

        string? steam = SteamPath();
        if (steam == null)
            return null;

        foreach (string library in Libraries(steam))
        {
            string cs2 = Path.Combine(library, "steamapps", "common", "Counter-Strike Global Offensive");
            if (Directory.Exists(cs2))
                return cs2;
        }

        return null;
    }

    static string? SteamPath()
    {
        if (OperatingSystem.IsWindows())
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
            if (key?.GetValue("SteamPath") is string steam)
                return steam.Replace('/', '\\');
        }

        string[] fallback =
        [
            @"C:\Program Files (x86)\Steam",
            @"C:\Program Files\Steam",
        ];
        return fallback.FirstOrDefault(Directory.Exists);
    }

    static List<string> Libraries(string steam)
    {
        var libraries = new List<string> { steam };
        string vdf = Path.Combine(steam, "steamapps", "libraryfolders.vdf");
        if (!File.Exists(vdf))
            return libraries;

        foreach (string line in File.ReadLines(vdf))
        {
            int key = line.IndexOf("\"path\"", StringComparison.Ordinal);
            if (key < 0)
                continue;
            int start = line.IndexOf('"', key + 6);
            int end = line.LastIndexOf('"');
            if (start < 0 || end <= start)
                continue;
            string path = line[(start + 1)..end].Replace("\\\\", "\\");
            if (path.Length > 0)
                libraries.Add(path);
        }

        return libraries;
    }
}

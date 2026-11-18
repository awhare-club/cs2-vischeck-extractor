namespace Cs2VischeckExtractor.Maps;

readonly record struct MapFile(string Name, string Path);

static class Catalog
{
    public static IReadOnlyList<MapFile> Official(string cs2, HashSet<string>? only)
    {
        string dir = System.IO.Path.Combine(cs2, "game", "csgo", "maps");
        if (!Directory.Exists(dir))
            return [];

        return Directory.GetFiles(dir, "*.vpk")
            .Select(path => new MapFile(System.IO.Path.GetFileNameWithoutExtension(path), path))
            .Where(map => IsPlayable(map.Name) && Wanted(map.Name, only))
            .ToList();
    }

    public static IReadOnlyList<MapFile> Workshop(string cs2, HashSet<string>? only)
    {
        string? steamapps = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(cs2));
        if (steamapps == null)
            return [];

        string workshop = System.IO.Path.Combine(steamapps, "workshop", "content", "730");
        if (!Directory.Exists(workshop))
            return [];

        var maps = new List<MapFile>();
        foreach (string item in Directory.GetDirectories(workshop))
        {
            string[] vpks = Directory.GetFiles(item, "*.vpk");
            string? package = BasePackage(vpks);
            if (package == null)
                continue;
            string name = System.IO.Path.GetFileNameWithoutExtension(package);
            if (!Wanted(name, only))
                continue;
            maps.Add(new MapFile(name, package));
        }

        return maps;
    }

    static bool IsPlayable(string name)
    {
        if (name.Contains("vanity", StringComparison.OrdinalIgnoreCase))
            return false;
        if (name.Contains("workshop", StringComparison.OrdinalIgnoreCase))
            return false;
        if (name.Contains("graphics_settings", StringComparison.OrdinalIgnoreCase))
            return false;
        if (name.Contains("lobby_mapveto", StringComparison.OrdinalIgnoreCase))
            return false;
        return name.StartsWith("de_", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("cs_", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("ar_", StringComparison.OrdinalIgnoreCase);
    }

    static bool Wanted(string name, HashSet<string>? only)
    {
        return only == null || only.Contains(name);
    }

    static string? BasePackage(string[] vpks)
    {
        return vpks.FirstOrDefault(path =>
        {
            string stem = System.IO.Path.GetFileNameWithoutExtension(path);
            if (stem.Contains("vanity", StringComparison.OrdinalIgnoreCase))
                return false;
            return !Chunk.IsMatch(stem);
        });
    }

    static readonly System.Text.RegularExpressions.Regex Chunk = new(@"_\d{3}$", System.Text.RegularExpressions.RegexOptions.CultureInvariant);
}

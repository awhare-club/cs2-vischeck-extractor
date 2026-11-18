namespace Cs2VischeckExtractor.Vpk;

readonly record struct WorldPhysicsFile(string MapName, byte[] Bytes, string ResourceName);

static class WorldPhysics
{
    public static WorldPhysicsFile? Read(string vpkPath)
    {
        using FileStream stream = File.OpenRead(vpkPath);
        return Read(stream, vpkPath, depth: 0);
    }

    static WorldPhysicsFile? Read(Stream stream, string? vpkPath, int depth)
    {
        Tree? directory = Tree.Open(stream);
        if (directory == null)
            return null;

        Entry? physics = null;
        foreach (Entry entry in directory.Entries)
        {
            if (!entry.Extension.Equals("vmdl_c", StringComparison.OrdinalIgnoreCase))
                continue;
            if (!entry.Name.Equals("world_physics", StringComparison.OrdinalIgnoreCase))
                continue;
            if (entry.Directory.Contains("sky", StringComparison.OrdinalIgnoreCase))
                continue;
            physics = entry;
        }

        if (physics is Entry found && found.Length > 0)
        {
            byte[] bytes = directory.Read(stream, vpkPath, found);
            string map = MapName(found.Directory, vpkPath);
            return new WorldPhysicsFile(map, bytes, $"{found.Directory}/world_physics.vmdl_c");
        }

        if (depth >= 1)
            return null;

        Entry? nested = ShortestMapVpk(directory.Entries);
        if (nested is not Entry inner || inner.Length == 0)
            return null;

        byte[] blob = directory.Read(stream, vpkPath, inner);
        using var memory = new MemoryStream(blob);
        return Read(memory, vpkPath: null, depth + 1);
    }

    static Entry? ShortestMapVpk(IReadOnlyList<Entry> entries)
    {
        Entry? best = null;
        foreach (Entry entry in entries)
        {
            if (!entry.Extension.Equals("vpk", StringComparison.OrdinalIgnoreCase))
                continue;
            if (!entry.Directory.Equals("maps", StringComparison.OrdinalIgnoreCase))
                continue;
            if (entry.Name.Contains("sky", StringComparison.OrdinalIgnoreCase))
                continue;
            if (best == null || entry.Name.Length < best.Value.Name.Length)
                best = entry;
        }

        return best;
    }

    static string MapName(string directory, string? vpkPath)
    {
        if (directory.Length > 0)
        {
            string[] parts = directory.Split('/', '\\');
            if (parts.Length > 0 && parts[^1].Length > 0)
                return parts[^1];
        }

        return vpkPath == null ? "map" : Path.GetFileNameWithoutExtension(vpkPath);
    }
}

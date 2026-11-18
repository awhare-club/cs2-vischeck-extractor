using System.Diagnostics;
using Cs2VischeckExtractor.Maps;
using Cs2VischeckExtractor.Physics;
using Cs2VischeckExtractor.Tri;
using Cs2VischeckExtractor.Vpk;

namespace Cs2VischeckExtractor.Cli;

static class Run
{
    readonly record struct MapResult(string Name, bool Wrote, string? Note, int Triangles, TimeSpan Time);

    public static int Start(string[] args)
    {
        if (!Args.TryParse(args, out Args options, out string error))
        {
            Out.Error(error);
            Console.Error.WriteLine(Args.Usage);
            return 2;
        }

        if (options.Help)
        {
            Console.WriteLine(Args.Usage);
            return 0;
        }

        string? cs2 = Install.Find(options.Cs2);
        if (cs2 == null)
        {
            Out.Error("CS2 installation not found.");
            return 1;
        }

        HashSet<string>? only = options.Only();
        var maps = new List<MapFile>();
        maps.AddRange(Catalog.Official(cs2, only));
        if (options.Workshop)
            maps.AddRange(Catalog.Workshop(cs2, only));
        if (maps.Count == 0)
        {
            Out.Error("No maps matched.");
            return 1;
        }

        Out.Cs2(cs2);
        Directory.CreateDirectory(options.Out);
        var results = new MapResult[maps.Count];
        var clock = Stopwatch.StartNew();
        Parallel.For(0, maps.Count, i => results[i] = WriteMap(maps[i], options.Out));
        clock.Stop();

        int wrote = 0;
        var written = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (MapResult result in results)
        {
            if (result.Wrote)
            {
                Out.Wrote(result.Name, result.Triangles, result.Time);
                written.Add(result.Name);
                wrote++;
            }
            else if (result.Note != null && result.Note.StartsWith("failed", StringComparison.Ordinal))
            {
                Out.Fail(result.Name, result.Note);
            }
            else
            {
                Out.Miss(result.Name, result.Note ?? "no mesh");
            }
        }

        Out.Done(wrote, maps.Count - wrote, clock.Elapsed, options.Out);
        if (only != null && only.Any(name => !written.Contains(name)))
            return 1;
        return wrote == 0 ? 1 : 0;
    }

    static MapResult WriteMap(MapFile map, string outDir)
    {
        var clock = Stopwatch.StartNew();
        try
        {
            WorldPhysicsFile? world = WorldPhysics.Read(map.Path);
            if (world == null)
                return new MapResult(map.Name, false, "no world physics", 0, clock.Elapsed);

            MeshExtract? mesh = Extract.FromResource(world.Value.Bytes, world.Value.ResourceName);
            if (mesh == null || mesh.Value.Triangles.Count == 0)
                return new MapResult(world.Value.MapName, false, "no default collision", 0, clock.Elapsed);

            TriFile.Write(Path.Combine(outDir, world.Value.MapName + ".tri"), mesh.Value.Triangles);
            return new MapResult(world.Value.MapName, true, null, mesh.Value.Triangles.Count, clock.Elapsed);
        }
        catch (Exception ex)
        {
            return new MapResult(map.Name, false, "failed " + ex.Message, 0, clock.Elapsed);
        }
    }
}

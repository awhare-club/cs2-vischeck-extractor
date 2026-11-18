using Cs2VischeckExtractor.Tri;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;

namespace Cs2VischeckExtractor.Physics;

readonly record struct MeshExtract(List<Triangle> Triangles, int Hulls, int Meshes, string? Phys);

static class Extract
{
    public static MeshExtract? FromResource(byte[] file, string resourceName, bool includePhys)
    {
        if (file.Length == 0)
            return null;

        using var resource = new Resource();
        using var stream = new MemoryStream(file, writable: false);
        resource.Read(stream);
        resource.FileName = resourceName;

        PhysAggregateData? data = null;
        foreach (Block block in resource.Blocks)
        {
            if (block is PhysAggregateData found)
            {
                data = found;
                break;
            }
        }

        if (data == null || data.Parts.Length == 0)
            return null;

        HashSet<int> groups = Collision.DefaultIndices(data.CollisionAttributes);
        var shape = data.Parts[0].Shape;
        var triangles = new List<Triangle>();
        int hulls = Hulls.Collect(shape.Hulls, groups, triangles);
        int meshes = Meshes.Collect(shape.Meshes, groups, triangles);
        string? text = includePhys ? Dump(data) : null;
        if (triangles.Count == 0 && string.IsNullOrEmpty(text))
            return null;
        return new MeshExtract(triangles, hulls, meshes, text);
    }

    static string? Dump(PhysAggregateData data)
    {
        string text = data.ToString() ?? "";
        return text.Length == 0 ? null : text;
    }
}

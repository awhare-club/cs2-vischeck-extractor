using Cs2VischeckExtractor.Tri;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;

namespace Cs2VischeckExtractor.Physics;

readonly record struct MeshExtract(List<Triangle> Triangles, int Hulls, int Meshes);

static class Extract
{
    public static MeshExtract? FromResource(byte[] file, string resourceName)
    {
        if (file.Length == 0)
            return null;

        using var resource = new Resource();
        using var stream = new MemoryStream(file, writable: false);
        resource.Read(stream);
        resource.FileName = resourceName;

        PhysAggregateData? phys = null;
        foreach (Block block in resource.Blocks)
        {
            if (block is PhysAggregateData data)
            {
                phys = data;
                break;
            }
        }

        if (phys == null || phys.Parts.Length == 0)
            return null;

        HashSet<int> groups = Collision.DefaultIndices(phys.CollisionAttributes);
        var shape = phys.Parts[0].Shape;
        var triangles = new List<Triangle>();
        int hulls = Hulls.Collect(shape.Hulls, groups, triangles);
        int meshes = Meshes.Collect(shape.Meshes, groups, triangles);
        if (triangles.Count == 0)
            return null;
        return new MeshExtract(triangles, hulls, meshes);
    }
}

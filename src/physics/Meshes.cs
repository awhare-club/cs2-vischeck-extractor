using Cs2VischeckExtractor.Tri;
using ValveResourceFormat.ResourceTypes.RubikonPhysics;
using ValveResourceFormat.ResourceTypes.RubikonPhysics.Shapes;

namespace Cs2VischeckExtractor.Physics;

static class Meshes
{
    public static int Collect(MeshDescriptor[] meshes, HashSet<int> groups, List<Triangle> triangles)
    {
        int count = 0;
        foreach (MeshDescriptor mesh in meshes)
        {
            if (!groups.Contains(mesh.CollisionAttributeIndex))
                continue;

            ReadOnlySpan<System.Numerics.Vector3> points = mesh.Shape.GetVertices();
            ReadOnlySpan<Mesh.Triangle> indices = mesh.Shape.GetTriangles();
            if (points.IsEmpty || indices.IsEmpty)
                continue;

            foreach (Mesh.Triangle index in indices)
            {
                if ((uint)index.X >= (uint)points.Length || (uint)index.Y >= (uint)points.Length || (uint)index.Z >= (uint)points.Length)
                    continue;
                triangles.Add(new Triangle(
                    new Vector3(points[index.X].X, points[index.X].Y, points[index.X].Z),
                    new Vector3(points[index.Y].X, points[index.Y].Y, points[index.Y].Z),
                    new Vector3(points[index.Z].X, points[index.Z].Y, points[index.Z].Z)));
            }

            count++;
        }

        return count;
    }
}

using Cs2VischeckExtractor.Tri;
using ValveResourceFormat.ResourceTypes.RubikonPhysics;
using ValveResourceFormat.ResourceTypes.RubikonPhysics.Shapes;

namespace Cs2VischeckExtractor.Physics;

static class Hulls
{
    public static int Collect(HullDescriptor[] hulls, HashSet<int> groups, List<Triangle> triangles)
    {
        int count = 0;
        foreach (HullDescriptor hull in hulls)
        {
            if (!groups.Contains(hull.CollisionAttributeIndex))
                continue;

            ReadOnlySpan<System.Numerics.Vector3> points = hull.Shape.GetVertexPositions();
            ReadOnlySpan<Hull.Face> faces = hull.Shape.GetFaces();
            ReadOnlySpan<Hull.HalfEdge> edges = hull.Shape.GetEdges();
            if (points.IsEmpty || faces.IsEmpty || edges.IsEmpty)
                continue;

            Fan(points, faces, edges, triangles);
            count++;
        }

        return count;
    }

    static void Fan(
        ReadOnlySpan<System.Numerics.Vector3> points,
        ReadOnlySpan<Hull.Face> faces,
        ReadOnlySpan<Hull.HalfEdge> edges,
        List<Triangle> triangles)
    {
        foreach (Hull.Face face in faces)
        {
            int start = face.Edge;
            if ((uint)start >= (uint)edges.Length)
                continue;

            int edge = edges[start].Next;
            for (int step = 0; edge != start && step < 1000; step++)
            {
                if ((uint)edge >= (uint)edges.Length)
                    break;
                int next = edges[edge].Next;
                if (next == start || (uint)next >= (uint)edges.Length)
                    break;

                int a = edges[start].Origin;
                int b = edges[edge].Origin;
                int c = edges[next].Origin;
                if ((uint)a < (uint)points.Length && (uint)b < (uint)points.Length && (uint)c < (uint)points.Length)
                {
                    triangles.Add(new Triangle(
                        new Vector3(points[a].X, points[a].Y, points[a].Z),
                        new Vector3(points[b].X, points[b].Y, points[b].Z),
                        new Vector3(points[c].X, points[c].Y, points[c].Z)));
                }

                edge = next;
            }
        }
    }
}

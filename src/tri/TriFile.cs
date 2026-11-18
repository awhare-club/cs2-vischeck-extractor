using System.Runtime.InteropServices;

namespace Cs2VischeckExtractor.Tri;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
readonly record struct Vector3(float X, float Y, float Z);

[StructLayout(LayoutKind.Sequential, Pack = 1)]
readonly record struct Triangle(Vector3 A, Vector3 B, Vector3 C);

static class TriFile
{
    public static void Write(string path, List<Triangle> triangles)
    {
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 1 << 20);
        stream.Write(MemoryMarshal.AsBytes(CollectionsMarshal.AsSpan(triangles)));
    }
}

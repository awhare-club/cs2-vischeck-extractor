using ValveKeyValue;

namespace Cs2VischeckExtractor.Physics;

static class Collision
{
    public static HashSet<int> DefaultIndices(IReadOnlyList<KVObject> attributes)
    {
        var indices = new HashSet<int>();
        for (int i = 0; i < attributes.Count; i++)
        {
            string group = attributes[i]["m_CollisionGroupString"].ToString()?.Trim().Trim('"') ?? "";
            if (group.Equals("default", StringComparison.OrdinalIgnoreCase) || group == "0")
                indices.Add(i);
        }

        if (indices.Count == 0)
            indices.Add(0);
        return indices;
    }
}

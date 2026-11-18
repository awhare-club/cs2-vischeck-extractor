using System.Text;

namespace Cs2VischeckExtractor.Vpk;

readonly record struct Entry(string Extension, string Directory, string Name, ushort Archive, uint Offset, uint Length);

sealed class Tree
{
    public long DataStart { get; }
    public IReadOnlyList<Entry> Entries { get; }

    Tree(long dataStart, List<Entry> entries)
    {
        DataStart = dataStart;
        Entries = entries;
    }

    public static Tree? Open(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true);
        if (stream.Length - stream.Position < 12)
            return null;
        if (reader.ReadUInt32() != 0x55AA1234)
            return null;

        uint version = reader.ReadUInt32();
        uint treeSize = reader.ReadUInt32();
        if (version == 2)
        {
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
        }
        else if (version != 1)
        {
            return null;
        }

        long headerSize = stream.Position;
        byte[] tree = reader.ReadBytes(checked((int)treeSize));
        if (tree.Length != treeSize)
            return null;

        return new Tree(headerSize + treeSize, Walk(tree));
    }

    public byte[] Read(Stream stream, string? vpkPath, Entry entry)
    {
        if (entry.Archive == 0x7FFF)
        {
            stream.Position = DataStart + entry.Offset;
            byte[] inline = new byte[entry.Length];
            stream.ReadExactly(inline);
            return inline;
        }

        if (vpkPath == null)
            throw new InvalidDataException("physics data is in another archive");

        string? folder = Path.GetDirectoryName(vpkPath);
        string chunk = Path.Combine(folder ?? "", Path.GetFileNameWithoutExtension(vpkPath) + $"_{entry.Archive:D3}.vpk");
        using var file = File.OpenRead(chunk);
        file.Position = entry.Offset;
        byte[] data = new byte[entry.Length];
        file.ReadExactly(data);
        return data;
    }

    static List<Entry> Walk(byte[] tree)
    {
        var entries = new List<Entry>();
        int pos = 0;
        while (pos < tree.Length)
        {
            string ext = ReadZ(tree, ref pos);
            if (ext.Length == 0)
                break;
            while (true)
            {
                string dir = ReadZ(tree, ref pos);
                if (dir.Length == 0)
                    break;
                while (true)
                {
                    string name = ReadZ(tree, ref pos);
                    if (name.Length == 0)
                        break;
                    if (pos + 18 > tree.Length)
                        return entries;
                    ushort preload = BitConverter.ToUInt16(tree, pos + 4);
                    ushort archive = BitConverter.ToUInt16(tree, pos + 6);
                    uint offset = BitConverter.ToUInt32(tree, pos + 8);
                    uint length = BitConverter.ToUInt32(tree, pos + 12);
                    pos += 18 + preload;
                    entries.Add(new Entry(ext, dir, name, archive, offset, length));
                }
            }
        }

        return entries;
    }

    static string ReadZ(byte[] tree, ref int pos)
    {
        int start = pos;
        while (pos < tree.Length && tree[pos] != 0)
            pos++;
        string text = Encoding.ASCII.GetString(tree, start, pos - start);
        if (pos < tree.Length)
            pos++;
        return text;
    }
}

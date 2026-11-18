namespace Cs2VischeckExtractor.Vphys;

static class PhysFile
{
    public static void Write(string path, string text)
    {
        File.WriteAllText(path, text);
    }
}

namespace Cs2VischeckExtractor.Cli;

static class Out
{
    static readonly bool Color = !Console.IsOutputRedirected;
    static string Name => Color ? "\x1b[36m" : "";
    static string Ok => Color ? "\x1b[32m" : "";
    static string Warn => Color ? "\x1b[33m" : "";
    static string Bad => Color ? "\x1b[31m" : "";
    static string Dim => Color ? "\x1b[2m" : "";
    static string Off => Color ? "\x1b[0m" : "";

    public static void Cs2(string path)
    {
        Console.WriteLine($"{Dim}cs2{Off}  {path}");
        Console.WriteLine();
    }

    public static void Wrote(string name, string files, TimeSpan time)
    {
        Console.WriteLine($"{Name}{name,-18}{Off}  {Ok}{files}{Off}  {Dim}{time.TotalSeconds:0.0}s{Off}");
    }

    public static void Miss(string name, string reason)
    {
        Console.WriteLine($"{Warn}{name,-18}{Off}  {Dim}{reason}{Off}");
    }

    public static void Fail(string name, string reason)
    {
        Console.WriteLine($"{Bad}{name,-18}{Off}  {reason}");
    }

    public static void Done(int wrote, int missed, TimeSpan time, string where)
    {
        Console.WriteLine();
        Console.WriteLine($"{Ok}{wrote} written{Off}  {Dim}{missed} missed   {time.TotalSeconds:0.0}s   {where}{Off}");
    }

    public static void Error(string message)
    {
        Console.Error.WriteLine($"{Bad}{message}{Off}");
    }
}

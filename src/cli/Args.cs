namespace Cs2VischeckExtractor.Cli;

sealed class Args
{
    public const string Usage =
        """
        cs2-vischeck-extractor [--only de_mirage,de_dust2] [--workshop] [--cs2 path]
                               [--tri] [--vphys]

          --only       comma-separated map names
          --tri        write .tri meshes
          --vphys      write the physics dump
          --workshop   also read workshop maps
          --cs2        Counter-Strike Global Offensive directory

        Files are written to ./out.
        """;

    public bool Help { get; private init; }
    public string? OnlyText { get; private init; }
    public string Out { get; private init; } = "";
    public bool Tri { get; private init; }
    public bool Vphys { get; private init; }
    public bool Workshop { get; private init; }
    public string? Cs2 { get; private init; }

    public HashSet<string>? Only()
    {
        if (string.IsNullOrWhiteSpace(OnlyText))
            return null;
        return new HashSet<string>(
            OnlyText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
            StringComparer.OrdinalIgnoreCase);
    }

    public static bool TryParse(string[] args, out Args options, out string error)
    {
        options = new Args { Out = Path.Combine(Directory.GetCurrentDirectory(), "out") };
        error = "";
        string? only = null;
        string? cs2 = null;
        bool tri = false;
        bool vphys = false;
        bool choseFile = false;
        bool workshop = false;
        bool help = false;

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];
            if (arg is "-h" or "--help")
            {
                help = true;
                continue;
            }

            string? value = Value(args, arg, ref i);
            switch (Name(arg))
            {
                case "--only":
                    only = value ?? Missing(out error);
                    break;
                case "--cs2":
                    cs2 = value ?? Missing(out error);
                    break;
                case "--tri":
                    if (value != null)
                    {
                        error = "--tri takes no value";
                        return false;
                    }
                    tri = true;
                    choseFile = true;
                    break;
                case "--vphys":
                    if (value != null)
                    {
                        error = "--vphys takes no value";
                        return false;
                    }
                    vphys = true;
                    choseFile = true;
                    break;
                case "--workshop":
                    if (value != null)
                    {
                        error = "--workshop takes no value";
                        return false;
                    }
                    workshop = true;
                    break;
                default:
                    error = $"Unknown argument: {arg}";
                    return false;
            }

            if (error.Length > 0)
                return false;
        }

        if (!choseFile)
            tri = true;

        options = new Args
        {
            Help = help,
            OnlyText = only,
            Out = options.Out,
            Tri = tri,
            Vphys = vphys,
            Workshop = workshop,
            Cs2 = cs2,
        };
        return true;
    }

    static string? Value(string[] args, string arg, ref int index)
    {
        int eq = arg.IndexOf('=');
        if (eq >= 0)
            return arg[(eq + 1)..];
        if (index + 1 < args.Length && !args[index + 1].StartsWith('-'))
            return args[++index];
        return null;
    }

    static string Name(string arg)
    {
        int eq = arg.IndexOf('=');
        return eq >= 0 ? arg[..eq] : arg;
    }

    static string Missing(out string error)
    {
        error = "Missing value";
        return "";
    }
}

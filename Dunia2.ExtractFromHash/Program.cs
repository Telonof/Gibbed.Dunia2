using Gibbed.Dunia2.FileFormats;
using Gibbed.Dunia2.FileFormats.Big;
using Gibbed.ProjectData;

internal class Program
{

    static private string originalOutputFolder;

    static private string outputFolder;

    static private string rootDirectory;

    private static string GetExecutableName()
    {
        return Path.GetFileName(Environment.ProcessPath);
    }

    private static void Main(String[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine($"Usage: {GetExecutableName()} <folder> <hash> [output]");
            return;
        }

        if (!Directory.Exists(args[0]))
        {
            Console.WriteLine("Could not find folder.");
            return;
        }

        originalOutputFolder = args.Length > 2 ? args[2] : "output";
        rootDirectory = args[0];

        searchRecursive(args[0], args[1]);
    }

    static private void searchRecursive(string directory, string hash)
    {
        //Ensure each game has it's own output folder.
        string[] subFolderName = directory.Replace(rootDirectory, "").Replace("\\", "/").Split("/");
        if (subFolderName.Length > 1)
        {
            outputFolder = Path.Combine(originalOutputFolder, subFolderName[1]);
        }

        string[] files = Directory.GetFiles(directory);

        foreach (string file in files)
        {
            if (!file.EndsWith(".fat"))
                continue;

            getHashes(file, hash, Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(file)));
        }

        files = Directory.GetDirectories(directory);

        foreach (string file in files)
        {
            searchRecursive(file, hash);
        }
    }

    static private void getHashes(string file, string hash, string output)
    {
        BigFile fat;
        using (var input = File.OpenRead(file))
        {
            fat = new BigFile();
            fat.Deserialize(input);
        }

        var hashes = Manager.Load().LoadListsFileNames(fat.Version);

        foreach (var entry in fat.Entries)
        {
            //Convert hash to little endian format
            string foundHash = entry.NameHash.ToString("X8");
            while (foundHash.Length < 16)
                foundHash = "0" + foundHash;

            string littleEndianHash = "";
            for (int i = foundHash.Length - 1; i > 0; i -= 2)
            {
                littleEndianHash += foundHash[i - 1];
                littleEndianHash += foundHash[i];
            }

            if (!littleEndianHash.Equals(hash))
                continue;

            string outputName = hashes[entry.NameHash];
            if (outputName == null)
            {
                while (foundHash.StartsWith("0"))
                    foundHash = foundHash.Substring(1);

                outputName = Path.Combine("unknown", foundHash);
            }

            output = Path.Combine(output, outputName);
            Directory.CreateDirectory(Path.GetDirectoryName(output));

            unpack(entry, file.Substring(0, file.Length - 3) + "dat", output);
            Console.WriteLine($"Found {hash} in file {file}");
        }
    }

    private static void unpack(Entry entry, string datFile, string outputPath)
    {
        Stream file = File.OpenRead(datFile);
        string path = Path.Combine(datFile, outputPath);
        using (var output = File.Create(outputPath))
        {
            EntryDecompression.Decompress(entry, file, output);
        }
        file.Close();
    }
}

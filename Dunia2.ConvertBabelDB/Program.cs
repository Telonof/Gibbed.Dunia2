using Gibbed.Dunia2.FileFormats;

namespace Dunia2.ConvertBabelDB
{
    internal class Program
    {
        private static readonly List<string> lines = [];

        private static string GetExecutableName()
        {
            return Path.GetFileName(Environment.ProcessPath);
        }

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Converts a BabDB file to a CSV.");
                Console.WriteLine($"Usage: {GetExecutableName()} <file>");
                return;
            }

            foreach (string arg in args)
            {
                if (!File.Exists(arg))
                {
                    Console.WriteLine($"File {arg} not found.");
                    continue;
                }

                Console.WriteLine($"Converting {arg}");
                ConvertToCSV(arg);
            }
        }

        private static void ConvertToCSV(string path)
        {
            lines.Clear();
            BabelDBFile babdb = new BabelDBFile();
            babdb.Deseralize(File.OpenRead(path));

            //Add column data
            string columns = "";

            foreach (Column column in babdb.Columns)
            {
                columns += column.Name + "\t";
            }

            lines.Add(columns);

            //Add row data
            string rowCSV = "";
            foreach (Row row in babdb.Rows)
            {
                foreach (byte[] data in row.data)
                {
                    rowCSV += Convert.ToHexString(data) + "\t";
                }

                lines.Add(rowCSV);
                rowCSV = "";
            }

            File.WriteAllLines(Path.ChangeExtension(path, ".csv"), lines);
        }
    }
}
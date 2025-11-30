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

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("File not found.");
                return;
            }

            BabelDBFile babdb = new BabelDBFile();
            babdb.Deseralize(File.OpenRead(args[0]));

            //Add column data
            string columns = "";

            foreach (Column column in babdb.Columns)
            {
                columns += column.Name + ",";
            }

            lines.Add(columns);

            //Add row data
            string rowCSV = "";
            foreach (Row row in babdb.Rows)
            {
                foreach (byte[] data in row.data)
                {
                    rowCSV += Convert.ToHexString(data) + ",";
                }

                lines.Add(rowCSV);
                rowCSV = "";
            }

            File.WriteAllLines(Path.ChangeExtension(args[0], ".csv"), lines);
        }
    }
}
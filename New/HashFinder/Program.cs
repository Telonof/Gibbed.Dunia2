using System.Globalization;

namespace HashFinder
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var y = Gibbed.Dunia2.FileFormats.CRC32.Hash("ab");
            var x = uint.Parse("9E83486D", NumberStyles.HexNumber);

            if (args.Length < 2 || !File.Exists(args[0]))
            {
                Console.WriteLine("Usage: HashFinder.exe <InputFile> <OutputFile>");

                return;
            }

            var values = new HashSet<string>();

            using (var reader = new BinaryReader(File.OpenRead(args[0])))
            {
                var bytes = new List<byte>();

                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    var readByte = reader.ReadByte();
                    var readChar = readByte < 127 ? (char)readByte : char.MinValue;

                    if (readChar != char.MinValue)
                    {
                        bytes.Add(readByte);
                    }
                    else
                    {
                        if (bytes.Count > 1 && IsValidStartChar(bytes[0]) && bytes.All(IsValidChar))
                        {
                            var value = Encoding.ASCII.GetString(bytes.ToArray());

                            values.Add(value);
                        }

                        bytes.Clear();
                    }
                }
            }

            using (var writer = File.CreateText(args[1]))
            {
                writer.WriteLine("{");

                var orderedValues = values.OrderBy(a => a).ToArray();

                foreach (var value in orderedValues)
                {
                    var bytes = Encoding.ASCII.GetBytes(value).ToArray();
                    var crcHash = BitConverter.GetBytes(Gibbed.Dunia2.FileFormats.CRC32.Hash(bytes, 0, bytes.Length)).Reverse().ToArray();
                    var hexString = BitConverter.ToUInt32(crcHash, 0).ToString("X8");

                    writer.Write($"    \"{hexString}\": \"{value}\"");
                    
                    writer.WriteLine(orderedValues.Last() != value ? "," : string.Empty);
                }

                writer.WriteLine("}");
            }
        }

        public static bool IsValidChar(byte c)
        {
            return IsValidStartChar(c) || c == '_' || (c >= '0' && c <= '9');
        }
        public static bool IsValidStartChar(byte c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }
    }
}

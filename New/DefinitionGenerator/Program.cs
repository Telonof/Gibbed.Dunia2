using CommandLine;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using Common.Extensions;
using Gibbed.Dunia2.FileFormats;

namespace DefinitionGenerator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(a =>
                {
                    Console.WriteLine(a.ToString());
                    Console.ReadLine();
                });
        }

        private static void RunOptions(Options options)
        {
            if (!File.Exists(options.InputXmlFile))
                throw new ApplicationException("Input file not found.");

            var sourceObject = FcObject.Deserialize(options.InputXmlFile);
            var sourceObjects = sourceObject.AllObjectsIncludingSelf();

            const string binHexType = "BinHex";

            foreach (var fcObject in sourceObjects)
            {
                foreach (var fcField in fcObject.Fields)
                {
                    if (fcField.Type == binHexType)
                    {
                        var name = fcField.Name;
                        var textBytes = fcField.Text != null ? HexExtensions.HexStringToBytes(fcField.Text) : new byte[0];

                        if (fcField.Type != binHexType || textBytes.Length == 0 || fcField.Text == null)
                            continue;

                        if (fcField.Name != null)
                        {
                            if (name.Length > 1 && (name.StartsWith("i") || name.StartsWith("n")) &&
                                name[1].IsUpper() && textBytes.Length == 4)
                            {
                                fcField.Type = "_Int32";
                            }
                            else if (name.Length > 1 && name.StartsWith("f") && name[1].IsUpper() &&
                                     textBytes.Length == 4)
                            {
                                fcField.Type = "_Float32";
                            }
                            else if (name.Length > 1 && (name.StartsWith("i") || name.StartsWith("n")) &&
                                     name[1].IsUpper() && textBytes.Length == 2)
                            {
                                fcField.Type = "_Int16";
                            }
                            else if (name.Length > 1 && name.StartsWith("f") && name[1].IsUpper() &&
                                     textBytes.Length == 2)
                            {
                                fcField.Type = "_Float16";
                            }
                            else if (name.Length > 6 && name.StartsWith("vector") && name[6].IsUpper() && textBytes.Length == 12)
                            {
                                fcField.Type = "_Vector3";
                            }

                            if (fcField.Type != binHexType)
                                continue;
                        }

                        if (textBytes.Length > 1 && textBytes.Take(textBytes.Length - 1).All(b => b >= 32 && b <= 126) && textBytes.Last() == '\0')
                        {
                            // String

                            fcField.Type = "_String";
                        }

                        if (fcField.Type != binHexType)
                            continue;

                        if (textBytes.Length == 4)
                        {
                            // Int32, Float32, Hash32, Enum, Id32

                            // Heuristic check for CRC
                            // Heuristic range check for int vs float
                            
                            if (fcField.Text.All(c => c > 0 && ((char)c).IsHexChar()))
                            {
                                fcField.Type = "_Hash32";
                            }
                            else
                            {
                                var intValue = BitConverter.ToInt32(textBytes, 0);

                                if (intValue < 99999999 && intValue > -99999999)
                                {
                                    fcField.Type = "_Int32";
                                }
                                else
                                {
                                    if (!float.IsNaN(BitConverter.ToSingle(textBytes, 0)))
                                        fcField.Type = "_Float32";
                                }
                            }
                        }

                        if (fcField.Type != binHexType)
                            continue;

                        if (textBytes.Length == 1 && (textBytes[0] == 1 || textBytes[0] == 0))
                        {
                            // Boolean

                            fcField.Type = "_Boolean";
                        }
                        
                        if (fcField.Type != binHexType)
                            continue;

                        if (textBytes.Length == 8)
                        {
                            // Int64, Id64, Float64

                            fcField.Type = "_Int64";
                        }
                    }
                }
            }

            var debugObject = sourceObject.Clone();

            foreach (var fcObject in debugObject.AllObjectsIncludingSelf())
            {
                if (fcObject.Name == null)
                    fcObject.Name = "##MISSING##";
            }

            foreach (var field in debugObject.AllFieldsIncludingSelf())
            {
                if (field.Name == null)
                    field.Name = "##MISSING##";

                if (field.Type == binHexType || field.Text == null || !field.Type.StartsWith("_"))
                    continue;

                var textBytes = field.Text != null ? HexExtensions.HexStringToBytes(field.Text) : new byte[0];

                switch (field.Type)
                {
                    case "_String":
                        var text = field.Text;

                        field.Text = text + " / " + Encoding.ASCII.GetString(textBytes).Replace("\0", string.Empty);

                        if (!field.Text.All(a => a >= 32 && a <= 126))
                        {
                            field.Text = text;
                            field.Type = binHexType;

                            Console.WriteLine($"Invalid string for '{field.Name}'.");
                        }
                        break;
                    case "_Int32":
                        field.Text = field.Text + " / " + BitConverter.ToInt32(textBytes, 0);
                        break;
                    case "_Float32":
                        field.Text = field.Text + " / " + BitConverter.ToSingle(textBytes, 0);
                        break;
                    case "_Int64":
                    case "_Id64":
                        field.Text = field.Text + " / " + BitConverter.ToInt64(textBytes, 0);
                        break;
                    case "_Boolean":
                        field.Text = field.Text + " / " + BitConverter.ToBoolean(textBytes, 0);
                        break;
                    case "_Vector3":
                        var vector = new Vector3
                        {
                            X = BitConverter.ToSingle(textBytes, 0),
                            Y = BitConverter.ToSingle(textBytes, 4),
                            Z = BitConverter.ToSingle(textBytes, 8),
                        };

                        field.Text = field.Text + " / " + vector.ToString();
                        break;
                }

                //field.Type = field.Type.Replace("_", "");
            }

            debugObject.Serialize(Path.Combine(options.OutputDirectory, "Definition.debug.xml"));

            foreach (var fcObject in sourceObjects)
            {
                foreach (var fcField in fcObject.Fields.ToArray())
                {
                    fcField.Type = fcField.Type.Replace("_", "");
                    fcField.Text = null;

                    if (fcField.Type == binHexType)
                        fcObject.Fields.Remove(fcField);
                }
            }

            sourceObject.Serialize(Path.Combine(options.OutputDirectory, "Definition.xml"));
        }
    }

    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input XML file.")]
        public string InputXmlFile { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output directory.")]
        public string OutputDirectory { get; set; }
    }

}

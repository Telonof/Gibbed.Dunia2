using Gibbed.Dunia2.BinaryObjectInfo;
using Gibbed.Dunia2.FileFormats;
using System.Xml.Linq;

namespace Dunia2.MergeBinaryObject
{
    public class BabelDBMerger
    {
        public BabelDBFile Merge(BabelDBFile original, XDocument file)
        {
            foreach (XElement node in file.Root.Elements("add"))
            {
                List<byte[]> data = [];

                foreach (XElement node2 in node.Elements())
                {
                    data.Add(GrabRowData(node2));
                }

                original.Rows.Add(new Row { data = data });
            }

            foreach (XElement node in file.Root.Elements("edit"))
            {
                int index = ValidateIndex(node, original.Rows.Count);
                if (index == -1)
                    continue;

                foreach (XElement node2 in node.Elements())
                {
                    int colIndex = original.GrabColumnIndex(node2.Name.LocalName);
                    if (colIndex < 0)
                    {
                        Console.WriteLine($"No column exists with the name {node2.Name.LocalName}");
                        continue;
                    }

                    original.Rows[index].data[colIndex] = GrabRowData(node2);
                }
            }

            return original;
        }

        private byte[] GrabRowData(XElement node)
        {
            byte[] columnData;

            if (node.Attribute("type") == null)
            {
                columnData = Convert.FromHexString(node.Value);
            }
            else
            {
                FieldType type = (FieldType)Enum.Parse(typeof(FieldType), node.Attribute("type").Value, true);
                columnData = FieldTypeSerializers.Serialize(type, node.Value);
            }

            return columnData;
        }

        private int ValidateIndex(XElement node, int rowCount)
        {
            if (node.Attribute("index") == null)
            {
                Console.WriteLine("No index attribute for edit.");
                return -1;
            }

            string val = node.Attribute("index").Value;

            if (string.IsNullOrWhiteSpace(val))
            {
                Console.WriteLine("Empty index value.");
                return -1;
            }

            if (!int.TryParse(val, out int index))
            {
                Console.WriteLine($"{val} is not a valid number.");
                return -1;
            }

            if (index < 0 || index >= rowCount)
            {
                Console.WriteLine($"Index {index} out of bounds.");
                return -1;
            }

            return index;
        }
    }
}
